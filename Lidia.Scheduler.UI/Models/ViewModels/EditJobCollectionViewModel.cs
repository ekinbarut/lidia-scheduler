using Lidia.Scheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lidia.Scheduler.UI.Models.ViewModels
{
    public class JobCollectionViewModel : BaseViewModel
    {
        public JobCollection Collection { get; set; }
    }
}