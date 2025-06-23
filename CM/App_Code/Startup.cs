using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CM.Startup))]
namespace CM
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
