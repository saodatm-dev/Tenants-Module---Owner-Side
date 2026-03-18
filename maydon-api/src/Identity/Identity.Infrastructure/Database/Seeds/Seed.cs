using System.Reflection;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Core.Domain.Roles;
using Identity.Application.Core.Abstractions.Authentication;
using Identity.Application.Core.Abstractions.Data;
using Identity.Domain.Accounts;
using Identity.Domain.Roles;
using Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Identity.Infrastructure.Database.Seeds;

internal static class Seed
{
	private const string AdminName = "Administrator";
	private const string AdminPhoneNumber = "998901890625";
	private const string WebHostName = "Maydon.Host";
	private const string PermissionKey = "Permission";
	private const string DefaultRolePermissionsKey = "DefaultRolePermissions";
	//private const string GroupNameKey = "Maydon.Owner.Abstractions.IEndpoint.GroupName";
	private const string PermissionGroupNameKey = "GroupName";
	//private const string IEndpointFullName = $"{WebHostName}.Abstractions.IEndpoint";
	private const string IPermissionFullName = $"{WebHostName}.Abstractions.IPermission";

	private static (List<Permission> Permissions, List<RolePermissionValue> RolePermissions) Permissions()
	{
		var maydonHostRuntimeLibrary = DependencyContext.Default?.RuntimeLibraries.FirstOrDefault(item => item.Name == WebHostName);
		if (maydonHostRuntimeLibrary is null)
			return (new List<Permission>(), new List<RolePermissionValue>());

		var assembly = Assembly.Load(new AssemblyName(maydonHostRuntimeLibrary.Name));
		if (assembly is null)
			return (new List<Permission>(), new List<RolePermissionValue>());

		var assemblyPermissions = assembly
			.DefinedTypes
			.Where(type => type is { IsAbstract: false, IsInterface: false } &&
				type.ImplementedInterfaces.Any(item => item.FullName == IPermissionFullName))
			.ToArray();

		var permissions = new List<Permission>();
		var rolePermissions = new List<RolePermissionValue>();
		foreach (var endpoint in assemblyPermissions)
		{
			var instance = Activator.CreateInstance(endpoint);
			string groupName = string.Empty;
			var groupNameProperty = endpoint.DeclaredProperties.FirstOrDefault(item => item.Name == PermissionGroupNameKey);
			if (groupNameProperty is not null)
				groupName = groupNameProperty.GetValue(instance)?.ToString();

			var permissionFields = endpoint.DeclaredFields;
			foreach (var permissionFieldInfo in permissionFields)
			{
				if (permissionFieldInfo.Name.StartsWith(PermissionKey, StringComparison.OrdinalIgnoreCase))
				{
					var permissionValue = permissionFieldInfo.GetValue(null) as PermissionType;
					if (permissionValue is not null)
						permissions.Add(new Permission(groupName, endpoint.Name, permissionValue.PermissionName, permissionValue.IsSystem));
				}
				if (permissionFieldInfo.Name.StartsWith(DefaultRolePermissionsKey, StringComparison.OrdinalIgnoreCase))
				{
					var rolePermissionValues = permissionFieldInfo.GetValue(null) as IEnumerable<RolePermissionValue>;
					if (rolePermissionValues is not null && rolePermissionValues.Any())
						rolePermissions.AddRange(rolePermissionValues);
				}
			}
		}

		return (permissions, rolePermissions);
	}
	private static List<Role> Roles() => [
		new Role($"{RoleType.System}",type: RoleType.System),
		new Role($"{RoleType.Owner}",type:RoleType.Owner),
		new Role($"{RoleType.Client}",type:RoleType.Client)];
	private static User Administrator(Guid roleId) =>
		new User(
			RegisterType.PhoneNumber,
			AdminName,
			AdminName,
			phoneNumber: AdminPhoneNumber);
	public static async Task SeedingAsync(IServiceProvider serviceProvider, IIdentityDbContext dbContext, CancellationToken cancellationToken)
	{
		var hasNewData = false;

		#region Permissions

		var modulePermissions = Seed.Permissions();
		var permissions = modulePermissions.Permissions;
		if (!await dbContext.Permissions.IgnoreQueryFilters().AnyAsync(cancellationToken))
		{
			await dbContext.Permissions.AddRangeAsync(permissions, cancellationToken);
			hasNewData = true;
		}
		else
		{
			permissions = await dbContext.Permissions.AsNoTracking().IgnoreQueryFilters().ToListAsync(cancellationToken);
			var newPermissions = modulePermissions.Permissions.ExceptBy(permissions.Select(item => item.Name), item => item.Name);
			if (newPermissions.Any())
			{
				await dbContext.Permissions.AddRangeAsync(newPermissions, cancellationToken);
				hasNewData = true;
				permissions.AddRange(newPermissions);
			}
		}
		#endregion

		#region Roles

		var roles = Seed.Roles();
		if (!await dbContext.Roles.IgnoreQueryFilters().AnyAsync(cancellationToken))
		{
			await dbContext.Roles.AddRangeAsync(roles, cancellationToken);
			hasNewData = true;
		}
		else
		{
			var dbRoles = await dbContext.Roles.AsNoTracking().IgnoreQueryFilters().ToListAsync(cancellationToken);
			var newRoles = roles.ExceptBy(dbRoles.Select(item => item.Name), item => item.Name);
			if (newRoles.Any())
			{
				await dbContext.Roles.AddRangeAsync(newRoles, cancellationToken);
				dbRoles.AddRange(newRoles);
			}

			dbRoles.AddRange(newRoles);

			roles = dbRoles;
		}

		#endregion

		#region RolePermissions

		if (roles.Any())
		{
			var dbRolePermissions = await dbContext.RolePermissions.AsNoTracking().ToListAsync(cancellationToken);
			var rolePermissions = new List<RolePermission>();
			foreach (var role in roles)
			{
				var moduleRolePermissions = modulePermissions.RolePermissions.Where(item => item.Type == role.Type && item.Value);
				foreach (var moduleRolePermission in moduleRolePermissions)
				{
					var currentPermission = permissions.Find(item => item.Name == moduleRolePermission.PermissionName);
					if (currentPermission is null)
						continue;

					if (dbRolePermissions.Find(item => item.RoleId == role.Id && item.PermissionId == currentPermission.Id) is not null)
						continue;

					rolePermissions.Add(new RolePermission(role.Id, currentPermission.Id));
				}
			}

			if (rolePermissions.Any())
			{
				await dbContext.RolePermissions.AddRangeAsync(rolePermissions, cancellationToken);
				hasNewData = true;
			}
		}
		#endregion

		#region Users

		if (!await dbContext.Users.AnyAsync(item => item.FirstName == Seed.AdminName && item.LastName == Seed.AdminName && item.PhoneNumber == Seed.AdminPhoneNumber, cancellationToken))
		{
			var roleId = roles.First(item => item.Type == RoleType.System)!.Id;
			var admin = Seed.Administrator(roleId);

			var passwordHasher = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IPasswordHasher>();
			passwordHasher.TryHash(AdminPhoneNumber, out var passwordHash, out var salt);

			await dbContext.Users.AddAsync(admin.ChangePassword(passwordHash, salt), cancellationToken);

			var account = new Account(admin.Id, admin.Id).ChangeRole(roleId);

			await dbContext.Accounts.AddAsync(account, cancellationToken);

			hasNewData = true;
		}

		#endregion

		if (hasNewData)
			await dbContext.SaveChangesAsync(cancellationToken);
	}

}

