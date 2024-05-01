namespace Bookworm.Web.Controllers.Identity
{
    using Microsoft.AspNetCore.Mvc;

    using static Bookworm.Common.Constants.GlobalConstants;

    [Route($"{AccountAreaName}/[action]")]
    public abstract class BaseIdentityController : BaseController
    {
    }
}
