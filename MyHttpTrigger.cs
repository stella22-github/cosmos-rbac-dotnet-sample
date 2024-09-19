using System;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cosmos_RBAC_net_st
{
    public class MyHttpTrigger
    {
        private readonly ILogger<MyHttpTrigger> _logger;

        // Replace these with your actual Cosmos DB endpoint and database details
        private static readonly string cosmosEndpoint = "https://xxxx.documents.azure.com:443/";
        private static readonly string databaseId = "testDB";
        private static readonly string containerId = "test_container";
        private static readonly string userManagedAssignedClientId = "xxxxx"; // User Managed Identity Client ID not the Object ID

        public MyHttpTrigger(ILogger<MyHttpTrigger> logger)
        {
            _logger = logger;
        }

        // Modify CosmosConnectionRBAC to return container properties as a string
        public async Task<string> CosmosConnectionRBAC()
        {
            string result = string.Empty;

            try
            {
                // Create ManagedIdentityCredential with User Assigned Managed Identity client ID
                var managedIdentityCredential = new ManagedIdentityCredential(userManagedAssignedClientId);
                // Create CosmosClient using the credential
                using CosmosClient cosmosClient = new CosmosClient(cosmosEndpoint, managedIdentityCredential);

                // Access a specific database and container
                var database = cosmosClient.GetDatabase(databaseId);
                await database.ReadAsync();  // Ensure the database exists
                var container = database.GetContainer(containerId);

                _logger.LogInformation("Successfully connected to Azure Cosmos DB!");

                // Fetch and return container properties
                var properties = await container.ReadContainerAsync();
                result = $"Container Id: {properties.Resource.Id}, Last Modified: {properties.Resource.LastModified}";
            }
            catch (CosmosException cosmosEx)
            {
                // Log Cosmos DB-specific exceptions
                _logger.LogError($"Cosmos DB Error: {cosmosEx.StatusCode} - {cosmosEx.Message}");
                result = $"Cosmos DB Error: {cosmosEx.StatusCode} - {cosmosEx.Message}";
            }
            catch (Exception ex)
            {
                // Log other exceptions
                _logger.LogError($"Error occurred: {ex.Message}");
                result = $"Error occurred: {ex.Message}";
            }

            return result;
        }

        [Function("MyHttpTrigger")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // Call CosmosConnectionRBAC and get the result
            string cosmosResult = await CosmosConnectionRBAC();

            // Log the result (if you want to log it to Azure Logs)
            _logger.LogInformation($"Cosmos DB Result: {cosmosResult}");

            // Return the result as an HTTP response
            return new OkObjectResult($"Cosmos DB Result: {cosmosResult}");
        }
    }
}
