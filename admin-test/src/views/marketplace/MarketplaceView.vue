<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '@/services/api'
import type { MarketplaceCategoryGroup, MarketplaceListing } from '@/types/marketplace'

const router = useRouter()

// Data state
const categoryGroups = ref<MarketplaceCategoryGroup[]>([])
const loading = ref(false)
const error = ref('')
const searchQuery = ref('')

// Image cache
const listingImages = ref<Map<string, string>>(new Map())

// Load main page listings
async function loadMainPage() {
  loading.value = true
  error.value = ''
  try {
    const response = await api.getMarketplaceMainPage()
    categoryGroups.value = response || []
    // Load images for all listings (don't wait, load in background)
    loadListingImages()
  } catch (e: unknown) {
    error.value = e instanceof Error ? e.message : 'Ошибка загрузки объявлений'
    console.error('Failed to load marketplace:', e)
  } finally {
    loading.value = false
  }
}

// Load listing images
async function loadListingImages() {
  for (const group of categoryGroups.value) {
    for (const listing of group.listings) {
      if (listing.image && !listingImages.value.has(listing.id)) {
        try {
          const url = await api.getPresignedDownloadUrl(listing.image)
          if (url) {
            listingImages.value.set(listing.id, url)
          }
        } catch (e) {
          console.error('Failed to load image for listing:', listing.id, e)
        }
      }
    }
  }
}

// Get image helper
function getListingImage(listing: MarketplaceListing): string | undefined {
  return listingImages.value.get(listing.id)
}

// Format price
function formatPrice(price?: number): string {
  if (!price) return 'Договорная'
  return new Intl.NumberFormat('uz-UZ').format(price) + ' UZS'
}

// Navigate to listing detail
function navigateToListing(id: string) {
  router.push(`/marketplace/${id}`)
}

// Navigate to category
function navigateToCategory(categoryId: string) {
  router.push(`/marketplace?category=${categoryId}`)
}

onMounted(() => {
  loadMainPage()
})
</script>

<template>
  <div class="marketplace-view">
    <!-- Header with Search -->
    <div class="view-header">
      <div class="search-bar">
        <svg class="search-icon" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <circle cx="11" cy="11" r="8"></circle>
          <path d="m21 21-4.35-4.35"></path>
        </svg>
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Фильтр и поиск"
          @keyup.enter="loadMainPage"
        />
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Загрузка объявлений...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="error-state">
      <p>{{ error }}</p>
      <button class="btn btn-secondary" @click="loadMainPage">
        Повторить
      </button>
    </div>

    <!-- Empty State -->
    <div v-else-if="categoryGroups.length === 0" class="empty-state">
      <div class="empty-icon">🏠</div>
      <h3>Нет объявлений</h3>
      <p>В данный момент нет доступных объявлений</p>
    </div>

    <!-- Content - Category Groups -->
    <div v-else class="category-sections">
      <section
        v-for="group in categoryGroups"
        :key="group.listingCategoryId"
        class="category-section"
      >
        <div class="section-header">
          <h2 class="section-title">{{ group.listingCategoryName }}</h2>
          <button class="section-link" @click="navigateToCategory(group.listingCategoryId)">
            Посмотреть еще
          </button>
        </div>

        <div class="listings-scroll">
          <div class="listings-row">
            <article
              v-for="listing in group.listings"
              :key="listing.id"
              class="listing-card"
              @click="navigateToListing(listing.id)"
            >
              <div class="card-image">
                <img v-if="getListingImage(listing)" :src="getListingImage(listing)" :alt="listing.description" />
                <div v-else class="card-placeholder">🏢</div>
              </div>
              <div class="card-body">
                <h4 class="card-title">{{ listing.building || listing.complex || 'Office' }}, {{ listing.district || listing.region || 'Tashkent' }}</h4>
                <p class="card-meta">
                  <span v-if="listing.totalArea">from {{ listing.totalArea }} м²</span>
                  <span v-if="listing.floorsCount"> • {{ listing.floorsCount }} plans</span>
                </p>
                <p class="card-price">
                  {{ formatPrice(listing.priceForMonth) }}
                  <span v-if="listing.priceForMonth" class="price-period">mo</span>
                </p>
              </div>
            </article>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>

