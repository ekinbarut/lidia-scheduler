using Lidia.Framework.SaaS.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lidia.Scheduler.UI.Models.ViewModels
{
    public class HeaderViewModel : BaseViewModel
    {
        public List<BreadcrumbItemViewModel> Breadcrumb { get; set; } = new List<BreadcrumbItemViewModel>();

    }
}