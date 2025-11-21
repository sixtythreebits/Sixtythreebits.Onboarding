namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record ProductIudDTO
    {
        #region Properties
        public string ProductName { get; init; }
        public decimal? ProductPrice { get; init; }
        public string ProductCoverImageFilename { get; init; }
        public int? CategoryID { get; init; }
        public bool? ProductIsPublished { get; init; }
        #endregion
    }
}