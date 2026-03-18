// Property Type Flow Configurations for Airbnb-Style Wizard
// Each property type has a tailored flow based on its characteristics

import type { RealEstateType } from '@/types/realestate'

export interface StepConfig {
    id: string
    title: string
    description: string
}

export interface PropertyTypeConfig {
    id: string
    name: string
    icon: string      // icon URL from API (Lucide SVG)
    description: string
    showBuildingSuggestion: boolean
    showFloorSuggestion: boolean
    canHaveUnits: boolean
    canHaveMeters: boolean
    steps: string[]
    requiredFields: string[]
    optionalFields: string[]
}

// Step definitions
export const STEP_DEFINITIONS: Record<string, StepConfig> = {
    type: {
        id: 'type',
        title: 'Property Type',
        description: 'What type of property are you adding?'
    },
    basic: {
        id: 'basic',
        title: 'Basic Info',
        description: 'Tell us about your property'
    },
    location: {
        id: 'location',
        title: 'Location',
        description: 'Where is your property located?'
    },
    building: {
        id: 'building',
        title: 'Building',
        description: 'Select your building and floor'
    },
    rooms: {
        id: 'rooms',
        title: 'Rooms',
        description: 'Add rooms to your property'
    },
    units: {
        id: 'units',
        title: 'Units',
        description: 'Add units to your property'
    },
    photos: {
        id: 'photos',
        title: 'Photos',
        description: 'Add photos of your property'
    },
    amenities: {
        id: 'amenities',
        title: 'Amenities',
        description: 'Select available amenities'
    }
}

// Step & field rules keyed by type name (case-insensitive matching)
// These define the wizard flow per type name. If a name isn't found, a sensible default is used.
const FLOW_BY_NAME: Record<string, { steps: string[]; requiredFields: string[]; optionalFields: string[] }> = {
    'apartment': {
        steps: ['type', 'basic', 'location', 'rooms', 'amenities', 'photos'],
        requiredFields: ['totalArea'],
        optionalFields: ['livingArea', 'roomsCount', 'renovationId', 'ceilingHeight', 'floorNumber']
    },
    'house': {
        steps: ['type', 'basic', 'location', 'rooms', 'amenities', 'photos'],
        requiredFields: ['totalArea'],
        optionalFields: ['livingArea', 'roomsCount', 'renovationId', 'ceilingHeight', 'totalFloors']
    },
    'townhouse': {
        steps: ['type', 'basic', 'location', 'rooms', 'amenities', 'photos'],
        requiredFields: ['totalArea'],
        optionalFields: ['livingArea', 'roomsCount', 'renovationId', 'ceilingHeight']
    },
    'office': {
        steps: ['type', 'basic', 'building', 'amenities', 'photos'],
        requiredFields: ['totalArea'],
        optionalFields: ['renovationId', 'ceilingHeight', 'cadastralNumber']
    },
    'commercial space': {
        steps: ['type', 'basic', 'building', 'amenities', 'photos'],
        requiredFields: ['totalArea'],
        optionalFields: ['renovationId', 'ceilingHeight', 'cadastralNumber']
    },
    'land plot': {
        steps: ['type', 'basic', 'location', 'amenities', 'photos'],
        requiredFields: ['totalArea'],
        optionalFields: ['cadastralNumber']
    },
    'warehouse': {
        steps: ['type', 'basic', 'location', 'amenities', 'photos'],
        requiredFields: ['totalArea'],
        optionalFields: ['ceilingHeight', 'cadastralNumber']
    },
    'garage': {
        steps: ['type', 'basic', 'location', 'photos'],
        requiredFields: ['totalArea'],
        optionalFields: ['cadastralNumber']
    }
}

const DEFAULT_FLOW = {
    steps: ['type', 'basic', 'location', 'amenities', 'photos'],
    requiredFields: ['totalArea'],
    optionalFields: ['renovationId', 'ceilingHeight', 'cadastralNumber']
}

/**
 * Build a PropertyTypeConfig from an API RealEstateType response.
 * Merges server data (id, name, icon, flags) with local step/field definitions.
 */
export function buildPropertyTypeConfig(apiType: RealEstateType): PropertyTypeConfig {
    const flow = FLOW_BY_NAME[apiType.typeName.toLowerCase()] || DEFAULT_FLOW
    return {
        id: apiType.id,
        name: apiType.name,
        icon: apiType.icon || '',
        description: apiType.description || '',
        showBuildingSuggestion: apiType.showBuildingSuggestion ?? false,
        showFloorSuggestion: apiType.showFloorSuggestion ?? false,
        canHaveUnits: apiType.canHaveUnits ?? false,
        canHaveMeters: apiType.canHaveMeters ?? false,
        steps: flow.steps,
        requiredFields: flow.requiredFields,
        optionalFields: flow.optionalFields
    }
}

/**
 * Build all configs from an API response array.
 */
export function buildPropertyTypeConfigs(apiTypes: RealEstateType[]): PropertyTypeConfig[] {
    return apiTypes.map(buildPropertyTypeConfig)
}

// Keep a mutable cache so it can be populated from the API at runtime
let _cachedConfigs: PropertyTypeConfig[] = []

/** Call once with the API data to populate the runtime cache. */
export function setPropertyTypeConfigs(configs: PropertyTypeConfig[]) {
    _cachedConfigs = configs
}

/** Get the cached configs (populated by setPropertyTypeConfigs). */
export function getPropertyTypeConfigs(): PropertyTypeConfig[] {
    return _cachedConfigs
}

// Helper to get config by ID
export function getPropertyTypeConfig(id: string): PropertyTypeConfig | undefined {
    return _cachedConfigs.find(config => config.id === id)
}

// Helper to get steps for a property type
export function getStepsForType(typeId: string): StepConfig[] {
    const config = getPropertyTypeConfig(typeId)
    if (!config) return []

    return config.steps
        .map(stepId => STEP_DEFINITIONS[stepId])
        .filter(Boolean) as StepConfig[]
}

// Check if a field should be shown for a property type
export function shouldShowField(typeId: string, fieldName: string): boolean {
    const config = getPropertyTypeConfig(typeId)
    if (!config) return false

    return config.requiredFields.includes(fieldName) ||
        config.optionalFields.includes(fieldName)
}

// Check if a field is required for a property type
export function isFieldRequired(typeId: string, fieldName: string): boolean {
    const config = getPropertyTypeConfig(typeId)
    if (!config) return false

    return config.requiredFields.includes(fieldName)
}
