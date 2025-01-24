resource "azurerm_service_plan" "main" {
  name                = local.common_app_name
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  os_type             = "Linux"
  sku_name            = "B1"
}

resource "azurerm_linux_function_app" "main" {
  name                = "${local.common_app_name}-api"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location

  storage_account_name       = azurerm_storage_account.main.name
  storage_account_access_key = azurerm_storage_account.main.primary_access_key
  service_plan_id            = azurerm_service_plan.main.id

  site_config {
    application_stack {
      docker {
        registry_url = "registry.hub.docker.com"
        image_name = "thooper91/draw-steel-api"
        image_tag = "latest"
        registry_username = data.azurerm_key_vault_secret.dockerhub_username.value
        registry_password = data.azurerm_key_vault_secret.dockerhub_token.value
      }
    }
  }
}