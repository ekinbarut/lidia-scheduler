using Hangfire;
using Lidia.Framework.Configuration;
using Lidia.Framework.Logging;
using System.Web.Hosting;

namespace Lidia.Scheduler.Hangfire
{
    public class HangfireApplicationPreload : IProcessHostPreloadClient
    {
        public void Preload(string[] parameters)
        {
            // Re-Load the configuration
            ConfigurationService.Load();

            // Log the service state
            LogService.Info("Lidia Scheduler preload requested!");

            // Hangfire
            HangfireBootstrapper.Instance.Start();

            // Log the service state
            LogService.Info("Hangfire started!");

            // Register the main job
            RecurringJob.AddOrUpdate("Daily Job Registration", () => HangfireService.UpdateJobs(), Cron.Daily);

            // Log the service state
            LogService.Info("Daily job registration service updated! Preload completed!");

        }
    }
}
