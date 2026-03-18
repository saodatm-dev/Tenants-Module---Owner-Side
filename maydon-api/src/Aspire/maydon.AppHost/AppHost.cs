using maydon.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

#region Maydon api

// Connection string
var connectionString = builder.Configuration.GetRequired<string>("ConnectionStrings:maydondb", "ConnectionStrings__maydondb");
var mongoConnectionString = builder.Configuration.GetOrDefault<string>("ConnectionStrings:MongoDB", "ConnectionStrings__MongoDB", "mongodb://admin:password@localhost:27017/maydon_versions?authSource=admin");

// MinioSecret
var coreOptionsMinIOSecret = builder.Configuration.GetOrDefault<string>("CoreOptions:MinIOSecret", "CoreOptions__MinIOSecret", "m@yd0nP@yd0n");

// JWT
var jwtOptionsKey = builder.Configuration.GetRequired<string>("JwtOptions:Key", "JwtOptions__Key");
var jwtOptionsIssuer = builder.Configuration.GetRequired<string>("JwtOptions:Issuer", "JwtOptions__Issuer");
var jwtOptionsAudience = builder.Configuration.GetRequired<string>("JwtOptions:Audience", "JwtOptions__Audience");
var jwtOptionsExpirationInMinutes = builder.Configuration.GetRequired<string>("JwtOptions:ExpirationInMinutes", "JwtOptions__ExpirationInMinutes");

// Application options
var applicationOptionsAccountKey = builder.Configuration.GetRequired<string>("ApplicationOptions:AccountKey", "ApplicationOptions__AccountKey");
var applicationOptionsInvitationKey = builder.Configuration.GetRequired<string>("ApplicationOptions:InvitationKey", "ApplicationOptions__InvitationKey");
var applicationOptionsUserStateKey = builder.Configuration.GetRequired<string>("ApplicationOptions:UserStateKey", "ApplicationOptions__UserStateKey");
var applicationOptionsUserStateExpiredTimeInMinutes = builder.Configuration.GetRequired<string>("ApplicationOptions:UserStateExpiredTimeInMinutes", "ApplicationOptions__UserStateExpiredTimeInMinutes");
var applicationOptionsRefreshTokenExpiredTimeInDays = builder.Configuration.GetRequired<string>("ApplicationOptions:RefreshTokenExpiredTimeInDays", "ApplicationOptions__RefreshTokenExpiredTimeInDays");
var applicationOptionsExpirationInMinutes = builder.Configuration.GetRequired<string>("ApplicationOptions:CookieName", "ApplicationOptions__CookieName");
var applicationOptionsCookieExpirationInMinutes = builder.Configuration.GetRequired<string>("ApplicationOptions:CookieExpirationInMinutes", "ApplicationOptions__CookieExpirationInMinutes");

// Eimzo options
var eImzoOptionsHost = builder.Configuration.GetRequired<string>("EImzoOptions:Host", "EImzoOptions__Host");

// OneId options
var oneIdOptionsUri = builder.Configuration.GetRequired<string>("OneIdOptions:Uri", "OneIdOptions__Uri");
var oneIdOptionsClientId = builder.Configuration.GetRequired<string>("OneIdOptions:ClientId", "OneIdOptions__ClientId");
var oneIdOptionsClientSecret = builder.Configuration.GetRequired<string>("OneIdOptions:ClientSecret", "OneIdOptions__ClientSecret");

// otp options
var otpOptionsUri = builder.Configuration.GetRequired<string>("OtpOptions:Uri", "OtpOptions__Uri");
var otpOptionsClientId = builder.Configuration.GetRequired<string>("OtpOptions:ClientId", "OtpOptions__ClientId");
var otpOptionsSecret = builder.Configuration.GetRequired<string>("OtpOptions:Secret", "OtpOptions__Secret");


