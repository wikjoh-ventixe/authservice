namespace Business.Models;

public abstract class ServiceResult<TResult, TData> where TResult : ServiceResult<TResult, TData>, new()
{
    public bool Succeeded { get; set; }
    public int StatusCode { get; set; }
    public string? ErrorMessage { get; set; }
    public TData? Data { get; set; }


    public static TResult Ok()
    {
        return new TResult
        {
            Succeeded = true,
            StatusCode = 200
        };
    }

    public static TResult Ok(TData data)
    {
        return new TResult
        {
            Succeeded = true,
            StatusCode = 200,
            Data = data
        };
    }

    public static TResult Created(TData data)
    {
        return new TResult
        {
            Succeeded = true,
            StatusCode = 201,
            Data = data
        };
    }

    public static TResult NoContent()
    {
        return new TResult
        {
            Succeeded = true,
            StatusCode = 204
        };
    }

    public static TResult BadRequest(string errorMessage)
    {
        return new TResult
        {
            Succeeded = false,
            StatusCode = 400,
            ErrorMessage = errorMessage
        };
    }

    public static TResult Unauthorized(string errorMessage)
    {
        return new TResult
        {
            Succeeded = false,
            StatusCode = 401,
            ErrorMessage = errorMessage
        };
    }

    public static TResult NotFound(string errorMessage)
    {
        return new TResult
        {
            Succeeded = false,
            StatusCode = 404,
            ErrorMessage = errorMessage
        };
    }

    public static TResult AlreadyExists(string errorMessage)
    {
        return new TResult
        {
            Succeeded = false,
            StatusCode = 409,
            ErrorMessage = errorMessage
        };
    }

    public static TResult InternalServerError(string errorMessage)
    {
        return new TResult
        {
            Succeeded = false,
            StatusCode = 500,
            ErrorMessage = errorMessage
        };
    }
}