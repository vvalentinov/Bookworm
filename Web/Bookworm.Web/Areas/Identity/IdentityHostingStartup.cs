using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Bookworm.Web.Areas.Identity.IdentityHostingStartup))]

namespace Bookworm.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}