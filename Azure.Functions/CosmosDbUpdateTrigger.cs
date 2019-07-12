using System;
using System.Collections.Generic;
using System.Net.Http;
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
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionPrefix ="CosmosDbUpdateTrigger")]IReadOnlyList<Document> input, ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Updating cinema data... \n");
                HttpClient client = new HttpClient();
                HttpResponseMessage responseMessage = client.GetAsync("https://cinevoazurefunctionsapp.azurewebsites.net/api/CinemaScrapper?code=ifaHC20/0xB/bvOReQ/bhsAO1wezZ4yWFlXzkLlkL7x1AHEbJnvV4Q==&name=Paco").Result;
                log.LogInformation("Message from cinemas scrapper " + responseMessage.StatusCode);
            }
        }
    }
}
