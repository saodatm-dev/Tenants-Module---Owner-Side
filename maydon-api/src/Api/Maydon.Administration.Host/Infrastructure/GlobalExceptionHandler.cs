using Core.Domain.Exceptions;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Maydon.Administration.Host.Infrastructure;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken cancellationToken)
	{
		logger.LogError(exception, "Unhandled exception occurred");

		var problemDetails = new ProblemDetails
		{
			Status = StatusCodes.Status500InternalServerError,
			Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
			Title = "Server failure",
		};

		if (exception is AuthorizationException authorizedException)
		{
			problemDetails.Status = StatusCodes.Status401Unauthorized;
			problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1";
			problemDetails.Title = authorizedException.Message;
			problemDetails.Detail = authorizedException.Message;
		}
		if (exception.InnerException is AuthorizationException innerAuthorizedException)
		{
			problemDetails.Status = StatusCodes.Status401Unauthorized;
			problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1";
			problemDetails.Title = innerAuthorizedException.Message;
			problemDetails.Detail = innerAuthorizedException.Message;
		}
		if (exception is AccessDeniedException accessDeniedException)
		{
			problemDetails.Status = StatusCodes.Status403Forbidden;
			problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3";
			problemDetails.Title = accessDeniedException.Message;
			problemDetails.Detail = accessDeniedException.Message;
		}
		if (exception.InnerException is AccessDeniedException innerAccessDeniedException)
		{
			problemDetails.Status = StatusCodes.Status403Forbidden;
			problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3";
			problemDetails.Title = innerAccessDeniedException.Message;
			problemDetails.Detail = innerAccessDeniedException.Message;
		}

		if (exception is AntiforgeryValidationException antiforgeryValidationException)
		{
			problemDetails.Status = StatusCodes.Status403Forbidden;
			problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3";
			problemDetails.Title = antiforgeryValidationException.Message;
			problemDetails.Detail = antiforgeryValidationException.Message;
		}
		if (exception.InnerException is AntiforgeryValidationException innerAntiforgeryValidationException)
		{
			problemDetails.Status = StatusCodes.Status403Forbidden;
			problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3";
			problemDetails.Title = innerAntiforgeryValidationException.Message;
			problemDetails.Detail = innerAntiforgeryValidationException.Message;
		}

		if (exception is BadHttpRequestException badRequestException)
		{
			problemDetails.Status = badRequestException.StatusCode;
			problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3";
			problemDetails.Title = badRequestException.Message;
			problemDetails.Detail = badRequestException.Message;
		}
		if (exception.InnerException is BadHttpRequestException innerBadRequestException)
		{
			problemDetails.Status = innerBadRequestException.StatusCode;
			problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3";
			problemDetails.Title = innerBadRequestException.Message;
			problemDetails.Detail = innerBadRequestException.Message;
		}

		httpContext.Response.StatusCode = problemDetails.Status.Value;

		await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

		return true;
	}
}
