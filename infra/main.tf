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
  subscription_id = "937c854e-4afb-4727-82ec-1bc86b6ed1d9"
}

resource "azurerm_resource_group" "main" {
  name     = "draw-steel"
  location = "West Europe"
}
