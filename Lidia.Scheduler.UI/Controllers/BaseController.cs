using Lidia.Framework.Models;
using Lidia.Framework.SaaS.Identity;
using Lidia.Framework.SaaS.Interfaces;
using Lidia.Framework.SaaS.Models;
using Lidia.Framework.SaaS.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Lidia.Scheduler.UI.Controllers
{
    public class BaseController : Controller
    {
        protected ITenantService tenantService;
        protected IUserService userService;

        #region [ Tenant properties ]

        public int TenantId { get; set; } = 0;

        public Tenant CurrentTenant { get; set; }

        public Application CurrentApplication { get; set; }

        #endregion

        #region [ User and session properties ]

        public LidiaUser CurrentUser { get; set; }

        public List<LidiaUserRole> UserRoles { get; set; } = new List<LidiaUserRole>();

        public string UserToken { get; set; }

        public int UserId
        {
            get
            {
                return User.Identity.IsAuthenticated ? User.Identity.GetUserId<int>() : 0;
            }
        }

        public string SessionId
        {
            get { return Session.SessionID; }
        }

        #endregion

        #region [ Culture ]

        public string CurrentCulture { get; private set; }

        #endregion

        static BaseController() { }

        public BaseController(ITenantService tenantService, IUserService userService)
        {
            // Set the core services
            this.tenantService = tenantService;
            this.userService = userService;

            // Set the current culture
            CurrentCulture = "tr";
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            #region [ User ]

            // Try to get user information from the session
            var user = Session["User"] as LidiaUser;

            if (user == null)
            {
                // Get the contact information from the related service
                user = HttpContext.GetOwinContext().GetUserManager<LidiaUserManager>()
                        .FindById(User.Identity.GetUserId<int>());

                // Store contact data in the session
                Session["User"] = user;
            }

            // Set the user
            CurrentUser = user;

            #endregion

            #region [ User roles ]

            if (CurrentUser != null)
            {
                // Try to get user information from the session
                var userRoles = CurrentUser.Roles;

                if (userRoles == null || userRoles.Count == 0)
                {
                    // Get the role information from the related service
                    userRoles = userService.GetRoles(user.Id);

                    // Set the roles in to the current user
                    foreach (var userRole in userRoles)
                    {
                        CurrentUser.Roles.Add(userRole);
                    }

                    // Re-Store user data in the session
                    Session["User"] = CurrentUser;
                }

                // Set user roles
                UserRoles = CurrentUser.Roles.ToList();
            }

            #endregion

            #region [ Tenant and Application ]

            if(CurrentUser != null)
            {
                // Try to get tenant information from the session
                var tenant = Session["Tenant"] as Tenant;

                if (tenant == null)
                {
                    // Get the tenant information from the related service
                    tenant = tenantService.GetTenantById(UserRoles.FirstOrDefault().TenantId.GetValueOrDefault());

                    // Store contact data in the session
                    Session["Tenant"] = tenant;
                }

                // Set tenant information
                CurrentTenant = tenant;

                // Try to get application information from the session
                var application = Session["Application"] as Application;

                if (application == null)
                {
                    // Get the first application of the tenant
                    application = CurrentTenant.Applications.FirstOrDefault(a => a.Status == EntityStatus.Active);

                    // Store contact data in the session
                    Session["Application"] = application;
                }

                // Set application information
                CurrentApplication = application;

                //if (User.IsInRole("TenantAdministrator") || User.IsInRole("TenantUser"))
                //{

                //}
            }

            #endregion

            #region [ User token ]

            // Get the user token
            var userToken = string.Empty;

            // Try to get the user token
            var tokenCookie = requestContext.HttpContext.Request.Cookies["p.token"];

            if (tokenCookie == null)
            {
                // Get the token from the identity server
                userToken = HttpContext.GetOwinContext().GetUserManager<LidiaUserManager>().GetUserGuid();

                // Set the cookie
                tokenCookie = new HttpCookie("p.token", userToken);
                tokenCookie.Expires = DateTime.MaxValue;
                requestContext.HttpContext.Response.Cookies.Add(tokenCookie);
            }
            else
            {
                // Get the token from the 
                userToken = tokenCookie.Value;
            }

            // Set the token
            UserToken = userToken;
            ViewBag.UserToken = userToken;

            #endregion

            #region [ User Id ]

            // Set the user id
            ViewBag.UserId = UserId;

            #endregion

            #region [ Session Id ]

            // Set the session id
            ViewBag.SessionId = SessionId;

            #endregion
        }

        #region [ Override JsonResult ]

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        #endregion

        #region [ Helper methods ]

        protected string ViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        #endregion
    }

    #region [ Json serialization corrections ]

    public class JsonNetResult : JsonResult
    {
        public JsonNetResult()
        {
            Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        public JsonSerializerSettings Settings { get; private set; }

        public override void ExecuteResult(ControllerContext context)
        {
            // Check for a valid contxt
            if (context == null)
                throw new ArgumentNullException("context");
            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("JSON GET is not allowed");

            // Get the response object
            HttpResponseBase response = context.HttpContext.Response;

            // Check for a redirect request
            if (!String.IsNullOrEmpty(response.RedirectLocation)) return;

            // Set the content type
            response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;

            // Set the encoding
            if (this.ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;
            if (this.Data == null)
                return;

            // Set the serializer and writer
            var scriptSerializer = JsonSerializer.Create(this.Settings);
            using (var sw = new StringWriter())
            {
                scriptSerializer.Serialize(sw, this.Data);
                response.Write(sw.ToString());
            }
        }
    }

    #endregion
}