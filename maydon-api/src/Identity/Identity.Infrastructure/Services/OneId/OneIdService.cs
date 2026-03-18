using System.Text.Json;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Services.OneId;
using Identity.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Services.OneId;

internal sealed class OneIdService(
	ILogger<OneIdService> logger,
	HttpClient httpClient,
	IOptions<OneIdOptions> options) : IOneIdService
{
	private readonly string AuthorizationEndpoint = "sso/oauth/Authorization.do?grant_type=one_authorization_code&client_id={0}&client_secret={1}&code={2}";
	private readonly string DataEndpoint = "sso/oauth/Authorization.do?grant_type=one_access_token_identify&client_id={0}&client_secret={1}&access_token={2}";

	public async ValueTask<Result<OneIdAccessTokenResponse>> AuthorizationAsync(Guid code, CancellationToken cancellationToken)
	{
		try
		{
			string url = string.Format(AuthorizationEndpoint, options.Value.ClientId, options.Value.ClientSecret, code);

			var response = await httpClient.PostAsync(url, null, cancellationToken);
			if (response.IsSuccessStatusCode)
			{
				var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
				if (string.IsNullOrWhiteSpace(responseContent))
					return Result<OneIdAccessTokenResponse>.None;

				var result = JsonSerializer.Deserialize<OneIdAccessTokenResponse>(responseContent);
				if (result is null)
					return Result<OneIdAccessTokenResponse>.None;

				logger.LogInformation("An result in OneIdHttpClient => AuthorizationAsync : {Message}", result);

				return Result.Success<OneIdAccessTokenResponse>(result);
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An exception occurred in OneIdHttpClient => AuthorizationAsync : {Message}", ex.Message);
		}

		return Result<OneIdAccessTokenResponse>.None;
	}
	public async ValueTask<Result<OneIdResponse>> GetAsync(Guid accessToken, CancellationToken cancellationToken)
	{
		try
		{
			string url = string.Format(DataEndpoint, options.Value.ClientId, options.Value.ClientSecret, accessToken);

			var response = await httpClient.PostAsync(url, null, cancellationToken);
			if (response.IsSuccessStatusCode)
			{
				var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
				if (string.IsNullOrWhiteSpace(responseContent))
					return Result<OneIdResponse>.None;

				var result = JsonSerializer.Deserialize<OneIdResponse>(responseContent);
				if (result is null)
					return Result<OneIdResponse>.None;

				logger.LogInformation("An result in OneIdHttpClient => GetAccessTokenByCodeAsync : {Message}", result);

				return Result.Success<OneIdResponse>(result);
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An exception occurred in OneIdHttpClient => GetAccessTokenByCodeAsync : {Message}", ex.Message);
		}

		return Result<OneIdResponse>.None;
	}
}
