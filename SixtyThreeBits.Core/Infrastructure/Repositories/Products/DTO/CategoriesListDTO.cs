namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public record CategoriesListDTO
    {
        #region Properties
        public int? CategoryID { get; init; }
        public string CategoryName { get; init; }
        #endregion
    }
}
