<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { api } from '@/services/api'
import type {
  Category,
  Amenity,
  CreateListingRequest
} from '@/types/listing'
import type { RealEstate, RealEstateUnit } from '@/types/realestate'
import type { Floor } from '@/types/building'
import type { Language } from '@/types/common'

const router = useRouter()
const route = useRoute()

// Edit mode
const isEditMode = computed(() => route.params.id !== undefined)
const listingId = computed(() => route.params.id as string)

// Wizard state
const currentStep = ref(1)
const totalSteps = 5
const loading = ref(false)
const saving = ref(false)
const error = ref('')

// ─── Step 1: Property & scope ───
const listingTitle = ref<string>('')
const selectedRealEstateId = ref<string>('')
const myRealEstates = ref<RealEstate[]>([])
const loadingRealEstates = ref(false)
const propertySearch = ref('')

// Scope: what parts of the property to list
type ListingScope = 'entire' | 'floors' | 'units'
const listingScope = ref<ListingScope>('entire')

// Floor selection
const floors = ref<Floor[]>([])
const selectedFloorIds = ref<string[]>([])
const loadingFloors = ref(false)

// Unit selection
const units = ref<RealEstateUnit[]>([])
const selectedUnitIds = ref<string[]>([])
const loadingUnits = ref(false)

// ─── Step 2: Categories ───
const selectedCategoryIds = ref<string[]>([])
const categories = ref<Category[]>([])

// ─── Step 3: Amenities ───
const selectedAmenityIds = ref<string[]>([])
const amenities = ref<Amenity[]>([])

// ─── Step 5: Price & Description ───
const description = ref<string>('')
const priceForMonthDisplay = ref<string>('')
const pricePerSquareMeterDisplay = ref<string>('')

// ─── Description translations ───
const descriptionRu = ref('')
const descriptionUz = ref('')
const descriptionEn = ref('')
const activeLang = ref<'ru' | 'uz' | 'en'>('ru')
const languages = ref<Language[]>([])
const isTranslating = ref(false)
let translateTimer: ReturnType<typeof setTimeout> | null = null

// ─── Step 5: Rental Conditions ───
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
const selectedRentalPurposeId = ref<string>('')
const selectedMinLeaseTerm = ref<number | null>(null)
const selectedUtilityPaymentType = ref<number | null>(null)
const availabilityOption = ref<'now' | 'date' | ''>('')
const nextAvailableDate = ref<string>('')

// ── Computed ──

const filteredRealEstates = computed(() => {
  const q = propertySearch.value.toLowerCase().trim()
  if (!q) return myRealEstates.value
  return myRealEstates.value.filter(re => {
    const fields = [re.number, re.realEstateTypeName, re.address, re.regionName, re.districtName]
    return fields.some(f => (f || '').toLowerCase().includes(q))
  })
})

const selectedProperty = computed(() =>
  myRealEstates.value.find(p => p.id === selectedRealEstateId.value)
)

const stepValidation = computed(() => {
  let step1Valid = selectedRealEstateId.value !== ''
  // If scope is floors/units, require at least one selection
  if (step1Valid && listingScope.value === 'floors') {
    step1Valid = selectedFloorIds.value.length > 0
  }
  if (step1Valid && listingScope.value === 'units') {
    step1Valid = selectedUnitIds.value.length > 0
  }
  return {
    1: step1Valid,
    2: selectedCategoryIds.value.length > 0,
    3: true,
    4: true,
    5: true
  }
})

const canProceed = computed(() =>
  stepValidation.value[currentStep.value as keyof typeof stepValidation.value]
)

// ── Data loading ──

async function loadReferenceData() {
  loading.value = true
  try {
    const [categoriesRes, amenitiesRes, rentalPurposesRes, languagesRes] = await Promise.all([
      api.getCategories({ pageSize: 100 }),
      api.getAmenities({ pageSize: 100 }),
      api.getRentalPurposes({ pageSize: 50 }),
      api.getLanguages({ pageSize: 10 })
    ])
    const rawCategories = categoriesRes?.items || categoriesRes || []
    const uniqueCategories = new Map()
    for (const cat of rawCategories) {
      if (!uniqueCategories.has(cat.id)) {
        uniqueCategories.set(cat.id, cat)
      }
    }
    categories.value = Array.from(uniqueCategories.values())
    amenities.value = amenitiesRes?.items || []
    rentalPurposes.value = rentalPurposesRes?.items || rentalPurposesRes || []
    languages.value = languagesRes?.items || []
  } catch (e) {
    console.error('Failed to load reference data:', e)
    error.value = 'Error loading data'
  } finally {
    loading.value = false
  }
}

async function loadMyRealEstates() {
  loadingRealEstates.value = true
  try {
    const res = await api.getMyRealEstates({ pageSize: 100 })
    myRealEstates.value = res?.items || []
  } catch (e) {
    console.error('Failed to load properties:', e)
    myRealEstates.value = []
  } finally {
    loadingRealEstates.value = false
  }
}

async function loadFloorsForProperty(buildingId: string) {
  loadingFloors.value = true
  try {
    const res = await api.getFloors({ buildingId, pageSize: 100 })
    floors.value = res?.items || []
  } catch (e) {
    console.error('Failed to load floors:', e)
    floors.value = []
  } finally {
    loadingFloors.value = false
  }
}

async function loadUnitsForProperty(realEstateId: string) {
  loadingUnits.value = true
  try {
    const res = await api.getRealEstateUnits({ realEstateId, pageSize: 100 })
    units.value = res?.items || []
  } catch (e) {
    console.error('Failed to load units:', e)
    units.value = []
  } finally {
    loadingUnits.value = false
  }
}

async function loadListing() {
  if (!isEditMode.value) return
  loading.value = true
  try {
    const data = await api.getListingById(listingId.value)
    if (data.realEstateId) {
      selectedRealEstateId.value = data.realEstateId
    }
    selectedCategoryIds.value = data.listingCategoryIds || data.categoryIds || []
    listingTitle.value = data.title || ''
    description.value = data.description || ''
    if (data.priceForMonth) priceForMonthDisplay.value = data.priceForMonth.toString()
    if (data.pricePerSquareMeter) pricePerSquareMeterDisplay.value = data.pricePerSquareMeter.toString()
  } catch (e: unknown) {
    error.value = (e as Error).message || 'Error loading listing'
  } finally {
    loading.value = false
  }
}

