using Core.Domain.Results;

namespace Core.Application.Abstractions.Services.Minio;

public interface IFileManager
{
	Task<Result<string>> UploadTempFileAsync(FileManagerRequest request, CancellationToken cancellationToken);
	Task<Result<string>> UploadPublicFileAsync(FileManagerRequest request, CancellationToken cancellationToken);
	Task<Result<string>> UploadPrivateFileAsync(FileManagerRequest request, CancellationToken cancellationToken);
	Task<Result<FileManagerResponse>> DownloadFileAsync(string key, CancellationToken cancellationToken);
	Task<Result<string>> GetPresignedUrlAsync(string key, int expiryInSeconds = 3600, CancellationToken cancellationToken = default);
	Task<Result> DeleteFileAsync(string key, CancellationToken cancellationToken);
	Task<Result<string>> CopyToPublicAsync(string key, string bucketName, bool deleteSource = false, CancellationToken cancellationToken = default);
	Task<Result<string>> CopyToPrivateAsync(string key, string bucketName, bool deleteSource = false, CancellationToken cancellationToken = default);
}
