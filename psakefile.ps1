$infraDirectory = "$(Get-Location)/infra"
$appDirectory = "$(Get-Location)/src"
$tfVarsFile = "$infraDirectory/environment.tfvars"

Task DockerUp {
    Exec { docker compose up }
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