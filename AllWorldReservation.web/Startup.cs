using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AllWorldReservation.web.Startup))]
namespace AllWorldReservation.web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
