using Lidia.Framework.SaaS.Models;
using Lidia.Framework.SaaS.Models.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lidia.Scheduler.Identity
{
    public class SchedulerUserStore : UserStore<LidiaUser, LidiaRole, int, LidiaUserLogin, LidiaUserRole, LidiaUserClaim>
    {
        public SchedulerUserStore(SaasDbContext context) : base(context) { }
    }
}
