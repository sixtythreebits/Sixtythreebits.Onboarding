using SixtyThreeBits.Core.Infrastructure.Database;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class RepositoryFactory
    {
        #region Properties                
        readonly DbContextFactory _dbContextFactory;
        #endregion

        #region Constructors
        public RepositoryFactory(string dbConnectionString)
        {
            _dbContextFactory = new DbContextFactory(dbConnectionString);
        }
        #endregion

        #region Methods
        public DictionariesRepository GetDictionariesRepository()
        {
            return new DictionariesRepository(_dbContextFactory);
        }

        public PermissionsRepository GetPermissionsRepository()
        {
            return new PermissionsRepository(_dbContextFactory);
        }

        public ProductsRepository GetProductsRepository()
        {
            return new ProductsRepository(_dbContextFactory);
        }

        public RolesRepository GetRolesRepository()
        {
            return new RolesRepository(_dbContextFactory);
        }

        public SystemPropertiesRepository GetSystemPropertiesRepository()
        {
            return new SystemPropertiesRepository(_dbContextFactory);
        }

        public UsersRepository GetUsersRepository()
        {
            return new UsersRepository(_dbContextFactory);
        }
        #endregion        
    }
}
