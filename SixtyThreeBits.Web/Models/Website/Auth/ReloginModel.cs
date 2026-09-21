using SixtyThreeBits.Core.Infrastructure.Repositories;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Website
{
    public class ReloginModel : WebsiteModelBase
    {
        #region Methods
        public async Task<string> ReloginAndGetRedirectUrl()
        {
            var sessionUser = SessionAssistance.GetUser<UserDTO>();
            if (sessionUser != null)
            {
                var repository = RepositoryFactory.CreateUsersRepository();
                var userResult = await repository.UsersGetSingleByID(sessionUser.UserID);
                var user = userResult.Value;
                if (user != null && user.UserIsActive)
                {
                    SessionAssistance.SetUser(user);
                }
            }
            
            return UrlPreviousPage;
        }
        #endregion
    }
}
