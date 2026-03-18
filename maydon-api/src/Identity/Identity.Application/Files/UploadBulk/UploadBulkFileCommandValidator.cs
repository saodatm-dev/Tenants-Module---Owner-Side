using Core.Application.Resources;
using FluentValidation;

namespace Identity.Application.Files.UploadBulk;

internal sealed class UploadBulkFileCommandValidator : AbstractValidator<UploadBulkFileCommand>
{
	public UploadBulkFileCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Files)
			.NotEmpty()
			.Must(item => item != null && item.Any())
			.WithMessage(sharedViewLocalizer.IsEmpty(nameof(UploadBulkFileCommand.Files)).Description);
	}
}
