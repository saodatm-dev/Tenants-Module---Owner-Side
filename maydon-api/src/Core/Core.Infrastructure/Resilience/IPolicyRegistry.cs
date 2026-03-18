using Polly;

namespace Core.Infrastructure.Resilience;

/// <summary>
/// Policy registry for managing named policies across the application
/// </summary>
public interface IPolicyRegistry
{
    IAsyncPolicy GetPolicy(string policyName);
    IAsyncPolicy<T> GetPolicy<T>(string policyName);
    void RegisterPolicy(string policyName, IAsyncPolicy policy);
    void RegisterPolicy<T>(string policyName, IAsyncPolicy<T> policy);
}
