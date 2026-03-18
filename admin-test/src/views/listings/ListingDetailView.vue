<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { api } from '@/services/api'
import type { ListingDetail } from '@/types/listing'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'

const router = useRouter()
const route = useRoute()

const listingId = computed(() => route.params.id as string)

// State
const listing = ref<ListingDetail | null>(null)
const loading = ref(false)
const error = ref('')
const selectedImageIndex = ref(0)
const imageUrls = ref<string[]>([])
const planUrl = ref('')
const showAllAmenities = ref(false)
const mapContainer = ref<HTMLElement | null>(null)
let mapInstance: L.Map | null = null

// Format price (prices are in sum)
function formatPrice(price?: number): string {
  if (!price) return 'Not specified'
  return new Intl.NumberFormat('ru-RU', { maximumFractionDigits: 0 }).format(price)
}

// Load listing
async function loadListing() {
  loading.value = true
  error.value = ''
  try {
    const data = await api.getListingById(listingId.value)
    listing.value = {
      ...data,
      regionName: data.region,
      districtName: data.district
    }
    // Load images and plan - URLs come directly from API
    if (data.images && data.images.length > 0) {
      imageUrls.value = data.images
    }
    if (data.plan) {
      planUrl.value = data.plan
    }
  } catch (e: unknown) {
    const err = e as Error
    error.value = err.message || 'Error loading listing'
  } finally {
    loading.value = false
  }
}

// Get floor numbers display
const floorDisplay = computed(() => {
  if (!listing.value?.floors || listing.value.floors.length === 0) return '-'
  const numbers = listing.value.floors.map(f => f.number).sort((a, b) => a - b)
  if (numbers.length === 1) return numbers[0].toString()
  return `${numbers[0]}-${numbers[numbers.length - 1]}`
})

// Get main category
const mainCategory = computed(() => {
  if (!listing.value?.categories || listing.value.categories.length === 0) return 'Property'
  return listing.value.categories[0]
})

// Visible amenities (max 6 unless expanded)
const visibleAmenities = computed(() => {
  if (!listing.value?.amenities) return []
  if (showAllAmenities.value) return listing.value.amenities
  return listing.value.amenities.slice(0, 6)
})

// Actions
function handleEdit() {
  router.push(`/listings/${listingId.value}/edit`)
}

async function handleDelete() {
  if (!confirm('Are you sure you want to delete this listing?')) return

  try {
    await api.deleteListing(listingId.value)
    router.push('/listings')
  } catch (e: unknown) {
    const err = e as Error
    alert(err.message || 'Delete error')
  }
}

// Initialize map with marker
function initMap() {
  if (!mapContainer.value || !listing.value?.latitude || !listing.value?.longitude) return
  if (mapInstance) return // Already initialized

  // Fix Leaflet default icon issue with bundlers
  delete (L.Icon.Default.prototype as unknown as Record<string, unknown>)._getIconUrl
  L.Icon.Default.mergeOptions({
    iconRetinaUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png',
    iconUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
    shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
  })

  // Use coordinates as provided by API
  const lat = listing.value.latitude
  const lng = listing.value.longitude

  mapInstance = L.map(mapContainer.value).setView([lat, lng], 15)

  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '© OpenStreetMap contributors'
  }).addTo(mapInstance)

  // Add marker with popup
  L.marker([lat, lng])
    .addTo(mapInstance)
    .bindPopup(listing.value.address || 'Location')
    .openPopup()
}

// Lifecycle
onMounted(async () => {
  await loadListing()
  // Initialize map after data is loaded
  setTimeout(() => initMap(), 100)
})
</script>

