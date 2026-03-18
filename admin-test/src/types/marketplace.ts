// Marketplace Types for Client-Side Listings

/**
 * Category group returned from /listings/main-page
 * Groups listings by their listing category
 */
export interface MarketplaceCategoryGroup {
    listingCategoryId: string
    listingCategoryName: string
    listings: MarketplaceListing[]
}

/**
 * Individual listing for marketplace display
 */
export interface MarketplaceListing {
    id: string
    ownerId: string
    title?: string
    categoryIds: string[]
    image: string
    building?: string
    complex?: string
    totalArea?: number
    floorsCount?: number
    description: string
    priceForMonth?: number
    pricePerSquareMeter?: number
    region?: string
    district?: string
    latitude?: number
    longitude?: number
    address?: string
}

/**
 * Full listing detail response from /listings/{id}
 */
export interface ListingDetail {
    id: string
    ownerId: string
    title?: string
    categories: string[]
    complex?: string
    building?: string
    floors?: ListingFloor[]
    roomsCount?: number
    description?: string
    amenities?: ListingAmenity[]
    plan?: string
    totalArea: number
    livingArea?: number
    ceilingHeight?: number
    priceForMonth?: number
    pricePerSquareMeter?: number
    region?: string
    district?: string
    latitude?: number
    longitude?: number
    address?: string
    status: string
    images?: string[]
}

export interface ListingFloor {
    id: string
    name: string
}

export interface ListingAmenity {
    iconUrl?: string
    name: string
}

/**
 * Paginated listings response from /listings
 */
export interface GetMarketplaceListingsResponse {
    items: MarketplaceListing[]
    page: number
    pageSize: number
    totalCount: number
    hasNextPage: boolean
    hasPreviousPage: boolean
}

/**
 * Client listing request (for client's "My Requests" view)
 */
export interface ClientListingRequest {
    id: string
    ownerId: string
    owner: string
    name: string
    content: string
    status: ClientListingRequestStatus
}

export enum ClientListingRequestStatus {
    Sent = 0,
    Received = 1,
    Accepted = 2,
    Canceled = 3,
    Rejected = 4
}

export interface GetClientListingRequestsResponse {
    items: ClientListingRequest[]
    page: number
    pageSize: number
    totalCount: number
    hasNextPage: boolean
    hasPreviousPage: boolean
}

// Helper functions
export function getClientRequestStatusLabel(status: ClientListingRequestStatus): string {
    switch (status) {
        case ClientListingRequestStatus.Sent: return 'Отправлено'
        case ClientListingRequestStatus.Received: return 'Получено'
        case ClientListingRequestStatus.Accepted: return 'Принято'
        case ClientListingRequestStatus.Canceled: return 'Отменено'
        case ClientListingRequestStatus.Rejected: return 'Отклонено'
        default: return 'Неизвестно'
    }
}

export function getClientRequestStatusColor(status: ClientListingRequestStatus): string {
    switch (status) {
        case ClientListingRequestStatus.Sent: return 'status-sent'
        case ClientListingRequestStatus.Received: return 'status-received'
        case ClientListingRequestStatus.Accepted: return 'status-accepted'
        case ClientListingRequestStatus.Canceled: return 'status-canceled'
        case ClientListingRequestStatus.Rejected: return 'status-rejected'
        default: return 'status-unknown'
    }
}

/**
 * Create listing request command
 */
export interface CreateListingRequestCommand {
    listingId: string
    content: string
}
