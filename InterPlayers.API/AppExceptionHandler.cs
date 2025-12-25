using System.Net;
using InterPlayers.Application.Exceptions;
using InterPlayers.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace InterPlayers.API;

public class AppExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        httpContext.Response.ContentType = "application/json";

        switch (exception)
        {
            case NotFoundException notFound:
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

                await httpContext.Response.WriteAsJsonAsync(
                    new { error = "NotFound", message = notFound.Message },
                    cancellationToken
                );

                return true;

            case DomainValidationException validationException:
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                await httpContext.Response.WriteAsJsonAsync(
                    new { error = "BadRequest", message = validationException.Message },
                    cancellationToken
                );

                return true;

            default:
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await httpContext.Response.WriteAsJsonAsync(
                    new { error = "ServerError", message = "An unexpected error occurred." },
                    cancellationToken
                );

                return true;
        }
    }
}