<template>
  <div class="listing-detail-view">
    <!-- Back button -->
    <button class="btn-back" @click="router.push('/listings')">
      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <path d="M19 12H5M12 19l-7-7 7-7"/>
      </svg>
      Back to Listings
    </button>

    <!-- Loading -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Loading...</p>
    </div>

    <!-- Error -->
    <div v-else-if="error" class="error-state">
      <p>{{ error }}</p>
      <button class="btn btn-secondary" @click="loadListing">Retry</button>
    </div>

    <!-- Content - Two Column Layout -->
    <div v-else-if="listing" class="detail-layout">
      <!-- Left Column -->
      <div class="left-column">
        <!-- Image Gallery -->
        <div class="gallery-section">
          <div class="main-image">
            <img
              v-if="imageUrls.length > 0"
              :src="imageUrls[selectedImageIndex]"
              alt="Listing image"
            />
            <div v-else class="no-image">
              <span>📷</span>
              <p>No images</p>
            </div>
          </div>
          <div v-if="imageUrls.length > 1" class="thumbnails">
            <div
              v-for="(url, index) in imageUrls.slice(0, 5)"
              :key="index"
              class="thumbnail"
              :class="{ active: selectedImageIndex === index }"
              @click="selectedImageIndex = index"
            >
              <img :src="url" alt="" />
            </div>
            <div v-if="imageUrls.length > 5" class="thumbnail more-photos">
              <span>+{{ imageUrls.length - 5 }}</span>
            </div>
          </div>
        </div>

        <!-- Property Stats Row -->
        <div class="stats-row">
          <div class="stat-box">
            <span class="stat-value">{{ listing.totalArea }}</span>
            <span class="stat-unit">м²</span>
            <span class="stat-label">Area</span>
          </div>
          <div class="stat-box" v-if="listing.roomsCount">
            <span class="stat-value">{{ listing.roomsCount }}</span>
            <span class="stat-label">Rooms</span>
          </div>
          <div class="stat-box">
            <span class="stat-value">{{ floorDisplay }}</span>
            <span class="stat-label">Floor</span>
          </div>
          <div class="stat-box">
            <span class="stat-value">{{ mainCategory }}</span>
            <span class="stat-label">Type</span>
          </div>
        </div>

        <!-- Amenities Section -->
        <div v-if="listing.amenities && listing.amenities.length > 0" class="section amenities-section">
          <h2>Characteristics</h2>
          <div class="amenities-grid">
            <div v-for="(amenity, index) in visibleAmenities" :key="index" class="amenity-item">
              <span class="amenity-icon">✓</span>
              <span class="amenity-name">{{ amenity.name }}</span>
            </div>
          </div>
          <button
            v-if="listing.amenities.length > 6"
            class="btn-show-all"
            @click="showAllAmenities = !showAllAmenities"
          >
            {{ showAllAmenities ? 'Show less' : `Show all: ${listing.amenities.length}` }}
          </button>
        </div>

        <!-- Floor Plan Section -->
        <div v-if="planUrl" class="section plan-section">
          <h2>Floor Plan</h2>
          <div class="plan-image">
            <img :src="planUrl" alt="Floor plan" />
          </div>
        </div>

        <!-- Location Section -->
        <div class="section location-section">
          <h2>Location</h2>
          <p class="address-text">{{ listing.address || 'No address specified' }}</p>
          <p class="region-text" v-if="listing.regionName || listing.districtName">
            {{ listing.regionName }}{{ listing.districtName ? `, ${listing.districtName}` : '' }}
          </p>

          <!-- Interactive Map -->
          <div v-if="listing.latitude && listing.longitude" class="map-container">
            <div ref="mapContainer" class="leaflet-map"></div>
          </div>
        </div>

        <!-- Description Section -->
        <div v-if="listing.description" class="section description-section">
          <h2>Description</h2>
          <p class="description-text">{{ listing.description }}</p>
        </div>
      </div>

      <!-- Right Column - Price Card -->
      <div class="right-column">
        <div class="price-card">
          <!-- Title -->
          <h3 class="price-title">
            {{ listing.totalArea }} м²{{ listing.roomsCount ? `, ${listing.roomsCount} rooms` : '' }}, {{ mainCategory }}
          </h3>

          <!-- Main Price -->
          <div class="price-main">
            <span class="price-amount">{{ formatPrice(listing.priceForMonth) }}</span>
            <span class="price-period">sum/month</span>
          </div>

          <!-- Price per m² -->
          <div v-if="listing.pricePerSquareMeter" class="price-per-sqm">
            {{ formatPrice(listing.pricePerSquareMeter) }} sum per m²
          </div>

          <!-- Action Buttons -->
          <div class="price-actions">
            <button class="btn btn-primary" @click="handleEdit">
              ✏️ Edit Listing
            </button>
            <button class="btn btn-outline-danger" @click="handleDelete">
              🗑️ Delete
            </button>
          </div>

          <!-- Location Info -->
          <div class="card-location">
            <div class="location-row">
              <span class="location-icon">📍</span>
              <span>{{ listing.address || 'No address' }}</span>
            </div>
            <div v-if="listing.regionName" class="location-region">
              {{ listing.regionName }}{{ listing.districtName ? `, ${listing.districtName}` : '' }}
            </div>
          </div>

          <!-- Building Info -->
          <div v-if="listing.building || listing.complex" class="card-building">
            <div class="building-icon">🏢</div>
            <div class="building-info">
              <span class="building-name">{{ listing.building || listing.complex }}</span>
              <span class="building-label">Building</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.listing-detail-view {
  padding: 20px 24px;
  max-width: 1200px;
  margin: 0 auto;
}

