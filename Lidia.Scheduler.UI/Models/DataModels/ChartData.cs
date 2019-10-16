using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lidia.Scheduler.UI.Models.DataModels
{
    public class ChartData
    {
        public List<string> Categories { get; set; } = new List<string>();

        public List<decimal> Series { get; set; } = new List<decimal>();
    }
}