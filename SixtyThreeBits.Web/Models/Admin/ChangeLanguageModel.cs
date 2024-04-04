using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Shared;
using System;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class ChangeLanguageModel : ModelBase
    {
        #region Methods
        public void ChangeLanguage(string culture)
        {
            if (Utilities.SupportedLanguageStrings.Contains(culture))
            {
                CookieAssistance.Set(
                    key: WebConstants.Cookies.AdminLanguageCultureCode, 
                    value: culture, 
                    expirationDate: DateTime.Now.AddMonths(12)
                );
            }
        }
        #endregion
    }
}
