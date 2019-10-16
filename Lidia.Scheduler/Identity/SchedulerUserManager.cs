using System;
using Lidia.Framework.SaaS.Models;
using Lidia.Framework.SaaS.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace Lidia.Scheduler.Identity
{
    public class SchedulerUserManager : UserManager<LidiaUser, int>
    {
        public SchedulerUserManager(IUserStore<LidiaUser, int> store) : base(store) { }

        public static SchedulerUserManager Create(IdentityFactoryOptions<SchedulerUserManager> options, IOwinContext context)
        {
            var manager = new SchedulerUserManager(new SchedulerUserStore(SaasDbContext.Create()));

            // Configure validation logic for usernames 
            manager.UserValidator = new UserValidator<LidiaUser, int>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords 
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            // Register two factor authentication providers. This application uses Phone 
            // and Emails as a step of receiving a code for verifying the user 
            // You can write your own provider and plug in here. 
            manager.RegisterTwoFactorProvider("PhoneCode",
                new PhoneNumberTokenProvider<LidiaUser, int>
                {
                    MessageFormat = "Your security code is: {0}"
                });
            manager.RegisterTwoFactorProvider("EmailCode",
                new EmailTokenProvider<LidiaUser, int>
                {
                    Subject = "Security Code",
                    BodyFormat = "Your security code is: {0}"
                });

            //manager.EmailService = new EmailService();
            //manager.SmsService = new SmsService();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<LidiaUser, int>(dataProtectionProvider.Create("FFIdentity"));
            }
            return manager;
        }

        public string GetUserGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
