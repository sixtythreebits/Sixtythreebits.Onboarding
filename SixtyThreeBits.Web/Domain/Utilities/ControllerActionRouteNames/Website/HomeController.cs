namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Website
        {
            public static class HomeController
            {
                public const string Index = $"{nameof(Website)}{nameof(HomeController)}{nameof(Index)}";
            }                       
        }
    }
}