<script setup lang="ts">
import { ref, watch, onMounted, onUnmounted } from 'vue'
import { api } from '@/services/api'
import type { PropertyTypeConfig } from '@/configs/propertyTypeFlows'
import GoogleMap from '@/components/GoogleMap.vue'

const props = defineProps<{
  propertyType: PropertyTypeConfig
  modelValue: {
    buildingId?: string
    floorId?: string
    floorNumber?: number
    regionId?: string
    districtId?: string
    address?: string
    latitude?: number
    longitude?: number
    buildingNumber?: string
  }
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: typeof props.modelValue): void
}>()

// Reference data
const regions = ref<any[]>([])
const districts = ref<any[]>([])
const loading = ref(false)
const showMap = ref(false)

// Address autocomplete
const addressInput = ref<HTMLInputElement | null>(null)
const suggestions = ref<google.maps.places.AutocompletePrediction[]>([])
const showSuggestions = ref(false)
const searchLoading = ref(false)
let autocompleteService: google.maps.places.AutocompleteService | null = null
let placesService: google.maps.places.PlacesService | null = null
let sessionToken: google.maps.places.AutocompleteSessionToken | null = null
let debounceTimer: ReturnType<typeof setTimeout> | null = null

// Load Google Maps Script (reuse from GoogleMap component)
function loadGoogleMapsScript(): Promise<void> {
  return new Promise((resolve, reject) => {
    if (window.google?.maps) {
      resolve()
      return
    }
    const apiKey = import.meta.env.VITE_GOOGLE_MAP_API_KEY
    if (!apiKey) {
      reject(new Error('Google Maps API key not found'))
      return
    }
    const script = document.createElement('script')
    script.src = `https://maps.googleapis.com/maps/api/js?key=${apiKey}&libraries=places`
    script.async = true
    script.defer = true
    script.onload = () => resolve()
    script.onerror = () => reject(new Error('Failed to load Google Maps'))
    document.head.appendChild(script)
  })
}

function initAutocomplete() {
  if (!window.google?.maps?.places) return
  autocompleteService = new google.maps.places.AutocompleteService()
  // Create a dummy div for PlacesService (needed for getDetails)
  const dummyDiv = document.createElement('div')
  placesService = new google.maps.places.PlacesService(dummyDiv)
  sessionToken = new google.maps.places.AutocompleteSessionToken()
}

function onAddressInput(event: Event) {
  const value = (event.target as HTMLInputElement).value
  updateField('address', value)

  if (debounceTimer) clearTimeout(debounceTimer)

  if (!value || value.length < 2) {
    suggestions.value = []
    showSuggestions.value = false
    return
  }

  searchLoading.value = true
  debounceTimer = setTimeout(() => {
    searchAddress(value)
  }, 300)
}

async function searchAddress(query: string) {
  if (!autocompleteService) {
    searchLoading.value = false
    return
  }

  try {
    const result = await autocompleteService.getPlacePredictions({
      input: query,
      componentRestrictions: { country: 'uz' },
      types: ['geocode', 'establishment'],
      sessionToken: sessionToken || undefined
    })
    suggestions.value = result?.predictions || []
    showSuggestions.value = suggestions.value.length > 0
  } catch (e) {
    console.error('Autocomplete error:', e)
    suggestions.value = []
    showSuggestions.value = false
  } finally {
    searchLoading.value = false
  }
}

function selectSuggestion(prediction: google.maps.places.AutocompletePrediction) {
  if (!placesService) return

  placesService.getDetails(
    {
      placeId: prediction.place_id,
      fields: ['geometry', 'formatted_address', 'name'],
      sessionToken: sessionToken || undefined
    },
    (place, status) => {
      if (status === google.maps.places.PlacesServiceStatus.OK && place?.geometry?.location) {
        const lat = place.geometry.location.lat()
        const lng = place.geometry.location.lng()
        const address = place.formatted_address || place.name || prediction.description

        emit('update:modelValue', {
          ...props.modelValue,
          address,
          latitude: lat,
          longitude: lng
        })

        showMap.value = true
      }

      // Reset session token after selection
      sessionToken = new google.maps.places.AutocompleteSessionToken()
    }
  )

  suggestions.value = []
  showSuggestions.value = false
}

function closeSuggestions() {
  // Delay to allow click to register
  setTimeout(() => {
    showSuggestions.value = false
  }, 200)
}

