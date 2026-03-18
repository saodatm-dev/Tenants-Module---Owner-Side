using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Results;

namespace Identity.Application.Files.Download;

internal sealed class DownloadFileQueryHandler(IFileManager fileManager) : IQueryHandler<DownloadFileQuery, FileManagerResponse>
{
	public Task<Result<FileManagerResponse>> Handle(DownloadFileQuery request, CancellationToken cancellationToken)
	{
		return fileManager.DownloadFileAsync(Uri.UnescapeDataString(request.Key), cancellationToken);
	}
}
