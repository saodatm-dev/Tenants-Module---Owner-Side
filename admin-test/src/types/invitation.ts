// Invitation Types for the admin dashboard
// Aligned with backend: Identity.Domain.Invitations

// Base invitation entity from API responses
export interface Invitation {
  id: string
  phoneNumber: string
  fullName: string
  roleName: string
  expiredTime: string  // ISO DateTime string
  status: InvitationStatus
  reason?: string | null
}

// Invitation status enum - matches Identity.Domain.Invitations.InvitationStatus
export enum InvitationStatus {
  Sent = 0,
  Received = 1,
  Accepted = 2,
  Canceled = 3,
  Rejected = 4
}

// Paged list response
export interface GetInvitationsResponse {
  items: Invitation[]
  page: number
  pageSize: number
  totalCount: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

// Single invitation detail response - same fields as list item
export type GetInvitationByIdResponse = Invitation

// Create invitation command - matches CreateInvitationCommand.cs
export interface CreateInvitationCommand {
  phoneNumber: string
  roleId: string  // GUID
}

// Update invitation command - matches UpdateInvitationCommand.cs
export interface UpdateInvitationCommand {
  id: string
  phoneNumber: string
}

// Reject invitation command - matches RejectInvitationCommand.cs
export interface RejectInvitationCommand {
  id: string
  reason?: string | null
}

// Query parameters for listing invitations
export interface InvitationsListParams {
  filter?: string
  page?: number
  pageSize?: number
}

// Helper to get status label
export function getInvitationStatusLabel(status: InvitationStatus): string {
  switch (status) {
    case InvitationStatus.Sent:
      return 'Sent'
    case InvitationStatus.Received:
      return 'Received'
    case InvitationStatus.Accepted:
      return 'Accepted'
    case InvitationStatus.Canceled:
      return 'Canceled'
    case InvitationStatus.Rejected:
      return 'Rejected'
    default:
      return 'Unknown'
  }
}

// Helper to get status color class
export function getInvitationStatusColor(status: InvitationStatus): string {
  switch (status) {
    case InvitationStatus.Sent:
      return 'status-sent'
    case InvitationStatus.Received:
      return 'status-received'
    case InvitationStatus.Accepted:
      return 'status-accepted'
    case InvitationStatus.Canceled:
      return 'status-canceled'
    case InvitationStatus.Rejected:
      return 'status-rejected'
    default:
      return 'status-unknown'
  }
}
