namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record CategoriesListDTO
    {
        #region Properties
        public int? CategoryID { get; init; }
        public string CategoryName { get; init; }
        #endregion
    }
}
