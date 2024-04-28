namespace Bookworm.Web.Areas.Account.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class ManageController : BaseController
    {
        public IActionResult Index() => this.View();

        public IActionResult ChangePassword() => this.View();
    }
}
