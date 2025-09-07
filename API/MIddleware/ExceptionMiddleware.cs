using System;
using System.Text.Json;
using API.Errors;

namespace API.MIddleware;

//at the top of the middleware pipeline
public class ExceptionMiddleware(IHostEnvironment hostEnvironment, RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {

            await HandleExceptionAsync(context, ex, hostEnvironment);
        }
    }
    private static Task HandleExceptionAsync(HttpContext context, Exception ex, IHostEnvironment hostEnvironment)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 500;

        var response = hostEnvironment.IsDevelopment() ? new ApiErrorResponse(context.Response.StatusCode, ex.Message, ex.StackTrace)
        : new ApiErrorResponse(context.Response.StatusCode, ex.Message, "Internal server error");

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);

        return context.Response.WriteAsync(json);
    }
}
