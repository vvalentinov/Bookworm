namespace Bookworm.Common
{
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

        public string SuccessMessage { get; }

        public string ErrorMessage { get; }

        public static OperationResult Ok() => new(true);

        public static OperationResult Ok(string successMessage) => new(true, successMessage);

        public static OperationResult<T> Ok<T>(T data) => new(true, data);

        public static OperationResult<T> Ok<T>(T data, string successMessage) => new(true, data, successMessage);

        public static OperationResult Fail(string errorMessage) => new(false, null, errorMessage);

        public static OperationResult<T> Fail<T>(string errorMessage) => new(false, default, null, errorMessage);
    }

    public class OperationResult<T> : OperationResult
    {
        public OperationResult(
            bool isSuccess,
            T data,
            string successMessage = null,
            string errorMessage = null)
            : base(isSuccess, successMessage, errorMessage)
        {
            this.Data = data;
        }

        public T Data { get; }
    }
}
