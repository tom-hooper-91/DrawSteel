data "azurerm_subscription" "current" {
}

data "azurerm_client_config" "current" {   
}

data "azuread_client_config" "current" {
}

data "azurerm_key_vault_secret" "dockerhub_token" {
  name         = "dockerhub-token"
  key_vault_id = azurerm_key_vault.main.id
}

data "azurerm_key_vault_secret" "dockerhub_username" {
  name         = "dockerhub-username"
  key_vault_id = azurerm_key_vault.main.id
}
