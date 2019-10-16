using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lidia.Framework.SaaS.Interfaces;
using Lidia.Scheduler.UI.Models.ViewModels;

namespace Lidia.Scheduler.UI.Controllers
{
    public class CommonController : BaseController
    {
        public CommonController(ITenantService tenantService, IUserService userService) : base(tenantService, userService) { }

        public ActionResult ApplicationSelection()
        {
            var model = new ApplicationSelectionViewModel()
            {
                CurrentCulture = CurrentCulture,
                CurrentUser = CurrentUser,
                CurrentTenant = CurrentTenant,
                CurrentApplication = CurrentApplication,
            };

            return PartialView(model);
        }

        public ActionResult Header(List<BreadcrumbItemViewModel> breadcrumb = null)
        {
            var model = new HeaderViewModel()
            {
                CurrentCulture = CurrentCulture,
                CurrentUser = CurrentUser,
                CurrentTenant = CurrentTenant,
                CurrentApplication = CurrentApplication,
            };

            // Set the breadcrumb if not null
            if (breadcrumb != null)
            {
                model.Breadcrumb = breadcrumb;
            }

            return PartialView(model);
        }

        public ActionResult Sidebar()
        {
            return PartialView();
        }

        public ActionResult RightSidebar()
        {
            return PartialView();
        }

        public ActionResult Footer()
        {
            return PartialView();
        }

        #region [ Service methods ]

        [Route("services/common/changeapplication")]
        public JsonResult ChangeApplication(int applicationId)
        {
            // Set selected application
            var selectedApplication = CurrentTenant.Applications.FirstOrDefault(a => a.Id == applicationId);

            if (selectedApplication != null)
            {
                Session["Application"] = selectedApplication;
            }

            return Json(new
            {
                ResponseCode = 200,
                CurrentApplicationId = applicationId
            });
        }

        #endregion
    }
}