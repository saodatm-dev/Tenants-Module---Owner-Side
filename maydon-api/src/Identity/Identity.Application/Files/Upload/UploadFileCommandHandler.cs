using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Results;
using Identity.Application.Files.Validators;

namespace Identity.Application.Files.Upload;

internal sealed class UploadFileCommandHandler(
	IExecutionContextProvider executionContextProvider,
	IFileManager fileManager) : ICommandHandler<UploadFileCommand, string>
{
	private const string BucketName = "uploadtemp";
	public async Task<Result<string>> Handle(UploadFileCommand command, CancellationToken cancellationToken)
	{
		var validationResult = FileValidator.Validate(command.File);
		if (validationResult.IsFailure)
			return Result.Failure<string>(validationResult.Error);

		using var stream = command.File.OpenReadStream();

		var result = await fileManager.UploadTempFileAsync(
			new FileManagerRequest(
				executionContextProvider.TenantId == Guid.Empty ? BucketName : executionContextProvider.TenantId.ToString(),
				command.File.FileName,
				command.File.ContentType,
				command.File.Length,
				stream), cancellationToken);

		if (result.IsFailure)
			return result;

		return result;
	}
}
