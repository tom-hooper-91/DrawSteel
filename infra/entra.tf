resource "azuread_application_registration" "github_actions" {
  display_name = "github_actions"
  description  = "Application for Github Actions"
}

resource "azuread_service_principal" "github_actions" {
  client_id                    = azuread_application_registration.github_actions.client_id
  app_role_assignment_required = false
  owners                       = [data.azuread_client_config.current.object_id]
}

resource "azuread_application_federated_identity_credential" "github_actions" {
  application_id = azuread_application_registration.github_actions.id
  display_name   = "GitHubActions"
  description    = "Used to authenticate GitHub Actions to Azure AD"
  audiences      = ["api://AzureADTokenExchange"]
  issuer         = "https://token.actions.githubusercontent.com"
  subject        = "repo:tom-hooper-91/DrawSteel:ref:refs/heads/main"
}