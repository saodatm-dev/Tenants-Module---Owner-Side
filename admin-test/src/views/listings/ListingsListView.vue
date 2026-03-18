<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '@/services/api'
import type { MyListing, Category } from '@/types/listing'

const router = useRouter()

// State
const allListings = ref<MyListing[]>([])
const categories = ref<Category[]>([])
const loading = ref(false)
const error = ref('')

// Filters
const filters = ref({
  categoryId: '',
  filter: ''
})

// Computed filtered list
const listings = computed(() => allListings.value)

// Image handling - store ALL images per listing
const listingImages = ref<Map<string, string[]>>(new Map())
const currentImageIndex = ref<Map<string, number>>(new Map())

// Load listings using /listings/my endpoint
async function loadListings() {
  loading.value = true
  error.value = ''
  try {
    const params: { page: number; pageSize: number; filter?: string } = { page: 1, pageSize: 50 }
    if (filters.value.filter) {
      params.filter = filters.value.filter
    }
    const response = await api.getMyListings(params)
    // Map API response fields
    allListings.value = (response.items || []).map((item: MyListing) => ({
      ...item,
      regionName: item.region,
      districtName: item.district
    }))
    // Load listing images directly from URLs in response
    loadListingImages()
  } catch (e: unknown) {
    const err = e as Error
    error.value = err.message || 'Error loading listings'
    console.error('Failed to load listings:', e)
  } finally {
    loading.value = false
  }
}

// Load categories for filter
async function loadCategories() {
  try {
    const response = await api.getCategories()
    categories.value = response?.items || response || []
  } catch (e) {
    console.error('Failed to load categories:', e)
  }
}

// Load ALL images for each listing - objectNames now contain full URLs
function loadListingImages() {
  for (const listing of allListings.value) {
    if (listing.objectNames && listing.objectNames.length > 0) {
      listingImages.value.set(listing.id, listing.objectNames)
      currentImageIndex.value.set(listing.id, 0)
    }
  }
}

// Get current image for display
function getListingImage(listing: MyListing): string | null {
  const images = listingImages.value.get(listing.id)
  if (!images || images.length === 0) return null
  const idx = currentImageIndex.value.get(listing.id) || 0
  return images[idx]
}

// Get image count for listing
function getImageCount(listing: MyListing): number {
  return listingImages.value.get(listing.id)?.length || 0
}

// Navigate carousel
function prevImage(listingId: string, event: Event) {
  event.stopPropagation()
  const images = listingImages.value.get(listingId)
  if (!images || images.length <= 1) return
  const current = currentImageIndex.value.get(listingId) || 0
  const newIdx = current === 0 ? images.length - 1 : current - 1
  currentImageIndex.value.set(listingId, newIdx)
}

function nextImage(listingId: string, event: Event) {
  event.stopPropagation()
  const images = listingImages.value.get(listingId)
  if (!images || images.length <= 1) return
  const current = currentImageIndex.value.get(listingId) || 0
  const newIdx = current === images.length - 1 ? 0 : current + 1
  currentImageIndex.value.set(listingId, newIdx)
}

// Format price
function formatPrice(price?: number): string {
  if (!price) return 'Not specified'
  return new Intl.NumberFormat('ru-RU', {
    style: 'currency',
    currency: 'UZS',
    maximumFractionDigits: 0
  }).format(price)
}

// Navigation
function navigateToCreate() {
  router.push('/listings/new')
}

function navigateToDetail(id: string) {
  router.push(`/listings/${id}`)
}

function navigateToEdit(id: string) {
  router.push(`/listings/${id}/edit`)
}

// Delete
async function handleDelete(id: string) {
  if (!confirm('Are you sure you want to delete this listing?')) return
  try {
    await api.deleteListing(id)
    allListings.value = allListings.value.filter(l => l.id !== id)
  } catch (e: any) {
    alert(e.message || 'Delete error')
  }
}

// Lifecycle
onMounted(async () => {
  await Promise.all([loadListings(), loadCategories()])
})

onUnmounted(() => {
  // Presigned URLs don't need cleanup - only blob URLs do
  // Keep this for any future local blob URLs
})
</script>

