resource "azurerm_resource_group" "rg" {
  name     = "GR-${var.PROJECT_NAME}"
  location = var.RESOURCE_GROUP_LOCATION
}

resource "azurerm_storage_account" "storage" {
  name                     = "st${lower(var.PROJECT_NAME)}"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_application_insights" "appinsights" {
  application_type    = var.APP_INSIGHTS_PROJECT_TYPE
  name                = "AppInsights-${var.PROJECT_NAME}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
}
