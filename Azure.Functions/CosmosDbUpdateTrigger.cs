using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Azure.Functions
{
    public static class CosmosDbUpdateTrigger
    {
        [FunctionName("CosmosDbUpdateTrigger")]
        public static void Run([CosmosDBTrigger(
            databaseName: "cinevo",
            collectionName: "versions",
            ConnectionStringSetting = "DatabaseConnection",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true, // Add this line
            LeaseCollectionPrefix ="CosmosDbUpdateTrigger")]IReadOnlyList<Document> input, ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);
            }
        }
    }
}
