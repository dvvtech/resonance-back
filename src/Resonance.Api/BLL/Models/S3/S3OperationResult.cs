namespace Resonance.Api.BLL.Models.S3
{
    public class S3OperationResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public string ErrorCode { get; set; }

        public static S3OperationResult<T> Ok(T data, string message = null) => new()
        {
            Success = true,
            Data = data,
            Message = message
        };

        public static S3OperationResult<T> Fail(string error, string code = null) => new()
        {
            Success = false,
            Message = error,
            ErrorCode = code
        };
    }
}
