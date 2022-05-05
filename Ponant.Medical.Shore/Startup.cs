[assembly: Microsoft.Owin.OwinStartupAttribute(typeof(Ponant.Medical.Shore.Startup))]
namespace Ponant.Medical.Shore
{
    using Owin;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
