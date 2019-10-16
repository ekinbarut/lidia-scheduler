using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lidia.Scheduler.Common.Models.Responses
{
    public class APICallLogRecord
    {
        public string Type { get; set; }

        public string Body { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
