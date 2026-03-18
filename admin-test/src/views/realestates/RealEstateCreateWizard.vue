<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { api } from '@/services/api'
import {
  getStepsForType,
  getPropertyTypeConfig,
  buildPropertyTypeConfigs,
  setPropertyTypeConfigs,
  type PropertyTypeConfig,
  type StepConfig
} from '@/configs/propertyTypeFlows'
import type { Renovation, RoomType } from '@/types/realestate'

// Step Components
import PropertyTypeSelector from './steps/PropertyTypeSelector.vue'
import BasicInfoStep from './steps/BasicInfoStep.vue'
import LocationStep from './steps/LocationStep.vue'
import RoomsUnitsStep from './steps/RoomsUnitsStep.vue'
import MediaStep from './steps/MediaStep.vue'
import AmenitiesStep from './steps/AmenitiesStep.vue'

const router = useRouter()
const route = useRoute()

// Edit mode
const isEditMode = computed(() => route.params.id !== undefined)
const realEstateId = computed(() => route.params.id as string)

// Wizard state
const currentStepIndex = ref(0)
const selectedPropertyType = ref<PropertyTypeConfig | null>(null)
const loading = ref(false)
const saving = ref(false)
const error = ref('')

// Reference data
const renovations = ref<Renovation[]>([])
const roomTypes = ref<RoomType[]>([])

// Form data
const formData = ref({
  // Basic info
  number: undefined as string | undefined,
  totalArea: 0,
  livingArea: undefined as number | undefined,
  roomsCount: undefined as number | undefined,
  renovationId: undefined as string | undefined,
  ceilingHeight: undefined as number | undefined,
  totalFloors: undefined as number | undefined,
  aboveFloors: undefined as number | undefined,
  belowFloors: undefined as number | undefined,
  cadastralNumber: undefined as string | undefined,

  // Location
  buildingId: undefined as string | undefined,
  floorId: undefined as string | undefined,
  floorNumber: undefined as number | undefined,
  regionId: undefined as string | undefined,
  districtId: undefined as string | undefined,
  address: undefined as string | undefined,
  latitude: undefined as number | undefined,
  longitude: undefined as number | undefined,
  buildingNumber: undefined as string | undefined,

  // Rooms & Units
  rooms: [] as { roomTypeId: string; area: number }[],
  units: [] as {
    totalArea: number
    floorNumber?: number
    renovationId?: string
    rooms: { roomTypeId: string; area: number }[]
    expanded: boolean
  }[],

  // Amenities
  amenityIds: [] as string[]
})

// Media state
const uploadedPlanKey = ref<string | null>(null)
const uploadedImageKeys = ref<string[]>([])
const existingImages = ref<{ id: string; image: string; url: string }[]>([])
const existingPlanUrl = ref<string | null>(null)

// Dynamic steps based on property type
const dynamicSteps = computed<StepConfig[]>(() => {
  if (!selectedPropertyType.value) {
    return [{ id: 'type', title: 'Property Type', description: 'Choose your property type' }]
  }
  return getStepsForType(selectedPropertyType.value.id)
})

const totalSteps = computed(() => dynamicSteps.value.length)
const currentStep = computed(() => dynamicSteps.value[currentStepIndex.value])
const isFirstStep = computed(() => currentStepIndex.value === 0)
const isLastStep = computed(() => currentStepIndex.value === totalSteps.value - 1)

// Validation per step
const canProceed = computed(() => {
  if (!currentStep.value) return false

  switch (currentStep.value.id) {
    case 'type':
      return !!selectedPropertyType.value
    case 'basic':
      return formData.value.totalArea > 0
    case 'location':
      return !!(formData.value.buildingId || formData.value.regionId)
    case 'building':
      return !!(formData.value.buildingId || formData.value.regionId)
    case 'rooms':
    case 'units':
    case 'photos':
    case 'amenities':
      return true // Optional steps
    default:
      return true
  }
})



// Load reference data
async function loadReferenceData() {
  try {
    const [renovationsRes, roomTypesRes, apiTypes] = await Promise.all([
      api.getRenovations({ pageSize: 50 }),
      api.getRoomTypes(),
      api.getRealEstateTypes()
    ])
    renovations.value = renovationsRes?.items || renovationsRes || []
    roomTypes.value = roomTypesRes?.items || roomTypesRes || []

    // Build and cache property type configs from API
    const configs = buildPropertyTypeConfigs(apiTypes)
    setPropertyTypeConfigs(configs)
  } catch (e) {
    console.error('Failed to load reference data:', e)
  }
}

