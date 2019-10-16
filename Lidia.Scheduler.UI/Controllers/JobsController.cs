using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Lidia.Scheduler.Models;
using Lidia.Scheduler.UI.Models.ViewModels;
using Lidia.Framework.SaaS.Interfaces;
using Lidia.Framework.Logging;
using Lidia.Scheduler.UI.Models.RequestModels;
using Newtonsoft.Json;
using Lidia.Scheduler.Hangfire;
using Lidia.Framework.SaaS.Models;

namespace Lidia.Scheduler.UI.Controllers
{
    public class JobsController : BaseController
    {
        public JobsController(ITenantService tenantService, IUserService userService) : base(tenantService, userService) { }

        // GET: Jobs
        public ActionResult Index()
        {
            List<Job> jobs = new List<Job>();

            try
            {
                // Get the jobs from the database
                if (User.IsInRole("SystemAdministrator"))
                {
                    using (var ctx = new SchedulerDbContext())
                    {
                        jobs = ctx.Jobs.Include("Collection").ToList();
                    }
                }
                else
                {
                    using (var ctx = new SchedulerDbContext())
                    {
                        jobs = ctx.Jobs.Where(i => i.Collection.TenantId == CurrentTenant.TenantId).Include("Collection").ToList();
                    }
                }

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Jobs"
                });

                ViewBag.Breadcrumb = breadcrumb;
            }

            catch (Exception ex)
            {
                LogService.Info("Jobs not found", ex.Message, ex.InnerException);
            }

