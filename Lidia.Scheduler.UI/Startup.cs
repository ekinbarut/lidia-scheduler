using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Lidia.Scheduler.UI.Startup))]

namespace Lidia.Scheduler.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            // Hangfire setup
            app.UseHangfireDashboard();
        }
    }
}
