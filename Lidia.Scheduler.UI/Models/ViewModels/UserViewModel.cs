using Lidia.Framework.SaaS.Models;
using Lidia.Framework.SaaS.Models.Identity;
using Lidia.Scheduler.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Lidia.Scheduler.UI.Models.ViewModels
{
    public class UserViewModel: BaseViewModel
    {
        public LidiaUser User { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public LidiaUserRole UserRole { get; set; }
    }
}