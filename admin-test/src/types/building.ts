// Building types from new 18.txt API (Building API)

import type { LanguageValue, PagedList } from './common'

// ==================== Enums ====================

export enum BuildingType {
    Commercial = 0,
    Living = 1
}

// ==================== Building ====================

export interface Building {
    id: string
    number?: string
    isCommercial: boolean
    isLiving: boolean
    regionId?: string
    districtId?: string
    complexId?: string
    totalArea?: number
    floorsCount?: number
    latitude?: number
    longitude?: number
    address?: string
    descriptions?: LanguageValue[]
    images?: string[]
    // Included from responses
    regionName?: string
    districtName?: string
    complexName?: string
}

export interface CreateBuildingCommand {
    isCommercial: boolean
    isLiving: boolean
    number?: string | null
    regionId?: string | null
    districtId?: string | null
    complexId?: string | null
    totalArea?: number | null
    floorsCount?: number | null
    latitude?: number | null
    longitude?: number | null
    address?: string | null
    descriptions?: LanguageValue[] | null
    images?: string[] | null
}

export interface UpdateBuildingCommand extends CreateBuildingCommand {
    id: string
}

// API response types
export type GetBuildingsResponse = PagedList<Building>
export type GetBuildingByIdResponse = Building

// ==================== Complex ====================

export interface Complex {
    id: string
    name?: string
    regionId: string
    districtId: string
    isCommercial: boolean
    isLiving: boolean
    latitude?: number
    longitude?: number
    address?: string
    descriptions?: LanguageValue[]
    images?: string[]
    // Included from responses
    regionName?: string
    districtName?: string
}

export interface CreateComplexCommand {
    regionId: string
    districtId: string
    name?: string | null
    descriptions?: LanguageValue[] | null
    isCommercial: boolean
    isLiving: boolean
    longitude?: number | null
    latitude?: number | null
    address?: string | null
    images?: string[] | null
}

export interface UpdateComplexCommand extends CreateComplexCommand {
    id: string
}

export type GetComplexesResponse = PagedList<Complex>
export type GetComplexByIdResponse = Complex

// ==================== Floor ====================

export enum FloorType {
    Regular = 0,
    Basement = 1,
    GroundFloor = 2,
    Mezzanine = 3,
    Parking = 4,
    Rooftop = 5,
    Penthouse = 6,
    Technical = 7
}

export const FloorTypeLabels: Record<FloorType, string> = {
    [FloorType.Regular]: 'Regular',
    [FloorType.Basement]: 'Basement',
    [FloorType.GroundFloor]: 'Ground Floor',
    [FloorType.Mezzanine]: 'Mezzanine',
    [FloorType.Parking]: 'Parking',
    [FloorType.Rooftop]: 'Rooftop',
    [FloorType.Penthouse]: 'Penthouse',
    [FloorType.Technical]: 'Technical'
}

export interface Floor {
    id: string
    buildingId?: string
    realEstateId?: string
    number: number
    type?: FloorType
    label?: string
    totalArea?: number
    ceilingHeight?: number
    plan?: string
}

export interface CreateFloorCommand {
    number: number
    type?: FloorType | null
    label?: string | null
    totalArea?: number | null
    ceilingHeight?: number | null
    plan?: string | null
    buildingId?: string | null
    realEstateId?: string | null
}

export interface UpdateFloorCommand extends CreateFloorCommand {
    id: string
}

export type GetFloorsResponse = PagedList<Floor>
export type GetFloorByIdResponse = Floor
