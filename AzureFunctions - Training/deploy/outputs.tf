output "ApplicationInsightsInstrumentationKey" {
  value     = azurerm_application_insights.appinsights.instrumentation_key
  sensitive = true
}

output "StorageConnectionString" {
  value     = azurerm_storage_account.storage.primary_connection_string
  sensitive = true
}

output "AppServiceFunctionName" {
  value     = azurerm_function_app.functionApp.name
  sensitive = true
}

output "AppServiceWebName" {
  value     = azurerm_app_service.asWeb.name
  sensitive = true
}

output "ResourceGroupName" {
  value = azurerm_resource_group.rg.name
  sensitive = true
}