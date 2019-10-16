using Lidia.Framework.SaaS.Models.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Lidia.Scheduler.UI.Models.ViewModels
{
    public class CreateUserViewModel : BaseViewModel
    {
        [Display(Name = "Firstname")]
        public string Firstname { get; set; }

        [Display(Name = "Lastname")]
        public string Lastname { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string MobileNumber { get; set; }
        
        public string Gender { get; set; }

        public DateTime Birthdate { get; set; }

        public LidiaUserRole UserRole { get; set; }

        [Display(Name = "TenantId")]
        public int TenantId { get; set; }

        [Display(Name = "RoleId")]
        public int RoleId { get; set; }

        [Display(Name = "Id")]
        public int Id { get; set; }
    }
}