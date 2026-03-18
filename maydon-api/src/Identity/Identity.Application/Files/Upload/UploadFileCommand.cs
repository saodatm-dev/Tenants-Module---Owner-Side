using Core.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;

namespace Identity.Application.Files.Upload;

public sealed record UploadFileCommand(IFormFile File) : ICommand<string>;
