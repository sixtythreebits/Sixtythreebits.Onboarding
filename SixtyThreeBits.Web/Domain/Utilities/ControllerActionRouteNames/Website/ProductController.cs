namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Website
        {
            public static class ProductController
            {
                public const string Product = $"{nameof(Website)}{nameof(ProductsController)}{nameof(Product)}";
            }
        }
    }
}