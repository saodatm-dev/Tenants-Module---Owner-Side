using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Data;
using Core.Domain.Entities;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Core.Infrastructure.Database;
using Identity.Application.Core.Abstractions.Data;
using Identity.Application.Users.GetPermissions;
using Identity.Domain.Accounts;
using Identity.Domain.BankProperties;
using Identity.Domain.Companies;
using Identity.Domain.CompanyUsers;
using Identity.Domain.IntegrationService;
using Identity.Domain.Invitations;
using Identity.Domain.OtpContents;
using Identity.Domain.Otps;
using Identity.Domain.Roles;
using Identity.Domain.Sessions;
using Identity.Domain.Users;
using Identity.Domain.UserStates;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Database;

internal sealed class IdentityDbContext(
	DbContextOptions<IdentityDbContext> options,
	IExecutionContextProvider executionContextProvider) : DbContext(options), IIdentityDbContext
{
	public DbSet<Account> Accounts { get; private set; }
	public DbSet<BankProperty> BankProperties { get; private set; }
	public DbSet<Company> Companies { get; private set; }
	public DbSet<CompanyUser> CompanyUsers { get; private set; }
	public DbSet<IntegrationService> IntegrationServices { get; private set; }
	public DbSet<Invitation> Invitations { get; private set; }
	public DbSet<Otp> Otps { get; private set; }
	public DbSet<OtpContent> OtpContents { get; private set; }
	public DbSet<Permission> Permissions { get; private set; }
	public DbSet<PermissionTranslate> PermissionTranslates { get; private set; }
	public DbSet<Role> Roles { get; private set; }
	public DbSet<RolePermission> RolePermissions { get; private set; }
	public DbSet<Session> Sessions { get; private set; }
	public DbSet<User> Users { get; private set; }
	public DbSet<UserState> UserStates { get; private set; }

	#region Compiled queries
	public IEnumerable<string> GetPermissionNamesByRoleId(Guid roleId) => RolePermissionNames(this, roleId);
	public IAsyncEnumerable<string> GetPermissionNamesByRoleIdAsync(Guid roleId) => RolePermissionNamesAsync(this, roleId);
	public Task<List<GetPermissionsResponse>> GetPermissionsByRoleIdAsync(Guid roleId) => RolePermissionsAsync(this, roleId);

	private static readonly Func<IdentityDbContext, Guid, IEnumerable<string>> RolePermissionNames =
		EF.CompileQuery((IdentityDbContext context, Guid roleId) =>
			(from rolePermission in context.RolePermissions.Where(item => item.RoleId == roleId)
			 join permission in context.Permissions
			 on rolePermission.PermissionId equals permission.Id

			 select permission.Name));

	private static readonly Func<IdentityDbContext, Guid, IAsyncEnumerable<string>> RolePermissionNamesAsync =
		EF.CompileAsyncQuery((IdentityDbContext context, Guid roleId) =>
			context.RolePermissions
				.AsNoTracking()
				.Where(item => item.RoleId == roleId)
				.Include(item => item.Permission)
				.Select(item => item.Permission.Name));

	private static readonly Func<IdentityDbContext, Guid, Task<List<GetPermissionsResponse>>> RolePermissionsAsync =
		EF.CompileAsyncQuery((IdentityDbContext context, Guid roleId) =>
			context.Set<Permission>()
				.AsNoTracking()
				.Include(item => item.Translates)
				.LeftJoin(context.Set<RolePermission>().Where(item => item.RoleId == roleId),
				p => p.Id,
				r => r.PermissionId,
				(p, rp) => new GetPermissionsResponse(p.Id, p.Instance, p.Group, p.Translates.First().Value, rp != null)).ToList());

	#endregion
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.HasDefaultSchema(Domain.AssemblyReference.Instance);

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);

		foreach (var type in modelBuilder.GetEntityTypes(Identity.Domain.AssemblyReference.Assembly))
		{
			var method = ModelBuilderExtensions.SetGlobalQueryMethod<IdentityDbContext>().MakeGenericMethod(type);
			method.Invoke(this, new object[] { modelBuilder });
		}
	}

	public void SetGlobalQuery<T>(ModelBuilder builder)
		where T : Entity
	{
		// is deleted filter
		var entityTypeBuilder = builder.Entity<T>()
					.HasQueryFilter(IApplicationDbContext.IsDeletedFilter, item => !item.IsDeleted);

		// is instance query filter 
		switch (typeof(T).Name)
		{
			case nameof(Permission):
				entityTypeBuilder
					.HasQueryFilter(IApplicationDbContext.IsInstanceFilter, item => EF.Property<string>(item, nameof(Permission.Instance)) == Domain.AssemblyReference.Instance);
				break;
		}
		// is active query filter 
		switch (typeof(T).Name)
		{
			case nameof(Account):
			case nameof(Company):
			case nameof(CompanyUser):
			case nameof(Permission):
			case nameof(Role):
			case nameof(User):
			case nameof(UserState):
				entityTypeBuilder
					.HasQueryFilter(IApplicationDbContext.IsActiveFilter, item => EF.Property<bool>(item, nameof(Role.IsActive)));
				break;
		}

		// Tenant filter
		switch (typeof(T).Name)
		{
			case nameof(Account):
				entityTypeBuilder
					.HasQueryFilter(IApplicationDbContext.TenantIdFilter, item => executionContextProvider.UserId != Guid.Empty ? EF.Property<Guid>(item, nameof(Account.UserId)) == executionContextProvider.UserId : false);
				break;

			case nameof(Invitation):
				entityTypeBuilder
					.HasQueryFilter(IApplicationDbContext.TenantIdFilter, item =>
						(executionContextProvider.TenantId != Guid.Empty ? EF.Property<Guid>(item, nameof(Invitation.SenderId)) == executionContextProvider.TenantId : false) ||
						(executionContextProvider.UserId != Guid.Empty ? EF.Property<Guid?>(item, nameof(Invitation.RecipientId)) == executionContextProvider.UserId : false));
				break;

			case nameof(CompanyUser):
				entityTypeBuilder
					.HasQueryFilter(IApplicationDbContext.TenantIdFilter, item => EF.Property<Guid>(item, nameof(CompanyUser.CompanyId)) == executionContextProvider.TenantId);
				break;
			case nameof(Role):
				entityTypeBuilder
					.HasQueryFilter(IApplicationDbContext.TenantIdFilter, item =>
					EF.Property<Guid?>(item, nameof(Role.TenantId)) == executionContextProvider.TenantId ||
					EF.Property<Guid?>(item, nameof(Role.TenantId)) == null);
				break;
		}

		// translate query filter 
		switch (typeof(T).Name)
		{
			case nameof(PermissionTranslate):
				entityTypeBuilder
					.HasQueryFilter(IApplicationDbContext.TranslateFilter, item => EF.Property<string>(item, nameof(PermissionTranslate.LanguageShortCode)) == executionContextProvider.LanguageShortCode);
				break;
		}
	}
}
