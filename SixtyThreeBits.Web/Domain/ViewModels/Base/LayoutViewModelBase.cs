using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using System.Collections.Generic;

namespace SixtyThreeBits.Web.Domain.ViewModels.Base
{
    public class LayoutViewModelBase
    {
        #region Properties
        public string ProjectName { get; set; }
        public PageTitle63 PageTitle { get; set; }
        public SuccessErrorToastPartialViewModel SuccessErrorPartialViewModel { get; set; }
        public List<Menu63ViewItem> Menu { get; set; }
        public bool HasMenu => Menu?.Count > 0;
        public Breadcrumbs63 Breadcrumbs { get; set; }
        public bool ShowBreadCrumbs { get; set; }
        public List<Menu63ViewItem> Tabs { get; set; }
        public bool HasTabs => Tabs?.Count > 0;
        public string UrlLogout { get; set; }
        public PluginsClientViewModel PluginsClient { get; set; }
        public readonly string TextError = Resources.TextError;
        public readonly string TextSuccess = Resources.TextSuccess;
        #endregion
    }
}
