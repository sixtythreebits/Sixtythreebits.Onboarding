using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin")]
    public class HomeControllers : AdminControllerBase<HomeModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.HomeController.Index)]
        public IActionResult Index()
        {
            return View(ViewNames.Admin.Home.IndexView);
        } 
        #endregion
    }
}