.btn-back {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  background: none;
  border: none;
  color: rgba(27, 27, 27, 0.7);
  font-size: 14px;
  cursor: pointer;
  padding: 10px 0;
  margin-bottom: 20px;
  border-radius: 8px;
  transition: all 0.2s;
}

.btn-back:hover {
  color: #1B1B1B;
}

.loading-state,
.error-state {
  text-align: center;
  padding: 60px;
  background: #FFFFFF;
  border-radius: 24px;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 3px solid rgba(27, 27, 27, 0.08);
  border-top-color: #FF5B3C;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 16px;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

/* Two Column Layout */
.detail-layout {
  display: grid;
  grid-template-columns: 1fr 380px;
  gap: 24px;
  align-items: start;
}

/* Left Column */
.left-column {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

/* Gallery */
.gallery-section {
  background: #FFFFFF;
  border-radius: 24px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
}

.main-image {
  width: 100%;
  height: 400px;
  background: #F5F5F5;
}

.main-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.no-image {
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: rgba(27, 27, 27, 0.3);
}

.no-image span {
  font-size: 48px;
  margin-bottom: 8px;
}

.thumbnails {
  display: flex;
  gap: 8px;
  padding: 12px;
}

.thumbnail {
  width: 80px;
  height: 60px;
  border-radius: 8px;
  overflow: hidden;
  cursor: pointer;
  border: 2px solid transparent;
  transition: all 0.2s;
}

.thumbnail.active {
  border-color: #FF5B3C;
}

.thumbnail img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.thumbnail.more-photos {
  background: rgba(27, 27, 27, 0.8);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 600;
}

/* Stats Row */
.stats-row {
  display: flex;
  gap: 16px;
  padding: 20px;
  background: #FFFFFF;
  border-radius: 24px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
}

.stat-box {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  padding: 12px;
}

.stat-value {
  font-size: 20px;
  font-weight: 700;
  color: #1B1B1B;
}

.stat-unit {
  font-size: 14px;
  color: #1B1B1B;
  margin-left: 2px;
}

.stat-label {
  font-size: 12px;
  color: rgba(27, 27, 27, 0.5);
  margin-top: 4px;
}

/* Section */
.section {
  background: #FFFFFF;
  border-radius: 24px;
  padding: 24px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
}

.section h2 {
  font-size: 18px;
  font-weight: 700;
  color: #1B1B1B;
  margin: 0 0 16px 0;
}

/* Amenities */
.amenities-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 12px;
}

.amenity-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 12px;
  background: #F7F7F7;
  border-radius: 12px;
}

