using Building.Application;
using Building.Infrastructure;
using Common.Application;
using Common.Infrastructure;
using Core.Application;
using Core.Infrastructure;
using Core.Infrastructure.Extensions;
using Core.Infrastructure.Web;
using Didox.Application;
using Didox.Infrastructure;
using Didox.Infrastructure.Client;
using Document.Application;
using Document.Infrastructure;
using Identity.Application;
using Identity.Infrastructure;
using Maydon.Administration.Host;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddMinioClient("minio");

builder.Host.UseDefaultServiceProvider(options =>
{
	options.ValidateOnBuild = builder.Environment.IsDevelopment();
	options.ValidateScopes = builder.Environment.IsDevelopment();
});

// core
builder.Services
	.AddCoreApplication([
		typeof(Identity.Application.DependencyInjection),
		typeof(Common.Application.DependencyInjection),
		typeof(Building.Application.DependencyInjection),
		typeof(Document.Application.DependencyInjection),
		typeof(Didox.Application.DependencyInjection)])
	.AddCoreInfrastructure(builder.Configuration);

builder.Services
	.AddIdentityApplication()
	.AddIdentityInfrastructure(builder.Configuration)
	.AddCommonApplication()
	.AddCommonInfrastructure(builder.Configuration)
	.AddBuildingApplication()
	.AddBuildingInfrastructure(builder.Configuration)
	.AddDocumentApplication()
	.AddDocumentInfrastructure(builder.Configuration)
	.AddDidoxApplication()
	.AddDidoxInfrastructure(builder.Configuration)
	.AddDidoxClient(builder.Configuration)
	.AddPdfRendering(builder.Configuration.GetValue<string>("Gotenberg:BaseUrl") ?? "http://localhost:3000");


builder.Services.AddPresentation(builder.Configuration);

var app = builder.Build();

app.MapDefaultEndpoints();

app.UsePresentation();

// SignalR hub mappings
app.MapHub<VersioningHub>("/hubs/versioning");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

using var scope = app.Services.CreateScope();
await scope.MigrateDatabasesAsync();

app.UseHttpsRedirection();

await app.RunAsync();

