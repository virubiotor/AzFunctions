variable "AZURE_SUBSCRIPTION_ID" {
  description = "The Azure subscription ID."
}

variable "AZURE_TENANT_ID" {
  description = "The Azure Tenant ID."
}

variable "PROJECT_NAME" {
  description = "The project group name."
}

variable "RESOURCE_GROUP_LOCATION" {
    description = "The resource group location."
}

variable "APP_INSIGHTS_PROJECT_TYPE" {
  description = "Application Insights project type."
}

variable "APPROVER_MAIL" {
  description = "Mail used for the approver of orders"
}

variable "SENDER_MAIL" {
  description = "Mail used for the sender of new orders"
}

variable "SENDGRID_KEY" {
  description = "SendGrid Key"
}