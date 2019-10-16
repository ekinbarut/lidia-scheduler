using Lidia.Scheduler.UI.Models.ViewModels;
using Lidia.Framework.Logging;
using Lidia.Framework.SaaS.Identity;
using Lidia.Framework.SaaS.Interfaces;
using Lidia.Framework.SaaS.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Lidia.Framework.SaaS.Models;

namespace Lidia.Scheduler.UI.Controllers
{
    public class IdentityController : BaseController
    {
        private LidiaSignInManager _signInManager;
        private LidiaUserManager _userManager;

        public LidiaSignInManager SignInManager
        {
            get
            {

                return _signInManager ?? HttpContext.GetOwinContext().Get<LidiaSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public LidiaUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<LidiaUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public IdentityController(ITenantService tenantService, IUserService userService) : base(tenantService, userService) { }

        #region [ Registration ]

        [Route("register")]
        public ActionResult Register()
        {
            // Add debug log
            LogService.Debug($"Getting the registration page. SessionId:{SessionId}");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("register")]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            // Add debug log
            LogService.Debug($"Getting the registration page (HTTP_POST). SessionId:{SessionId}");

            if (ModelState.IsValid)
            {
                // Create the user object
                var user = new LidiaUser { UserName = model.Email, Email = model.Email };

                // Create the user at the user store
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // Add debug log
                    LogService.Debug($"User registration complete, now redirecting to home page. SessionId:{SessionId}");

                    return RedirectToAction("Index", "Home");
                }

                AddErrors(result);
            }

            // Add debug log
            LogService.Debug($"User registration failed, re-opening the registration page. SessionId:{SessionId}");

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region [ Login and logout ]

        [AllowAnonymous]
        [Route("login")]
        public ActionResult Login(string returnUrl)
        {
            // Add debug log
            LogService.Debug($"Getting the login page. SessionId:{SessionId}");

            // Create the model
            var model = new LoginViewModel()
            {
            };

            // Set the return url into the viewbag
            ViewBag.ReturnUrl = returnUrl;

            //LogService.Debug("Login view model created.", "LOGIN");

            return View(model);
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        [Route("login")]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            // Add debug log
            LogService.Debug($"Getting the login page (HTTP_POST). SessionId:{SessionId}");

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.Remember, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:

                    // Return success
                    return RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.Remember });

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid credentials!");

                    return View(model);
            }
        }

        [Route("logoff")]
        public ActionResult LogOff()
        {
            // Add debug log
            LogService.Debug($"Getting the logoff page. SessionId:{SessionId}");

            AuthenticationManager.SignOut();

            // Clear the session values
            Session.Clear();

            return Redirect("/");
        }

        #endregion

        #region [ Password ]
        [Route("forgot-password/{id?}")]
        [AllowAnonymous]
        public ActionResult ForgotPassword(int ?id)
        {
            if (id != null)
            {
                using (var ctx = new SaasDbContext())
                {
                    // Get the user from db
                    var user = ctx.Users.Where(u => u.Id == id).FirstOrDefault();

                    var tenantid = ctx.UserRoles.Where(t => t.UserId == id).FirstOrDefault().TenantId;
                    
                    if(CurrentTenant.TenantId==tenantid && User.IsInRole("TenantAdministrator"))
                    {
                        ViewBag.Email = user.Email;
                    }
                    else if (User.IsInRole("SystemAdministrator"))
                    {
                        ViewBag.Email = user.Email;
                    }
                }
            }
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("forgot-password")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "E-mail address did not find.");
                    return View();
                }

                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Identity", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                #region Forgot Password Mail

                SchedulerMailService schedulerMailService = new SchedulerMailService();

                // Send reset password mail
                schedulerMailService.SendNewPasswordMail(callbackUrl, user.Id, user.Email);

                #endregion
                ViewBag.Link = callbackUrl;
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        [Route("forgot-PasswordConfirmation")]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
    

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        [Route("ResetPassword")]
        public ActionResult ResetPassword(int? userId, string code)
        {

            if (code == null)
            {
                return View("Error");
            }
            var model = new ResetPasswordViewModel();
            if (userId.HasValue)
            {
                var user = UserManager.FindById(userId.Value);
                model = new ResetPasswordViewModel
                {
                    Email = user.Email,
                    Code = code
                };
            }
            return View(model);

        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("ResetPassword")]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {

            var temp = HttpContext.Request.Url.Authority;
            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;

                return Json(new IdentityResult("Password is not correct format"), JsonRequestBehavior.AllowGet);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new IdentityResult("Error !"), JsonRequestBehavior.AllowGet);
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                  return RedirectToAction("ResetPasswordConfirmation");
                //return Json(new { Succeeded = true, redirectUrl = HttpContext.Request.Url.Authority + Url.Action("ResetPasswordConfirmation") }, JsonRequestBehavior.AllowGet);
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        [Route("ResetPasswordConfirmation")]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #endregion

        #region [ Helper methods ]

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            //return RedirectToAction("Index", "Home");
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        
        #endregion
    }
}