namespace MyAPI.Services
{
    public class ServiceResult<T>
    {
        public bool Succeeded { get; init; }
        public int StatusCode { get; init; }
        public string? Message { get; init; }
        public T? Data { get; init; }

        public static ServiceResult<T> Ok(T data)
        {
            return new ServiceResult<T>
            {
                Succeeded = true,
                StatusCode = StatusCodes.Status200OK,
                Data = data
            };
        }
        public static ServiceResult<T> OK(string message)
        {
            return new ServiceResult<T>
            {
                Succeeded = true,
                StatusCode = StatusCodes.Status200OK,
                Message = message
            };
        }

        public static ServiceResult<T> BadRequest(string message)
        {
            return Error(StatusCodes.Status400BadRequest, message);
        }

        public static ServiceResult<T> NotFound(string message)
        {
            return Error(StatusCodes.Status404NotFound, message);
        }

        public static ServiceResult<T> Forbidden(string message = "Bạn không có quyền thực hiện thao tác này.")
        {
            return Error(StatusCodes.Status403Forbidden, message);
        }

        public static ServiceResult<T> Error(int statusCode, string message)
        {
            return new ServiceResult<T>
            {
                Succeeded = false,
                StatusCode = statusCode,
                Message = message
            };
        }
    }
}
