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

resource "azurerm_app_service_plan" "sp" {
  name                = "SP-${var.PROJECT_NAME}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  
  sku {
    tier = "Standard"
    size = "S1"
  }
}

resource "azurerm_function_app" "functionApp" {
  name                       = "${var.PROJECT_NAME}FuncDEV"
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  app_service_plan_id        = azurerm_app_service_plan.sp.id
  storage_account_name       = azurerm_storage_account.storage.name
  storage_account_access_key = azurerm_storage_account.storage.primary_access_key
  version                    = "~3"
  
  site_config {
    cors {
      allowed_origins = ["*"]
    }
  }
  app_settings = {
    "APPINSIGHTS_INSTRUMENTATIONKEY" = azurerm_application_insights.appinsights.instrumentation_key
    "SendGridKey"= "${var.SENDGRID_KEY}",
    "ApproverEmail"= "${var.APPROVER_MAIL}",
    "SenderEmail"= "${var.SENDER_MAIL}",
    "Host"= "https://${var.PROJECT_NAME}WebDev.azurewebsites.net",
    "AzureWebJobsSecretStorageType": "files",
    "WEBSITE_RUN_FROM_PACKAGE": true,
    "FUNCTIONS_WORKER_RUNTIME": "dotnet"
  }
}

resource "azurerm_app_service" "asWeb" {
  name                = "${var.PROJECT_NAME}WebDev"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  app_service_plan_id = azurerm_app_service_plan.sp.id
}