<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { api } from '@/services/api'
import type { Building, Floor } from '@/types/building'
import type { RealEstate } from '@/types/realestate'
import FloorModal from '@/components/FloorModal.vue'

const router = useRouter()
const route = useRoute()

const buildingId = route.params.id as string
const building = ref<Building | null>(null)
const floors = ref<Floor[]>([])
const realEstates = ref<RealEstate[]>([])

const currentBuildingImageIndex = ref(0)
const isLoading = ref(true)
const isLoadingFloors = ref(false)
const isLoadingRealEstates = ref(false)
const errorMessage = ref('')
const isDeleting = ref(false)

// Floor modal state
const isFloorModalOpen = ref(false)
const selectedFloor = ref<Floor | null>(null)
const deletingFloorId = ref<string | null>(null)
const floorToDelete = ref<Floor | null>(null)
const showDeleteConfirm = ref(false)

onMounted(async () => {
  try {
    building.value = await api.getBuildingById(buildingId)
    // Load floors and real estates in parallel
    await Promise.all([loadFloors(), loadRealEstates()])
  } catch (error) {
    console.error('Failed to load building:', error)
    errorMessage.value = error instanceof Error ? error.message : 'Error loading building'
  } finally {
    isLoading.value = false
  }
})

// Building image navigation
function nextBuildingImage() {
  if (building.value?.images && currentBuildingImageIndex.value < building.value.images.length - 1) {
    currentBuildingImageIndex.value++
  }
}

function prevBuildingImage() {
  if (currentBuildingImageIndex.value > 0) {
    currentBuildingImageIndex.value--
  }
}

function getCurrentBuildingImageUrl(): string | undefined {
  return building.value?.images?.[currentBuildingImageIndex.value]
}

async function loadFloors() {
  isLoadingFloors.value = true
  try {
    const response = await api.getFloors({ buildingId, pageSize: 100 })
    floors.value = (response.items || []).sort((a: Floor, b: Floor) => a.number - b.number)
  } catch (error) {
    console.error('Failed to load floors:', error)
  } finally {
    isLoadingFloors.value = false
  }
}

async function loadRealEstates() {
  isLoadingRealEstates.value = true
  try {
    const response = await api.getRealEstates({ buildingId, pageSize: 100 })
    realEstates.value = response.items || []
  } catch (error) {
    console.error('Failed to load real estates:', error)
  } finally {
    isLoadingRealEstates.value = false
  }
}

function goBack() {
  router.push('/buildings')
}

function editBuilding() {
  router.push(`/buildings/${buildingId}/edit`)
}

async function deleteBuilding() {
  if (!confirm('Are you sure you want to delete this building?')) return

  isDeleting.value = true
  try {
    await api.deleteBuilding(buildingId)
    router.push('/buildings')
  } catch (error) {
    console.error('Failed to delete building:', error)
    errorMessage.value = error instanceof Error ? error.message : 'Error deleting building'
  } finally {
    isDeleting.value = false
  }
}

// Floor management
function openAddFloorModal() {
  selectedFloor.value = null
  isFloorModalOpen.value = true
}

function openEditFloorModal(floor: Floor) {
  selectedFloor.value = floor
  isFloorModalOpen.value = true
}

function closeFloorModal() {
  isFloorModalOpen.value = false
  selectedFloor.value = null
}

async function handleFloorSaved() {
  // Wait 1 second before fetching images to allow MinIO to process the upload
  await new Promise(resolve => setTimeout(resolve, 1000))
  await loadFloors()
}

async function handleDeleteFloor(event: Event, floor: Floor) {
  event.preventDefault()
  event.stopPropagation()

  // Show confirm modal
  floorToDelete.value = floor
  showDeleteConfirm.value = true
}

function cancelDeleteFloor() {
  floorToDelete.value = null
  showDeleteConfirm.value = false
}

