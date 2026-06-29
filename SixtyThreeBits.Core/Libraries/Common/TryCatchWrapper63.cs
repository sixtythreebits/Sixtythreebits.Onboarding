using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Libraries.Common
{
    public class TryCatchWrapper63
    {
        #region Properties
        readonly ILogger _logger;

        public bool IsError { private set; get; }
        public string ErrorMessage { private set; get; }
        public Exception Exception { private set; get; }
        #endregion

        #region Constructors
        public TryCatchWrapper63(ILogger logger)
        {
            _logger = logger;
        }
        #endregion

        #region Methods
        protected void TryExecute(string logString, Action actionToTry, Action actionForCatch = null, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            try
            {
                IsError = false;
                ErrorMessage = null;
                actionToTry();
            }
            catch (Exception ex)
            {
                if (actionForCatch == null)
                {
                    processException(
                        logString: logString,
                        exception: ex,
                        callerFilePath: callerFilePath,
                        callerLineNumber: callerLineNumber
                    );
                }
                else
                {
                    Exception = ex;
                    actionForCatch.Invoke();
                }
            }
        }

        protected async Task TryExecuteAsyncTask(string logString, Func<Task> asyncFuncToTry, Func<Task> asyncFuncForCatch = null, Action actionForCatch = null, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            try
            {
                IsError = false;
                ErrorMessage = null;
                await asyncFuncToTry();
            }
            catch (Exception ex)
            {
                if (actionForCatch == null)
                {
                    processException(
                        logString: logString,
                        exception: ex,
                        callerFilePath: callerFilePath,
                        callerLineNumber: callerLineNumber
                    );
                }
                else if (asyncFuncForCatch != null)
                {
                    await asyncFuncForCatch();
                }
                else
                {
                    actionForCatch();
                }
            }
        }

        protected T TryToReturn<T>(string logString, Func<T> funcToTry, Func<T> funcForCatch = null, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            var result = default(T);
            try
            {
                IsError = false;
                ErrorMessage = null;
                result = funcToTry();
            }
            catch (Exception ex)
            {
                if (funcForCatch == null)
                {
                    processException(
                        logString: logString,
                        exception: ex,
                        callerFilePath: callerFilePath,
                        callerLineNumber: callerLineNumber
                    );
                }
                else
                {
                    Exception = ex;
                    result = funcForCatch();
                }
            }
            return result;
        }

        protected async Task<T> TryToReturnAsyncTask<T>(string logString = null, Func<Task<T>> asyncFuncToTry = null, Func<Task<T>> asyncFuncForCatch = null, Func<T> funcForCatch = null, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            var result = default(T);
            try
            {
                IsError = false;
                ErrorMessage = null;
                result = await asyncFuncToTry();
            }
            catch (Exception ex)
            {
                Exception = ex;

                if (asyncFuncForCatch == null && funcForCatch == null)
                {
                    processException(
                        logString: logString,
                        exception: ex,
                        callerFilePath: callerFilePath,
                        callerLineNumber: callerLineNumber
                    );
                }
                else if (asyncFuncForCatch != null)
                {
                    result = await asyncFuncForCatch();
                }
                else
                {
                    result = funcForCatch();
                }
            }
            return result;
        }

        void processException(string logString, Exception exception, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            if (_logger != null)
            {
                var errorMessageBuilder = new StringBuilder();
                if (exception is SqlException)
                {
                    var ex = exception as SqlException;
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        if (ex.Errors[i].Number > 50000)
                        {
                            errorMessageBuilder.Append(ex.Message);
                        }
                        else
                        {
                            errorMessageBuilder.Append(ex.Errors[i].Message).Append(Environment.NewLine);
                        }
                    }
                }
                else
                {
                    if (exception.InnerException == null)
                    {
                        errorMessageBuilder.Append($"{exception.Message}{Environment.NewLine}");
                    }
                    else
                    {
                        errorMessageBuilder.Append($"Exception: {exception.Message}{Environment.NewLine}InnerException: {exception.InnerException.Message}{Environment.NewLine}");
                    }
                    errorMessageBuilder.Append($"{Environment.NewLine}StackTrace:{Environment.NewLine}{exception.StackTrace}{Environment.NewLine}");
                }

                var message = string.Format("Source File - {0}{4}Line Number - {1}{4}{2} --- {3}", callerFilePath, callerLineNumber, logString, errorMessageBuilder.ToString(), Environment.NewLine);
                _logger.LogError(exception: exception, message: message);
            }
            IsError = true;
            Exception = exception;
            ErrorMessage = exception.Message;            
        }
        #endregion
    }
}