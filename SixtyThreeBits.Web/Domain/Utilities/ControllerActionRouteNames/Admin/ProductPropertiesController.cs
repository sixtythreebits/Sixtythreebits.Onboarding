namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class ProductPropertiesController
            {
                public const string Properties = $"{nameof(Admin)}{nameof(ProductPropertiesController)}{nameof(Properties)}";
                public const string DeleteImage = $"{nameof(Admin)}{nameof(ProductPropertiesController)}{nameof(DeleteImage)}";
            }
        }
    }
}