using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Libraries.Common;
using SixtyThreeBits.Core.Libraries.Database;
using SixtyThreeBits.Core.Libraries.Extensions;
using System.Collections.Generic;
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

        public async Task<Result63<List<DictionariesDTO>>> DictionariesList()
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
                            .ThenBy(item => item.DictionaryCaption);
                        var result = await resultQueryable.ToListAsync();
                        
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<Result63<List<DictionariesDTO>>> DictionariesListByLevelCodeIsVisible(int? dictionaryLevel, int? dictionaryCode, bool? dictionaryIsVisible = null)
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
                        var result = await resultQueryable.ToListAsync();                        

                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<Result63<List<KeyValueTuple<int?, string>>>> DictionariesListAsKeyValueTuple(int? dictionaryCode, bool isDictionaryIntCodeAsKey = false)
        {
            var dictionariesResult = await DictionariesListByLevelCodeIsVisible(dictionaryLevel: 1, dictionaryCode: dictionaryCode);
            if (dictionariesResult.IsError)
            {
                return Result63<List<KeyValueTuple<int?, string>>>.Failure(errorMessage: dictionariesResult.ErrorMessage, exception: dictionariesResult.Exception);
            }
            else
            {
                var result = dictionariesResult.Value
                    ?.Select(item => new KeyValueTuple<int?, string>
                    {
                        Key = isDictionaryIntCodeAsKey ? item.DictionaryIntCode : item.DictionaryID,
                        Value = item.DictionaryCaption
                    }).ToList();
                return Result63<List<KeyValueTuple<int?, string>>>.Success(result);
            }
        }

        public async Task<Result63<List<KeyValueSelectedTuple<int?, string>>>> DictionariesListAsKeyValueSelectedTuple(int? dictionaryCode, int? selectedValue, bool isDictionaryIntCodeAsKey = false)
        {
            var dictionariesResult = await DictionariesListByLevelCodeIsVisible(dictionaryLevel: 1, dictionaryCode: dictionaryCode);
            if (dictionariesResult.IsError)
            {
                return Result63<List<KeyValueSelectedTuple<int?, string>>>.Failure(errorMessage: dictionariesResult.ErrorMessage, exception: dictionariesResult.Exception);
            }
            else
            {
                var result = dictionariesResult.Value
                    ?.Select(item => new KeyValueSelectedTuple<int?, string>
                    {
                        Key = isDictionaryIntCodeAsKey ? item.DictionaryIntCode : item.DictionaryID,
                        Value = item.DictionaryCaption,
                        IsSelected = isDictionaryIntCodeAsKey ? (item.DictionaryIntCode == selectedValue) : (item.DictionaryID == selectedValue)
                    }).ToList();
                return Result63<List<KeyValueSelectedTuple<int?, string>>>.Success(result);
            }
        }
        #endregion
    }
}