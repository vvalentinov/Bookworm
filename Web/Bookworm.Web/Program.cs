namespace Bookworm.Web
{
    using Bookworm.Web.Extensions;
    using Microsoft.AspNetCore.Builder;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApplicationServices(builder.Configuration);

            var app = builder.Build();

            app.ConfigurePipeline();

            app.Run();
        }
    }
}
