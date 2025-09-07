namespace API.Errors;
public class ApiErrorResponse(int statusCode,string? message,string? details)
{
    public int StatusCode
    {
        get { return statusCode; }
        set { statusCode = value; }
    }
    public string? ExceptionMessage
    {
        get { return message; }
        set { message = value; }
    }
    public string? ExceptionDetails
    { 
        get { return details; }
        set { details = value; }
    }
}