// Load reference data on mount
async function loadReferenceData() {
  loading.value = true
  try {
    const regionsRes = await api.getRegions({ pageSize: 50 })
    regions.value = regionsRes?.items || regionsRes || []
  } catch (e) {
    console.error('Failed to load reference data:', e)
  } finally {
    loading.value = false
  }
}

// Watch region to load districts
watch(() => props.modelValue.regionId, async (regionId) => {
  if (regionId) {
    try {
      const res = await api.getDistricts({ regionId, pageSize: 50 })
      districts.value = res?.items || res || []
    } catch (e) {
      console.error('Failed to load districts:', e)
    }
  } else {
    districts.value = []
  }
}, { immediate: true })

function updateField<K extends keyof typeof props.modelValue>(field: K, value: typeof props.modelValue[K]) {
  emit('update:modelValue', { ...props.modelValue, [field]: value })
}

function onLocationUpdate(location: { lat: number; lng: number; address?: string }) {
  emit('update:modelValue', {
    ...props.modelValue,
    latitude: location.lat,
    longitude: location.lng,
    address: location.address || props.modelValue.address
  })
}

onMounted(async () => {
  loadReferenceData()
  try {
    await loadGoogleMapsScript()
    initAutocomplete()
  } catch (e) {
    console.error('Failed to load Google Maps for autocomplete:', e)
  }
})

onUnmounted(() => {
  if (debounceTimer) clearTimeout(debounceTimer)
})
</script>

<template>
  <div class="location-step">
    <h2 class="section-title">Location</h2>

    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Loading locations...</p>
    </div>

    <div v-else class="form-section">
        <!-- Region -->
        <div class="fields-row">
          <div class="field">
            <label class="field-label">Region</label>
            <div class="pill-select">
              <select
                :value="modelValue.regionId"
                @change="updateField('regionId', ($event.target as HTMLSelectElement).value || undefined)"
              >
                <option value="">Select region...</option>
                <option v-for="r in regions" :key="r.id" :value="r.id">
                  {{ r.name }}
                </option>
              </select>
              <svg class="chevron" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M6 9l6 6 6-6"/></svg>
            </div>
          </div>
        </div>

        <!-- District -->
        <div class="fields-row">
          <div class="field">
            <label class="field-label">District</label>
            <div class="pill-select" :class="{ disabled: !modelValue.regionId }">
              <select
                :value="modelValue.districtId"
                :disabled="!modelValue.regionId"
                @change="updateField('districtId', ($event.target as HTMLSelectElement).value || undefined)"
              >
                <option value="">Select district...</option>
                <option v-for="d in districts" :key="d.id" :value="d.id">
                  {{ d.name }}
                </option>
              </select>
              <svg class="chevron" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M6 9l6 6 6-6"/></svg>
            </div>
          </div>
        </div>

        <!-- Address + Map button -->
        <div class="fields-row">
          <div class="field full">
            <label class="field-label">Street Address</label>
            <div class="pill-input-row">
              <div class="pill-input address-autocomplete">
                <svg class="input-icon" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7z"/><circle cx="12" cy="9" r="2.5"/></svg>
                <input
                  ref="addressInput"
                  type="text"
                  :value="modelValue.address"
                  @input="onAddressInput"
                  @focus="showSuggestions = suggestions.length > 0"
                  @blur="closeSuggestions"
                  placeholder="Search address..."
                  autocomplete="off"
                />
                <div v-if="searchLoading" class="search-spinner"></div>

                <!-- Suggestions dropdown -->
                <transition name="dropdown">
                  <div v-if="showSuggestions && suggestions.length > 0" class="suggestions-dropdown">
                    <button
                      v-for="s in suggestions"
                      :key="s.place_id"
                      type="button"
                      class="suggestion-item"
                      @mousedown.prevent="selectSuggestion(s)"
                    >
                      <svg class="suggestion-icon" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7z"/><circle cx="12" cy="9" r="2.5"/></svg>
                      <div class="suggestion-text">
                        <span class="suggestion-main">{{ s.structured_formatting.main_text }}</span>
                        <span class="suggestion-secondary">{{ s.structured_formatting.secondary_text }}</span>
                      </div>
                    </button>
                  </div>
                </transition>
              </div>
              <button
                type="button"
                class="pill-btn"
                :class="{ active: showMap }"
                @click="showMap = !showMap"
              >
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                  <path d="M1 6v16l7-4 8 4 7-4V2l-7 4-8-4-7 4z"/>
                  <path d="M8 2v16M16 6v16"/>
                </svg>
                Map
              </button>
            </div>
          </div>
        </div>

        <!-- Map -->
        <transition name="map-slide">
          <div v-if="showMap" class="map-area">
            <div class="map-wrapper">
              <GoogleMap
                :latitude="modelValue.latitude"
                :longitude="modelValue.longitude"
                :editable="true"
                :enable-reverse-geocoding="true"
                height="260px"
                @update:location="onLocationUpdate"
              />
            </div>
            <p class="map-hint">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><path d="M12 16v-4M12 8h.01"/></svg>
              Click on the map to select a precise location
            </p>
          </div>
        </transition>

        <!-- Coordinates Pill -->
        <transition name="fade">
          <div v-if="modelValue.latitude && modelValue.longitude" class="coords-pill">
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><path d="M20 6L9 17l-5-5"/></svg>
            <span>{{ modelValue.latitude.toFixed(6) }}, {{ modelValue.longitude.toFixed(6) }}</span>
          </div>
        </transition>
    </div>
  </div>
