// API Service for backend communication

import type {
  AuthenticationResponse,
  EImzoAuthRequest,
  OneIdAuthRequest,
  LoginCredentials
} from '@/types/auth'

import type {
  GetInvitationsResponse,
  GetInvitationByIdResponse,
  CreateInvitationCommand,
  UpdateInvitationCommand,
  RejectInvitationCommand
} from '@/types/invitation'

import type {
  GetListingRequestsResponse
} from '@/types/listingRequest'

import type {
  MarketplaceCategoryGroup,
  GetMarketplaceListingsResponse,
  GetClientListingRequestsResponse,
  CreateListingRequestCommand
} from '@/types/marketplace'

// API base path - hardcoded for production
const API_BASE = 'https://maydon-dev.maydon.tech/api/v1'

class ApiError extends Error {
  constructor(public status: number, message: string) {
    super(message)
    this.name = 'ApiError'
  }
}

// Helper to parse error responses
function parseErrorMessage(errorText: string, fallback: string): string {
  try {
    const errorJson = JSON.parse(errorText)
    if (errorJson.errors && Array.isArray(errorJson.errors)) {
      return errorJson.errors.map((e: { description?: string }) => e.description).join(', ')
    } else if (errorJson.message) {
      return errorJson.message
    } else if (errorJson.detail) {
      return errorJson.detail
    } else if (errorJson.title) {
      return errorJson.title
    }
  } catch {
    // Not JSON
  }
  return errorText || fallback
}

// Token refresh state
let isRefreshing = false
let refreshPromise: Promise<boolean> | null = null

// Check if token is expired or about to expire (within 1 minute)
function isTokenExpired(): boolean {
  const expiryStr = localStorage.getItem('token_expiry')
  if (!expiryStr) return true

  const expiry = new Date(expiryStr).getTime()
  const now = Date.now()
  const oneMinute = 60 * 1000

  return now >= (expiry - oneMinute)
}

// Refresh the token
async function refreshToken(): Promise<boolean> {
  const token = localStorage.getItem('auth_token')
  const storedRefreshToken = localStorage.getItem('refresh_token')

  if (!storedRefreshToken) {
    return false
  }

  try {
    const headers: Record<string, string> = {
      'Content-Type': 'application/json',
    }
    if (token) {
      headers['Authorization'] = `Bearer ${token}`
    }

    const response = await fetch(`${API_BASE}/authentication/refresh-token`, {
      method: 'POST',
      headers,
      body: JSON.stringify({ refreshToken: storedRefreshToken }),
    })

    if (!response.ok) {
      // Refresh failed - clear auth and redirect to login
      localStorage.removeItem('auth_token')
      localStorage.removeItem('refresh_token')
      localStorage.removeItem('token_expiry')
      localStorage.removeItem('refresh_token_expiry')
      window.location.href = '/login'
      return false
    }

    const data = await response.json()

    // Store new tokens
    localStorage.setItem('auth_token', data.token)
    localStorage.setItem('refresh_token', data.refreshToken)
    localStorage.setItem('token_expiry', data.tokenExpiredTime)
    localStorage.setItem('refresh_token_expiry', data.refreshTokenExpiredTime)

    return true
  } catch (error) {
    console.error('Token refresh failed:', error)
    return false
  }
}

// Ensure token is valid before making request
async function ensureValidToken(): Promise<void> {
  if (isTokenExpired()) {
    // If already refreshing, wait for that to complete
    if (isRefreshing && refreshPromise) {
      await refreshPromise
      return
    }

    isRefreshing = true
    refreshPromise = refreshToken()
    await refreshPromise
    isRefreshing = false
    refreshPromise = null
  }
}

async function request<T>(
  endpoint: string,
  options: RequestInit = {},
  skipAuth = false
): Promise<T> {
  // Check and refresh token if needed (skip for auth endpoints)
  if (!skipAuth && localStorage.getItem('auth_token')) {
    await ensureValidToken()
  }

  const token = localStorage.getItem('auth_token')

  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    'lang': 'en',
    ...options.headers as Record<string, string>,
  }

  if (token && !skipAuth) {
    headers['Authorization'] = `Bearer ${token}`
  }

  const url = `${API_BASE}${endpoint}`
  console.log('[API] Fetching:', url, options.method || 'GET')
  let response = await fetch(url, {
    ...options,
    headers,
  })

  // If 401, try to refresh token and retry once
  if (response.status === 401 && !skipAuth) {
    const refreshed = await refreshToken()
    if (refreshed) {
      const newToken = localStorage.getItem('auth_token')
      if (newToken) {
        headers['Authorization'] = `Bearer ${newToken}`
      }
      response = await fetch(`${API_BASE}${endpoint}`, {
        ...options,
        headers,
      })
    }
  }

  if (!response.ok) {
    const errorText = await response.text()
    throw new ApiError(response.status, parseErrorMessage(errorText, response.statusText))
  }

  // Handle empty responses
  const text = await response.text()
  if (!text) return {} as T

  return JSON.parse(text) as T
}

