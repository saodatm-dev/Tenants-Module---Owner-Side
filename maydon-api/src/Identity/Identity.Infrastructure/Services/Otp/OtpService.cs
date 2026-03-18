using System.Text;
using System.Text.Json;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Services.Otp;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Services.Otp;

internal sealed class OtpService(
	ILogger<OtpService> logger,
	HttpClient httpClient) : IOtpService
{
	JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
	{
		Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
	};
	public async ValueTask<Result> SendAsync(string phoneNumber, string content, CancellationToken cancellationToken = default)
	{
		try
		{
			var request = new OtpServiceRequest([
				new Message(phoneNumber.Replace('+',' ').Trim(),
					new MessageSms(
						new MessageSmsContent(content)))]);

			var stringContent = new StringContent(JsonSerializer.Serialize(request, jsonSerializerOptions), Encoding.UTF8, "application/json");

			var response = await httpClient.PostAsync("/broker-api/send", stringContent, cancellationToken);
			if (!response.IsSuccessStatusCode)
			{
				logger.LogInformation("An error occured in OtpService.SendAsync : {Response}", await response.Content.ReadAsStringAsync(cancellationToken));
				return Result.None;
			}

			return Result.Success();
		}
		catch (Exception ex)
		{
			logger.LogError("An exception occurred in OtpService.SendAsync : {Message}", ex.Message);
			return Result.None;
		}
	}
}
