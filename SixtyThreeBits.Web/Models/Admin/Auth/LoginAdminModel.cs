using SixtyThreeBits.Core.Libraries.Extensions;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using System;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class LoginAdminModel : AdminModelBase
    {
        #region Methods
        public ViewModel GetViewModel(ViewModel viewModel = null)
        {
            if (viewModel == null)
            {
                viewModel = new ViewModel();
            }
            viewModel.PluginsClient = PluginsClient;
            viewModel.ProjectName = SystemProperties.ProjectName;
            return viewModel;
        }        

        public async Task<bool> AuthenticateUser(ViewModel viewModel) 
        {
            bool isAuthenticated = false;

            var repository = RepositoryFactory.CreateUsersRepository();
            var user = await repository.UsersGetSingleByEmailAndPassword(userEmail: viewModel.Username, userPassword: viewModel.Password);
            if (user == null)
            {
                viewModel.IsLoginFailed = true;
            }
            else
            {
                isAuthenticated = true;
                SessionAssistance.SetUser(user);
                if (viewModel.IsRememberMeChecked)
                {
                    CookieAssistance.Set(
                        key: CookieKeys63.User, 
                        value: user.UserID.ToString().AesEncryptString(), 
                        expirationDate: DateTime.Now.AddDays(30)
                    );
                }
            }

            return isAuthenticated;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties         
            public PluginsClientViewModel PluginsClient { get; set; }
            public string ProjectName { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public bool IsRememberMeChecked { get; set; }
            public bool IsLoginFailed { get; set; }

            public readonly string TextAdminWelcomeTitle = Resources.TextAdminWelcomeTitle;
            public readonly string TextAdminWelcomeSubTitle = Resources.TextAdminWelcomeSubTitle;
            public readonly string TextUsername = Resources.TextUsername;
            public readonly string TextPassword = Resources.TextPassword;
            public readonly string TextRememberMe = Resources.TextRememberMe;
            public readonly string TextLogin = Resources.TextLogin;
            public readonly string ValidationUserInvalidUsernameOrPassword = Resources.ValidationUserInvalidUsernameOrPassword;
            #endregion
        }
        #endregion
    }    
}