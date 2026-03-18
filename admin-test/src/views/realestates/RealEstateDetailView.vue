<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { api } from '@/services/api'
import type { RealEstate } from '@/types/realestate'

const router = useRouter()
const route = useRoute()

const realEstateId = computed(() => route.params.id as string)

// State
const realEstate = ref<RealEstate | null>(null)
const loading = ref(false)
const error = ref('')
const expandedUnits = ref<Set<string>>(new Set())

// Room type icons
function getRoomIcon(typeName?: string): string {
  const name = (typeName || '').toLowerCase()
  if (name.includes('bedroom') || name.includes('спальн')) return '🛏️'
  if (name.includes('living') || name.includes('гостин')) return '🛋️'
  if (name.includes('kitchen') || name.includes('кухн')) return '🍳'
  if (name.includes('bath') || name.includes('ванн') || name.includes('санузел')) return '🚿'
  if (name.includes('balcon') || name.includes('балкон')) return '🌅'
  if (name.includes('office') || name.includes('кабинет')) return '💼'
  if (name.includes('storage') || name.includes('кладов')) return '📦'
  if (name.includes('hall') || name.includes('прихож') || name.includes('коридор')) return '🚪'
  return '🏠'
}

// Toggle unit expansion
function toggleUnit(unitId: string) {
  if (expandedUnits.value.has(unitId)) {
    expandedUnits.value.delete(unitId)
  } else {
    expandedUnits.value.add(unitId)
  }
  expandedUnits.value = new Set(expandedUnits.value) // trigger reactivity
}

// Image handling
const currentImageIndex = ref(0)

// Helper to get current image URL directly from API response
const currentImageUrl = computed(() => realEstate.value?.images?.[currentImageIndex.value])

// Status helpers
function getStatusBadgeClass(status?: string): string {
  switch (status) {
    case 'Accept': return 'badge-success'
    case 'InModeration': return 'badge-warning'
    case 'Reject': return 'badge-danger'
    case 'Cancel': return 'badge-secondary'
    case 'Block': return 'badge-dark'
    default: return 'badge-secondary'
  }
}

function getStatusLabel(status?: string): string {
  switch (status) {
    case 'Accept': return 'Approved'
    case 'InModeration': return 'In Moderation'
    case 'Reject': return 'Rejected'
    case 'Cancel': return 'Cancelled'
    case 'Block': return 'Blocked'
    default: return 'Draft'
  }
}

// Load real estate
async function loadRealEstate() {
  loading.value = true
  error.value = ''
  try {
    realEstate.value = await api.getRealEstateById(realEstateId.value)
  } catch (e: any) {
    error.value = e.message || 'Error loading property'
  } finally {
    loading.value = false
  }
}



// Image navigation
function nextImage() {
  if (realEstate.value?.images && currentImageIndex.value < realEstate.value.images.length - 1) {
    currentImageIndex.value++
  }
}

function prevImage() {
  if (currentImageIndex.value > 0) {
    currentImageIndex.value--
  }
}

// Actions
function handleEdit() {
  router.push(`/realestates/${realEstateId.value}/edit`)
}

async function handleDelete() {
  if (!confirm('Are you sure you want to delete this property?')) return

  try {
    await api.deleteRealEstate(realEstateId.value)
    router.push('/realestates')
  } catch (e: any) {
    alert(e.message || 'Delete error')
  }
}

// Lifecycle
onMounted(() => {
  loadRealEstate()
})


</script>

