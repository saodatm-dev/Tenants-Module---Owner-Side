<script setup lang="ts">
import { ref, computed, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '@/services/api'

const router = useRouter()

// ==================== MOCK DATA ====================

// Mock Properties (Buildings with floors and rooms)
const mockProperties = [
  {
    id: 'prop-1',
    name: 'Tashkent Business Center',
    address: '15 Amir Temur Avenue, Tashkent',
    totalArea: 12500,
    image: 'https://images.unsplash.com/photo-1486406146926-c627a92ad1ab?w=400&h=300&fit=crop',
    floors: [
      {
        id: 'floor-1-1',
        number: 1,
        totalArea: 2500,
        rooms: [
          { id: 'room-1-1-1', name: 'Office 101', area: 85, type: 'Office' },
          { id: 'room-1-1-2', name: 'Office 102', area: 120, type: 'Office' },
          { id: 'room-1-1-3', name: 'Conference Room', area: 65, type: 'Meeting' },
          { id: 'room-1-1-4', name: 'Storage 1A', area: 45, type: 'Storage' },
        ]
      },
      {
        id: 'floor-1-2',
        number: 2,
        totalArea: 2500,
        rooms: [
          { id: 'room-1-2-1', name: 'Office 201', area: 95, type: 'Office' },
          { id: 'room-1-2-2', name: 'Office 202', area: 110, type: 'Office' },
          { id: 'room-1-2-3', name: 'Kitchen Area', area: 40, type: 'Common' },
          { id: 'room-1-2-4', name: 'Office 203', area: 75, type: 'Office' },
        ]
      },
      {
        id: 'floor-1-3',
        number: 3,
        totalArea: 2500,
        rooms: [
          { id: 'room-1-3-1', name: 'Executive Suite', area: 180, type: 'Office' },
          { id: 'room-1-3-2', name: 'Board Room', area: 90, type: 'Meeting' },
          { id: 'room-1-3-3', name: 'Reception', area: 50, type: 'Common' },
        ]
      },
    ]
  },
  {
    id: 'prop-2',
    name: 'Samarkand Plaza',
    address: '42 Registan Street, Samarkand',
    totalArea: 8200,
    image: 'https://images.unsplash.com/photo-1545324418-cc1a3fa10c00?w=400&h=300&fit=crop',
    floors: [
      {
        id: 'floor-2-1',
        number: 1,
        totalArea: 2700,
        rooms: [
          { id: 'room-2-1-1', name: 'Retail Space A', area: 150, type: 'Retail' },
          { id: 'room-2-1-2', name: 'Retail Space B', area: 200, type: 'Retail' },
          { id: 'room-2-1-3', name: 'Cafe Area', area: 80, type: 'Catering' },
        ]
      },
      {
        id: 'floor-2-2',
        number: 2,
        totalArea: 2750,
        rooms: [
          { id: 'room-2-2-1', name: 'Office 2A', area: 100, type: 'Office' },
          { id: 'room-2-2-2', name: 'Office 2B', area: 130, type: 'Office' },
          { id: 'room-2-2-3', name: 'Meeting Room', area: 55, type: 'Meeting' },
        ]
      },
    ]
  },
  {
    id: 'prop-3',
    name: 'Bukhara Industrial Park',
    address: '88 Industrial Zone, Bukhara',
    totalArea: 25000,
    image: 'https://images.unsplash.com/photo-1565610222536-ef125c59da2e?w=400&h=300&fit=crop',
    floors: [
      {
        id: 'floor-3-1',
        number: 1,
        totalArea: 12500,
        rooms: [
          { id: 'room-3-1-1', name: 'Warehouse A', area: 3000, type: 'Warehouse' },
          { id: 'room-3-1-2', name: 'Warehouse B', area: 3500, type: 'Warehouse' },
          { id: 'room-3-1-3', name: 'Loading Dock', area: 800, type: 'Logistics' },
        ]
      },
      {
        id: 'floor-3-2',
        number: 2,
        totalArea: 12500,
        rooms: [
          { id: 'room-3-2-1', name: 'Factory Floor', area: 5000, type: 'Factory' },
          { id: 'room-3-2-2', name: 'Assembly Area', area: 4000, type: 'Factory' },
          { id: 'room-3-2-3', name: 'Quality Control', area: 500, type: 'Office' },
        ]
      },
    ]
  },
  {
    id: 'prop-4',
    name: 'Navoi Hotel Complex',
    address: '5 Independence Avenue, Navoi',
    totalArea: 6800,
    image: 'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=400&h=300&fit=crop',
    floors: [
      {
        id: 'floor-4-1',
        number: 1,
        totalArea: 1700,
        rooms: [
          { id: 'room-4-1-1', name: 'Lobby', area: 200, type: 'Common' },
          { id: 'room-4-1-2', name: 'Restaurant', area: 350, type: 'Catering' },
          { id: 'room-4-1-3', name: 'Conference Hall', area: 280, type: 'Meeting' },
        ]
      },
      {
        id: 'floor-4-2',
        number: 2,
        totalArea: 1700,
        rooms: [
          { id: 'room-4-2-1', name: 'Suite 201', area: 65, type: 'Hotel' },
          { id: 'room-4-2-2', name: 'Suite 202', area: 65, type: 'Hotel' },
          { id: 'room-4-2-3', name: 'Suite 203', area: 85, type: 'Hotel' },
          { id: 'room-4-2-4', name: 'Suite 204', area: 85, type: 'Hotel' },
        ]
      },
      {
        id: 'floor-4-3',
        number: 3,
        totalArea: 1700,
        rooms: [
          { id: 'room-4-3-1', name: 'Penthouse Suite', area: 180, type: 'Hotel' },
          { id: 'room-4-3-2', name: 'Spa Center', area: 150, type: 'Wellness' },
        ]
      },
    ]
  },
]

// Mock Categories
const mockCategories = [
  { id: 'cat-1', name: 'Apartment', icon: '🏠', color: '#3B82F6' },
  { id: 'cat-2', name: 'House', icon: '🏡', color: '#10B981' },
  { id: 'cat-3', name: 'Office', icon: '🏢', color: '#6366F1' },
  { id: 'cat-4', name: 'Retail Space', icon: '🏪', color: '#F59E0B' },
  { id: 'cat-5', name: 'Catering', icon: '🍽️', color: '#EF4444' },
  { id: 'cat-6', name: 'Multi-purpose', icon: '🔄', color: '#8B5CF6' },
  { id: 'cat-7', name: 'Factory', icon: '🏭', color: '#64748B' },
  { id: 'cat-8', name: 'Hotel', icon: '🏨', color: '#EC4899' },
  { id: 'cat-9', name: 'Warehouse', icon: '📦', color: '#78716C' },
]

// ==================== WIZARD STATE ====================

const currentStep = ref(1)
const totalSteps = 5

// Step 1: Property selection
const selectedPropertyId = ref<string>('')

// Step 2: Category selection
const selectedCategoryId = ref<string>('')

// Step 3: Floor selection
const selectedFloorIds = ref<string[]>([])
const selectAllFloors = ref(false)

// Step 4: Room selection (only if specific floors selected, not all)
const selectedRoomIds = ref<string[]>([])
const sharedSpaceRoomIds = ref<string[]>([])

// Step 5: Terms
const rentalPurposes = ref<{ id: string; name: string }[]>([])
const minLeaseTermOptions = [
  { value: 0, label: 'Не указано' },
  { value: 1, label: '1 месяц' },
  { value: 2, label: '3 месяца' },
  { value: 3, label: '6 месяцев' },
  { value: 4, label: '1 год' },
  { value: 5, label: '2 года' },
  { value: 6, label: '3 года' },
]

const terms = reactive({
  fixedPrice: '',
  pricePerSqm: '',
  rentalPurposeId: '' as string,
  minLeaseTerm: null as number | null,
  utilityPaymentType: null as number | null,
  availabilityOption: '' as 'now' | 'date' | '',
  nextAvailableDate: '',
})

// Load rental purposes on mount
onMounted(async () => {
  try {
    const response = await api.getRentalPurposes({ pageSize: 50 })
    rentalPurposes.value = response.items || response || []
  } catch (e) {
    console.error('Failed to load rental purposes:', e)
  }
})

// ==================== COMPUTED ====================

const selectedProperty = computed(() =>
  mockProperties.find(p => p.id === selectedPropertyId.value)
)

const selectedCategory = computed(() =>
  mockCategories.find(c => c.id === selectedCategoryId.value)
)

const availableFloors = computed(() =>
  selectedProperty.value?.floors || []
)

const availableRooms = computed(() => {
  if (!selectedProperty.value) return []
  if (selectAllFloors.value) {
    // All rooms from all floors
    return selectedProperty.value.floors.flatMap(f =>
      f.rooms.map(r => ({ ...r, floorNumber: f.number }))
    )
  }
  // Only rooms from selected floors
  return selectedProperty.value.floors
    .filter(f => selectedFloorIds.value.includes(f.id))
    .flatMap(f => f.rooms.map(r => ({ ...r, floorNumber: f.number })))
})


// Step validation
const stepValidation = computed(() => ({
  1: selectedPropertyId.value !== '',
  2: selectedCategoryId.value !== '',
  3: selectAllFloors.value || selectedFloorIds.value.length > 0,
  4: selectAllFloors.value || selectedRoomIds.value.length > 0,
  5: terms.fixedPrice !== '' || terms.pricePerSqm !== '',
}))

const canProceed = computed(() => {
  const step = currentStep.value
  if (step === 4 && selectAllFloors.value) return true // Skip room validation if all floors
  return stepValidation.value[step as keyof typeof stepValidation.value]
})

// Calculate total selected area
const totalSelectedArea = computed(() => {
  if (selectAllFloors.value) {
    return availableFloors.value.reduce((sum, f) => sum + f.totalArea, 0)
  }
  return availableRooms.value
    .filter(r => selectedRoomIds.value.includes(r.id))
    .reduce((sum, r) => sum + r.area, 0)
})

// ==================== METHODS ====================

function nextStep() {
  if (currentStep.value === 3 && selectAllFloors.value) {
    // Skip room selection if all floors are selected
    currentStep.value = 5
  } else if (canProceed.value && currentStep.value < totalSteps) {
    currentStep.value++
  }
}

function prevStep() {
  if (currentStep.value === 5 && selectAllFloors.value) {
    // Go back to floor selection if we skipped rooms
    currentStep.value = 3
  } else if (currentStep.value > 1) {
    currentStep.value--
  }
}

function goToStep(step: number) {
  // Allow going back to any previous step
  if (step < currentStep.value) {
    currentStep.value = step
  }
}

// Property selection
function selectProperty(propertyId: string) {
  selectedPropertyId.value = propertyId
}

// Category selection
function selectCategory(categoryId: string) {
  selectedCategoryId.value = categoryId
}

// Floor selection
function toggleFloor(floorId: string) {
  const index = selectedFloorIds.value.indexOf(floorId)
  if (index >= 0) {
    selectedFloorIds.value.splice(index, 1)
  } else {
    selectedFloorIds.value.push(floorId)
  }
  selectAllFloors.value = false
}

function toggleSelectAllFloors() {
  selectAllFloors.value = !selectAllFloors.value
  if (selectAllFloors.value) {
    selectedFloorIds.value = availableFloors.value.map(f => f.id)
  }
}

function isFloorSelected(floorId: string): boolean {
  return selectedFloorIds.value.includes(floorId)
}

// Room selection
function toggleRoom(roomId: string) {
  const index = selectedRoomIds.value.indexOf(roomId)
  if (index >= 0) {
    selectedRoomIds.value.splice(index, 1)
    // Also remove from shared spaces if deselected
    const sharedIndex = sharedSpaceRoomIds.value.indexOf(roomId)
    if (sharedIndex >= 0) {
      sharedSpaceRoomIds.value.splice(sharedIndex, 1)
    }
  } else {
    selectedRoomIds.value.push(roomId)
  }
}

function toggleSelectAllRooms() {
  if (selectedRoomIds.value.length === availableRooms.value.length) {
    selectedRoomIds.value = []
    sharedSpaceRoomIds.value = []
  } else {
    selectedRoomIds.value = availableRooms.value.map(r => r.id)
  }
}

function isRoomSelected(roomId: string): boolean {
  return selectedRoomIds.value.includes(roomId)
}

function toggleSharedSpace(roomId: string) {
  if (!isRoomSelected(roomId)) return
  const index = sharedSpaceRoomIds.value.indexOf(roomId)
  if (index >= 0) {
    sharedSpaceRoomIds.value.splice(index, 1)
  } else {
    sharedSpaceRoomIds.value.push(roomId)
  }
}

function isSharedSpace(roomId: string): boolean {
  return sharedSpaceRoomIds.value.includes(roomId)
}

// Submit
function handleSubmit() {
  const listingData = {
    property: selectedProperty.value,
    category: selectedCategory.value,
    floors: selectAllFloors.value
      ? 'All Floors'
      : availableFloors.value.filter(f => selectedFloorIds.value.includes(f.id)),
    rooms: selectAllFloors.value
      ? 'Entire Building'
      : availableRooms.value.filter(r => selectedRoomIds.value.includes(r.id)),
    sharedSpaces: availableRooms.value.filter(r => sharedSpaceRoomIds.value.includes(r.id)),
    terms: {
      fixedPrice: terms.fixedPrice ? parseFloat(terms.fixedPrice) : null,
      pricePerSqm: terms.pricePerSqm ? parseFloat(terms.pricePerSqm) : null,
      rentalPurposeId: terms.rentalPurposeId || null,
      minLeaseTerm: terms.minLeaseTerm,
      utilityPaymentType: terms.utilityPaymentType,
      nextAvailableDate: terms.availabilityOption === 'now'
        ? new Date().toISOString().split('T')[0]
        : terms.nextAvailableDate || null,
    },
    totalArea: totalSelectedArea.value,
  }

  console.log('Creating listing:', listingData)
  alert('Listing created successfully! Check console for details.')
  router.push('/listings')
}

// Step labels
const stepLabels = ['Property', 'Category', 'Floors', 'Rooms', 'Terms']
</script>

<template>
  <div class="wizard-container">
    <!-- Header -->
    <div class="wizard-header">
      <button class="btn-back" @click="router.back()">
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M19 12H5M12 19l-7-7 7-7"/>
        </svg>
        Back
      </button>
      <h1 class="wizard-title">Create New Listing</h1>
    </div>

    <!-- Progress Steps -->
    <div class="progress-container">
      <div class="progress-steps">
        <template v-for="(label, index) in stepLabels" :key="index">
          <div
            class="step-item"
            :class="{
              active: currentStep === index + 1,
              completed: currentStep > index + 1,
              clickable: index + 1 < currentStep,
              skipped: index + 1 === 4 && selectAllFloors
            }"
            @click="goToStep(index + 1)"
          >
            <div class="step-circle">
              <span v-if="currentStep > index + 1" class="check-icon">✓</span>
              <span v-else>{{ index + 1 }}</span>
            </div>
            <span class="step-label">{{ label }}</span>
          </div>
          <div v-if="index < stepLabels.length - 1" class="step-connector" :class="{ completed: currentStep > index + 1 }"></div>
        </template>
      </div>
    </div>

    <!-- Step Content -->
    <div class="step-content">
      <!-- Step 1: Select Property -->
      <div v-if="currentStep === 1" class="step-panel">
        <div class="step-header">
          <h2>Select Property</h2>
          <p>Choose a property from your portfolio to create a listing</p>
        </div>

        <div class="cards-grid properties-grid">
          <div
            v-for="property in mockProperties"
            :key="property.id"
            class="property-card"
            :class="{ selected: selectedPropertyId === property.id }"
            @click="selectProperty(property.id)"
          >
            <div class="card-image">
              <img :src="property.image" :alt="property.name" />
              <div class="card-check" v-if="selectedPropertyId === property.id">
                <span>✓</span>
              </div>
            </div>
            <div class="card-content">
              <h3 class="card-title">{{ property.name }}</h3>
              <p class="card-address">{{ property.address }}</p>
              <div class="card-meta">
                <span class="meta-item">
                  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <rect x="3" y="3" width="18" height="18" rx="2"/>
                  </svg>
                  {{ property.totalArea.toLocaleString() }} m²
                </span>
                <span class="meta-item">
                  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V9z"/>
                  </svg>
                  {{ property.floors.length }} floors
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Step 2: Select Category -->
      <div v-if="currentStep === 2" class="step-panel">
        <div class="step-header">
          <h2>Select Category</h2>
          <p>What type of rental property is this?</p>
        </div>

        <div class="cards-grid categories-grid">
          <div
            v-for="category in mockCategories"
            :key="category.id"
            class="category-card"
            :class="{ selected: selectedCategoryId === category.id }"
            :style="{ '--category-color': category.color }"
            @click="selectCategory(category.id)"
          >
            <div class="category-icon">{{ category.icon }}</div>
            <div class="category-name">{{ category.name }}</div>
            <div class="category-check" v-if="selectedCategoryId === category.id">
              <span>✓</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Step 3: Select Floors -->
      <div v-if="currentStep === 3" class="step-panel">
        <div class="step-header">
          <h2>Select Floor(s)</h2>
          <p>Choose which floors you want to include in this listing</p>
        </div>

        <!-- Select All Option -->
        <div
          class="select-all-card"
          :class="{ selected: selectAllFloors }"
          @click="toggleSelectAllFloors"
        >
          <div class="select-all-icon">🏢</div>
          <div class="select-all-content">
            <h3>Select All Floors</h3>
            <p>List the entire building as a single unit</p>
          </div>
          <div class="select-all-check">
            <span v-if="selectAllFloors">✓</span>
          </div>
        </div>

        <div class="divider-text">
          <span>Or select specific floors</span>
        </div>

        <div class="cards-grid floors-grid">
          <div
            v-for="floor in availableFloors"
            :key="floor.id"
            class="floor-card"
            :class="{ selected: isFloorSelected(floor.id), disabled: selectAllFloors }"
            @click="!selectAllFloors && toggleFloor(floor.id)"
          >
            <div class="floor-number">
              <span>{{ floor.number }}</span>
            </div>
            <div class="floor-content">
              <h3>Floor {{ floor.number }}</h3>
              <p>{{ floor.totalArea.toLocaleString() }} m² • {{ floor.rooms.length }} rooms</p>
            </div>
            <div class="floor-check" v-if="isFloorSelected(floor.id) && !selectAllFloors">
              <span>✓</span>
            </div>
          </div>
        </div>

        <div class="selection-summary" v-if="selectAllFloors || selectedFloorIds.length > 0">
          <span v-if="selectAllFloors">📋 Entire building selected ({{ availableFloors.length }} floors)</span>
          <span v-else>📋 {{ selectedFloorIds.length }} floor(s) selected</span>
        </div>
      </div>

      <!-- Step 4: Select Rooms -->
      <div v-if="currentStep === 4" class="step-panel">
        <div class="step-header">
          <h2>Select Rooms</h2>
          <p>Choose rooms to include and mark shared spaces</p>
        </div>

        <!-- Select All Rooms -->
        <div
          class="select-all-card compact"
          :class="{ selected: selectedRoomIds.length === availableRooms.length }"
          @click="toggleSelectAllRooms"
        >
          <div class="select-all-icon">🚪</div>
          <div class="select-all-content">
            <h3>Select All Rooms</h3>
            <p>Include all {{ availableRooms.length }} rooms from selected floors</p>
          </div>
          <div class="select-all-check">
            <span v-if="selectedRoomIds.length === availableRooms.length">✓</span>
          </div>
        </div>

        <div class="rooms-list">
          <div
            v-for="room in availableRooms"
            :key="room.id"
            class="room-item"
            :class="{ selected: isRoomSelected(room.id), shared: isSharedSpace(room.id) }"
          >
            <div class="room-select" @click="toggleRoom(room.id)">
              <div class="room-checkbox">
                <span v-if="isRoomSelected(room.id)">✓</span>
              </div>
              <div class="room-info">
                <h4>{{ room.name }}</h4>
                <p>Floor {{ room.floorNumber }} • {{ room.area }} m² • {{ room.type }}</p>
              </div>
            </div>
            <button
              class="shared-toggle"
              :class="{ active: isSharedSpace(room.id) }"
              :disabled="!isRoomSelected(room.id)"
              @click.stop="toggleSharedSpace(room.id)"
              title="Mark as shared space"
            >
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
                <circle cx="9" cy="7" r="4"/>
                <path d="M23 21v-2a4 4 0 0 0-3-3.87"/>
                <path d="M16 3.13a4 4 0 0 1 0 7.75"/>
              </svg>
              <span>Shared</span>
            </button>
          </div>
        </div>

        <div class="selection-summary" v-if="selectedRoomIds.length > 0">
          <span>📋 {{ selectedRoomIds.length }} room(s) selected</span>
          <span v-if="sharedSpaceRoomIds.length > 0"> • {{ sharedSpaceRoomIds.length }} shared space(s)</span>
        </div>
      </div>

      <!-- Step 5: Rental Conditions -->
      <div v-if="currentStep === 5" class="step-panel">
        <div class="step-header">
          <h2>Условия сдачи</h2>
        </div>

        <div class="rental-conditions-form">
          <!-- Rental Terms Section -->
          <div class="rc-section">
            <h3 class="rc-section-title">Условия аренды</h3>

            <div class="rc-field">
              <select v-model="terms.rentalPurposeId" class="rc-select">
                <option value="" disabled>Rental purpose</option>
                <option v-for="rp in rentalPurposes" :key="rp.id" :value="rp.id">{{ rp.name }}</option>
              </select>
              <svg class="rc-select-chevron" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M6 9l6 6 6-6"/></svg>
            </div>

            <div class="rc-field">
              <select v-model="terms.minLeaseTerm" class="rc-select">
                <option :value="null" disabled>Минимальный срок аренды</option>
                <option v-for="opt in minLeaseTermOptions" :key="opt.value" :value="opt.value">{{ opt.label }}</option>
              </select>
              <svg class="rc-select-chevron" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M6 9l6 6 6-6"/></svg>
            </div>
          </div>

          <!-- Utility Payment Section -->
          <div class="rc-section">
            <h3 class="rc-section-title">Коммунальные услуги</h3>
            <div class="rc-radio-group">
              <label class="rc-radio" @click="terms.utilityPaymentType = 0">
                <span class="rc-radio-circle" :class="{ active: terms.utilityPaymentType === 0 }"></span>
                <span>От владельца</span>
              </label>
              <label class="rc-radio" @click="terms.utilityPaymentType = 1">
                <span class="rc-radio-circle" :class="{ active: terms.utilityPaymentType === 1 }"></span>
                <span>От арендатора</span>
              </label>
              <label class="rc-radio" @click="terms.utilityPaymentType = 2">
                <span class="rc-radio-circle" :class="{ active: terms.utilityPaymentType === 2 }"></span>
                <span>Договорная</span>
              </label>
            </div>
          </div>

          <!-- Availability Section -->
          <div class="rc-section">
            <h3 class="rc-section-title">Доступен для аренды</h3>
            <div class="rc-radio-group">
              <label class="rc-radio" @click="terms.availabilityOption = 'now'">
                <span class="rc-radio-circle" :class="{ active: terms.availabilityOption === 'now' }"></span>
                <span>Сейчас</span>
              </label>
              <label class="rc-radio" @click="terms.availabilityOption = 'date'">
                <span class="rc-radio-circle" :class="{ active: terms.availabilityOption === 'date' }"></span>
                <span>Выбрать дату</span>
              </label>
            </div>
            <div v-if="terms.availabilityOption === 'date'" class="rc-date-field">
              <input
                type="date"
                v-model="terms.nextAvailableDate"
                class="rc-date-input"
                :min="new Date().toISOString().split('T')[0]"
              />
            </div>
          </div>
        </div>

        <!-- Footer Buttons -->
        <div class="rc-footer">
          <button class="rc-btn-draft" @click="handleSubmit">Сохранить как черновик</button>
          <button class="rc-btn-continue" @click="handleSubmit">Продолжить</button>
        </div>
      </div>
    </div>

    <!-- Navigation -->
    <div class="wizard-navigation">
      <button
        v-if="currentStep > 1"
        class="btn btn-secondary"
        @click="prevStep"
      >
        ← Previous
      </button>
      <div class="nav-spacer"></div>
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
        class="btn btn-success"
        :disabled="!canProceed"
        @click="handleSubmit"
      >
        🎉 Create Listing
      </button>
    </div>
  </div>
