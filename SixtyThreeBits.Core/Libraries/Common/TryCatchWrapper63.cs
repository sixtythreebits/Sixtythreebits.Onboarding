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
        #endregion

        #region Constructors
        public TryCatchWrapper63(ILogger logger)
        {
            _logger = logger;
        }
        #endregion

        #region Methods
        protected Result63 Try(string logString, Action tryAction, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            var errorMessage = default(string);
            var exception = default(Exception);
            var result = default(Result63);

            try
            {
                tryAction();
                result = Result63.Success();
            }
            catch (Exception ex)
            {
                exception = ex;
                errorMessage = getErrorMessageFromException(exception);
                logError(
                    exception: exception,
                    errorMessage: errorMessage,
                    logString: logString,
                    callerFilePath: callerFilePath,
                    callerLineNumber: callerLineNumber
                );
                result = Result63.Failure(errorMessage: errorMessage, exception: exception);
            }

            return result;
        }

        protected Result63<T> Try<T>(string logString, Func<T> tryFunc, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            var errorMessage = default(string);
            var resultValue = default(T);
            var exception = default(Exception);
            var result = default(Result63<T>);

            try
            {
                resultValue = tryFunc();
                result = Result63<T>.Success(resultValue);
            }
            catch (Exception ex)
            {
                exception = ex;
                errorMessage = getErrorMessageFromException(exception);                
                logError(
                    exception: exception,
                    errorMessage: errorMessage,
                    logString: logString,
                    callerFilePath: callerFilePath,
                    callerLineNumber: callerLineNumber
                );
                result = Result63<T>.Failure(errorMessage: errorMessage, exception: exception);
            }

            return result;
        }

        protected async Task<Result63<T>> TryAsync<T>(string logString, Func<Task<T>> tryFunc, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            var errorMessage = default(string);
            var resultValue = default(T);
            var exception = default(Exception);
            var result = default(Result63<T>);

            try
            {
                resultValue = await tryFunc();
                result = Result63<T>.Success(resultValue);
            }
            catch (Exception ex)
            {
                exception = ex;
                errorMessage = getErrorMessageFromException(exception);
                logError(
                    exception: exception,
                    errorMessage: errorMessage,
                    logString: logString,
                    callerFilePath: callerFilePath,
                    callerLineNumber: callerLineNumber
                );
                result = Result63<T>.Failure(errorMessage: errorMessage, exception: exception);
            }

            return result;
        }

        protected async Task<Result63> TryAsync(string logString, Func<Task> tryFunc, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            var errorMessage = default(string);
            var exception = default(Exception);
            var result = default(Result63);

            try
            {
                await tryFunc();
                result = Result63.Success();
            }
            catch (Exception ex)
            {
                exception = ex;
                errorMessage = getErrorMessageFromException(exception);
                logError(
                    exception: exception,
                    errorMessage: errorMessage,
                    logString: logString,
                    callerFilePath: callerFilePath,
                    callerLineNumber: callerLineNumber
                );
                result = Result63.Failure(errorMessage: errorMessage, exception: exception);
            }

            return result;
        }
        #endregion

        #region Private Methods
        string getErrorMessageFromException(Exception exception)
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

            return errorMessageBuilder.ToString();
        }

        void logError(Exception exception, string errorMessage, string logString, string callerFilePath, int callerLineNumber)
        {
            if (_logger != null)
            {
                var message = string.Format("Source File - {0}{4}Line Number - {1}{4}{2} --- {3}", callerFilePath, callerLineNumber, logString, errorMessage, Environment.NewLine);
                _logger.LogError(exception: exception, message: message);
            }
        }
        #endregion
    }
}