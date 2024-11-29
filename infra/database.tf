resource "azurerm_cosmosdb_account" "main" {
  name                = "drawsteel"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  offer_type          = "Standard"
  kind                = "MongoDB"
  free_tier_enabled = true

  capabilities {
    name = "MongoDBv3.4"
  }

  capabilities {
    name = "EnableMongo"
  }

  consistency_policy {
    consistency_level       = "Strong"
  }

  geo_location {
    location          = azurerm_resource_group.main.location
    failover_priority = 0
    }
}

resource "azurerm_cosmosdb_mongo_database" "main" {
  name                = "drawsteel"
  resource_group_name = azurerm_resource_group.main.name
  account_name        = azurerm_cosmosdb_account.main.name
  throughput          = 400
}