</template>

<style scoped>
/* ==================== BASE STYLES ==================== */
.wizard-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
  padding: 24px;
}

/* ==================== HEADER ==================== */
.wizard-header {
  max-width: 1200px;
  margin: 0 auto 24px;
}

.btn-back {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 10px 16px;
  color: #64748b;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  margin-bottom: 16px;
}

.btn-back:hover {
  background: #f8fafc;
  border-color: #cbd5e1;
  color: #334155;
}

.wizard-title {
  font-size: 32px;
  font-weight: 700;
  color: #0f172a;
  margin: 0;
  background: linear-gradient(135deg, #0f172a 0%, #334155 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

/* ==================== PROGRESS STEPS ==================== */
.progress-container {
  max-width: 900px;
  margin: 0 auto 32px;
  background: white;
  border-radius: 20px;
  padding: 24px 32px;
  box-shadow: 0 4px 6px -1px rgba(0,0,0,0.05), 0 2px 4px -2px rgba(0,0,0,0.05);
}

.progress-steps {
  display: flex;
  align-items: center;
  justify-content: center;
}

.step-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  cursor: default;
  transition: all 0.3s ease;
}

.step-item.clickable {
  cursor: pointer;
}

.step-item.clickable:hover .step-circle {
  transform: scale(1.1);
}

.step-circle {
  width: 44px;
  height: 44px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  font-size: 16px;
  background: #e2e8f0;
  color: #64748b;
  transition: all 0.3s ease;
}

.step-item.active .step-circle {
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
  color: white;
  box-shadow: 0 4px 12px rgba(16, 185, 129, 0.4);
}

.step-item.completed .step-circle {
  background: #10b981;
  color: white;
}

.step-item.skipped .step-circle {
  background: #94a3b8;
  color: white;
}

.step-label {
  font-size: 13px;
  font-weight: 500;
  color: #64748b;
}

.step-item.active .step-label {
  color: #10b981;
  font-weight: 600;
}

.step-item.completed .step-label {
  color: #10b981;
}

.step-connector {
  width: 60px;
  height: 3px;
  background: #e2e8f0;
  margin: 0 8px;
  margin-bottom: 24px;
  border-radius: 2px;
  transition: background 0.3s ease;
}

.step-connector.completed {
  background: #10b981;
}

.check-icon {
  font-size: 18px;
}

/* ==================== STEP CONTENT ==================== */
.step-content {
  max-width: 1200px;
  margin: 0 auto;
}

.step-panel {
  animation: fadeIn 0.4s ease;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.step-header {
  text-align: center;
  margin-bottom: 32px;
}

.step-header h2 {
  font-size: 28px;
  font-weight: 700;
  color: #0f172a;
  margin: 0 0 8px;
}

.step-header p {
  font-size: 16px;
  color: #64748b;
  margin: 0;
}

/* ==================== CARDS GRID ==================== */
.cards-grid {
  display: grid;
  gap: 20px;
}

.properties-grid {
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
}

.categories-grid {
  grid-template-columns: repeat(auto-fill, minmax(160px, 1fr));
  max-width: 900px;
  margin: 0 auto;
}

.floors-grid {
  grid-template-columns: repeat(auto-fill, minmax(240px, 1fr));
  max-width: 900px;
  margin: 0 auto;
}

/* ==================== PROPERTY CARDS ==================== */
.property-card {
  background: white;
  border-radius: 20px;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.3s ease;
  border: 2px solid transparent;
  box-shadow: 0 4px 6px -1px rgba(0,0,0,0.05);
}

.property-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 24px -4px rgba(0,0,0,0.12);
}

.property-card.selected {
  border-color: #10b981;
  box-shadow: 0 8px 24px rgba(16, 185, 129, 0.25);
}

.card-image {
  position: relative;
  height: 180px;
  overflow: hidden;
}

.card-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.property-card:hover .card-image img {
  transform: scale(1.05);
}

.card-check {
  position: absolute;
  top: 12px;
  right: 12px;
  width: 32px;
  height: 32px;
  background: #10b981;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 600;
  animation: scaleIn 0.3s ease;
}

@keyframes scaleIn {
  from {
    transform: scale(0);
  }
  to {
    transform: scale(1);
  }
}

.card-content {
  padding: 20px;
}

.card-title {
  font-size: 18px;
  font-weight: 600;
  color: #0f172a;
  margin: 0 0 6px;
}

.card-address {
  font-size: 14px;
  color: #64748b;
  margin: 0 0 16px;
}

.card-meta {
  display: flex;
  gap: 16px;
}

.meta-item {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 13px;
  color: #475569;
}

.meta-item svg {
  color: #94a3b8;
}

/* ==================== CATEGORY CARDS ==================== */
.category-card {
  background: white;
  border-radius: 16px;
  padding: 24px;
  text-align: center;
  cursor: pointer;
  transition: all 0.3s ease;
  border: 2px solid transparent;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
  position: relative;
}

.category-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(0,0,0,0.1);
  border-color: var(--category-color);
}

