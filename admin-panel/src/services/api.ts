import axios, { AxiosError } from 'axios'
import type {
  PagedList,
  User,
  LoginRequest,
  Region,
  District,
  Bank,
  Currency,
  Language,
  SystemUser,
  Complex,
  Building,
  Floor,
  Amenity,
  AmenityCategory,
  RoomType,
  MeterType,
  MeterTariff,
  Category,
  RealEstateType,
  ListingCategory,
  Renovation,
  LandCategory,
  ProductionType,
  ModerationListing,
  ModerationRealEstate,
  ModerationRealEstateUnit,
  ModerationListingDetail,
  ModerationRealEstateDetail,
  ModerationUnitDetail,
  ModerationFilterParams,
  PaginationParams,
} from '../types'

const API_VERSION = 'v1'

const api = axios.create({
  baseURL: `/api/${API_VERSION}`,
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json',
  },
})

api.interceptors.response.use(
  response => response,
  (error: AxiosError) => {
    if (error.response?.status === 401 && !window.location.pathname.includes('/login')) {
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

// Auth Service
export const authService = {
  login: async (credentials: LoginRequest): Promise<void> => {
    await api.post('/authentication/phone-number', credentials)
  },

  logout: (): void => {
    document.cookie = 'mtoken=; Max-Age=0; path=/'
  },

  refreshToken: async (): Promise<void> => {
    await api.post('/authentication/refresh-token')
  },

  getProfile: async (): Promise<User> => {
    const response = await api.get<User>('/users/profile')
    return response.data
  },
}

// Generic CRUD service factory
function createCrudService<T, CreateDto = Partial<T>, UpdateDto = Partial<T>>(endpoint: string) {
  return {
    getAll: async (params?: PaginationParams): Promise<PagedList<T>> => {
      const apiParams = params ? {
        page: params.pageNumber,
        pageSize: params.pageSize,
      } : undefined
      const response = await api.get<PagedList<T>>(endpoint, { params: apiParams })
      return response.data
    },

    getById: async (id: string): Promise<T> => {
      const response = await api.get<T>(`${endpoint}/${id}`)
      return response.data
    },

    create: async (data: CreateDto): Promise<string> => {
      const response = await api.post<string>(endpoint, data)
      return response.data
    },

    update: async (data: UpdateDto): Promise<string> => {
      const response = await api.put<string>(endpoint, data)
      return response.data
    },

    remove: async (id: string): Promise<void> => {
      await api.delete(`${endpoint}/${id}`)
    },
  }
}

// Common Module Services
export const regionsService = createCrudService<Region>('/regions')
export const districtsService = {
  ...createCrudService<District>('/districts'),
  getAll: async (params?: PaginationParams & { regionId?: string }): Promise<PagedList<District>> => {
    const apiParams: Record<string, unknown> = {}
    if (params) {
      apiParams.Page = params.pageNumber
      apiParams.PageSize = params.pageSize
      if (params.regionId) apiParams.RegionId = params.regionId
    }
    const response = await api.get<PagedList<District>>('/districts', { params: apiParams })
    return response.data
  },
}
export const banksService = createCrudService<Bank>('/banks')
export const currenciesService = createCrudService<Currency>('/currencies')
export const languagesService = createCrudService<Language>('/languages')

// Identity Module Services
export const usersService = {
  getAll: async (params?: PaginationParams & { filter?: string }): Promise<PagedList<SystemUser>> => {
    const apiParams = params ? {
      page: params.pageNumber,
      pageSize: params.pageSize,
      filter: params.filter,
    } : undefined
    const response = await api.get<PagedList<SystemUser>>('/users', { params: apiParams })
    return response.data
  },

  getById: async (id: string): Promise<SystemUser> => {
    const response = await api.get<SystemUser>(`/users/${id}`)
    return response.data
  },
}

// Building Module Services
export const complexesService = createCrudService<Complex>('/complexes')
export const buildingsService = createCrudService<Building>('/buildings')

// Floors require buildingId for listing
export const floorsService = {
  getAll: async (params?: PaginationParams & { buildingId?: string }): Promise<PagedList<Floor>> => {
    if (!params?.buildingId) {
      return {
        items: [],
        pageNumber: 1,
        totalPages: 0,
        totalCount: 0,
        hasPreviousPage: false,
        hasNextPage: false,
      }
    }
    const apiParams = {
      buildingId: params.buildingId,
      page: params.pageNumber,
      pageSize: params.pageSize,
    }
    const response = await api.get<PagedList<Floor>>('/floors', { params: apiParams })
    return response.data
  },

  getById: async (id: string): Promise<Floor> => {
    const response = await api.get<Floor>(`/floors/${id}`)
    return response.data
  },

  create: async (data: Partial<Floor>): Promise<string> => {
    const response = await api.post<string>('/floors', data)
    return response.data
  },

  update: async (data: Partial<Floor>): Promise<string> => {
    const response = await api.put<string>('/floors', data)
    return response.data
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/floors/${id}`)
  },
}

export const amenitiesService = createCrudService<Amenity>('/amenities')
export const amenityCategoriesService = createCrudService<AmenityCategory>('/amenity-categories')
export const roomTypesService = createCrudService<RoomType>('/room-types')
export const meterTypesService = {
  getAll: async (params?: PaginationParams): Promise<PagedList<MeterType>> => {
    const response = await api.get<any>('/meter-types', { params: { Page: params?.pageNumber, PageSize: params?.pageSize } })
    const data = response.data
    // Handle both paginated response and raw array
    if (data && data.items) {
      return data
    }
    const items = Array.isArray(data) ? data : []
    return {
      items,
      pageNumber: params?.pageNumber || 1,
      totalPages: 1,
      totalCount: items.length,
      hasPreviousPage: false,
      hasNextPage: false,
    }
  },

  getById: async (id: string): Promise<MeterType> => {
    const response = await api.get<MeterType>(`/meter-types/${id}`)
    return response.data
  },

  create: async (data: Partial<MeterType>): Promise<string> => {
    const response = await api.post<string>('/meter-types', data)
    return response.data
  },

  update: async (data: Partial<MeterType>): Promise<string> => {
    const response = await api.put<string>('/meter-types', data)
    return response.data
  },

  remove: async (id: string): Promise<void> => {
    await api.delete(`/meter-types/${id}`)
  },
}
export const meterTariffsService = {
  getAll: async (params?: PaginationParams): Promise<PagedList<MeterTariff>> => {
    const response = await api.get<any>('/meter-tariffs', { params: { Page: params?.pageNumber, PageSize: params?.pageSize } })
    const data = response.data
    // Handle both paginated response and raw array
    if (data && data.items) {
      return data
    }
    const items = Array.isArray(data) ? data : []
    return {
      items,
      pageNumber: params?.pageNumber || 1,
      totalPages: 1,
      totalCount: items.length,
      hasPreviousPage: false,
      hasNextPage: false,
    }
  },

  getById: async (id: string): Promise<MeterTariff> => {
    const response = await api.get<MeterTariff>(`/meter-tariffs/${id}`)
    return response.data
  },

  create: async (data: Partial<MeterTariff>): Promise<string> => {
    const response = await api.post<string>('/meter-tariffs', data)
    return response.data
  },

  update: async (data: Partial<MeterTariff>): Promise<string> => {
    const response = await api.put<string>('/meter-tariffs', data)
    return response.data
  },

  remove: async (id: string): Promise<void> => {
    await api.delete(`/meter-tariffs/${id}`)
  },
}
export const categoriesService = createCrudService<Category>('/categories')

// RealEstateTypes returns IEnumerable (not paginated)
export const realEstateTypesService = {
  getAll: async (params?: PaginationParams): Promise<PagedList<RealEstateType>> => {
    const response = await api.get<RealEstateType[]>('/real-estate-types')
    const items = response.data || []
    return {
      items,
      pageNumber: params?.pageNumber || 1,
      totalPages: 1,
      totalCount: items.length,
      hasPreviousPage: false,
      hasNextPage: false,
    }
  },

  getById: async (id: string): Promise<RealEstateType> => {
    const response = await api.get<RealEstateType>(`/real-estate-types/${id}`)
    return response.data
  },

  create: async (data: Partial<RealEstateType>): Promise<string> => {
    const response = await api.post<string>('/real-estate-types', data)
    return response.data
  },

  update: async (data: Partial<RealEstateType>): Promise<string> => {
    const response = await api.put<string>('/real-estate-types', data)
    return response.data
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/real-estate-types/${id}`)
  },
}

export const listingCategoriesService = createCrudService<ListingCategory>('/listing-categories')
export const renovationsService = createCrudService<Renovation>('/renovations')
export const landCategoriesService = {
  ...createCrudService<LandCategory>('/land-categories'),

  getById: async (id: string): Promise<LandCategory> => {
    const response = await api.get<{ id: string; translates: Array<{ languageId: string; languageShortCode: string; value: string }> }>(`/land-categories/${id}`)
    const item = response.data
    const nameEn = item.translates?.find(t => t.languageShortCode === 'en')?.value || ''
    const nameUz = item.translates?.find(t => t.languageShortCode === 'uz')?.value || ''
    const nameRu = item.translates?.find(t => t.languageShortCode === 'ru')?.value || ''
    return { id: item.id, name: nameEn, nameEn, nameUz, nameRu } as LandCategory
  },

  create: async (data: Partial<LandCategory>): Promise<string> => {
    const langsRes = await api.get('/languages')
    const langs: Language[] = langsRes.data?.items || langsRes.data || []
    const translates = buildTranslates(data, langs)
    const response = await api.post<string>('/land-categories', { translates })
    return response.data
  },

  update: async (data: Partial<LandCategory>): Promise<string> => {
    const langsRes = await api.get('/languages')
    const langs: Language[] = langsRes.data?.items || langsRes.data || []
    const translates = buildTranslates(data, langs)
    const response = await api.put<string>('/land-categories', { id: data.id, translates })
    return response.data
  },
}

export const productionTypesService = {
  ...createCrudService<ProductionType>('/production-types'),

  getById: async (id: string): Promise<ProductionType> => {
    const response = await api.get<{ id: string; translates: Array<{ languageId: string; languageShortCode: string; value: string }> }>(`/production-types/${id}`)
    const item = response.data
    const nameEn = item.translates?.find(t => t.languageShortCode === 'en')?.value || ''
    const nameUz = item.translates?.find(t => t.languageShortCode === 'uz')?.value || ''
    const nameRu = item.translates?.find(t => t.languageShortCode === 'ru')?.value || ''
    return { id: item.id, name: nameEn, nameEn, nameUz, nameRu } as ProductionType
  },

  create: async (data: Partial<ProductionType>): Promise<string> => {
    const langsRes = await api.get('/languages')
    const langs: Language[] = langsRes.data?.items || langsRes.data || []
    const translates = buildTranslates(data, langs)
    const response = await api.post<string>('/production-types', { translates })
    return response.data
  },

  update: async (data: Partial<ProductionType>): Promise<string> => {
    const langsRes = await api.get('/languages')
    const langs: Language[] = langsRes.data?.items || langsRes.data || []
    const translates = buildTranslates(data, langs)
    const response = await api.put<string>('/production-types', { id: data.id, translates })
    return response.data
  },
}

// Helper to build translates array from nameEn/nameUz/nameRu fields
function buildTranslates(data: Partial<LandCategory> | Partial<ProductionType>, langs: Language[]) {
  const langMap: Record<string, string | undefined> = {
    en: data.nameEn,
    uz: data.nameUz,
    ru: data.nameRu,
  }
  return langs
    .filter(l => langMap[l.shortCode])
    .map(l => ({ languageId: l.id, languageShortCode: l.shortCode, value: langMap[l.shortCode]! }))
}

// ── Moderation Services ──
export const moderationService = {
  // Listings
  getListings: async (params?: ModerationFilterParams): Promise<PagedList<ModerationListing>> => {
    const apiParams: Record<string, unknown> = {}
    if (params) {
      apiParams.page = params.pageNumber
      apiParams.pageSize = params.pageSize
      if (params.filter) apiParams.filter = params.filter
    }
    const response = await api.get<PagedList<ModerationListing>>('/moderations/listings', { params: apiParams })
    return response.data
  },

  getListingById: async (id: string): Promise<ModerationListingDetail> => {
    const response = await api.get<ModerationListingDetail>(`/moderations/listings/${id}`)
    return response.data
  },

  acceptListing: async (id: string): Promise<string> => {
    const response = await api.post<string>('/moderations/listings/accept', { id })
    return response.data
  },

  rejectListing: async (id: string, reason?: string): Promise<string> => {
    const response = await api.post<string>('/moderations/listings/reject', { id, reason })
    return response.data
  },

  blockListing: async (id: string, reason?: string): Promise<string> => {
    const response = await api.post<string>('/moderations/listings/block', { id, reason })
    return response.data
  },

  // Real Estates
  getRealEstates: async (params?: ModerationFilterParams): Promise<PagedList<ModerationRealEstate>> => {
    const apiParams: Record<string, unknown> = {}
    if (params) {
      apiParams.page = params.pageNumber
      apiParams.pageSize = params.pageSize
      if (params.filter) apiParams.filter = params.filter
    }
    const response = await api.get<PagedList<ModerationRealEstate>>('/moderations/realestates', { params: apiParams })
    return response.data
  },

  getRealEstateById: async (id: string): Promise<ModerationRealEstateDetail> => {
    const response = await api.get<ModerationRealEstateDetail>(`/moderations/realestates/${id}`)
    return response.data
  },

  acceptRealEstate: async (id: string): Promise<string> => {
    const response = await api.post<string>('/moderations/realestates/accept', { id })
    return response.data
  },

  rejectRealEstate: async (id: string, reason?: string): Promise<string> => {
    const response = await api.post<string>('/moderations/realestates/reject', { id, reason })
    return response.data
  },

  blockRealEstate: async (id: string, reason?: string): Promise<string> => {
    const response = await api.post<string>('/moderations/realestates/block', { id, reason })
    return response.data
  },

  // Real Estate Units
  getUnits: async (params?: ModerationFilterParams): Promise<PagedList<ModerationRealEstateUnit>> => {
    const apiParams: Record<string, unknown> = {}
    if (params) {
      apiParams.page = params.pageNumber
      apiParams.pageSize = params.pageSize
      if (params.filter) apiParams.filter = params.filter
    }
    const response = await api.get<PagedList<ModerationRealEstateUnit>>('/moderations/realestateunits', { params: apiParams })
    return response.data
  },

  getUnitById: async (id: string): Promise<ModerationUnitDetail> => {
    const response = await api.get<ModerationUnitDetail>(`/moderations/realestateunits/${id}`)
    return response.data
  },

  acceptUnit: async (id: string): Promise<string> => {
    const response = await api.post<string>('/moderations/realestateunits/accept', { id })
    return response.data
  },

  rejectUnit: async (id: string, reason?: string): Promise<string> => {
    const response = await api.post<string>('/moderations/realestateunits/reject', { id, reason })
    return response.data
  },

  blockUnit: async (id: string, reason?: string): Promise<string> => {
    const response = await api.post<string>('/moderations/realestateunits/block', { id, reason })
    return response.data
  },
}

// Files Service (upload/download with antiforgery token)
export const filesService = {
  getAntiforgeryToken: async (): Promise<string> => {
    const response = await api.get<string>('/files/antiforgery')
    return typeof response.data === 'string' ? response.data.replace(/^"|"$/g, '') : response.data
  },

  uploadFile: async (file: File): Promise<string> => {
    const antiforgeryToken = await filesService.getAntiforgeryToken()
    const formData = new FormData()
    formData.append('file', file)
    const response = await api.post<string>('/files/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
        'RequestVerificationToken': antiforgeryToken,
      },
    })
    return typeof response.data === 'string' ? response.data.replace(/^"|"$/g, '') : response.data
  },

  uploadFilesBulk: async (files: File[]): Promise<string[]> => {
    if (files.length === 0) return []
    const antiforgeryToken = await filesService.getAntiforgeryToken()
    const formData = new FormData()
    for (const file of files) {
      formData.append('files', file)
    }
    const response = await api.post<string[] | string>('/files/upload-bulk', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
        'RequestVerificationToken': antiforgeryToken,
      },
    })
    if (Array.isArray(response.data)) return response.data
    return [typeof response.data === 'string' ? response.data.replace(/^"|"$/g, '') : response.data]
  },

  uploadFiles: async (files: File[]): Promise<string[]> => {
    if (files.length === 0) return []
    try {
      return await filesService.uploadFilesBulk(files)
    } catch {
      // Fallback: sequential
      const results: string[] = []
      for (const file of files) {
        const key = await filesService.uploadFile(file)
        results.push(key)
      }
      return results
    }
  },

  getDownloadUrl: (key: string): string => {
    const encodedKey = encodeURIComponent(key)
    return `/api/v1/files/download/${encodedKey}`
  },

  getPresignedDownloadUrl: async (key: string): Promise<string> => {
    const encodedKey = encodeURIComponent(key)
    const response = await api.get<string | { url: string }>(`/files/download-url?key=${encodedKey}`)
    if (typeof response.data === 'string') return response.data
    return response.data.url
  },

  downloadFile: async (key: string): Promise<string> => {
    const encodedKey = encodeURIComponent(key)
    const response = await api.get(`/files/download/${encodedKey}`, { responseType: 'blob' })
    return URL.createObjectURL(response.data)
  },
}

export default api
