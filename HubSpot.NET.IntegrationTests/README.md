# Custom Object Integration Tests

There are several integration tests located in the `Api/CustomObject` directory that specifically test the functionality of Custom Object operations in the HubSpot API.

These tests require a live HubSpot account, and they involve creating and manipulating an actual Custom Object schema, specifically for a 'Machine'.

## Testing Scripts
We have provided two PowerShell scripts to effectively manage this schema - `CreateMachineSchema.ps1` and `DeleteMachineSchema.ps1`.

- `CreateMachineSchema.ps1`: This script is for setting up the schema on your HubSpot account before running the tests. It creates a 'Machine' Custom Object schema.
- `DeleteMachineSchema.ps1`: This script is for cleanup purposes after running the tests. It deletes the 'Machine' Custom Object schema.

These scripts ensure that the tests do not manipulate any pre-existing schemas on your HubSpot account.

## How to Run the Tests
Before running the integration tests, provide the HubSpot token in the scripts:
```powershell 
    $hubSpotApiKey = "your_hubspot_api_key_here"
```

Then run the `CreateMachineSchema.ps1` script first.

1. Navigate to the `Scripts` directory of the `HubSpot.NET.IntegrationTests` project.
2. Press `Shift + Right-Click` in the directory and select 'Open PowerShell window here'.
3. Enter the command to create the Custom Object schema on your HubSpot account:
    ```powershell
    .\CreateMachineSchema.ps1
    ```
4. Now, you can remove the 'skip' attribute from the Custom Object integration tests and run the tests.
5. Once you have completed the testing, use the `DeleteMachineSchema.ps1` script in the same way you ran the create script:
    ```powershell
    .\DeleteMachineSchema.ps1
    ```