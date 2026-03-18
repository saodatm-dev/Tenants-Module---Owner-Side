export interface ContractListItem {
    id: string
    type: string
    contractNumber: string
    contractDate: string
    status: string
    documentName?: string
    didoxDocumentId?: string
    partiesCount: number
    signedCount: number
    createdAt: string
}

export interface ContractDetail extends ContractListItem {
    initiatorId: string
    didoxDocumentUrl?: string
    parties: ContractParty[]
}

export interface ContractParty {
    id: string
    userId: string
    tin: string
    role: string
    name: string
    address?: string
    isSigned: boolean
    signedAt?: string
}

export interface CreateContractData {
    type: number
    contractNumber: string
    contractDate: string
    documentName?: string
    parties: CreatePartyData[]
}

export interface CreatePartyData {
    userId: string
    tin: string
    role: number
    name: string
    address?: string
}

export interface DidoxStatus {
    isConnected: boolean
    login?: string
}

export interface SyncStatusResponse {
    status: string
    didoxStatus: string
    isSigned: boolean
}

export const CONTRACT_STATUSES = [
    { value: 'Draft', label: 'Draft', color: 'gray' },
    { value: 'PendingSigning', label: 'Pending', color: 'amber' },
    { value: 'PartiallySigned', label: 'Partial', color: 'blue' },
    { value: 'FullySigned', label: 'Signed', color: 'green' },
    { value: 'Cancelled', label: 'Cancelled', color: 'red' },
] as const

export const CONTRACT_PARTY_ROLES = [
    { value: 0, label: 'Owner', key: 'Owner' },
    { value: 1, label: 'Agent', key: 'Agent' },
    { value: 2, label: 'Tenant', key: 'Tenant' },
] as const
