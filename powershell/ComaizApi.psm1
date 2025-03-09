# ComaizApi.psm1

enum Collection {
    Clients
    Contracts
    ContractRates
    FixedCosts
    Workers
    WorkRecords
    Invoices
    InvoiceItems
}

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

function Get-Items {
    param (
        [string]$BaseUrl,
        [string]$IdToken,
        [Collection]$Collection
    )

    $headers = @{
        Authorization = "Bearer $IdToken"
    }

    $response = Invoke-RestMethod -Uri "$BaseUrl/api/$Collection" -Method Get -Headers $headers
    return $response
}

function Get-ItemById {
    param (
        [string]$BaseUrl,
        [string]$IdToken,
        [Collection]$Collection,
        [int]$ItemId
    )

    $headers = @{
        Authorization = "Bearer $IdToken"
    }

    $response = Invoke-RestMethod -Uri "$BaseUrl/api/$Collection/$ItemId" -Method Get -Headers $headers
    return $response
}

function Add-Item {
    param (
        [string]$BaseUrl,
        [string]$IdToken,
        [Collection]$Collection,
        [object]$Item
    )

    $headers = @{
        Authorization = "Bearer $IdToken"
        "Content-Type" = "application/json"
    }

    $response = Invoke-RestMethod -Uri "$BaseUrl/api/$Collection" -Method Post -Headers $headers -Body ($Item | ConvertTo-Json)
    return $response
}

function Update-Item {
    param (
        [string]$BaseUrl,
        [string]$IdToken,
        [Collection]$Collection,
        [object]$Item
    )

    $headers = @{
        Authorization = "Bearer $IdToken"
        "Content-Type" = "application/json"
    }

    $response = Invoke-RestMethod -Uri "$BaseUrl/api/$Collection" -Method Put -Headers $headers -Body ($Item | ConvertTo-Json)
    return $response
}

function Remove-Item {
    param (
        [string]$BaseUrl,
        [string]$IdToken,
        [Collection]$Collection,
        [int]$ItemId
    )

    $headers = @{
        Authorization = "Bearer $IdToken"
    }

    $response = Invoke-RestMethod -Uri "$BaseUrl/api/$Collection/$ItemId" -Method Delete -Headers $headers
    return $response
}

Export-ModuleMember -Function *-Item*, Get-IdToken
