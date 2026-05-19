using SixtyThreeBits.Web.Domain.Utilities;

namespace SixtyThreeBits.Web.Models.Website
{
    public class LogoutModel : WebsiteModelBase
    {
        #region Methods
        public string LogoutAndGetRedirectUrl()
        {
            SessionAssistance.Clear();
            CookieAssistance.Remove(CookieKeys63.User);

            var urlHome = UrlFactory.Website.CreateUrlHome();
            
            return urlHome;
        }
        #endregion
    }
}
