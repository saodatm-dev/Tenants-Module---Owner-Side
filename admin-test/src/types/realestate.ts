// Real Estate Module Types

export interface RealEstateType {
    id: string
    typeName: string
    name: string
    description?: string
    icon?: string
    showBuildingSuggestion?: boolean
    showFloorSuggestion?: boolean
    canHaveUnits?: boolean
    canHaveMeters?: boolean
    canHaveFloors?: boolean
    translations?: { languageShortCode: string; value: string }[]
}

export interface Renovation {
    id: string
    name: string
    translations?: { languageShortCode: string; value: string }[]
}

export interface LandCategory {
    id: string
    name: string
    translations?: { languageShortCode: string; value: string }[]
}

export interface RoomType {
    id: string
    name: string
    translations?: { languageShortCode: string; value: string }[]
}

export interface RoomValue {
    roomTypeId: string
    area: number
}

export interface RealEstateRoom {
    id: string
    roomTypeId: string
    roomTypeName?: string
    totalArea: number
}

export interface RealEstateUnitRequest {
    renovationId?: string
    totalArea: number
    floorNumber?: number
    livingArea?: number
    ceilingHeight?: number
    roomsCount?: number
    plan?: string
    images?: string[]
    rooms?: RoomValue[]
}

export type ModerationStatus = 'InModeration' | 'Accept' | 'Reject' | 'Cancel' | 'Block'
export type RealEstateStatus = 'Draft' | 'Active' | 'Inactive'

export interface RealEstateUnit {
    id: string
    realEstateId?: string
    realEstateTypeId?: string
    realEstateTypeName?: string
    renovationId?: string
    renovationName?: string
    totalArea: number
    floorNumber?: number
    number?: string
    livingArea?: number
    ceilingHeight?: number
    roomsCount?: number
    plan?: string
    coordinates?: { x: number; y: number }[]  // Polygon coordinates
    reason?: string  // Legacy: used to store polygon coordinates as JSON string
    images?: string[]
    rooms?: RealEstateRoom[]
    status?: RealEstateStatus
    moderationStatus?: ModerationStatus
}

export interface RealEstate {
    id: string
    ownerId: string
    realEstateTypeId: string
    realEstateTypeName?: string
    renovationId?: string
    renovationName?: string
    landCategoryId?: string
    landCategoryName?: string
    buildingId?: string
    buildingNumber?: string
    floorId?: string
    floorNumber?: number
    totalFloors?: number
    aboveFloors?: number
    belowFloors?: number
    number?: string
    totalArea?: number
    livingArea?: number
    ceilingHeight?: number
    roomsCount?: number
    cadastralNumber?: string
    regionId?: string
    regionName?: string
    districtId?: string
    districtName?: string
    latitude?: number
    longitude?: number
    address?: string
    plan?: string
    images?: string[]
    status?: RealEstateStatus
    moderationStatus?: ModerationStatus
    coordinates?: { x: number; y: number }[]  // Polygon coordinates
    reason?: string  // Legacy field
    rooms?: RealEstateRoom[]
    units?: RealEstateUnit[]
    createdAt?: string
    updatedAt?: string
}

export interface CreateRealEstateRequest {
    realEstateTypeId: string
    totalArea: number
    renovationId?: string
    landCategoryId?: string
    cadastralNumber?: string
    number?: string
    buildingId?: string
    floorId?: string
    buildingNumber?: string
    floorNumber?: number
    livingArea?: number
    ceilingHeight?: number
    totalFloors?: number
    aboveFloors?: number
    belowFloors?: number
    roomsCount?: number
    units?: RealEstateUnitRequest[]
    rooms?: RoomValue[]
    regionId?: string
    districtId?: string
    latitude?: number
    longitude?: number
    address?: string
    plan?: string
    images?: string[]
    amenityIds?: string[]  // Selected amenities
}

export interface UpdateRealEstateRequest extends CreateRealEstateRequest {
    id: string
}

export interface RealEstatesListParams {
    realEstateTypeId?: string
    regionId?: string
    districtId?: string
    renovationId?: string
    landCategoryId?: string
    roomsCount?: number
    floorNumber?: number
    filter?: string
    page?: number
    pageSize?: number
}
