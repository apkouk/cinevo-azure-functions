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
using System.Linq;
using Microsoft.Extensions.Logging;

namespace SetVersion.Workers
{
    class VersionWorker
    {
        private CosmosClient cosmosClient;
        private DatabaseResponse database;
        private ContainerResponse container;
        private IConfigurationRoot _config;
        private ILogger _log;


        public async Task SetVersion(IConfigurationRoot config, ILogger log)
        {
            _config = config;
            _log = log;
            cosmosClient = new CosmosClient(_config["EndpointUri"], _config["SecondaryKey"]);
            await CreateDatabase();
            await CreateContainer();
        }

        private async Task CreateDatabase()
        {
            database = await cosmosClient.CreateDatabaseIfNotExistsAsync(_config["DatabaseId"]);
            _log.LogInformation("Created Database: \n", database.Database.Id);
        }

        private async Task DeleteContainer()
        {
            Container container = database.Database.GetContainer(_config["VersionContainerId"]);
            await container.DeleteContainerAsync();
            _log.LogInformation("Deleted Container: {0}\n", container.Id);
        }

        private async Task CreateContainer()
        {
            _log.LogInformation("Creating version container");
           container = await database.Database.CreateContainerIfNotExistsAsync(_config["VersionContainerId"], "/version");

            FeedIterator<Version> setIterator = container.Container.GetItemLinqQueryable<Version>()
                     .Where(b => b.Date < DateTime.Now)
                     .ToFeedIterator();

           
            if (setIterator.HasMoreResults)
            {
                while (setIterator.HasMoreResults)
                {
                    FeedResponse<Version> queryResponse = await setIterator.ReadNextAsync();
                    IEnumerator<Version> iter = queryResponse.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        Version version = new Version { Id = iter.Current.Id, Date = DateTime.Now };
                        await container.Container.UpsertItemAsync<Version>(version);
                        _log.LogInformation("Version Updated: \n" + version.Id + " Date: " + version.Date);
                    }
                }
            }
            else
            {
                await CreateBaseVersion();
            }

        }

        private async Task CreateBaseVersion()
        {

            Version version = new Version { Id = Guid.NewGuid(), Date = DateTime.Now };

            PartitionKey partitionKey = new PartitionKey("version");
            ItemResponse<Version> versionResponse = await container.Container.ReadItemAsync<Version>(version.Id.ToString(), partitionKey);

            if (versionResponse.StatusCode == HttpStatusCode.NotFound)
            {
                versionResponse = await container.Container.CreateItemAsync<Version>(version);
                _log.LogInformation("Created item in database with id: {0} Operation consumed {1} RUs.\n", versionResponse.Resource.Id, versionResponse.RequestCharge);
            }
            else
            {
                _log.LogInformation("Item in database with id: {0} already exists\n", versionResponse.Resource.Id);
            }
        }
    }
}
