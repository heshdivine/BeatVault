using System.Net;
using System.Text.Json;

namespace BeatVault.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Try to pass the request to the next step (Controller)
            await _next(context);
        }
        catch (Exception ex)
        {
            // If ANY error happens above, catch it here
            _logger.LogError(ex, ex.Message); // 1. Log the error (for you)

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // 2. Create a safe response
            var response = _env.IsDevelopment()
                ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                : new ApiException(context.Response.StatusCode, "Internal Server Error", "Something went wrong");

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            // 3. Send safe JSON to React
            await context.Response.WriteAsync(json);
        }
    }
}

// Simple wrapper class for errors
public class ApiException
{
    public ApiException(int statusCode, string message, string? details = null)
    {
        StatusCode = statusCode;
        Message = message;
        Details = details;
    }

    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string? Details { get; set; }
}