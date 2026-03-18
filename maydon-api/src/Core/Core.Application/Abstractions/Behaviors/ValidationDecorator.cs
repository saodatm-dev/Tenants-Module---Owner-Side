using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using FluentValidation;
using FluentValidation.Results;

namespace Core.Application.Abstractions.Behaviors;

internal static class ValidationDecorator
{
	internal sealed class QueryHandler<TQuery, TResponse>(
		IQueryHandler<TQuery, TResponse> innerHandler,
		IEnumerable<IValidator<TQuery>> validators)
		: IQueryHandler<TQuery, TResponse>
		where TQuery : IQuery<TResponse>
	{
		public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
		{
			var validationFailures = await ValidateAsync(query, validators);

			if (validationFailures.Length == 0)
				return await innerHandler.Handle(query, cancellationToken);

			return Result.Failure<TResponse>(CreateValidationError(validationFailures));
		}
	}
	internal sealed class CommandHandler<TCommand, TResponse>(
		ICommandHandler<TCommand, TResponse> innerHandler,
		IEnumerable<IValidator<TCommand>> validators)
		: ICommandHandler<TCommand, TResponse>
		where TCommand : ICommand<TResponse>
	{
		public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
		{
			var validationFailures = await ValidateAsync(command, validators);

			if (validationFailures.Length == 0)
				return await innerHandler.Handle(command, cancellationToken);

			return Result.Failure<TResponse>(CreateValidationError(validationFailures));
		}
	}

	internal sealed class CommandBaseHandler<TCommand>(
		ICommandHandler<TCommand> innerHandler,
		IEnumerable<IValidator<TCommand>> validators)
		: ICommandHandler<TCommand>
		where TCommand : ICommand
	{
		public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
		{
			var validationFailures = await ValidateAsync(command, validators);

			if (validationFailures.Length == 0)
				return await innerHandler.Handle(command, cancellationToken);

			return Result.Failure(CreateValidationError(validationFailures));
		}
	}

	private static async Task<ValidationFailure[]> ValidateAsync<TCommand>(
		TCommand command,
		IEnumerable<IValidator<TCommand>> validators)
	{
		if (!validators.Any())
			return [];

		var context = new ValidationContext<TCommand>(command);

		var validationResults = await Task.WhenAll(
			validators.Select(validator => validator.ValidateAsync(context)));

		var validationFailures = validationResults
			.Where(validationResult => !validationResult.IsValid)
			.SelectMany(validationResult => validationResult.Errors)
			.ToArray();

		return validationFailures;
	}

	private static ValidationError CreateValidationError(ValidationFailure[] validationFailures) =>
		new(validationFailures.Select(item => Error.Problem(item.PropertyName, item.ErrorMessage)).ToArray());
}
