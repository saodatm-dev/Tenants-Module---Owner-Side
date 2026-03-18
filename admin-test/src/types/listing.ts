// Listing Types for the cabinet-front application

// Amenity in listing detail
export interface ListingAmenity {
  iconUrl?: string
  name: string
}

// Listing entity from API
export interface Listing {
  id: string
  ownerId: string
  title?: string
  categories: string[]
  complex?: string
  building?: string
  floorNumbers?: number[]
  roomsCount?: number
  totalArea: number
  livingArea?: number
  ceilingHeight?: number
  region?: string
  district?: string
  latitude?: number
  longitude?: number
  address?: string
  priceForMonth?: number
  pricePerSquareMeter?: number
  description?: string
  // Additional mapped fields for display
  regionName?: string
  districtName?: string
}

// My Listings includes moderation status and images
export interface MyListing extends Listing {
  moderationStatus?: number
  status?: number
  reason?: string
  objectNames?: string[] // URL-encoded image keys from API
}

// Floor in listing detail
export interface ListingFloor {
  id: string
  number: number
  type?: number
  label?: string
  totalArea?: number
  ceilingHeight?: number
  plan?: string
}

// Real Estate in floor
export interface ListingRealEstateDetail {
  id: string
  number?: string
  roomsCount?: number
  totalArea?: number
  livingArea?: number
  ceilingHeight?: number
  plan?: string
}

// Listing detail with floors, amenities, images
export interface ListingDetail extends Listing {
  floors?: ListingFloor[]
  amenities?: ListingAmenity[]
  plan?: string  // Floor plan image URL
  images?: string[]  // Image URLs/keys
  status?: number
}

// Real Estate reference for listing creation
export interface ListingRealEstateInput {
  realEstateId: string
  coordinates?: RealEstatePlanCoordinate[]
}

export interface RealEstatePlanCoordinate {
  x: number
  y: number
}

// Create Listing Request (matches POST /api/v1/listings)
// realEstateId is required, floorIds/roomIds/unitIds are optional scoping
export interface CreateListingRequest {
  realEstateId: string
  listingCategoryIds: string[]
  floorIds?: string[]
  roomIds?: string[]
  unitIds?: string[]
  amenityIds?: string[]
  priceForMonth?: number
  pricePerSquareMeter?: number
  description?: string
  title?: string
  descriptionTranslates?: { languageId: string; languageShortCode: string; value: string }[]
  rentalPurposeId?: string
  minLeaseTerm?: number
  utilityPaymentType?: number
  nextAvailableDate?: string
}

// Update Listing Request
export interface UpdateListingRequest {
  id: string
  realEstateId: string
  listingCategoryIds: string[]
  floorIds?: string[]
  roomIds?: string[]
  unitIds?: string[]
  amenityIds?: string[]
  priceForMonth?: number
  pricePerSquareMeter?: number
  description?: string
  title?: string
  descriptionTranslates?: { languageId: string; languageShortCode: string; value: string }[]
  rentalPurposeId?: string
  minLeaseTerm?: number
  utilityPaymentType?: number
  nextAvailableDate?: string
}

// Listing filters for querying
export interface ListingsListParams {
  categoryId?: string
  regionId?: string
  districtId?: string
  renovationId?: string
  roomsCount?: number
  fromFloorNumber?: number
  toFloorNumber?: number
  fromSquare?: number
  toSquare?: number
  fromPrice?: number
  toPrice?: number
  filter?: string
  page?: number
  pageSize?: number
}

// My Listings params (simpler)
export interface MyListingsParams {
  filter?: string
  page?: number
  pageSize?: number
}

// Category reference data
export interface Category {
  id: string
  name: string
  description?: string
  parentId?: string | null
  iconUrl?: string
}

// Amenity reference data
export interface Amenity {
  id: string
  name: string
  iconUrl?: string
}
