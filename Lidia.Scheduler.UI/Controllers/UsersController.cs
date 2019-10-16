using Lidia.Framework.Logging;
using Lidia.Framework.SaaS.Interfaces;
using Lidia.Framework.SaaS.Models;
using Lidia.Framework.SaaS.Models.Identity;
using Lidia.Scheduler.Identity;
using Lidia.Scheduler.UI.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Lidia.Scheduler.UI.Controllers
{
    public class UsersController : BaseController
    {

        private SchedulerUserManager _userManager;

        public SchedulerUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<SchedulerUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public UsersController(ITenantService tenantService, IUserService userService) : base(tenantService, userService) { }
        // GET: Users
        public ActionResult Index()
        {
            //Create the model
            var model = new UserListViewModel();

            try
            {
                // Get the users from the database
                var users = new List<LidiaUser>();
                if (User.IsInRole("SystemAdministrator"))
                {
                    using (var ctx = new SaasDbContext())
                    {
                        users = ctx.Users.ToList();
                    }
                }
                else
                {
                    using (var ctx = new SaasDbContext())
                    {
                        users = ctx.UserRoles.Where(ur => ur.TenantId == CurrentTenant.TenantId).Select(ur => ur.User).ToList();
                    }
                }

                // Set the user collection into the model
                model.Users = users.Select(u => new UserViewModel()
                {
                    User = u
                }).ToList();

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Users"
                });

                ViewBag.Breadcrumb = breadcrumb;
            }

            catch (Exception ex)
            {
                LogService.Info("Users not found", ex.Message, ex.InnerException);
            }

            return View(model);
        }

        [Route("Users/Create")]
        public ActionResult Create()
        {
            try
            {
                using (var ctx = new SaasDbContext())
                {
                    if (User.IsInRole("SystemAdministrator"))
                    {
                        ViewBag.TenantId = new SelectList(ctx.Tenants.ToList(), "TenantId", "Name");
                        ViewBag.Id = new SelectList(ctx.Roles.ToList(), "Id", "Name");
                    }
                    else if (User.IsInRole("TenantAdministrator"))
                    {
                        ViewBag.TenantId = new SelectList(ctx.Tenants.Where(t => t.TenantId == CurrentTenant.TenantId).ToList(), "TenantId", "Name");
                        ViewBag.Id = new SelectList(ctx.Roles.Where(t => t.Id != 3).ToList(), "Id", "Name");
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
                    Text = "Users",
                    Link = "/Users"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "New"
                });

                ViewBag.Breadcrumb = breadcrumb;

            }

            catch (Exception ex)
            {
                LogService.Info("Users create error", ex.Message, ex.InnerException);
            }
            return View();
        }


        [Route("Users/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "RoleId,TenantId,Firstname,LastName,Username,Email,Password,MobileNumber,Gender,Created")] CreateUserViewModel user)
        {

            try
            {
                //Create the LidiaUser
                var newUser = new LidiaUser()
                {
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    Email = user.Email,
                    Gender = user.Gender,
                    MobileNumber = user.MobileNumber,
                    UserName = user.Email
                };


                using (var ctx = new SaasDbContext())
                {
                    if (User.IsInRole("SystemAdministrator") || user.TenantId == CurrentTenant.TenantId)
                    {
                        var result = await UserManager.CreateAsync(newUser, user.Password);

                        if (result.Succeeded)
                        {
                            //Add user role
                            var userRole = new LidiaUserRole()
                            {
                                RoleId = user.RoleId,
                                TenantId = user.TenantId,
                                UserId = newUser.Id
                            };
                            ctx.UserRoles.Add(userRole);
                            ctx.SaveChanges();
                            return RedirectToAction("/Index");

                        }
                    }


                    if (User.IsInRole("SystemAdministrator"))
                    {
                        ViewBag.TenantId = new SelectList(ctx.Tenants.ToList(), "TenantId", "Name");
                        ViewBag.Id = new SelectList(ctx.Roles.ToList(), "Id", "Name");
                    }
                    else if (User.IsInRole("TenantAdministrator"))
                    {
                        ViewBag.TenantId = new SelectList(ctx.Tenants.Where(t => t.TenantId == CurrentTenant.TenantId).ToList(), "TenantId", "Name");
                        ViewBag.Id = new SelectList(ctx.Roles.Where(t => t.Id != 3).ToList(), "Id", "Name");
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

            return View(user);
        }


        [Route("Users/{id}/Edit")]
        public ActionResult Edit(int? id)
        {
            //Create the model
            var model = new EditUserViewModel();

            // Create the user
            var user = new LidiaUser();

            var userRole = new LidiaUserRole();


            try
            {
                using (var ctx = new SaasDbContext())
                {
                    //Get user from db
                    user = ctx.Users.Where(u => u.Id == id).Include("Roles").FirstOrDefault();
                    //Get user role from db
                    userRole = ctx.UserRoles.Where(s => s.UserId == user.Id).FirstOrDefault();

                    if (User.IsInRole("SystemAdministrator"))
                    {
                        model.Firstname = user.Firstname;
                        model.Lastname = user.Lastname;
                        model.Email = user.Email;
                        model.MobileNumber = user.MobileNumber;
                        model.Gender = user.Gender;
                        model.UserId = user.Id;
                        model.Id = userRole.RoleId;
                        model.TenantId = userRole.TenantId;
                        model.PasswordHash = user.PasswordHash;
                        model.Birthdate = user.Birthdate;
                        model.SecurityStamp = user.SecurityStamp;

                        //Add user to model
                        ViewBag.Id = new SelectList(ctx.Roles.ToList(), "Id", "Name");
                    }
                    else if (CurrentTenant.TenantId == userRole.TenantId && User.IsInRole("TenantAdministrator"))
                    {
                        model.Firstname = user.Firstname;
                        model.Lastname = user.Lastname;
                        model.Email = user.Email;
                        model.MobileNumber = user.MobileNumber;
                        model.Gender = user.Gender;
                        model.UserId = user.Id;
                        model.Id = userRole.RoleId;
                        model.TenantId = userRole.TenantId;
                        model.PasswordHash = user.PasswordHash;
                        model.Birthdate = user.Birthdate;
                        model.SecurityStamp = user.SecurityStamp;

                        ViewBag.Id = new SelectList(ctx.Roles.Where(r => r.Id != 3).ToList(), "Id", "Name");
                    }
                    else if (CurrentUser.Id == user.Id)
                    {

                        model.Firstname = user.Firstname;
                        model.Lastname = user.Lastname;
                        model.Email = user.Email;
                        model.MobileNumber = user.MobileNumber;
                        model.Gender = user.Gender;
                        model.UserId = user.Id;
                        model.Id = userRole.RoleId;
                        model.TenantId = userRole.TenantId;
                        model.PasswordHash = user.PasswordHash;
                        model.Birthdate = user.Birthdate;
                        model.SecurityStamp = user.SecurityStamp;

                        ViewBag.Id = new SelectList(ctx.Roles.Where(r => r.Id == 1).ToList(), "Id", "Name");
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
                    Text = "Users",
                    Link = "/Users"
                });


                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = user.Firstname + " " + user.Lastname,
                    Link = "/Users/" + user.Id + "/Details"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Edit"
                });

                ViewBag.Breadcrumb = breadcrumb;

                // Add currentUserId to model 
                ViewBag.CurrentUser = CurrentUser.Id;
                ViewBag.CurrentTenant = CurrentTenant.TenantId;

                if (model != null)
                {
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Home/Index");
                }
            }
            catch (Exception ex)
            {
                LogService.Info("User not found to edit", ex.Message, ex.InnerException);
                return RedirectToAction("Home/Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Users/{id}/Edit")]
        public ActionResult Edit([Bind(Include = "Id,UserId,TenantId,Firstname,LastName,Username,Email,PasswordHash,SecurityStamp,MobileNumber,Gender,Birthdate,Created")] EditUserViewModel userviewmodel)
        {
            try
            {
                using (var ctx = new SaasDbContext())
                {
                    // Get existing user from db
                    var user = ctx.Users.Where(u => u.Id == userviewmodel.UserId).FirstOrDefault();

                    user.Firstname = userviewmodel.Firstname;
                    user.Lastname = userviewmodel.Lastname;
                    user.Email = userviewmodel.Email;
                    user.UserName = userviewmodel.Email;
                    user.Gender = userviewmodel.Gender;
                    user.MobileNumber = userviewmodel.MobileNumber;

                    //Get the user role from db
                    var userRoleold = ctx.UserRoles.Where(ui => ui.UserId == userviewmodel.UserId).FirstOrDefault();
                    //Remove user role from db
                    ctx.UserRoles.Remove(userRoleold);
                    ctx.SaveChanges();


                    //Create a new user role 
                    var modelRole = new LidiaUserRole()
                    {
                        UserId = userviewmodel.UserId,
                        RoleId = userviewmodel.Id,
                        TenantId = userviewmodel.TenantId
                    };

                    if (userviewmodel.UserId == CurrentUser.Id || User.IsInRole("SystemAdministrator") || User.IsInRole("TenantAdministrator"))
                    {
                        if (ModelState.IsValid)
                        {
                            //Update the user 
                            ctx.Entry(user).State = EntityState.Modified;
                            ctx.UserRoles.Add(modelRole);
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

            return View(userviewmodel);
        }

        [Route("Users/{id}/Delete")]
        public ActionResult Delete(int id)
        {
            //Create the model
            var model = new UserViewModel();

            //Create the user
            var user = new LidiaUser();

            try
            {
                using (var ctx = new SaasDbContext())
                {
                    // Get the users from the database
                    user = ctx.Users.Include("Roles").Where(u => u.Id == id).FirstOrDefault();

                    //Get tenant id from db
                    var tenantid = ctx.UserRoles.Where(t => t.UserId == id).FirstOrDefault().TenantId;

                    if (User.IsInRole("SystemAdministrator"))
                    {
                        //Add user to model
                        model.User = user;
                    }
                    else if (User.IsInRole("TenantAdministrator") && CurrentTenant.TenantId == tenantid)
                    {
                        //Add user to model
                        model.User = user;
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
                    Text = "Users",
                    Link = "/Users"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = user.Firstname + " " + user.Lastname,
                    Link = "/Users/" + user.Id + "/Details"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Delete Confirm"
                });

                ViewBag.Breadcrumb = breadcrumb;


                if (model.User != null)
                {
                    return View(model);
                }
                else
                {
                    return View("Authorize");
                }
            }
            catch (Exception ex)
            {
                LogService.Info("User not found to delete");
                return RedirectToAction("Home/Index", ex.Message, ex.InnerException);
            }

        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Users/{id}/Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            //Create the user
            var user = new LidiaUser();
            try
            {
                using (var ctx = new SaasDbContext())
                {
                    //Get the user from db
                    user = ctx.Users.Where(u => u.Id == id).FirstOrDefault();

                    //Get user's tenant from db
                    var tenantId = user.Roles.Where(tı => tı.UserId == id).FirstOrDefault().TenantId;


                    if (User.IsInRole("SystemAdministrator"))
                    {
                        // Delete user from db
                        ctx.Users.Remove(user);
                        ctx.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else if (User.IsInRole("TenantAdministrator") && CurrentTenant.TenantId == tenantId)
                    {
                        // Delete user from db
                        ctx.Users.Remove(user);
                        ctx.SaveChanges();
                        return RedirectToAction("Index");
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

        [Route("Users/{id}/Details")]
        public ActionResult Details(int id)
        {
            //Create the model
            var model = new UserViewModel();

            //Create the user
            var user = new LidiaUser();

            try
            {
                using (var ctx = new SaasDbContext())
                {
                    // Get the user from the database
                    user = ctx.Users.Include("Roles").Where(u => u.Id == id).FirstOrDefault();

                    //Get user's tenant from db
                    var tenantId = user.Roles.Where(tı => tı.UserId == id).FirstOrDefault().TenantId;

                    if (User.IsInRole("SystemAdministrator"))
                    {
                        //Add user to model
                        model.User = user;
                    }
                    else if (User.IsInRole("TenantAdministrator") && CurrentTenant.TenantId == tenantId)
                    {
                        //Add user to model
                        model.User = user;
                    }
                    else if (id == CurrentUser.Id)
                    {
                        //Add user to model
                        model.User = user;
                    }
                    else
                    {
                        return View("Authorize");
                    }
                }

                // Create the breadcrumb 
                var breadcrumb = new List<BreadcrumbItemViewModel>();

                // Set breadcrumb item 
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Users",
                    Link = "/Users"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = user.Firstname + " " + user.Lastname,
                    Link = "/Users/" + user.Id + "/Details"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Details"
                });

                //set breadcrumb item to Viewbag
                ViewBag.Breadcrumb = breadcrumb;

            }

            catch (Exception ex)
            {
                LogService.Info("User not found", ex.Message, ex.InnerException);
            }


            return View(model);
        }

        #region [Password]

        [Route("Users/{id}/ChangePassword")]
        public ActionResult ChangePassword(int id)
        {
            var model = new ChangePasswordViewModel();
            try
            {
                using (var ctx = new SaasDbContext())
                {
                    //Get user from db
                    var user = ctx.Users.Where(u => u.Id == id).FirstOrDefault();

                    //Get tenant ıd from db
                    var userRole = ctx.UserRoles.Where(ur => ur.UserId == id).FirstOrDefault().TenantId;


                    //Add user to model
                    if (CurrentUser.Id == id)
                    {
                        model.User = user;
                        ViewBag.TenantId = new SelectList(ctx.Tenants.ToList(), "TenantId", "Name");
                    }
                    else if (userRole == CurrentTenant.TenantId && User.IsInRole("TenantAdministrator"))
                    {
                        return View("ResetPasswordMail");
                    }
                    else if (User.IsInRole("SystemAdministrator"))
                    {
                        return View("ResetPasswordMail");
                    }
                    else
                    {
                        return View("Authorize");
                    }
                }
            }

            catch (Exception ex)
            {
                LogService.Info("User not found", ex.Message, ex.InnerException);
            }

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Users",
                Link = "/Users"
            });

            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = model.User.Firstname + " " + model.User.Lastname,
                Link = "/Users/" + model.User.Id + "/Details"
            });

            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Change Password",
            });

            ViewBag.Breadcrumb = breadcrumb;

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Users/{id}/ChangePassword")]
        public ActionResult ChangePassword([Bind(Include = "Id,Password,NewPassword,NewPasswordConfirm,Created")] ChangePasswordViewModel model, int id)
        {
            try
            {
                using (var ctx = new SaasDbContext())
                {
                    //Check the user role
                    if (id == CurrentUser.Id)
                    {
                        if (model.NewPassword == model.NewPasswordConfirm)
                        {
                            //Change password
                            UserManager.ChangePassword(id, model.Password, model.NewPassword);
                            LogService.Info("User password changed", model.User.Id.ToString(), model.User.Firstname + "" + model.User.Lastname);
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
                LogService.Info("User not found", ex.Message, ex.InnerException);
            }

            return View(model);
        }

        #endregion
    }
}