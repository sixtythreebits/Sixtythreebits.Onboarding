using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Base;

namespace SixtyThreeBits.Web.Domain.ViewModels.Admin
{
    public class AdminLayoutViewModel : LayoutViewModelBase
    {
        #region Properties                        
        public string UserFullname { get; set; }
        public string UserEmail { get; set; }
        public ValueWrapper<bool> IsSidebarCollapsed { get; set; }
        public string UrlRelogin { get; set; }

        public readonly string TextBackToWebsite = Resources.TextBackToWebsite;
        public readonly string TextRelogin = Resources.TextRelogin;
        public readonly string TextLogout = Resources.TextLogout;

        public readonly string SidebarStatusCookieKey = WebConstants.Cookies.IsAdminSideBarCollapsed;
        public readonly string HeaderSectionName = WebConstants.ViewSections.HeaderSection;
        public readonly string FooterSectionName = WebConstants.ViewSections.FooterSection;
        #endregion
    }    
}