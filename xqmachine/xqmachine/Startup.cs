using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(xqmachine.Startup))]
namespace xqmachine
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
