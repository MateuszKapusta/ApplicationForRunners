using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ApplicationForRunnersService.Startup))]

namespace ApplicationForRunnersService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}