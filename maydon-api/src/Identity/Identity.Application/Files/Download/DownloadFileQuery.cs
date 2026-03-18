using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;

namespace Identity.Application.Files.Download;

public sealed record DownloadFileQuery(string Key) : IQuery<FileManagerResponse>;
