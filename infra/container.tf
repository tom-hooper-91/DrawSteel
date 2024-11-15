resource "azurerm_log_analytics_workspace" "main" {
  name                = "draw-steel"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

resource "azurerm_container_app_environment" "main" {
  name                       = "Draw-Steel"
  location                   = azurerm_resource_group.main.location
  resource_group_name        = azurerm_resource_group.main.name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.main.id
}

resource "azurerm_container_app" "main" {
  name                         = "draw-steel"
  container_app_environment_id = azurerm_container_app_environment.main.id
  resource_group_name          = azurerm_resource_group.main.name
  revision_mode                = "Single"

ingress {
  target_port = 8080
  external_enabled = true
  traffic_weight {
    latest_revision = true
    percentage = 100
  }
}
  template {
    container {
      name   = "draw-steel"
      image  = "thooper91/draw-steel:latest"
      cpu    = 0.25
      memory = "0.5Gi"
    }
  }

  identity {
    type = "UserAssigned"
    identity_ids = [
        azurerm_user_assigned_identity.draw_steel.id
    ]
  }
}

resource "azurerm_user_assigned_identity" "draw_steel" {
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  name                = "draw-steel"
}