<template>
  <div class="listings-list-view">
    <div class="page-header">
      <div class="header-content">
        <h1>Listings</h1>
        <p class="subtitle">Commercial rental offers</p>
      </div>
      <button class="btn btn-primary" @click="navigateToCreate">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M12 5v14"></path>
          <path d="M5 12h14"></path>
        </svg>
        Create Listing
      </button>
    </div>

    <!-- Filters -->
    <div class="filters-section">
      <div class="filter-group">
        <label>Category</label>
        <select v-model="filters.categoryId" @change="loadListings">
          <option value="">All Categories</option>
          <option v-for="cat in categories" :key="cat.id" :value="cat.id">
            {{ cat.name }}
          </option>
        </select>
      </div>
      <div class="filter-group">
        <label>Search</label>
        <input
          v-model="filters.filter"
          type="text"
          placeholder="Search..."
          @keyup.enter="loadListings"
        />
      </div>
      <button class="btn btn-secondary" @click="loadListings">
        Apply
      </button>
    </div>

    <!-- Loading state -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Loading listings...</p>
    </div>

    <!-- Error state -->
    <div v-else-if="error" class="error-state">
      <p>{{ error }}</p>
      <button class="btn btn-secondary" @click="loadListings">Retry</button>
    </div>

    <!-- Empty state -->
    <div v-else-if="listings.length === 0" class="empty-state">
      <div class="empty-icon">📋</div>
      <h3>No Listings</h3>
      <p>Create your first commercial listing</p>
      <button class="btn btn-primary" @click="navigateToCreate">
        Create Listing
      </button>
    </div>

    <!-- Listings grid -->
    <div v-else class="listings-grid">
      <div
        v-for="listing in listings"
        :key="listing.id"
        class="listing-card"
        @click="navigateToDetail(listing.id)"
      >
        <div class="card-image">
          <img
            v-if="getListingImage(listing)"
            :src="getListingImage(listing)!"
            alt="Listing"
          />
          <div v-else class="image-placeholder">
            <span>🏠</span>
          </div>
          <!-- Carousel Navigation -->
          <template v-if="getImageCount(listing) > 1">
            <button class="carousel-nav prev" @click="prevImage(listing.id, $event)">
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M15 18l-6-6 6-6"/>
              </svg>
            </button>
            <button class="carousel-nav next" @click="nextImage(listing.id, $event)">
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M9 18l6-6-6-6"/>
              </svg>
            </button>
            <!-- Image Counter -->
            <div class="image-counter">
              {{ (currentImageIndex.get(listing.id) || 0) + 1 }} / {{ getImageCount(listing) }}
            </div>
          </template>
        </div>
        <div class="card-content">
          <div class="card-header">
            <div class="categories">
              <span class="category-badge" v-for="cat in listing.categories?.slice(0, 2)" :key="cat">
                {{ cat }}
              </span>
            </div>
          </div>
          <h3 class="card-title">
            {{ listing.address || `${listing.regionName || ''} ${listing.districtName || ''}`.trim() || 'No address' }}
          </h3>
          <div class="card-details">
            <span class="detail" v-if="listing.totalArea">
              <span class="icon">📐</span>
              {{ listing.totalArea }} м²
            </span>
            <span class="detail" v-if="listing.roomsCount">
              <span class="icon">🚪</span>
              {{ listing.roomsCount }} rooms
            </span>
            <span class="detail" v-if="listing.floorNumbers?.length">
              <span class="icon">🏢</span>
              {{ listing.floorNumbers.join(', ') }} floor
            </span>
          </div>
          <div class="card-price" v-if="listing.priceForMonth">
            {{ formatPrice(listing.priceForMonth) }} / mo
          </div>
        </div>
        <div class="card-actions" @click.stop>
          <button class="btn-icon" @click="navigateToEdit(listing.id)" title="Edit">
            ✏️
          </button>
          <button class="btn-icon btn-danger" @click="handleDelete(listing.id)" title="Delete">
            🗑️
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.listings-list-view {
  padding: 20px 24px;
  max-width: 1400px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.header-content h1 {
  font-size: 24px;
  font-weight: 700;
  color: #1B1B1B;
  margin: 0;
}

.subtitle {
  color: rgba(27, 27, 27, 0.5);
  margin: 4px 0 0 0;
  font-size: 14px;
}

/* Buttons - Matching RealEstates style */
.btn {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 10px 18px;
  border-radius: 8px;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  border: none;
  transition: all 0.2s;
}

.btn-primary {
  background: #1B1B1B;
  color: #fff;
}

.btn-primary:hover {
  background: #333;
}

.btn-secondary {
  background: #F7F7F7;
  color: #1B1B1B;
}

.btn-secondary:hover {
  background: #EBEBEB;
}

/* Filters */
.filters-section {
  display: flex;
  gap: 16px;
  align-items: flex-end;
  margin-bottom: 24px;
  padding: 16px;
  background: #FFFFFF;
  border-radius: 16px;
  border: 0.5px solid rgba(27, 27, 27, 0.04);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.filter-group label {
  font-size: 12px;
  font-weight: 600;
  color: rgba(27, 27, 27, 0.5);
}

.filter-group select,
.filter-group input {
  padding: 10px 14px;
  border: 1px solid rgba(27, 27, 27, 0.08);
  border-radius: 8px;
  font-size: 14px;
  min-width: 180px;
  background: #F7F7F7;
  color: #1B1B1B;
}

.filter-group select:focus,
.filter-group input:focus {
  outline: none;
  border-color: #FF5B3C;
  box-shadow: 0 0 0 3px rgba(255, 91, 60, 0.1);
}

/* States */
.loading-state,
.error-state,
.empty-state {
  text-align: center;
  padding: 60px 20px;
  background: #FFFFFF;
  border-radius: 24px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
}

.spinner {
  width: 36px;
  height: 36px;
  border: 3px solid rgba(27, 27, 27, 0.08);
  border-top-color: #FF5B3C;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
  margin: 0 auto 12px;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.empty-icon {
  font-size: 40px;
  margin-bottom: 12px;
}

.empty-state h3 {
  font-size: 18px;
  margin: 0 0 6px 0;
  color: #1B1B1B;
}

.empty-state p {
  color: rgba(27, 27, 27, 0.5);
  margin: 0 0 16px;
  font-size: 14px;
}

/* Grid */
.listings-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 20px;
}

.listing-card {
  background: #FFFFFF;
  border-radius: 24px;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.2s ease;
  position: relative;
  border: 0.5px solid rgba(27, 27, 27, 0.04);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
}

.listing-card:hover {
  transform: translateY(-3px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.08);
  border-color: #FF5B3C;
}

.card-image {
  position: relative;
  height: 160px;
  background: #F6F6F6;
  overflow: hidden;
  border-radius: 20px 20px 0 0;
}

.card-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.image-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 36px;
  opacity: 0.25;
}

/* Carousel Navigation */
.carousel-nav {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.9);
  border: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: all 0.2s;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
  z-index: 2;
}

