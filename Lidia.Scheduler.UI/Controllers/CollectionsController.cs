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
using Lidia.Framework.SaaS.Models;
using Lidia.Scheduler.UI.Models.ViewModels;
using Lidia.Framework.SaaS.Interfaces;
using Lidia.Framework.Logging;

namespace Lidia.Scheduler.UI.Controllers
{
    [Authorize(Roles = "SystemAdministrator,TenantAdministrator")]
    public class CollectionsController : BaseController
    {
        public CollectionsController(ITenantService tenantService, IUserService userService) : base(tenantService, userService) { }

        public ActionResult Index()
        {
            List<JobCollection> collections = new List<JobCollection>();
            try
            {
                using (var ctx = new SchedulerDbContext())
                {
                    if (User.IsInRole("SystemAdministrator"))
                    {
                        //Get all collections with tenant from db
                        collections = ctx.Collections.Include(c => c.Tenant).Include(c => c.Application).ToList();
                    }
                    else
                    {
                        collections = ctx.Collections.Where(i => i.TenantId == CurrentTenant.TenantId).Include(xi => xi.Application).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Info("Collections not found", ex.Message, ex.InnerException);
            }

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Collections"
            });

            ViewBag.Breadcrumb = breadcrumb;

            return View(collections);
        }

        [Route("Collections/{id}/Details")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var model = new JobCollectionViewModel();
            var collection = new JobCollection();

            try
            {
                using (var ctx = new SchedulerDbContext())
                {
                    //Get collection from db

                    collection = ctx.Collections.Where(i => i.CollectionId == id).Include("Jobs").Include("Tenant").Include("Application").FirstOrDefault();

                    if (User.IsInRole("SystemAdministrator") || collection.TenantId == CurrentTenant.TenantId)
                    {
                        // Add collection to model 
                        model.Collection = collection;
                    }
                    else
                    {
                        return View("Authorize");
                    }
                }
            }

            catch (Exception ex)
            {
                LogService.Info("Collections not found", ex.Message, ex.InnerException);
            }

            if (collection == null)
            {
                return View("Error");
            }

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Collections",
                Link = "/Collections"
            });

            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = collection.Name,
                Link = "/Collections/" + id + "/Details"
            });

            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Details"
            });

            ViewBag.Breadcrumb = breadcrumb;
            return View(model);
        }

        [Route("Collections/Create")]
        public ActionResult Create()
        {
            try
            {
                //Get tenantId and ApplicationId
                using (SaasDbContext saasCtx = new SaasDbContext())
                {
                    if (User.IsInRole("SystemAdministrator"))
                    {
                        ViewBag.TenantId = new SelectList(saasCtx.Tenants.ToList(), "TenantId", "Name");
                        ViewBag.ApplicationId = new SelectList(saasCtx.Applications.ToList(), "Id", "Name");
                    }
                    else
                    {
                        ViewBag.TenantId = new SelectList(saasCtx.Tenants.Where(t => t.TenantId == CurrentTenant.TenantId).ToList(), "TenantId", "Name");
                        ViewBag.ApplicationId = new SelectList(saasCtx.Applications.Where(at => at.TenantId == CurrentTenant.TenantId).ToList(), "Id", "Name");
                    }

                    // Create the breadcrumb
                    var breadcrumb = new List<BreadcrumbItemViewModel>();
                    breadcrumb.Add(new BreadcrumbItemViewModel()
                    {
                        Text = "Collections",
                        Link = "/Collections"
                    });

                    breadcrumb.Add(new BreadcrumbItemViewModel()
                    {
                        Text = "New"
                    });

                    ViewBag.Breadcrumb = breadcrumb;
                }

            }
            catch (Exception ex)
            {
                LogService.Info("Collections not found", ex.Message, ex.InnerException);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Collections/Create")]
        public ActionResult Create([Bind(Include = "CollectionId,TenantId,ApplicationId,Name,Created,Status")] JobCollection collection)
        {
            try
            {
                using (SchedulerDbContext ctx = new SchedulerDbContext())
                {
                    if (User.IsInRole("SystemAdministrator") || collection.TenantId == CurrentTenant.TenantId)
                    {
                        if (ModelState.IsValid)
                        {
                            ctx.Collections.Add(collection);
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
                LogService.Info("Collections not found", ex.Message, ex.InnerException);
            }

            using (var saasDb = new SaasDbContext())
            {
                if (User.IsInRole("SystemAdministrator"))
                {
                    ViewBag.TenantId = new SelectList(saasDb.Tenants.ToList(), "TenantId", "Name");
                    ViewBag.ApplicationId = new SelectList(saasDb.Applications.ToList(), "Id", "Name");
                }
                else
                {
                    ViewBag.TenantId = new SelectList(saasDb.Tenants.Where(i => i.TenantId == CurrentTenant.TenantId).ToList(), "TenantId", "Name");
                    ViewBag.ApplicationId = new SelectList(saasDb.Applications.Where(a => a.TenantId == CurrentTenant.TenantId).ToList(), "Id", "Name");
                }
            }

            return View(collection);
        }

        [Route("Collections/{id}/Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Create the model
            var model = new EditJobCollectionViewModel();

            var collection = new JobCollection();

            try
            {
                using (var ctx = new SchedulerDbContext())
                {
                    //Get collection from db
                    collection = ctx.Collections.Find(id);

                    if (collection == null)
                    {
                        return HttpNotFound();
                    }

                    using (var saaSctx = new SaasDbContext())
                    {

                        ViewBag.TenantId = new SelectList(saaSctx.Tenants.ToList(), "TenantId", "Name");
                        ViewBag.ApplicationId = new SelectList(saaSctx.Applications.ToList(), "Id", "Name");

                        //Add collection to model
                        model.ApplicationId = collection.ApplicationId;
                        model.CollectionId = collection.CollectionId;
                        model.Name = collection.Name;
                        model.Created = collection.Created;
                        model.Updated = collection.Updated;
                        model.Status = collection.Status;

                        if (User.IsInRole("SystemAdministrator") || collection.TenantId == CurrentTenant.TenantId)
                        {
                            model.TenantId = collection.TenantId;
                        }
                        else
                        {
                            return View("Authorize");
                        }
                    }
                }

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Collections",
                    Link = "/Collections"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = collection.Name,
                    Link = "/Collections/" + id + "/Details"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Edit"
                });

                ViewBag.Breadcrumb = breadcrumb;

            }

            catch (Exception ex)
            {
                LogService.Info("Collections not found", ex.Message, ex.InnerException);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Collections/{id}/Edit")]
        public ActionResult Edit([Bind(Include = "CollectionId,ApplicationId,TenantId,Name,Created,Status")] EditJobCollectionViewModel collectionViewModel)
        {
            try
            {
                if (User.IsInRole("SystemAdministrator") || collectionViewModel.TenantId == CurrentTenant.TenantId)
                {
                    using (var ctx = new SchedulerDbContext())
                    {
                        // Get existing collection from db
                        var collection = ctx.Collections.Where(c => c.CollectionId == collectionViewModel.CollectionId).FirstOrDefault();

                        collection.Name = collectionViewModel.Name;
                        collection.Status = collectionViewModel.Status;

                        if (ModelState.IsValid)
                        {
                            //Update collection
                            ctx.Entry(collection).State = EntityState.Modified;
                            ctx.SaveChanges();
                            return RedirectToAction("Index");
                        }

                    }
                }
                else
                {
                    return View("Authorize");
                }

                using (var saaSctx = new SaasDbContext())
                {
                    ViewBag.TenantId = new SelectList(saaSctx.Tenants.ToList(), "TenantId", "Name");
                    ViewBag.ApplicationId = new SelectList(saaSctx.Applications.ToList(), "Id", "Name");
                }

            }

            catch (Exception ex)
            {
                LogService.Info("Collections not found", ex.Message, ex.InnerException);
            }

            return View(collectionViewModel);
        }

        [Route("Collections/{id}/Delete")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Create the model
            var model = new JobCollectionViewModel();

            var collection = new JobCollection();

            try
            {
                using (var ctx = new SchedulerDbContext())
                {
                    // Get the collection db
                    collection = ctx.Collections.Where(i => i.CollectionId == id).Include("Tenant").Include("Application").FirstOrDefault();

                    if (User.IsInRole("SystemAdministrator") || collection.TenantId == CurrentTenant.TenantId)
                    {
                        //Add collection to model
                        model.Collection = collection;
                    }
                    else
                    {
                        return View("Authorize");
                    }
                }

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Collections",
                    Link = "/Collections"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = collection.Name,
                    Link = "/Collections/" + id + "/Details"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Delete Confirm"
                });

                ViewBag.Breadcrumb = breadcrumb;
            }

            catch (Exception ex)
            {
                LogService.Info("Collections not found", ex.Message, ex.InnerException);
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Collections/{id}/Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (var ctx = new SchedulerDbContext())
                {
                    //Get collection from db
                    JobCollection collection = ctx.Collections.Find(id);

                    if (User.IsInRole("SystemAdministrator") || collection.TenantId == CurrentTenant.TenantId)
                    {
                        //Remove collection from db
                        ctx.Collections.Remove(collection);
                        ctx.SaveChanges();
                    }
                }
            }

            catch (Exception ex)
            {
                LogService.Info("Collections not found", ex.Message, ex.InnerException);
            }

            return RedirectToAction("Index");
        }

    }
}