<template>
  <div class="realestate-detail-view">
    <!-- Header -->
    <div class="page-header">
      <button class="btn-back" @click="router.back()">
        <svg width="20" height="20" viewBox="0 0 14 14" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M8.75 3.5L5.25 7L8.75 10.5" stroke="#1B1B1B" stroke-width="1.67" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </button>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Loading...</p>
    </div>

    <!-- Error -->
    <div v-else-if="error" class="error-state">
      <p>{{ error }}</p>
      <button class="btn btn-secondary" @click="loadRealEstate">Retry</button>
    </div>

    <!-- Content -->
    <div v-else-if="realEstate" class="detail-content">

      <!-- Mosaic Gallery -->
      <div class="gallery-mosaic">
        <div class="gallery-main">
          <img
            v-if="currentImageUrl"
            :src="currentImageUrl"
            alt="Property Photo"
          />
          <div v-else class="image-placeholder">
            <span>🏠</span>
            <p>No photos</p>
          </div>

          <!-- Navigation arrows -->
          <template v-if="realEstate.images && realEstate.images.length > 1">
            <button
              class="nav-arrow prev"
              @click="prevImage"
              :disabled="currentImageIndex === 0"
            >
              ‹
            </button>
            <button
              class="nav-arrow next"
              @click="nextImage"
              :disabled="currentImageIndex === realEstate.images.length - 1"
            >
              ›
            </button>
          </template>
        </div>

        <!-- Thumbnail Grid -->
        <div v-if="realEstate.images && realEstate.images.length > 1" class="gallery-thumbs">
          <div
            v-for="(key, index) in realEstate.images.slice(0, 4)"
            :key="key"
            class="thumb-item"
            :class="{ active: index === currentImageIndex }"
            @click="currentImageIndex = index"
          >
            <img :src="key" alt="" />
          </div>
          <div v-if="realEstate.images.length > 4" class="thumb-more">
            <span>📷 All Photos</span>
          </div>
        </div>
      </div>

      <!-- Title Section -->
      <div class="title-section">
        <h1>{{ realEstate.address || 'Property Details' }}</h1>
        <p class="location-text">
          {{ realEstate.regionName }}{{ realEstate.districtName ? `, ${realEstate.districtName}` : '' }}
        </p>
      </div>

      <!-- Property Badges -->
      <div class="property-badges">
        <div class="badge-item" v-if="realEstate.realEstateTypeName">
          <span class="badge-icon">
            <svg width="20" height="20" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
              <rect x="6" y="8" width="36" height="36" rx="2" stroke="currentColor" stroke-width="2"/>
              <path d="M6 16H42" stroke="currentColor" stroke-width="2"/>
              <rect x="12" y="22" width="8" height="6" rx="1" stroke="currentColor" stroke-width="2"/>
              <rect x="28" y="22" width="8" height="6" rx="1" stroke="currentColor" stroke-width="2"/>
            </svg>
          </span>
          <span class="badge-text">{{ realEstate.realEstateTypeName }}</span>
        </div>
        <div class="badge-item" v-if="realEstate.totalArea">
          <span class="badge-icon">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M4 4L4 20M4 20H20M4 4H20M20 4V20M20 20L4 4" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
          </span>
          <span class="badge-text">{{ realEstate.totalArea }} m²</span>
        </div>
      </div>

      <!-- Main Content Grid -->
      <div class="content-grid">
        <!-- Left Column - Main Content -->
        <div class="content-main">

          <!-- Key Stats Cards -->
          <div class="stats-cards">
            <div class="stat-card" v-if="realEstate.totalArea">
              <span class="stat-icon">
                <svg width="32" height="32" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
                  <path d="M6 42V12L24 6L42 12V42M6 42H42M14 18H20M28 18H34M14 28H20M28 28H34M18 42V34H30V42" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
              </span>
              <div class="stat-info">
                <span class="stat-value">{{ realEstate.totalArea }} m²</span>
                <span class="stat-label">Total Area</span>
              </div>
            </div>
            <div class="stat-card" v-if="realEstate.livingArea">
              <span class="stat-icon">
                <svg width="32" height="32" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
                  <path d="M6 22L24 6L42 22" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                  <path d="M10 20V42H38V20" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                  <path d="M20 42V30H28V42" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                  <rect x="30" y="24" width="5" height="5" rx="1" stroke="currentColor" stroke-width="2"/>
                </svg>
              </span>
              <div class="stat-info">
                <span class="stat-value">{{ realEstate.livingArea }} m²</span>
                <span class="stat-label">Living Area</span>
              </div>
            </div>
            <div class="stat-card" v-if="realEstate.roomsCount">
              <span class="stat-icon">
                <svg width="32" height="32" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
                  <rect x="8" y="6" width="32" height="36" rx="2" stroke="currentColor" stroke-width="2"/>
                  <path d="M8 18H40" stroke="currentColor" stroke-width="2"/>
                  <rect x="28" y="24" width="6" height="12" rx="1" stroke="currentColor" stroke-width="2"/>
                  <circle cx="32" cy="30" r="1.5" fill="currentColor"/>
                </svg>
              </span>
              <div class="stat-info">
                <span class="stat-value">{{ realEstate.roomsCount }}</span>
                <span class="stat-label">Rooms</span>
              </div>
            </div>
            <div class="stat-card" v-if="realEstate.floorNumber">
              <span class="stat-icon">
                <svg width="32" height="32" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
                  <rect x="8" y="8" width="32" height="36" rx="2" stroke="currentColor" stroke-width="2"/>
                  <path d="M8 16H40M8 24H40M8 32H40" stroke="currentColor" stroke-width="2"/>
                  <path d="M20 8V44M28 8V44" stroke="currentColor" stroke-width="2"/>
                </svg>
              </span>
              <div class="stat-info">
                <span class="stat-value">{{ realEstate.floorNumber }}{{ realEstate.totalFloors ? ` / ${realEstate.totalFloors}` : '' }}</span>
                <span class="stat-label">Floor{{ realEstate.aboveFloors ? ` · ${realEstate.aboveFloors} above` : '' }}{{ realEstate.belowFloors ? ` · ${realEstate.belowFloors} below` : '' }}</span>
              </div>
            </div>
          </div>

          <!-- Rooms Section -->
          <div v-if="realEstate.rooms && realEstate.rooms.length > 0" class="section-card">
            <div class="section-header">
              <h3>Rooms</h3>
              <span class="section-count">{{ realEstate.rooms.length }} rooms</span>
            </div>
            <div class="rooms-grid-modern">
              <div v-for="room in realEstate.rooms" :key="room.id" class="room-card-modern">
                <span class="room-icon">{{ getRoomIcon(room.roomTypeName) }}</span>
                <div class="room-info">
                  <span class="room-type">{{ room.roomTypeName || 'Room' }}</span>
                  <span class="room-area">{{ room.totalArea }} m²</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Units Section -->
          <div v-if="realEstate.units && realEstate.units.length > 0" class="section-card">
            <div class="section-header">
              <h3>Units</h3>
              <span class="section-count">{{ realEstate.units.length }} units</span>
            </div>
            <div class="units-accordion">
              <div
                v-for="(unit, index) in realEstate.units"
                :key="unit.id"
                class="unit-accordion-item"
                :class="{ expanded: expandedUnits.has(unit.id) }"
              >
                <div class="unit-accordion-header" @click="toggleUnit(unit.id)">
                  <div class="unit-header-left">
                    <span class="unit-indicator">{{ index + 1 }}</span>
                    <div class="unit-header-info">
                      <span class="unit-title">Unit {{ index + 1 }}</span>
                      <div class="unit-badges">
                        <span class="unit-badge area">📐 {{ unit.totalArea }} m²</span>
                        <span v-if="unit.roomsCount" class="unit-badge rooms">🚪 {{ unit.roomsCount }} rooms</span>
                        <span v-if="unit.floorNumber" class="unit-badge floor">🏢 Floor {{ unit.floorNumber }}</span>
                      </div>
                    </div>
                  </div>
                  <span class="expand-icon" :class="{ rotated: expandedUnits.has(unit.id) }">▼</span>
                </div>

                <div class="unit-accordion-content" v-show="expandedUnits.has(unit.id)">
                  <div class="unit-specs-grid">
                    <div class="unit-spec" v-if="unit.livingArea">
                      <span class="spec-label">Living Area</span>
                      <span class="spec-value">{{ unit.livingArea }} m²</span>
                    </div>
                    <div class="unit-spec" v-if="unit.ceilingHeight">
                      <span class="spec-label">Ceiling Height</span>
                      <span class="spec-value">{{ unit.ceilingHeight }} m</span>
                    </div>
                    <div class="unit-spec" v-if="unit.renovationName">
                      <span class="spec-label">Renovation</span>
                      <span class="spec-value">{{ unit.renovationName }}</span>
                    </div>
                  </div>

                  <div v-if="unit.rooms && unit.rooms.length > 0" class="unit-rooms-section">
                    <span class="unit-rooms-title">Room breakdown</span>
                    <div class="rooms-grid-compact">
                      <div v-for="room in unit.rooms" :key="room.id" class="room-chip">
                        <span class="chip-icon">{{ getRoomIcon(room.roomTypeName) }}</span>
                        <span class="chip-text">{{ room.roomTypeName || 'Room' }}</span>
                        <span class="chip-area">{{ room.totalArea }} m²</span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Right Column - Sidebar -->
        <div class="content-sidebar">
          <!-- Status Card -->
          <div class="sidebar-card status-card">
            <div class="status-header">
              <span class="status-badge" :class="getStatusBadgeClass(realEstate.moderationStatus)">
                {{ getStatusLabel(realEstate.moderationStatus) }}
              </span>
            </div>
          </div>

          <!-- Specs Card -->
          <div class="sidebar-card specs-card">
            <h4>Specifications</h4>
            <div class="specs-list">
              <div class="spec-row" v-if="realEstate.cadastralNumber">
                <span class="spec-name">Cadastral Number</span>
                <span class="spec-val">{{ realEstate.cadastralNumber }}</span>
              </div>
              <div class="spec-row" v-if="realEstate.number">
                <span class="spec-name">Unit Number</span>
                <span class="spec-val">{{ realEstate.number }}</span>
              </div>
              <div class="spec-row" v-if="realEstate.ceilingHeight">
                <span class="spec-name">Ceiling Height</span>
                <span class="spec-val">{{ realEstate.ceilingHeight }} m</span>
              </div>
              <div class="spec-row" v-if="realEstate.buildingNumber">
                <span class="spec-name">Building</span>
                <span class="spec-val">{{ realEstate.buildingNumber }}</span>
              </div>
            </div>
          </div>

          <!-- Actions Card -->
          <div class="sidebar-card actions-card">
            <button class="btn btn-primary btn-full" @click="handleEdit">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M11 4H4C3.46957 4 2.96086 4.21071 2.58579 4.58579C2.21071 4.96086 2 5.46957 2 6V20C2 20.5304 2.21071 21.0391 2.58579 21.4142C2.96086 21.7893 3.46957 22 4 22H18C18.5304 22 19.0391 21.7893 19.4142 21.4142C19.7893 21.0391 20 20.5304 20 20V13" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                <path d="M18.5 2.50001C18.8978 2.10219 19.4374 1.87869 20 1.87869C20.5626 1.87869 21.1022 2.10219 21.5 2.50001C21.8978 2.89784 22.1213 3.4374 22.1213 4.00001C22.1213 4.56262 21.8978 5.10219 21.5 5.50001L12 15L8 16L9 12L18.5 2.50001Z" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
              </svg>
              Edit Property
            </button>
            <button class="btn btn-danger-outline btn-full" @click="handleDelete">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M3 6H5H21" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                <path d="M19 6V20C19 20.5304 18.7893 21.0391 18.4142 21.4142C18.0391 21.7893 17.5304 22 17 22H7C6.46957 22 5.96086 21.7893 5.58579 21.4142C5.21071 21.0391 5 20.5304 5 20V6M8 6V4C8 3.46957 8.21071 2.96086 8.58579 2.58579C8.96086 2.21071 9.46957 2 10 2H14C14.5304 2 15.0391 2.21071 15.4142 2.58579C15.7893 2.96086 16 3.46957 16 4V6" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
              </svg>
              Delete
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.realestate-detail-view {
  padding: 24px;
  max-width: 1400px;
  margin: 0 auto;
}

