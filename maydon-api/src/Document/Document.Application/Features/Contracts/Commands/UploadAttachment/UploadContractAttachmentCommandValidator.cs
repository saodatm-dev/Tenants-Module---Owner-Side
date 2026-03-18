using Document.Contract.Contracts.Commands;
using FluentValidation;

namespace Document.Application.Features.Contracts.Commands.UploadAttachment;

public sealed class UploadContractAttachmentCommandValidator : AbstractValidator<UploadContractAttachmentCommand>
{
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

    private static readonly HashSet<string> AllowedContentTypes =
    [
        "application/pdf",
        "image/png",
        "image/jpeg",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
    ];

    public UploadContractAttachmentCommandValidator()
    {
        RuleFor(x => x.ContractId)
            .NotEmpty()
            .WithMessage("ContractId is required");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("FileName is required")
            .MaximumLength(255)
            .WithMessage("FileName must not exceed 255 characters");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("ContentType is required")
            .Must(ct => AllowedContentTypes.Contains(ct))
            .When(x => !string.IsNullOrEmpty(x.ContentType))
            .WithMessage("Only PDF, PNG, JPG, and DOCX files are allowed");

        RuleFor(x => x.FileSize)
            .GreaterThan(0)
            .WithMessage("File must not be empty")
            .LessThanOrEqualTo(MaxFileSizeBytes)
            .WithMessage($"File size must not exceed {MaxFileSizeBytes / (1024 * 1024)} MB");

        RuleFor(x => x.DocumentType)
            .IsInEnum()
            .WithMessage("Invalid document type");
    }
}
