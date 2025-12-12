resource "azurerm_application_insights" "main" {
  name                = local.common_app_name
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  workspace_id        = azurerm_log_analytics_workspace.main.id
  application_type    = "web"

  # Free tier limits
  daily_data_cap_in_gb                  = 0.1 # 100MB/day to stay within free tier
  daily_data_cap_notifications_disabled = false
  retention_in_days                     = 30
  sampling_percentage                   = 100
}

output "application_insights_connection_string" {
  value     = azurerm_application_insights.main.connection_string
  sensitive = true
}
