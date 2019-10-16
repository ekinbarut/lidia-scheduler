using Lidia.Framework.Logging;
using Lidia.Scheduler.Base;
using Lidia.Scheduler.Common.Models.Responses;
using Lidia.Scheduler.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Lidia.Scheduler
{
    public static class JobService
    {
        private static SchedulerService schedulerService = new SchedulerService();
        private static JobServiceBase jobServiceBase = new JobServiceBase();
        private static JobLogEntryServiceBase jobLogEntryServiceBase = new JobLogEntryServiceBase();
        private static SchedulerMailService mailService = new SchedulerMailService();
        public static bool Process(int jobId, string name)
        {
            // Load the job
            var job = jobServiceBase.FindIncluding(j => j.JobId == jobId, "Collection", "Tags", "Properties").Result.FirstOrDefault();
            

            // Log the service state
            LogService.Info("Starting job! JobId:" + jobId + "; Name:" + job.Name + "; Collection:" + job.Collection.Name);
            

            // Add the log entry
            var startLog = new JobLogEntry()
            {
                JobId = job.JobId,
                ActivityType = JobActivityType.Start,
                Summary = "Job started.",
                Created = DateTime.UtcNow,
            };

            // Save the log
            var logResult = jobLogEntryServiceBase.Create(startLog);

            if (job.Type == JobType.APIFunction)
            {
                using (var client = new HttpClient())
                {
                    // Create the api client
                    client.BaseAddress = new Uri(job.JobUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromMinutes(60);

                    using (var ctx = new SchedulerDbContext())
                    {
                        try
                        {
                            // Call the api
                            using (HttpResponseMessage response = client.GetAsync(string.Empty).Result)
                            using (HttpContent content = response.Content)
                            {
                                // Read the result
                                var result = content.ReadAsStringAsync().Result;

                                // Check the result length
                                if (result.Length > 1024000)
                                {
                                    result = "Result too long to log! Please check the related service log for details.";
                                    LogService.Debug("Result too long to log! Please check the related service log for details. Job: " + job.AppKey);
                                }

                                // Log the service state
                                LogService.Debug("API result: " + result);

                                
                                // Check the status 
                                if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    #region [ Response processing ]

                                    try
                                    {
                                        // Try to parse the result
                                        var processedResult = JsonConvert.DeserializeObject<APICallResponse>(result);
                                        
                                    }
                                    catch (Exception ex)
                                    {
                                        // Log the error
                                        LogService.Error("Error while processing the API result. JobId:" + job.JobId + ", Result:" + result);
                                    }

                                    // Add the log entry
                                    var endLog = new JobLogEntry()
                                    {
                                        JobId = job.JobId,
                                        ActivityType = JobActivityType.End,
                                        Summary = "Job ended with HTTP Status OK.",
                                        Details = result,
                                        Created = DateTime.UtcNow,
                                    };

                                    // Save the log
                                    jobLogEntryServiceBase.Create(endLog);
                                    

                                    return true;
                                }
                                else
                                {
                                    // Add the log entry
                                    var endLog = new JobLogEntry()
                                    {
                                        JobId = job.JobId,
                                        ActivityType = JobActivityType.End,
                                        Summary = "Job ended with HTTP Status NOT OK.",
                                        Details = result,
                                        Created = DateTime.UtcNow,
                                    };

                                    // Save the log
                                    jobLogEntryServiceBase.Create(endLog);

                                    

                                    return false;
                                }
                                #endregion
                            }
                        }


                        catch (Exception ex)
                        {

                            // Log the service state
                            LogService.Info("API exception: " + ex.ToString());

                            // Add the log entry
                            var exceptionLog = new JobLogEntry()
                            {
                                JobId = job.JobId,
                                ActivityType = JobActivityType.End,
                                Summary = "Job ended with EXCEPTION!",
                                Details = ex.ToString(),
                                Created = DateTime.UtcNow,
                            };

                            // Save the log
                            jobLogEntryServiceBase.Create(exceptionLog);

                            throw new Exception("General error while calling the api URL! Exception:" + ex, ex);
                        }
                    }
                }
            }


            return false;
        }

    }
}