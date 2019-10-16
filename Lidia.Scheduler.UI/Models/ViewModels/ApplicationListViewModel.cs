using Lidia.Framework.SaaS.Models;
using Lidia.Scheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lidia.Scheduler.UI.Models.ViewModels
{
    public class ApplicationListViewModel : BaseViewModel
    {
        public List<Application> Applications { get; set; }

        public List<JobCollection> Collections { get; set; }
    }
}