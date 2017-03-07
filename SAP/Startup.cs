using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SAP.Startup))]
namespace SAP
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
