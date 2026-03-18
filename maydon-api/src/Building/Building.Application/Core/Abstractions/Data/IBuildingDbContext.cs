using Building.Application.Amenities.GetByRealEstateId;
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
using Building.Domain.RentalPurposes;
using Building.Domain.RealEstateTypes;
using Building.Domain.Renovations;
using Building.Domain.Rooms;
using Building.Domain.RoomTypes;
using Building.Domain.Units;
using Building.Domain.Wishlists;
using Common.Domain.Districts;
using Common.Domain.Languages;
using Common.Domain.Regions;
using Core.Application.Abstractions.Data;
using Identity.Domain.Companies;
using Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Core.Abstractions.Data;

public interface IBuildingDbContext : IApplicationDbContext
{
	DbSet<Amenity> Amenities { get; }                                   //удобства
	DbSet<AmenityTranslate> AmenityTranslates { get; }
	DbSet<AmenityCategory> AmenityCategories { get; }
	DbSet<AmenityCategoryTranslate> AmenityCategoryTranslates { get; }
	DbSet<Building.Domain.Buildings.Building> Buildings { get; }
	DbSet<BuildingImage> BuildingImages { get; }
	DbSet<BuildingTranslate> BuildingTranslates { get; }
	DbSet<Category> Categories { get; }
	DbSet<CategoryTranslate> CategoryTranslates { get; }
	DbSet<Company> Companies { get; }
	DbSet<Building.Domain.Complexes.Complex> Complexes { get; }
	DbSet<ComplexImage> ComplexImages { get; }
	DbSet<ComplexTranslate> ComplexTranslates { get; }
	DbSet<DistrictTranslate> DistrictTranslates { get; }
	DbSet<Floor> Floors { get; }
	DbSet<LandCategory> LandCategories { get; }
	DbSet<LandCategoryTranslate> LandCategoryTranslates { get; }
	DbSet<ProductionType> ProductionTypes { get; }
	DbSet<ProductionTypeTranslate> ProductionTypeTranslates { get; }
	DbSet<Lease> Leases { get; }
	DbSet<LeaseItem> LeaseItems { get; }
	DbSet<Listing> Listings { get; }
	DbSet<ListingAmenity> ListingAmenities { get; }
	DbSet<ListingCategory> ListingCategories { get; }
	DbSet<ListingCategoryTranslate> ListingCategoryTranslates { get; }
	DbSet<ListingTranslate> ListingTranslates { get; }
	DbSet<ListingRequest> ListingRequests { get; }
	DbSet<CommunalBill> CommunalBills { get; }
	DbSet<CommunalPayment> CommunalPayments { get; }
	DbSet<Meter> Meters { get; }
	DbSet<MeterReading> MeterReadings { get; }
	DbSet<MeterTariff> MeterTariffs { get; }
	DbSet<MeterType> MeterTypes { get; }
	DbSet<MeterTypeTranslate> MeterTypeTranslates { get; }
	DbSet<RealEstateAmenity> RealEstateAmenities { get; }
	DbSet<RealEstateImage> RealEstateImages { get; }
	DbSet<RealEstate> RealEstates { get; }
	DbSet<RealEstateDelegation> RealEstateDelegations { get; }
	DbSet<RealEstateTranslate> RealEstateTranslates { get; }
	DbSet<RealEstateType> RealEstateTypes { get; }
	DbSet<RealEstateTypeTranslate> RealEstateTypeTranslates { get; }

	DbSet<RentalPurpose> RentalPurposes { get; }
	DbSet<RentalPurposeTranslate> RentalPurposeTranslates { get; }
	DbSet<RegionTranslate> RegionTranslates { get; }
	DbSet<Renovation> Renovations { get; }                              //ремонт
	DbSet<RenovationTranslate> RenovationTranslates { get; }
	DbSet<Room> Rooms { get; }
	DbSet<RoomType> RoomTypes { get; }
	DbSet<RoomTypeTranslate> RoomTypeTranslates { get; }
	DbSet<Wishlist> Wishlists { get; }
	DbSet<Unit> Units { get; }
	DbSet<User> Users { get; }

	// common
	DbSet<Language> Languages { get; }
	string? GetRegionName(Guid regionId);
	string? GetDistrictName(Guid districtId);
	IEnumerable<GetAmenityResponse>? GetAmenityByCategoryId(Guid categoryId);
	string GetCategoryNameById(Guid categoryId);
	IEnumerable<string> GetCategoryNamesByIds(List<Guid> categoryIds);
	IEnumerable<Category> GetBulkCategories(IEnumerable<Guid> categoryIds);
	IEnumerable<BuildingType> GetBulkCategoryBuildingTypes(IEnumerable<Guid> categoryIds);
	string? GetRoomTypeTranslate(Guid roomTypeId);
	string GetTenantNameById(Guid id);
	string GetMeterTypeNameById(Guid id);

	#region Listings
	IEnumerable<string> GetListingCategoryNamesByIds(IEnumerable<Guid> listingCategoryIds);
	IEnumerable<GetListingByIdAmenityResponse> GetListingAmenitiesByIds(IEnumerable<Guid> amenityIds);
	#endregion

	#region User Info Helpers
	string? GetUserPhone(Guid userId);
	string? GetUserCompanyName(Guid tenantId);
	bool GetUserIsVerified(Guid userId);
	string? GetUserPhoto(Guid userId);
	#endregion
}
