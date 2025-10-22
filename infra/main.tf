terraform {
  required_version = "~> 1.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.49.0"
    }
    azuread = {
      source  = "hashicorp/azuread"
      version = "3.0.1"
    }
    auth0 = {
      source  = "auth0/auth0"
      version = "1.26.0"
    }
    github = {
      source  = "integrations/github"
      version = "6.3.1"
    }
    random = {
      source  = "hashicorp/random"
      version = "3.6.3"
    }
  }
  backend "azurerm" {}
}

provider "azurerm" {
  features {}
  subscription_id = "c91bcad2-4067-42b8-922a-59fdb7707ad6"
}

provider "azuread" {
  tenant_id = "367128d9-c2d4-4615-98a3-002700e530d3"
}

provider "auth0" {
  domain        = data.azurerm_key_vault_secret.auth0_domain.value
  client_id     = data.azurerm_key_vault_secret.auth0_client_id.value
  client_secret = data.azurerm_key_vault_secret.auth0_client_secret.value
}

provider "github" {
}

resource "azurerm_resource_group" "main" {
  name     = local.common_app_name
  location = "West Europe"
}