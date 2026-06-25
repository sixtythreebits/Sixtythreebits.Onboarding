using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Base;
using SixtyThreeBits.Web.Models.Website;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Website
{
    public class ReloginController : ControllerBase63<ReloginModel>
    {
        #region Actions
        [Route("relogin", Name = $"{nameof(ReloginController)}{nameof(Relogin)}")]
        public async Task<IActionResult> Relogin()
        {
            var redirectUrl = await Model.ReloginAndGetRedirectUrl();
            return Redirect(redirectUrl);
        }
        #endregion
    }    
}