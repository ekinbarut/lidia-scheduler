using Lidia.Framework.SaaS.Models;
using Lidia.Framework.SaaS.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lidia.Scheduler.UI.Models.ViewModels
{
    public class BaseViewModel
    {
        public LidiaUser CurrentUser { get; set; }

        public Tenant CurrentTenant { get; set; }

        public Application CurrentApplication { get; set; }

        public string CurrentCulture { get; set; }
    }
}