using Core.Application.Resources;
using FluentValidation;

namespace Identity.Application.Files.Upload;

internal sealed class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
{
	public UploadFileCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.File)
			.NotEmpty()
			.Must(item => item.Length > 0)
			.WithMessage(sharedViewLocalizer.IsEmpty(nameof(UploadFileCommand.File)).Description);
	}
}
