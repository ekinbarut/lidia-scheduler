using Hangfire;
using Lidia.Framework.Configuration;
using Lidia.Framework.Logging;
using Lidia.Framework.SaaS.Interfaces;
using Lidia.Scheduler.Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lidia.Scheduler.UI.Controllers
{
    public class SystemHealthController : BaseController
    {
        public SystemHealthController(ITenantService tenantService, IUserService userService) : base(tenantService, userService) { }

        [Route("heartbeat")]
        public ActionResult Heartbeat()
        {
            // Re-Load the configuration
            ConfigurationService.Load();

            // Log the service state
            LogService.Info("Heartbeat requested!");

            // Hangfire
            HangfireBootstrapper.Instance.Start();

            // Log the service state
            LogService.Info("Hangfire re-started!");

            // Register the main job
            RecurringJob.AddOrUpdate("Daily Job Registration", () => HangfireService.UpdateJobs(), Cron.Daily);

            // Log the service state
            LogService.Info("Daily job registration service updated! Lub-dub!");

            return Json("lub-dub", JsonRequestBehavior.AllowGet);
        }
    }
}