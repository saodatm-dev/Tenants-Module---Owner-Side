using Core.Domain.Results;
using Identity.Application.Files.Formats;
using Microsoft.AspNetCore.Http;

namespace Identity.Application.Files.Validators;

internal sealed class FileValidator
{
	private static readonly List<FileFormatDescriptor> AllowedFormats = [new Image(), new MSOffice(), new Pdf()];
	private static int Size = 1024 * 1024 * 25; // 25 mb
	public static Result Validate(IFormFile file)
	{
		var fileExtension = Path.GetExtension(file.FileName);
		var targetType = AllowedFormats.FirstOrDefault(x => x.IsIncludedExtension(fileExtension));
		if (targetType is null)
		{
			return Result.Failure("not_supported", "Not supported format");
		}
		var result = targetType.Validate(file);
		if (result.IsFailure)
			return result;

		return ValidateSize(file);
	}

	private static Result ValidateSize(IFormFile file)
	{
		if (file.Length == 0 || file.Length > Size)
			return Result.Failure("size_is_large", "File size exceeds the maximum allowed size (25 MB).");

		return Result.Success();
	}
}
