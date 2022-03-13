namespace Bookworm.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class BookController : Controller
    {
        public IActionResult Upload()
        {
            return this.View();
        }
    }
}