// Request that returns raw text instead of JSON
async function requestText(
  endpoint: string,
  options: RequestInit = {}
): Promise<string> {
  // Check and refresh token if needed
  if (localStorage.getItem('auth_token')) {
    await ensureValidToken()
  }

  const token = localStorage.getItem('auth_token')

  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    'lang': 'en',
    ...options.headers as Record<string, string>,
  }

  if (token) {
    headers['Authorization'] = `Bearer ${token}`
  }

  let response = await fetch(`${API_BASE}${endpoint}`, {
    ...options,
    headers,
  })

  // If 401, try to refresh token and retry once
  if (response.status === 401) {
    const refreshed = await refreshToken()
    if (refreshed) {
      const newToken = localStorage.getItem('auth_token')
      if (newToken) {
        headers['Authorization'] = `Bearer ${newToken}`
      }
      response = await fetch(`${API_BASE}${endpoint}`, {
        ...options,
        headers,
      })
    }
  }

  if (!response.ok) {
    const errorText = await response.text()
    throw new ApiError(response.status, parseErrorMessage(errorText, response.statusText))
  }

  const text = await response.text()
  // Remove surrounding quotes if present
  return text.replace(/^"|"$/g, '')
}

// Response types
interface OtpResponse {
  success?: boolean
}

export interface DashboardStatistics {
  realEstates: { total: number; active: number; inactive: number; booked: number; rented: number }
  listings: { total: number; active: number }
  listingRequests: { total: number; pending: number }
  leasesCount: number
  wishlistsCount: number
  occupancyRate: number
}

