using Core.Domain.Results;

namespace Core.Application.Abstractions.Pdf;

/// <summary>
/// Service for rendering HTML content to PDF
/// </summary>
public interface IPdfRenderer
{
	Task<Result<byte[]>> ConvertHtmlToPdfAsync(string html, PdfRenderOptions? options = null, CancellationToken cancellationToken = default);
	Task<Result<Stream>> ConvertHtmlToStreamAsync(string html, PdfRenderOptions? options = null, CancellationToken cancellationToken = default);
	Task<Result<string>> ConvertHtmlToPdfBase64Async(string html, PdfRenderOptions? options = null, CancellationToken cancellationToken = default);
	Task<Result<byte[]>> ConvertMultipleHtmlToPdfAsync(Dictionary<string, string> htmlFiles, PdfRenderOptions? options = null, CancellationToken cancellationToken = default);
}
