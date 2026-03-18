namespace Maydon.Administration.Host.Abstractions;

public interface IEndpoint
{
	void MapEndpoint(IEndpointRouteBuilder app);
	string GroupName { get; }
}
