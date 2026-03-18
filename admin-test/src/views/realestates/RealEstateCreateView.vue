<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { api } from '@/services/api'
import { compressImage } from '@/utils/imageCompression'
import type {
  RealEstateType,
  Renovation,
  RoomType,
  RoomValue,
  CreateRealEstateRequest,
  RealEstateUnitRequest
} from '@/types/realestate'
import GoogleMap from '@/components/GoogleMap.vue'

const router = useRouter()
const route = useRoute()

// Edit mode
const isEditMode = computed(() => route.params.id !== undefined)
const realEstateId = computed(() => route.params.id as string)

// Wizard state
const currentStep = ref(1)
const totalSteps = 4
const loading = ref(false)
const saving = ref(false)
const error = ref('')

// Form data
const form = ref<CreateRealEstateRequest>({
  realEstateTypeId: '',
  totalArea: 0,
  renovationId: undefined,
  cadastralNumber: undefined,
  number: undefined,
  buildingId: undefined,
  floorId: undefined,
  buildingNumber: undefined,
  floorNumber: undefined,
  livingArea: undefined,
  ceilingHeight: undefined,
  totalFloors: undefined as number | undefined,
  aboveFloors: undefined as number | undefined,
  belowFloors: undefined as number | undefined,
  roomsCount: undefined,
  regionId: undefined,
  districtId: undefined,
  latitude: undefined,
  longitude: undefined,
  address: undefined,
  plan: undefined,
  images: [],
  rooms: []
})

// Reference data
const realEstateTypes = ref<RealEstateType[]>([])
const renovations = ref<Renovation[]>([])
const roomTypes = ref<RoomType[]>([])
const regions = ref<any[]>([])
const districts = ref<any[]>([])
const buildings = ref<any[]>([])
const floors = ref<any[]>([])

// Image handling
const imageFiles = ref<{ file: File; preview: string; uploading: boolean }[]>([])
const planFile = ref<{ file: File; preview: string; uploading: boolean } | null>(null)
const uploadedImageKeys = ref<string[]>([])
const uploadedPlanKey = ref<string | null>(null)
// For displaying existing images in edit mode (with id for deletion)
const existingImages = ref<{ id: string; image: string; url: string }[]>([])
const existingPlanUrl = ref<string | null>(null)

// Room management
const rooms = ref<{ roomTypeId: string; area: number }[]>([])

// Units management
type UnitFormData = {
  renovationId?: string
  totalArea: number
  floorNumber?: number
  livingArea?: number
  ceilingHeight?: number
  roomsCount?: number
  rooms: { roomTypeId: string; area: number }[]
  expanded: boolean // UI state for collapsible
}
const units = ref<UnitFormData[]>([])
const activeTab = ref<'rooms' | 'units'>('rooms')

// Validation
const stepValidation = computed(() => ({
  1: !!form.value.realEstateTypeId && form.value.totalArea > 0,
  2: !!(form.value.buildingId || form.value.regionId),
  3: true, // All optional
  4: true, // All optional
  5: true  // All optional
}))

const canProceed = computed(() => stepValidation.value[currentStep.value as keyof typeof stepValidation.value])

// Load reference data
async function loadReferenceData() {
  loading.value = true
  try {
    const [typesRes, renovationsRes, roomTypesRes, regionsRes, buildingsRes] = await Promise.all([
      api.getRealEstateTypes(),
      api.getRenovations({ pageSize: 50 }),
      api.getRoomTypes(),
      api.getRegions({ pageSize: 50 }),
      api.getBuildings({ pageSize: 100 })
    ])
    realEstateTypes.value = typesRes || []
    renovations.value = renovationsRes?.items || renovationsRes || []
    roomTypes.value = roomTypesRes?.items || roomTypesRes || []
    regions.value = regionsRes?.items || regionsRes || []
    buildings.value = buildingsRes?.items || buildingsRes || []
  } catch (e) {
    console.error('Failed to load reference data:', e)
    error.value = 'Error loading data'
  } finally {
    loading.value = false
  }
}

// Load districts when region changes
watch(() => form.value.regionId, async (regionId) => {
  if (regionId) {
    try {
      const res = await api.getDistricts({ regionId, pageSize: 50 })
      districts.value = res?.items || res || []
    } catch (e) {
      console.error('Failed to load districts:', e)
    }
  } else {
    districts.value = []
    form.value.districtId = undefined
  }
})

// Load floors when building changes
watch(() => form.value.buildingId, async (buildingId) => {
  if (buildingId) {
    try {
      const res = await api.getFloors({ buildingId, pageSize: 100 })
      floors.value = res?.items || res || []
    } catch (e) {
      console.error('Failed to load floors:', e)
    }
  } else {
    floors.value = []
    form.value.floorId = undefined
  }
})

