using Lidia.Framework.Logging;
using Lidia.Framework.SaaS.Interfaces;
using Lidia.Framework.SaaS.Models;
using Lidia.Scheduler.Models;
using Lidia.Scheduler.UI.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Lidia.Scheduler.UI.Controllers
{
    [Authorize(Roles = "SystemAdministrator,TenantAdministrator")]
    public class ApplicationsController : BaseController
    {
        public ApplicationsController(ITenantService tenantService, IUserService userService) : base(tenantService, userService) { }

        // GET: Applications
        public ActionResult Index()
        {
            List<Application> applications = new List<Application>();

            try
            {
                using (var ctx = new SaasDbContext())
                {
                    if (User.IsInRole("SystemAdministrator"))
                    {
                        //Get all applications from db
                        applications = ctx.Applications.Include(xi => xi.Tenant).ToList();
                    }
                    else
                    {
                        //Get current tenant applications from db
                        applications = ctx.Applications.Where(i => i.TenantId == CurrentTenant.TenantId).Include(xi => xi.Tenant).ToList();
                    }
                    //test
                    // Create the breadcrumb
                    var breadcrumb = new List<BreadcrumbItemViewModel>();
                    breadcrumb.Add(new BreadcrumbItemViewModel()
                    {
                        Text = "Applications"
                    });

                    ViewBag.Breadcrumb = breadcrumb;
                }

            }

            catch (Exception ex)
            {
                LogService.Info("Applications not found", ex.Message, ex.InnerException);
            }

            return View(applications);
        }

        [Route("Applications/{id}/Details")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Create the model
            var model = new ApplicationViewModel();
            try
            {
                using (var saaSctx = new SaasDbContext())
                {
                    using (var scHctx = new SchedulerDbContext())
                    {
                        var application = saaSctx.Applications.Where(a => a.Id == id).Include("Tenant").FirstOrDefault();

                        if (User.IsInRole("SystemAdministrator"))
                        {
                            // Get application from db
                            model.Application = application;

                            // Get collections from db
                            model.Collections = scHctx.Collections.Where(i => i.ApplicationId == id).Include("Application").ToList();
                        }
                        else if (application.TenantId == CurrentTenant.TenantId)
                        {
                            // Get application from db
                            model.Application = application;

                            // Get collections from db
                            model.Collections = scHctx.Collections.Where(i => i.ApplicationId == id).Where(t => t.TenantId == CurrentTenant.TenantId).Include("Application").ToList();
                        }
                        else
                        {
                            return View("Authorize");
                        }
                    }
                }


                if (model.Application == null)
                {
                    return View("Error");
                }

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Applications",
                    Link = "/Applications"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = model.Application.Name.ToString()
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Details"
                });

                ViewBag.Breadcrumb = breadcrumb;
            }

            catch (Exception ex)
            {
                LogService.Info("Applications not found", ex.Message, ex.InnerException);
            }
            return View(model);
        }

        [Route("Applications/Create")]
        public ActionResult Create()
        {
            using (var ctx = new SaasDbContext())
            {
                ViewBag.TenantId = new SelectList(ctx.Tenants.ToList(), "TenantId", "Name");
            }

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Applications",
                Link = "/Applications"
            });

            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "New"
            });

            ViewBag.Breadcrumb = breadcrumb;
            ViewBag.CurrentTenantId = CurrentTenant.TenantId;
            return View();
        }

        [Route("Applications/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TenantId,Code,Domain,Name,Culture,TimeZone,Updated,Created,Status")] Application application)
        {
            try
            {
                using (var ctx = new SaasDbContext())
                {
                    if (application.TenantId == CurrentTenant.TenantId || User.IsInRole("SystemAdministrator"))
                    {
                        if (ModelState.IsValid)
                        {
                            // Add application to db
                            ctx.Applications.Add(application);
                            ctx.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        return View("Authorize");
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Info("Application do not create", ex.Message, ex.InnerException);
            }

            return View(application);
        }

        [Route("Applications/{id}/Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Create the model
            var model = new EditApplicationViewModel();

            try
            {
                using (var ctx = new SaasDbContext())
                {
                    //Get application from db
                    var application = ctx.Applications.Find(id);

                    if (User.IsInRole("SystemAdministrator") || application.TenantId == CurrentTenant.TenantId)
                    {
                        // Add application item to model
                        model.Id = application.Id;
                        model.Name = application.Name;
                        model.ParentId = application.ParentId;
                        model.TenantId = application.TenantId;
                        model.TimeZone = application.TimeZone;
                        model.Code = application.Code;
                        model.Culture = application.Culture;
                        model.Domain = application.Domain;
                        model.Created = application.Created;
                        model.Status = application.Status;
                    }
                    else
                    {
                        return View("Authorize");
                    }

                    if (model == null)
                    {
                        return View("Error");
                    }
                }
                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Applications",
                    Link = "/Applications"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = model.Name.ToString(),
                    Link = "/Application/" + id + "/Details"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Edit"
                });

                ViewBag.Breadcrumb = breadcrumb;
            }

            catch (Exception ex)
            {
                LogService.Info("Application not found", ex.Message, ex.InnerException);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Applications/{id}/Edit")]
        public ActionResult Edit([Bind(Include = "Id,TenantId,Code,Domain,Name,Culture,TimeZone,Updated,Created,Status")] EditApplicationViewModel applicationViewModel)
        {
            try
            {
                using (var ctx = new SaasDbContext())
                {
                    if (applicationViewModel.TenantId == CurrentTenant.TenantId || User.IsInRole("SystemAdministrator"))
                    {
                        //Get existing application from db
                        var application = ctx.Applications.Where(a => a.Id == applicationViewModel.Id).FirstOrDefault();

                        application.Name = applicationViewModel.Name;
                        application.Code = applicationViewModel.Code;
                        application.Domain = applicationViewModel.Domain;
                        application.Culture = applicationViewModel.Culture;
                        application.TimeZone = applicationViewModel.TimeZone;
                        application.Status = applicationViewModel.Status;

                        if (ModelState.IsValid)
                        {
                            // Update application
                            ctx.Entry(application).State = EntityState.Modified;
                            ctx.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        ViewBag.TenantId = new SelectList(ctx.Tenants.ToList(), "TenantId", "Name");
                    }
                    else
                    {
                        return View("Authorize");
                    }
                }
            }

            catch (Exception ex)
            {
                LogService.Info("Applications not found", ex.Message, ex.InnerException);
            }

            return View(applicationViewModel);
        }

        [Route("Applications/{id}/Delete")]
        public ActionResult Delete(int? id)
        {
            // Create the model
            var model = new ApplicationViewModel();
            try
            {
                using (var ctx = new SaasDbContext())
                {

                    // Get application from db
                    var application = ctx.Applications.Where(i => i.Id == id).Include("Tenant").FirstOrDefault();

                    if (application.TenantId == CurrentTenant.TenantId || User.IsInRole("SystemAdministrator"))
                    {
                        // Add application to model
                        model.Application = application;
                    }
                    else
                    {
                        return View("Authorize");
                    }

                    if (model == null)
                    {
                        return View("Error");
                    }
                }

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Applications",
                    Link = "/Applications"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = model.Application.Name.ToString(),
                    Link = "/Application/" + id + "/Details"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Delete Confirm"
                });

                ViewBag.Breadcrumb = breadcrumb;


                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

            }

            catch (Exception ex)
            {
                LogService.Info("Applications not found", ex.Message, ex.InnerException);
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Applications/{id}/Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (var ctx = new SaasDbContext())
                {
                    //Get application from db
                    Application application = ctx.Applications.Find(id);

                    if (application.TenantId == CurrentTenant.TenantId || User.IsInRole("SystemAdministrator"))
                    {
                        if (ModelState.IsValid)
                        {
                            // Remove application from db
                            ctx.Applications.Remove(application);
                            ctx.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        return View("Authorize");
                    }
                }
            }

            catch (Exception ex)
            {
                LogService.Info("Applications not found", ex.Message, ex.InnerException);
            }

            return RedirectToAction("Index");
        }
    }
}