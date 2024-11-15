resource "azuread_application_registration" "github_actions" {
  display_name = "github_actions"
  description  = "Application for Github Actions"
}