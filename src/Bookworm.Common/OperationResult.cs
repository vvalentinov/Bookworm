namespace Bookworm.Common
{
    using System.Collections.Generic;

    public class OperationResult
    {
        private protected OperationResult(
            bool isSuccess,
            string successMessage = null,
            string errorMessage = null)
        {
            this.IsSuccess = isSuccess;
            this.SuccessMessage = successMessage;
            this.ErrorMessage = errorMessage;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !this.IsSuccess;

        public string SuccessMessage { get; }

        public string ErrorMessage { get; }

        // Ok Methods
        public static OperationResult Ok(string successMessage = null)
            => new(true, successMessage);

        public static OperationResult<T> Ok<T>(T data, string successMessage = null)
           => new(true, data, successMessage);

        public static OperationResult<IEnumerable<T>> Ok<T>(List<T> data)
            => new(true, data);

        // Fail Methods
        public static OperationResult Fail(string errorMessage)
            => new(false, null, errorMessage);

        public static OperationResult<T> Fail<T>(string errorMessage)
            => new(false, default, null, errorMessage);
    }

    public class OperationResult<T>(
        bool isSuccess,
        T data,
        string successMessage = null,
        string errorMessage = null) : OperationResult(isSuccess, successMessage, errorMessage)
    {
        public T Data { get; } = data;
    }
}
