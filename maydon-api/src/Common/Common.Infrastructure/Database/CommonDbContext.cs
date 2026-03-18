using Common.Application.Core.Abstractions.Data;
using Common.Domain.Banks;
using Common.Domain.Currencies;
using Common.Domain.Districts;
using Common.Domain.Languages;
using Common.Domain.Regions;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Data;
using Core.Domain.Entities;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Core.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.Database;

internal sealed class CommonDbContext(
	DbContextOptions<CommonDbContext> options,
	IExecutionContextProvider executionContextProvider) : DbContext(options), ICommonDbContext
{
	public DbSet<Bank> Banks { get; set; }
	public DbSet<BankTranslate> BankTranslates { get; set; }
	public DbSet<Currency> Currencies { get; set; }
	public DbSet<CurrencyTranslate> CurrencyTranslates { get; set; }
	public DbSet<District> Districts { get; set; }
	public DbSet<DistrictTranslate> DistrictTranslates { get; set; }
	public DbSet<Language> Languages { get; set; }
	public DbSet<Permission> Permissions { get; set; }
	public DbSet<PermissionTranslate> PermissionTranslates { get; set; }
	public DbSet<Region> Regions { get; set; }
	public DbSet<RegionTranslate> RegionTranslates { get; set; }
	public DbSet<RolePermission> RolePermissions { get; set; }
	public IAsyncEnumerable<string> GetPermissionNamesByRoleIdAsync(Guid roleId) => RolePermissionNamesAsync(this, roleId);

	private static readonly Func<CommonDbContext, Guid, IAsyncEnumerable<string>> RolePermissionNamesAsync =
		EF.CompileAsyncQuery((CommonDbContext context, Guid roleId) =>
			context.RolePermissions
				.AsNoTracking()
				.Where(item => item.RoleId == roleId)
				.Include(item => item.Permission)
				.Select(item => item.Permission.Name));
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(CommonDbContext).Assembly);

		foreach (var type in modelBuilder.GetEntityTypes(Common.Domain.AssemblyReference.Assembly))
		{
			var method = ModelBuilderExtensions.SetGlobalQueryMethod<CommonDbContext>().MakeGenericMethod(type);
			method.Invoke(this, new object[] { modelBuilder });
		}

		base.OnModelCreating(modelBuilder);
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
			case nameof(Language):
			case nameof(Permission):
				entityTypeBuilder
					.HasQueryFilter(IApplicationDbContext.IsActiveFilter, item => EF.Property<bool>(item, nameof(Language.IsActive)));
				break;
		}

		// translate query filter 
		switch (typeof(T).Name)
		{
			case nameof(BankTranslate):
			case nameof(CurrencyTranslate):
			case nameof(DistrictTranslate):
			case nameof(RegionTranslate):
			case nameof(PermissionTranslate):
				entityTypeBuilder
					.HasQueryFilter(IApplicationDbContext.TranslateFilter, item => EF.Property<string>(item, nameof(BankTranslate.LanguageShortCode)) == executionContextProvider.LanguageShortCode);
				break;
		}
	}
}