// Load existing real estate for edit mode
async function loadRealEstate() {
  if (!isEditMode.value) return

  loading.value = true
  try {
    const data = await api.getRealEstateById(realEstateId.value)

    // Find and set the property type
    const typeConfig = getPropertyTypeConfig(data.realEstateTypeId)
    if (typeConfig) {
      selectedPropertyType.value = typeConfig
    }

    // Populate form data
    formData.value = {
      number: data.number,
      totalArea: data.totalArea || 0,
      livingArea: data.livingArea,
      roomsCount: data.roomsCount,
      renovationId: data.renovationId,
      ceilingHeight: data.ceilingHeight,
      totalFloors: data.totalFloors,
      aboveFloors: data.aboveFloors,
      belowFloors: data.belowFloors,
      cadastralNumber: data.cadastralNumber,
      buildingId: data.buildingId,
      floorId: data.floorId,
      floorNumber: data.floorNumber,
      regionId: data.regionId,
      districtId: data.districtId,
      address: data.address,
      latitude: data.latitude,
      longitude: data.longitude,
      buildingNumber: data.buildingNumber,
      rooms: data.rooms?.map((r: any) => ({ roomTypeId: r.roomTypeId, area: r.area || r.totalArea })) || [],
      units: data.units?.map((u: any) => ({
        totalArea: u.totalArea || 0,
        floorNumber: u.floorNumber,
        renovationId: u.renovationId,
        rooms: u.rooms?.map((r: any) => ({ roomTypeId: r.roomTypeId, area: r.area || r.totalArea })) || [],
        expanded: true
      })) || [],
      amenityIds: data.amenityIds || []
    }

    uploadedPlanKey.value = data.plan || null

    // Load existing images
    try {
      const imageRecords = await api.getRealEstateImages(realEstateId.value)
      for (const record of imageRecords) {
        try {
          const url = await api.downloadFile(record.image)
          existingImages.value.push({ id: record.id, image: record.image, url })
        } catch (e) {
          console.error('Failed to load image:', record.image, e)
        }
      }
    } catch (e) {
      console.error('Failed to fetch image records:', e)
    }

    // Load existing plan
    if (data.plan) {
      try {
        existingPlanUrl.value = await api.downloadFile(data.plan)
      } catch (e) {
        console.error('Failed to load plan:', e)
      }
    }

    // Skip to step 1 (basic info) since we already have the type
    currentStepIndex.value = 1
  } catch (e: any) {
    error.value = e.message || 'Error loading property'
  } finally {
    loading.value = false
  }
}

// Navigation
function onPropertyTypeSelect(config: PropertyTypeConfig) {
  selectedPropertyType.value = config
  // Auto-advance after selection
  setTimeout(() => nextStep(), 300)
}

function nextStep() {
  if (canProceed.value && currentStepIndex.value < totalSteps.value - 1) {
    currentStepIndex.value++
  }
}

function prevStep() {
  if (currentStepIndex.value > 0) {
    currentStepIndex.value--
  }
}

function goToStep(index: number) {
  if (index <= currentStepIndex.value) {
    currentStepIndex.value = index
  }
}

// Media handlers
function onPlanKeyUpdate(key: string | null) {
  uploadedPlanKey.value = key
}

function onImageKeysUpdate(keys: string[]) {
  uploadedImageKeys.value = keys
}

async function onDeleteExistingImage(id: string) {
  try {
    await api.deleteRealEstateImage(id)
    existingImages.value = existingImages.value.filter(img => img.id !== id)
  } catch (e) {
    console.error('Failed to delete image:', e)
  }
}