// Load existing real estate for edit mode
async function loadRealEstate() {
  if (!isEditMode.value) return

  loading.value = true
  try {
    const data = await api.getRealEstateById(realEstateId.value)
    // Populate form
    form.value = {
      realEstateTypeId: data.realEstateTypeId || '',
      totalArea: data.totalArea || 0,
      renovationId: data.renovationId,
      cadastralNumber: data.cadastralNumber,
      number: data.number,
      buildingId: data.buildingId,
      floorId: data.floorId,
      buildingNumber: data.buildingNumber,
      floorNumber: data.floorNumber,
      livingArea: data.livingArea,
      ceilingHeight: data.ceilingHeight,
      totalFloors: data.totalFloors,
      aboveFloors: data.aboveFloors,
      belowFloors: data.belowFloors,
      roomsCount: data.roomsCount,
      regionId: data.regionId,
      districtId: data.districtId,
      latitude: data.latitude,
      longitude: data.longitude,
      address: data.address,
      plan: data.plan,
      images: data.images || [],
      rooms: data.rooms?.map((r: any) => ({ roomTypeId: r.roomTypeId, area: r.area || r.totalArea })) || []
    }
    rooms.value = form.value.rooms || []
    uploadedImageKeys.value = data.images || []
    uploadedPlanKey.value = data.plan || null

    // Load units data for edit mode
    if (data.units && Array.isArray(data.units)) {
      units.value = data.units.map((u: any) => ({
        renovationId: u.renovationId,
        totalArea: u.totalArea || 0,
        floorNumber: u.floorNumber,
        livingArea: u.livingArea,
        ceilingHeight: u.ceilingHeight,
        roomsCount: u.roomsCount,
        rooms: u.rooms?.map((r: any) => ({ roomTypeId: r.roomTypeId, area: r.area || r.totalArea })) || [],
        expanded: true // Expand all units by default when editing
      }))
      // Switch to units tab if there are units
      if (units.value.length > 0) {
        activeTab.value = 'units'
      }
    }

    // Load existing images for display using the realestateimages API
    try {
      const imageRecords = await api.getRealEstateImages(realEstateId.value)
      existingImages.value = []
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

    // Load existing plan for display
    if (data.plan) {
      try {
        existingPlanUrl.value = await api.downloadFile(data.plan)
      } catch (e) {
        console.error('Failed to load plan:', e)
      }
    }
  } catch (e: any) {
    error.value = e.message || 'Error loading property'
  } finally {
    loading.value = false
  }
}

// Navigation
function nextStep() {
  if (canProceed.value && currentStep.value < totalSteps) {
    currentStep.value++
  }
}

function prevStep() {
  if (currentStep.value > 1) {
    currentStep.value--
  }
}

function goToStep(step: number) {
  // Only allow going to completed steps or current step
  if (step <= currentStep.value) {
    currentStep.value = step
  }
}

// Image handling - uploads immediately when selected with compression
async function handleImageSelect(event: Event) {
  const input = event.target as HTMLInputElement
  if (!input.files) return

  const files = Array.from(input.files)
  input.value = ''

  for (const file of files) {
    const preview = URL.createObjectURL(file)
    const imageItem = { file, preview, uploading: true }
    imageFiles.value.push(imageItem)

    // Compress and upload immediately
    try {
      const compressedFile = await compressImage(file)
      const key = await api.uploadFile(compressedFile)
      uploadedImageKeys.value.push(key)
      imageItem.uploading = false
    } catch (e) {
      console.error('Failed to upload image:', e)
      // Remove failed upload from preview
      const index = imageFiles.value.indexOf(imageItem)
      if (index > -1) {
        URL.revokeObjectURL(imageItem.preview)
        imageFiles.value.splice(index, 1)
      }
    }
  }
}

async function handlePlanSelect(event: Event) {
  const input = event.target as HTMLInputElement
  const file = input.files?.[0]
  if (!file) return

  const preview = URL.createObjectURL(file)
  planFile.value = { file, preview, uploading: true }
  input.value = ''

  // Compress and upload immediately
  try {
    const compressedFile = await compressImage(file)
    const key = await api.uploadFile(compressedFile)
    uploadedPlanKey.value = key
    planFile.value.uploading = false
  } catch (e) {
    console.error('Failed to upload plan:', e)
    URL.revokeObjectURL(preview)
    planFile.value = null
  }
}

function removeImage(index: number) {
  const item = imageFiles.value[index]
  if (item) {
    URL.revokeObjectURL(item.preview)
  }
  imageFiles.value.splice(index, 1)
}

async function removeExistingImage(index: number) {
  const item = existingImages.value[index]
  if (item) {
    // Call the API to delete the image from the real estate
    try {
      await api.deleteRealEstateImage(item.id)
    } catch (e) {
      console.error('Failed to delete image:', e)
      // Continue with removal from UI even if API fails
    }
    // Remove from uploadedImageKeys so it won't be included in future uploads
    const keyIndex = uploadedImageKeys.value.indexOf(item.image)
    if (keyIndex > -1) {
      uploadedImageKeys.value.splice(keyIndex, 1)
    }
  }
  existingImages.value.splice(index, 1)
}

function removePlan() {
  if (planFile.value) {
    URL.revokeObjectURL(planFile.value.preview)
    planFile.value = null
  }
  existingPlanUrl.value = null
  uploadedPlanKey.value = null
}

// Room management
function addRoom() {
  rooms.value.push({ roomTypeId: '', area: 0 })
}

function removeRoom(index: number) {
  rooms.value.splice(index, 1)
}

// Unit management
function addUnit() {
  units.value.push({
    renovationId: undefined,
    totalArea: 0,
    floorNumber: undefined,
    livingArea: undefined,
    ceilingHeight: undefined,
    roomsCount: undefined,
    rooms: [],
    expanded: true
  })
}

function removeUnit(index: number) {
  units.value.splice(index, 1)
}

function toggleUnitExpanded(index: number) {
  const unit = units.value[index]
  if (unit) unit.expanded = !unit.expanded
}

function addRoomToUnit(unitIndex: number) {
  const unit = units.value[unitIndex]
  if (unit) {
    unit.rooms.push({ roomTypeId: '', area: 0 })
  }
}

function removeRoomFromUnit(unitIndex: number, roomIndex: number) {
  const unit = units.value[unitIndex]
  if (unit) {
    unit.rooms.splice(roomIndex, 1)
  }
}

// Submit
async function handleSubmit() {
  saving.value = true
  error.value = ''

  try {
    // Images are already uploaded when selected, use the uploaded keys directly
    const allImageKeys = [...uploadedImageKeys.value]

    // Plan is also already uploaded when selected, use the uploaded key
    const planKey = uploadedPlanKey.value

    // Prepare request - do NOT include images in the main request body
    // Images will be associated via the separate realestateimages endpoint
    const request: any = {
      ...form.value,
      plan: planKey,
      rooms: rooms.value.filter(r => r.roomTypeId && r.area > 0).map(r => ({ roomTypeId: r.roomTypeId, area: r.area })),
      // Include units if any
      units: units.value.filter(u => u.totalArea > 0).map(u => ({
        renovationId: u.renovationId,
        totalArea: u.totalArea,
        floorNumber: u.floorNumber,
        livingArea: u.livingArea,
        ceilingHeight: u.ceilingHeight,
        roomsCount: u.roomsCount,
        rooms: u.rooms.filter(r => r.roomTypeId && r.area > 0).map(r => ({ roomTypeId: r.roomTypeId, area: r.area }))
      }))
    }

    // Remove images from request - will be handled separately
    delete request.images

    // Clean up undefined values and empty arrays
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
      // Create returns the new ID
      savedRealEstateId = await api.createRealEstate(request)
    }

    // Now associate images with the real estate using the dedicated endpoint
    if (allImageKeys.length > 0) {
      try {
        await api.uploadRealEstateImages(savedRealEstateId, allImageKeys)
      } catch (e) {
        console.error('Failed to associate images with real estate:', e)
        // Don't fail the whole operation, the real estate was saved
        // Just log the error - images may need to be re-uploaded
      }
    }

    router.push('/realestates')
  } catch (e: any) {
    error.value = e.message || 'Error saving'
  } finally {
    saving.value = false
  }
}