.category-card.selected {
  border-color: var(--category-color);
  background: linear-gradient(135deg, white 0%, color-mix(in srgb, var(--category-color) 8%, white) 100%);
}

.category-icon {
  font-size: 40px;
  margin-bottom: 12px;
}

.category-name {
  font-size: 15px;
  font-weight: 600;
  color: #334155;
}

.category-check {
  position: absolute;
  top: 10px;
  right: 10px;
  width: 24px;
  height: 24px;
  background: var(--category-color);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 12px;
  font-weight: 600;
  animation: scaleIn 0.3s ease;
}

/* ==================== SELECT ALL CARD ==================== */
.select-all-card {
  max-width: 600px;
  margin: 0 auto 24px;
  background: linear-gradient(135deg, #fefce8 0%, #fef9c3 100%);
  border: 2px solid #fbbf24;
  border-radius: 16px;
  padding: 20px 24px;
  display: flex;
  align-items: center;
  gap: 16px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.select-all-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(251, 191, 36, 0.25);
}

.select-all-card.selected {
  background: linear-gradient(135deg, #fef9c3 0%, #fde047 100%);
  border-color: #f59e0b;
}

.select-all-card.compact {
  padding: 16px 20px;
}

.select-all-icon {
  font-size: 32px;
}

.select-all-content h3 {
  font-size: 16px;
  font-weight: 600;
  color: #92400e;
  margin: 0 0 4px;
}

.select-all-content p {
  font-size: 13px;
  color: #a16207;
  margin: 0;
}

.select-all-check {
  margin-left: auto;
  width: 28px;
  height: 28px;
  border: 2px solid #f59e0b;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #92400e;
  font-weight: 600;
  background: white;
}

.select-all-card.selected .select-all-check {
  background: #f59e0b;
  color: white;
}

/* ==================== DIVIDER ==================== */
.divider-text {
  text-align: center;
  margin: 24px 0;
  position: relative;
}

.divider-text::before {
  content: '';
  position: absolute;
  left: 0;
  right: 0;
  top: 50%;
  height: 1px;
  background: #e2e8f0;
}

.divider-text span {
  background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
  padding: 0 16px;
  font-size: 14px;
  color: #64748b;
  position: relative;
}

/* ==================== FLOOR CARDS ==================== */
.floor-card {
  background: white;
  border-radius: 14px;
  padding: 20px;
  display: flex;
  align-items: center;
  gap: 16px;
  cursor: pointer;
  transition: all 0.3s ease;
  border: 2px solid transparent;
}

.floor-card:hover:not(.disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(0,0,0,0.1);
}

.floor-card.selected {
  border-color: #10b981;
  background: linear-gradient(135deg, white 0%, #ecfdf5 100%);
}

.floor-card.disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.floor-number {
  width: 48px;
  height: 48px;
  background: linear-gradient(135deg, #e0f2fe 0%, #bae6fd 100%);
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px;
  font-weight: 700;
  color: #0284c7;
}

.floor-card.selected .floor-number {
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
  color: white;
}

.floor-content h3 {
  font-size: 16px;
  font-weight: 600;
  color: #0f172a;
  margin: 0 0 4px;
}

.floor-content p {
  font-size: 13px;
  color: #64748b;
  margin: 0;
}

.floor-check {
  margin-left: auto;
  width: 28px;
  height: 28px;
  background: #10b981;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 14px;
  font-weight: 600;
  animation: scaleIn 0.3s ease;
}

/* ==================== ROOMS LIST ==================== */
.rooms-list {
  max-width: 800px;
  margin: 24px auto 0;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.room-item {
  background: white;
  border-radius: 14px;
  padding: 16px 20px;
  display: flex;
  align-items: center;
  gap: 16px;
  transition: all 0.3s ease;
  border: 2px solid transparent;
}

.room-item.selected {
  border-color: #10b981;
  background: linear-gradient(135deg, white 0%, #ecfdf5 100%);
}

.room-item.shared {
  border-color: #6366f1;
  background: linear-gradient(135deg, white 0%, #eef2ff 100%);
}

.room-select {
  flex: 1;
  display: flex;
  align-items: center;
  gap: 14px;
  cursor: pointer;
}

.room-checkbox {
  width: 24px;
  height: 24px;
  border: 2px solid #cbd5e1;
  border-radius: 6px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 12px;
  font-weight: 600;
  transition: all 0.2s ease;
}

.room-item.selected .room-checkbox {
  background: #10b981;
  border-color: #10b981;
}

.room-info h4 {
  font-size: 15px;
  font-weight: 600;
  color: #0f172a;
  margin: 0 0 4px;
}

.room-info p {
  font-size: 13px;
  color: #64748b;
  margin: 0;
}

.shared-toggle {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 14px;
  background: #f1f5f9;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  font-size: 13px;
  font-weight: 500;
  color: #64748b;
  cursor: pointer;
  transition: all 0.2s ease;
}

.shared-toggle:hover:not(:disabled) {
  background: #e2e8f0;
}

.shared-toggle.active {
  background: #6366f1;
  border-color: #6366f1;
  color: white;
}

.shared-toggle:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

/* ==================== SELECTION SUMMARY ==================== */
.selection-summary {
  max-width: 600px;
  margin: 24px auto 0;
  text-align: center;
  padding: 16px;
  background: linear-gradient(135deg, #ecfdf5 0%, #d1fae5 100%);
  border-radius: 12px;
  color: #065f46;
  font-weight: 500;
}

/* ==================== TERMS FORM ==================== */
.terms-form {
  max-width: 700px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  gap: 32px;
}

.terms-section {
  background: white;
  border-radius: 16px;
  padding: 24px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
}

.section-title {
  font-size: 18px;
  font-weight: 600;
  color: #0f172a;
  margin: 0 0 20px;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-size: 14px;
  font-weight: 500;
  color: #475569;
}

.input-with-icon {
  position: relative;
}

.input-with-icon input {
  width: 100%;
  padding: 14px 16px;
  padding-right: 60px;
  border: 2px solid #e2e8f0;
  border-radius: 12px;
  font-size: 15px;
  transition: all 0.2s ease;
}

.input-with-icon input:focus {
  outline: none;
  border-color: #10b981;
  box-shadow: 0 0 0 4px rgba(16, 185, 129, 0.1);
}

.input-suffix {
  position: absolute;
  right: 16px;
  top: 50%;
  transform: translateY(-50%);
  font-size: 13px;
  color: #94a3b8;
  font-weight: 500;
}

input[type="date"] {
  width: 100%;
  padding: 14px 16px;
  border: 2px solid #e2e8f0;
  border-radius: 12px;
  font-size: 15px;
  transition: all 0.2s ease;
}

input[type="date"]:focus {
  outline: none;
  border-color: #10b981;
  box-shadow: 0 0 0 4px rgba(16, 185, 129, 0.1);
}

.calculated-price {
  margin-top: 16px;
  padding: 12px 16px;
  background: linear-gradient(135deg, #ecfdf5 0%, #d1fae5 100%);
  border-radius: 10px;
  font-size: 14px;
  color: #065f46;
}

.calculated-price strong {
  font-size: 16px;
}

.area-info {
  opacity: 0.8;
}

/* ==================== RENTAL CONDITIONS ==================== */
.rental-conditions-form {
  max-width: 520px;
  margin: 0 auto;
}

.rc-section {
  margin-bottom: 28px;
}

.rc-section-title {
  font-size: 15px;
  font-weight: 600;
  color: #0f172a;
  margin: 0 0 14px;
}

.rc-field {
  position: relative;
  margin-bottom: 12px;
}

.rc-select {
  width: 100%;
  padding: 14px 40px 14px 16px;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  font-size: 15px;
  color: #0f172a;
  background: white;
  appearance: none;
  cursor: pointer;
  transition: border-color 0.2s ease;
}

.rc-select:focus {
  outline: none;
  border-color: #0f172a;
}

.rc-select option[disabled] {
  color: #94a3b8;
}

.rc-select-chevron {
  position: absolute;
  right: 14px;
  top: 50%;
  transform: translateY(-50%);
  color: #94a3b8;
  pointer-events: none;
}

.rc-radio-group {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.rc-radio {
  display: flex;
  align-items: center;
  gap: 12px;
  cursor: pointer;
  font-size: 15px;
  color: #0f172a;
  user-select: none;
}

.rc-radio-circle {
  width: 20px;
  height: 20px;
  border-radius: 50%;
  border: 2px solid #d1d5db;
  flex-shrink: 0;
  transition: all 0.2s ease;
  position: relative;
}

.rc-radio-circle.active {
  border-color: #0f172a;
}

.rc-radio-circle.active::after {
  content: '';
  position: absolute;
  top: 3px;
  left: 3px;
  width: 10px;
  height: 10px;
  border-radius: 50%;
  background: #0f172a;
}

.rc-date-field {
  margin-top: 12px;
}

.rc-date-input {
  width: 100%;
  padding: 14px 16px;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  font-size: 15px;
  color: #0f172a;
  transition: border-color 0.2s ease;
}

.rc-date-input:focus {
  outline: none;
  border-color: #0f172a;
}

.rc-footer {
  max-width: 520px;
  margin: 40px auto 0;
  display: flex;
  justify-content: center;
  gap: 12px;
}

.rc-btn-draft {
  padding: 14px 28px;
  border-radius: 28px;
  font-size: 15px;
  font-weight: 500;
  cursor: pointer;
  border: 1px solid #e2e8f0;
  background: white;
  color: #0f172a;
  transition: all 0.2s ease;
}

.rc-btn-draft:hover {
  background: #f8fafc;
  border-color: #cbd5e1;
}

.rc-btn-continue {
  padding: 14px 32px;
  border-radius: 28px;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  border: none;
  background: #0f172a;
  color: white;
  transition: all 0.2s ease;
}

.rc-btn-continue:hover {
  background: #1e293b;
}

/* ==================== LISTING SUMMARY ==================== */
.listing-summary {
  max-width: 700px;
  margin: 32px auto 0;
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
  border-radius: 20px;
  padding: 24px;
  color: white;
}

.listing-summary h3 {
  font-size: 18px;
  font-weight: 600;
  margin: 0 0 20px;
  color: white;
}

.summary-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 16px;
}

.summary-item {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.summary-item .label {
  font-size: 12px;
  color: #94a3b8;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.summary-item .value {
  font-size: 15px;
  font-weight: 500;
  color: white;
}

/* ==================== NAVIGATION ==================== */
.wizard-navigation {
  max-width: 1200px;
  margin: 32px auto 0;
  display: flex;
  align-items: center;
  padding: 24px;
  background: white;
  border-radius: 16px;
  box-shadow: 0 -4px 20px rgba(0,0,0,0.05);
}

.nav-spacer {
  flex: 1;
}

.btn {
  padding: 14px 28px;
  border-radius: 12px;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  border: none;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-secondary {
  background: #f1f5f9;
  color: #475569;
}

.btn-secondary:hover:not(:disabled) {
  background: #e2e8f0;
}

.btn-primary {
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
  color: white;
}

.btn-primary:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(16, 185, 129, 0.35);
}

.btn-success {
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
  color: white;
  padding: 14px 36px;
}

.btn-success:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(16, 185, 129, 0.35);
}

/* ==================== RESPONSIVE ==================== */
@media (max-width: 768px) {
  .wizard-container {
    padding: 16px;
  }

  .wizard-title {
    font-size: 24px;
  }

  .progress-container {
    padding: 16px;
  }

  .step-connector {
    width: 30px;
  }

  .step-label {
    display: none;
  }

  .form-row {
    grid-template-columns: 1fr;
  }

  .utilities-options {
    grid-template-columns: 1fr;
  }

  .wizard-navigation {
    padding: 16px;
  }

  .btn {
    padding: 12px 20px;
    font-size: 14px;
  }
}
</style>
