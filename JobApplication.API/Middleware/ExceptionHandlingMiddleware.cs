using JobApplication.Application.Exceptions;
using JobApplication.Domain.Exceptions;

namespace JobApplication.API.Middleware
{
    // Maps business exceptions to HTTP status codes. Unknown exceptions are not handled here.
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) when (ex is NotFoundException or ForbiddenException or UnauthorizedException or BadRequestException or DomainException)
            {
                context.Response.StatusCode = ex switch
                {
                    NotFoundException => StatusCodes.Status404NotFound,
                    ForbiddenException => StatusCodes.Status403Forbidden,
                    UnauthorizedException => StatusCodes.Status401Unauthorized,
                    _ => StatusCodes.Status400BadRequest
                };
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
            }
        }
    }
}
