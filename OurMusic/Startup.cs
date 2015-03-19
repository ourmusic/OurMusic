using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OurMusic.Startup))]
namespace OurMusic
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}