// Submit
async function handleSubmit() {
  if (!selectedPropertyType.value) return

  saving.value = true
  error.value = ''

  try {
    // Build request
    const request: any = {
      realEstateTypeId: selectedPropertyType.value.id,
      totalArea: formData.value.totalArea,
      renovationId: formData.value.renovationId,
      cadastralNumber: formData.value.cadastralNumber,
      number: formData.value.number,
      buildingId: formData.value.buildingId,
      floorId: formData.value.floorId,
      buildingNumber: formData.value.buildingNumber,
      floorNumber: formData.value.floorNumber,
      livingArea: formData.value.livingArea,
      ceilingHeight: formData.value.ceilingHeight,
      totalFloors: formData.value.totalFloors,
      aboveFloors: formData.value.aboveFloors,
      belowFloors: formData.value.belowFloors,
      roomsCount: formData.value.roomsCount,
      regionId: formData.value.regionId,
      districtId: formData.value.districtId,
      latitude: formData.value.latitude,
      longitude: formData.value.longitude,
      address: formData.value.address,
      plan: uploadedPlanKey.value,
      rooms: formData.value.rooms.filter(r => r.roomTypeId && r.area > 0),
      units: formData.value.units.filter(u => u.totalArea > 0).map(u => ({
        renovationId: u.renovationId,
        totalArea: u.totalArea,
        floorNumber: u.floorNumber,
        rooms: u.rooms.filter(r => r.roomTypeId && r.area > 0)
      })),
      amenityIds: formData.value.amenityIds.length > 0 ? formData.value.amenityIds : undefined
    }

    // Clean up undefined/empty values
    Object.keys(request).forEach(key => {
      if (request[key] === undefined || request[key] === '' ||
          (Array.isArray(request[key]) && request[key].length === 0)) {
        delete request[key]
      }
    })

    let savedRealEstateId: string

    if (isEditMode.value) {
      request.id = realEstateId.value
      await api.updateRealEstate(request)
      savedRealEstateId = realEstateId.value
    } else {
      savedRealEstateId = await api.createRealEstate(request)
    }

    // Associate new images
    if (uploadedImageKeys.value.length > 0) {
      try {
        await api.uploadRealEstateImages(savedRealEstateId, uploadedImageKeys.value)
      } catch (e) {
        console.error('Failed to associate images:', e)
      }
    }

    router.push('/realestates')
  } catch (e: any) {
    error.value = e.message || 'Error saving property'
  } finally {
    saving.value = false
  }
}

// Lifecycle
onMounted(async () => {
  loading.value = true
  await loadReferenceData()
  if (isEditMode.value) {
    await loadRealEstate()
  }
  loading.value = false
})
</script>

<template>
  <div class="wizard-view">
    <!-- Header -->
    <div class="wizard-header">
      <button class="btn-back" @click="router.back()">
        <svg width="20" height="20" viewBox="0 0 14 14" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M8.75 3.5L5.25 7L8.75 10.5" stroke="#1B1B1B" stroke-width="1.67" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </button>
      <h1>{{ isEditMode ? 'Edit Property' : 'Add Property' }}</h1>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Loading...</p>
    </div>

    <div v-else class="wizard-container">
      <!-- Step Indicator (Pill Tabs) -->
      <div v-if="selectedPropertyType" class="step-indicator">
        <div class="step-tabs">
          <button
            v-for="(step, index) in dynamicSteps"
            :key="step.id"
            class="step-tab"
            :class="{
              active: index === currentStepIndex,
              completed: index < currentStepIndex,
              clickable: index <= currentStepIndex
            }"
            @click="goToStep(index)"
          >
            {{ step.title }}
          </button>
        </div>
      </div>

      <!-- Error -->
      <div v-if="error" class="error-message">
        {{ error }}
        <button class="dismiss-btn" @click="error = ''">×</button>
      </div>

      <!-- Step Content -->
      <div class="step-content">
        <!-- Property Type Selection -->
        <PropertyTypeSelector
          v-if="currentStep?.id === 'type'"
          :selected-type-id="selectedPropertyType?.id"
          @select="onPropertyTypeSelect"
        />

        <!-- Basic Info -->
        <BasicInfoStep
          v-else-if="currentStep?.id === 'basic' && selectedPropertyType"
          :property-type="selectedPropertyType"
          v-model="formData"
          :renovations="renovations"
        />

        <!-- Location / Building -->
        <LocationStep
          v-else-if="(currentStep?.id === 'location' || currentStep?.id === 'building') && selectedPropertyType"
          :property-type="selectedPropertyType"
          v-model="formData"
        />

        <!-- Rooms -->
        <RoomsUnitsStep
          v-else-if="(currentStep?.id === 'rooms' || currentStep?.id === 'units') && selectedPropertyType"
          :property-type="selectedPropertyType"
          v-model="formData"
          :room-types="roomTypes"
          :renovations="renovations"
        />

        <!-- Photos -->
        <MediaStep
          v-else-if="currentStep?.id === 'photos' && selectedPropertyType"
          :property-type="selectedPropertyType"
          :existing-plan-url="existingPlanUrl"
          :existing-images="existingImages"
          @update:plan-key="onPlanKeyUpdate"
          @update:image-keys="onImageKeysUpdate"
          @delete:existing-image="onDeleteExistingImage"
        />

        <!-- Amenities -->
        <AmenitiesStep
          v-else-if="currentStep?.id === 'amenities' && selectedPropertyType"
          v-model="formData.amenityIds"
        />
      </div>

      <!-- Navigation -->
      <div class="wizard-navigation">
        <button
          v-if="!isFirstStep"
          class="btn btn-secondary"
          @click="prevStep"
        >
          ← Back
        </button>
        <div class="spacer"></div>
        <button
          v-if="!isLastStep"
          class="btn btn-primary"
          :disabled="!canProceed"
          @click="nextStep"
        >
          Continue →
        </button>
        <button
          v-if="isLastStep"
          class="btn btn-primary btn-submit"
          :disabled="saving || !canProceed"
          @click="handleSubmit"
        >
          <span v-if="saving" class="btn-spinner"></span>
          {{ saving ? 'Saving...' : (isEditMode ? 'Save Changes' : 'Create Property') }}
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.wizard-view {
  min-height: 100vh;
  background: #FFFFFF;
  padding: 24px;
}

