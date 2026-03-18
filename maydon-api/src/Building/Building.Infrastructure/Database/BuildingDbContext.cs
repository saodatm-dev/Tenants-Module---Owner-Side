using Building.Application.Amenities.GetByRealEstateId;
using Building.Application.Core.Abstractions.Data;
using Building.Application.Listings.GetById;
using Building.Domain.Amenities;
using Building.Domain.AmenityCategories;
using Building.Domain.BuildingImages;
using Building.Domain.Buildings;
using Building.Domain.Categories;
using Building.Domain.CommunalBills;
using Building.Domain.Complexes;
using Building.Domain.ComplexImages;
using Building.Domain.Floors;
using Building.Domain.LandCategories;
using Building.Domain.ProductionTypes;
using Building.Domain.Leases;
using Building.Domain.ListingAmenities;
using Building.Domain.ListingCategories;
using Building.Domain.ListingRequests;
using Building.Domain.Listings;
using Building.Domain.MeterReadings;
using Building.Domain.Meters;
using Building.Domain.MeterTariffs;
using Building.Domain.MeterTypes;
using Building.Domain.RealEstateAmenities;
using Building.Domain.RealEstateDelegations;
using Building.Domain.RealEstateImages;
using Building.Domain.RealEstates;
using Building.Domain.RealEstateTypes;
using Building.Domain.RentalPurposes;
using Building.Domain.Renovations;
using Building.Domain.Rooms;
using Building.Domain.RoomTypes;
using Building.Domain.Units;
using Building.Domain.Wishlists;
using Common.Domain.Districts;
using Common.Domain.Languages;
using Common.Domain.Regions;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Data;
using Core.Domain.Entities;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Core.Domain.ValueObjects;
using Core.Infrastructure.Converters;
using Core.Infrastructure.Database;
using Identity.Domain.Companies;
using Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Building.Infrastructure.Database;

internal sealed class BuildingDbContext : DbContext, IBuildingDbContext
{
	private readonly IExecutionContextProvider? _executionContextProvider;

	public BuildingDbContext(
		DbContextOptions<BuildingDbContext> options,
		IExecutionContextProvider executionContextProvider) : base(options)
	{
		_executionContextProvider = executionContextProvider;
	}