// Map visibility toggle
const showMap = ref(false)

function toggleMap() {
  showMap.value = !showMap.value
}

// Handle location selection from map
function onLocationUpdate(location: { lat: number; lng: number; address?: string }) {
  form.value.latitude = location.lat
  form.value.longitude = location.lng
  // If address is provided from reverse geocoding, update the address field
  if (location.address) {
    form.value.address = location.address
  }
}

// Lifecycle
onMounted(async () => {
  await loadReferenceData()
  if (isEditMode.value) {
    await loadRealEstate()
  }
})
</script>

<template>
  <div class="realestate-create-view">
    <div class="page-header">
      <button class="btn-back" @click="router.back()">
        ← Back
      </button>
      <h1>{{ isEditMode ? 'Edit Property' : 'Add Real Estate' }}</h1>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Loading...</p>
    </div>

    <div v-else class="wizard-container">
      <!-- Step indicator -->
      <div class="step-indicator">
        <div
          v-for="step in totalSteps"
          :key="step"
          class="step-item"
          :class="{
            active: step === currentStep,
            completed: step < currentStep,
            clickable: step <= currentStep
          }"
          @click="goToStep(step)"
        >
          <div class="step-number">{{ step }}</div>
          <div class="step-label">
            {{ step === 1 ? 'Basic' :
               step === 2 ? 'Location' :
               step === 3 ? 'Rooms' : 'Media' }}
          </div>
        </div>
      </div>

      <!-- Error -->
      <div v-if="error" class="error-message">
        {{ error }}
      </div>

      <!-- Step content -->
      <div class="step-content">
        <!-- Step 1: Basic Info -->
        <div v-if="currentStep === 1" class="step-panel">
          <h2>Basic Information</h2>
          <p class="step-description">Specify the type and characteristics of the property</p>

          <div class="form-grid">
            <div class="form-group required">
              <label>Property Type *</label>
              <select v-model="form.realEstateTypeId">
                <option value="">Select type</option>
                <option v-for="type in realEstateTypes" :key="type.id" :value="type.id">
                  {{ type.name }}
                </option>
              </select>
            </div>

            <div class="form-group">
              <label>Cadastral Number</label>
              <input type="text" v-model="form.cadastralNumber" placeholder="Registration number" />
            </div>

            <div class="form-group required">
              <label>Total Area (m²) *</label>
              <input type="number" v-model.number="form.totalArea" min="0" step="0.1" />
            </div>

            <div class="form-group">
              <label>Renovation Level</label>
              <select v-model="form.renovationId">
                <option :value="undefined">Not specified</option>
                <option v-for="ren in renovations" :key="ren.id" :value="ren.id">
                  {{ ren.name }}
                </option>
              </select>
            </div>

            <div class="form-group">
              <label>Number of Rooms</label>
              <input type="number" v-model.number="form.roomsCount" min="0" />
            </div>

            <div class="form-group">
              <label>Living Area (m²)</label>
              <input type="number" v-model.number="form.livingArea" min="0" step="0.1" />
            </div>

            <div class="form-group">
              <label>Ceiling Height (m)</label>
              <input type="number" v-model.number="form.ceilingHeight" min="0" step="0.1" />
            </div>
          </div>
        </div>

        <!-- Step 2: Location -->
        <div v-if="currentStep === 2" class="step-panel">
          <h2>Location</h2>
          <p class="step-description">Select a building or specify an address</p>

          <div class="location-options">
            <div class="option-card" :class="{ selected: form.buildingId }">
              <h3>Select Building</h3>
              <p>From your buildings</p>

              <div class="form-group">
                <label>Building</label>
                <select v-model="form.buildingId">
                  <option :value="undefined">Not selected</option>
                  <option v-for="b in buildings" :key="b.id" :value="b.id">
                    {{ b.number || b.address || b.id }}
                  </option>
                </select>
              </div>

              <div v-if="form.buildingId && floors.length > 0" class="form-group">
                <label>Floor</label>
                <select v-model="form.floorId">
                  <option :value="undefined">Not selected</option>
                  <option v-for="f in floors" :key="f.id" :value="f.id">
                    Floor {{ f.number }}
                  </option>
                </select>
              </div>
            </div>

            <div class="divider">or</div>

            <div class="option-card" :class="{ selected: !form.buildingId && form.regionId }">
              <h3>Specify Address</h3>
              <p>Standalone property</p>

              <div class="form-group">
                <label>Region</label>
                <select v-model="form.regionId" :disabled="!!form.buildingId">
                  <option :value="undefined">Select region</option>
                  <option v-for="r in regions" :key="r.id" :value="r.id">
                    {{ r.name }}
                  </option>
                </select>
              </div>

              <div class="form-group">
                <label>District</label>
                <select v-model="form.districtId" :disabled="!form.regionId || !!form.buildingId">
                  <option :value="undefined">Select district</option>
                  <option v-for="d in districts" :key="d.id" :value="d.id">
                    {{ d.name }}
                  </option>
                </select>
              </div>

              <div class="form-group full-width">
                <label>Address</label>
                <div class="address-input-row">
                  <input type="text" v-model="form.address" placeholder="Street, house" :disabled="!!form.buildingId" />
                  <button
                    type="button"
                    class="btn-map-toggle"
                    :class="{ active: showMap }"
                    @click="toggleMap"
                    :disabled="!!form.buildingId"
                    title="Select on map"
                  >
                    📍
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Location Map (shown when toggle button is clicked) -->
          <div v-if="!form.buildingId && showMap" class="map-section mt-4">
            <h3>Select location on map</h3>
            <p class="map-description">Click on the map to select an address</p>
            <GoogleMap
              :latitude="form.latitude"
              :longitude="form.longitude"
              :editable="true"
              :enable-reverse-geocoding="true"
              height="300px"
              @update:location="onLocationUpdate"
            />
          </div>

          <div class="form-grid mt-4">
            <div class="form-group">
              <label>Floor Number</label>
              <input type="number" v-model.number="form.floorNumber" />
            </div>

            <div class="form-group">
              <label>Total Floors in Building</label>
              <input type="number" v-model.number="form.totalFloors" min="1" />
            </div>

            <div class="form-group">
              <label>Above Ground Floors</label>
              <input type="number" v-model.number="form.aboveFloors" min="0" />
            </div>

            <div class="form-group">
              <label>Below Ground Floors</label>
              <input type="number" v-model.number="form.belowFloors" min="0" />
            </div>

            <div class="form-group">
              <label>Building Number</label>
              <input type="text" v-model="form.buildingNumber" />
            </div>

            <div class="form-group">
              <label>Property Name / Number</label>
              <input type="text" v-model="form.number" placeholder="e.g.: My Apartment, Unit 4B" />
            </div>
          </div>
        </div>

        <!-- Step 4: Media -->
        <div v-if="currentStep === 4" class="step-panel">
          <h2>Photos and Floor Plan</h2>
          <p class="step-description">Add photos of the property and floor plan</p>

          <!-- Floor Plan -->
          <div class="media-section">
            <h3>Floor Plan</h3>
            <div class="upload-zone" @click="($refs.planInput as HTMLInputElement).click()">
              <input
                ref="planInput"
                type="file"
                accept="image/*"
                class="hidden-input"
                @change="handlePlanSelect"
              />
              <div v-if="!planFile && !uploadedPlanKey" class="upload-placeholder">
                <span class="icon">📋</span>
                <span>Upload floor plan</span>
              </div>
              <div v-else class="plan-preview">
                <img v-if="planFile" :src="planFile.preview" alt="Plan" />
                <img v-else-if="existingPlanUrl" :src="existingPlanUrl" alt="Plan" />
                <div v-else class="uploaded-indicator">Floor plan uploaded</div>
                <button class="remove-btn" @click.stop="removePlan">×</button>
              </div>
            </div>
          </div>

          <!-- Property Images -->
          <div class="media-section">
            <h3>Property Photos</h3>
            <div class="images-grid">
              <!-- Existing images from API -->
              <div
                v-for="(img, index) in existingImages"
                :key="'existing-' + index"
                class="image-item"
              >
                <img :src="img.url" alt="Photo" />
                <button class="remove-btn" @click="removeExistingImage(index)">×</button>
              </div>

              <!-- Newly selected images -->
              <div
                v-for="(img, index) in imageFiles"
                :key="'new-' + index"
                class="image-item"
              >
                <img :src="img.preview" alt="Photo" />
                <div v-if="img.uploading" class="uploading-overlay">
                  <div class="spinner small"></div>
                </div>
                <button class="remove-btn" @click="removeImage(index)">×</button>
              </div>

              <div class="upload-add" @click="($refs.imagesInput as HTMLInputElement).click()">
                <input
                  ref="imagesInput"
                  type="file"
                  accept="image/*"
                  multiple
                  class="hidden-input"
                  @change="handleImageSelect"
                />
                <span class="icon">+</span>
                <span>Add photo</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Step 3: Rooms & Units -->
        <div v-if="currentStep === 3" class="step-panel">
          <h2>Rooms and Units</h2>
          <p class="step-description">Add rooms or units (optional)</p>

          <!-- Tab Navigation -->
          <div class="tabs-nav">
            <button
              class="tab-btn"
              :class="{ active: activeTab === 'rooms' }"
              @click="activeTab = 'rooms'"
            >
              🏠 Rooms
            </button>
            <button
              class="tab-btn"
              :class="{ active: activeTab === 'units' }"
              @click="activeTab = 'units'"
            >
              🏢 Units
            </button>
          </div>

          <!-- Rooms Tab -->
          <div v-if="activeTab === 'rooms'" class="tab-content">
            <div class="rooms-list">
              <div v-for="(room, index) in rooms" :key="index" class="room-item">
                <div class="form-group">
                  <label>Room Type</label>
                  <select v-model="room.roomTypeId">
                    <option value="">Select type</option>
                    <option v-for="rt in roomTypes" :key="rt.id" :value="rt.id">
                      {{ rt.name }}
                    </option>
                  </select>
                </div>
                <div class="form-group">
                  <label>Area (m²)</label>
                  <input type="number" v-model.number="room.area" min="0" step="0.1" />
                </div>
                <button class="btn-remove" @click="removeRoom(index)">×</button>
              </div>
            </div>

            <button class="btn btn-secondary" @click="addRoom">
              + Add Room
            </button>
          </div>

          <!-- Units Tab -->
          <div v-if="activeTab === 'units'" class="tab-content">
            <div class="units-list">
              <div v-for="(unit, unitIndex) in units" :key="unitIndex" class="unit-card">
                <!-- Unit Header -->
                <div class="unit-header" @click="toggleUnitExpanded(unitIndex)">
                  <span class="unit-title">
                    <span class="expand-icon">{{ unit.expanded ? '▼' : '▶' }}</span>
                    Unit {{ unitIndex + 1 }}
                    <span v-if="unit.totalArea > 0" class="unit-badge">{{ unit.totalArea }} м²</span>
                  </span>
                  <button class="btn-remove" @click.stop="removeUnit(unitIndex)">×</button>
                </div>

                <!-- Unit Content (Collapsible) -->
                <div v-if="unit.expanded" class="unit-content">
                  <div class="form-grid">
                    <div class="form-group">
                      <label>Area (m²) *</label>
                      <input type="number" v-model.number="unit.totalArea" min="0" step="0.1" placeholder="0" />
                    </div>

                    <div class="form-group">
                      <label>Renovation</label>
                      <select v-model="unit.renovationId">
                        <option :value="undefined">Not specified</option>
                        <option v-for="r in renovations" :key="r.id" :value="r.id">
                          {{ r.name }}
                        </option>
                      </select>
                    </div>

                    <div class="form-group">
                      <label>Floor</label>
                      <input type="number" v-model.number="unit.floorNumber" min="0" placeholder="0" />
                    </div>

                    <div class="form-group">
                      <label>Living Area (m²)</label>
                      <input type="number" v-model.number="unit.livingArea" min="0" step="0.1" placeholder="0" />
                    </div>

                    <div class="form-group">
                      <label>Ceiling Height (m)</label>
                      <input type="number" v-model.number="unit.ceilingHeight" min="0" step="0.1" placeholder="0" />
                    </div>

                    <div class="form-group">
                      <label>Number of Rooms</label>
                      <input type="number" v-model.number="unit.roomsCount" min="0" placeholder="0" />
                    </div>
                  </div>

                  <!-- Unit Rooms -->
                  <div class="unit-rooms-section">
                    <h4>Unit Rooms</h4>
                    <div class="rooms-list">
                      <div v-for="(room, roomIndex) in unit.rooms" :key="roomIndex" class="room-item compact">
                        <div class="form-group">
                          <select v-model="room.roomTypeId">
                            <option value="">Room type</option>
                            <option v-for="rt in roomTypes" :key="rt.id" :value="rt.id">
                              {{ rt.name }}
                            </option>
                          </select>
                        </div>
                        <div class="form-group">
                          <input type="number" v-model.number="room.area" min="0" step="0.1" placeholder="м²" />
                        </div>
                        <button class="btn-remove-sm" @click="removeRoomFromUnit(unitIndex, roomIndex)">×</button>
                      </div>
                    </div>
                    <button class="btn btn-ghost btn-sm" @click="addRoomToUnit(unitIndex)">
                      + Add Room
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <button class="btn btn-secondary" @click="addUnit">
              + Add Unit
            </button>
          </div>
        </div>
      </div>

      <!-- Navigation buttons -->
      <div class="wizard-actions">
        <button
          v-if="currentStep > 1"
          class="btn btn-secondary"
          @click="prevStep"
        >
          ← Back
        </button>
        <div class="spacer"></div>
        <button
          v-if="currentStep < totalSteps"
          class="btn btn-primary"
          :disabled="!canProceed"
          @click="nextStep"
        >
          Next →
        </button>
        <button
          v-if="currentStep === totalSteps"
          class="btn btn-primary"
          :disabled="saving"
          @click="handleSubmit"
        >
          {{ saving ? 'Saving...' : (isEditMode ? 'Save' : 'Create Property') }}
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.realestate-create-view {
  padding: 24px;
  max-width: 900px;
  margin: 0 auto;
}

