namespace Bookworm.Web.Controllers
{
    using System.Diagnostics;

    using Bookworm.Web.ViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class ErrorController : BaseController
    {
        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult HandleError()
        {
            return this.View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier,
            });
        }

        [AllowAnonymous]
        [ActionName("404")]
        public IActionResult NotFound404() => this.View();
    }
}