	/// <summary>Design-time constructor for EF migrations</summary>
	internal BuildingDbContext(DbContextOptions<BuildingDbContext> options) : base(options) { }
	public DbSet<Amenity> Amenities { get; private set; }
	public DbSet<AmenityTranslate> AmenityTranslates { get; private set; }
	public DbSet<AmenityCategory> AmenityCategories { get; private set; }
	public DbSet<AmenityCategoryTranslate> AmenityCategoryTranslates { get; private set; }
	public DbSet<Domain.Buildings.Building> Buildings { get; private set; }
	public DbSet<BuildingImage> BuildingImages { get; private set; }
	public DbSet<BuildingTranslate> BuildingTranslates { get; private set; }
	public DbSet<Category> Categories { get; private set; }
	public DbSet<CategoryTranslate> CategoryTranslates { get; private set; }
	public DbSet<Company> Companies { get; private set; }
	public DbSet<Complex> Complexes { get; private set; }
	public DbSet<ComplexImage> ComplexImages { get; private set; }
	public DbSet<ComplexTranslate> ComplexTranslates { get; private set; }
	public DbSet<DistrictTranslate> DistrictTranslates { get; private set; }
	public DbSet<Floor> Floors { get; private set; }
	public DbSet<LandCategory> LandCategories { get; private set; }
	public DbSet<LandCategoryTranslate> LandCategoryTranslates { get; private set; }
	public DbSet<ProductionType> ProductionTypes { get; private set; }
	public DbSet<ProductionTypeTranslate> ProductionTypeTranslates { get; private set; }
	public DbSet<Lease> Leases { get; private set; }
	public DbSet<LeaseItem> LeaseItems { get; private set; }
	public DbSet<Listing> Listings { get; private set; }
	public DbSet<ListingAmenity> ListingAmenities { get; private set; }
	public DbSet<ListingCategory> ListingCategories { get; private set; }
	public DbSet<ListingCategoryTranslate> ListingCategoryTranslates { get; private set; }
	public DbSet<ListingTranslate> ListingTranslates { get; private set; }
	public DbSet<ListingRequest> ListingRequests { get; private set; }
	public DbSet<CommunalBill> CommunalBills { get; private set; }
	public DbSet<CommunalPayment> CommunalPayments { get; private set; }
	public DbSet<Meter> Meters { get; private set; }
	public DbSet<MeterReading> MeterReadings { get; private set; }
	public DbSet<MeterTariff> MeterTariffs { get; private set; }
	public DbSet<MeterType> MeterTypes { get; private set; }
	public DbSet<MeterTypeTranslate> MeterTypeTranslates { get; private set; }
	public DbSet<RealEstateAmenity> RealEstateAmenities { get; private set; }
	public DbSet<RealEstateImage> RealEstateImages { get; private set; }
	public DbSet<RealEstate> RealEstates { get; private set; }
	public DbSet<RealEstateDelegation> RealEstateDelegations { get; private set; }
	public DbSet<RealEstateTranslate> RealEstateTranslates { get; private set; }
	public DbSet<RealEstateType> RealEstateTypes { get; private set; }
	public DbSet<RealEstateTypeTranslate> RealEstateTypeTranslates { get; private set; }
	public DbSet<RegionTranslate> RegionTranslates { get; private set; }
	public DbSet<RentalPurpose> RentalPurposes { get; private set; }
	public DbSet<RentalPurposeTranslate> RentalPurposeTranslates { get; private set; }
	public DbSet<Renovation> Renovations { get; private set; }
	public DbSet<RenovationTranslate> RenovationTranslates { get; private set; }
	public DbSet<RolePermission> RolePermissions { get; private set; }
	public DbSet<Room> Rooms { get; private set; }
	public DbSet<RoomType> RoomTypes { get; private set; }
	public DbSet<RoomTypeTranslate> RoomTypeTranslates { get; private set; }
	public DbSet<Wishlist> Wishlists { get; private set; }
	//public DbSet<WishlistItem> WishlistItems { get; set; }
	public DbSet<Language> Languages { get; private set; }
	public DbSet<Permission> Permissions { get; private set; }
	public DbSet<PermissionTranslate> PermissionTranslates { get; private set; }
	public DbSet<Unit> Units { get; private set; }
	public DbSet<User> Users { get; private set; }

	protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
	{
		configurationBuilder.Properties<Money>().HaveConversion<MoneyToDecimalConverter>().HaveColumnType("numeric(18,2)");
	}

	public IAsyncEnumerable<string> GetPermissionNamesByRoleIdAsync(Guid roleId) => RolePermissionNamesAsync(this, roleId);
	public string? GetRegionName(Guid regionId) => RegionName(this, regionId, _executionContextProvider!.LanguageShortCode);
	public string? GetDistrictName(Guid districtId) => DistrictName(this, districtId, _executionContextProvider!.LanguageShortCode);
	public string GetCategoryNameById(Guid categoryId) => GetCategoryName(this, categoryId);
	public IEnumerable<GetAmenityResponse>? GetAmenityByCategoryId(Guid categoryId) => GetAmenities(this, categoryId);
	public IEnumerable<Category> GetBulkCategories(IEnumerable<Guid> categoryIds)
	{
		if (!categoryIds.Any())
			return Enumerable.Empty<Category>();
		return GetCategories(this, categoryIds);
	}
	public IEnumerable<BuildingType> GetBulkCategoryBuildingTypes(IEnumerable<Guid> categoryIds)
	{
		if (!categoryIds.Any())
			return Enumerable.Empty<BuildingType>();

		return GetCategoryBuildingTypes(this, categoryIds);
	}
	public IEnumerable<string> GetCategoryNamesByIds(List<Guid> categoryIds) => GetCategoryNames(this, categoryIds);

