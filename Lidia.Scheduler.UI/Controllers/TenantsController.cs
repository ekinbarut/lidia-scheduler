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
using System.Web.Mvc;
namespace Lidia.Scheduler.UI.Controllers
{
    [Authorize(Roles = "SystemAdministrator")]
    public class TenantsController : BaseController
    {
        public TenantsController(ITenantService tenantService, IUserService userService) : base(tenantService, userService) { }

        // GET: Tenants
        [Route("Tenants")]
        public ActionResult Index()
        {
            // Create the model
            var model = new TenantListViewModel();
            try
            {
                using (var ctx = new SaasDbContext())
                {
                    // Get tenants from the db
                    var tenants = ctx.Tenants.Include("Applications").ToList();

                    // Fill the view object
                    model.Tenants = tenants.Select(t => new TenantViewModel()
                    {
                        Tenant = t
                    }).ToList();
                }

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Tenants"
                });
                ViewBag.Breadcrumb = breadcrumb;
            }

            catch (Exception ex)
            {
                LogService.Info("Tenants not found", ex.Message, ex.InnerException);
            }

            return View(model);
        }

        [Route("Tenants/Create")]
        public ActionResult Create()
        {
            try
            {   // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Tenants",
                    Link = "/Tenants"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "New"
                });

                ViewBag.Breadcrumb = breadcrumb;
            }

            catch (Exception ex)
            {
                LogService.Info("Tenants not found", ex.Message, ex.InnerException);
            }
            return View();
        }

        // POST: Tenants/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Tenants/Create")]
        public ActionResult Create([Bind(Include = "Code,Name,Description,Culture,TimeZone,Created,Status")] Tenant tenant)
        {
            try
            {
                using (var ctx = new SaasDbContext())
                {
                    if (ModelState.IsValid)
                    {
                        ctx.Tenants.Add(tenant);
                        ctx.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Info("Tenants not found", ex.Message, ex.InnerException);
            }

            return View(tenant);
        }

        [Route("Tenants/{id}/Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Create the model
            var model = new EditTenantViewModel();

            var tenant = new Tenant();
            try
            {
                using (var ctx = new SaasDbContext())
                {
                    tenant = ctx.Tenants.Find(id);
                }
                if (tenant == null)
                {
                    return HttpNotFound();
                }

                //Add tenant to model
                model.TenantId = tenant.TenantId;
                model.Code = tenant.Code;
                model.Name = tenant.Name;
                model.Description = tenant.Description;
                model.Culture = tenant.Culture;
                model.TimeZone = tenant.TimeZone;
                model.Created = tenant.Created;
                model.Status = tenant.Status;

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Tenants",
                    Link = "/Tenants"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = tenant.Name,
                    Link = "/Tenants/" + tenant.TenantId + "/Details"
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

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Tenants/{id}/Edit")]
        public ActionResult Edit([Bind(Include = "TenantId,Code,Name,Description,Culture,TimeZone,Created,Status")] EditTenantViewModel tenantViewModel)
        {
            try
            {
                using (var ctx = new SaasDbContext())
                {
                    // Get existing tenant from db
                    var tenant = ctx.Tenants.Where(t => t.TenantId == tenantViewModel.TenantId).FirstOrDefault();
                    tenant.Code = tenantViewModel.Code;
                    tenant.Name = tenantViewModel.Name;
                    tenant.Description = tenantViewModel.Description;
                    tenant.Culture = tenantViewModel.Culture;
                    tenant.TimeZone = tenantViewModel.TimeZone;
                    tenant.Status = tenantViewModel.Status;

                    if (ModelState.IsValid)
                    {
                        ctx.Entry(tenant).State = EntityState.Modified;
                        ctx.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            }

            catch (Exception ex)
            {
                LogService.Info("Tenants not found", ex.Message, ex.InnerException);
            }

            return View(tenantViewModel);
        }

        [Route("Tenants/{id}/Details")]
        public ActionResult Details(int? id)
        {
            // Create the model
            var model = new TenantViewModel();
            var tenant = new Tenant();

            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                using (var ctx = new SaasDbContext())
                {
                    // Get tenant from db 
                    tenant = ctx.Tenants.Include("Applications").Where(i => i.TenantId == id).FirstOrDefault();
                }

                if (tenant == null)
                {
                    return HttpNotFound();
                }

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Tenants",
                    Link = "/Tenants"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = tenant.Name,
                    Link = "/Tenants/" + tenant.TenantId + "/Details"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Details"
                });

                ViewBag.Breadcrumb = breadcrumb;

                // Set the tenant
                model.Tenant = tenant;
            }

            catch (Exception ex)
            {
                LogService.Info("Tenants not found", ex.Message, ex.InnerException);
            }

            return View(model);
        }

        // GET: Tenants/5/Delete
        [Route("Tenants/{id}/Delete")]
        public ActionResult Delete(int? id)
        {
            // Create the model
            var model = new TenantViewModel();
            var tenant = new Tenant();

            try
            {
                using (var ctx = new SaasDbContext())
                {
                    // Get tenant from db 
                    tenant = ctx.Tenants.Include("Applications").Where(i => i.TenantId == id).FirstOrDefault();
                }
                if (tenant == null)
                {
                    return HttpNotFound();
                }

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Tenants",
                    Link = "/Tenants"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = tenant.Name,
                    Link = "/Tenants/" + tenant.TenantId + "/Details"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Delete Confirm"
                });

                ViewBag.Breadcrumb = breadcrumb;

                // Set the tenant
                model.Tenant = tenant;
            }

            catch (Exception ex)
            {
                LogService.Info("Tenants not found", ex.Message, ex.InnerException);
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Tenants/{id}/Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (var ctx = new SaasDbContext())
                {
                    //Get tenant from db
                    Tenant tenant = ctx.Tenants.Find(id);
                    ctx.Tenants.Remove(tenant);
                    ctx.SaveChanges();
                }

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Tenants",
                    Link = "/Tenants"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Delete Confirm"
                });

                ViewBag.Breadcrumb = breadcrumb;
            }

            catch (Exception ex)
            {
                LogService.Info("Tenants not found", ex.Message, ex.InnerException);
            }
            return RedirectToAction("Index");
        }

    }
}