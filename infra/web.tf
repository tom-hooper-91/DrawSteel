resource "azurerm_service_plan" "main" {
  name                = "draw-steel"
  resource_group_name = azurerm_resource_group.draw_steel.name
  location            = azurerm_resource_group.draw_steel.location
  os_type             = "Linux"
  sku_name            = "F1"
}

resource "azurerm_linux_web_app" "main" {
  name                = "draw-steel"
  resource_group_name = azurerm_resource_group.draw_steel.name
  location            = azurerm_service_plan.main.location
  service_plan_id     = azurerm_service_plan.main.id

  site_config {
    always_on = false
  }
}