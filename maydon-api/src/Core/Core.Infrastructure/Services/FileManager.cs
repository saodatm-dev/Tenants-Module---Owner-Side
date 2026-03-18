using Core.Application.Abstractions.Services.Minio;
using Core.Application.Resources;
using Core.Domain.Results;
using Core.Infrastructure.Helpers;
using Core.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Response;
using Minio.Exceptions;

namespace Core.Infrastructure.Services;

internal sealed class 
    FileManager(
	ILogger<FileManager> logger,
	ISharedViewLocalizer sharedViewLocalizer,
	IMinioClient client,
	IOptions<CoreOptions> options) : IFileManager
{
	private const string TempDirectoryName = "temporary";
	private const string PublicDirectoryName = "public";
	private const string PrivateDirectoryName = "private";
	private const string KeySplitter = "_###_";
	private const string NameSplitter = "__#####__";

	public Task<Result<string>> UploadTempFileAsync(FileManagerRequest request, CancellationToken cancellationToken) =>
		UploadFileAsync(request, $"{TempDirectoryName}/{Guid.CreateVersion7()}{NameSplitter}{request.Name}", cancellationToken);
	
	public Task<Result<string>> UploadPublicFileAsync(FileManagerRequest request, CancellationToken cancellationToken) =>
		UploadFileAsync(request, $"{PublicDirectoryName}/{Guid.CreateVersion7()}{NameSplitter}{request.Name}", cancellationToken);
	
	public Task<Result<string>> UploadPrivateFileAsync(FileManagerRequest request, CancellationToken cancellationToken) =>
		UploadFileAsync(request, $"{PrivateDirectoryName}/{Guid.CreateVersion7()}{NameSplitter}{request.Name}", cancellationToken);
	
	public async Task<Result<FileManagerResponse>> DownloadFileAsync(string key, CancellationToken cancellationToken)
	{
		try
		{
			//await Task.Delay(1000, cancellationToken);

			var result = Decrypt(key);
			if (result.IsFailure)
				return Result.Failure<FileManagerResponse>(result.Error);

			var objectStat = await client.StatObjectAsync(new StatObjectArgs()
				.WithBucket(result.Value.BucketName)
				.WithObject(result.Value.ObjectName), cancellationToken);

			if (objectStat is null)
				return Result<FileManagerResponse>.None;

			var stream = new MemoryStream();
			var tsc = new TaskCompletionSource<bool>();

			var getObjectArgs = new GetObjectArgs()
				.WithBucket(result.Value.BucketName)
				.WithObject(result.Value.ObjectName)
				.WithCallbackStream(cs =>
				{
					cs.CopyTo(stream);
					tsc.SetResult(true);
				});

			await client.GetObjectAsync(getObjectArgs, cancellationToken);
			await tsc.Task;
			stream.Seek(0, SeekOrigin.Begin);

			//remove unique prefix from object name
			string objectName = result.Value.ObjectName;
			var nameParts = result.Value.ObjectName.Split(NameSplitter);
			if (nameParts.Length == 2)
				objectName = nameParts[1];

			return new FileManagerResponse(stream.ToArray(), objectName, objectStat.ContentType, objectStat.Size);
		}
		catch (Exception ex)
		{
			logger.LogError("An occured exception while downloading file {Messages}", ex.Message);

			return Result.Failure<FileManagerResponse>(sharedViewLocalizer.StorageDownloadFailed(key));
		}
	}


	public async Task<Result<string>> GetPresignedUrlAsync(string key, int expiryInSeconds = 3600, CancellationToken cancellationToken = default)
	{
		try
		{
			var result = Decrypt(key);
			if (result.IsFailure)
				return Result.Failure<string>(result.Error);

			var presignedUrl = await client.PresignedGetObjectAsync(new PresignedGetObjectArgs()
				.WithBucket(result.Value.BucketName)
				.WithObject(result.Value.ObjectName)
				.WithExpiry(expiryInSeconds));

			return Result.Success(presignedUrl.Replace("http://", "https://"));
		}
		catch (Exception ex)
		{
			logger.LogError("An exception occurred while generating presigned URL: {Message}", ex.Message);
			return Result.Failure<string>(sharedViewLocalizer.StorageDownloadFailed(key));
		}
	}

	public async Task<Result> DeleteFileAsync(string key, CancellationToken cancellationToken)
	{
		try
		{
			var result = Decrypt(key);
			if (result.IsFailure)
				return Result.Failure(result.Error);

			await client.RemoveObjectAsync(new RemoveObjectArgs()
				.WithBucket(result.Value.BucketName)
				.WithObject(result.Value.ObjectName), cancellationToken);

			return Result.Success();
		}
		catch (Exception ex)
		{
			logger.LogError("An occured exception while deleting file {Messages}", ex.Message);

			return Result.Failure(sharedViewLocalizer.MinioApiDeleteFailed(key));
		}
	}
	
	public Task<Result<string>> CopyToPublicAsync(string key, string bucketName, bool deleteSource = false, CancellationToken cancellationToken = default) =>
		CopyAsync(key, bucketName, true, deleteSource, cancellationToken);
	
	public Task<Result<string>> CopyToPrivateAsync(string key, string bucketName, bool deleteSource = false, CancellationToken cancellationToken = default) =>
		CopyAsync(key, bucketName, false, deleteSource, cancellationToken);
	
	private Task<bool> BucketExistsAsync(string name, CancellationToken cancellationToken) =>
		client.BucketExistsAsync(new BucketExistsArgs().WithBucket(name), cancellationToken);
	
	private async Task<Result> CreateIfNotExistBucketAsync(string name, CancellationToken cancellationToken)
	{
		try
		{
			var result = await BucketExistsAsync(name, cancellationToken);
			if (!result)
				await client.MakeBucketAsync(new MakeBucketArgs().WithBucket(name), cancellationToken);

			return Result.Success();
		}
		catch (InvalidBucketNameException invalidBucketNameEx)
		{
			logger.LogError(invalidBucketNameEx, "Invalid bucket name");

			await client.MakeBucketAsync(new MakeBucketArgs().WithBucket(name), cancellationToken);

			return Result.Success();
		}
		catch (Exception ex)
		{
			logger.LogError("An occured exception while creating bucket {Messages}", ex.Message);
			return Result.Failure(sharedViewLocalizer.InvalidValue(name));
		}
	}
	
	private async Task<Result<string>> UploadFileAsync(FileManagerRequest request, string objectName, CancellationToken cancellationToken)
	{
		try
		{
			var bucketResult = await CreateIfNotExistBucketAsync(request.BucketName, cancellationToken);
			if (bucketResult.IsFailure)
				return Result.Failure<string>(bucketResult.Error);

			PutObjectResponse? putObjectResponse = await client.PutObjectAsync(
				new PutObjectArgs()
					.WithBucket(request.BucketName)
					.WithObject(objectName)
					.WithStreamData(request.Stream)
					.WithObjectSize(request.Length)
					.WithContentType(request.Type), cancellationToken);

			if (putObjectResponse.ResponseStatusCode == System.Net.HttpStatusCode.OK)
				return Encrypt(request.BucketName, objectName);

			return Result.Failure<string>(sharedViewLocalizer.MinioApiUploadFailed(request.Name));

		}
		catch (Exception ex)
		{
			logger.LogError("An occured exception while uploading file {Messages}", ex.Message);

			return Result.Failure<string>(sharedViewLocalizer.MinioApiUploadFailed(request.Name));
		}
	}
	
	private Result<string> Encrypt(string bucketName, string fileName)
	{
		var encryptedKey = AesEncryptionHelper.Encrypt($"{bucketName}{KeySplitter}{fileName}", options.Value.MinIOSecret);
		if (string.IsNullOrWhiteSpace(encryptedKey))
			return Result.Failure<string>(sharedViewLocalizer.MinioApiUploadFailed(fileName));

		return Result.Success(Uri.EscapeDataString(encryptedKey));
	}
	
	private Result<(string BucketName, string ObjectName)> Decrypt(string key)
	{
		var decryptedKey = AesEncryptionHelper.Decrypt(Uri.UnescapeDataString(key), options.Value.MinIOSecret);
		if (string.IsNullOrWhiteSpace(decryptedKey))
			return Result.Failure<(string BucketName, string ObjectName)>(sharedViewLocalizer.MinioApiDeleteFailed(key));

		var keyValues = decryptedKey.Split(KeySplitter);
		if (keyValues.Length != 2)
			return Result.Failure<(string BucketName, string ObjectName)>(sharedViewLocalizer.MinioApiDeleteFailed(key));

		return Result.Success((keyValues[0], keyValues[1]));
	}
	
	private async Task<Result<string>> CopyAsync(string key, string destinationBucketName, bool isPublic, bool deleteSource = false, CancellationToken cancellationToken = default)
	{
		try
		{
			var decryptedResult = Decrypt(key);
			if (decryptedResult.IsFailure)
				return Result.Failure<string>(decryptedResult.Error);

			var bucketResult = await CreateIfNotExistBucketAsync(destinationBucketName, cancellationToken);
			if (bucketResult.IsFailure)
				return Result.Failure<string>(bucketResult.Error);

			// clean up object name

			string objectName = decryptedResult.Value.ObjectName
				.Replace(TempDirectoryName, string.Empty)
				.Replace(PublicDirectoryName, string.Empty)
				.Replace(PrivateDirectoryName, string.Empty);

			var destinationObjectName = $"{(isPublic ? PublicDirectoryName : PrivateDirectoryName)}{objectName}";

			var sourceObjectArgs = new CopySourceObjectArgs()
				.WithBucket(decryptedResult.Value.BucketName)
				.WithObject(decryptedResult.Value.ObjectName);

			var copyObjectArgs = new CopyObjectArgs()
				.WithBucket(destinationBucketName)
				.WithObject(destinationObjectName)
				.WithCopyObjectSource(sourceObjectArgs);

			await client.CopyObjectAsync(copyObjectArgs, cancellationToken);

			if (deleteSource)
				await DeleteFileAsync(key, cancellationToken);

			return Encrypt(destinationBucketName, destinationObjectName);
		}
		catch (Exception ex)
		{
			logger.LogError("An occured exception while copying file {Messages}", ex.Message);

			return Result.Failure<string>(sharedViewLocalizer.MinioApiUploadFailed(destinationBucketName));
		}
	}
}
