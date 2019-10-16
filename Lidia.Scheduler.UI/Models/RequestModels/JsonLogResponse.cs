using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lidia.Scheduler.UI.Models.RequestModels
{
    public class JsonLogResponse
    {
        public string PreProcessingTook { get; set; }
      
        public string ServiceTook { get; set; }

        public string TotalTook { get; set; }

        public string Type { get; set; }

        public string Source { get; set; }

        public string ResponseCode { get; set; }

        public string Message { get; set; }

        public string MoreInfo { get; set; }

        public string Token { get; set; }

        public string InnerResponse { get; set; }
    }
}