</template>

<style scoped>
.location-step {
  padding: 8px 0 32px;
  font-family: -apple-system, BlinkMacSystemFont, 'SF Pro Display', 'SF Pro Text', 'Helvetica Neue', Arial, sans-serif;
}

.section-title {
  font-size: 22px;
  font-weight: 700;
  letter-spacing: -0.3px;
  color: #1d1d1f;
  margin: 0 0 28px 0;
  max-width: 640px;
  margin-left: auto;
  margin-right: auto;
}

/* ── Loading ── */
.loading-state {
  text-align: center;
  padding: 60px 20px;
}

.loading-state p {
  color: #86868b;
  font-size: 14px;
  margin: 0;
}

.spinner {
  width: 32px;
  height: 32px;
  border: 2.5px solid rgba(0,0,0,0.06);
  border-top-color: #FF5B3C;
  border-radius: 50%;
  animation: spin 0.8s ease-in-out infinite;
  margin: 0 auto 14px;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

/* ── Form ── */
.form-section {
  max-width: 640px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

/* ── Fields Row ── */
.fields-row {
  display: flex;
  gap: 12px;
  align-items: flex-end;
}

.field {
  flex: 1;
  min-width: 0;
}

.field.full {
  flex: 1;
}

.field-label {
  display: block;
  font-size: 13px;
  font-weight: 600;
  color: #1d1d1f;
  letter-spacing: -0.08px;
  margin-bottom: 6px;
  padding-left: 16px;
}

/* ── Pill Select ── */
.pill-select {
  position: relative;
}

.pill-select select {
  width: 100%;
  height: 46px;
  padding: 0 40px 0 18px;
  border: 1.5px solid rgba(0,0,0,0.08);
  border-radius: 999px;
  font-size: 15px;
  color: #1d1d1f;
  background: #f5f5f7;
  appearance: none;
  -webkit-appearance: none;
  cursor: pointer;
  transition: all 0.2s ease;
  font-family: inherit;
}

.pill-select select:hover {
  background: #efefef;
  border-color: rgba(0,0,0,0.12);
}

.pill-select select:focus {
  outline: none;
  border-color: #FF5B3C;
  background: #fff;
  box-shadow: 0 0 0 3px rgba(255,91,60,0.1);
}

.pill-select.disabled select {
  color: #c7c7cc;
  cursor: not-allowed;
  background: #fafafa;
}

.chevron {
  position: absolute;
  right: 16px;
  top: 50%;
  transform: translateY(-50%);
  color: #86868b;
  pointer-events: none;
}

/* ── Pill Input ── */
.pill-input-row {
  display: flex;
  gap: 10px;
  align-items: center;
}

.pill-input {
  position: relative;
  flex: 1;
}

.input-icon {
  position: absolute;
  left: 18px;
  top: 50%;
  transform: translateY(-50%);
  color: #86868b;
  pointer-events: none;
  z-index: 1;
}

.pill-input input {
  width: 100%;
  height: 46px;
  padding: 0 18px 0 44px;
  border: 1.5px solid rgba(0,0,0,0.08);
  border-radius: 999px;
  font-size: 15px;
  color: #1d1d1f;
  background: #f5f5f7;
  transition: all 0.2s ease;
  font-family: inherit;
}

.pill-input input::placeholder {
  color: #c7c7cc;
}

.pill-input input:hover {
  background: #efefef;
  border-color: rgba(0,0,0,0.12);
}

.pill-input input:focus {
  outline: none;
  border-color: #FF5B3C;
  background: #fff;
  box-shadow: 0 0 0 3px rgba(255,91,60,0.1);
}

/* ── Search spinner ── */
.search-spinner {
  position: absolute;
  right: 16px;
  top: 50%;
  transform: translateY(-50%);
  width: 16px;
  height: 16px;
  border: 2px solid rgba(0,0,0,0.06);
  border-top-color: #FF5B3C;
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
}

/* ── Suggestions Dropdown ── */
.address-autocomplete {
  position: relative;
}

.suggestions-dropdown {
  position: absolute;
  top: calc(100% + 6px);
  left: 0;
  right: 0;
  background: #fff;
  border: 1px solid rgba(0,0,0,0.08);
  border-radius: 16px;
  box-shadow:
    0 4px 16px rgba(0,0,0,0.08),
    0 1px 3px rgba(0,0,0,0.04);
  overflow: hidden;
  z-index: 100;
  max-height: 280px;
  overflow-y: auto;
}

.suggestion-item {
  display: flex;
  align-items: center;
  gap: 12px;
  width: 100%;
  padding: 12px 16px;
  border: none;
  background: none;
  cursor: pointer;
  text-align: left;
  transition: background 0.15s ease;
  font-family: inherit;
}

.suggestion-item:hover {
  background: #f5f5f7;
}

.suggestion-item:not(:last-child) {
  border-bottom: 1px solid rgba(0,0,0,0.04);
}

.suggestion-icon {
  flex-shrink: 0;
  color: #FF5B3C;
}

.suggestion-text {
  display: flex;
  flex-direction: column;
  gap: 2px;
  min-width: 0;
}

.suggestion-main {
  font-size: 14px;
  font-weight: 500;
  color: #1d1d1f;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.suggestion-secondary {
  font-size: 12px;
  color: #86868b;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

/* ── Dropdown transition ── */
.dropdown-enter-active,
.dropdown-leave-active {
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
}

.dropdown-enter-from,
.dropdown-leave-to {
  opacity: 0;
  transform: translateY(-4px);
}

/* ── Pill Button ── */
.pill-btn {
  display: flex;
  align-items: center;
  gap: 6px;
  height: 46px;
  padding: 0 20px;
  border: 1.5px solid rgba(0,0,0,0.08);
  border-radius: 999px;
  background: #f5f5f7;
  color: #86868b;
  font-size: 14px;
  font-weight: 500;
  font-family: inherit;
  cursor: pointer;
  transition: all 0.2s ease;
  white-space: nowrap;
  flex-shrink: 0;
}

.pill-btn:hover {
  background: #efefef;
  color: #1d1d1f;
  border-color: rgba(0,0,0,0.12);
}

.pill-btn.active {
  background: linear-gradient(135deg, #FF6B4A, #FF3D2E);
  border-color: transparent;
  color: #fff;
  box-shadow: 0 2px 10px rgba(255,91,60,0.3);
}

/* ── Map ── */
.map-area {
  border-radius: 20px;
  overflow: hidden;
  border: 1.5px solid rgba(0,0,0,0.08);
}

.map-wrapper {
  overflow: hidden;
}

.map-hint {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  padding: 10px 20px;
  background: #f5f5f7;
  color: #86868b;
  font-size: 13px;
  margin: 0;
}

.map-slide-enter-active,
.map-slide-leave-active {
  transition: all 0.35s cubic-bezier(0.4, 0, 0.2, 1);
  max-height: 400px;
  opacity: 1;
}

.map-slide-enter-from,
.map-slide-leave-to {
  max-height: 0;
  opacity: 0;
}

/* ── Coordinates Pill ── */
.coords-pill {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 8px 18px;
  background: rgba(52,199,89,0.08);
  color: #34C759;
  font-size: 13px;
  font-weight: 500;
  border-radius: 999px;
}

.coords-pill svg {
  flex-shrink: 0;
}

/* ── Fade ── */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.25s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

/* ── Responsive ── */
@media (max-width: 600px) {
  .fields-row {
    flex-direction: column;
    gap: 16px;
    align-items: stretch;
  }

  .pill-input-row {
    flex-direction: column;
  }

  .pill-btn {
    width: 100%;
    justify-content: center;
  }
}
</style>
