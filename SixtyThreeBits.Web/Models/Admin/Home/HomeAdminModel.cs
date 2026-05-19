using SixtyThreeBits.Web.Models.Base;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class HomeAdminModel : AdminModelBase
    {
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();
            viewModel.UserFullname = User.UserFullname;
            viewModel.RoleName = User.RoleName;
            return viewModel;
        }

        #region Nested Classes
        public class ViewModel
        {
            #region properties
            public string UserFullname { get; set; }
            public string RoleName { get; set; }
            #endregion
        }
        #endregion
    }
}
