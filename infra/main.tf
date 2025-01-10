terraform {
  required_version = "~> 1.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.14.0"
    }
    azuread = {
      source = "hashicorp/azuread"
      version = "3.0.1"
    }
    github = {
      source = "integrations/github"
      version = "6.3.1"
    }
    random = {
      source = "hashicorp/random"
      version = "3.6.3"
    }
  }
  backend "azurerm" {}
}

provider "azurerm" {
  features {}
  subscription_id = var.azure_subscription_id
}

provider "azuread" {
  tenant_id = var.azure_tenant_id
}

provider "github" {
}

resource "azurerm_resource_group" "main" {
  name     = "draw-steel"
  location = "West Europe"
}