using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Core.Application.Abstractions.Authentication;
using Core.Domain.Providers;
using Core.Infrastructure.Authentication;
using Core.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Core.Infrastructure.Extensions;

public static class AuthenticationExtensions
{
	public static string DefaultCookieName => "mtoken";
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddAuthenticationInternal()
		{
			var authenticationOptions = services.BuildServiceProvider().GetRequiredService<IOptions<JwtOptions>>();
			ArgumentNullException.ThrowIfNullOrWhiteSpace(nameof(authenticationOptions.Value.Key));

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.RequireHttpsMetadata = false;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationOptions.Value.Key)),
					ValidIssuer = authenticationOptions.Value.Issuer,
					ValidAudience = authenticationOptions.Value.Audience,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidateIssuer = true,
					ValidateAudience = true,
					ClockSkew = TimeSpan.Zero
				};

				options.Events = new JwtBearerEvents
				{
					OnMessageReceived = context =>
					{
						if (context.Request.Cookies.TryGetValue(DefaultCookieName, out var token))
							context.Token = token;

						return Task.CompletedTask;
					},
					OnAuthenticationFailed = async context =>
					{
						if (context.Exception is SecurityTokenExpiredException && context.Request.Path.StartsWithSegments("/api/v1/authentication/refresh-token"))
						{
							context.NoResult();

							// Manually validate token without lifetime check
							var tokenHandler = new JwtSecurityTokenHandler();
							var validationParams = context.Options.TokenValidationParameters.Clone();
							validationParams.ValidateLifetime = false;

							try
							{
								var token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

								var tokenValidationResult = await tokenHandler.ValidateTokenAsync(token, validationParams);
								if (tokenValidationResult.IsValid)
								{
									context.Principal = new System.Security.Claims.ClaimsPrincipal(tokenValidationResult.ClaimsIdentity);
									context.Success();
								}
								else
									context.Fail("Invalid token");
							}
							catch
							{
								context.Fail("Invalid token");
								// Token invalid for other reasons
							}
						}

						await Task.CompletedTask;
					}
				};
				//options.Events = new JwtBearerEvents
				//{
				//	OnAuthenticationFailed = context =>
				//	{
				//		return Task.CompletedTask;
				//	},

				//	OnTokenValidated = context =>
				//	{
				//		return Task.CompletedTask;
				//	}
				//};
			});

			services.AddHttpContextAccessor();
			services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
			services.TryAddScoped<IExecutionContextProvider, ExecutionContextProvider>();

			return services;
		}
	}
}
