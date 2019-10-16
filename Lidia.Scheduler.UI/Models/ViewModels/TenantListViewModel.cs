using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lidia.Scheduler.UI.Models.ViewModels
{
    public class TenantListViewModel : BaseViewModel
    {
        public List<TenantViewModel> Tenants { get; set; }
    }
}