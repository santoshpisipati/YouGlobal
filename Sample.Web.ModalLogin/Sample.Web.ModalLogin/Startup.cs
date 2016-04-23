using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sample.Web.ModalLogin.Startup))]
namespace Sample.Web.ModalLogin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
