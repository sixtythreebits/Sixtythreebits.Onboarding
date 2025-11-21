using System;
using System.Collections.Generic;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record ProductDTO
    {
        #region Properties
        public int? ProductID { get; init; }
        public string ProductName { get; init; }
        public decimal? ProductPrice { get; init; }
        public string ProductCoverImageFilename { get; init; }
        public bool ProductIsPublished { get; init; }
        public DateTime? ProductDateCreated { get; init; }
        public int? CategoryID { get; init; }
        public string CategoryName { get; init; }
        public List<ProductImage> ProductImages { get; init; }
        #endregion

        #region Nested Classes
        public record ProductImage
        {
            #region Properties
            public int? ProductImageID { get; init; }
            public string ProductImageFilename { get; init; }
            #endregion
        }
        #endregion
    }
}