.amenity-icon {
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(76, 175, 80, 0.1);
  color: #4CAF50;
  border-radius: 6px;
  font-size: 12px;
}

.amenity-name {
  font-size: 14px;
  color: #1B1B1B;
}

.btn-show-all {
  margin-top: 12px;
  padding: 10px 20px;
  background: #F7F7F7;
  border: none;
  border-radius: 100px;
  font-size: 14px;
  font-weight: 600;
  color: #1B1B1B;
  cursor: pointer;
}

.btn-show-all:hover {
  background: #EBEBEB;
}

/* Plan */
.plan-section .plan-image {
  width: 100%;
  border-radius: 12px;
  overflow: hidden;
  background: #F7F7F7;
}

.plan-image img {
  width: 100%;
  object-fit: contain;
}

/* Location */
.address-text {
  font-size: 16px;
  color: #1B1B1B;
  margin: 0 0 4px 0;
}

.region-text {
  font-size: 14px;
  color: rgba(27, 27, 27, 0.5);
  margin: 0 0 16px 0;
}

.map-container {
  width: 100%;
  height: 200px;
  border-radius: 12px;
  overflow: hidden;
  background: #F7F7F7;
}

.map-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: rgba(27, 27, 27, 0.4);
}

.map-placeholder span {
  font-size: 32px;
  margin-bottom: 8px;
}

/* Description */
.description-text {
  font-size: 15px;
  line-height: 1.6;
  color: #1B1B1B;
  margin: 0;
}

/* Right Column - Price Card */
.right-column {
  position: sticky;
  top: 24px;
}

.price-card {
  background: #FFFFFF;
  border-radius: 24px;
  padding: 24px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
}

.price-title {
  font-size: 16px;
  font-weight: 600;
  color: #1B1B1B;
  margin: 0 0 12px 0;
}

.price-main {
  margin-bottom: 4px;
}

.price-amount {
  font-size: 28px;
  font-weight: 700;
  color: #1B1B1B;
}

.price-period {
  font-size: 14px;
  color: rgba(27, 27, 27, 0.5);
  margin-left: 4px;
}

.price-per-sqm {
  font-size: 14px;
  color: rgba(27, 27, 27, 0.5);
  margin-bottom: 20px;
}

.price-actions {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-bottom: 20px;
}

.btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  padding: 14px 20px;
  border-radius: 100px;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  border: none;
  transition: all 0.2s;
}

.btn-primary {
  background: #FF5B3C;
  color: white;
}

.btn-primary:hover {
  background: #E54A2D;
}

.btn-outline-danger {
  background: #FFFFFF;
  color: #F44336;
  border: 1px solid #F44336;
}

.btn-outline-danger:hover {
  background: #FEF2F2;
}

.btn-secondary {
  background: #F7F7F7;
  color: #1B1B1B;
}

.card-location {
  padding: 16px 0;
  border-top: 1px solid rgba(27, 27, 27, 0.08);
}

.location-row {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  color: #1B1B1B;
}

.location-icon {
  font-size: 16px;
}

.location-region {
  font-size: 13px;
  color: rgba(27, 27, 27, 0.5);
  margin-top: 4px;
  padding-left: 24px;
}

.card-building {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 16px;
  background: #F7F7F7;
  border-radius: 16px;
  margin-top: 16px;
}

.building-icon {
  font-size: 24px;
}

.building-info {
  display: flex;
  flex-direction: column;
}

.building-name {
  font-size: 14px;
  font-weight: 600;
  color: #1B1B1B;
}

.building-label {
  font-size: 12px;
  color: rgba(27, 27, 27, 0.5);
}

/* Responsive */
@media (max-width: 900px) {
  .detail-layout {
    grid-template-columns: 1fr;
  }

  .right-column {
    position: static;
  }

  .stats-row {
    flex-wrap: wrap;
  }

  .stat-box {
    min-width: 45%;
  }

  .amenities-grid {
    grid-template-columns: 1fr;
  }
}
</style>
