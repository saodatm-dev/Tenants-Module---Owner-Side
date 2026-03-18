using Core.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;

namespace Identity.Application.Files.UploadBulk;

public sealed record UploadBulkFileCommand(IEnumerable<IFormFile> Files) : ICommand<IEnumerable<string>>;
