export interface ContractTemplate {
    id: string
    name: string
    type: string
    variablesJson: string
    isSystem: boolean
    createdAt: string
}

export interface ContractTemplateDetail extends ContractTemplate {
    htmlContent: string
}

export interface TemplateVariable {
    name: string
    label: string
    type: 'text' | 'number' | 'date'
    required: boolean
}

export interface CreateTemplateData {
    name: string
    type: number
    htmlContent: string
    variablesJson: string
}

export interface UpdateTemplateData {
    name: string
    htmlContent: string
    variablesJson: string
}

export const CONTRACT_TYPES = [
    { value: 0, label: 'Delegation', key: 'Delegation' },
    { value: 1, label: 'Lease', key: 'Lease' },
    { value: 2, label: 'Handover', key: 'Handover' },
    { value: 3, label: 'Communal Bill', key: 'CommunalBill' },
    { value: 4, label: 'Service Act', key: 'ServiceAct' },
    { value: 5, label: 'Reconciliation', key: 'Reconciliation' },
] as const
