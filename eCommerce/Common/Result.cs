using System;

namespace eCommerce.Common
{
    public class Result
    {
        private bool Success { get; }
        public string Error { get; }
        public bool IsSuccess => Success;
        public bool IsFailure => !Success;

        protected Result(bool success, string error)
        {
            if (success && error != string.Empty)
                throw new InvalidOperationException();
            if (!success && error == string.Empty)
                throw new InvalidOperationException();
            Success = success;
            Error = error;
        }

        public static Result Fail(string message)
        {
            return new Result(false, message);
        }

        public static Result<T> Fail<T>(string message)
        {
            return new Result<T>(default(T), false, message);
        }

        public static Result Ok()
        {
            return new Result(true, string.Empty);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value, true, string.Empty);
        }

        public String GetErrorReason()
        {
            return this.Error;
        }
    }
  
    public class Result<T> : Result
    {
        public T Value { get; }

        protected internal Result(T value, bool success, string error) : base(success, error)
        {
            Value = value;
        }

        public T GetValue()
        {
            return Value;
        }
    }
}