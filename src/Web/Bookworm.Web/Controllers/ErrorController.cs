namespace Bookworm.Web.Controllers
{
    using System.Diagnostics;

    using Bookworm.Web.ViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class ErrorController : BaseController
    {
        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult HandleError()
        {
            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier,
            };

            return this.View(model);
        }

        [AllowAnonymous]
        public IActionResult StatusCodeError(int id)
        {
            if (id == StatusCodes.Status404NotFound)
            {
                return this.View("NotFound");
            }

            return this.Content($"Error with status code: {id}", "text/plain");
        }
    }
}
