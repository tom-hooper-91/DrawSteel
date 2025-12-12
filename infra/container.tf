locals {
  dockerhub_token_secret_name            = "registryhubdockercom-thooper91"
  mongo_db_connection_string_secret_name = "mongodb-connection-string"
  appinsights_connection_string_name     = "applicationinsights-connection-string"
}

resource "azurerm_log_analytics_workspace" "main" {
  name                = local.common_app_name
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

resource "azurerm_container_app_environment" "main" {
  name                       = local.common_app_name
  location                   = azurerm_resource_group.main.location
  resource_group_name        = azurerm_resource_group.main.name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.main.id
}

resource "azurerm_container_app" "main" {
  name                         = local.common_app_name
  container_app_environment_id = azurerm_container_app_environment.main.id
  resource_group_name          = azurerm_resource_group.main.name
  revision_mode                = "Single"

  ingress {
    target_port      = 8080
    external_enabled = true
    traffic_weight {
      latest_revision = true
      percentage      = 100
    }
  }
  template {
    container {
      name   = local.common_app_name
      image  = "thooper91/draw-steel-api:latest"
      cpu    = 0.25
      memory = "0.5Gi"

      env {
        name        = "MONGODB_CONNECTION_STRING"
        secret_name = local.mongo_db_connection_string_secret_name
      }

      env {
        name        = "APPLICATIONINSIGHTS_CONNECTION_STRING"
        secret_name = local.appinsights_connection_string_name
      }

      env {
        name  = "OTEL_SERVICE_NAME"
        value = "DrawSteel.API"
      }

      # Startup probe - gives container time to start before other probes kick in
      startup_probe {
        transport = "HTTP"
        path      = "/health/live"
        port      = 8080
        # Allows up to 30 * 10 = 300 seconds (5 minutes) for the container to start
        failure_count_threshold = 30
        interval_seconds        = 10
        timeout                 = 5
      }

      # Liveness probe - checks if the container is still running
      liveness_probe {
        transport               = "HTTP"
        path                    = "/health/live"
        port                    = 8080
        failure_count_threshold = 3
        interval_seconds        = 30
        timeout                 = 5
      }

      # Readiness probe - checks if the container can accept traffic
      readiness_probe {
        transport               = "HTTP"
        path                    = "/health/ready"
        port                    = 8080
        failure_count_threshold = 3
        success_count_threshold = 1
        interval_seconds        = 10
        timeout                 = 5
      }
    }
  }

  registry {
    password_secret_name = local.dockerhub_token_secret_name
    server               = "registry.hub.docker.com"
    username             = "thooper91"
  }

  secret {
    name  = local.dockerhub_token_secret_name
    value = data.azurerm_key_vault_secret.dockerhub_token.value
  }

  secret {
    name  = local.mongo_db_connection_string_secret_name
    value = azurerm_cosmosdb_account.main.primary_mongodb_connection_string
  }

  secret {
    name  = local.appinsights_connection_string_name
    value = azurerm_application_insights.main.connection_string
  }

  lifecycle {
    ignore_changes = [
      template[0].container[0].image
    ]
  }
}