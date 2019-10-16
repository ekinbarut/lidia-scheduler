using Hangfire;
using Lidia.Framework.Logging;
using Lidia.Framework.Models;
using Lidia.Scheduler.Base;
using Lidia.Scheduler.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace Lidia.Scheduler.Hangfire
{
    public class HangfireService
    {
        private static JobServiceBase jobServiceBase = new JobServiceBase();

        [AutomaticRetry(Attempts = 5)]
        public static void UpdateJobs()
        {
            // Log the state
            LogService.Debug("Registering hangfire jobs!");
            

            // Get the active jobs
            var activeJobs =  jobServiceBase.GetIncluding(j => j.Status == EntityStatus.Active, "Collection").Result;

            // Log the state
            LogService.Debug("Active job count:" + activeJobs.Count);

            foreach (var job in activeJobs)
            {
                if (!string.IsNullOrEmpty(job.CronExpression))
                {
                    try
                    {
                        RecurringJob.AddOrUpdate(job.AppKey + "." + job.Process, () => JobService.Process(job.JobId, job.Name), job.CronExpression, TimeZoneInfo.Local);
                    }
                    catch (Exception ex)
                    {
                        // Log the state
                        LogService.Debug("Error while registering job! JobId:" + job.JobId + "; JobName:" + job.Name + "; Exception:" + ex.ToString());
                    }
                }                
            }

            // Get the paused jobs
            var pausedJobs = jobServiceBase.GetIncluding(j => j.Status == EntityStatus.Freezed, "Collection").Result;

            // Log the state
            LogService.Debug("Paused job count:" + pausedJobs.Count);

            foreach (var job in pausedJobs)
            {
                RecurringJob.RemoveIfExists(job.AppKey + "." + job.Process);
            }

            // Get the passive jobs
            var passiveJobs = jobServiceBase.GetIncluding(j => j.Status == EntityStatus.Passive, "Collection").Result;

            // Log the state
            LogService.Debug("Passive job count:" + pausedJobs.Count);

            foreach (var job in passiveJobs)
            {
                RecurringJob.RemoveIfExists(job.AppKey + "." + job.Process);
            }

            // Log the state
            LogService.Debug("All active jobs registered!");

        }


        [AutomaticRetry(Attempts = 5)]
        public static void RegisterJobs()
        {
            // Log the state
            LogService.Debug("Registering jobs!");

            using (var db = new SchedulerDbContext())
            {
                // Get the active jobs
                var activeJobs = db.Jobs.Where(j => j.Status == EntityStatus.Active).Include("Collection").ToList();

                // Log the state
                LogService.Debug("Active job count:" + activeJobs.Count);

                foreach (var job in activeJobs)
                {
                    if (!string.IsNullOrEmpty(job.CronExpression))
                    {
                        try
                        {
                            RecurringJob.AddOrUpdate(job.AppKey + "." + job.Process, () => JobService.Process(job.JobId, job.Name), job.CronExpression, TimeZoneInfo.Local);
                        }
                        catch (Exception ex)
                        {
                            // Log the state
                            LogService.Debug("Error while registering job! JobId:" + job.JobId + "; JobName:" + job.Name + "; Exception:" + ex.ToString());
                        }
                    }
                }

                // Get the paused jobs
                var pausedJobs = db.Jobs.Where(j => j.Status == EntityStatus.Freezed).Include("Collection").ToList();

                // Log the state
                LogService.Debug("Paused job count:" + pausedJobs.Count);

                foreach (var job in pausedJobs)
                {
                    RecurringJob.RemoveIfExists(job.AppKey + "." + job.Process);
                }

                // Get the passive jobs
                var passiveJobs = db.Jobs.Where(j => j.Status == EntityStatus.Passive).Include("Collection").ToList();

                // Log the state
                LogService.Debug("Passive job count:" + pausedJobs.Count);

                foreach (var job in passiveJobs)
                {
                    RecurringJob.RemoveIfExists(job.AppKey + "." + job.Process);
                }
            }

            // Log the state
            LogService.Debug("All active jobs registered!");

        }
    }
}
