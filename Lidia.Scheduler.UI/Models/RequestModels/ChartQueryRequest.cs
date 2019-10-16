using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lidia.Scheduler.UI.Models.RequestModels
{
    public class ChartQueryRequest
    {
        public string Type { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}