.page-header {
  margin-bottom: 24px;
}

.btn-back {
  background: none;
  border: none;
  color: #6b7280;
  font-size: 14px;
  cursor: pointer;
  padding: 8px 0;
  margin-bottom: 8px;
}

.btn-back:hover {
  color: #4f46e5;
}

.page-header h1 {
  font-size: 28px;
  font-weight: 700;
  color: #1a1a2e;
  margin: 0;
}

.loading-state {
  text-align: center;
  padding: 60px;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 3px solid #e5e7eb;
  border-top-color: #4f46e5;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 16px;
}

.spinner.small {
  width: 24px;
  height: 24px;
  border-width: 2px;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

/* Step Indicator */
.step-indicator {
  display: flex;
  justify-content: space-between;
  margin-bottom: 32px;
  position: relative;
}

.step-indicator::before {
  content: '';
  position: absolute;
  top: 20px;
  left: 40px;
  right: 40px;
  height: 2px;
  background: #e5e7eb;
  z-index: 0;
}

.step-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  z-index: 1;
  cursor: default;
}

.step-item.clickable {
  cursor: pointer;
}

.step-number {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: #e5e7eb;
  color: #6b7280;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  font-size: 14px;
  transition: all 0.3s;
}

.step-item.active .step-number {
  background: linear-gradient(135deg, #4f46e5, #7c3aed);
  color: white;
}

.step-item.completed .step-number {
  background: #10b981;
  color: white;
}

.step-label {
  margin-top: 8px;
  font-size: 12px;
  color: #6b7280;
  font-weight: 500;
}

.step-item.active .step-label {
  color: #4f46e5;
  font-weight: 600;
}

/* Wizard Container */
.wizard-container {
  background: white;
  border-radius: 16px;
  padding: 32px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.error-message {
  background: #fef2f2;
  color: #dc2626;
  padding: 12px 16px;
  border-radius: 8px;
  margin-bottom: 16px;
}

.step-content {
  min-height: 400px;
}

.step-panel h2 {
  font-size: 20px;
  font-weight: 600;
  color: #1a1a2e;
  margin: 0 0 4px 0;
}

.step-description {
  color: #6b7280;
  font-size: 14px;
  margin: 0 0 24px 0;
}

/* Form */
.form-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.form-group.full-width {
  grid-column: span 2;
}

.form-group label {
  font-size: 13px;
  font-weight: 600;
  color: #374151;
}

.form-group.required label::after {
  content: ' *';
  color: #dc2626;
}

.form-group input,
.form-group select {
  padding: 12px 14px;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  font-size: 14px;
  transition: all 0.2s;
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: #4f46e5;
  box-shadow: 0 0 0 3px rgba(79, 70, 229, 0.1);
}

.form-group input:disabled,
.form-group select:disabled {
  background: #f9fafb;
  color: #9ca3af;
}

/* Location Options */
.location-options {
  display: grid;
  grid-template-columns: 1fr auto 1fr;
  gap: 24px;
  align-items: start;
}

.option-card {
  padding: 24px;
  border: 2px solid #e5e7eb;
  border-radius: 12px;
  transition: all 0.2s;
}

.option-card.selected {
  border-color: #4f46e5;
  background: #f5f3ff;
}

.option-card h3 {
  font-size: 16px;
  font-weight: 600;
  margin: 0 0 4px 0;
}

.option-card > p {
  color: #6b7280;
  font-size: 13px;
  margin: 0 0 16px 0;
}

.divider {
  color: #9ca3af;
  font-size: 14px;
  padding-top: 40px;
}

.mt-4 {
  margin-top: 24px;
}

/* Map Section */
.map-section {
  padding: 20px;
  background: #f9fafb;
  border-radius: 12px;
  border: 1px solid #e5e7eb;
}

.map-section h3 {
  font-size: 15px;
  font-weight: 600;
  margin: 0 0 8px 0;
  color: #1a1a2e;
}

.map-description {
  font-size: 13px;
  color: #6b7280;
  margin: 0 0 16px 0;
}

/* Address Input with Map Button */
.address-input-row {
  display: flex;
  gap: 8px;
  align-items: center;
}

.address-input-row input {
  flex: 1;
}

.btn-map-toggle {
  width: 44px;
  height: 44px;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  background: white;
  font-size: 18px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
}

.btn-map-toggle:hover:not(:disabled) {
  border-color: #4f46e5;
  background: #f5f3ff;
}

.btn-map-toggle.active {
  border-color: #4f46e5;
  background: #eef2ff;
  box-shadow: 0 0 0 2px rgba(79, 70, 229, 0.2);
}

.btn-map-toggle:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Media */
.media-section {
  margin-bottom: 32px;
}

.media-section h3 {
  font-size: 16px;
  font-weight: 600;
  margin: 0 0 12px 0;
}

.hidden-input {
  display: none;
}

.upload-zone {
  border: 2px dashed #e5e7eb;
  border-radius: 12px;
  padding: 32px;
  text-align: center;
  cursor: pointer;
  transition: all 0.2s;
}

.upload-zone:hover {
  border-color: #4f46e5;
  background: #f5f3ff;
}

.upload-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  color: #6b7280;
}

.upload-placeholder .icon {
  font-size: 32px;
}

.plan-preview {
  position: relative;
  display: inline-block;
}

.plan-preview img {
  max-width: 200px;
  max-height: 150px;
  border-radius: 8px;
}

.images-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
  gap: 12px;
}