async function confirmDeleteFloor() {
  if (!floorToDelete.value) return

  const floor = floorToDelete.value
  showDeleteConfirm.value = false
  deletingFloorId.value = floor.id

  try {
    await api.deleteFloor(floor.id)
    floors.value = floors.value.filter(f => f.id !== floor.id)
  } catch (error) {
    console.error('Failed to delete floor:', error)
    errorMessage.value = error instanceof Error ? error.message : 'Error deleting floor'
  } finally {
    deletingFloorId.value = null
    floorToDelete.value = null
  }
}

function getFloorPlanImage(floor: Floor): string | undefined {
  return floor.plan || undefined
}

function getRealEstateImage(re: RealEstate): string | undefined {
  return re.images?.[0] || undefined
}

function goToRealEstateDetail(id: string) {
  router.push(`/realestates/${id}`)
}

function goToCreateRealEstate() {
  router.push(`/realestates/new?buildingId=${buildingId}`)
}
</script>

<template>
  <div class="building-detail-page">
    <!-- Header -->
    <div class="page-header">
      <button class="btn btn-ghost" @click="goBack">
        ← Back
      </button>
      <h1 class="page-title">{{ building?.number || 'Building' }}</h1>
      <div class="page-actions">
        <button class="btn btn-ghost" @click="editBuilding">
          ✏️ Edit
        </button>
        <button class="btn btn-ghost text-error" @click="deleteBuilding" :disabled="isDeleting">
          🗑️ Delete
        </button>
      </div>
    </div>

    <!-- Error Alert -->
    <div v-if="errorMessage" class="alert alert-error mb-lg">
      {{ errorMessage }}
      <button class="alert-close" @click="errorMessage = ''">✕</button>
    </div>

    <!-- Loading -->
    <div v-if="isLoading" class="loading-state">
      <div class="spinner spinner-lg"></div>
    </div>

    <!-- Content -->
    <div v-else-if="building" class="building-content">

      <!-- Building Images Gallery -->
      <div v-if="building.images && building.images.length > 0" class="building-gallery">
        <div class="gallery-main">
          <img
            v-if="getCurrentBuildingImageUrl()"
            :src="getCurrentBuildingImageUrl()!"
            alt="Building Photo"
          />
          <div v-else class="image-loading">
            <div class="spinner"></div>
          </div>

          <!-- Navigation arrows -->
          <template v-if="building.images.length > 1">
            <button
              class="nav-arrow prev"
              @click="prevBuildingImage"
              :disabled="currentBuildingImageIndex === 0"
            >
              ‹
            </button>
            <button
              class="nav-arrow next"
              @click="nextBuildingImage"
              :disabled="currentBuildingImageIndex === building.images.length - 1"
            >
              ›
            </button>
          </template>

          <!-- Image counter -->
          <div class="image-counter" v-if="building.images.length > 1">
            {{ currentBuildingImageIndex + 1 }} / {{ building.images.length }}
          </div>
        </div>

        <!-- Thumbnail Grid -->
        <div v-if="building.images.length > 1" class="gallery-thumbs">
          <div
            v-for="(key, index) in building.images.slice(0, 6)"
            :key="key"
            class="thumb-item"
            :class="{ active: index === currentBuildingImageIndex }"
            @click="currentBuildingImageIndex = index"
          >
            <img :src="key" alt="" />

          </div>
          <div v-if="building.images.length > 6" class="thumb-more">
            +{{ building.images.length - 6 }}
          </div>
        </div>
      </div>

      <!-- Info Card -->
      <div class="info-card">
        <h2>Basic Information</h2>

        <div class="info-grid">
          <div class="info-item">
            <label>Number</label>
            <span>{{ building.number || '—' }}</span>
          </div>
          <div class="info-item">
            <label>Address</label>
            <span>{{ building.address || '—' }}</span>
          </div>
          <div class="info-item">
            <label>Area</label>
            <span>{{ building.totalArea ? `${building.totalArea} м²` : '—' }}</span>
          </div>
          <div class="info-item">
            <label>Floors</label>
            <span>{{ building.floorsCount || '—' }}</span>
          </div>
          <div class="info-item">
            <label>Type</label>
            <div class="badges">
              <span v-if="building.isCommercial" class="badge badge-commercial">Commercial</span>
              <span v-if="building.isLiving" class="badge badge-living">Residential</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Description -->
      <div v-if="building.descriptions?.length" class="info-card">
        <h2>Description</h2>
        <div v-for="desc in building.descriptions" :key="desc.language" class="description-item">
          <span class="lang-badge">{{ desc.language.toUpperCase() }}</span>
          <p>{{ desc.value }}</p>
        </div>
      </div>

      <!-- Floors Section -->
      <div class="info-card floors-section">
        <div class="card-header">
          <h2>Floors</h2>
          <button class="btn btn-primary btn-sm" @click="openAddFloorModal">
            ➕ Add Floor
          </button>
        </div>

        <!-- Loading Floors -->
        <div v-if="isLoadingFloors" class="loading-state-sm">
          <div class="spinner"></div>
        </div>

        <!-- Floors Grid -->
        <div v-else-if="floors.length > 0" class="floors-grid">
          <div
            v-for="floor in floors"
            :key="floor.id"
            class="floor-card"
          >
            <div class="floor-card-header">
              <div class="floor-number-badge">{{ floor.number }}</div>
              <div class="floor-actions">
                <button
                  type="button"
                  class="floor-action-btn"
                  @click="openEditFloorModal(floor)"
                  title="Edit"
                >
                  ✏️
                </button>
                <button
                  type="button"
                  class="floor-action-btn floor-action-delete"
                  @click.prevent.stop="handleDeleteFloor($event, floor)"
                  :disabled="deletingFloorId === floor.id"
                  title="Delete"
                >
                  <template v-if="deletingFloorId === floor.id">
                    <span class="spinner spinner-sm"></span>
                  </template>
                  <template v-else>🗑️</template>
                </button>
              </div>
            </div>

            <div class="floor-card-body">
              <!-- Floor Plan Thumbnail -->
              <div v-if="floor.plan" class="floor-plan-thumb">
                <img
                  v-if="getFloorPlanImage(floor)"
                  :src="getFloorPlanImage(floor)"
                  alt="Floor plan"
                />
                <div v-else class="floor-plan-loading">
                  <span class="spinner spinner-sm"></span>
                </div>
              </div>

              <div class="floor-info">
                <div v-if="floor.totalArea" class="floor-stat">
                  <span class="floor-stat-label">Area</span>
                  <span class="floor-stat-value">{{ floor.totalArea }} м²</span>
                </div>
                <div v-if="floor.ceilingHeight" class="floor-stat">
                  <span class="floor-stat-label">Height</span>
                  <span class="floor-stat-value">{{ floor.ceilingHeight }} м</span>
                </div>
              </div>

              <p v-if="floor.note" class="floor-note">{{ floor.note }}</p>
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div v-else class="empty-message">
          <div class="empty-icon">🏗️</div>
          <p>No floors yet</p>
          <button class="btn btn-secondary btn-sm" @click="openAddFloorModal">
            Add first floor
          </button>
        </div>
      </div>

      <!-- Real Estates Section -->
      <div class="info-card">
        <div class="card-header">
          <h2>Real Estate in Building</h2>
          <button class="btn btn-primary btn-sm" @click="goToCreateRealEstate">
            ➕ Add
          </button>
        </div>

        <!-- Loading Real Estates -->
        <div v-if="isLoadingRealEstates" class="loading-state-sm">
          <div class="spinner"></div>
        </div>

        <!-- Real Estates Grid -->
        <div v-else-if="realEstates.length > 0" class="real-estates-grid">
          <div
            v-for="re in realEstates"
            :key="re.id"
            class="real-estate-card"
            @click="goToRealEstateDetail(re.id)"
          >
            <div class="real-estate-image">
              <img
                v-if="getRealEstateImage(re)"
                :src="getRealEstateImage(re)"
                alt="Real Estate"
              />
              <div v-else class="real-estate-placeholder">🏠</div>
            </div>
            <div class="real-estate-info">
              <h4>{{ re.number || re.realEstateTypeName || 'Property' }}</h4>
              <div class="real-estate-meta">
                <span v-if="re.totalArea">{{ re.totalArea }} м²</span>
                <span v-if="re.floorNumber">Floor {{ re.floorNumber }}</span>
                <span v-if="re.roomsCount">{{ re.roomsCount }} rooms</span>
              </div>
              <div v-if="re.status" class="real-estate-status">
                <span :class="['status-badge', `status-${re.status?.toLowerCase()}`]">
                  {{ re.status === 'Draft' ? 'Draft' : re.status === 'Active' ? 'Active' : re.status }}
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div v-else class="empty-message">
          <div class="empty-icon">🏠</div>
          <p>No real estate in this building yet</p>
          <button class="btn btn-secondary btn-sm" @click="goToCreateRealEstate">
            Add property
          </button>
        </div>
      </div>
    </div>

    <!-- Floor Modal -->
    <FloorModal
      :buildingId="buildingId"
      :floor="selectedFloor"
      :isOpen="isFloorModalOpen"
      @close="closeFloorModal"
      @saved="handleFloorSaved"
    />

    <!-- Delete Confirmation Modal -->
    <Teleport to="body">
      <Transition name="modal">
        <div v-if="showDeleteConfirm" class="confirm-backdrop" @click.self="cancelDeleteFloor">
          <div class="confirm-dialog">
            <div class="confirm-icon">⚠️</div>
            <h3>Delete Floor {{ floorToDelete?.number }}?</h3>
            <p>This action cannot be undone.</p>
            <div class="confirm-actions">
              <button class="btn btn-secondary" @click="cancelDeleteFloor">
                Cancel
              </button>
              <button class="btn btn-danger" @click="confirmDeleteFloor">
                🗑️ Delete
              </button>
            </div>
          </div>
        </div>
      </Transition>
    </Teleport>
  </div>
