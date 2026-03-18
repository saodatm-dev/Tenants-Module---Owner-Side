using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Microsoft.Extensions.Logging;

namespace Core.Application.Abstractions.Behaviors;

internal static class LoggingDecorator
{
	internal sealed class CommandHandler<TCommand, TResponse>(
		ICommandHandler<TCommand, TResponse> innerHandler,
		ILogger<CommandHandler<TCommand, TResponse>> logger) : ICommandHandler<TCommand, TResponse>
		where TCommand : ICommand<TResponse>
	{
		public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
		{
			string commandName = typeof(TCommand).Name;

			logger.LogInformation("Processing command {Command}", commandName);

			var result = await innerHandler.Handle(command, cancellationToken);

			if (result.IsSuccess)
				LogSuccess(logger, commandName);
			if (result.IsFailure && result.Error == Error.None)
				LogWarning(logger, commandName);
			else if (result.IsFailure && result.Error != Error.None)
				LogError(logger, commandName, result.Error);

			return result;
		}
	}

	internal sealed class CommandBaseHandler<TCommand>(
		ICommandHandler<TCommand> innerHandler,
		ILogger<CommandBaseHandler<TCommand>> logger) : ICommandHandler<TCommand>
		where TCommand : ICommand
	{
		public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
		{
			string commandName = typeof(TCommand).Name;

			logger.LogInformation("Processing command {Command}", commandName);

			var result = await innerHandler.Handle(command, cancellationToken);

			if (result.IsSuccess)
				LogSuccess(logger, commandName);
			if (result.IsFailure && result.Error == Error.None)
				LogWarning(logger, commandName);
			else if (result.IsFailure && result.Error != Error.None)
				LogError(logger, commandName, result.Error);

			return result;
		}
	}
	internal sealed class QueryHandler<TQuery, TResponse>(
		IQueryHandler<TQuery, TResponse> innerHandler,
		ILogger<QueryHandler<TQuery, TResponse>> logger) : IQueryHandler<TQuery, TResponse>
		where TQuery : IQuery<TResponse>
	{
		public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
		{
			string queryName = typeof(TQuery).Name;

			logger.LogInformation("Processing query {Query}", queryName);

			var result = await innerHandler.Handle(query, cancellationToken);

			if (result.IsSuccess)
				LogSuccess(logger, queryName);
			if (result.IsFailure && result.Error == Error.None)
				LogWarning(logger, queryName);
			else if (result.IsFailure && result.Error != Error.None)
				LogError(logger, queryName, result.Error);

			return result;
		}
	}
	private static void LogSuccess(ILogger logger, string commandName)
	{
		logger.LogInformation("Completed {Command}", commandName);
	}
	private static void LogWarning(ILogger logger, string commandName)
	{
		logger.LogWarning("Completed {Command}", commandName);
	}

	private static void LogError(ILogger logger, string commandName, Error error)
	{
		var data = new Dictionary<string, object>
		{
			["Error"] = error
		};
		using (logger.BeginScope(data))
		{
			logger.LogError("Completed command {Command} with error", commandName);
		}
	}
}
