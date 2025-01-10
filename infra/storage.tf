resource "random_id" "storage_account" {
  byte_length = 6
}

resource "azurerm_storage_account" "main" {
  name                     = "drawsteel${random_id.storage_account.hex}"
  resource_group_name      = azurerm_resource_group.main.name
  location                 = azurerm_resource_group.main.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_storage_container" "terraform_state" {
  name                  = "tfstate"
  storage_account_id    = azurerm_storage_account.main.id
  container_access_type = "private"
}