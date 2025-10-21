# Comaiz API PowerShell Examples

# Import the module
Import-Module -Name .\powershell\ComaizApi.psm1 -Force

# Set your API base URL
$baseUrl = "https://localhost:7057"

# Example 1: Login and get a token
Write-Host "`n=== Example 1: Login ===" -ForegroundColor Cyan
$token = Get-ComaizToken -BaseUrl $baseUrl -Username "admin" -Password "Admin@123"
Write-Host "Token obtained: $($token.Substring(0, 50))..." -ForegroundColor Green

# Example 2: Get all clients
Write-Host "`n=== Example 2: Get All Clients ===" -ForegroundColor Cyan
$clients = Get-Items -BaseUrl $baseUrl -Token $token -Collection Clients
Write-Host "Found $($clients.Count) clients:" -ForegroundColor Green
$clients | Format-Table Id, ShortName, Name

# Example 3: Get a specific client by ID
Write-Host "`n=== Example 3: Get Client by ID ===" -ForegroundColor Cyan
if ($clients.Count -gt 0) {
    $firstClientId = $clients[0].Id
    $client = Get-ItemById -BaseUrl $baseUrl -Token $token -Collection Clients -ItemId $firstClientId
    Write-Host "Client details:" -ForegroundColor Green
    $client | Format-List
}

# Example 4: Add a new client
Write-Host "`n=== Example 4: Add New Client ===" -ForegroundColor Cyan
$newClient = @{
    ShortName = "PS"
    Name = "PowerShell Test Client"
}
$createdClient = Add-Item -BaseUrl $baseUrl -Token $token -Collection Clients -Item $newClient
Write-Host "Created client with ID: $($createdClient.Id)" -ForegroundColor Green
$createdClient | Format-List

# Example 5: Update a client
Write-Host "`n=== Example 5: Update Client ===" -ForegroundColor Cyan
$createdClient.Name = "PowerShell Updated Client"
Update-Item -BaseUrl $baseUrl -Token $token -Collection Clients -Item $createdClient
Write-Host "Client updated successfully!" -ForegroundColor Green

# Verify the update
$updatedClient = Get-ItemById -BaseUrl $baseUrl -Token $token -Collection Clients -ItemId $createdClient.Id
$updatedClient | Format-List

# Example 6: Get all workers
Write-Host "`n=== Example 6: Get All Workers ===" -ForegroundColor Cyan
$workers = Get-Items -BaseUrl $baseUrl -Token $token -Collection Workers
Write-Host "Found $($workers.Count) workers:" -ForegroundColor Green
$workers | Format-Table Id, Name

# Example 7: Add a new worker
Write-Host "`n=== Example 7: Add New Worker ===" -ForegroundColor Cyan
$newWorker = @{
    Name = "PowerShell Test Worker"
}
$createdWorker = Add-Item -BaseUrl $baseUrl -Token $token -Collection Workers -Item $newWorker
Write-Host "Created worker with ID: $($createdWorker.Id)" -ForegroundColor Green

# Example 8: Delete the test client (cleanup)
Write-Host "`n=== Example 8: Delete Test Client ===" -ForegroundColor Cyan
Remove-Item -BaseUrl $baseUrl -Token $token -Collection Clients -ItemId $createdClient.Id
Write-Host "Test client deleted successfully!" -ForegroundColor Green

# Example 9: Delete the test worker (cleanup)
Write-Host "`n=== Example 9: Delete Test Worker ===" -ForegroundColor Cyan
Remove-Item -BaseUrl $baseUrl -Token $token -Collection Workers -ItemId $createdWorker.Id
Write-Host "Test worker deleted successfully!" -ForegroundColor Green

Write-Host "`n=== All Examples Completed ===" -ForegroundColor Cyan

<#
.NOTES
Running this script:
1. Ensure the API is running (dotnet run in comaiz.api folder)
2. Run this script: .\powershell\Examples.ps1
3. The script will demonstrate all CRUD operations with authentication

Error Handling:
- If you get a 401 error, your token may have expired (default: 60 minutes)
- If you get SSL/TLS errors, you may need to trust the development certificate:
  dotnet dev-certs https --trust
#>