.carousel-nav:hover {
  background: #FFFFFF;
  transform: translateY(-50%) scale(1.1);
}

.carousel-nav.prev {
  left: 8px;
}

.carousel-nav.next {
  right: 8px;
}

.card-image:hover .carousel-nav {
  opacity: 1;
}

.image-counter {
  position: absolute;
  bottom: 8px;
  right: 8px;
  background: rgba(0, 0, 0, 0.6);
  color: white;
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 11px;
  font-weight: 500;
  z-index: 2;
}

.card-content {
  padding: 12px;
}

.card-header {
  margin-bottom: 6px;
}

.categories {
  display: flex;
  gap: 6px;
  flex-wrap: wrap;
}

.category-badge {
  background: rgba(255, 91, 60, 0.1);
  color: #FF5B3C;
  padding: 4px 10px;
  border-radius: 100px;
  font-size: 11px;
  font-weight: 600;
}

.card-title {
  font-size: 14px;
  font-weight: 600;
  color: #1B1B1B;
  margin: 0 0 8px 0;
  line-height: 1.3;
}

.card-details {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  margin-bottom: 8px;
}

.detail {
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: 12px;
  color: rgba(27, 27, 27, 0.5);
}

.detail .icon {
  font-size: 12px;
}

.card-price {
  font-size: 14px;
  font-weight: 700;
  color: #FF5B3C;
  margin: 0;
}

.card-actions {
  position: absolute;
  top: 10px;
  left: 10px;
  display: flex;
  gap: 6px;
  opacity: 0;
  transition: opacity 0.2s;
}

.listing-card:hover .card-actions {
  opacity: 1;
}

.btn-icon {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  border: none;
  background: #FFFFFF;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.12);
  transition: all 0.2s;
}

.btn-icon:hover {
  transform: scale(1.1);
}

.btn-icon.btn-danger:hover {
  background: #FEF2F2;
}

/* Responsive */
@media (max-width: 640px) {
  .filters-section {
    flex-direction: column;
    align-items: stretch;
  }

  .filter-group select,
  .filter-group input {
    min-width: 100%;
  }

  .listings-grid {
    grid-template-columns: 1fr 1fr;
    gap: 12px;
  }
}
</style>
