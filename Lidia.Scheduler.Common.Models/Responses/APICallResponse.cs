using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lidia.Scheduler.Common.Models.Responses
{
    public class APICallResponse<T>
    {
        public APICallResponse()
        {
            PreProcessingTook = 0;
            ServiceTook = 0;
            TotalTook = 0;
            Result = new List<T>();
            Errors = new List<string>();
        }

        public List<T> Result { get; set; }

        public long PreProcessingTook { get; set; }

        public long ServiceTook { get; set; }

        public long TotalTook { get; set; }

        public ServiceResponseTypes Type { get; set; }

        public string Code { get; set; }

        public string Message { get; set; }

        public string Details { get; set; }

        public int ResultCount
        {
            get
            {
                return this.Result != null ? this.Result.Count() : 0;
            }
        }

        public List<string> Errors { get; set; }

        public List<APICallLogRecord> LogRecords { get; set; }

        public APICallResponse<T> InnerResponse { get; set; }
    }

    public class APICallResponse
    {
        public APICallResponse()
        {
            PreProcessingTook = 0;
            ServiceTook = 0;
            TotalTook = 0;
            Errors = new List<string>();
        }

        public long PreProcessingTook { get; set; }

        public long ServiceTook { get; set; }

        public long TotalTook { get; set; }

        public ServiceResponseTypes Type { get; set; }

        public string Code { get; set; }

        public string Message { get; set; }

        public string Details { get; set; }

        public List<string> Errors { get; set; }

        public List<APICallLogRecord> LogRecords { get; set; }

        public APICallResponse InnerResponse { get; set; }
    }
}
