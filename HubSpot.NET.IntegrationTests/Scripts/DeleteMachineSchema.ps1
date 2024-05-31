# Define your HubSpot Private App Key
$hubSpotApiKey = "your_hubspot_api_key_here"

$objectToDelete = "machine"

# First, list all the objects
$listEndpoint = "https://api.hubapi.com/crm/v3/objects/$($objectType)"
try {
    $listResponse = Invoke-RestMethod -Method Get -Uri $listEndpoint -Headers @{ Authorization = "Bearer $hubSpotApiKey" }
}
catch {
    $listErrorMessage = $_.Exception.Response.GetResponseStream()
    $listReader = New-Object System.IO.StreamReader($listErrorMessage)
    $responseBody = $listReader.ReadToEnd()
    Write-Output "Error listing '$objectType' objects: $responseBody"
    return
}

# Then, delete all the objects
foreach($result in $listResponse.results) {
    $objectIdToDelete = $result.properties.hs_object_id
    $deleteEndpoint = "https://api.hubapi.com/crm/v3/objects/$($objectType)/$($objectIdToDelete)"

    try {
        $deleteResponse = Invoke-RestMethod -Method Delete -Uri $deleteEndpoint -Headers @{ Authorization = "Bearer $hubSpotApiKey" }
        Write-Output "Object '$objectIdToDelete' deleted successfully."
    }
    catch {
        $errorMessage = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($errorMessage)
        $responseBody = $reader.ReadToEnd()
        Write-Output "Error deleting object '$objectIdToDelete': $responseBody"
    }
}

# Finally, delete the custom object schema
$deleteSchemaEndpoint = "https://api.hubapi.com/crm/v3/schemas/$($objectType)"
try {
    $deleteResponse = Invoke-RestMethod -Method Delete -Uri $deleteSchemaEndpoint -Headers @{ Authorization = "Bearer $hubSpotApiKey" }
    Write-Output "Schema '$objectType' deleted successfully."
}
catch {
    $errorMessage = $_.Exception.Response.GetResponseStream()
    $reader = New-Object System.IO.StreamReader($errorMessage)
    $responseBody = $reader.ReadToEnd()
    Write-Output "Error deleting schema '$objectType': $responseBody"
}