<style scoped>
.marketplace-view {
  padding: 20px 24px;
  max-width: 1400px;
  margin: 0 auto;
}

/* Header - Centered with search above tabs */
.view-header {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 16px;
  margin-bottom: 32px;
}

/* Search Bar - Figma: Width 412px, Height 44px, Radius 100px */
.search-bar {
  display: flex;
  align-items: center;
  gap: 10px;
  height: 44px;
  padding: 0 16px;
  background: #fff;
  border: 0.5px solid rgba(27, 27, 27, 0.04);
  border-radius: 100px;
  width: 412px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
}

.search-icon {
  color: rgba(27, 27, 27, 0.4);
  flex-shrink: 0;
}

.search-bar input {
  flex: 1;
  border: none;
  background: transparent;
  font-size: 13px;
  outline: none;
  color: #1B1B1B;
}

.search-bar input::placeholder {
  color: rgba(27, 27, 27, 0.4);
}

/* States */
.loading-state,
.error-state,
.empty-state {
  text-align: center;
  padding: 80px 20px;
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
  font-size: 48px;
  margin-bottom: 16px;
}

.empty-state h3 {
  font-size: 20px;
  margin: 0 0 8px;
  color: #1B1B1B;
}

.empty-state p,
.error-state p {
  color: rgba(27, 27, 27, 0.5);
  margin: 0 0 20px;
  font-size: 14px;
}

.btn-secondary {
  padding: 10px 20px;
  border: 1px solid #1B1B1B;
  background: transparent;
  border-radius: 100px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-secondary:hover {
  background: #1B1B1B;
  color: white;
}

/* Category Sections */
.category-sections {
  display: flex;
  flex-direction: column;
  gap: 40px;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.section-title {
  font-size: 18px;
  font-weight: 700;
  color: #1B1B1B;
  margin: 0;
}

.section-link {
  font-size: 13px;
  color: rgba(27, 27, 27, 0.5);
  background: none;
  border: none;
  cursor: pointer;
  padding: 0;
  transition: color 0.2s;
}

.section-link:hover {
  color: #FF5B3C;
}

/* Horizontal Scroll - Airbnb style */
.listings-scroll {
  overflow-x: auto;
  margin: 0 -24px;
  padding: 0 24px;
  scrollbar-width: none;
  -ms-overflow-style: none;
}

.listings-scroll::-webkit-scrollbar {
  display: none;
}

.listings-row {
  display: flex;
  gap: 16px;
  padding-bottom: 8px;
}

/* Listing Cards */
.listing-card {
  flex-shrink: 0;
  width: 200px;
  background: #FFFFFF;
  border-radius: 16px;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.2s ease;
  border: 0.5px solid rgba(27, 27, 27, 0.04);
}

.listing-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.1);
}

.card-image {
  height: 120px;
  background: #F6F6F6;
  overflow: hidden;
  border-radius: 16px 16px 0 0;
}

.card-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.card-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 32px;
  opacity: 0.25;
}

.card-body {
  padding: 12px;
}

.card-title {
  font-size: 14px;
  font-weight: 600;
  color: #1B1B1B;
  margin: 0 0 4px;
  line-height: 1.3;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.card-meta {
  font-size: 12px;
  color: rgba(27, 27, 27, 0.5);
  margin: 0 0 6px;
}

.card-price {
  font-size: 14px;
  font-weight: 700;
  color: #1B1B1B;
  margin: 0;
}

.price-period {
  font-weight: 400;
  color: rgba(27, 27, 27, 0.4);
}

/* Responsive */
@media (max-width: 768px) {
  .search-bar {
    width: 100%;
    max-width: 412px;
  }

  .listing-card {
    width: 160px;
  }

  .card-image {
    height: 100px;
  }
}
</style>
