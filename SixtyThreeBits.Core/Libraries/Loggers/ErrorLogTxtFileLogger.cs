using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;

namespace SixtyThreeBits.Core.Libraries.Loggers
{
    public class ErrorLogTxtFileLogger : ILogger
    {
        #region Properties
        readonly string _errorLogTxtFileDirectoryPath;
        readonly string _errorLogTxtFilePath;
        static object _lockFileCreate = new object();
        static object _lockFileWrite = new object();
        #endregion

        #region Constructors
        public ErrorLogTxtFileLogger()
        {
            _errorLogTxtFileDirectoryPath = $"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}App_Data";
            _errorLogTxtFilePath = $"{_errorLogTxtFileDirectoryPath}{Path.DirectorySeparatorChar}ErrorLog.txt";
        } 
        #endregion

        #region Methods
        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == LogLevel.Error;            
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter = null)
        {
            if (IsEnabled(logLevel))
            {
                var isErrorLogFileExists = this.isErrorLogFileExists();
                if (isErrorLogFileExists)
                {
                    var errorMessage = state?.ToString();
                    writeErrorMessageToErroLogTxtFile(errorMessage: errorMessage);                    
                }
            }            
        }

        bool isErrorLogFileExists()
        {            
            var isFileExists = false;

            if (File.Exists(_errorLogTxtFilePath))
            {
                isFileExists = true;
            }
            else  
            {

                lock (_lockFileCreate)
                {
                    try
                    {
                        if (!Directory.Exists(_errorLogTxtFileDirectoryPath))
                        {
                            Directory.CreateDirectory(_errorLogTxtFileDirectoryPath);
                        }
                        File.Create(_errorLogTxtFilePath).Close();
                        isFileExists = true;
                    }
                    catch { }
                }
            }

            return isFileExists;
        }

        void writeErrorMessageToErroLogTxtFile(string errorMessage)
        {
            lock (_lockFileWrite)
            {
                try
                {
                    File.AppendAllText(_errorLogTxtFilePath, $"------------------------------------\r\n{DateTime.Now}\r\n{errorMessage}------------------------------------\r\n\r\n", Encoding.UTF8);
                }
                catch { }
            }
        }
        #endregion        
    }
}