/* Header */
.wizard-header {
  max-width: 900px;
  margin: 0 auto 24px;
  display: flex;
  align-items: center;
  gap: 16px;
}

.btn-back {
  display: flex;
  width: 44px;
  height: 44px;
  padding: 12px 15px;
  justify-content: center;
  align-items: center;
  background: rgba(27, 27, 27, 0.04);
  border: none;
  border-radius: 100px;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-back:hover {
  background: rgba(27, 27, 27, 0.08);
}

.wizard-header h1 {
  font-size: 24px;
  font-weight: 700;
  color: #1B1B1B;
  margin: 0;
}

/* Loading */
.loading-state {
  text-align: center;
  padding: 80px 24px;
}

.spinner {
  width: 48px;
  height: 48px;
  border: 4px solid #e5e7eb;
  border-top-color: #FF5B3C;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 16px;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

/* Container */
.wizard-container {
  max-width: 900px;
  margin: 0 auto;
  padding: 0 32px;
}

/* Step Indicator - Pill Tabs */
.step-indicator {
  display: flex;
  justify-content: center;
  margin-bottom: 32px;
}

.step-tabs {
  display: flex;
  padding: 4px;
  background: #F6F6F6;
  border-radius: 100px;
  gap: 0;
}

.step-tab {
  padding: 12px 20px;
  font-size: 14px;
  font-weight: 500;
  color: #19191C;
  background: transparent;
  border: none;
  border-radius: 100px;
  cursor: default;
  transition: all 0.2s ease;
  white-space: nowrap;
}

.step-tab.clickable {
  cursor: pointer;
}

.step-tab.clickable:hover:not(.active) {
  background: rgba(25, 25, 28, 0.05);
}

.step-tab.active {
  background: #19191C;
  color: #FFFFFF;
  font-weight: 600;
}

.step-tab.completed {
  color: #19191C;
}

/* Error */
.error-message {
  display: flex;
  align-items: center;
  justify-content: space-between;
  background: #fef2f2;
  color: #dc2626;
  padding: 12px 16px;
  border-radius: 10px;
  margin-bottom: 24px;
  font-size: 14px;
}

.dismiss-btn {
  background: none;
  border: none;
  color: #dc2626;
  font-size: 20px;
  cursor: pointer;
  padding: 0 4px;
}

/* Content */
.step-content {
  min-height: 400px;
}

/* Navigation */
.wizard-navigation {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-top: 32px;
  padding-top: 24px;
  border-top: 1px solid #e5e7eb;
}

.spacer {
  flex: 1;
}

.btn {
  padding: 12px 24px;
  border-radius: 100px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  border: none;
  display: inline-flex;
  align-items: center;
  gap: 6px;
}

.btn-secondary {
  background: #F7F7F7;
  color: #1B1B1B;
}

.btn-secondary:hover {
  background: #EBEBEB;
}

.btn-primary {
  background: #1B1B1B;
  color: #fff;
}

.btn-primary:hover:not(:disabled) {
  background: #333;
}

.btn-primary:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-submit {
  padding: 12px 32px;
}

.btn-spinner {
  width: 16px;
  height: 16px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top-color: #fff;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

/* Responsive */
@media (max-width: 600px) {
  .wizard-view {
    padding: 16px;
  }

  .wizard-container {
    padding: 20px;
    border-radius: 16px;
  }

  .step-dots {
    overflow-x: auto;
    gap: 16px;
    justify-content: flex-start;
    padding-bottom: 8px;
  }

  .step-label {
    display: none;
  }

  .wizard-navigation {
    flex-wrap: wrap;
  }

  .btn {
    flex: 1;
    min-width: 120px;
    justify-content: center;
  }
}
</style>
