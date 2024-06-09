namespace Bookworm.Web
{
    using Bookworm.Web.Extensions;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure(builder.Configuration);

            var app = builder.Build();

            app.Configure();

            app.Run();
        }
    }
}
