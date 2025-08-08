data "azurerm_subscription" "current" {
}

data "azurerm_client_config" "current" {
}

data "azurerm_key_vault" "draw_keys" {
  name                = "draw-keys"
  resource_group_name = "draw-steel-state"
}

data "azurerm_key_vault_secret" "dockerhub_token" {
  name         = "dockerhub-token"
  key_vault_id = data.azurerm_key_vault.draw_keys.id
}

data "azurerm_key_vault_secret" "auth0_domain" {
  name         = "auth0-domain"
  key_vault_id = data.azurerm_key_vault.draw_keys.id
}

data "azurerm_key_vault_secret" "auth0_client_id" {
  name         = "auth0-client-id"
  key_vault_id = data.azurerm_key_vault.draw_keys.id
}

data "azurerm_key_vault_secret" "auth0_client_secret" {
  name         = "auth0-client-secret"
  key_vault_id = data.azurerm_key_vault.draw_keys.id
}
