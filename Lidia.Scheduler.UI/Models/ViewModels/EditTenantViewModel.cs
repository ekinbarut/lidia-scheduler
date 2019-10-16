using Lidia.Framework.Models;
using Lidia.Framework.SaaS.Models;
using Lidia.Scheduler.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lidia.Scheduler.UI.Models.ViewModels
{
    public class EditTenantViewModel : BaseViewModel
    {
        [Key]
        public int TenantId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Culture { get; set; }

        public string TimeZone { get; set; }
        
        public EntityStatus Status { get; set; }

        public DateTime Created { get; set; }

    }
}