# cosmos-rbac-dotnet-sample

This sample code is using .NET SDK and it using Azure Function App connect to Cosmos DB via User Assigned Managed Idenity. For setting up permission for Data Plane using UAMI at Cosmos DB, please refer to this document.

Prerequisites:
    1. assign your application with proper RBAC permission, please check doc (https://learn.microsoft.com/en-us/azure/cosmos-db/how-to-setup-rbac)
    2. or use the following Azure CLI sample to grant built-in role permission (Data Plane)to your application:
    $subscriptionid = "your_subscription_id"
    $resourceGroupName = "your_resource_group_name"
    $accountName = "your_cosmosdb_account_name"
    az account set --subscription $subscriptionid
 
    $buildInRoleId = "00000000-0000-0000-0000-000000000002" #Cosmos DB Built-in Data Contributor
    $principalId = "your_application_object_id" #AAD Application ObjectId, not Application Id
    az cosmosdb sql role assignment create --resource-group $resourceGroupName --account-name $accountName --scope "/" --principal-id $principalId --role-definition-id $buildInRoleId

    Cosmos DB Built-in Data Reader: 00000000-0000-0000-0000-000000000001
    Microsoft.DocumentDB/databaseAccounts/readMetadata
    Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/read
    Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/executeQuery
    Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/readChangeFeed
 
    Cosmos DB Built-in Data Contributor: 00000000-0000-0000-0000-000000000002
    Microsoft.DocumentDB/databaseAccounts/readMetadata
    Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/*
    Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/*
