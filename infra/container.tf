locals {
  dockerhub_token_secret_name = "registryhubdockercom-thooper91"
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
  target_port = 80
  external_enabled = true
  traffic_weight {
    latest_revision = true
    percentage = 100
  }
}
  template {
    container {
      name   = local.common_app_name
      image  = "thooper91/draw-steel-api:latest"
      cpu    = 0.25
      memory = "0.5Gi"

      env {
        name       = "MONGODB_CONNECTION_STRING"
        secret_name = "mongodb-connection-string"
      }

      env {
        name  = "AzureWebJobsSecretStorageType"
        value = "KeyVault"
      }
      
      env {
        name  = "AzureWebJobsSecretStorageKeyVaultName"
        value = azurerm_key_vault.main.name
      }
      
      env {
        name  = "AZURE_FUNCTIONS_ENVIRONMENT"
        value = "Production"
      }

      env {
        name  = "AZURE_FUNCTION_MANAGED_IDENTITY"
        value = "true"
      }

      env {
        name  = "WEBSITE_LOAD_USER_PROFILE"
        value = "1"
      }
      env {
        name  = "AzureWebJobsStorage__accountName"
        value = azurerm_storage_account.main.name
      }
      env {
        name  = "AZURE_TENANT_ID"
        value = data.azurerm_client_config.current.tenant_id
      }

      env {
        name  = "FUNCTIONS_EXTENSION_VERSION"
        value = "~4"
      }

      env {
        name  = "FUNCTIONS_WORKER_RUNTIME"
        value = "dotnet-isolated"  # Adjust based on your runtime (.NET isolated, node, etc.)
      }

      env {
        name  = "APPLICATIONINSIGHTS_CONNECTION_STRING"
        value = azurerm_application_insights.app.connection_string
      }
    }
  }

  registry {
    password_secret_name = local.dockerhub_token_secret_name
    server = "registry.hub.docker.com"
    username = "thooper91"
  }

  secret {
    name  = local.dockerhub_token_secret_name
    value = data.azurerm_key_vault_secret.dockerhub_token.value
  }

  secret {
    name = "mongodb-connection-string"
    value = azurerm_cosmosdb_account.main.primary_mongodb_connection_string
  }

  lifecycle {
    ignore_changes = [
      template[0].container[0].image
    ]
  }

  identity {
    type = "SystemAssigned"
  }
}

resource "azurerm_application_insights" "app" {
  name                = "${local.common_app_name}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  application_type    = "web"
}