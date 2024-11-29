$infraDirectory = "$(Get-Location)/infra"
$tfVarsFile = "$infraDirectory/environment.tfvars"

Task Run {
    Exec { docker compose -f .\docker-compose.yml up --build --watch }
}

Task RunApi {
    Exec { docker compose -f .\docker-compose.api.yml up --build --watch }
}

Task TerraformInit {
    Exec { terraform init -upgrade -reconfigure } -workingDirectory $infraDirectory
}

Task TerraformValidate -depends TerraformInit {
    Exec { terraform validate } -workingDirectory $infraDirectory
}

Task PlanInfra -depends TerraformValidate {
    Exec { terraform plan -var-file="$tfVarsFile" } -workingDirectory $infraDirectory
}

Task ApplyInfra -depends TerraformValidate {
    Exec { terraform apply -var-file="$tfVarsFile" } -workingDirectory $infraDirectory
}

Task DestroyInfra {
    Exec { terraform destroy -var-file="$tfVarsFile" } -workingDirectory $infraDirectory
}