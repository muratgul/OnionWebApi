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
            await HandleExceptionAsync(httpContext, ex);
        }
    }
    private static Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var statusCode = GetStatusCode(exception);
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = statusCode;

        if (exception.GetType() == typeof(FluentValidation.ValidationException))
        {
            return httpContext.Response.WriteAsync(new ExceptionModel
            {
                Errors = ((FluentValidation.ValidationException)exception).Errors.Select(x => x.ErrorMessage),
                StatusCode = StatusCodes.Status400BadRequest
            }.ToString());
        }

        List<string> errors = [$"Hata Mesajı : {exception.Message}"];

        return httpContext.Response.WriteAsync(new ExceptionModel
        {
            Errors = errors,
            StatusCode = statusCode
        }.ToString());

    }
    private static int GetStatusCode(Exception exception) =>
            exception switch
            {
                BadRequestException => StatusCodes.Status400BadRequest,
                NotFoundException => StatusCodes.Status400BadRequest,
                FluentValidation.ValidationException => StatusCodes.Status422UnprocessableEntity,
                _ => StatusCodes.Status500InternalServerError
            };
}
