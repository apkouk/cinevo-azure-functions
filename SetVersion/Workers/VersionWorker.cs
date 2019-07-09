using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using SetVersion.Models;
using Version = SetVersion.Models.Version;

namespace SetVersion.Workers
{
    class VersionWorker
    {
        private CosmosClient cosmosClient;
        private DatabaseResponse database;
        private ContainerResponse container;
        private IConfigurationRoot _config;


        public async Task SetVersion(IConfigurationRoot config)
        {
            _config = config;
            cosmosClient = new CosmosClient(_config["EndpointUri"], _config["SecondaryKey"]);
            await CreateDatabase();
            await DeleteContainer();
            await CreateContainer();
            await AddItemsToContainer();
        }

        private async Task CreateDatabase()
        {
            database = await cosmosClient.CreateDatabaseIfNotExistsAsync(_config["DatabaseId"]);
            Console.WriteLine("Created Database: {0}\n", database.Database.Id);
        }

        private async Task DeleteContainer()
        {
            Container container = database.Database.GetContainer(_config["VersionContainerId"]);
            await container.DeleteContainerAsync();
            Console.WriteLine("Deleted Container: {0}\n", container.Id);
        }

        private async Task CreateContainer()
        {
            container = await database.Database.CreateContainerIfNotExistsAsync(_config["VersionContainerId"], "/version");
            Console.WriteLine("Created Container: {0}\n", container.Container.Id);
        }

        private async Task AddItemsToContainer()
        {

           Version version = new Version { Id = Guid.NewGuid(), Date = DateTime.Now };

            PartitionKey partitionKey = new PartitionKey("version");
            ItemResponse<Version> versionResponse = await container.Container.ReadItemAsync<Version>(version.Id.ToString(), partitionKey);

            if (versionResponse.StatusCode == HttpStatusCode.NotFound)
            {
                versionResponse = await container.Container.CreateItemAsync<Version>(version);
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", versionResponse.Resource.Id, versionResponse.RequestCharge);
            }
            else
            {
                Console.WriteLine("Item in database with id: {0} already exists\n", versionResponse.Resource.Id);
            }
        }
    }
}