</template>

<style scoped>
.building-detail-page {
  max-width: 900px;
}

/* Building Images Gallery */
.building-gallery {
  margin-bottom: 1.5rem;
  border-radius: var(--radius-xl);
  overflow: hidden;
  background: var(--color-surface, var(--bg-card));
}

.gallery-main {
  position: relative;
  aspect-ratio: 16/9;
  background: var(--bg-secondary, #f7f7f7);
}

.gallery-main img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.image-loading {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--bg-secondary, #f7f7f7);
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
  display: flex;
  align-items: center;
  justify-content: center;
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

.image-counter {
  position: absolute;
  bottom: 16px;
  right: 16px;
  background: rgba(0, 0, 0, 0.6);
  color: white;
  padding: 6px 12px;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 500;
}

.gallery-thumbs {
  display: flex;
  gap: 8px;
  padding: 12px;
  background: var(--color-surface, var(--bg-card));
}

.thumb-item {
  width: 80px;
  height: 60px;
  border-radius: var(--radius-md);
  overflow: hidden;
  cursor: pointer;
  border: 2px solid transparent;
  transition: all 0.2s;
}

.thumb-item:hover {
  opacity: 0.8;
}

.thumb-item.active {
  border-color: var(--color-brand, #FF5B3C);
}

.thumb-item img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.thumb-loading {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--bg-secondary, #f7f7f7);
}

.thumb-more {
  width: 80px;
  height: 60px;
  border-radius: var(--radius-md);
  background: rgba(0, 0, 0, 0.6);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  font-size: 0.9rem;
}

.page-header {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 2rem;
}

.page-title {
  flex: 1;
  font-size: 1.5rem;
  font-weight: 700;
  margin: 0;
}

.page-actions {
  display: flex;
  gap: 0.5rem;
}

.loading-state {
  text-align: center;
  padding: 4rem;
  background: var(--color-surface, var(--bg-card));
  border-radius: var(--radius-xl);
}

.loading-state-sm {
  text-align: center;
  padding: 2rem;
}

.building-content {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.info-card {
  background: var(--color-surface, var(--bg-card));
  border-radius: var(--radius-xl);
  padding: 1.5rem;
}

.info-card h2 {
  font-size: 1.1rem;
  font-weight: 600;
  margin: 0 0 1.25rem;
  color: var(--color-text, var(--text-primary));
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.card-header h2 {
  margin: 0;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.info-item label {
  font-size: 0.75rem;
  color: var(--color-text-muted, var(--text-muted));
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.info-item span {
  font-size: 0.95rem;
  color: var(--color-text, var(--text-primary));
}

.badges {
  display: flex;
  gap: 0.5rem;
}

.badge {
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  font-size: 0.7rem;
  font-weight: 600;
}

.badge-commercial {
  background: rgba(59, 130, 246, 0.1);
  color: #3b82f6;
}

.badge-living {
  background: rgba(34, 197, 94, 0.1);
  color: #22c55e;
}

.description-item {
  display: flex;
  gap: 0.75rem;
  margin-bottom: 0.75rem;
}

.description-item:last-child {
  margin-bottom: 0;
}

.lang-badge {
  flex-shrink: 0;
  width: 28px;
  height: 28px;
  background: var(--color-background, var(--bg-secondary));
  border-radius: 4px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.65rem;
  font-weight: 600;
  color: var(--color-text-muted, var(--text-muted));
}

.description-item p {
  margin: 0;
  color: var(--color-text, var(--text-primary));
  line-height: 1.5;
}

/* Alert */
.alert {
  position: relative;
}

.alert-close {
  position: absolute;
  right: 0.75rem;
  top: 50%;
  transform: translateY(-50%);
  background: none;
  border: none;
  cursor: pointer;
  font-size: 1rem;
  opacity: 0.7;
}

.alert-close:hover {
  opacity: 1;
}

/* Floors Section */
.floors-section {
  background: linear-gradient(135deg, var(--bg-card) 0%, var(--bg-secondary) 100%);
}

.floors-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
  gap: 1rem;
}

.floor-card {
  background: var(--bg-primary);
  border-radius: var(--radius-lg);
  overflow: hidden;
  border: 1px solid var(--border-color);
  transition: all var(--transition-base);
}

.floor-card:hover {
  border-color: var(--color-brand);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.08);
}

.floor-card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background: var(--bg-secondary);
  border-bottom: 1px solid var(--border-color);
}

.floor-number-badge {
  width: 48px;
  height: 48px;
  background: linear-gradient(135deg, var(--color-brand) 0%, #ff8a6c 100%);
  color: white;
  border-radius: var(--radius-md);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  font-weight: 700;
  box-shadow: 0 4px 12px rgba(255, 91, 60, 0.3);
}

.floor-actions {
  display: flex;
  gap: 0.5rem;
  position: relative;
  z-index: 10;
}

.floor-action-btn {
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--bg-primary);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-sm);
  cursor: pointer;
  transition: all var(--transition-fast);
  font-size: 0.9rem;
}

.floor-action-btn:hover:not(:disabled) {
  background: var(--color-primary-light);
  border-color: var(--color-primary);
}

.floor-action-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.floor-action-delete:hover:not(:disabled) {
  background: var(--color-error-light);
  border-color: var(--color-error);
}

.floor-card-body {
  padding: 1rem;
}

.floor-plan-thumb {
  width: 100%;
  height: 100px;
  background: var(--bg-secondary);
  border-radius: var(--radius-md);
  margin-bottom: 1rem;
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
}

.floor-plan-thumb img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.floor-plan-loading {
  display: flex;
  align-items: center;
  justify-content: center;
}

.floor-info {
  display: flex;
  gap: 1.5rem;
  margin-bottom: 0.75rem;
}

.floor-stat {
  display: flex;
  flex-direction: column;
  gap: 0.125rem;
}

.floor-stat-label {
  font-size: 0.7rem;
  color: var(--text-muted);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.floor-stat-value {
  font-size: 0.9rem;
  font-weight: 600;
  color: var(--text-primary);
}

.floor-note {
  font-size: 0.8rem;
  color: var(--text-secondary);
  margin: 0;
  line-height: 1.4;
}

/* Real Estates Section */
.real-estates-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1rem;
}

.real-estate-card {
  display: flex;
  gap: 1rem;
  padding: 1rem;
  background: var(--bg-secondary);
  border-radius: var(--radius-lg);
  cursor: pointer;
  transition: all var(--transition-base);
  border: 1px solid transparent;
}

.real-estate-card:hover {
  border-color: var(--color-brand);
  background: var(--color-brand-light);
}

.real-estate-image {
  width: 80px;
  height: 80px;
  border-radius: var(--radius-md);
  overflow: hidden;
  flex-shrink: 0;
  background: var(--bg-primary);
  display: flex;
  align-items: center;
  justify-content: center;
}

.real-estate-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.real-estate-placeholder {
  font-size: 2rem;
  opacity: 0.3;
}

.real-estate-info {
  flex: 1;
  min-width: 0;
}

.real-estate-info h4 {
  font-size: 0.95rem;
  font-weight: 600;
  margin: 0 0 0.5rem;
  color: var(--text-primary);
}

.real-estate-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
  font-size: 0.8rem;
  color: var(--text-muted);
  margin-bottom: 0.5rem;
}

.real-estate-status {
  margin-top: 0.25rem;
}

.status-badge {
  padding: 0.2rem 0.5rem;
  border-radius: 4px;
  font-size: 0.7rem;
  font-weight: 600;
}

.status-draft {
  background: rgba(107, 114, 128, 0.1);
  color: #6b7280;
}

.status-active {
  background: rgba(34, 197, 94, 0.1);
  color: #22c55e;
}

.status-inactive {
  background: rgba(239, 68, 68, 0.1);
  color: #ef4444;
}

.empty-message {
  text-align: center;
  padding: 2rem;
  color: var(--color-text-muted, var(--text-muted));
}

.empty-icon {
  font-size: 2.5rem;
  margin-bottom: 0.75rem;
}

.empty-message p {
  margin: 0 0 1rem;
}

.btn-sm {
  padding: 0.5rem 0.75rem;
  font-size: 0.8rem;
}

.spinner-sm {
  width: 14px;
  height: 14px;
  border-width: 2px;
}

/* Confirmation Modal */
.confirm-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(4px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 1rem;
}

.confirm-dialog {
  background: var(--bg-primary);
  border-radius: var(--radius-xl);
  padding: 2rem;
  text-align: center;
  max-width: 360px;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
}

.confirm-icon {
  font-size: 3rem;
  margin-bottom: 1rem;
}

.confirm-dialog h3 {
  font-size: 1.25rem;
  margin: 0 0 0.5rem;
  color: var(--text-primary);
}

.confirm-dialog p {
  color: var(--text-muted);
  margin: 0 0 1.5rem;
  font-size: 0.9rem;
}

.confirm-actions {
  display: flex;
  gap: 0.75rem;
  justify-content: center;
}

.btn-danger {
  background: var(--color-error);
  color: white;
}

.btn-danger:hover:not(:disabled) {
  background: #dc2626;
}

/* Modal transitions */
.modal-enter-active,
.modal-leave-active {
  transition: all 0.25s ease;
}

.modal-enter-active .confirm-dialog,
.modal-leave-active .confirm-dialog {
  transition: all 0.25s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-from .confirm-dialog,
.modal-leave-to .confirm-dialog {
  transform: scale(0.95) translateY(-10px);
  opacity: 0;
}
</style>