	public string? GetRoomTypeTranslate(Guid roomTypeId) => GetRoomTypeTranslates(this, roomTypeId);
	public string GetTenantNameById(Guid id) => GetTenant(this, id);
	public string GetMeterTypeNameById(Guid id) => GetMeterTypeName(this, id);
	#region Listings
	public IEnumerable<string> GetListingCategoryNamesByIds(IEnumerable<Guid> listingCategoryIds) => GetListingCategoryNames(this, listingCategoryIds);
	public IEnumerable<GetListingByIdAmenityResponse> GetListingAmenitiesByIds(IEnumerable<Guid> amenityIds) => GetListingAmenityNames(this, amenityIds);
	#endregion

	#region User Info Helpers
	public string? GetUserPhone(Guid userId) => UserPhone(this, userId);
	public string? GetUserCompanyName(Guid tenantId) => CompanyName(this, tenantId);
	public bool GetUserIsVerified(Guid userId) => UserIsVerified(this, userId);
	public string? GetUserPhoto(Guid userId) => UserPhoto(this, userId);
	#endregion

	private static readonly Func<BuildingDbContext, Guid, string, string?> RegionName =
		EF.CompileQuery((BuildingDbContext context, Guid regionId, string languageShortCode) =>
			context.RegionTranslates
				.AsNoTracking()
				.Where(item => item.RegionId == regionId && item.LanguageShortCode == languageShortCode)
				.Select(item => item.Value)
				.FirstOrDefault());

	private static readonly Func<BuildingDbContext, Guid, string, string?> DistrictName =
		EF.CompileQuery((BuildingDbContext context, Guid districtId, string languageShortCode) =>
			context.DistrictTranslates
				.AsNoTracking()
				.Where(item => item.DistrictId == districtId && item.LanguageShortCode == languageShortCode)
				.Select(item => item.Value)
				.FirstOrDefault());

	private static readonly Func<BuildingDbContext, Guid, IEnumerable<GetAmenityResponse>?> GetAmenities =
		EF.CompileQuery((BuildingDbContext context, Guid amenityCategoryId) =>
			context.AmenityTranslates
				.AsNoTracking()
				.Include(item => item.Amenity)
				.Where(item => item.Amenity.AmenityCategoryId == amenityCategoryId)
				.Select(item => new GetAmenityResponse(item.AmenityId, item.Value, item.Amenity.IconUrl)));

	private static readonly Func<BuildingDbContext, IEnumerable<Guid>, IEnumerable<Category>> GetCategories =
		EF.CompileQuery((BuildingDbContext context, IEnumerable<Guid> categoryIds) =>
			context.Categories
				.AsNoTracking()
				.Where(item => categoryIds.Contains(item.Id)));

	private static readonly Func<BuildingDbContext, IEnumerable<Guid>, IEnumerable<BuildingType>> GetCategoryBuildingTypes =
		EF.CompileQuery((BuildingDbContext context, IEnumerable<Guid> categoryIds) =>
			context.Categories
				.AsNoTracking()
				.Where(item => categoryIds.Contains(item.Id))
				.Select(item => item.BuildingType));

	private static readonly Func<BuildingDbContext, Guid, string?> GetCategoryName =
		EF.CompileQuery((BuildingDbContext context, Guid categoryId) =>
			context.Categories
				.AsNoTracking()
				.Where(item => item.Id == categoryId)
				.Include(item => item.Translates)
				.Select(item => item.Translates.First().Value)
				.FirstOrDefault());

