terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.4.0"
    }
  }

  required_version = "1.9.7"
}

provider "azurerm" {
  features {}
  subscription_id = "c91bcad2-4067-42b8-922a-59fdb7707ad6"
}

resource "azurerm_resource_group" "draw_steel" {
  name     = "draw-steel"
  location = "West Europe"
}
