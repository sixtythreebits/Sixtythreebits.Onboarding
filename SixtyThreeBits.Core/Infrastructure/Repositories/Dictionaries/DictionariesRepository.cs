using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Libraries.Common;
using SixtyThreeBits.Core.Libraries.Database;
using SixtyThreeBits.Core.Libraries.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class DictionariesRepository : RepositoryBase
    {
        #region Contructors
        public DictionariesRepository(DbContextFactory dbContextFactory, ILogger logger) : base(dbContextFactory, logger)
        {
        }
        #endregion

        #region Methods
        public async Task<Result63> DictionariesDeleteRecursive(int? dictionaryID)
        {
            var result = await TryAsync(
                logString: $"{nameof(DictionariesDeleteRecursive)}({nameof(dictionaryID)} = {dictionaryID})",
                tryFunc: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(DictionariesDeleteRecursive),
                            sqlParameters:
                            [
                                dictionaryID.ToSqlParameter(SqlDbType.Int)
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                    }
                }
            );
            return result;
        }

        public async Task<Result63<int?>> DictionariesIUD(DatabaseActions databaseAction, int? dictionaryID, DictionariesIudDTO dictionary)
        {
            var dictionaryJson = dictionary.ToJson();

            var result = await TryAsync(
                logString: $"{nameof(DictionariesIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(dictionaryID)} = {dictionaryID}, {nameof(dictionary)} = {dictionaryJson})",
                tryFunc: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(DictionariesIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(SqlDbType.TinyInt),
                                dictionaryID.ToSqlParameterOutput(SqlDbType.Int),
                                dictionaryJson.ToSqlParameter(SqlDbType.NVarChar)
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                        dictionaryID = sqb.GetNextOutputParameterValue<int?>();
                        return dictionaryID;
                    }
                }
            );
            return result;
        }

        public async Task<Result63<ReadOnlyCollection<DictionariesDTO>>> DictionariesList()
        {
            var result = await TryAsync(
                logString: $"{nameof(DictionariesList)}()",
                tryFunc: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(DictionariesList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<DictionariesDTO>();
                        resultQueryable = resultQueryable
                            .OrderByDescending(item => item.DictionaryIsDefault)
                            .ThenBy(item => item.DictionarySortIndex)
                            .ThenBy(item => item.DictionaryCaption).OrderByDescending(item => item.DictionaryIsDefault);
                            
                        var result = await resultQueryable.ToReadOnlyListAsync();

                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<Result63<ReadOnlyCollection<DictionariesDTO>>> DictionariesListByLevelCodeIsVisible(int? dictionaryLevel, int? dictionaryCode, bool? dictionaryIsVisible = null)
        {
            var result = await TryAsync(
                logString: $"{nameof(DictionariesListByLevelCodeIsVisible)}({nameof(dictionaryLevel)} = {dictionaryLevel}, {nameof(dictionaryCode)} = {dictionaryCode}, {nameof(dictionaryIsVisible)} = {dictionaryIsVisible})",
                tryFunc: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(DictionariesListByLevelCodeIsVisible),
                            sqlParameters:
                            [
                                dictionaryLevel.ToSqlParameter(SqlDbType.Int),
                                dictionaryCode.ToSqlParameter(SqlDbType.Int),
                                dictionaryIsVisible.ToSqlParameter(SqlDbType.Bit)
                            ]
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<DictionariesDTO>();
                        resultQueryable = resultQueryable
                            .OrderByDescending(item => item.DictionaryIsDefault)
                            .ThenBy(item => item.DictionarySortIndex)
                            .ThenBy(item => item.DictionaryCaption);
                        var result = await resultQueryable.ToReadOnlyListAsync();

                        return result;
                    }
                }
            );
            return result;
        }        
        #endregion
    }
}