// ── Watchers ──

// When property changes, reset scope and load floors/units
watch(selectedRealEstateId, (newId) => {
  listingScope.value = 'entire'
  selectedFloorIds.value = []
  selectedUnitIds.value = []
  floors.value = []
  units.value = []

  if (newId) {
    const prop = myRealEstates.value.find(p => p.id === newId)
    // Load floors if property has a building
    if (prop?.buildingId) {
      loadFloorsForProperty(prop.buildingId)
    }
    // Load units
    loadUnitsForProperty(newId)
  }
})

// When scope changes, reset sub-selections
watch(listingScope, () => {
  selectedFloorIds.value = []
  selectedUnitIds.value = []
})

// ── Navigation ──

function nextStep() {
  if (canProceed.value && currentStep.value < totalSteps) currentStep.value++
}

function prevStep() {
  if (currentStep.value > 1) currentStep.value--
}

function goToStep(step: number) {
  if (step <= currentStep.value) currentStep.value = step
}

// ── Selections ──

function selectProperty(id: string) {
  selectedRealEstateId.value = id
}

function isPropertySelected(id: string): boolean {
  return selectedRealEstateId.value === id
}

function toggleFloor(floorId: string) {
  const idx = selectedFloorIds.value.indexOf(floorId)
  if (idx >= 0) selectedFloorIds.value.splice(idx, 1)
  else selectedFloorIds.value.push(floorId)
}

function isFloorSelected(floorId: string): boolean {
  return selectedFloorIds.value.includes(floorId)
}

function toggleUnit(unitId: string) {
  const idx = selectedUnitIds.value.indexOf(unitId)
  if (idx >= 0) selectedUnitIds.value.splice(idx, 1)
  else selectedUnitIds.value.push(unitId)
}

function isUnitSelected(unitId: string): boolean {
  return selectedUnitIds.value.includes(unitId)
}

function toggleCategory(categoryId: string) {
  const idx = selectedCategoryIds.value.indexOf(categoryId)
  if (idx >= 0) selectedCategoryIds.value.splice(idx, 1)
  else selectedCategoryIds.value.push(categoryId)
}

function isCategorySelected(categoryId: string): boolean {
  return selectedCategoryIds.value.includes(categoryId)
}

function toggleAmenity(amenityId: string) {
  const idx = selectedAmenityIds.value.indexOf(amenityId)
  if (idx >= 0) selectedAmenityIds.value.splice(idx, 1)
  else selectedAmenityIds.value.push(amenityId)
}

function isAmenitySelected(amenityId: string): boolean {
  return selectedAmenityIds.value.includes(amenityId)
}

// ── Submit ──

// ── Translation (silent, debounced) ──

async function translateText(text: string, from: string, to: string): Promise<string> {
  if (!text.trim()) return ''
  try {
    const url = `https://translate.googleapis.com/translate_a/single?client=gtx&sl=${from}&tl=${to}&dt=t&q=${encodeURIComponent(text)}`
    const res = await fetch(url)
    if (!res.ok) return ''
    const data = await res.json()
    if (Array.isArray(data) && Array.isArray(data[0])) {
      return data[0].map((s: string[]) => s[0]).join('')
    }
  } catch { /* silent fail */ }
  return ''
}

async function silentTranslate(sourceLang: 'ru' | 'uz' | 'en') {
  const refs = { ru: descriptionRu, uz: descriptionUz, en: descriptionEn }
  const sourceText = refs[sourceLang].value.trim()
  if (!sourceText) return

  const targets = (['ru', 'uz', 'en'] as const).filter(l => l !== sourceLang && !refs[l].value.trim())
  if (!targets.length) return

  isTranslating.value = true
  const results = await Promise.all(targets.map(t => translateText(sourceText, sourceLang, t)))
  targets.forEach((lang, i) => { if (results[i]) refs[lang].value = results[i] })
  isTranslating.value = false
}

function scheduleTranslate(lang: 'ru' | 'uz' | 'en') {
  if (translateTimer) clearTimeout(translateTimer)
  translateTimer = setTimeout(() => silentTranslate(lang), 1500)
}

watch(descriptionRu, () => { if (activeLang.value === 'ru') scheduleTranslate('ru') })
watch(descriptionUz, () => { if (activeLang.value === 'uz') scheduleTranslate('uz') })
watch(descriptionEn, () => { if (activeLang.value === 'en') scheduleTranslate('en') })

// ── Submit ──

