using System.Text.Json;
using Core.Application.Resources;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Services.EImzo;

using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Services.EImzo;

public sealed class EImzoService(
	ILogger<EImzoService> logger,
	ISharedViewLocalizer sharedViewLocalizer,
	HttpClient httpClient) : IEImzoService
{
	private const string AuthorizationEndpoint = "/backend/auth";
	private const string ChallengeEndpoint = "/frontend/challenge";
	private const string PingEndpoint = "/ping";

	private const string MobileAuthInitEndpoint = "/frontend/mobile/auth";
	private const string MobileAuthStatusEndpoint = "/frontend/mobile/status";
	private const string MobileAuthenticateEndpoint = "/backend/mobile/authenticate";

	public async ValueTask<Result<AuthResponse>> AuthAsync(string pkcs7, CancellationToken cancellationToken)
	{
		try
		{
			var stringContent = new StringContent(pkcs7);

			var response = await httpClient.PostAsync(AuthorizationEndpoint, stringContent, cancellationToken);

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync(cancellationToken);
				if (string.IsNullOrWhiteSpace(content))
				{
					logger.LogWarning("EImzo AuthAsync returned empty response body");
					return Result.Failure<AuthResponse>(sharedViewLocalizer.EImzoAuthFailed(nameof(pkcs7)));
				}

				var authResponse = JsonSerializer.Deserialize<AuthResponse>(content);
				if (authResponse is null || authResponse.Status != 1)
				{
					logger.LogWarning("EImzo AuthAsync failed. Status: {Status}, Response: {Response}",
						authResponse?.Status, content);
					return Result.Failure<AuthResponse>(sharedViewLocalizer.EImzoAuthFailed(nameof(pkcs7)));
				}

				return Result.Success(authResponse);
			}

			var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
			logger.LogWarning("EImzo AuthAsync HTTP error. StatusCode: {StatusCode}, Body: {Body}",
				response.StatusCode, errorBody);
			return Result.Failure<AuthResponse>(sharedViewLocalizer.EImzoServiceUnavailable(nameof(pkcs7)));
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An exception occurred in AuthAsync : {Message}", ex.Message);
			return Result.Failure<AuthResponse>(sharedViewLocalizer.EImzoServiceUnavailable(nameof(pkcs7)));
		}
	}

	public async ValueTask<Result<ChallengeResponse>> ChallengeAsync(CancellationToken cancellationToken)
	{
		try
		{
			var response = await httpClient.GetAsync(ChallengeEndpoint, cancellationToken);

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync(cancellationToken);
				if (string.IsNullOrWhiteSpace(content))
				{
					logger.LogWarning("EImzo ChallengeAsync returned empty response body");
					return Result.Failure<ChallengeResponse>(sharedViewLocalizer.EImzoServiceUnavailable("challenge"));
				}

				var challengeResponse = JsonSerializer.Deserialize<ChallengeResponse>(content);
				if (challengeResponse is null)
				{
					logger.LogWarning("EImzo ChallengeAsync deserialization failed. Response: {Response}", content);
					return Result.Failure<ChallengeResponse>(sharedViewLocalizer.EImzoServiceUnavailable("challenge"));
				}

				return Result.Success(challengeResponse);
			}

			var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
			logger.LogWarning("EImzo ChallengeAsync HTTP error. StatusCode: {StatusCode}, Body: {Body}",
				response.StatusCode, errorBody);
			return Result.Failure<ChallengeResponse>(sharedViewLocalizer.EImzoServiceUnavailable("challenge"));
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An exception occurred in ChallengeAsync : {Message}", ex.Message);
			return Result.Failure<ChallengeResponse>(sharedViewLocalizer.EImzoServiceUnavailable("challenge"));
		}
	}

	public async ValueTask<Result<PingResponse>> PingAsync(CancellationToken cancellationToken)
	{
		try
		{
			var response = await httpClient.GetAsync(PingEndpoint, cancellationToken);

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync(cancellationToken);
				if (string.IsNullOrWhiteSpace(content))
				{
					logger.LogWarning("EImzo PingAsync returned empty response body");
					return Result.Failure<PingResponse>(sharedViewLocalizer.EImzoServiceUnavailable("ping"));
				}

				var pingResponse = JsonSerializer.Deserialize<PingResponse>(content);
				if (pingResponse is null)
				{
					logger.LogWarning("EImzo PingAsync deserialization failed. Response: {Response}", content);
					return Result.Failure<PingResponse>(sharedViewLocalizer.EImzoServiceUnavailable("ping"));
				}

				return Result.Success(pingResponse);
			}

			var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
			logger.LogWarning("EImzo PingAsync HTTP error. StatusCode: {StatusCode}, Body: {Body}",
				response.StatusCode, errorBody);
			return Result.Failure<PingResponse>(sharedViewLocalizer.EImzoServiceUnavailable("ping"));
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An exception occurred in PingAsync : {Message}", ex.Message);
			return Result.Failure<PingResponse>(sharedViewLocalizer.EImzoServiceUnavailable("ping"));
		}
	}

	public async ValueTask<Result<MobileAuthInitResponse>> MobileAuthInitAsync(CancellationToken cancellationToken)
	{
		try
		{
			var response = await httpClient.PostAsync(MobileAuthInitEndpoint, null, cancellationToken);

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync(cancellationToken);
				if (string.IsNullOrWhiteSpace(content))
				{
					logger.LogWarning("EImzo MobileAuthInitAsync returned empty response body");
					return Result.Failure<MobileAuthInitResponse>(sharedViewLocalizer.EImzoServiceUnavailable("mobileAuthInit"));
				}

				var mobileAuthInitResponse = JsonSerializer.Deserialize<MobileAuthInitResponse>(content);
				if (mobileAuthInitResponse is null || mobileAuthInitResponse.Status != 1)
				{
					logger.LogWarning("EImzo MobileAuthInitAsync failed. Status: {Status}, Response: {Response}",
						mobileAuthInitResponse?.Status, content);
					return Result.Failure<MobileAuthInitResponse>(sharedViewLocalizer.EImzoAuthFailed("mobileAuthInit"));
				}

				return Result.Success(mobileAuthInitResponse);
			}

			var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
			logger.LogWarning("EImzo MobileAuthInitAsync HTTP error. StatusCode: {StatusCode}, Body: {Body}",
				response.StatusCode, errorBody);
			return Result.Failure<MobileAuthInitResponse>(sharedViewLocalizer.EImzoServiceUnavailable("mobileAuthInit"));
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An exception occurred in MobileAuthInitAsync : {Message}", ex.Message);
			return Result.Failure<MobileAuthInitResponse>(sharedViewLocalizer.EImzoServiceUnavailable("mobileAuthInit"));
		}
	}

	public async ValueTask<Result<MobileAuthStatusResponse>> MobileAuthStatusAsync(string documentId, CancellationToken cancellationToken)
	{
		try
		{
			var content = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("documentId", documentId)
			});

			var response = await httpClient.PostAsync(MobileAuthStatusEndpoint, content, cancellationToken);

			if (response.IsSuccessStatusCode)
			{
				var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
				if (string.IsNullOrWhiteSpace(responseContent))
				{
					logger.LogWarning("EImzo MobileAuthStatusAsync returned empty response body for DocumentId: {DocumentId}", documentId);
					return Result.Failure<MobileAuthStatusResponse>(sharedViewLocalizer.EImzoServiceUnavailable(nameof(documentId)));
				}

				var statusResponse = JsonSerializer.Deserialize<MobileAuthStatusResponse>(responseContent);
				if (statusResponse is null)
				{
					logger.LogWarning("EImzo MobileAuthStatusAsync deserialization failed. Response: {Response}", responseContent);
					return Result.Failure<MobileAuthStatusResponse>(sharedViewLocalizer.EImzoServiceUnavailable(nameof(documentId)));
				}

				return Result.Success(statusResponse);
			}

			var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
			logger.LogWarning("EImzo MobileAuthStatusAsync HTTP error. StatusCode: {StatusCode}, Body: {Body}",
				response.StatusCode, errorBody);
			return Result.Failure<MobileAuthStatusResponse>(sharedViewLocalizer.EImzoServiceUnavailable(nameof(documentId)));
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An exception occurred in MobileAuthStatusAsync : {Message}", ex.Message);
			return Result.Failure<MobileAuthStatusResponse>(sharedViewLocalizer.EImzoServiceUnavailable(nameof(documentId)));
		}
	}

	public async ValueTask<Result<AuthResponse>> MobileAuthenticateAsync(string documentId, CancellationToken cancellationToken)
	{
		try
		{
			var endpoint = $"{MobileAuthenticateEndpoint}/{documentId}";
			var response = await httpClient.GetAsync(endpoint, cancellationToken);

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync(cancellationToken);
				if (string.IsNullOrWhiteSpace(content))
				{
					logger.LogWarning("EImzo MobileAuthenticateAsync returned empty response body for DocumentId: {DocumentId}", documentId);
					return Result.Failure<AuthResponse>(sharedViewLocalizer.EImzoAuthFailed(nameof(documentId)));
				}

				var authenticateResponse = JsonSerializer.Deserialize<AuthResponse>(content);
				if (authenticateResponse is null || authenticateResponse.Status != 1)
				{
					logger.LogWarning("EImzo MobileAuthenticateAsync failed. Status: {Status}, Response: {Response}",
						authenticateResponse?.Status, content);
					return Result.Failure<AuthResponse>(sharedViewLocalizer.EImzoAuthFailed(nameof(documentId)));
				}

				return Result.Success(authenticateResponse);
			}

			var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
			logger.LogWarning("EImzo MobileAuthenticateAsync HTTP error. StatusCode: {StatusCode}, Body: {Body}",
				response.StatusCode, errorBody);
			return Result.Failure<AuthResponse>(sharedViewLocalizer.EImzoServiceUnavailable(nameof(documentId)));
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An exception occurred in MobileAuthenticateAsync : {Message}", ex.Message);
			return Result.Failure<AuthResponse>(sharedViewLocalizer.EImzoServiceUnavailable(nameof(documentId)));
		}
	}
}
