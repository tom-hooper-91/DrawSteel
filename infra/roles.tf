resource "azurerm_role_assignment" "cosmos_db_operator" {
  principal_id         = azurerm_container_app.main.identity[0].principal_id
  role_definition_name = "Cosmos DB Operator"
  scope                = azurerm_cosmosdb_account.main.id
}