async function handleSubmit() {
  saving.value = true
  error.value = ''

  try {
    const priceForMonth = priceForMonthDisplay.value
      ? parseInt(priceForMonthDisplay.value, 10) : undefined
    const pricePerSquareMeter = pricePerSquareMeterDisplay.value
      ? parseInt(pricePerSquareMeterDisplay.value, 10) : undefined

    // Build description translates
    const descriptionTranslates: { languageId: string; languageShortCode: string; value: string }[] = []
    const ruLang = languages.value.find(l => l.shortCode === 'ru')
    const uzLang = languages.value.find(l => l.shortCode === 'uz')
    const enLang = languages.value.find(l => l.shortCode === 'en')

    if (descriptionRu.value && ruLang) {
      descriptionTranslates.push({ languageId: ruLang.id, languageShortCode: 'ru', value: descriptionRu.value })
    }
    if (descriptionUz.value && uzLang) {
      descriptionTranslates.push({ languageId: uzLang.id, languageShortCode: 'uz', value: descriptionUz.value })
    }
    if (descriptionEn.value && enLang) {
      descriptionTranslates.push({ languageId: enLang.id, languageShortCode: 'en', value: descriptionEn.value })
    }

    const request: CreateListingRequest = {
      realEstateId: selectedRealEstateId.value,
      listingCategoryIds: selectedCategoryIds.value,
      amenityIds: selectedAmenityIds.value.length > 0 ? selectedAmenityIds.value : undefined,
      priceForMonth,
      pricePerSquareMeter,
      description: descriptionRu.value || descriptionUz.value || descriptionEn.value || undefined,
      title: listingTitle.value || undefined,
      descriptionTranslates: descriptionTranslates.length > 0 ? descriptionTranslates : undefined,
      rentalPurposeId: selectedRentalPurposeId.value || undefined,
      minLeaseTerm: selectedMinLeaseTerm.value ?? undefined,
      utilityPaymentType: selectedUtilityPaymentType.value ?? undefined,
      nextAvailableDate: availabilityOption.value === 'now'
        ? new Date().toISOString().split('T')[0]
        : (nextAvailableDate.value || undefined),
    }

    // Add scope-specific IDs
    if (listingScope.value === 'floors' && selectedFloorIds.value.length > 0) {
      request.floorIds = selectedFloorIds.value
    }
    if (listingScope.value === 'units' && selectedUnitIds.value.length > 0) {
      request.unitIds = selectedUnitIds.value
    }

    if (isEditMode.value) {
      await api.updateListing({ id: listingId.value, ...request })
    } else {
      await api.createListing(request)
    }

    router.push('/listings')
  } catch (e: unknown) {
    error.value = (e as Error).message || 'Error saving'
  } finally {
    saving.value = false
  }
}

// ── Lifecycle ──

onMounted(async () => {
  await Promise.all([loadReferenceData(), loadMyRealEstates()])
  if (isEditMode.value) await loadListing()
})
</script>

