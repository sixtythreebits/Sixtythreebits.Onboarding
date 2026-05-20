using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Models.Website;

namespace SixtyThreeBits.Web.Controllers.Website
{
    public class HomeWebsiteController : WebsiteControllerBase<HomeModel>
    {
        #region Properties
        const string _viewName = "~/Views/Website/Home/IndexWebsiteView.cshtml";
        #endregion

        #region Actions
        [Route("", Name = $"{nameof(HomeWebsiteController)}{nameof(Index)}")]
        public IActionResult Index()
        {
            return View(_viewName);
        }
        #endregion
    }
}