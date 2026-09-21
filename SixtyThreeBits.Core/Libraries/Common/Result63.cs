using System;

namespace SixtyThreeBits.Core.Libraries.Common
{
    public record Result63
    {
        #region Properties
        public bool IsError { get; }
        public string ErrorMessage { get; }
        public Exception Exception { get; }
        #endregion

        #region Constructors
        protected Result63()
        {
        }

        protected Result63(bool isError, string errorMessage)
        {
            IsError = isError;
            ErrorMessage = errorMessage;
        }

        protected Result63(bool isError, string errorMessage, Exception exception)
        {
            IsError = isError;
            ErrorMessage = errorMessage;
            Exception = exception;
        }
        #endregion

        #region Methods
        public static Result63 Success() => new();
        public static Result63 Failure(string errorMessage) => new(isError: true, errorMessage: errorMessage);
        public static Result63 Failure(string errorMessage, Exception exception) => new(isError: true, errorMessage: errorMessage, exception: exception);
        #endregion        
    }

    public record Result63<T> : Result63
    {
        #region Properties
        public T Value { get; }
        #endregion

        #region Constructors
        private Result63(T value) : base(isError: false, errorMessage: null) => Value = value;
        private Result63(string errorMessage) : base(isError: true, errorMessage: errorMessage) { }
        private Result63(string errorMessage, Exception exception) : base(isError: true, errorMessage: errorMessage, exception: exception) { }
        #endregion

        #region Methods
        public static Result63<T> Success(T value) => new(value);
        public static new Result63<T> Failure(string errorMessage) => new(errorMessage);
        public static new Result63<T> Failure(string errorMessage, Exception exception) => new(errorMessage, exception);
        #endregion
    }
}