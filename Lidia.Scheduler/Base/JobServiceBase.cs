using Lidia.Framework.Data.Services;
using Lidia.Scheduler.DataAccess;
using Lidia.Scheduler.Models;

namespace Lidia.Scheduler.Base
{
    public class JobServiceBase : EFServiceBase<Job, JobDAO, SchedulerDbContext>
    {
    }
}
