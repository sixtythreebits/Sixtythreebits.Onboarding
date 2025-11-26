using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries.Database;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class ProductsRepository : RepositoryBase
    {
        #region Constructors
        public ProductsRepository(DbContextFactory dbContextFactory, ILogger logger) : base(dbContextFactory, logger)
        {

        }
        #endregion

        #region Methods
        public async Task<List<CategoriesListDTO>> CategoriesList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(CategoriesList)}()",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(CategoriesList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<CategoriesListDTO>();
                        resultQueryable = resultQueryable.OrderBy(item => item.CategoryName);
                        var result = await resultQueryable.ToListAsync();

                        return result;
                    }
                }
            );

            return result;
        }

        public async Task<ProductDTO> ProductsGetSingleByID(int? productID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(ProductsGetSingleByID)}({nameof(productID)} = {productID})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductsGetSingleByID),
                            sqlParameters:
                            [
                                productID.ToSqlParameter(nameof(productID), SqlDbType.Int)
                            ]
                        );
                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();
                        var result = resultJson.DeserializeJsonTo<ProductDTO>();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<int?> ProductsIUD(Enums.DatabaseActions databaseAction, int? productID, ProductIudDTO product)
        {
            var productJson = product.ToJson();

            productID = await TryToReturnAsyncTask(
                logString: $"{nameof(ProductsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(productID)} = {product}, {nameof(productJson)} = {productJson})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductsIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                                productID.ToSqlOutputParameter(nameof(productID),SqlDbType.Int),
                                productJson.ToSqlParameter(nameof(productJson),SqlDbType.NVarChar),
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                        productID = sqb.GetNextOutputParameterValue<int?>();
                        return productID;
                    }
                }
            );
            return productID;
        }

        public async Task<List<ProductsListDTO>> ProductsList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(ProductsList)}()",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductsList)
                        );
                        
                        var resultQueryable = sqb.ExecuteTableValuedFunction<ProductsListDTO>();
                        resultQueryable = resultQueryable.OrderByDescending(item => item.ProductDateCreated);
                        var result = await resultQueryable.ToListAsync();

                        return result;
                    }
                }
            );
            return result;
        }
        #endregion
    }
}