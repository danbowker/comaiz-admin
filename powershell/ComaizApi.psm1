# ComaizApi.psm1

function Get-IdToken {
    param (
        [string]$ClientId,
        [string]$ClientSecret,
        [string]$RefreshToken
    )

    $body = @{
        client_id = $ClientId
        client_secret = $ClientSecret
        refresh_token = $RefreshToken
        grant_type = "refresh_token"
    }

    $response = Invoke-RestMethod -Uri "https://oauth2.googleapis.com/token" -Method Post -ContentType "application/x-www-form-urlencoded" -Body $body
    return $response.id_token
}

function Get-Clients {
    param (
        [string]$BaseUrl,
        [string]$IdToken
    )

    $headers = @{
        Authorization = "Bearer $IdToken"
    }

    $response = Invoke-RestMethod -Uri "$BaseUrl/api/Clients" -Method Get -Headers $headers
    return $response
}

function Get-ClientById {
    param (
        [string]$BaseUrl,
        [string]$IdToken,
        [int]$ClientId
    )

    $headers = @{
        Authorization = "Bearer $IdToken"
    }

    $response = Invoke-RestMethod -Uri "$BaseUrl/api/Clients/$ClientId" -Method Get -Headers $headers
    return $response
}

function Add-Client {
    param (
        [string]$BaseUrl,
        [string]$IdToken,
        [object]$Client
    )

    $headers = @{
        Authorization = "Bearer $IdToken"
        "Content-Type" = "application/json"
    }

    $response = Invoke-RestMethod -Uri "$BaseUrl/api/Clients" -Method Post -Headers $headers -Body ($Client | ConvertTo-Json)
    return $response
}

function Update-Client {
    param (
        [string]$BaseUrl,
        [string]$IdToken,
        [int]$ClientId,
        [object]$Client
    )

    $headers = @{
        Authorization = "Bearer $IdToken"
        "Content-Type" = "application/json"
    }

    $response = Invoke-RestMethod -Uri "$BaseUrl/api/Clients/$ClientId" -Method Put -Headers $headers -Body ($Client | ConvertTo-Json)
    return $response
}

function Remove-Client {
    param (
        [string]$BaseUrl,
        [string]$IdToken,
        [int]$ClientId
    )

    $headers = @{
        Authorization = "Bearer $IdToken"
    }

    $response = Invoke-RestMethod -Uri "$BaseUrl/api/Clients/$ClientId" -Method Delete -Headers $headers
    return $response
}

Export-ModuleMember -Function *-Client*, Get-IdToken
