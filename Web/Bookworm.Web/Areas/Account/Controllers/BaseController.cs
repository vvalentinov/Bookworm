namespace Bookworm.Web.Areas.Account.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using static Bookworm.Common.Constants.GlobalConstants;

    [Authorize]
    [Area(AccountAreaName)]
    public abstract class BaseController : Controller
    {
    }
}
