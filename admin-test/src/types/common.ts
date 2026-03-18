// Common types for reference data from new 16.txt API

export interface PagedList<T> {
    page: number
    pageSize: number
    totalCount: number
    hasNextPage: boolean
    hasPreviousPage: boolean
    items: T[]
}

export interface Region {
    id: string
    name: string
    order: number
}

export interface District {
    id: string
    name: string
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
    order: number
}

export interface Language {
    id: string
    name: string
    shortCode: string
    order: number
}

// Multi-language text support
export interface LanguageValue {
    language: string // 'uz', 'ru', 'en'
    value: string
}

// Pagination query params
export interface PaginationParams {
    page?: number
    pageSize?: number
    filter?: string
}
