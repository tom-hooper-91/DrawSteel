resource "azuread_application_registration" "github_actions" {
  display_name = "github_actions"
  description  = "Application for Github Actions"
}

resource "azuread_service_principal" "github_actions" {
  client_id                    = azuread_application_registration.github_actions.client_id
  app_role_assignment_required = false
  owners                       = [data.azuread_client_config.current.object_id]
}