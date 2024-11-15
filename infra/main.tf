terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.4.0"
    }
    azuread = {
      source = "hashicorp/azuread"
      version = "3.0.1"
    }
    github = {
      source = "integrations/github"
      version = "6.3.1"
    }
  }
  required_version = "1.9.8"
}

provider "azurerm" {
  features {}
  subscription_id = "c91bcad2-4067-42b8-922a-59fdb7707ad6"
}

provider "azuread" {
}

provider "github" {
}

resource "azurerm_resource_group" "main" {
  name     = "draw-steel"
  location = "West Europe"
}