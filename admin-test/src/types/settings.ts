// Settings page types

// User profile
export interface UserProfile {
    userId: string
    isIndividual: boolean
    phoneNumber: string
    firstName: string
    lastName: string
    middleName: string
    photo: string | null
    permissions: string[]
    companyName: string | null
    companyTin: string | null
}

// Profile update request
export interface UpdateProfileRequest {
    profilePicture: string | null
    firstName: string
    lastName: string
    middleName: string
}

// Company user
export interface CompanyUser {
    userId: string
    fullName: string
    phoneNumber: string
    photo: string | null
    roleName: string
    isOwner: boolean
    isActive: boolean
    joinedAt: string
}

// Paginated company users response
export interface CompanyUsersResponse {
    page: number
    pageSize: number
    totalCount: number
    hasNextPage: boolean
    hasPreviousPage: boolean
    items: CompanyUser[]
}
