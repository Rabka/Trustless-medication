using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TrustlessClientWeb.Startup))]
namespace TrustlessClientWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
