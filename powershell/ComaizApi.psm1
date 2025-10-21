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

function Get-ComaizToken {
    <#
    .SYNOPSIS
    Authenticates with the Comaiz API and returns a JWT token.
    
    .DESCRIPTION
    Authenticates with the Comaiz API using username and password, and returns a JWT bearer token
    that can be used for subsequent API calls.
    
    .PARAMETER BaseUrl
    The base URL of the Comaiz API (e.g., https://localhost:7057)
    
    .PARAMETER Username
    The username for authentication
    
    .PARAMETER Password
    The password for authentication
    
    .EXAMPLE
    $token = Get-ComaizToken -BaseUrl "https://localhost:7057" -Username "admin" -Password "Admin@123"
    #>
    param (
        [Parameter(Mandatory=$true)]
        [string]$BaseUrl,
        
        [Parameter(Mandatory=$true)]
        [string]$Username,
        
        [Parameter(Mandatory=$true)]
        [string]$Password
    )

    $loginRequest = @{
        username = $Username
        password = $Password
    } | ConvertTo-Json

    try {
        $response = Invoke-RestMethod -Uri "$BaseUrl/api/auth/login" `
            -Method Post `
            -ContentType "application/json" `
            -Body $loginRequest
        
        return $response.token
    }
    catch {
        Write-Error "Authentication failed: $_"
        throw
    }
}

function Get-IdToken {
    <#
    .SYNOPSIS
    Legacy function - Gets an ID token from Google OAuth2.
    
    .DESCRIPTION
    This function is maintained for backward compatibility but is not used with the new JWT authentication.
    Use Get-ComaizToken instead for Comaiz API authentication.
    #>
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
    <#
    .SYNOPSIS
    Gets all items from a collection.
    
    .PARAMETER BaseUrl
    The base URL of the Comaiz API
    
    .PARAMETER Token
    The JWT bearer token (get from Get-ComaizToken)
    
    .PARAMETER Collection
    The collection to query (Clients, Contracts, etc.)
    
    .EXAMPLE
    $token = Get-ComaizToken -BaseUrl "https://localhost:7057" -Username "admin" -Password "Admin@123"
    $clients = Get-Items -BaseUrl "https://localhost:7057" -Token $token -Collection Clients
    #>
    param (
        [Parameter(Mandatory=$true)]
        [string]$BaseUrl,
        
        [Parameter(Mandatory=$true)]
        [string]$Token,
        
        [Parameter(Mandatory=$true)]
        [Collection]$Collection
    )

    $headers = @{
        Authorization = "Bearer $Token"
    }

    $response = Invoke-RestMethod -Uri "$BaseUrl/api/$Collection" -Method Get -Headers $headers
    return $response
}

function Get-ItemById {
    <#
    .SYNOPSIS
    Gets a specific item by ID from a collection.
    
    .PARAMETER BaseUrl
    The base URL of the Comaiz API
    
    .PARAMETER Token
    The JWT bearer token
    
    .PARAMETER Collection
    The collection to query
    
    .PARAMETER ItemId
    The ID of the item to retrieve
    
    .EXAMPLE
    $client = Get-ItemById -BaseUrl "https://localhost:7057" -Token $token -Collection Clients -ItemId 1
    #>
    param (
        [Parameter(Mandatory=$true)]
        [string]$BaseUrl,
        
        [Parameter(Mandatory=$true)]
        [string]$Token,
        
        [Parameter(Mandatory=$true)]
        [Collection]$Collection,
        
        [Parameter(Mandatory=$true)]
        [int]$ItemId
    )

    $headers = @{
        Authorization = "Bearer $Token"
    }

    $response = Invoke-RestMethod -Uri "$BaseUrl/api/$Collection/$ItemId" -Method Get -Headers $headers
    return $response
}

function Add-Item {
    <#
    .SYNOPSIS
    Adds a new item to a collection.
    
    .PARAMETER BaseUrl
    The base URL of the Comaiz API
    
    .PARAMETER Token
    The JWT bearer token
    
    .PARAMETER Collection
    The collection to add to
    
    .PARAMETER Item
    The item object to add
    
    .EXAMPLE
    $newClient = @{ ShortName = "ACME"; Name = "Acme Corp" }
    $client = Add-Item -BaseUrl "https://localhost:7057" -Token $token -Collection Clients -Item $newClient
    #>
    param (
        [Parameter(Mandatory=$true)]
        [string]$BaseUrl,
        
        [Parameter(Mandatory=$true)]
        [string]$Token,
        
        [Parameter(Mandatory=$true)]
        [Collection]$Collection,
        
        [Parameter(Mandatory=$true)]
        [object]$Item
    )

    $headers = @{
        Authorization = "Bearer $Token"
        "Content-Type" = "application/json"
    }

    $response = Invoke-RestMethod -Uri "$BaseUrl/api/$Collection" -Method Post -Headers $headers -Body ($Item | ConvertTo-Json)
    return $response
}

function Update-Item {
    <#
    .SYNOPSIS
    Updates an existing item in a collection.
    
    .PARAMETER BaseUrl
    The base URL of the Comaiz API
    
    .PARAMETER Token
    The JWT bearer token
    
    .PARAMETER Collection
    The collection containing the item
    
    .PARAMETER Item
    The updated item object (must include Id)
    
    .EXAMPLE
    $client.Name = "Updated Name"
    Update-Item -BaseUrl "https://localhost:7057" -Token $token -Collection Clients -Item $client
    #>
    param (
        [Parameter(Mandatory=$true)]
        [string]$BaseUrl,
        
        [Parameter(Mandatory=$true)]
        [string]$Token,
        
        [Parameter(Mandatory=$true)]
        [Collection]$Collection,
        
        [Parameter(Mandatory=$true)]
        [object]$Item
    )

    $headers = @{
        Authorization = "Bearer $Token"
        "Content-Type" = "application/json"
    }

    $response = Invoke-RestMethod -Uri "$BaseUrl/api/$Collection" -Method Put -Headers $headers -Body ($Item | ConvertTo-Json)
    return $response
}

function Remove-Item {
    <#
    .SYNOPSIS
    Removes an item from a collection.
    
    .PARAMETER BaseUrl
    The base URL of the Comaiz API
    
    .PARAMETER Token
    The JWT bearer token
    
    .PARAMETER Collection
    The collection containing the item
    
    .PARAMETER ItemId
    The ID of the item to remove
    
    .EXAMPLE
    Remove-Item -BaseUrl "https://localhost:7057" -Token $token -Collection Clients -ItemId 1
    #>
    param (
        [Parameter(Mandatory=$true)]
        [string]$BaseUrl,
        
        [Parameter(Mandatory=$true)]
        [string]$Token,
        
        [Parameter(Mandatory=$true)]
        [Collection]$Collection,
        
        [Parameter(Mandatory=$true)]
        [int]$ItemId
    )

    $headers = @{
        Authorization = "Bearer $Token"
    }

    $response = Invoke-RestMethod -Uri "$BaseUrl/api/$Collection/$ItemId" -Method Delete -Headers $headers
    return $response
}

Export-ModuleMember -Function Get-ComaizToken, Get-Items, Get-ItemById, Add-Item, Update-Item, Remove-Item, Get-IdToken

