using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SMP_WebApplication.Startup))]
namespace SMP_WebApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
