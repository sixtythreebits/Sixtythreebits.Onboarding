using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Libraries.Common;
using SixtyThreeBits.Core.Libraries.Database;
using SixtyThreeBits.Core.Libraries.Extensions;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class SystemPropertiesRepository : RepositoryBase
    {
        #region Contructors
        public SystemPropertiesRepository(DbContextFactory dbContextFactory, ILogger logger) : base(dbContextFactory, logger)
        {
        }
        #endregion

        #region Methods
        public async Task<Result63<SystemPropertiesDTO>> SystemPropertiesGet()
        {
            var result = await TryAsync(
                logString: $"{nameof(SystemPropertiesGet)}()", 
                tryFunc: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(SystemPropertiesGet)
                        );

                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();
                        var result = resultJson.DeserializeJsonTo<SystemPropertiesDTO>();

                        return result;
                    }
                }
            );
            return result;
        }        
        #endregion
    }        
}