export const api = {
  // ==================== Challenge Endpoints ====================
  async getEImzoChallenge(): Promise<string> {
    const response = await fetch(`${API_BASE}/authentication/challenge`)
    if (!response.ok) throw new ApiError(response.status, 'Не удалось получить challenge')
    const text = await response.text()
    try {
      const json = JSON.parse(text)
      return json.challenge || json
    } catch {
      return text.replace(/"/g, '')
    }
  },

  // ==================== E-IMZO Authentication ====================
  async eimzoRegister(data: EImzoAuthRequest): Promise<AuthenticationResponse> {
    return request<AuthenticationResponse>('/registration/eimzo', {
      method: 'POST',
      body: JSON.stringify(data),
    })
  },

  async eimzoLogin(data: EImzoAuthRequest): Promise<AuthenticationResponse> {
    return request<AuthenticationResponse>('/login/eimzo', {
      method: 'POST',
      body: JSON.stringify(data),
    })
  },

  // ==================== OneID Authentication ====================
  async oneidRegister(data: OneIdAuthRequest): Promise<AuthenticationResponse> {
    return request<AuthenticationResponse>('/registration/oneid', {
      method: 'POST',
      body: JSON.stringify(data),
    })
  },

  async oneidLogin(data: OneIdAuthRequest): Promise<AuthenticationResponse> {
    return request<AuthenticationResponse>('/login/oneid', {
      method: 'POST',
      body: JSON.stringify(data),
    })
  },

  // ==================== Phone Registration (3-step) ====================
  // Step 1: Check if phone number already exists
  // Returns true if phone exists (200), false if not (404)
  async checkPhoneNumber(phoneNumber: string): Promise<boolean> {
    const response = await fetch(`${API_BASE}/registration/check-phone-number`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ phoneNumber }),
    })
    // 200 = phone exists, 404 = phone doesn't exist
    if (response.status === 404) {
      return false // Phone not registered
    }
    if (response.ok) {
      return true // Phone already registered
    }
    // Other errors
    const errorText = await response.text()
    throw new ApiError(response.status, parseErrorMessage(errorText, 'Failed to check phone number'))
  },

  // Send OTP to phone number
  async sendOtp(phoneNumber: string): Promise<OtpResponse> {
    return request<OtpResponse>('/otps', {
      method: 'POST',
      body: JSON.stringify({ phoneNumber }),
    })
  },

  // Step 2: Verify OTP and get session key (phone-number-confirm now takes phoneNumber + code)
  async verifyPhoneOtp(phoneNumber: string, code: string): Promise<string> {
    return requestText('/registration/phone-number-confirm', {
      method: 'POST',
      body: JSON.stringify({ phoneNumber, code }),
    })
  },

  // Step 3: Create password with session key (returns auth token)
  async createPassword(key: string, password: string): Promise<AuthenticationResponse> {
    return request<AuthenticationResponse>('/registration/create-password', {
      method: 'POST',
      body: JSON.stringify({ key, password }),
    })
  },

  // ==================== Password Login ====================
  async login(data: LoginCredentials): Promise<AuthenticationResponse> {
    return request<AuthenticationResponse>('/login/phone-number', {
      method: 'POST',
      body: JSON.stringify(data),
    })
  },

  // ==================== Forgot Password ====================
  // Step 1: Send OTP and verify code (returns session key)
  async forgotPassword(phoneNumber: string, code: string): Promise<string> {
    return requestText('/authentication/forgot-password', {
      method: 'POST',
      body: JSON.stringify({ phoneNumber, code }),
    })
  },

  // Step 2: Confirm password reset with new password (returns auth token)
  async forgotPasswordConfirm(key: string, password: string): Promise<AuthenticationResponse> {
    return request<AuthenticationResponse>('/authentication/forgot-password-confirm', {
      method: 'POST',
      body: JSON.stringify({ key, password }),
    })
  },

  // ==================== Token Management ====================
  async refreshToken(refreshToken: string): Promise<AuthenticationResponse> {
    return request<AuthenticationResponse>('/auth/refresh', {
      method: 'POST',
      body: JSON.stringify({ refreshToken }),
    })
  },

  async logout(): Promise<void> {
    return request<void>('/auth/logout', {
      method: 'POST',
    })
  },

  // ==================== Account Switching ====================
  async getMyAccounts(): Promise<import('@/types/auth').Account[]> {
    return request<import('@/types/auth').Account[]>('/accounts/my')
  },

  async changeAccount(key: string): Promise<import('@/types/auth').AuthenticationResponse> {
    // The key is already URL-encoded, but we need to encode it again for the path
    const encodedKey = encodeURIComponent(key)
    return request<import('@/types/auth').AuthenticationResponse>(`/accounts/change/${encodedKey}`, {
      method: 'POST',
    })
  },

  async createOwnerAccount(): Promise<import('@/types/auth').AuthenticationResponse> {
    return request<import('@/types/auth').AuthenticationResponse>('/accounts/create-owner', {
      method: 'POST',
    })
  },

  async createClientAccount(): Promise<import('@/types/auth').AuthenticationResponse> {
    return request<import('@/types/auth').AuthenticationResponse>('/accounts/create-client', {
      method: 'POST',
    })
  },

  // ==================== User Profile ====================
  async getUserProfile(): Promise<import('@/types/settings').UserProfile> {
    return request<import('@/types/settings').UserProfile>('/users/profile')
  },

  async updateUserProfile(data: import('@/types/settings').UpdateProfileRequest): Promise<void> {
    return request<void>('/users/profile', {
      method: 'PUT',
      body: JSON.stringify(data),
    })
  },

  // ==================== Company Users ====================
  async getCompanyUsers(page = 1, pageSize = 10): Promise<import('@/types/settings').CompanyUsersResponse> {
    return request<import('@/types/settings').CompanyUsersResponse>(`/companies/users?Page=${page}&PageSize=${pageSize}`)
  },

  async activateCompanyUser(userId: string): Promise<string> {
    return request<string>(`/accounts/${userId}/activate`, {
      method: 'POST',
    })
  },

  async deactivateCompanyUser(userId: string): Promise<string> {
    return request<string>(`/accounts/${userId}/deactivate`, {
      method: 'POST',
    })
  },

  // ==================== Buildings ====================
  async getBuildings(params?: { page?: number; pageSize?: number; filter?: string }) {
    const query = new URLSearchParams()
    if (params?.page) query.set('Page', params.page.toString())
    if (params?.pageSize) query.set('PageSize', params.pageSize.toString())
    if (params?.filter) query.set('Filter', params.filter)
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<any>(`/buildings${queryStr}`)
  },

  async getBuildingById(id: string) {
    return request<any>(`/buildings/${id}`)
  },

  async createBuilding(data: any): Promise<string> {
    return request<string>('/buildings', {
      method: 'POST',
      body: JSON.stringify(data),
    })
  },

  async updateBuilding(data: any): Promise<string> {
    return request<string>('/buildings', {
      method: 'PUT',
      body: JSON.stringify(data),
    })
  },

  async deleteBuilding(id: string): Promise<string> {
    return request<string>(`/buildings/${id}`, {
      method: 'DELETE',
    })
  },

  // ==================== Complexes ====================
  async getComplexes(params?: { page?: number; pageSize?: number; filter?: string }) {
    const query = new URLSearchParams()
    if (params?.page) query.set('Page', params.page.toString())
    if (params?.pageSize) query.set('PageSize', params.pageSize.toString())
    if (params?.filter) query.set('Filter', params.filter)
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<any>(`/complexes${queryStr}`)
  },

  async getComplexById(id: string) {
    return request<any>(`/complexes/${id}`)
  },

  // ==================== Real Estates ====================
  async getRealEstates(params?: {
    buildingId?: string
    floorId?: string
    realEstateTypeId?: string
    regionId?: string
    districtId?: string
    renovationId?: string
    roomsCount?: number
    floorNumber?: number
    filter?: string
    page?: number
    pageSize?: number
  }) {
    const query = new URLSearchParams()
    if (params?.buildingId) query.set('BuildingId', params.buildingId)
    if (params?.floorId) query.set('FloorId', params.floorId)
    if (params?.realEstateTypeId) query.set('RealEstateTypeId', params.realEstateTypeId)
    if (params?.regionId) query.set('RegionId', params.regionId)
    if (params?.districtId) query.set('DistrictId', params.districtId)
    if (params?.renovationId) query.set('RenovationId', params.renovationId)
    if (params?.roomsCount) query.set('RoomsCount', params.roomsCount.toString())
    if (params?.floorNumber) query.set('FloorNumber', params.floorNumber.toString())
    if (params?.filter) query.set('Filter', params.filter)
    if (params?.page) query.set('Page', params.page.toString())
    if (params?.pageSize) query.set('PageSize', params.pageSize.toString())
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<any>(`/real-estates${queryStr}`)
  },

  async getMyRealEstates(params?: {
    buildingId?: string
    regionId?: string
    districtId?: string
    filter?: string
    page?: number
    pageSize?: number
  }) {
    const query = new URLSearchParams()
    if (params?.buildingId) query.set('BuildingId', params.buildingId)
    if (params?.regionId) query.set('RegionId', params.regionId)
    if (params?.districtId) query.set('DistrictId', params.districtId)
    if (params?.filter) query.set('Filter', params.filter)
    if (params?.page) query.set('Page', params.page.toString())
    if (params?.pageSize) query.set('PageSize', params.pageSize.toString())
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<any>(`/real-estates/my${queryStr}`)
  },

  async getRealEstateById(id: string) {
    return request<any>(`/real-estates/${id}`)
  },

  async createRealEstate(data: any): Promise<string> {
    return request<string>('/real-estates', {
      method: 'POST',
      body: JSON.stringify(data),
    })
  },

  async updateRealEstate(data: any): Promise<string> {
    return request<string>('/real-estates', {
      method: 'PUT',
      body: JSON.stringify(data),
    })
  },

  async deleteRealEstate(id: string): Promise<string> {
    return request<string>(`/real-estates/${id}`, {
      method: 'DELETE',
    })
  },

  // ==================== Real Estate Reference Data ====================
  async getRealEstateTypes(filter?: string): Promise<import('@/types/realestate').RealEstateType[]> {
    const query = filter ? `?Filter=${encodeURIComponent(filter)}` : ''
    return request<import('@/types/realestate').RealEstateType[]>(`/real-estate-types${query}`)
  },

  async getRenovations(params?: { page?: number; pageSize?: number; filter?: string }) {
    const query = new URLSearchParams()
    if (params?.page) query.set('Page', params.page.toString())
    if (params?.pageSize) query.set('PageSize', params.pageSize.toString())
    if (params?.filter) query.set('Filter', params.filter)
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<any>(`/renovations${queryStr}`)
  },

  async getLandCategories(params?: { page?: number; pageSize?: number; filter?: string }) {
    const query = new URLSearchParams()
    if (params?.page) query.set('Page', params.page.toString())
    if (params?.pageSize) query.set('PageSize', params.pageSize.toString())
    if (params?.filter) query.set('Filter', params.filter)
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<any>(`/land-categories${queryStr}`)
  },

  async getRoomTypes(filter?: string) {
    const query = filter ? `?Filter=${encodeURIComponent(filter)}` : ''
    return request<any>(`/room-types${query}`)
  },

  async getFloors(params: { buildingId: string; page?: number; pageSize?: number; filter?: string }) {
    const query = new URLSearchParams()
    query.set('BuildingId', params.buildingId)
    if (params.page) query.set('Page', params.page.toString())
    if (params.pageSize) query.set('PageSize', params.pageSize.toString())
    if (params.filter) query.set('Filter', params.filter)
    return request<any>(`/floors?${query.toString()}`)
  },

  async getFloorById(id: string) {
    return request<any>(`/floors/${id}`)
  },

  async createFloor(data: any): Promise<string> {
    return request<string>('/floors', {
      method: 'POST',
      body: JSON.stringify(data),
    })
  },

  async updateFloor(data: any): Promise<string> {
    return request<string>('/floors', {
      method: 'PUT',
      body: JSON.stringify(data),
    })
  },

  async deleteFloor(id: string): Promise<string> {
    return request<string>(`/floors/${id}`, {
      method: 'DELETE',
    })
  },

  // Get image records for a real estate by ID
  // Returns array of { id: string, image: string } where image is the file key
  async getRealEstateImages(realEstateId: string): Promise<{ id: string; image: string }[]> {
    return request<{ id: string; image: string }[]>(`/real-estate-images/${realEstateId}`)
  },

  // Delete a real estate image by its image record ID (not the file key)
  async deleteRealEstateImage(imageId: string): Promise<void> {
    return request<void>(`/real-estate-images/${imageId}`, {
      method: 'DELETE',
    })
  },

  // Upload/associate image keys with a real estate
  // This should be called AFTER uploading files to /files/upload and getting the keys
  async uploadRealEstateImages(realEstateId: string, imageKeys: string[]): Promise<void> {
    return request<void>(`/real-estate-images/upload/${realEstateId}`, {
      method: 'POST',
      body: JSON.stringify({
        realEstateId,
        images: imageKeys
      }),
    })
  },

  // ==================== Real Estate Units ====================
  async getRealEstateUnits(params?: {
    realEstateId?: string
    realEstateTypeId?: string
    renovationId?: string
    floorNumber?: number
    filter?: string
    page?: number
    pageSize?: number
  }) {
    const query = new URLSearchParams()
    if (params?.realEstateId) query.set('RealEstateId', params.realEstateId)
    if (params?.realEstateTypeId) query.set('RealEstateTypeId', params.realEstateTypeId)
    if (params?.renovationId) query.set('RenovationId', params.renovationId)
    if (params?.floorNumber) query.set('FloorNumber', params.floorNumber.toString())
    if (params?.filter) query.set('Filter', params.filter)
    if (params?.page) query.set('Page', params.page.toString())
    if (params?.pageSize) query.set('PageSize', params.pageSize.toString())
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<any>(`/real-estate-units${queryStr}`)
  },

  async getRealEstateUnitById(id: string) {
    return request<any>(`/real-estate-units/${id}`)
  },

  async createRealEstateUnit(data: {
    realEstateId: string
    realEstateTypeId: string
    floorId?: string
    roomId?: string
    renovationId?: string
    floorNumber?: number
    roomNumber?: string
    totalArea: number
    ceilingHeight?: number
    plan?: string
    coordinates?: { x: number; y: number }[]
    images?: string[]
  }): Promise<string> {
    return request<string>('/real-estate-units', {
      method: 'POST',
      body: JSON.stringify(data),
    })
  },

  async updateRealEstateUnit(data: {
    id: string
    realEstateTypeId: string
    floorId?: string
    roomId?: string
    renovationId?: string
    floorNumber?: number
    roomNumber?: string
    totalArea: number
    ceilingHeight?: number
    plan?: string
    coordinates?: { x: number; y: number }[]
    images?: string[]
  }): Promise<string> {
    return request<string>('/real-estate-units', {
      method: 'PUT',
      body: JSON.stringify(data),
    })
  },

  async deleteRealEstateUnit(id: string): Promise<void> {
    return request<void>(`/real-estate-units/${id}`, {
      method: 'DELETE',
    })
  },

  // ==================== Listings ====================
  async getListings(params?: {
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
  }) {
    const query = new URLSearchParams()
    if (params?.categoryId) query.set('CategoryId', params.categoryId)
    if (params?.regionId) query.set('RegionId', params.regionId)
    if (params?.districtId) query.set('DistrictId', params.districtId)
    if (params?.renovationId) query.set('RenovationId', params.renovationId)
    if (params?.roomsCount) query.set('RoomsCount', params.roomsCount.toString())
    if (params?.fromFloorNumber) query.set('FromFloorNumber', params.fromFloorNumber.toString())
    if (params?.toFloorNumber) query.set('ToFloorNumber', params.toFloorNumber.toString())
    if (params?.fromSquare) query.set('FromSquare', params.fromSquare.toString())
    if (params?.toSquare) query.set('ToSquare', params.toSquare.toString())
    if (params?.fromPrice) query.set('FromPrice', params.fromPrice.toString())
    if (params?.toPrice) query.set('ToPrice', params.toPrice.toString())
    if (params?.filter) query.set('Filter', params.filter)
    if (params?.page) query.set('Page', params.page.toString())
    if (params?.pageSize) query.set('PageSize', params.pageSize.toString())
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<any>(`/listings${queryStr}`)
  },

  async getMyListings(params?: { filter?: string; page?: number; pageSize?: number }) {
    const query = new URLSearchParams()
    if (params?.filter) query.set('Filter', params.filter)
    if (params?.page) query.set('Page', params.page.toString())
    if (params?.pageSize) query.set('PageSize', params.pageSize.toString())
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<any>(`/listings/my${queryStr}`)
  },

  async getListingById(id: string) {
    return request<any>(`/listings/${id}`)
  },

  async createListing(data: any): Promise<string> {
    return request<string>('/listings', {
      method: 'POST',
      body: JSON.stringify(data),
    })
  },

  async updateListing(data: any): Promise<string> {
    return request<string>('/listings', {
      method: 'PUT',
      body: JSON.stringify(data),
    })
  },

  async deleteListing(id: string): Promise<string> {
    return request<string>(`/listings/${id}`, {
      method: 'DELETE',
    })
  },

  async getCategories(params?: { page?: number; pageSize?: number; filter?: string }) {
    const query = new URLSearchParams()
    if (params?.page) query.set('Page', params.page.toString())
    if (params?.pageSize) query.set('PageSize', params.pageSize.toString())
    if (params?.filter) query.set('Filter', params.filter)
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<any>(`/listing-categories${queryStr}`)
  },

  async getAmenities(params?: { page?: number; pageSize?: number; filter?: string }) {
    const query = new URLSearchParams()
    if (params?.page) query.set('Page', params.page.toString())
    if (params?.pageSize) query.set('PageSize', params.pageSize.toString())
    if (params?.filter) query.set('Filter', params.filter)
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<any>(`/amenities${queryStr}`)
  },

  // Get amenities for a specific real estate (grouped by category)
  async getRealEstateAmenities(realEstateId: string): Promise<{ categoryName: string; amenities: { id: string; name: string; iconUrl?: string }[] }[]> {
    return request<{ categoryName: string; amenities: { id: string; name: string; iconUrl?: string }[] }[]>(`/amenities/${realEstateId}`)
  },

  // Get all amenity categories with their amenities
  async getAmenityCategoriesWithAmenities(): Promise<{ id: string; name: string; amenities: { id: string; name: string; iconUrl: string }[] }[]> {
    return request<{ id: string; name: string; amenities: { id: string; name: string; iconUrl: string }[] }[]>('/amenity-categories/amenities-with-categories')
  },

  async getRentalPurposes(params?: { page?: number; pageSize?: number; filter?: string }) {
    const query = new URLSearchParams()
    if (params?.page) query.set('Page', params.page.toString())
    if (params?.pageSize) query.set('PageSize', params.pageSize.toString())
    if (params?.filter) query.set('Filter', params.filter)
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<any>(`/rental-purposes${queryStr}`)
  },

  // ==================== Common Reference Data ====================
  async getRegions(params?: { page?: number; pageSize?: number; filter?: string }) {
    const query = new URLSearchParams()
    if (params?.page) query.set('Page', params.page.toString())
    if (params?.pageSize) query.set('PageSize', params.pageSize.toString())
    if (params?.filter) query.set('Filter', params.filter)
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<any>(`/regions${queryStr}`)
  },

  async getDistricts(params?: { page?: number; pageSize?: number; filter?: string; regionId?: string }) {
    const query = new URLSearchParams()
    if (params?.page) query.set('Page', params.page.toString())
    if (params?.pageSize) query.set('PageSize', params.pageSize.toString())
    if (params?.filter) query.set('Filter', params.filter)
    if (params?.regionId) query.set('RegionId', params.regionId)
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<any>(`/districts${queryStr}`)
  },

  async getBanks(params?: { page?: number; pageSize?: number; filter?: string }) {
    const query = new URLSearchParams()
    if (params?.page) query.set('Page', params.page.toString())
    if (params?.pageSize) query.set('PageSize', params.pageSize.toString())
    if (params?.filter) query.set('Filter', params.filter)
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<any>(`/banks${queryStr}`)
  },

  async getCurrencies(params?: { page?: number; pageSize?: number; filter?: string }) {
    const query = new URLSearchParams()
    if (params?.page) query.set('Page', params.page.toString())
    if (params?.pageSize) query.set('PageSize', params.pageSize.toString())
    if (params?.filter) query.set('Filter', params.filter)
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<any>(`/currencies${queryStr}`)
  },

  async getLanguages(params?: { page?: number; pageSize?: number; filter?: string }) {
    const query = new URLSearchParams()
    if (params?.page) query.set('Page', params.page.toString())
    if (params?.pageSize) query.set('PageSize', params.pageSize.toString())
    if (params?.filter) query.set('Filter', params.filter)
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<any>(`/languages${queryStr}`)
  },

  // ==================== File Upload ====================
  // Step 1: Get antiforgery token (required before upload)
  async getAntiforgeryToken(): Promise<string> {
    const token = localStorage.getItem('auth_token')
    const headers: Record<string, string> = {}
    if (token) headers['Authorization'] = `Bearer ${token}`

    const response = await fetch(`${API_BASE}/files/antiforgery`, {
      method: 'GET',
      headers,
    })

    if (!response.ok) {
      throw new ApiError(response.status, 'Не удалось получить токен безопасности')
    }

    const text = await response.text()
    return text.replace(/^"|"$/g, '')
  },

  // Step 2: Upload file with antiforgery token
  async uploadFile(file: File): Promise<string> {
    // First get antiforgery token
    const antiforgeryToken = await this.getAntiforgeryToken()

    const formData = new FormData()
    formData.append('file', file)

    const token = localStorage.getItem('auth_token')
    const headers: Record<string, string> = {
      'RequestVerificationToken': antiforgeryToken,
    }
    if (token) headers['Authorization'] = `Bearer ${token}`

    const response = await fetch(`${API_BASE}/files/upload`, {
      method: 'POST',
      headers,
      body: formData,
    })

    if (!response.ok) {
      const errorText = await response.text()
      throw new ApiError(response.status, parseErrorMessage(errorText, 'Ошибка загрузки файла'))
    }

    const text = await response.text()
    return text.replace(/^"|"$/g, '')
  },

  // Upload multiple files using bulk endpoint
  async uploadFilesBulk(files: File[]): Promise<string[]> {
    if (files.length === 0) return []

    // Get antiforgery token
    const antiforgeryToken = await this.getAntiforgeryToken()

    const formData = new FormData()
    // Use 'files' as the form field name for bulk upload
    for (const file of files) {
      formData.append('files', file)
    }

    const token = localStorage.getItem('auth_token')
    const headers: Record<string, string> = {
      'RequestVerificationToken': antiforgeryToken,
    }
    if (token) headers['Authorization'] = `Bearer ${token}`

    const response = await fetch(`${API_BASE}/files/upload-bulk`, {
      method: 'POST',
      headers,
      body: formData,
    })

    if (!response.ok) {
      const errorText = await response.text()
      throw new ApiError(response.status, parseErrorMessage(errorText, 'Ошибка загрузки файлов'))
    }

    const text = await response.text()
    try {
      // Returns array of encrypted keys
      return JSON.parse(text) as string[]
    } catch {
      // Single key returned as string
      return [text.replace(/^"|"$/g, '')]
    }
  },

  // Upload multiple files (uses bulk endpoint or falls back to sequential)
  async uploadFiles(files: File[]): Promise<string[]> {
    if (files.length === 0) return []

    // Try bulk upload first
    try {
      return await this.uploadFilesBulk(files)
    } catch (e) {
      console.warn('Bulk upload failed, falling back to sequential:', e)
      // Fallback: Upload files one by one
      const results: string[] = []
      for (const file of files) {
        const key = await this.uploadFile(file)
        results.push(key)
      }
      return results
    }
  },

  // Get the download URL (for reference, but images need auth)
  getFileDownloadUrl(key: string): string {
    // URL encode the key for safe URL usage
    const encodedKey = encodeURIComponent(key)
    return `${API_BASE}/files/download/${encodedKey}`
  },

  // Get a presigned URL for direct download from MinIO (faster, bypasses API)
  async getPresignedDownloadUrl(key: string): Promise<string> {
    const encodedKey = encodeURIComponent(key)
    const response = await request<string | { url: string }>(`/files/download-url?key=${encodedKey}`)
    // Handle both string and object response formats
    if (typeof response === 'string') {
      return response
    }
    return response.url
  },

  // Download file with authentication and return blob URL for display
  async downloadFile(key: string): Promise<string> {
    // Ensure valid token
    if (localStorage.getItem('auth_token')) {
      await ensureValidToken()
    }

    const token = localStorage.getItem('auth_token')
    const encodedKey = encodeURIComponent(key)

    const headers: Record<string, string> = {}
    if (token) {
      headers['Authorization'] = `Bearer ${token}`
    }

    const response = await fetch(`${API_BASE}/files/download/${encodedKey}`, {
      method: 'GET',
      headers,
    })

    if (!response.ok) {
      throw new ApiError(response.status, 'Ошибка загрузки файла')
    }

    const blob = await response.blob()
    return URL.createObjectURL(blob)
  },

  // Download multiple files and return blob URLs
  async downloadFiles(keys: string[]): Promise<string[]> {
    const results: string[] = []
    for (const key of keys) {
      try {
        const blobUrl = await this.downloadFile(key)
        results.push(blobUrl)
      } catch (error) {
        console.error('Error downloading file:', key, error)
        results.push('') // Push empty for failed downloads
      }
    }
    return results
  },

  // Legacy method for backward compatibility
  getFileUrl(key: string): string {
    return this.getFileDownloadUrl(key)
  },

  // ==================== Invitations ====================
  async getInvitations(params?: { filter?: string; page?: number; pageSize?: number }) {
    const query = new URLSearchParams()
    if (params?.filter) query.set('Filter', params.filter)
    if (params?.page) query.set('Page', params.page.toString())
    if (params?.pageSize) query.set('PageSize', params.pageSize.toString())
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<GetInvitationsResponse>(`/invitations${queryStr}`)
  },

  async getInvitationById(id: string): Promise<GetInvitationByIdResponse> {
    return request<GetInvitationByIdResponse>(`/invitations/${id}`)
  },

  async createInvitation(data: CreateInvitationCommand): Promise<string> {
    return request<string>('/invitations', {
      method: 'POST',
      body: JSON.stringify(data),
    })
  },

  async updateInvitation(data: UpdateInvitationCommand): Promise<string> {
    return request<string>('/invitations', {
      method: 'PUT',
      body: JSON.stringify(data),
    })
  },

  async deleteInvitation(id: string): Promise<string> {
    return request<string>(`/invitations/${id}`, {
      method: 'DELETE',
    })
  },

  async acceptInvitation(id: string): Promise<void> {
    return request<void>(`/invitations/accept/${id}`, {
      method: 'POST',
    })
  },

  async cancelInvitation(id: string): Promise<void> {
    return request<void>(`/invitations/cancel/${id}`, {
      method: 'POST',
    })
  },

  async rejectInvitation(data: RejectInvitationCommand): Promise<void> {
    return request<void>('/invitations/reject', {
      method: 'POST',
      body: JSON.stringify(data),
    })
  },

  // ==================== Listing Requests (Owner) ====================
  async getOwnerListingRequests(params?: { page?: number; pageSize?: number }) {
    const query = new URLSearchParams()
    if (params?.page) query.set('Page', params.page.toString())
    if (params?.pageSize) query.set('PageSize', params.pageSize.toString())
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<GetListingRequestsResponse>(`/listing-requests/owner${queryStr}`)
  },

  async acceptListingRequest(id: string): Promise<string> {
    return request<string>(`/listing-requests/${id}/accept`, {
      method: 'POST',
    })
  },

  async rejectListingRequest(id: string, reason: string): Promise<string> {
    return request<string>('/listing-requests/reject', {
      method: 'POST',
      body: JSON.stringify({ id, reason }),
    })
  },

  // ==================== Dashboard ====================
  async getDashboardStatistics(): Promise<DashboardStatistics> {
    return request<DashboardStatistics>('/dashboard/statistics')
  },

  // ==================== Marketplace (Client) ====================
  // Get listings grouped by category for main page
  async getMarketplaceMainPage(): Promise<MarketplaceCategoryGroup[]> {
    return request<MarketplaceCategoryGroup[]>('/listings/main-page')
  },

  // Get paginated listings with filters
  async getMarketplaceListings(params?: {
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
  }): Promise<GetMarketplaceListingsResponse> {
    const query = new URLSearchParams()
    if (params?.categoryId) query.set('CategoryId', params.categoryId)
    if (params?.regionId) query.set('RegionId', params.regionId)
    if (params?.districtId) query.set('DistrictId', params.districtId)
    if (params?.renovationId) query.set('RenovationId', params.renovationId)
    if (params?.roomsCount) query.set('RoomsCount', params.roomsCount.toString())
    if (params?.fromFloorNumber) query.set('FromFloorNumber', params.fromFloorNumber.toString())
    if (params?.toFloorNumber) query.set('ToFloorNumber', params.toFloorNumber.toString())
    if (params?.fromSquare) query.set('FromSquare', params.fromSquare.toString())
    if (params?.toSquare) query.set('ToSquare', params.toSquare.toString())
    if (params?.fromPrice) query.set('FromPrice', params.fromPrice.toString())
    if (params?.toPrice) query.set('ToPrice', params.toPrice.toString())
    if (params?.filter) query.set('Filter', params.filter)
    if (params?.page) query.set('Page', params.page.toString())
    if (params?.pageSize) query.set('PageSize', params.pageSize.toString())
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<GetMarketplaceListingsResponse>(`/listings${queryStr}`)
  },

  // ==================== Listing Requests (Client) ====================
  // Get client's own listing requests
  async getClientListingRequests(params?: { page?: number; pageSize?: number }): Promise<GetClientListingRequestsResponse> {
    const query = new URLSearchParams()
    if (params?.page) query.set('Page', params.page.toString())
    if (params?.pageSize) query.set('PageSize', params.pageSize.toString())
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<GetClientListingRequestsResponse>(`/listing-requests/client${queryStr}`)
  },

  // Create a new listing request (draft)
  async createListingRequest(data: CreateListingRequestCommand): Promise<string> {
    return request<string>('/listing-requests', {
      method: 'POST',
      body: JSON.stringify(data),
    })
  },

  // Send the listing request to owner
  async sendListingRequest(id: string): Promise<string> {
    return request<string>(`/listing-requests/${id}/send`, {
      method: 'POST',
    })
  },

  // Cancel a listing request
  async cancelListingRequest(id: string): Promise<string> {
    return request<string>(`/listing-requests/${id}/cancel`, {
      method: 'POST',
    })
  },

  // ==================== Meters & Communal ====================
  async getMeterTypes(): Promise<import('@/types/meter').MeterType[]> {
    return request<import('@/types/meter').MeterType[]>('/meter-types')
  },

  async getMeters(realEstateId: string): Promise<import('@/types/meter').Meter[]> {
    const query = new URLSearchParams()
    query.set('RealEstateId', realEstateId)
    return request<import('@/types/meter').Meter[]>(`/meters?${query.toString()}`)
  },

  async getMeterById(id: string): Promise<import('@/types/meter').Meter> {
    return request<import('@/types/meter').Meter>(`/meters/${id}`)
  },

  async createMeter(data: import('@/types/meter').CreateMeterCommand): Promise<string> {
    return request<string>('/meters', {
      method: 'POST',
      body: JSON.stringify(data),
    })
  },

  async updateMeter(data: import('@/types/meter').UpdateMeterCommand): Promise<string> {
    return request<string>('/meters', {
      method: 'PUT',
      body: JSON.stringify(data),
    })
  },

  async deleteMeter(id: string): Promise<string> {
    return request<string>(`/meters/${id}`, {
      method: 'DELETE',
    })
  },

  async getMeterReadings(params: { realEstateId: string; meterId?: string; meterTypeId?: string; page?: number; pageSize?: number }): Promise<import('@/types/meter').PagedMeterReadings> {
    const query = new URLSearchParams()
    query.set('RealEstateId', params.realEstateId)
    if (params.meterId) query.set('MeterId', params.meterId)
    if (params.meterTypeId) query.set('MeterTypeId', params.meterTypeId)
    if (params.page) query.set('Page', params.page.toString())
    if (params.pageSize) query.set('PageSize', params.pageSize.toString())
    return request<import('@/types/meter').PagedMeterReadings>(`/meter-readings?${query.toString()}`)
  },

  async upsertMeterReading(data: import('@/types/meter').UpsertMeterReadingCommand): Promise<string> {
    return request<string>('/meter-readings', {
      method: 'POST',
      body: JSON.stringify(data),
    })
  },

  async getMeterTariffs(params?: { page?: number; pageSize?: number }): Promise<import('@/types/meter').PagedMeterTariffs> {
    const query = new URLSearchParams()
    if (params?.page) query.set('Page', params.page.toString())
    if (params?.pageSize) query.set('PageSize', params.pageSize.toString())
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<import('@/types/meter').PagedMeterTariffs>(`/meter-tariffs${queryStr}`)
  },

  // ==================== Contract Templates ====================

  async getContractTemplates(type?: string): Promise<import('@/types/contractTemplate').ContractTemplate[]> {
    const query = type ? `?Type=${type}` : ''
    return request<import('@/types/contractTemplate').ContractTemplate[]>(`/contract-templates${query}`)
  },

  async getContractTemplate(id: string): Promise<import('@/types/contractTemplate').ContractTemplateDetail> {
    return request<import('@/types/contractTemplate').ContractTemplateDetail>(`/contract-templates/${id}`)
  },

  async createContractTemplate(data: import('@/types/contractTemplate').CreateTemplateData): Promise<string> {
    return request<string>('/contract-templates', {
      method: 'POST',
      body: JSON.stringify(data),
    })
  },

  async updateContractTemplate(id: string, data: import('@/types/contractTemplate').UpdateTemplateData): Promise<void> {
    return request<void>(`/contract-templates/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    })
  },

  // ==================== Contract Documents ====================

  async saveDocumentFromEditor(contractId: string, htmlContent: string): Promise<string> {
    return request<string>(`/contract-documents/${contractId}/from-editor`, {
      method: 'POST',
      body: JSON.stringify({ htmlContent }),
    })
  },

  async saveDocumentFromTemplate(contractId: string, templateId: string, templateVariablesJson: string): Promise<string> {
    return request<string>(`/contract-documents/${contractId}/from-template`, {
      method: 'POST',
      body: JSON.stringify({ templateId, templateVariablesJson }),
    })
  },

  async approveDocument(contractId: string, notes?: string): Promise<void> {
    return request<void>(`/contract-documents/${contractId}/approve`, {
      method: 'POST',
      body: JSON.stringify({ notes }),
    })
  },

  async rejectDocument(contractId: string, notes: string): Promise<void> {
    return request<void>(`/contract-documents/${contractId}/reject`, {
      method: 'POST',
      body: JSON.stringify({ notes }),
    })
  },

  // ==================== Contracts ====================

  async getMyContracts(type?: string, status?: string): Promise<import('@/types/contract').ContractListItem[]> {
    const query = new URLSearchParams()
    if (type) query.set('Type', type)
    if (status) query.set('Status', status)
    const queryStr = query.toString() ? `?${query.toString()}` : ''
    return request<import('@/types/contract').ContractListItem[]>(`/contracts/my${queryStr}`)
  },

  async getContractById(id: string): Promise<import('@/types/contract').ContractDetail> {
    return request<import('@/types/contract').ContractDetail>(`/contracts/${id}`)
  },

  async createContract(data: import('@/types/contract').CreateContractData): Promise<string> {
    return request<string>('/contracts', {
      method: 'POST',
      body: JSON.stringify(data),
    })
  },

  async signContract(id: string): Promise<void> {
    return request<void>(`/contracts/${id}/sign`, {
      method: 'POST',
    })
  },

  async cancelContract(id: string): Promise<void> {
    return request<void>(`/contracts/${id}/cancel`, {
      method: 'POST',
    })
  },

  async syncContractStatus(id: string): Promise<import('@/types/contract').SyncStatusResponse> {
    return request<import('@/types/contract').SyncStatusResponse>(`/contracts/${id}/sync-status`, {
      method: 'POST',
    })
  },


  // ==================== Didox ====================

  async getDidoxStatus(): Promise<import('@/types/contract').DidoxStatus> {
    return request<import('@/types/contract').DidoxStatus>('/didox-settings/status')
  },

  async connectDidox(login: string, password: string): Promise<void> {
    return request<void>('/didox-settings/connect', {
      method: 'POST',
      body: JSON.stringify({ login, password }),
    })
  },

  async disconnectDidox(): Promise<void> {
    return request<void>('/didox-settings/disconnect', {
      method: 'POST',
    })
  },
}

export { ApiError }
export type { OtpResponse }

