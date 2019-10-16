using Hangfire;
using Lidia.Framework.Configuration;
using Lidia.Framework.Logging;
using Lidia.Framework.SaaS;
using Lidia.Framework.SaaS.Interfaces;
using Lidia.Scheduler.Hangfire;
using Lidia.Scheduler.Interfaces;
using LightInject;
using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Lidia.Scheduler.UI
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Load the configuration
            ConfigurationService.Load();

            // Log the application initialization
            LogService.Debug("Lidia Scheduler UI initialized");

            // Hangfire
            HangfireBootstrapper.Instance.Start();

            // Register the main job
            RecurringJob.AddOrUpdate("Daily Job Registration", () => HangfireService.UpdateJobs(), Cron.Daily);

            #region [ Dependency Injection ]

            // Create the container as usual.
            var container = new ServiceContainer();
            var lifeTime = new PerRequestLifeTime();

            // Register your types, for instanceScoped);
            container.Register<ITenantService, TenantService>(lifeTime);
            container.Register<IUserService, UserService>(lifeTime);
            container.Register<ISchedulerService, SchedulerService>(lifeTime);

            container.RegisterControllers();

            container.EnableMvc();

            #endregion
        }
        protected void Application_End(object sender, EventArgs e)
        {
            HangfireBootstrapper.Instance.Stop();
        }
    }
}