var maydonApi = builder.AddProject<Projects.Maydon_Host>("maydonapi")
	.WithExternalHttpEndpoints()
	.WithEnvironment("ConnectionStrings__maydondb", connectionString)
	.WithEnvironment("ConnectionStrings__MongoDB", mongoConnectionString)
	.WithEnvironment("CoreOptions__MinIOSecret", coreOptionsMinIOSecret)
	// jwt
	.WithEnvironment("JwtOptions__Key", jwtOptionsKey)
	.WithEnvironment("JwtOptions__Issuer", jwtOptionsIssuer)
	.WithEnvironment("JwtOptions__Audience", jwtOptionsAudience)
	.WithEnvironment("JwtOptions__ExpirationInMinutes", jwtOptionsExpirationInMinutes)
	// application options
	.WithEnvironment("ApplicationOptions__AccountKey", applicationOptionsAccountKey)
	.WithEnvironment("ApplicationOptions__InvitationKey", applicationOptionsInvitationKey)
	.WithEnvironment("ApplicationOptions__UserStateKey", applicationOptionsUserStateKey)
	.WithEnvironment("ApplicationOptions__UserStateExpiredTimeInMinutes", applicationOptionsUserStateExpiredTimeInMinutes)
	.WithEnvironment("ApplicationOptions__RefreshTokenExpiredTimeInDays", applicationOptionsRefreshTokenExpiredTimeInDays)
	.WithEnvironment("ApplicationOptions__CookieName", applicationOptionsExpirationInMinutes)
	.WithEnvironment("ApplicationOptions__CookieExpirationInMinutes", applicationOptionsCookieExpirationInMinutes)
	// eimzo
	.WithEnvironment("EImzoOptions__Host", eImzoOptionsHost)
	// one id
	.WithEnvironment("OneIdOptions__Uri", oneIdOptionsUri)
	.WithEnvironment("OneIdOptions__ClientId", oneIdOptionsClientId)
	.WithEnvironment("OneIdOptions__ClientSecret", oneIdOptionsClientSecret)
	// otp
	.WithEnvironment("OtpOptions__Uri", otpOptionsUri)
	.WithEnvironment("OtpOptions__ClientId", otpOptionsClientId)
	.WithEnvironment("OtpOptions__Secret", otpOptionsSecret);

#endregion

#region Administration api

var administrationApi = builder.AddProject<Projects.Maydon_Administration_Host>("maydon-administration-host")
	.WithExternalHttpEndpoints()
	.WithEnvironment("ConnectionStrings__maydondb", connectionString)
	.WithEnvironment("ConnectionStrings__MongoDB", mongoConnectionString)
	// jwt
	.WithEnvironment("JwtOptions__Key", jwtOptionsKey)
	.WithEnvironment("JwtOptions__Issuer", jwtOptionsIssuer)
	.WithEnvironment("JwtOptions__Audience", jwtOptionsAudience)
	.WithEnvironment("JwtOptions__ExpirationInMinutes", jwtOptionsExpirationInMinutes)
	// application options
	.WithEnvironment("ApplicationOptions__AccountKey", applicationOptionsAccountKey)
	.WithEnvironment("ApplicationOptions__InvitationKey", applicationOptionsInvitationKey)
	.WithEnvironment("ApplicationOptions__UserStateKey", applicationOptionsUserStateKey)
	.WithEnvironment("ApplicationOptions__UserStateExpiredTimeInMinutes", applicationOptionsUserStateExpiredTimeInMinutes)
	.WithEnvironment("ApplicationOptions__RefreshTokenExpiredTimeInDays", applicationOptionsRefreshTokenExpiredTimeInDays)
	.WithEnvironment("ApplicationOptions__CookieName", applicationOptionsExpirationInMinutes)
	.WithEnvironment("ApplicationOptions__CookieExpirationInMinutes", applicationOptionsCookieExpirationInMinutes);
#endregion

#region Minio
var minioConnectionString = builder.Configuration.GetRequired<string>("ConnectionStrings:minio", "ConnectionStrings__minio");
if (string.IsNullOrWhiteSpace(minioConnectionString))
{
	var username = builder.AddParameter("user", "maydonminioadmin");
	var password = builder.AddParameter("password", "maydonminioadmin", secret: true);

	var minio = builder.AddMinioContainer("minio", username, password, 9000)
		.WithDataVolume("minio-data");

	maydonApi.WithReference(minio)
	.WaitFor(minio);

	administrationApi.WithReference(minio)
	.WaitFor(minio);
}
else
{
	maydonApi.WithEnvironment("ConnectionStrings__minio", minioConnectionString);
	administrationApi.WithEnvironment("ConnectionStrings__minio", minioConnectionString);
}

#endregion
await builder.Build().RunAsync();

