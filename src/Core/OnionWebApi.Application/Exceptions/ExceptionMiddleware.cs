namespace OnionWebApi.Application.Exceptions;
public class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An unhandled exception occurred while processing the request");

            if (httpContext.Response.HasStarted)
            {
                Log.Warning("Cannot handle exception because response has already started");
                throw;
            }

            await HandleExceptionAsync(httpContext, ex);
        }
    }
    private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var statusCode = GetStatusCode(exception);
        var response = CreateErrorResponse(exception, statusCode);

        httpContext.Response.Clear();
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = statusCode;

        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        await httpContext.Response.WriteAsync(jsonResponse);        
    }

    private static ErrorResponse CreateErrorResponse(Exception exception, int statusCode)
    {
        return exception switch
        {
            FluentValidation.ValidationException validationEx => new ErrorResponse
            {
                StatusCode = statusCode,
                Title = "Validation Error",
                Errors = validationEx.Errors.Select(x => new ErrorDetail
                {
                    Field = x.PropertyName,
                    Message = x.ErrorMessage,
                    Code = x.ErrorCode
                }).ToList()
            },
            BadRequestException badRequestEx => new ErrorResponse
            {
                StatusCode = statusCode,
                Title = "Bad Request",
                Message = badRequestEx.Message,
                Errors = new List<ErrorDetail>()
            },
            NotFoundException notFoundEx => new ErrorResponse
            {
                StatusCode = statusCode,
                Title = "Not Found",
                Message = notFoundEx.Message,
                Errors = new List<ErrorDetail>()
            },
            _ => new ErrorResponse
            {
                StatusCode = statusCode,
                Title = "Internal Server Error",
                Message = "An unexpected error occurred",
                Errors = new List<ErrorDetail>()
            }
        };
    }
    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            BadRequestException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound, // 404 olmalı
            FluentValidation.ValidationException => StatusCodes.Status422UnprocessableEntity,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            ArgumentException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
    }
        

    
}