.image-item {
  position: relative;
  aspect-ratio: 1;
  border-radius: 8px;
  overflow: hidden;
}

.image-item img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.uploading-overlay {
  position: absolute;
  inset: 0;
  background: rgba(255, 255, 255, 0.8);
  display: flex;
  align-items: center;
  justify-content: center;
}

.remove-btn {
  position: absolute;
  top: 4px;
  right: 4px;
  width: 24px;
  height: 24px;
  border-radius: 50%;
  background: rgba(0, 0, 0, 0.6);
  color: white;
  border: none;
  cursor: pointer;
  font-size: 16px;
  line-height: 1;
}

.upload-add {
  aspect-ratio: 1;
  border: 2px dashed #e5e7eb;
  border-radius: 8px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 4px;
  cursor: pointer;
  color: #6b7280;
  font-size: 12px;
  transition: all 0.2s;
}

.upload-add:hover {
  border-color: #4f46e5;
  color: #4f46e5;
}

.upload-add .icon {
  font-size: 24px;
}

/* Rooms */
.rooms-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-bottom: 16px;
}

.room-item {
  display: flex;
  gap: 16px;
  align-items: flex-end;
  padding: 16px;
  background: #f9fafb;
  border-radius: 10px;
}

.room-item .form-group {
  flex: 1;
}

