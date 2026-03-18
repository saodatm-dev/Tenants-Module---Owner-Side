import type { ReactNode } from 'react'

// API Response Types
export interface PagedList<T> {
  items: T[]
  pageNumber: number
  totalPages: number
  totalCount: number
  hasPreviousPage: boolean
  hasNextPage: boolean
}

export interface Result<T> {
  isSuccess: boolean
  value?: T
  error?: string
}

// Common Module Types
export interface Region {
  id: string
  name: string
  order: number
}

export interface District {
  id: string
  name: string
  nameEn?: string
  nameRu?: string
  nameUz?: string
  regionName: string
  order: number
}

export interface Bank {
  id: string
  name: string
}

export interface Currency {
  id: string
  code: string
  name: string
  symbol?: string
  order: number
}

export interface Language {
  id: string
  name: string
  code?: string
  shortCode: string
  nativeName?: string
  order: number
}

// Identity Module Types
export type RegisterType = 'PhoneNumber' | 'EImzo' | 'OneID'

export interface SystemUser {
  id: string
  phoneNumber?: string
  firstName: string
  lastName: string
  middleName?: string
  photo?: string
  accountsCount: number
  isVerified: boolean
  registerType: RegisterType | number
  isActive: boolean
}

// Building Module Types
export type BuildingType = 'Living' | 'Commercial'

export interface Complex {
  id: string
  region: string
  district: string
  districtId?: string
  name: string
  description?: string
  isCommercial: boolean
  isLiving: boolean
  latitude?: number
  longitude?: number
  address?: string
  images?: string[]
}

export interface Building {
  id: string
  // List response fields (GetBuildingsResponse)
  complex?: string  // Complex name from list response
  region: string
  district: string
  number: string
  description: string
  isCommercial: boolean
  isLiving: boolean
  latitude?: number
  longitude?: number
  address?: string
  totalArea?: number
  images?: string[]
  floors: Floor[]
  // Detail response fields (GetBuildingByIdResponse) for edit context
  complexId?: string
  regionId?: string
  districtId?: string
  floorsCount?: number
}

export interface BuildingCreate {
  isCommercial: boolean
  isLiving: boolean
  number: string
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
}

export interface BuildingUpdate {
  id: string
  number: string
  descriptions?: LanguageValue[]
  isCommercial: boolean
  isLiving: boolean
  regionId?: string
  districtId?: string
  complexId?: string
  totalSquare?: number
  floorsCount?: number
  latitude?: number
  longitude?: number
  address?: string
  images?: string[]
}

export interface Floor {
  id: string
  buildingNumber: string
  buildingTypes?: BuildingType[]
  number: number
  type?: number
  label?: string
  totalArea?: number
  ceilingHeight?: number
  plan?: string
}

export interface FloorCreate {
  buildingId: string
  number: number
  type?: number
  label?: string
  totalArea?: number
  ceilingHeight?: number
  plan?: string
  realEstateId?: string
}

export interface FloorUpdate {
  id: string
  buildingId: string
  number: number
  totalArea?: number
  ceilingHeight?: number
  floorPlan?: string
}

export interface Amenity {
  id: string
  name: string
  nameEn?: string
  nameRu?: string
  nameUz?: string
  categoryId?: string
  icon?: string
  iconUrl: string
}

export interface AmenityCategory {
  id: string
  name: string
  nameEn?: string
  nameRu?: string
  nameUz?: string
}

export interface RoomType {
  id: string
  name: string
  nameEn?: string
  nameRu?: string
  nameUz?: string
}

// LanguageValue for multi-language fields from backend
export interface LanguageValue {
  languageId: string
  languageShortCode: string
  value: string
}

// MeterType list response (GET /metertypes)
export interface MeterType {
  id: string
  name: string
  description: string
  icon?: string
}

// MeterType detail response (GET /metertypes/{id})
export interface MeterTypeDetail {
  id: string
  names: LanguageValue[]
  description: LanguageValue[]
  icon?: string
}

// MeterType create/update request
export interface MeterTypeRequest {
  id?: string
  names: LanguageValue[]
  description: LanguageValue[]
  icon?: string
}

export interface MeterTariff {
  id: string
  meterTypeId: string
  type: number
  price: number
  fixedPrice?: number
  validFrom?: string
  validTo?: string
  isActual?: boolean
  minLimit?: number
  maxLimit?: number
  season?: number
  socialNormLimit?: number
  comment?: string
}

export interface Category {
  id: string
  buildingType: BuildingType
  iconUrl: string
  name: string
  nameEn?: string
  nameRu?: string
  nameUz?: string
}

export interface RealEstateType {
  id: string
  icon?: string
  name: string
  nameEn?: string
  nameRu?: string
  nameUz?: string
  description: string
  showBuildingSuggestion: boolean
  showFloorSuggestion: boolean
  canHaveUnits: boolean
  canHaveMeters: boolean
}

