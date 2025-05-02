locals {
    repo_name = "DrawSteel"
}

resource "github_actions_secret" "dockerhub_username" {
  repository       = "DrawSteel"
  secret_name      = "DOCKERHUB_USERNAME"
  plaintext_value  = data.azurerm_key_vault_secret.dockerhub_username.value
}

resource "github_actions_secret" "dockerhub_token" {
  repository       = local.repo_name
  secret_name      = "DOCKERHUB_TOKEN"
  plaintext_value = data.azurerm_key_vault_secret.dockerhub_token.value
}

resource "github_actions_secret" "azure_client_id" {
  repository       = local.repo_name
  secret_name      = "AZURE_CLIENT_ID"
  plaintext_value  = azuread_service_principal.github_actions.client_id
}

resource "github_actions_secret" "azure_subscription_id" {
  repository       = local.repo_name
  secret_name      = "AZURE_SUBSCRIPTION_ID"
  plaintext_value  = data.azurerm_subscription.current.subscription_id
}

resource "github_actions_secret" "azure_tenant_id" {
  repository       = local.repo_name
  secret_name      = "AZURE_TENANT_ID"
  plaintext_value  = azuread_service_principal.github_actions.application_tenant_id
}

resource "github_actions_variable" "container_app_name" {
    repository       = local.repo_name
    variable_name    = "CONTAINER_APP_NAME"
    value            = azurerm_container_app.main.name
}

resource "github_actions_variable" "container_env_name" {
    repository       = local.repo_name
    variable_name    = "CONTAINER_ENV_NAME"
    value            = azurerm_container_app_environment.main.name
}

resource "github_actions_variable" "resource_group_name" {
    repository       = local.repo_name
    variable_name    = "RESOURCE_GROUP_NAME"
    value            = azurerm_resource_group.main.name
}

resource "azurerm_role_assignment" "github_actions_contributor" {
  principal_id   = azuread_service_principal.github_actions.object_id
  role_definition_name = "Contributor"
  scope          = azurerm_resource_group.main.id
  principal_type = "ServicePrincipal"
}