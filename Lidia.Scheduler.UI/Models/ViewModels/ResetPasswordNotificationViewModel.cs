using Lidia.Framework.SaaS.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lidia.Scheduler.UI.Models.ViewModels
{
    public class ResetPasswordNotificationViewModel
    {
        public LidiaUser User { get; set; }

        public string SiteRoot { get; set; }

        public string SecurityStamp { get; set; }

        public string ResetToken { get; set; }
    }
}