<template>
  <div class="listing-create-view">
    <div class="page-header">
      <button class="btn-back" @click="router.back()">
        <svg width="20" height="20" viewBox="0 0 14 14" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M8.75 3.5L5.25 7L8.75 10.5" stroke="#1B1B1B" stroke-width="1.67" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </button>
      <h1>{{ isEditMode ? 'Edit Listing' : 'Create Listing' }}</h1>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Loading...</p>
    </div>

    <div v-else class="wizard-container">
      <!-- Step Indicator -->
      <div class="step-indicator">
        <div class="step-tabs">
          <button
            v-for="step in totalSteps"
            :key="step"
            class="step-tab"
            :class="{
              active: step === currentStep,
              completed: step < currentStep,
              clickable: step <= currentStep
            }"
            @click="goToStep(step)"
          >
            {{ step === 1 ? 'Property' :
               step === 2 ? 'Categories' :
               step === 3 ? 'Amenities' :
               step === 4 ? 'Terms' : 'Price' }}
          </button>
        </div>
      </div>

      <!-- Error -->
      <div v-if="error" class="error-message">{{ error }}</div>

      <!-- Step content -->
      <div class="step-content">

        <!-- ═══════ STEP 1: PROPERTY & SCOPE ═══════ -->
        <div v-if="currentStep === 1" class="step-panel">
          <h2>Select Property</h2>
          <p class="step-description">Choose the property you want to list</p>

          <!-- Listing Title -->
          <div class="form-group listing-title-group">
            <label>Listing Title</label>
            <input
              v-model="listingTitle"
              type="text"
              class="listing-title-input"
              placeholder="e.g. Cozy 2-bedroom apartment in city center"
              maxlength="200"
              autocomplete="off"
            />
          </div>

          <!-- Search -->
          <div class="search-pill">
            <svg class="search-icon" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <circle cx="11" cy="11" r="8"/><path d="M21 21l-4.35-4.35"/>
            </svg>
            <input
              v-model="propertySearch"
              type="text"
              placeholder="Search by name, type, or address..."
              autocomplete="off"
            />
          </div>

          <!-- Loading -->
          <div v-if="loadingRealEstates" class="loading-properties">
            <div class="spinner-small"></div>
            <span>Loading your properties...</span>
          </div>

          <!-- Empty -->
          <div v-else-if="myRealEstates.length === 0" class="empty-state">
            <div class="empty-icon">
              <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="#c7c7cc" stroke-width="1.5">
                <path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z"/>
                <polyline points="9 22 9 12 15 12 15 22"/>
              </svg>
            </div>
            <p class="empty-title">No properties yet</p>
            <p class="empty-text">Create a property first, then come back to list it.</p>
            <button class="btn btn-primary" @click="router.push('/realestates/new')">
              Create Property
            </button>
          </div>

          <!-- No search results -->
          <div v-else-if="filteredRealEstates.length === 0" class="empty-state">
            <p class="empty-title">No properties found</p>
            <p class="empty-text">Try adjusting your search query.</p>
          </div>

          <!-- Property cards -->
          <div v-else class="property-grid">
            <div
              v-for="re in filteredRealEstates"
              :key="re.id"
              class="property-card"
              :class="{ selected: isPropertySelected(re.id) }"
              @click="selectProperty(re.id)"
            >
              <div class="card-check">
                <svg v-if="isPropertySelected(re.id)" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="3">
                  <path d="M20 6L9 17l-5-5"/>
                </svg>
              </div>
              <div class="card-body">
                <div class="card-top">
                  <span class="type-badge">{{ re.realEstateTypeName || 'Property' }}</span>
                  <span v-if="re.totalArea" class="area-badge">{{ re.totalArea }} m²</span>
                </div>
                <div class="card-title">{{ re.number || re.address || 'Unnamed Property' }}</div>
                <div class="card-meta">
                  <span v-if="re.address" class="meta-item">
                    <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7z"/><circle cx="12" cy="9" r="2.5"/></svg>
                    {{ re.address }}
                  </span>
                  <span v-if="re.regionName" class="meta-item">
                    {{ re.regionName }}<template v-if="re.districtName">, {{ re.districtName }}</template>
                  </span>
                  <span v-if="re.roomsCount" class="meta-item">{{ re.roomsCount }} rooms</span>
                </div>
              </div>
            </div>
          </div>

          <!-- ── Scope Selector (appears after property is selected) ── -->
          <transition name="slide">
            <div v-if="selectedProperty" class="scope-section">
              <h3 class="scope-title">What do you want to list?</h3>

              <div class="scope-options">
                <button
                  class="scope-option"
                  :class="{ active: listingScope === 'entire' }"
                  @click="listingScope = 'entire'"
                >
                  <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                    <path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z"/>
                    <polyline points="9 22 9 12 15 12 15 22"/>
                  </svg>
                  Entire Property
                </button>
                <button
                  v-if="selectedProperty.buildingId"
                  class="scope-option"
                  :class="{ active: listingScope === 'floors' }"
                  @click="listingScope = 'floors'"
                >
                  <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                    <rect x="3" y="3" width="18" height="18" rx="2"/>
                    <path d="M3 9h18M3 15h18"/>
                  </svg>
                  Specific Floors
                </button>
                <button
                  class="scope-option"
                  :class="{ active: listingScope === 'units' }"
                  @click="listingScope = 'units'"
                >
                  <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                    <rect x="3" y="3" width="7" height="7" rx="1"/>
                    <rect x="14" y="3" width="7" height="7" rx="1"/>
                    <rect x="3" y="14" width="7" height="7" rx="1"/>
                    <rect x="14" y="14" width="7" height="7" rx="1"/>
                  </svg>
                  Specific Units
                </button>
              </div>

              <!-- Floor picker -->
              <div v-if="listingScope === 'floors'" class="sub-selection">
                <div v-if="loadingFloors" class="loading-properties">
                  <div class="spinner-small"></div>
                  <span>Loading floors...</span>
                </div>
                <div v-else-if="floors.length === 0" class="empty-sub">
                  No floors available for this property's building.
                </div>
                <div v-else class="chips-grid">
                  <button
                    v-for="floor in floors"
                    :key="floor.id"
                    class="chip"
                    :class="{ selected: isFloorSelected(floor.id) }"
                    @click="toggleFloor(floor.id)"
                  >
                    <span class="chip-check">
                      <svg v-if="isFloorSelected(floor.id)" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3">
                        <path d="M20 6L9 17l-5-5"/>
                      </svg>
                    </span>
                    <span>Floor {{ floor.number }}</span>
                    <span v-if="floor.totalArea" class="chip-sub">{{ floor.totalArea }} m²</span>
                  </button>
                </div>
                <div v-if="selectedFloorIds.length > 0" class="sub-summary">
                  {{ selectedFloorIds.length }} floor(s) selected
                </div>
              </div>

              <!-- Unit picker -->
              <div v-if="listingScope === 'units'" class="sub-selection">
                <div v-if="loadingUnits" class="loading-properties">
                  <div class="spinner-small"></div>
                  <span>Loading units...</span>
                </div>
                <div v-else-if="units.length === 0" class="empty-sub">
                  No units available for this property.
                </div>
                <div v-else class="chips-grid">
                  <button
                    v-for="unit in units"
                    :key="unit.id"
                    class="chip"
                    :class="{ selected: isUnitSelected(unit.id) }"
                    @click="toggleUnit(unit.id)"
                  >
                    <span class="chip-check">
                      <svg v-if="isUnitSelected(unit.id)" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3">
                        <path d="M20 6L9 17l-5-5"/>
                      </svg>
                    </span>
                    <span>{{ unit.number || `Unit` }}</span>
                    <span v-if="unit.totalArea" class="chip-sub">{{ unit.totalArea }} m²</span>
                    <span v-if="unit.roomsCount" class="chip-sub">{{ unit.roomsCount }} rooms</span>
                  </button>
                </div>
                <div v-if="selectedUnitIds.length > 0" class="sub-summary">
                  {{ selectedUnitIds.length }} unit(s) selected
                </div>
              </div>

              <!-- Ready confirmation -->
              <transition name="fade">
                <div v-if="canProceed" class="selection-confirm">
                  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><path d="M20 6L9 17l-5-5"/></svg>
                  <span>
                    <strong>{{ selectedProperty.number || selectedProperty.address || 'Property' }}</strong>
                    <template v-if="listingScope === 'entire'"> — entire property</template>
                    <template v-else-if="listingScope === 'floors'"> — {{ selectedFloorIds.length }} floor(s)</template>
                    <template v-else-if="listingScope === 'units'"> — {{ selectedUnitIds.length }} unit(s)</template>
                     selected
                  </span>
                </div>
              </transition>
            </div>
          </transition>
        </div>

        <!-- ═══════ STEP 2: CATEGORIES ═══════ -->
        <div v-if="currentStep === 2" class="step-panel">
          <h2>Select Categories</h2>
          <p class="step-description">Specify the type of offer</p>

          <div class="categories-grid">
            <div
              v-for="cat in categories"
              :key="cat.id"
              class="category-card"
              :class="{ selected: isCategorySelected(cat.id) }"
              @click="toggleCategory(cat.id)"
            >
              <div class="category-check">
                <span v-if="isCategorySelected(cat.id)">✓</span>
              </div>
              <img v-if="cat.iconUrl" :src="cat.iconUrl" class="category-icon" alt="" />
              <div class="category-name">{{ cat.name }}</div>
            </div>
          </div>

          <div class="selection-summary" v-if="selectedCategoryIds.length > 0">
            Selected: {{ selectedCategoryIds.length }} category(ies)
          </div>
        </div>

        <!-- ═══════ STEP 3: AMENITIES ═══════ -->
        <div v-if="currentStep === 3" class="step-panel">
          <h2>Select Amenities</h2>
          <p class="step-description">Choose the amenities available with this listing (optional)</p>

          <div class="categories-grid">
            <div
              v-for="amenity in amenities"
              :key="amenity.id"
              class="category-card"
              :class="{ selected: isAmenitySelected(amenity.id) }"
              @click="toggleAmenity(amenity.id)"
            >
              <div class="category-check">
                <span v-if="isAmenitySelected(amenity.id)">✓</span>
              </div>
              <img v-if="amenity.iconUrl" :src="amenity.iconUrl" class="category-icon" alt="" />
              <div class="category-name">{{ amenity.name }}</div>
            </div>
          </div>

          <div class="selection-summary" v-if="selectedAmenityIds.length > 0">
            Selected: {{ selectedAmenityIds.length }} amenity(ies)
          </div>
        </div>

        <!-- ═══════ STEP 4: RENTAL CONDITIONS ═══════ -->
        <div v-if="currentStep === 4" class="step-panel">
          <h2>Условия сдачи</h2>
          <p class="step-description">Укажите условия аренды</p>

          <div class="rental-conditions-form">
            <!-- Rental Terms -->
            <div class="rc-section">
              <h3 class="rc-section-title">Условия аренды</h3>

              <div class="rc-field">
                <select v-model="selectedRentalPurposeId" class="rc-select">
                  <option value="" disabled>Rental purpose</option>
                  <option v-for="rp in rentalPurposes" :key="rp.id" :value="rp.id">{{ rp.name }}</option>
                </select>
                <svg class="rc-select-chevron" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M6 9l6 6 6-6"/></svg>
              </div>

              <div class="rc-field">
                <select v-model="selectedMinLeaseTerm" class="rc-select">
                  <option :value="null" disabled>Минимальный срок аренды</option>
                  <option v-for="opt in minLeaseTermOptions" :key="opt.value" :value="opt.value">{{ opt.label }}</option>
                </select>
                <svg class="rc-select-chevron" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M6 9l6 6 6-6"/></svg>
              </div>
            </div>

            <!-- Utility Payment -->
            <div class="rc-section">
              <h3 class="rc-section-title">Коммунальные услуги</h3>
              <div class="rc-radio-group">
                <label class="rc-radio" @click="selectedUtilityPaymentType = 0">
                  <span class="rc-radio-circle" :class="{ active: selectedUtilityPaymentType === 0 }"></span>
                  <span>От владельца</span>
                </label>
                <label class="rc-radio" @click="selectedUtilityPaymentType = 1">
                  <span class="rc-radio-circle" :class="{ active: selectedUtilityPaymentType === 1 }"></span>
                  <span>От арендатора</span>
                </label>
                <label class="rc-radio" @click="selectedUtilityPaymentType = 2">
                  <span class="rc-radio-circle" :class="{ active: selectedUtilityPaymentType === 2 }"></span>
                  <span>Договорная</span>
                </label>
              </div>
            </div>

            <!-- Availability -->
            <div class="rc-section">
              <h3 class="rc-section-title">Доступен для аренды</h3>
              <div class="rc-radio-group">
                <label class="rc-radio" @click="availabilityOption = 'now'">
                  <span class="rc-radio-circle" :class="{ active: availabilityOption === 'now' }"></span>
                  <span>Сейчас</span>
                </label>
                <label class="rc-radio" @click="availabilityOption = 'date'">
                  <span class="rc-radio-circle" :class="{ active: availabilityOption === 'date' }"></span>
                  <span>Выбрать дату</span>
                </label>
              </div>
              <div v-if="availabilityOption === 'date'" class="rc-date-field">
                <input
                  type="date"
                  v-model="nextAvailableDate"
                  class="rc-date-input"
                  :min="new Date().toISOString().split('T')[0]"
                />
              </div>
            </div>
          </div>
        </div>

        <!-- ═══════ STEP 5: PRICING ═══════ -->
        <div v-if="currentStep === 5" class="step-panel">
          <h2>Set Price & Description</h2>
          <p class="step-description">Specify pricing and description (optional)</p>

          <div class="form-grid">
            <div class="form-group">
              <label>Price per Month (UZS)</label>
              <input
                type="number"
                v-model="priceForMonthDisplay"
                placeholder="e.g.: 5000000"
                min="0"
              />
            </div>

            <div class="form-group">
              <label>Price per m² (UZS)</label>
              <input
                type="number"
                v-model="pricePerSquareMeterDisplay"
                placeholder="e.g.: 50000"
                min="0"
              />
            </div>
          </div>

          <!-- Description -->
          <div class="desc-section">
            <label class="form-label-clean">Описание</label>

            <div class="seg-control">
              <button type="button" class="seg-item" :class="{ active: activeLang === 'ru' }" @click="activeLang = 'ru'">RU</button>
              <button type="button" class="seg-item" :class="{ active: activeLang === 'uz' }" @click="activeLang = 'uz'">UZ</button>
              <button type="button" class="seg-item" :class="{ active: activeLang === 'en' }" @click="activeLang = 'en'">EN</button>
            </div>

            <div class="desc-input-wrap" :class="{ translating: isTranslating }">
              <textarea
                v-show="activeLang === 'ru'"
                v-model="descriptionRu"
                class="desc-input"
                placeholder="Введите описание..."
                rows="5"
                maxlength="2000"
              ></textarea>
              <textarea
                v-show="activeLang === 'uz'"
                v-model="descriptionUz"
                class="desc-input"
                placeholder="Tavsif kiriting..."
                rows="5"
                maxlength="2000"
              ></textarea>
              <textarea
                v-show="activeLang === 'en'"
                v-model="descriptionEn"
                class="desc-input"
                placeholder="Enter description..."
                rows="5"
                maxlength="2000"
              ></textarea>
            </div>
          </div>
        </div>
      </div>

      <!-- Navigation buttons -->
      <div class="wizard-actions">
        <button v-if="currentStep > 1" class="btn btn-secondary" @click="prevStep">
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
          :disabled="saving || !selectedRealEstateId || !selectedCategoryIds.length"
          @click="handleSubmit"
        >
          {{ saving ? 'Saving...' : (isEditMode ? 'Save' : 'Create Listing') }}
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.listing-create-view {
  min-height: 100vh;
  background: #FFFFFF;
  padding: 24px;
  font-family: -apple-system, BlinkMacSystemFont, 'SF Pro Display', 'SF Pro Text', 'Helvetica Neue', Arial, sans-serif;
}