export interface ListingCategory {
  id: string
  parentId?: string
  buildingType: BuildingType
  iconUrl: string
  name: string
  nameEn?: string
  nameRu?: string
  nameUz?: string
}

export interface Renovation {
  id: string
  name: string
  nameEn?: string
  nameRu?: string
  nameUz?: string
}

export interface LandCategory {
  id: string
  name: string
  nameEn?: string
  nameRu?: string
  nameUz?: string
}

export interface ProductionType {
  id: string
  name: string
  nameEn?: string
  nameRu?: string
  nameUz?: string
}

// Moderation Types - Backend returns numeric enum values
// ModerationStatus: 0=Cancel, 1=InModeration, 2=Accept, 3=Reject, 4=Block
// Status: 0=Draft, 1=Active, 2=Inactive, 3=Booked, 4=Rented
export type ModerationStatus = number
export type Status = number

// Moderation filter params extending standard pagination
export interface ModerationFilterParams extends PaginationParams {
  filter?: string
  moderationStatus?: number
}

// ── List-level types (GET /moderations/...) ──

export interface ModerationListing {
  id: string
  ownerId: string
  complexName?: string
  buildingNumber?: string
  address?: string
  categories: string[]
  totalArea: number
  roomsCount?: number
  priceForMonth?: number
  pricePerSquareMeter?: number
  description?: string
  status: Status
  moderationStatus: ModerationStatus
  createdAt: string
  objectNames?: string[]
}

export interface ModerationRealEstate {
  id: string
  ownerId: string
  number?: string
  buildingNumber?: string
  floorNumber?: number
  address?: string
  realEstateType: string
  totalArea?: number
  roomsCount?: number
  status: Status
  moderationStatus: ModerationStatus
  createdAt: string
  objectNames?: string[]
}

export interface ModerationRealEstateUnit {
  id: string
  ownerId: string
  roomNumber?: string
  floorNumber?: number
  totalArea?: number
  ceilingHeight?: number
  status: Status
  moderationStatus: ModerationStatus
  createdAt: string
  objectNames?: string[]
}

// ── Detail-level types (GET /moderations/.../{ id }) ──

export interface ModerationListingFloor {
  floorId: string
  floorNumber: number
}

export interface ModerationListingDetail {
  id: string
  ownerId: string
  status: Status
  moderationStatus: ModerationStatus
  reason?: string
  categories: string[]
  complex?: string
  building?: string
  floors?: ModerationListingFloor[]
  floorNumbers?: number[]
  roomsCount?: number
  totalArea: number
  livingArea?: number
  ceilingHeight?: number
  priceForMonth?: number
  pricePerSquareMeter?: number
  description?: string
  region?: string
  district?: string
  latitude?: number
  longitude?: number
  address?: string
  createdAt: string
  objectNames?: string[]
}

export interface ModerationRealEstateDetail {
  id: string
  ownerId: string
  status: Status
  moderationStatus: ModerationStatus
  reason?: string
  type: string
  buildingName?: string
  floor?: number
  number?: string
  roomsCount?: number
  totalArea?: number
  livingArea?: number
  ceilingHeight?: number
  cadastralNumber?: string
  region?: string
  district?: string
  latitude?: number
  longitude?: number
  address?: string
  plan?: string
  createdAt: string
  objectNames?: string[]
}

export interface ModerationUnitDetail {
  id: string
  ownerId: string
  moderationStatus: ModerationStatus
  type: string
  floor?: number
  room?: string
  totalArea?: number
  ceilingHeight?: number
  plan?: string
  images?: string[]
}

// Identity Types
export interface User {
  userId: string
  isIndividual: boolean
  phoneNumber: string
  firstName: string
  lastName: string
  middleName?: string
  photo?: string
  permissions: string[]
  companyName?: string
  companyTin?: string
}

export type AccountType = 'Client' | 'Owner'

export interface Account {
  photo: string
  name: string
  accountType: AccountType
  key: string
}

// Auth Types
export interface LoginRequest {
  phoneNumber: string
  password: string
}

export interface AuthState {
  isAuthenticated: boolean
  user: User | null
  isLoading: boolean
}

// App Context Types
export type Theme = 'light' | 'dark'
export type LanguageCode = 'en' | 'uz' | 'ru'

export interface Notification {
  id: string
  title: string
  message: string
  read: boolean
  createdAt: string
}

export interface AppState {
  theme: Theme
  language: LanguageCode
  notifications: Notification[]
  sidebarCollapsed: boolean
}

// Table & Pagination Types
export interface PaginationParams {
  pageNumber: number
  pageSize: number
}

export interface TableColumn<T> {
  key: keyof T | string
  header: string
  render?: (item: T) => ReactNode
}
