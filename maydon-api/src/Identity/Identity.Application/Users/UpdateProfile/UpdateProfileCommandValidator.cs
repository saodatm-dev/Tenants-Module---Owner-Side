using FluentValidation;

namespace Identity.Application.Users.UpdateProfile;

internal sealed class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
	public UpdateProfileCommandValidator()
	{
		When(x => x.FirstName is not null, () =>
		{
			RuleFor(x => x.FirstName)
				.MaximumLength(100);
		});

		When(x => x.LastName is not null, () =>
		{
			RuleFor(x => x.LastName)
				.MaximumLength(100);
		});

		When(x => x.MiddleName is not null, () =>
		{
			RuleFor(x => x.MiddleName)
				.MaximumLength(100);
		});
	}
}