.page-header {
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
  justify-content: center;
  align-items: center;
  background: rgba(27, 27, 27, 0.04);
  border: none;
  border-radius: 100px;
  cursor: pointer;
  transition: all 0.2s;
}
.btn-back:hover { background: rgba(27, 27, 27, 0.08); }

.page-header h1 {
  font-size: 24px;
  font-weight: 700;
  color: #1B1B1B;
  margin: 0;
}

.loading-state { text-align: center; padding: 60px; }

.spinner {
  width: 40px; height: 40px;
  border: 3px solid rgba(27,27,27,0.08);
  border-top-color: #FF5B3C;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 16px;
}

@keyframes spin { to { transform: rotate(360deg); } }

/* ── Steps ── */
.step-indicator { display: flex; justify-content: center; margin-bottom: 32px; }

.step-tabs {
  display: flex; padding: 4px;
  background: #F6F6F6; border-radius: 100px;
}

.step-tab {
  padding: 12px 20px; font-size: 14px; font-weight: 500;
  color: #19191C; background: transparent; border: none;
  border-radius: 100px; cursor: default;
  transition: all 0.2s; white-space: nowrap;
}
.step-tab.clickable { cursor: pointer; }
.step-tab.clickable:hover:not(.active) { background: rgba(25,25,28,0.05); }
.step-tab.active { background: #19191C; color: #FFF; font-weight: 600; }

/* ── Wizard ── */
.wizard-container { max-width: 900px; margin: 0 auto; padding: 0 32px; }

.error-message {
  background: #fef2f2; color: #dc2626;
  padding: 12px 16px; border-radius: 12px; margin-bottom: 16px; font-size: 14px;
}

.step-content { min-height: 300px; }

.step-panel h2 {
  font-size: 22px; font-weight: 700; color: #1d1d1f;
  margin: 0 0 4px; letter-spacing: -0.3px;
}
.step-description {
  color: #86868b; font-size: 15px; margin: 0 0 24px;
}

/* ── Search ── */
.search-pill { position: relative; margin-bottom: 24px; }

.search-icon {
  position: absolute; left: 18px; top: 50%;
  transform: translateY(-50%); color: #86868b; pointer-events: none;
}

.search-pill input {
  width: 100%; height: 48px; padding: 0 18px 0 46px;
  border: 1.5px solid rgba(0,0,0,0.08); border-radius: 999px;
  font-size: 15px; color: #1d1d1f; background: #f5f5f7;
  transition: all 0.2s; font-family: inherit;
}
.search-pill input::placeholder { color: #c7c7cc; }
.search-pill input:hover { background: #efefef; border-color: rgba(0,0,0,0.12); }
.search-pill input:focus {
  outline: none; border-color: #FF5B3C; background: #fff;
  box-shadow: 0 0 0 3px rgba(255,91,60,0.1);
}

/* ── Loading ── */
.loading-properties {
  display: flex; align-items: center; justify-content: center;
  gap: 10px; padding: 48px 20px; color: #86868b; font-size: 14px;
}

.spinner-small {
  width: 20px; height: 20px;
  border: 2px solid rgba(27,27,27,0.08);
  border-top-color: #FF5B3C; border-radius: 50%;
  animation: spin 1s linear infinite;
}

/* ── Empty ── */
.empty-state {
  text-align: center; padding: 48px 24px;
  background: #f9fafb; border-radius: 20px;
  border: 1.5px dashed rgba(0,0,0,0.08);
}
.empty-icon { margin-bottom: 16px; }
.empty-title { font-size: 17px; font-weight: 600; color: #1d1d1f; margin: 0 0 6px; }
.empty-text { font-size: 14px; color: #86868b; margin: 0 0 20px; }

/* ── Property Grid ── */
.property-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 14px;
}

.property-card {
  display: flex; gap: 14px; padding: 18px 20px;
  border: 2px solid rgba(0,0,0,0.06); border-radius: 18px;
  cursor: pointer; transition: all 0.2s; background: #fff;
}
.property-card:hover {
  border-color: rgba(0,0,0,0.12);
  box-shadow: 0 2px 12px rgba(0,0,0,0.04);
}
.property-card.selected {
  border-color: #FF5B3C;
  background: rgba(255,91,60,0.03);
  box-shadow: 0 0 0 3px rgba(255,91,60,0.08);
}

.card-check {
  width: 26px; height: 26px; border-radius: 8px;
  border: 2px solid rgba(0,0,0,0.1);
  display: flex; align-items: center; justify-content: center;
  flex-shrink: 0; margin-top: 2px; transition: all 0.2s;
}
.property-card.selected .card-check {
  background: #FF5B3C; border-color: #FF5B3C;
}

.card-body { flex: 1; min-width: 0; }
.card-top { display: flex; align-items: center; gap: 8px; margin-bottom: 6px; }

.type-badge {
  display: inline-flex; padding: 3px 10px;
  background: rgba(99,102,241,0.08); color: #6366F1;
  font-size: 11px; font-weight: 600; border-radius: 999px;
  letter-spacing: 0.2px; text-transform: uppercase;
}
.area-badge { font-size: 12px; color: #86868b; font-weight: 500; }

.card-title {
  font-size: 16px; font-weight: 600; color: #1d1d1f; margin-bottom: 6px;
  white-space: nowrap; overflow: hidden; text-overflow: ellipsis;
}

.card-meta { display: flex; flex-wrap: wrap; gap: 12px; }
.meta-item {
  display: inline-flex; align-items: center; gap: 4px;
  font-size: 13px; color: #86868b;
}
.meta-item svg { flex-shrink: 0; }

/* ── Scope Section ── */
.scope-section {
  margin-top: 28px;
  padding-top: 24px;
  border-top: 1px solid rgba(0,0,0,0.06);
}

.scope-title {
  font-size: 16px; font-weight: 600; color: #1d1d1f;
  margin: 0 0 14px;
}

.scope-options {
  display: flex; gap: 10px; flex-wrap: wrap;
}

.scope-option {
  display: inline-flex; align-items: center; gap: 8px;
  padding: 10px 20px;
  border: 1.5px solid rgba(0,0,0,0.08);
  border-radius: 999px;
  background: #f5f5f7;
  color: #1d1d1f;
  font-size: 14px; font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  font-family: inherit;
}
.scope-option:hover {
  border-color: rgba(0,0,0,0.15);
  background: #efefef;
}
.scope-option.active {
  border-color: #FF5B3C;
  background: rgba(255,91,60,0.06);
  color: #FF5B3C;
}
.scope-option svg {
  stroke: currentColor;
}

/* ── Sub-selection (floors/units) ── */
.sub-selection {
  margin-top: 20px;
}

.chips-grid {
  display: flex; flex-wrap: wrap; gap: 10px;
}

.chip {
  display: inline-flex; align-items: center; gap: 8px;
  padding: 10px 16px;
  border: 1.5px solid rgba(0,0,0,0.08);
  border-radius: 14px;
  background: #fff;
  color: #1d1d1f;
  font-size: 14px; font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  font-family: inherit;
}
.chip:hover {
  border-color: rgba(0,0,0,0.15);
  background: #fafafa;
}
.chip.selected {
  border-color: #FF5B3C;
  background: rgba(255,91,60,0.04);
}

.chip-check {
  width: 18px; height: 18px;
  border-radius: 5px;
  border: 1.5px solid rgba(0,0,0,0.12);
  display: flex; align-items: center; justify-content: center;
  flex-shrink: 0;
  transition: all 0.2s;
}
.chip.selected .chip-check {
  background: #FF5B3C; border-color: #FF5B3C;
  color: white;
}

.chip-sub {
  font-size: 12px; color: #86868b; font-weight: 400;
}

.sub-summary {
  margin-top: 12px;
  font-size: 13px; font-weight: 500; color: #FF5B3C;
}

.empty-sub {
  padding: 20px; text-align: center; color: #86868b;
  background: #f9fafb; border-radius: 12px; font-size: 14px;
}

/* ── Selection confirmation ── */
.selection-confirm {
  display: flex; align-items: center; gap: 8px;
  margin-top: 20px; padding: 12px 20px;
  background: rgba(52,199,89,0.06);
  color: #34C759;
  font-size: 14px; font-weight: 500;
  border-radius: 999px;
}
.selection-confirm strong { color: #1d1d1f; }

/* ── Categories ── */
.categories-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 12px;
}

.category-card {
  display: flex; gap: 12px; align-items: center;
  padding: 16px;
  border: 2px solid rgba(0,0,0,0.06); border-radius: 14px;
  cursor: pointer; transition: all 0.2s;
}
.category-card:hover { border-color: rgba(0,0,0,0.12); }
.category-card.selected {
  border-color: #FF5B3C;
  background: rgba(255,91,60,0.04);
}

.category-check {
  width: 24px; height: 24px; border-radius: 6px;
  border: 2px solid rgba(0,0,0,0.1);
  display: flex; align-items: center; justify-content: center;
  flex-shrink: 0; font-size: 14px; color: white;
  transition: all 0.2s;
}
.category-card.selected .category-check {
  background: #FF5B3C; border-color: #FF5B3C;
}

.category-name { font-weight: 500; color: #1d1d1f; }
.category-icon { width: 24px; height: 24px; object-fit: contain; }

.selection-summary {
  margin-top: 16px; padding: 12px 20px;
  background: rgba(255,91,60,0.06); border-radius: 999px;
  color: #FF5B3C; font-weight: 500; font-size: 14px;
  display: inline-block;
}

/* ── Form ── */
.form-grid {
  display: grid; grid-template-columns: repeat(2, 1fr); gap: 20px;
}

.form-group { display: flex; flex-direction: column; gap: 6px; }

.form-group label {
  font-size: 13px; font-weight: 600; color: #1d1d1f; padding-left: 4px;
}

.form-group input {
  padding: 12px 16px; border: 1.5px solid rgba(0,0,0,0.08);
  border-radius: 14px; font-size: 15px; font-family: inherit;
  color: #1d1d1f; background: #f5f5f7; transition: all 0.2s;
}
.form-group input:focus {
  outline: none; border-color: #FF5B3C; background: #fff;
  box-shadow: 0 0 0 3px rgba(255,91,60,0.1);
}

.description-textarea {
  width: 100%; padding: 12px 16px;
  border: 1.5px solid rgba(0,0,0,0.08); border-radius: 14px;
  font-size: 15px; font-family: inherit; color: #1d1d1f;
  background: #f5f5f7; resize: vertical; min-height: 100px;
  transition: all 0.2s;
}
.description-textarea:focus {
  outline: none; border-color: #FF5B3C; background: #fff;
  box-shadow: 0 0 0 3px rgba(255,91,60,0.1);
}

.price-hint {
  margin-top: 16px; padding: 12px 20px;
  background: rgba(255,91,60,0.04); border-radius: 14px;
  color: #1B1B1B; font-size: 14px;
}
.price-hint p { margin: 0; }

/* ── Description (Apple style) ── */
.desc-section {
  margin-top: 28px;
}

.form-label-clean {
  display: block;
  font-size: 13px;
  font-weight: 600;
  color: #1d1d1f;
  padding-left: 4px;
  margin-bottom: 8px;
}

.seg-control {
  display: inline-flex;
  background: #f5f5f7;
  border-radius: 9px;
  padding: 2px;
  margin-bottom: 10px;
}

.seg-item {
  padding: 6px 18px;
  font-size: 13px;
  font-weight: 600;
  font-family: inherit;
  color: #86868b;
  background: transparent;
  border: none;
  border-radius: 7px;
  cursor: pointer;
  transition: all 0.2s cubic-bezier(.4,0,.2,1);
}
.seg-item:hover:not(.active) {
  color: #6e6e73;
}
.seg-item.active {
  background: #fff;
  color: #1d1d1f;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1), 0 1px 2px rgba(0,0,0,0.06);
}

.desc-input-wrap {
  transition: opacity 0.3s ease;
}
.desc-input-wrap.translating {
  opacity: 0.6;
}

.desc-input {
  width: 100%;
  padding: 12px 16px;
  border: 1.5px solid rgba(0,0,0,0.08);
  border-radius: 14px;
  font-size: 15px;
  font-family: inherit;
  color: #1d1d1f;
  background: #f5f5f7;
  resize: vertical;
  min-height: 120px;
  line-height: 1.55;
  transition: all 0.2s;
}
.desc-input:focus {
  outline: none;
  border-color: #FF5B3C;
  background: #fff;
  box-shadow: 0 0 0 3px rgba(255,91,60,0.08);
}
.desc-input::placeholder {
  color: #c7c7cc;
}

/* ── Actions ── */
.wizard-actions {
  display: flex; align-items: center; margin-top: 32px;
  padding-top: 24px; border-top: 1px solid rgba(0,0,0,0.06);
}

.spacer { flex: 1; }

.btn {
  display: inline-flex; align-items: center; gap: 6px;
  padding: 12px 24px; border-radius: 100px;
  font-weight: 600; font-size: 14px;
  cursor: pointer; border: none;
  transition: all 0.2s; font-family: inherit;
}
.btn:disabled { opacity: 0.4; cursor: not-allowed; }
.btn-primary { background: #1B1B1B; color: white; }
.btn-primary:hover:not(:disabled) { background: #333; }
.btn-secondary { background: #F7F7F7; color: #1B1B1B; }
.btn-secondary:hover:not(:disabled) { background: #EBEBEB; }

/* ── Rental Conditions (Step 5) ── */
.rental-conditions-form {
  max-width: 480px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  gap: 32px;
}

.rc-section-title {
  font-size: 15px;
  font-weight: 600;
  color: #1B1B1B;
  margin: 0 0 16px;
}

.rc-field {
  position: relative;
  margin-bottom: 12px;
}

.rc-select {
  width: 100%;
  height: 56px;
  padding: 0 40px 0 20px;
  border: none;
  border-radius: 28px;
  font-size: 14px;
  color: #1B1B1B;
  background: #F4F4F4;
  appearance: none;
  -webkit-appearance: none;
  cursor: pointer;
  font-family: inherit;
  transition: background 0.2s;
}

.rc-select:focus {
  outline: none;
  background: #EBEBEB;
}

.rc-select option[disabled] {
  color: rgba(27, 27, 27, 0.35);
}

.rc-select-chevron {
  position: absolute;
  right: 14px;
  top: 50%;
  transform: translateY(-50%);
  pointer-events: none;
  color: rgba(27, 27, 27, 0.4);
}

.rc-radio-group {
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.rc-radio {
  display: flex;
  align-items: center;
  gap: 12px;
  cursor: pointer;
  font-size: 14px;
  color: #1B1B1B;
  user-select: none;
}

.rc-radio-circle {
  width: 20px;
  height: 20px;
  border-radius: 50%;
  border: 2px solid rgba(0, 0, 0, 0.2);
  flex-shrink: 0;
  position: relative;
  transition: border-color 0.2s;
}

.rc-radio-circle.active {
  border-color: #1B1B1B;
}

.rc-radio-circle.active::after {
  content: '';
  position: absolute;
  top: 4px;
  left: 4px;
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: #1B1B1B;
}

.rc-date-field {
  margin-top: 12px;
}

.rc-date-input {
  width: 100%;
  height: 48px;
  padding: 0 16px;
  border: 1px solid rgba(0, 0, 0, 0.12);
  border-radius: 12px;
  font-size: 14px;
  color: #1B1B1B;
  font-family: inherit;
  transition: border-color 0.2s;
}

.rc-date-input:focus {
  outline: none;
  border-color: #1B1B1B;
}

.rc-footer {
  display: flex;
  justify-content: center;
  gap: 12px;
  margin-top: 40px;
  padding-top: 24px;
}

.rc-btn-draft {
  padding: 12px 24px;
  border: 1.5px solid #1B1B1B;
  background: transparent;
  border-radius: 100px;
  font-size: 14px;
  font-weight: 600;
  color: #1B1B1B;
  cursor: pointer;
  font-family: inherit;
  transition: all 0.2s;
}

.rc-btn-draft:hover {
  background: rgba(27, 27, 27, 0.04);
}

.rc-btn-continue {
  padding: 12px 32px;
  border: none;
  background: #1B1B1B;
  border-radius: 100px;
  font-size: 14px;
  font-weight: 600;
  color: #fff;
  cursor: pointer;
  font-family: inherit;
  transition: all 0.2s;
}

.rc-btn-continue:hover {
  background: #333;
}

/* ── Transitions ── */
.fade-enter-active, .fade-leave-active { transition: opacity 0.25s ease; }
.fade-enter-from, .fade-leave-to { opacity: 0; }

.slide-enter-active { transition: all 0.3s ease-out; }
.slide-leave-active { transition: all 0.2s ease-in; }
.slide-enter-from { opacity: 0; transform: translateY(12px); }
.slide-leave-to { opacity: 0; transform: translateY(-8px); }

/* ── Responsive ── */
@media (max-width: 680px) {
  .wizard-container { padding: 0 16px; }
  .property-grid { grid-template-columns: 1fr; }
  .categories-grid { grid-template-columns: 1fr; }
  .form-grid { grid-template-columns: 1fr; }
  .step-tabs { overflow-x: auto; -webkit-overflow-scrolling: touch; }
  .step-tab { padding: 10px 16px; font-size: 13px; }
  .scope-options { flex-direction: column; }
}

/* ── Listing Title ── */
.listing-title-group {
  margin-bottom: 20px;
}
.listing-title-group label {
  display: block;
  font-size: 13px;
  font-weight: 600;
  color: #6e6e73;
  text-transform: uppercase;
  letter-spacing: 0.04em;
  margin-bottom: 8px;
}
.listing-title-input {
  width: 100%;
  padding: 14px 16px;
  font-size: 16px;
  font-weight: 500;
  color: #1B1B1B;
  background: #F6F6F6;
  border: 2px solid transparent;
  border-radius: 14px;
  outline: none;
  transition: all 0.2s ease;
  box-sizing: border-box;
  font-family: inherit;
}
.listing-title-input::placeholder {
  color: #a1a1a6;
  font-weight: 400;
}
.listing-title-input:focus {
  background: #fff;
  border-color: #FF5B3C;
  box-shadow: 0 0 0 4px rgba(255, 91, 60, 0.08);
}
</style>
