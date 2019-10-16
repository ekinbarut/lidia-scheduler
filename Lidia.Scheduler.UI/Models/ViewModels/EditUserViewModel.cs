using Lidia.Framework.SaaS.Models.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Lidia.Scheduler.UI.Models.ViewModels
{
    public class EditUserViewModel : BaseViewModel
    {
        [Display(Name = "Firstname")]
        public string Firstname { get; set; }

        [Display(Name = "Lastname")]
        public string Lastname { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        public string MobileNumber { get; set; }
        
        public string Gender { get; set; }

        public DateTime ?Birthdate { get; set; }
        

        [Display(Name = "TenantId")]
        public int ?TenantId { get; set; }

        [Display(Name = "uSERId")]
        public int UserId { get; set; }

        [Display(Name = "Id")]
        public int Id { get; set; }

        public string PasswordHash { get; set; }

        public string SecurityStamp { get; set; }
    }
}