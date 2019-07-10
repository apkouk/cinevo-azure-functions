using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SetVersion.Workers;
using System.Threading.Tasks;

namespace SetVersion
{
    public static class SetVersion
    {
        [FunctionName("SetVersion")]
        //{second} {minute} {hour} {day} {month} {day-of-week}
        public static async Task RunAsync([TimerTrigger("0 0 5 * * *")]TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            var config = new ConfigurationBuilder().SetBasePath(context.FunctionAppDirectory)
                                                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                                                    .AddEnvironmentVariables()
                                                    .Build();

            await new VersionWorker().SetVersion(config, log);

        }
    }
}
