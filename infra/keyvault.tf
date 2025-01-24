resource "random_id" "key_vault" {
  byte_length = 6
}

resource "azurerm_key_vault" "main" {
  name                        = "drawsteel${random_id.key_vault.hex}"
  location                    = azurerm_resource_group.main.location
  resource_group_name         = azurerm_resource_group.main.name
  enabled_for_disk_encryption = true
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  soft_delete_retention_days  = 7
  purge_protection_enabled    = false

  sku_name = "standard"
}

resource "azurerm_key_vault_access_policy" "client" {
  key_vault_id = azurerm_key_vault.main.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = data.azurerm_client_config.current.object_id

  secret_permissions = [
        "Backup",
        "Delete",
        "Get",
        "List",
        "Purge",
        "Recover",
        "Restore",
        "Set"
  ]
}

resource "azurerm_key_vault_access_policy" "container_app" {
  key_vault_id = azurerm_key_vault.main.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id = azurerm_container_app.main.identity[0].principal_id

  secret_permissions = [
        "Get",
        "List"
  ]
}