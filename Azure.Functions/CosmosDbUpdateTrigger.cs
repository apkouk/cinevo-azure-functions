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
                HttpResponseMessage responseMessage = client.GetAsync("https://cinevoazurefunctions.azurewebsites.net/api/CinemaScrapper?code=NYIH2qVhqSyEHRjohSINjYseGrAbEGqAdfSMskzoSXIEkVP2Zj5/bg==&name=Paco").Result;
                log.LogInformation("Message from cinemas scrapper " + responseMessage.StatusCode);
            }
        }
    }
}
