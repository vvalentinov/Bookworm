namespace Bookworm.Web.Areas.Admin.Controllers
{
    using Bookworm.Common;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Area("Admin")]
    public class BaseController : Controller
    {
    }
}
