$infraDirectory = "$(Get-Location)/infra"
$appDirectory = "$(Get-Location)/src"
$tfVarsFile = "$infraDirectory/environment.tfvars"

Task Run {
    Exec { func start --worker-runtime dotnet-isolated } -workingDirectory "$appDirectory/API"
}

Task DockerRun {
    Exec { docker compose -p "draw-steel" up --build -d } -workingDirectory $appDirectory
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