            return View(jobs);
        }

        // GET: Jobs/5/Details
        [Route("Jobs/{id}/Details")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Create the model
            var model = new JobViewModel();

            var job = new Job();
            try
            {
                using (var ctx = new SchedulerDbContext())
                {
                    if (User.IsInRole("SystemAdministrator"))
                    {
                        job = ctx.Jobs.Where(i => i.JobId == id).Include("Collection").FirstOrDefault();
                    }
                    else
                    {
                        job = ctx.Jobs.Where(i => i.JobId == id).Where(i => i.Collection.TenantId == CurrentTenant.TenantId).Include("Collection").FirstOrDefault();
                    }
                }
                if (job == null)
                {
                    return View("Authorize");
                }

                model.Job = job;

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Jobs",
                    Link = "/Jobs"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = job.Name,
                    Link = "/Jobs/" + job.JobId + "/Details"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Details"
                });

                ViewBag.Breadcrumb = breadcrumb;

            }

            catch (Exception ex)
            {
                LogService.Info("Jobs not found", ex.Message, ex.InnerException);
            }

            return View(model);
        }

        // GET: Jobs/Create
        [Route("Jobs/Create")]
        public ActionResult Create()
        {
            try
            {
                using (var ctx = new SchedulerDbContext())
                {
                    if (User.IsInRole("SystemAdministrator"))
                    {
                        ViewBag.CollectionId = new SelectList(ctx.Collections.ToList(), "CollectionId", "Name");
                    }
                    else
                    {
                        ViewBag.CollectionId = new SelectList(ctx.Collections.Where(i => i.TenantId == CurrentTenant.TenantId).ToList(), "CollectionId", "Name");
                    }
                }
                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Jobs",
                    Link = "/Jobs"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "New"
                });

                ViewBag.Breadcrumb = breadcrumb;
            }

            catch (Exception ex)
            {
                LogService.Info("Jobs not found", ex.Message, ex.InnerException);
            }

            return View();
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Jobs/Create")]
        public ActionResult Create([Bind(Include = "JobId,CollectionId,Type,Name,AppKey,Process,Description,CronExpression,JobUrl,ProcessResult,SendSummary,Created,Updated,Status")] Job job)
        {
            try
            {
                using (var ctx = new SchedulerDbContext())
                {
                    if (User.IsInRole("SystemAdministrator"))
                    {
                        if (ModelState.IsValid)
                        {
                            ctx.Jobs.Add(job);
                            ctx.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        var tenantid = ctx.Collections.Where(i => i.CollectionId == job.CollectionId).FirstOrDefault().TenantId;

                        if (tenantid == CurrentTenant.TenantId)
                        {
                            if (ModelState.IsValid)
                            {
                                ctx.Jobs.Add(job);
                                ctx.SaveChanges();
                                return RedirectToAction("Index");
                            }
                        }
                    }

                    ViewBag.CollectionId = new SelectList(ctx.Collections, "CollectionId", "Name", job.CollectionId);
                }

            }

            catch (Exception ex)
            {
                LogService.Info("Jobs not found", ex.Message, ex.InnerException);
            }

            return View(job);
        }

        // GET: Jobs/5/Edit
        [Route("Jobs/{id}/Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Create the model
            var model = new EditJobViewModel();
            var job = new Job();

            try
            {
                using (var ctx = new SchedulerDbContext())
                {
                    //Get the job from db
                    job = ctx.Jobs.Find(id);

                    ViewBag.CollectionId = new SelectList(ctx.Collections.ToList(), "CollectionId", "Name", job.CollectionId);

                    //Add job to model
                    model.JobId = job.JobId;
                    model.CollectionId = job.CollectionId;
                    model.Type = job.Type;
                    model.Name = job.Name;
                    model.AppKey = job.AppKey;
                    model.Process = job.Process;
                    model.Description = job.Description;
                    model.CronExpression = job.CronExpression;
                    model.JobUrl = job.JobUrl;
                    model.ProcessResult = job.ProcessResult;
                    model.SendSummary = job.SendSummary;
                    model.Created = job.Created;
                    model.Status = job.Status;
                }

                if (job == null)
                {
                    return HttpNotFound();
                }

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Jobs",
                    Link = "/Jobs"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = job.Name,
                    Link = "/Jobs/" + job.JobId + "/Details"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Edit"
                });

                ViewBag.Breadcrumb = breadcrumb;
            }

            catch (Exception ex)
            {
                LogService.Info("Jobs not found", ex.Message, ex.InnerException);
            }

            if (User.IsInRole("SystemAdministrator") || job.Collection.TenantId == CurrentTenant.TenantId)
            {
                return View(model);
            }

            return View("Authorize");
        }

        // POST: Jobs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Jobs/{id}/Edit")]
        public ActionResult Edit([Bind(Include = "JobId,CollectionId,Type,Name,AppKey,Process,Description,CronExpression,JobUrl,ProcessResult,SendSummary,Created,Updated,Status")] EditJobViewModel jobViewModel)
        {
            try
            {
                using (var ctx = new SchedulerDbContext())
                {
                    // Get existing job from db
                    var job = ctx.Jobs.Where(j => j.JobId == jobViewModel.JobId).FirstOrDefault();

                    job.Type = jobViewModel.Type;
                    job.Name = jobViewModel.Name;
                    job.CronExpression = jobViewModel.CronExpression;
                    job.AppKey = jobViewModel.AppKey;
                    job.Process = jobViewModel.Process;
                    job.Description = jobViewModel.Description;
                    job.JobUrl = jobViewModel.JobUrl;
                    job.Status = jobViewModel.Status;

                    //Get tenantId
                    var tenantID = ctx.Collections.Where(t => t.CollectionId == jobViewModel.CollectionId).FirstOrDefault().TenantId;

                    if (User.IsInRole("SystemAdministrator") || tenantID == CurrentTenant.TenantId)
                    {
                        if (ModelState.IsValid)
                        {
                            ctx.Entry(job).State = EntityState.Modified;
                            ctx.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        ViewBag.CollectionId = new SelectList(ctx.Collections.ToList(), "CollectionId", "Name", job.CollectionId);
                    }
                    else
                    {
                        return View("Authorize");
                    }
                }
            }

            catch (Exception ex)
            {
                LogService.Info("Jobs not found", ex.Message, ex.InnerException);
            }


            return View(jobViewModel);
        }

        // GET: Jobs/5/Delete
        [Route("Jobs/{id}/Delete")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Create the model
            var model = new JobViewModel();
            var job = new Job();

            try
            {
                using (var ctx = new SchedulerDbContext())
                {
                    job = ctx.Jobs.Where(j => j.JobId == id).Include("Collection").FirstOrDefault();
                }

                if (job == null)
                {
                    return View("Error");
                }

                //Add job to model
                model.Job = job;

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Jobs",
                    Link = "/Jobs"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = job.Name,
                    Link = "/Jobs/" + job.JobId + "/Details"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Delete Confirm"
                });

                ViewBag.Breadcrumb = breadcrumb;


                if (User.IsInRole("SystemAdministrator") || job.Collection.TenantId == CurrentTenant.TenantId)
                {
                    return View(model);
                }

            }

            catch (Exception ex)
            {
                LogService.Info("Jobs not found", ex.Message, ex.InnerException);
            }

            return View("Authorize");
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Jobs/{id}/Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var job = new Job();
            try
            {
                using (var ctx = new SchedulerDbContext())
                {
                    job = ctx.Jobs.Where(j => j.JobId == id).Include("Collection").FirstOrDefault();

                    if (User.IsInRole("SystemAdministrator") || job.Collection.TenantId == CurrentTenant.TenantId)
                    {
                        ctx.Jobs.Remove(job);
                        ctx.SaveChanges();
                    }
                }
            }

            catch (Exception ex)
            {
                LogService.Info("Jobs not found", ex.Message, ex.InnerException);
            }

            return RedirectToAction("Index");
        }

        #region [ Data services ]

        [Route("Jobs/Filtered/{jobid}/{val?}")]
        public JsonResult Filtered(int jobid, string val)
        {
            //Create the model
            var model = new JobViewModel();
            var job = new Job();
            try
            {
                using (var ctx = new SchedulerDbContext())
                {
                    job = ctx.Jobs.Include("Log").Where(i => i.JobId == jobid).FirstOrDefault();
                }

                //Add job to model
                model.Job = job;

                if (String.IsNullOrEmpty(val))
                {
                    // Do nothing
                    model.Job.Log.ToList();
                }
                else
                {
                    model.Job.Log = job.Log.Where(x => x.Details == val).ToList();
                }
            }

            catch (Exception ex)
            {
                LogService.Info("Jobs not found", ex.Message, ex.InnerException);
            }

            return Json(new { data = model.Job.Log.Select(l => l).ToList() }, JsonRequestBehavior.AllowGet);
        }

        [Route("Jobs/{jobid}/Logs/{logid}/Details")]
        public JsonResult JobLogDetails(int jobid, int logid)
        {
            try
            {
                using (var ctx = new SchedulerDbContext())
                {
                    // Get the related entry
                    var logEntry = ctx.JobLogEntries.FirstOrDefault(jle => jle.EntryId == logid);

                    var str = logEntry.Details;

                    // Convert to json 
                    JsonLogResponse json = JsonConvert.DeserializeObject<JsonLogResponse>(str);

                    if (logEntry != null)
                    {
                        //return Json(new { Test1 = logEntry.Change, Test2 = logEntry.Summary }, JsonRequestBehavior.AllowGet);

                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                }

            }

            catch (Exception ex)
            {
                LogService.Info("Job log not found", ex.Message, ex.InnerException);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }


        #endregion


        #region [ Service methods ]

        public async Task<ActionResult> Register()
        {
            HangfireService.RegisterJobs();

            return Json(new { Type = 200 }, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}