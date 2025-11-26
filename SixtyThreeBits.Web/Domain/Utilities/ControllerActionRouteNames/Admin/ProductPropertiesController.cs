namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        #region Nested Classes
        public static partial class Admin
        {
            #region Nested Classes
            public static class ProductPropertiesController
            {
                #region Properties
                public const string Properties = $"{nameof(Admin)}{nameof(ProductPropertiesController)}{nameof(Properties)}";
                public const string DeleteImage = $"{nameof(Admin)}{nameof(ProductPropertiesController)}{nameof(DeleteImage)}";
                #endregion
            } 
            #endregion
        }
        #endregion
    }
}