using Polly;

namespace Core.Infrastructure.Resilience;

public sealed class PolicyRegistry : IPolicyRegistry
{
    private readonly Dictionary<string, IAsyncPolicy> _policies = new();
    private readonly Dictionary<string, object> _typedPolicies = new();

    public IAsyncPolicy GetPolicy(string policyName)
    {
        return _policies.TryGetValue(policyName, out var policy) ? policy 
            : throw new InvalidOperationException($"Policy '{policyName}' not found in registry");
    }

    public IAsyncPolicy<T> GetPolicy<T>(string policyName)
    {
        if (_typedPolicies.TryGetValue(policyName, out var policy))
        {
            return (IAsyncPolicy<T>)policy;
        }
        
        throw new InvalidOperationException($"Typed policy '{policyName}' not found in registry");
    }

    public void RegisterPolicy(string policyName, IAsyncPolicy policy)
    {
        _policies[policyName] = policy;
    }

    public void RegisterPolicy<T>(string policyName, IAsyncPolicy<T> policy)
    {
        _typedPolicies[policyName] = policy;
    }
}
