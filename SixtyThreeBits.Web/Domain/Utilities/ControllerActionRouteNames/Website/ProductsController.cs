namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
      
        public static partial class Website
        {
            public static class ProductsController
            {
                #region Proeprties
                public const string Products = $"{nameof(Website)}{nameof(ProductsController)}{nameof(Products)}";
                #endregion
            }
        }
    }
}