.page-header {
  margin-bottom: 16px;
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

.loading-state,
.error-state {
  text-align: center;
  padding: 60px;
  background: white;
  border-radius: 16px;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 3px solid #e5e7eb;
  border-top-color: var(--color-brand, #FF5B3C);
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 16px;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

/* ========================================
   MOSAIC GALLERY
   ======================================== */
.gallery-mosaic {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 8px;
  margin-bottom: 24px;
  border-radius: 20px;
  overflow: hidden;
}

.gallery-main {
  position: relative;
  aspect-ratio: 16/10;
  background: var(--bg-secondary, #f7f7f7);
  border-radius: 20px 0 0 20px;
  overflow: hidden;
}

.gallery-main img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.image-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: var(--text-muted, #9ca3af);
  background: var(--bg-secondary, #f7f7f7);
}

.image-placeholder span {
  font-size: 64px;
}

.nav-arrow {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  width: 44px;
  height: 44px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.95);
  border: none;
  font-size: 24px;
  cursor: pointer;
  transition: all 0.2s;
  box-shadow: 0 2px 8px rgba(0,0,0,0.15);
}

.nav-arrow:hover:not(:disabled) {
  background: white;
  transform: translateY(-50%) scale(1.05);
}

.nav-arrow:disabled {
  opacity: 0.3;
  cursor: not-allowed;
}

.nav-arrow.prev {
  left: 16px;
}

.nav-arrow.next {
  right: 16px;
}

.gallery-thumbs {
  display: grid;
  grid-template-columns: 1fr 1fr;
  grid-template-rows: 1fr 1fr;
  gap: 8px;
}

.thumb-item {
  position: relative;
  overflow: hidden;
  cursor: pointer;
  background: var(--bg-secondary, #f7f7f7);
  transition: all 0.2s;
}

.thumb-item:first-child {
  border-radius: 0 16px 0 0;
}

.thumb-item:nth-child(2) {
  border-radius: 0;
}

.thumb-item:nth-child(3) {
  border-radius: 0;
}

.thumb-item:nth-child(4) {
  border-radius: 0 0 16px 0;
}

.thumb-item:hover {
  opacity: 0.85;
}

.thumb-item.active {
  box-shadow: inset 0 0 0 3px var(--color-brand, #FF5B3C);
}

.thumb-item img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.thumb-more {
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(0, 0, 0, 0.6);
  color: white;
  font-size: 13px;
  font-weight: 500;
  border-radius: 0 0 16px 0;
  cursor: pointer;
  transition: background 0.2s;
}

.thumb-more:hover {
  background: rgba(0, 0, 0, 0.75);
}

/* ========================================
   TITLE SECTION
   ======================================== */
.title-section {
  margin-bottom: 16px;
}

.title-section h1 {
  font-size: 28px;
  font-weight: 700;
  color: var(--text-primary, #1b1b1b);
  margin: 0 0 4px 0;
  line-height: 1.3;
}

.location-text {
  font-size: 15px;
  color: var(--text-secondary, rgba(27, 27, 27, 0.6));
  margin: 0;
}

/* ========================================
   PROPERTY BADGES
   ======================================== */
.property-badges {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
  margin-bottom: 24px;
  padding-bottom: 24px;
  border-bottom: 1px solid var(--border-color, rgba(27, 27, 27, 0.1));
}

.badge-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 20px;
  background: var(--bg-card, #ffffff);
  border: 1px solid var(--border-color, rgba(27, 27, 27, 0.1));
  border-radius: 12px;
  transition: all 0.2s;
}

.badge-item:hover {
  border-color: var(--color-brand, #FF5B3C);
  box-shadow: 0 2px 8px rgba(255, 91, 60, 0.1);
}

.badge-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  color: #FF5B3C;
}

.badge-icon svg {
  width: 18px;
  height: 18px;
}

.badge-text {
  font-size: 14px;
  font-weight: 600;
  color: var(--text-primary, #1b1b1b);
}

/* ========================================
   CONTENT GRID (Two Column Layout)
   ======================================== */
.content-grid {
  display: grid;
  grid-template-columns: 1fr 360px;
  gap: 32px;
  align-items: start;
}

.content-main {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

/* ========================================
   STATS CARDS
   ======================================== */
.stats-cards {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
}

.stat-card {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 20px;
  background: var(--bg-card, #ffffff);
  border: 1px solid var(--border-color, rgba(27, 27, 27, 0.1));
  border-radius: 16px;
  transition: all 0.25s;
}

.stat-card:hover {
  border-color: var(--color-brand, #FF5B3C);
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(255, 91, 60, 0.1);
}

.stat-card .stat-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 48px;
  height: 48px;
  color: #FF5B3C;
  background: rgba(255, 91, 60, 0.08);
  border-radius: 12px;
  flex-shrink: 0;
}

.stat-card .stat-icon svg {
  width: 28px;
  height: 28px;
}

.stat-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.stat-info .stat-value {
  font-size: 18px;
  font-weight: 700;
  color: var(--text-primary, #1b1b1b);
}

.stat-info .stat-label {
  font-size: 12px;
  color: var(--text-muted, #9ca3af);
}

/* ========================================
   SECTION CARDS
   ======================================== */
.section-card {
  background: var(--bg-card, #ffffff);
  border: 1px solid var(--border-color, rgba(27, 27, 27, 0.1));
  border-radius: 20px;
  padding: 24px;
}

.section-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 20px;
}

.section-header h3 {
  font-size: 18px;
  font-weight: 600;
  margin: 0;
  color: var(--text-primary, #1b1b1b);
}

.section-count {
  font-size: 13px;
  color: var(--text-muted, #9ca3af);
  background: var(--bg-secondary, #f7f7f7);
  padding: 6px 12px;
  border-radius: 20px;
}

/* ========================================
   SIDEBAR
   ======================================== */
.content-sidebar {
  position: sticky;
  top: 24px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.sidebar-card {
  background: var(--bg-card, #ffffff);
  border: 1px solid var(--border-color, rgba(27, 27, 27, 0.1));
  border-radius: 20px;
  padding: 24px;
}

.status-card {
  text-align: center;
}

.status-badge {
  display: inline-block;
  padding: 8px 20px;
  border-radius: 24px;
  font-size: 14px;
  font-weight: 600;
}

.badge-success { background: rgba(34, 197, 94, 0.15); color: #16a34a; }
.badge-warning { background: rgba(245, 158, 11, 0.15); color: #d97706; }
.badge-danger { background: rgba(239, 68, 68, 0.15); color: #dc2626; }
.badge-secondary { background: var(--bg-secondary, #f7f7f7); color: var(--text-secondary, #6b7280); }
.badge-dark { background: #374151; color: white; }

.specs-card h4 {
  font-size: 16px;
  font-weight: 600;
  margin: 0 0 16px 0;
  color: var(--text-primary, #1b1b1b);
}

.specs-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.spec-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-bottom: 12px;
  border-bottom: 1px solid var(--border-color, rgba(27, 27, 27, 0.08));
}

.spec-row:last-child {
  border-bottom: none;
  padding-bottom: 0;
}

.spec-name {
  font-size: 13px;
  color: var(--text-muted, #9ca3af);
}

.spec-val {
  font-size: 14px;
  font-weight: 600;
  color: var(--text-primary, #1b1b1b);
}

.actions-card {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  padding: 12px 24px;
  border-radius: 100px !important;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  border: none;
  transition: all 0.2s;
}

.btn-full {
  width: 100%;
}

.btn-primary {
  background: #1B1B1B !important;
  color: white !important;
}

.btn-primary:hover {
  background: #333333 !important;
}

.btn-danger-outline {
  background: #F7F7F7;
  color: #dc2626;
}

.btn-danger-outline:hover {
  background: rgba(220, 38, 38, 0.1);
}

.btn-secondary {
  background: #F7F7F7;
  color: #1B1B1B;
}

.btn-secondary:hover {
  background: #EBEBEB;
}

/* ========================================
   RESPONSIVE
   ======================================== */
@media (max-width: 1024px) {
  .content-grid {
    grid-template-columns: 1fr;
  }

  .content-sidebar {
    position: static;
    flex-direction: row;
    flex-wrap: wrap;
  }

  .sidebar-card {
    flex: 1;
    min-width: 200px;
  }

  .stats-cards {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .gallery-mosaic {
    grid-template-columns: 1fr;
  }

  .gallery-main {
    border-radius: 16px;
  }

  .gallery-thumbs {
    grid-template-columns: repeat(4, 1fr);
    margin-top: 8px;
  }

  .thumb-item {
    border-radius: 8px !important;
    aspect-ratio: 1;
  }

  .stats-cards {
    grid-template-columns: 1fr 1fr;
  }

  .title-section h1 {
    font-size: 22px;
  }
}

/* Old compatibility (keep .thumbnail for any remaining references) */
.thumbnail {
  opacity: 1;
  box-shadow: 0 0 0 2px #4f46e5;
}

.thumbnail img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

/* Info Section */
.info-section {
  background: white;
  border-radius: 16px;
  padding: 24px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.info-header {
  margin-bottom: 24px;
}

.title-row {
  display: flex;
  gap: 12px;
  margin-bottom: 12px;
}

.type-badge {
  background: #ede9fe;
  color: #7c3aed;
  padding: 4px 12px;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 600;
}

.status-badge {
  padding: 4px 12px;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 600;
}

.badge-success { background: #d1fae5; color: #059669; }
.badge-warning { background: #fef3c7; color: #d97706; }
.badge-danger { background: #fee2e2; color: #dc2626; }
.badge-secondary { background: #f3f4f6; color: #6b7280; }
.badge-dark { background: #374151; color: white; }

.info-header h1 {
  font-size: 24px;
  font-weight: 700;
  color: #1a1a2e;
  margin: 0 0 4px 0;
}

.location {
  color: #6b7280;
  font-size: 14px;
  margin: 0;
}

/* Stats Grid */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 16px;
  margin-bottom: 24px;
}

.stat-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 16px;
  background: #f9fafb;
  border-radius: 12px;
}

.stat-icon {
  font-size: 24px;
}

.stat-content {
  display: flex;
  flex-direction: column;
}

.stat-value {
  font-size: 18px;
  font-weight: 700;
  color: #1a1a2e;
}

.stat-label {
  font-size: 12px;
  color: #6b7280;
}

/* Details Table */
.details-section {
  margin-bottom: 24px;
}

.details-section h3,
.rooms-section h3 {
  font-size: 16px;
  font-weight: 600;
  margin: 0 0 12px 0;
}

.details-table {
  width: 100%;
  border-collapse: collapse;
}

.details-table td {
  padding: 12px 0;
  border-bottom: 1px solid #f3f4f6;
  font-size: 14px;
}

.details-table td:first-child {
  color: #6b7280;
}

.details-table td:last-child {
  text-align: right;
  font-weight: 500;
}

/* Rooms */
.rooms-section {
  margin-bottom: 24px;
}

.rooms-grid {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
}

.room-card {
  display: flex;
  flex-direction: column;
  padding: 12px 16px;
  background: #f3f4f6;
  border-radius: 10px;
}

.room-type {
  font-weight: 600;
  font-size: 14px;
  color: #374151;
}

.room-area {
  font-size: 12px;
  color: #6b7280;
}

/* Actions */
.actions-section {
  display: flex;
  gap: 12px;
  padding-top: 24px;
  border-top: 1px solid #f3f4f6;
}

.btn {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 12px 20px;
  border-radius: 10px;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  border: none;
  transition: all 0.2s;
}

.btn-primary {
  background: linear-gradient(135deg, #4f46e5, #7c3aed);
  color: white;
}

.btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(79, 70, 229, 0.4);
}

.btn-danger {
  background: #fef2f2;
  color: #dc2626;
}

.btn-danger:hover {
  background: #dc2626;
  color: white;
}

.btn-secondary {
  background: #f3f4f6;
  color: #374151;
}

@media (max-width: 768px) {
  .detail-content {
    grid-template-columns: 1fr;
  }

  .gallery-section {
    position: static;
  }
}

/* Units Section */
.units-section {
  margin-bottom: 24px;
}

.units-section h3 {
  font-size: 16px;
  font-weight: 600;
  margin: 0 0 16px 0;
  color: #1a1a2e;
}

.units-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.unit-card {
  background: #f9fafb;
  border-radius: 12px;
  overflow: hidden;
  border: 1px solid #e5e7eb;
}

.unit-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px;
  background: linear-gradient(135deg, #4f46e5 0%, #7c3aed 100%);
  color: white;
}

.unit-number {
  font-weight: 600;
  font-size: 14px;
}

.unit-area {
  background: rgba(255, 255, 255, 0.2);
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 13px;
  font-weight: 500;
}

.unit-details {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 12px;
  padding: 16px;
}

.unit-detail {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.detail-label {
  font-size: 11px;
  color: #9ca3af;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.detail-value {
  font-size: 14px;
  font-weight: 600;
  color: #374151;
}

.unit-rooms {
  padding: 12px 16px 16px;
  border-top: 1px solid #e5e7eb;
}

.unit-rooms-label {
  display: block;
  font-size: 12px;
  color: #6b7280;
  margin-bottom: 8px;
}

.rooms-grid.compact {
  gap: 8px;
}

.room-card.small {
  padding: 8px 12px;
  background: #e5e7eb;
}

.room-card.small .room-type {
  font-size: 12px;
}

.room-card.small .room-area {
  font-size: 11px;
}

/* ========================================
   MODERN ROOMS SECTION
   ======================================== */

.section-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 16px;
}

.section-header h3 {
  font-size: 16px;
  font-weight: 600;
  margin: 0;
  color: var(--text-primary, #1a1a2e);
}

.room-count,
.unit-count {
  font-size: 12px;
  color: var(--text-muted, #6b7280);
  background: var(--bg-secondary, #f7f7f7);
  padding: 4px 10px;
  border-radius: 20px;
}

.rooms-grid-modern {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(140px, 1fr));
  gap: 12px;
}

.room-card-modern {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 14px 16px;
  background: rgba(255, 255, 255, 0.7);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 91, 60, 0.1);
  border-radius: 14px;
  transition: all 0.25s ease;
  cursor: default;
}

.room-card-modern:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(255, 91, 60, 0.12);
  border-color: rgba(255, 91, 60, 0.3);
}

.room-icon {
  font-size: 24px;
  line-height: 1;
}

.room-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.room-info .room-type {
  font-weight: 600;
  font-size: 13px;
  color: var(--text-primary, #1b1b1b);
}

.room-info .room-area {
  font-size: 12px;
  color: var(--text-secondary, rgba(27, 27, 27, 0.6));
}

/* ========================================
   MODERN UNITS SECTION (ACCORDION)
   ======================================== */

.units-section-modern {
  margin-bottom: 24px;
}

.units-accordion {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.unit-accordion-item {
  background: var(--bg-card, #ffffff);
  border: 1px solid var(--border-color, rgba(27, 27, 27, 0.12));
  border-radius: 16px;
  overflow: hidden;
  transition: all 0.3s ease;
}

.unit-accordion-item:hover {
  border-color: var(--color-brand, #FF5B3C);
  box-shadow: 0 4px 16px rgba(255, 91, 60, 0.1);
}

.unit-accordion-item.expanded {
  border-color: var(--color-brand, #FF5B3C);
  box-shadow: 0 8px 24px rgba(255, 91, 60, 0.15);
}

.unit-accordion-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px 20px;
  cursor: pointer;
  user-select: none;
  transition: background 0.2s ease;
}

.unit-accordion-header:hover {
  background: var(--bg-secondary, #f7f7f7);
}

.unit-header-left {
  display: flex;
  align-items: center;
  gap: 16px;
}

.unit-indicator {
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, var(--color-brand, #FF5B3C) 0%, #ff7a5c 100%);
  color: white;
  font-weight: 700;
  font-size: 14px;
  border-radius: 10px;
}

.unit-header-info {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.unit-title {
  font-weight: 600;
  font-size: 15px;
  color: var(--text-primary, #1b1b1b);
}

.unit-badges {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.unit-badge {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 4px 10px;
  font-size: 12px;
  font-weight: 500;
  border-radius: 20px;
  background: var(--bg-secondary, #f7f7f7);
  color: var(--text-secondary, rgba(27, 27, 27, 0.7));
}

.unit-badge.area {
  background: rgba(255, 91, 60, 0.1);
  color: var(--color-brand, #FF5B3C);
}

.unit-badge.rooms {
  background: rgba(79, 70, 229, 0.1);
  color: #4f46e5;
}

.unit-badge.floor {
  background: rgba(34, 197, 94, 0.1);
  color: #22c55e;
}

.expand-icon {
  font-size: 12px;
  color: var(--text-muted, #9ca3af);
  transition: transform 0.3s ease;
}

.expand-icon.rotated {
  transform: rotate(180deg);
}

.unit-accordion-content {
  padding: 0 20px 20px;
  border-top: 1px solid var(--border-color, rgba(27, 27, 27, 0.08));
  animation: slideDown 0.3s ease;
}

@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.unit-specs-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 16px;
  padding: 16px 0;
}

.unit-spec {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.spec-label {
  font-size: 11px;
  color: var(--text-muted, #9ca3af);
  text-transform: uppercase;
  letter-spacing: 0.5px;
  font-weight: 500;
}

.spec-value {
  font-size: 15px;
  font-weight: 600;
  color: var(--text-primary, #374151);
}

.unit-rooms-section {
  padding-top: 16px;
  border-top: 1px solid var(--border-color, rgba(27, 27, 27, 0.08));
}

.unit-rooms-title {
  display: block;
  font-size: 12px;
  font-weight: 600;
  color: var(--text-secondary, #6b7280);
  margin-bottom: 12px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.rooms-grid-compact {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.room-chip {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 8px 12px;
  background: linear-gradient(135deg, rgba(255, 91, 60, 0.08) 0%, rgba(255, 122, 92, 0.08) 100%);
  border: 1px solid rgba(255, 91, 60, 0.15);
  border-radius: 24px;
  transition: all 0.2s ease;
}

.room-chip:hover {
  background: linear-gradient(135deg, rgba(255, 91, 60, 0.15) 0%, rgba(255, 122, 92, 0.15) 100%);
  transform: translateY(-1px);
}

.chip-icon {
  font-size: 16px;
}

.chip-text {
  font-size: 12px;
  font-weight: 600;
  color: var(--text-primary, #1b1b1b);
}

.chip-area {
  font-size: 11px;
  color: var(--text-muted, #9ca3af);
  padding-left: 6px;
  border-left: 1px solid rgba(27, 27, 27, 0.1);
}

/* Mobile Responsive */
@media (max-width: 768px) {
  .unit-specs-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .unit-badges {
    flex-direction: column;
    align-items: flex-start;
    gap: 4px;
  }

  .rooms-grid-modern {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>
