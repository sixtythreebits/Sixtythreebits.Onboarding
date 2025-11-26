using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Infrastructure.Repositories;

namespace SixtyThreeBits.Core.Factories
{
    public class RepositoryFactory
    {
        #region Properties                
        readonly DbContextFactory _dbContextFactory;
        readonly ILogger _logger;
        #endregion

        #region Constructors
        public RepositoryFactory(string dbConnectionString, ILogger logger = null)
        {
            _dbContextFactory = new DbContextFactory(dbConnectionString);
            _logger = logger;
        }
        #endregion

        #region Methods
        public DictionariesRepository CreateDictionariesRepository()
        {
            return new DictionariesRepository(_dbContextFactory, _logger);
        }

        public PermissionsRepository CreatePermissionsRepository()
        {
            return new PermissionsRepository(_dbContextFactory, _logger);
        }

        public RolesRepository CreateRolesRepository()
        {
            return new RolesRepository(_dbContextFactory, _logger);
        }

        public SystemPropertiesRepository CreateSystemPropertiesRepository()
        {
            return new SystemPropertiesRepository(_dbContextFactory, _logger);
        }

        public UsersRepository CreateUsersRepository()
        {
            return new UsersRepository(_dbContextFactory, _logger);
        }
        #endregion        
    }
}