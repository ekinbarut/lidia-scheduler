using Lidia.Framework.Data.Channels;
using Lidia.Scheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lidia.Scheduler.DataAccess
{
    public class JobLogEntryDAO : EFDataChannel<JobLogEntry, SchedulerDbContext>
    {
    }
}
