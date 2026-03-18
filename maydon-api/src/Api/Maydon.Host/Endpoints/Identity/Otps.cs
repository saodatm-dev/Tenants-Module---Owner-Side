using Core.Application.Abstractions.Messaging;
using Identity.Application.Otps.Send;
using Identity.Domain;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;

namespace Maydon.Host.Endpoints.Identity;

internal sealed class Otps : IEndpoint
{
	string IEndpoint.GroupName => AssemblyReference.Instance;
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost("/", async (
		   SendOtpCommand command,
		   ICommandHandler<SendOtpCommand> handler,
		   CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);

			return result.Match(CustomResults.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status400BadRequest);

		//app.MapPost("/registration", async (
		//   RegistrationOtpCommand command,
		//   ICommandHandler<RegistrationOtpCommand> handler,
		//   CancellationToken cancellationToken) =>
		//{
		//	var result = await handler.Handle(command, cancellationToken);

		//	return result.Match(CustomResults.Ok, CustomResults.Problem);
		//})
		//	.AllowAnonymous()
		//	.Produces(StatusCodes.Status200OK)
		//	.Produces(StatusCodes.Status400BadRequest);

		//app.MapPost("/restore-password", async (
		//   RestorePasswordOtpCommand command,
		//   ICommandHandler<RestorePasswordOtpCommand> handler,
		//   CancellationToken cancellationToken) =>
		//{
		//	var result = await handler.Handle(command, cancellationToken);

		//	return result.Match(CustomResults.Ok, CustomResults.Problem);
		//})
		//	.AllowAnonymous()
		//	.Produces(StatusCodes.Status200OK)
		//	.Produces(StatusCodes.Status400BadRequest);
	}
}
