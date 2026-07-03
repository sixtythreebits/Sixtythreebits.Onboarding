using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;

namespace SixtyThreeBits.Web.Domain.Libraries
{
    public class ToastNotificationManager63
    {
        #region Properties        
        readonly SessionAssistance63 _sessionAssistance;
        readonly PluginsClientViewModel _pluginsClient;

        public readonly SuccessErrorToastPartialViewModel SuccessErrorToastPartialViewModel = new();
        #endregion

        #region Constructors
        public ToastNotificationManager63(SessionAssistance63 sessionAssistance, PluginsClientViewModel pluginsClient) 
        {
            _sessionAssistance = sessionAssistance;
            _pluginsClient = pluginsClient;
        }

        public void InitNotificationFromSession()
        {
            var errorMessage = _sessionAssistance.Get<string>(SessionKeys63.SuccessErrorToastError);
            if (errorMessage != null)
            {
                SuccessErrorToastPartialViewModel.ShowError = true;
                SuccessErrorToastPartialViewModel.Message = errorMessage;
                _sessionAssistance.Remove(SessionKeys63.SuccessErrorToastError);
                _pluginsClient.Enable63BitsSuccessErrorToast(true);
            }
            else
            {
                var successMessage = _sessionAssistance.Get<string>(SessionKeys63.SuccessErrorToastSuccess);
                if (successMessage != null)
                {
                    SuccessErrorToastPartialViewModel.ShowSuccess = true;
                    SuccessErrorToastPartialViewModel.Message = successMessage;
                    _sessionAssistance.Remove(SessionKeys63.SuccessErrorToastSuccess);
                    _pluginsClient.Enable63BitsSuccessErrorToast(true);
                }
            }
        }

        public void ShowSuccess(string message = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = Resources.TextSuccess;
            }
            _sessionAssistance.Set(SessionKeys63.SuccessErrorToastSuccess, message);
        }

        public void ShowError(string message = null, bool shouldDisplayAfterPageReload = false)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = Resources.TextError;
            }

            if (shouldDisplayAfterPageReload)
            {
                _sessionAssistance.Set(SessionKeys63.SuccessErrorToastError, message);
            }
            else
            {
                SuccessErrorToastPartialViewModel.ShowError = true;
                SuccessErrorToastPartialViewModel.Message = message;
                _pluginsClient.Enable63BitsSuccessErrorToast(true);
            }
        }
        #endregion
    }
}