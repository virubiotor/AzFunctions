terraform {
  required_version = ">= 1.0.9"
  required_providers {
    azurerm = {
      version = "~> 2.88.0"
      source  = "hashicorp/azurerm"
    }
  }
  backend "azurerm" {
    resource_group_name = "RGNAME"    
    storage_account_name = "STRACC"    
    container_name = "CONTAINER"    
    key = "STATEFILE"   
  }
}

provider "azurerm" {
  subscription_id = var.AZURE_SUBSCRIPTION_ID
  tenant_id       = var.AZURE_TENANT_ID
  features {}
}