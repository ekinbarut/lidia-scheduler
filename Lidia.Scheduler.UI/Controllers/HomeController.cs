using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lidia.Framework.SaaS.Interfaces;
using Lidia.Scheduler.UI.Models.ViewModels;

namespace Lidia.Scheduler.UI.Controllers
{
    [Authorize(Roles = "SystemAdministrator,TenantAdministrator,TenantUser")]
    public class HomeController : BaseController
    {
        public HomeController(ITenantService tenantService, IUserService userService) : base(tenantService, userService) { }

        public ActionResult Index()
        {
            var model = new DashboardViewModel()
            {
                CurrentCulture = CurrentCulture,
                CurrentUser = CurrentUser,
                CurrentTenant = CurrentTenant,
                CurrentApplication = CurrentApplication
            };

            return View(model);
        }        
    }
}