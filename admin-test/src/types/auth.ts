// Authentication Types

export interface AuthenticationResponse {
  token: string
  tokenExpiredTime: string
  refreshToken: string
  refreshTokenExpiredTime: string
  accountType: number  // 0 = Individual, 1 = Professional, etc.
}

// JWT claims structure (matches backend IExecutionContextProvider)
export interface SessionClaims {
  userId: string        // user_id
  tenantId: string      // tenant_id
  accountId: string     // account_id
  sessionId: string     // session_id
  roleId: string        // role_id
  accountType: string   // account_type
  isIndividual: boolean // is_individual
  isOwner: boolean      // is_owner (Owner = property owner, Client = renter/searcher)
  exp: number
}

export interface PhoneVerifyResponse {
  key: string
  message?: string
}

export interface EImzoKeyItem {
  disk: string
  path: string
  name: string
  alias: string
  serialNumber: string
  validFrom: Date
  validTo: Date
  CN: string
  TIN: string
  UID: string
  PINFL: string
  O: string  // Organization name
  T: string  // Title/Position
  type: string
}

export interface UserInfo {
  userId: string
  fullName?: string
  firstName?: string
  lastName?: string
  phoneNumber?: string
  tin?: string
  pinfl?: string
  isIndividual: boolean
  isOwner: boolean      // Owner = property owner, Client = renter/searcher
  accountType?: number  // 0 = Client, 1 = Owner
  companyName?: string
  tenantId: string
}

export type AuthMethod = 'phone' | 'eimzo' | 'oneid'

export interface LoginCredentials {
  phoneNumber: string
  password: string
}

export interface PhoneRegisterRequest {
  phoneNumber: string
  code: string
}

export interface PhoneConfirmRequest {
  key: string
  password: string
}

export interface EImzoAuthRequest {
  pkcs7: string
}

export interface OneIdAuthRequest {
  code: string
}

// Account switching
export interface Account {
  photo: string | null
  firstName: string | null
  lastName: string | null
  middleName: string | null
  companyName: string | null  // Company name for business accounts
  accountType: number  // 0 = Owner, 1 = Client
  isCurrent: boolean
  key: string  // URL-encoded key for switching
}
