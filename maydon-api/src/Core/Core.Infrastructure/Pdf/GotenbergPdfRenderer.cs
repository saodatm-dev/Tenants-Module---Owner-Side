using System.Text;
using Core.Application.Abstractions.Pdf;
using Core.Domain.Results;
using Microsoft.Extensions.Logging;
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace Core.Infrastructure.Pdf;

/// <summary>
/// PDF renderer using Gotenberg service
/// </summary>
public sealed class GotenbergPdfRenderer(HttpClient httpClient, ILogger<GotenbergPdfRenderer> logger) : IPdfRenderer
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly ILogger<GotenbergPdfRenderer> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<Result<byte[]>> ConvertHtmlToPdfAsync(string html, PdfRenderOptions? options = null, CancellationToken cancellationToken = default)
    {
        // Use stream API and buffer to byte array
        var streamResult = await ConvertHtmlToStreamAsync(html, options, cancellationToken);

        if (streamResult.IsFailure)
        {
            return Result.Failure<byte[]>(streamResult.Error);
        }

        try
        {
            await using var stream = streamResult.Value;
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, cancellationToken);
            
            var bytes = memoryStream.ToArray();
            
            _logger.LogInformation("PDF generated successfully. Size: {Size} KB", bytes.Length / 1024.0);
            
            return Result.Success(bytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read PDF stream");
            return Result.Failure<byte[]>(Error.Failure("PdfRenderer.StreamReadError", "Failed to read PDF stream"));
        }
    }

    /// <inheritdoc />
    public async Task<Result<Stream>> ConvertHtmlToStreamAsync(string html, PdfRenderOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return Result.Failure<Stream>(Error.Validation("PdfRenderer.InvalidInput", "HTML content cannot be empty"));
        }

        options ??= new PdfRenderOptions();

        HttpResponseMessage response;
        MultipartFormDataContent form;

        try
        {
            form = CreateMultipartFormData(html, options);

            _logger.LogDebug("Sending PDF conversion request to Gotenberg");
            
            using var request = new HttpRequestMessage(HttpMethod.Post, "/forms/chromium/convert/html");
            request.Content = form;

            response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                
                _logger.LogError("PDF rendering failed. Status={Status}, Error={Error}", response.StatusCode, errorContent);

                response.Dispose();
                form.Dispose();

                return Result.Failure<Stream>(Error.Failure("PdfRenderer.RenderingFailed", $"PDF rendering failed with status {response.StatusCode}: {errorContent}"));
            }

            var contentType = response.Content.Headers.ContentType?.MediaType;
            if (contentType != "application/pdf")
            {
                _logger.LogError("Unexpected response content type: {ContentType}", contentType);

                response.Dispose();
                form.Dispose();

                return Result.Failure<Stream>(Error.Failure("PdfRenderer.InvalidContentType", $"Gotenberg returned unexpected content type: {contentType}"));
            }

            var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);

            // Log content length if available
            if (response.Content.Headers.ContentLength.HasValue)
            {
                _logger.LogDebug("PDF stream ready. Size: {Size} KB", response.Content.Headers.ContentLength.Value / 1024.0);
            }
            
            // Transfer ownership of response and form to the stream
            var disposableStream = new DisposableStream(contentStream, response, form);

            return Result.Success<Stream>(disposableStream);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request to Gotenberg failed");

            return Result.Failure<Stream>(Error.Failure("PdfRenderer.ConnectionFailed", $"Failed to connect to Gotenberg service: {ex.Message}"));
        }
        catch (TaskCanceledException ex) when (ex.CancellationToken == cancellationToken)
        {
            _logger.LogWarning("PDF rendering request was cancelled");

            return Result.Failure<Stream>(Error.Failure("PdfRenderer.Cancelled", "PDF rendering request was cancelled"));
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "PDF rendering request timed out");

            return Result.Failure<Stream>(Error.Failure("PdfRenderer.Timeout", "PDF rendering request timed out"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during PDF rendering");

            return Result.Failure<Stream>(Error.Failure("PdfRenderer.UnexpectedError", $"An unexpected error occurred: {ex.Message}"));
        }
    }

    /// <inheritdoc />
    public async Task<Result<string>> ConvertHtmlToPdfBase64Async(string html, PdfRenderOptions? options = null, CancellationToken cancellationToken = default)
    {
        var result = await ConvertHtmlToPdfAsync(html, options, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<string>(result.Error);
        }

        try
        {
            var base64 = Convert.ToBase64String(result.Value);
            return Result.Success(base64);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to convert PDF to Base64");
            return Result.Failure<string>(Error.Failure("PdfRenderer.Base64ConversionError", "Failed to convert PDF to Base64"));
        }
    }

    /// <inheritdoc />
    public async Task<Result<byte[]>> ConvertMultipleHtmlToPdfAsync(
        Dictionary<string, string> htmlFiles,
        PdfRenderOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (htmlFiles == null || htmlFiles.Count == 0)
        {
            return Result.Failure<byte[]>(Error.Validation("PdfRenderer.InvalidInput", "HTML files dictionary cannot be empty"));
        }

        options ??= new PdfRenderOptions();

        HttpResponseMessage? response = null;
        MultipartFormDataContent? form = null;

        try
        {
            form = new MultipartFormDataContent();

            // Add all HTML files
            foreach (var (fileName, htmlContent) in htmlFiles)
            {
                if (string.IsNullOrWhiteSpace(htmlContent))
                {
                    _logger.LogWarning("Skipping empty HTML file: {FileName}", fileName);
                    continue;
                }

                var content = new StringContent(htmlContent, Encoding.UTF8, "text/html");
                
                // Ensure filename ends with .html
                var normalizedFileName = fileName.EndsWith(".html", StringComparison.OrdinalIgnoreCase)
                    ? fileName
                    : $"{fileName}.html";
                
                form.Add(content, "files", normalizedFileName);
            }

            // Add Gotenberg options
            AddGotenbergOptions(form, options);

            _logger.LogDebug("Sending multi-file PDF conversion request to Gotenberg. Files: {Count}", htmlFiles.Count);

            response = await _httpClient.PostAsync("/forms/chromium/convert/html", form, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                
                _logger.LogError(
                    "Multi-file PDF rendering failed. Status={Status}, Error={Error}", response.StatusCode, errorContent);

                return Result.Failure<byte[]>(Error.Failure("PdfRenderer.RenderingFailed", $"PDF rendering failed with status {response.StatusCode}: {errorContent}"));
            }

            var pdfBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);

            _logger.LogInformation(
                "Multi-file PDF generated successfully. Files: {Count}, Size: {Size} KB", htmlFiles.Count, pdfBytes.Length / 1024.0);

            return Result.Success(pdfBytes);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request to Gotenberg failed");
            return Result.Failure<byte[]>(Error.Failure("PdfRenderer.ConnectionFailed", $"Failed to connect to Gotenberg service: {ex.Message}"));
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "PDF rendering request timed out");
            return Result.Failure<byte[]>(Error.Failure("PdfRenderer.Timeout", "PDF rendering request timed out"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during multi-file PDF rendering");
            return Result.Failure<byte[]>(Error.Failure("PdfRenderer.UnexpectedError", $"An unexpected error occurred: {ex.Message}"));
        }
        finally
        {
            response?.Dispose();
            form?.Dispose();
        }
    }

    #region Private Helper Methods

    private static MultipartFormDataContent CreateMultipartFormData(string html, PdfRenderOptions options)
    {
        var form = new MultipartFormDataContent();

        // Add main HTML content
        var htmlContent = new StringContent(html, Encoding.UTF8, "text/html");
        form.Add(htmlContent, "files", "index.html");

        // Add header if provided
        if (!string.IsNullOrEmpty(options.HeaderHtml))
        {
            var headerContent = new StringContent(options.HeaderHtml, Encoding.UTF8, "text/html");
            form.Add(headerContent, "files", "header.html");
        }

        // Add footer if provided
        if (!string.IsNullOrEmpty(options.FooterHtml))
        {
            var footerContent = new StringContent(options.FooterHtml, Encoding.UTF8, "text/html");
            form.Add(footerContent, "files", "footer.html");
        }

        // Add Gotenberg options
        AddGotenbergOptions(form, options);

        return form;
    }

    private static void AddGotenbergOptions(MultipartFormDataContent form, PdfRenderOptions options)
    {
        // Paper dimensions (REQUIRED - in inches)
        form.Add(new StringContent(options.PaperWidth), "paperWidth");
        form.Add(new StringContent(options.PaperHeight), "paperHeight");

        // Margins (in inches - NOT millimeters!)
        form.Add(new StringContent(options.MarginTop), "marginTop");
        form.Add(new StringContent(options.MarginBottom), "marginBottom");
        form.Add(new StringContent(options.MarginLeft), "marginLeft");
        form.Add(new StringContent(options.MarginRight), "marginRight");

        // Orientation
        if (options.Landscape)
        {
            form.Add(new StringContent("true"), "landscape");
        }

        // Background printing
        if (options.PrintBackground)
        {
            form.Add(new StringContent("true"), "printBackground");
        }

        // Omit background (transparent)
        if (options.OmitBackground)
        {
            form.Add(new StringContent("true"), "omitBackground");
        }

        // Scale
        if (!string.IsNullOrEmpty(options.Scale) && options.Scale != "1.0")
        {
            form.Add(new StringContent(options.Scale), "scale");
        }

        // PDF format
        if (!string.IsNullOrEmpty(options.PdfFormat))
        {
            form.Add(new StringContent(options.PdfFormat), "pdfa");
        }

        // Wait settings
        if (!string.IsNullOrEmpty(options.WaitDelay))
        {
            form.Add(new StringContent(options.WaitDelay), "waitDelay");
        }

        if (!string.IsNullOrEmpty(options.WaitForExpression))
        {
            form.Add(new StringContent(options.WaitForExpression), "waitForExpression");
        }

        // Skip network idle for faster rendering
        if (options.SkipNetworkIdleEvent)
        {
            form.Add(new StringContent("true"), "skipNetworkIdleEvent");
        }

        // CSS page size preference
        if (options.PreferCssPageSize)
        {
            form.Add(new StringContent("true"), "preferCssPageSize");
        }

        // Document outline
        if (options.GenerateDocumentOutline)
        {
            form.Add(new StringContent("true"), "generateDocumentOutline");
        }
    }
    
    #endregion
}
