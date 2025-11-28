resource "azurerm_cosmosdb_account" "main" {
  name                 = local.common_app_name
  location             = azurerm_resource_group.main.location
  resource_group_name  = azurerm_resource_group.main.name
  offer_type           = "Standard"
  kind                 = "MongoDB"
  free_tier_enabled    = true
  mongo_server_version = "4.2"

  #   capabilities {
  #     name = "EnableAggregationPipeline"
  #   }

  #   capabilities {
  #     name = "mongoEnableDocLevelTTL"
  #   }

  capabilities {
    name = "EnableMongo"
  }

  consistency_policy {
    consistency_level = "Session"
  }

  geo_location {
    location          = "westus"
    failover_priority = 0
  }
}

resource "azurerm_cosmosdb_mongo_database" "main" {
  name                = local.common_app_name
  resource_group_name = azurerm_cosmosdb_account.main.resource_group_name
  account_name        = azurerm_cosmosdb_account.main.name
}