	private static readonly Func<BuildingDbContext, List<Guid>, IEnumerable<string>> GetCategoryNames =
		EF.CompileQuery((BuildingDbContext context, List<Guid> categoryIds) =>
			context.Categories
				.AsNoTracking()
				.Include(item => item.Translates)
				.AsSplitQuery()
				.Where(item => categoryIds.Contains(item.Id))
				.Select(item => item.Translates.First().Value));

	private static readonly Func<BuildingDbContext, IEnumerable<Guid>, IEnumerable<string>> GetListingCategoryNames =
		EF.CompileQuery((BuildingDbContext context, IEnumerable<Guid> listingCategoryIds) =>
		context.ListingCategoryTranslates
			.AsNoTracking()
			.Where(item => listingCategoryIds.Contains(item.ListingCategoryId))
			.Select(item => item.Value));

	private static readonly Func<BuildingDbContext, IEnumerable<Guid>, IEnumerable<GetListingByIdAmenityResponse>> GetListingAmenityNames =
		EF.CompileQuery((BuildingDbContext context, IEnumerable<Guid> amenityIds) =>
		context.AmenityTranslates
			.AsNoTracking()
			.Include(item => item.Amenity)
			.Where(item => amenityIds.Contains(item.AmenityId))
			.Select(item => new GetListingByIdAmenityResponse(item.Amenity.IconUrl, item.Value)));

	private static readonly Func<BuildingDbContext, Guid, string?> GetRoomTypeTranslates =
		EF.CompileQuery((BuildingDbContext context, Guid id) =>
			context.RoomTypeTranslates
				.AsNoTracking()
				.Where(item => item.Id == id)
				.Select(item => item.Value)
				.FirstOrDefault());

	private static readonly Func<BuildingDbContext, Guid, IAsyncEnumerable<string>> RolePermissionNamesAsync =
		EF.CompileAsyncQuery((BuildingDbContext context, Guid roleId) =>
			context.RolePermissions
				.AsNoTracking()
				.Where(item => item.RoleId == roleId)
				.Include(item => item.Permission)
				.Select(item => item.Permission.Name));

	private static readonly Func<BuildingDbContext, Guid, string?> GetTenant =
		EF.CompileQuery((BuildingDbContext context, Guid id) =>
			context.Companies.Any(item => item.Id == id)
				? context.Companies.First(item => item.Id == id).Name
				: context.Users.Any(item => item.Id == id)
					? context.Users.First(item => item.Id == id).FullName
					: null);

	private static readonly Func<BuildingDbContext, Guid, string?> GetMeterTypeName =
		EF.CompileQuery((BuildingDbContext context, Guid id) =>
			context.MeterTypeTranslates
				.AsNoTracking()
				.Where(item => item.MeterTypeId == id)
				.Select(item => item.Value)
				.FirstOrDefault());

	// User info compiled queries
	private static readonly Func<BuildingDbContext, Guid, string?> UserPhone =
		EF.CompileQuery((BuildingDbContext context, Guid userId) =>
			context.Users
				.AsNoTracking()
				.Where(u => u.Id == userId)
				.Select(u => u.PhoneNumber)
				.FirstOrDefault());

	private static readonly Func<BuildingDbContext, Guid, string?> CompanyName =
		EF.CompileQuery((BuildingDbContext context, Guid tenantId) =>
			context.Companies
				.AsNoTracking()
				.Where(c => c.Id == tenantId)
				.Select(c => c.Name)
				.FirstOrDefault());

	private static readonly Func<BuildingDbContext, Guid, bool> UserIsVerified =
		EF.CompileQuery((BuildingDbContext context, Guid userId) =>
			context.Users
				.AsNoTracking()
				.Where(u => u.Id == userId)
				.Select(u => u.IsVerified)
				.FirstOrDefault());

