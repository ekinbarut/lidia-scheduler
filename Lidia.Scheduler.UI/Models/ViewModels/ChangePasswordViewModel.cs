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
    public class ChangePasswordViewModel: BaseViewModel
    {
        public LidiaUser User { get; set; }

        public string Password { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string NewPassword { get; set; }


        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "NewPassword")]
        [Compare("NewPassword",ErrorMessage = "Passwords is not match")]
        public string NewPasswordConfirm { get; set; }
        
    }
}