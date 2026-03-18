// Listing Request types

export enum ListingRequestStatus {
    Sent = 0,
    Received = 1,
    Accepted = 2,
    Canceled = 3,
    Rejected = 4
}

export interface ListingRequest {
    id: string
    clientId: string
    client: string
    clientPhone?: string
    clientCompany?: string
    isVerified: boolean
    clientPhoto?: string
    name: string
    content: string
    status: ListingRequestStatus
}

export interface GetListingRequestsResponse {
    items: ListingRequest[]
    page: number
    pageSize: number
    totalCount: number
    hasNextPage: boolean
    hasPreviousPage: boolean
}

export function getListingRequestStatusLabel(status: ListingRequestStatus): string {
    switch (status) {
        case ListingRequestStatus.Sent:
            return 'Sent'
        case ListingRequestStatus.Received:
            return 'Received'
        case ListingRequestStatus.Accepted:
            return 'Accepted'
        case ListingRequestStatus.Canceled:
            return 'Canceled'
        case ListingRequestStatus.Rejected:
            return 'Rejected'
        default:
            return 'Unknown'
    }
}

export function getListingRequestStatusColor(status: ListingRequestStatus): string {
    switch (status) {
        case ListingRequestStatus.Sent:
            return 'status-sent'
        case ListingRequestStatus.Received:
            return 'status-received'
        case ListingRequestStatus.Accepted:
            return 'status-accepted'
        case ListingRequestStatus.Canceled:
            return 'status-canceled'
        case ListingRequestStatus.Rejected:
            return 'status-rejected'
        default:
            return 'status-unknown'
    }
}