	private static readonly Func<BuildingDbContext, Guid, string?> UserPhoto =
		EF.CompileQuery((BuildingDbContext context, Guid userId) =>
			context.Users
				.AsNoTracking()
				.Where(u => u.Id == userId)
				.Select(u => u.ObjectName)
				.FirstOrDefault());

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(BuildingDbContext).Assembly);

		foreach (var type in modelBuilder.GetEntityTypes(Domain.AssemblyReference.Assembly))
		{
			var method = ModelBuilderExtensions.SetGlobalQueryMethod<BuildingDbContext>().MakeGenericMethod(type);
			method.Invoke(this, [modelBuilder]);
		}

		base.OnModelCreating(modelBuilder);

		modelBuilder.HasPostgresExtension("postgis");
		//modelBuilder.HasPostgresExtension("uuid-ossp");
		//modelBuilder.HasPostgresExtension("pg_trgm");  // For text search
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
			case nameof(Permission):
				entityTypeBuilder
					.HasQueryFilter(IApplicationDbContext.IsActiveFilter, item => EF.Property<bool>(item, nameof(Language.IsActive)));
				break;
		}

		// is tenant query filter 
		switch (typeof(T).Name)
		{
			case nameof(Lease):
				entityTypeBuilder
					.HasQueryFilter(IApplicationDbContext.TenantIdFilter, item =>
						_executionContextProvider!.TenantId != Guid.Empty
							? EF.Property<Guid>(item, nameof(Lease.OwnerId)) == _executionContextProvider!.TenantId ||
							  EF.Property<Guid>(item, nameof(Lease.ClientId)) == _executionContextProvider!.TenantId ||
							  (EF.Property<Guid?>(item, nameof(Lease.AgentId)) != null ? EF.Property<Guid?>(item, nameof(Lease.AgentId)) == _executionContextProvider!.TenantId : false)
							: true);
				break;
			case nameof(Listing):
				entityTypeBuilder
					.HasQueryFilter(IApplicationDbContext.TenantIdFilter, item => _executionContextProvider!.TenantId != Guid.Empty ? EF.Property<Guid>(item, nameof(Listing.OwnerId)) == _executionContextProvider!.TenantId : true);
				break;
			case nameof(RealEstate):
				entityTypeBuilder
					.HasQueryFilter(IApplicationDbContext.TenantIdFilter, item => _executionContextProvider!.TenantId != Guid.Empty ? EF.Property<Guid>(item, nameof(RealEstate.OwnerId)) == _executionContextProvider!.TenantId : true);
				break;
			case nameof(Wishlist):
				entityTypeBuilder
					.HasQueryFilter(IApplicationDbContext.TenantIdFilter, item =>
						_executionContextProvider!.TenantId != Guid.Empty ? EF.Property<Guid>(item, nameof(Wishlist.TenantId)) == _executionContextProvider!.TenantId : false &&
							_executionContextProvider!.UserId != Guid.Empty && EF.Property<Guid>(item, nameof(Wishlist.UserId)) == _executionContextProvider!.UserId);
				break;
		}

		// translate query filter 
		switch (typeof(T).Name)
		{
			case nameof(AmenityCategoryTranslate):
			case nameof(AmenityTranslate):
			case nameof(BuildingTranslate):
			case nameof(CategoryTranslate):
			case nameof(ComplexTranslate):
			case nameof(DistrictTranslate):
			case nameof(LandCategoryTranslate):
			case nameof(ProductionTypeTranslate):
			case nameof(ListingCategoryTranslate):
			case nameof(ListingTranslate):
			case nameof(MeterTypeTranslate):
			case nameof(RentalPurposeTranslate):
			case nameof(RenovationTranslate):
			case nameof(RoomTypeTranslate):
			case nameof(RealEstateTypeTranslate):
			case nameof(RegionTranslate):
			case nameof(PermissionTranslate):
				entityTypeBuilder
					.HasQueryFilter(IApplicationDbContext.TranslateFilter, item => EF.Property<string>(item, nameof(PermissionTranslate.LanguageShortCode)) == _executionContextProvider!.LanguageShortCode);
				break;
		}
	}

}
