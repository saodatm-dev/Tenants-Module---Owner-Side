using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Core.Infrastructure.Authorization;

internal sealed class PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
{
	public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
	{
		var policy = await base.GetPolicyAsync(policyName);

		if (policy is not null)
			return policy;

		var permissionPolicy = new AuthorizationPolicyBuilder()
			.AddRequirements(new PermissionRequirement(policyName))
			.Build();

		options.Value.AddPolicy(policyName, permissionPolicy);

		return permissionPolicy;
	}
}