.btn-remove {
  width: 36px;
  height: 36px;
  border-radius: 8px;
  border: none;
  background: #fef2f2;
  color: #dc2626;
  cursor: pointer;
  font-size: 18px;
}

.btn-remove:hover {
  background: #dc2626;
  color: white;
}

/* Actions */
.wizard-actions {
  display: flex;
  align-items: center;
  margin-top: 32px;
  padding-top: 24px;
  border-top: 1px solid #e5e7eb;
}

.spacer {
  flex: 1;
}

.btn {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 12px 24px;
  border-radius: 10px;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  border: none;
  transition: all 0.2s;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-primary {
  background: linear-gradient(135deg, #4f46e5, #7c3aed);
  color: white;
}

.btn-primary:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(79, 70, 229, 0.4);
}

.btn-secondary {
  background: #f3f4f6;
  color: #374151;
}

.btn-secondary:hover:not(:disabled) {
  background: #e5e7eb;
}

/* Tabs Navigation */
.tabs-nav {
  display: flex;
  gap: 8px;
  margin-bottom: 24px;
  padding: 4px;
  background: #f3f4f6;
  border-radius: 12px;
}

.tab-btn {
  flex: 1;
  padding: 12px 20px;
  border: none;
  background: transparent;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 500;
  color: #6b7280;
  cursor: pointer;
  transition: all 0.2s;
}

.tab-btn:hover {
  color: #374151;
}

.tab-btn.active {
  background: white;
  color: #4f46e5;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.tab-content {
  animation: fadeIn 0.2s ease;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(4px); }
  to { opacity: 1; transform: translateY(0); }
}

