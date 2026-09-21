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
        public static Result63<T> Success<T>(T value) => Result63<T>.Success(value);
        public static ResultFailure63 Failure(string errorMessage) => new(errorMessage);
        public static ResultFailure63 Failure(string errorMessage, Exception exception) => new(errorMessage, exception);
        #endregion

        #region Operators
        public static implicit operator Result63(ResultFailure63 failure) => new(isError: true, errorMessage: failure.ErrorMessage, exception: failure.Exception);
        #endregion
    }

    /// <summary>
    /// Untyped failure returned by Result63.Failure(...). Implicitly converts to Result63 and Result63<T>,
    /// so a method returning Result63<T> can write: return Result63.Failure("message");
    /// </summary>
    public sealed record ResultFailure63(string ErrorMessage, Exception Exception = null);

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

        #region Operators
        public static implicit operator Result63<T>(ResultFailure63 failure) => Failure(failure.ErrorMessage, failure.Exception);
        #endregion
    }
}