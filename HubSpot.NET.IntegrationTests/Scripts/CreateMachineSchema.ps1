# API Docs https://developers.hubspot.com/docs/api/crm/crm-custom-objects
# Make sure your token has the necessary scopes
$hubSpotApiKey = "your_hubspot_api_key_here"

$customObject = @{
  "primaryDisplayProperty"= "model"
  "name" = "machine"
  "labels" = @{
    "singular" = "Machine"
    "plural" = "Machines"
  }
  "properties" = @(
    @{
      "name" = "model"
      "label" = "Model"
      "type" = "string"
      "fieldType" = "text"
    },
    @{
      "name" = "year"
      "label" = "Year"
      "type" = "date"
      "fieldType" = "date"
    },
    @{
      "name" = "km"
      "label" = "Km"
      "type" = "number"
      "fieldType" = "number"
    }
  )
  "associatedObjects" = @("contact")
  "requiredProperties" = @("model")
}

$objectName = $customObject.name
$objectDetails = $customObject | ConvertTo-Json

$endpoint = "https://api.hubapi.com/crm/v3/schemas"

try {
  Write-Output "Object to write'$objectDetails'."
  $response = Invoke-RestMethod -Method Post -Uri $endpoint -Headers @{ Authorization = "Bearer $hubSpotApiKey" } -ContentType "application/json" -Body $objectDetails
  Write-Output "Object '$objectName' created successfully."
}catch {
  $errorMessage = $_.Exception.Response.GetResponseStream()
  $reader = New-Object System.IO.StreamReader($errorMessage)
  $reader.BaseStream.Position = 0
  $reader.DiscardBufferedData()
  $responseBody = $reader.ReadToEnd()
  Write-Output "Error creating object '$objectToDelete': $responseBody"
}