/* Rooms List */
.rooms-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-bottom: 16px;
}

.room-item {
  display: flex;
  gap: 12px;
  align-items: flex-end;
  padding: 16px;
  background: #f9fafb;
  border-radius: 10px;
  border: 1px solid #e5e7eb;
}

.room-item .form-group {
  flex: 1;
  margin: 0;
}

.room-item .form-group label {
  font-size: 12px;
  margin-bottom: 4px;
}

.room-item.compact {
  padding: 12px;
}

.room-item.compact .form-group label {
  display: none;
}

/* Units List */
.units-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
  margin-bottom: 16px;
}

.unit-card {
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  overflow: hidden;
  background: white;
}

.unit-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px;
  background: #f9fafb;
  cursor: pointer;
  user-select: none;
  transition: background 0.2s;
}

.unit-header:hover {
  background: #f3f4f6;
}

.unit-title {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 600;
  font-size: 14px;
  color: #374151;
}

.expand-icon {
  font-size: 10px;
  color: #9ca3af;
  width: 16px;
}

.unit-badge {
  background: #e0e7ff;
  color: #4f46e5;
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.unit-content {
  padding: 20px;
  border-top: 1px solid #e5e7eb;
  animation: slideDown 0.2s ease;
}

@keyframes slideDown {
  from { opacity: 0; max-height: 0; }
  to { opacity: 1; max-height: 1000px; }
}

.unit-rooms-section {
  margin-top: 20px;
  padding-top: 16px;
  border-top: 1px solid #e5e7eb;
}

.unit-rooms-section h4 {
  font-size: 13px;
  font-weight: 600;
  color: #6b7280;
  margin: 0 0 12px 0;
}

/* Remove Buttons */
.btn-remove {
  width: 32px;
  height: 32px;
  border: none;
  background: #fee2e2;
  color: #dc2626;
  border-radius: 6px;
  font-size: 16px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
  flex-shrink: 0;
}

.btn-remove:hover {
  background: #fecaca;
}

.btn-remove-sm {
  width: 24px;
  height: 24px;
  border: none;
  background: #fee2e2;
  color: #dc2626;
  border-radius: 4px;
  font-size: 14px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.btn-remove-sm:hover {
  background: #fecaca;
}

/* Ghost Button */
.btn-ghost {
  background: transparent;
  color: #4f46e5;
  border: 1px dashed #c7d2fe;
  padding: 8px 12px;
}

.btn-ghost:hover:not(:disabled) {
  background: #f5f3ff;
  border-color: #a5b4fc;
}

.btn-sm {
  font-size: 12px;
  padding: 8px 12px;
}
</style>
