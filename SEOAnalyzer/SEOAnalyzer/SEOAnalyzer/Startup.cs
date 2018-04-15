using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SEOAnalyzer.Startup))]
namespace SEOAnalyzer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
