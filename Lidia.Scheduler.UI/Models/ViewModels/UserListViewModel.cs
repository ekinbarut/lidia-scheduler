using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lidia.Scheduler.UI.Models.ViewModels
{
    public class UserListViewModel : BaseViewModel
    {
        public List<UserViewModel> Users { get; set; }
    }
}