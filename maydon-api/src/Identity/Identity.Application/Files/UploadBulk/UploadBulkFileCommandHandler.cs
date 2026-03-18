using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Results;
using Identity.Application.Files.Validators;

namespace Identity.Application.Files.UploadBulk;

internal sealed class UploadBulkFileCommandHandler(
	IExecutionContextProvider executionContextProvider,
	IFileManager fileManager) : ICommandHandler<UploadBulkFileCommand, IEnumerable<string>>
{
	private const string BucketName = "uploadtemp";
	public async Task<Result<IEnumerable<string>>> Handle(UploadBulkFileCommand command, CancellationToken cancellationToken)
	{
		var validationResult = command.Files.Select(file => FileValidator.Validate(file));
		if (!validationResult.All(item => item.IsSuccess))
			return Result.Failure<IEnumerable<string>>(validationResult.First(item => item.IsFailure).Error);

		var objectNames = new List<string>();
		foreach (var file in command.Files)
		{
			using var stream = file.OpenReadStream();

			var result = await fileManager.UploadTempFileAsync(
				new FileManagerRequest(
					executionContextProvider.TenantId == Guid.Empty ? BucketName : executionContextProvider.TenantId.ToString(),
					file.FileName,
					file.ContentType,
					file.Length,
					stream), cancellationToken);

			if (result.IsFailure)
				return Result.Failure<IEnumerable<string>>(result.Error);

			objectNames.Add(result.Value);
		}

		return objectNames;
	}
}
