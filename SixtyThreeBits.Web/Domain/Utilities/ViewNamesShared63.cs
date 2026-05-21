namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static class ViewNamesShared63
    {
        #region Properties
        public const string PluginsClientFooterPartial = "~/Views/Shared/PluginsClientFooterPartial.cshtml";
        public const string PluginsClientHeaderPartial = "~/Views/Shared/PluginsClientHeaderPartial.cshtml";
        public const string SuccessErrorToastPartial = "~/Views/Shared/SuccessErrorToastPartial.cshtml";
        #endregion

        #region Nested Classes
        public static class Admin
        {
            #region Properties
            public const string Layout = "~/Views/Admin/Shared/_Layout.cshtml";
            public const string NotFoundView = "~/Views/Admin/Shared/NotFoundAdminView.cshtml";
            public const string TabsPartial = "~/Views/Admin/Shared/TabsPartial.cshtml";
            public const string SuccessErrorToastPartial = "~/Views/Admin/Shared/SuccessErrorToastPartial.cshtml";
            #endregion
        }

        public static class Website
        {
            #region Properties
            public const string Layout = "~/Views/Website/Shared/_Layout.cshtml";
            public const string NotFoundView = "~/Views/Website/Shared/NotWebsiteFound.cshtml";
            #endregion
        }
        #endregion
    }
}