resource "github_actions_secret" "dockerhub_username" {
  repository       = "DrawSteel"
  secret_name      = "DOCKERHUB_USERNAME"
  plaintext_value  = "THooper91"
}

resource "github_actions_secret" "dockerhub_token" {
  repository       = "DrawSteel"
  secret_name      = "DOCKERHUB_TOKEN"
  plaintext_value = var.dockerhub_token
}