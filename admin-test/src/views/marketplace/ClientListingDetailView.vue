<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { api } from '@/services/api'
import type { ListingDetail } from '@/types/marketplace'

const route = useRoute()
const router = useRouter()

// Data state
const listing = ref<ListingDetail | null>(null)
const loading = ref(false)
const error = ref('')
const showRequestModal = ref(false)
const requestContent = ref('')
const submittingRequest = ref(false)

// Image state
const listingImages = ref<string[]>([])
const currentImageIndex = ref(0)

// Get listing ID from route
const listingId = route.params.id as string

// Load listing
async function loadListing() {
  loading.value = true
  error.value = ''
  try {
    const response = await api.getListingById(listingId)
    listing.value = response
    // Load images
    if (response.images && response.images.length > 0) {
      await loadImages(response.images)
    }
  } catch (e: unknown) {
    const errorMessage = e instanceof Error ? e.message : 'Ошибка загрузки объявления'
    error.value = errorMessage
    console.error('Failed to load listing:', e)
  } finally {
    loading.value = false
  }
}

// Load images
async function loadImages(imageKeys: string[]) {
  const loadedUrls: string[] = []
  for (const key of imageKeys) {
    try {
      const url = await api.getPresignedDownloadUrl(key)
      if (url) {
        loadedUrls.push(url)
      }
    } catch (e) {
      console.error('Failed to load image:', e)
    }
  }
  listingImages.value = loadedUrls
}

// Navigate images
function nextImage() {
  if (currentImageIndex.value < listingImages.value.length - 1) {
    currentImageIndex.value++
  } else {
    currentImageIndex.value = 0
  }
}

function prevImage() {
  if (currentImageIndex.value > 0) {
    currentImageIndex.value--
  } else {
    currentImageIndex.value = listingImages.value.length - 1
  }
}

// Format price
function formatPrice(price?: number): string {
  if (!price) return 'Договорная'
  return new Intl.NumberFormat('uz-UZ').format(price) + ' UZS'
}

// Send request
async function submitRequest() {
  if (!requestContent.value.trim()) {
    alert('Пожалуйста, введите сообщение')
    return
  }

  submittingRequest.value = true
  try {
    // Create request
    const requestId = await api.createListingRequest({
      listingId: listingId,
      content: requestContent.value
    })
    // Send request
    await api.sendListingRequest(requestId)

    alert('Заявка успешно отправлена!')
    showRequestModal.value = false
    requestContent.value = ''
    router.push('/my-requests')
  } catch (e: unknown) {
    const errorMessage = e instanceof Error ? e.message : 'Ошибка при отправке заявки'
    alert('Ошибка: ' + errorMessage)
  } finally {
    submittingRequest.value = false
  }
}

// Go back
function goBack() {
  router.back()
}

onMounted(() => {
  loadListing()
})
</script>

<template>
  <div class="listing-detail-view">
    <!-- Back Button -->
    <button class="back-btn" @click="goBack">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <path d="M19 12H5M12 19l-7-7 7-7"/>
      </svg>
      Назад
    </button>

    <!-- Loading State -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Загрузка...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="error-state">
      <p>{{ error }}</p>
      <button class="btn btn-secondary" @click="loadListing">
        Повторить
      </button>
    </div>

    <!-- Content -->
    <div v-else-if="listing" class="listing-content">
      <!-- Image Gallery -->
      <div class="image-gallery">
        <div v-if="listingImages.length > 0" class="gallery-main">
          <img :src="listingImages[currentImageIndex]" :alt="listing.description || 'Listing'" />

          <!-- Navigation arrows -->
          <button v-if="listingImages.length > 1" class="gallery-nav prev" @click="prevImage">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M15 18l-6-6 6-6"/>
            </svg>
          </button>
          <button v-if="listingImages.length > 1" class="gallery-nav next" @click="nextImage">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M9 18l6-6-6-6"/>
            </svg>
          </button>

          <!-- Dots indicator -->
          <div v-if="listingImages.length > 1" class="gallery-dots">
            <span
              v-for="(_, idx) in listingImages"
              :key="idx"
              class="dot"
              :class="{ active: idx === currentImageIndex }"
              @click="currentImageIndex = idx"
            ></span>
          </div>
        </div>
        <div v-else class="gallery-placeholder">
          <span>🏢</span>
          <p>Нет изображений</p>
        </div>
      </div>

      <!-- Main Info -->
      <div class="main-info">
        <div class="info-left">
          <div class="categories">
            <span v-for="cat in listing.categories" :key="cat" class="category-tag">
              {{ cat }}
            </span>
          </div>

          <h1 class="listing-title">
            {{ listing.building || listing.complex || 'Помещение' }}
          </h1>

          <p class="listing-location">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"/>
              <circle cx="12" cy="10" r="3"/>
            </svg>
            {{ listing.district || listing.region || 'Tashkent' }}{{ listing.address ? ', ' + listing.address : '' }}
          </p>

          <!-- Specs Grid -->
          <div class="specs-grid">
            <div class="spec-item">
              <span class="spec-icon">📐</span>
              <span class="spec-value">{{ listing.totalArea }} м²</span>
              <span class="spec-label">Общая площадь</span>
            </div>
            <div v-if="listing.livingArea" class="spec-item">
              <span class="spec-icon">🏠</span>
              <span class="spec-value">{{ listing.livingArea }} м²</span>
              <span class="spec-label">Жилая площадь</span>
            </div>
            <div v-if="listing.roomsCount" class="spec-item">
              <span class="spec-icon">🚪</span>
              <span class="spec-value">{{ listing.roomsCount }}</span>
              <span class="spec-label">Комнат</span>
            </div>
            <div v-if="listing.ceilingHeight" class="spec-item">
              <span class="spec-icon">↕️</span>
              <span class="spec-value">{{ listing.ceilingHeight }} м</span>
              <span class="spec-label">Высота потолков</span>
            </div>
          </div>

          <!-- Description -->
          <div v-if="listing.description" class="description-section">
            <h3>Описание</h3>
            <p>{{ listing.description }}</p>
          </div>

          <!-- Amenities -->
          <div v-if="listing.amenities && listing.amenities.length > 0" class="amenities-section">
            <h3>Удобства</h3>
            <div class="amenities-grid">
              <div v-for="amenity in listing.amenities" :key="amenity.name" class="amenity-item">
                <img v-if="amenity.iconUrl" :src="amenity.iconUrl" :alt="amenity.name" class="amenity-icon-img" />
                <span v-else class="amenity-icon">✓</span>
                {{ amenity.name }}
              </div>
            </div>
          </div>
        </div>

        <!-- Price Card -->
        <div class="price-card">
          <div class="price-main">
            <span class="price-label">Аренда в месяц</span>
            <span class="price-value">{{ formatPrice(listing.priceForMonth) }}</span>
          </div>
          <div v-if="listing.pricePerSquareMeter" class="price-secondary">
            {{ formatPrice(listing.pricePerSquareMeter) }} / м²
          </div>
          <button class="btn-request" @click="showRequestModal = true">
            Отправить заявку
          </button>
        </div>
      </div>
    </div>

    <!-- Request Modal -->
    <div v-if="showRequestModal" class="modal-overlay" @click.self="showRequestModal = false">
      <div class="modal-content">
        <div class="modal-header">
          <h3>Отправить заявку</h3>
          <button class="modal-close" @click="showRequestModal = false">×</button>
        </div>
        <div class="modal-body">
          <label>Ваше сообщение владельцу:</label>
          <textarea
            v-model="requestContent"
            placeholder="Расскажите о себе и ваших требованиях..."
            rows="5"
          ></textarea>
        </div>
        <div class="modal-footer">
          <button class="btn-secondary" @click="showRequestModal = false">Отмена</button>
          <button
            class="btn-primary"
            :disabled="submittingRequest"
            @click="submitRequest"
          >
            {{ submittingRequest ? 'Отправка...' : 'Отправить' }}
          </button>
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

/* Back Button */
.back-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 16px;
  background: #FFFFFF;
  border: 1px solid rgba(27, 27, 27, 0.1);
  border-radius: 100px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  margin-bottom: 24px;
  transition: all 0.2s;
}

.back-btn:hover {
  background: #1B1B1B;
  color: white;
}

/* States */
.loading-state,
.error-state {
  text-align: center;
  padding: 80px 20px;
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

/* Image Gallery */
.image-gallery {
  margin-bottom: 32px;
}

.gallery-main {
  position: relative;
  height: 400px;
  background: #F6F6F6;
  border-radius: 24px;
  overflow: hidden;
}

.gallery-main img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.gallery-nav {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  width: 44px;
  height: 44px;
  background: rgba(255, 255, 255, 0.9);
  border: none;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s;
}

.gallery-nav:hover {
  background: white;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.gallery-nav.prev {
  left: 16px;
}

.gallery-nav.next {
  right: 16px;
}

.gallery-dots {
  position: absolute;
  bottom: 16px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  gap: 8px;
}

.dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.5);
  cursor: pointer;
  transition: all 0.2s;
}

.dot.active {
  background: white;
  width: 24px;
  border-radius: 4px;
}

.gallery-placeholder {
  height: 300px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background: #F6F6F6;
  border-radius: 24px;
  color: rgba(27, 27, 27, 0.3);
}

.gallery-placeholder span {
  font-size: 64px;
  margin-bottom: 16px;
}

/* Main Info Layout */
.main-info {
  display: grid;
  grid-template-columns: 1fr 360px;
  gap: 32px;
  align-items: start;
}

.info-left {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

/* Categories */
.categories {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.category-tag {
  padding: 6px 12px;
  background: rgba(255, 91, 60, 0.1);
  color: #FF5B3C;
  border-radius: 100px;
  font-size: 12px;
  font-weight: 500;
}

/* Title */
.listing-title {
  font-size: 32px;
  font-weight: 700;
  color: #1B1B1B;
  margin: 0;
}

.listing-location {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  color: rgba(27, 27, 27, 0.6);
  margin: 0;
}

/* Specs Grid */
.specs-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
  gap: 16px;
  background: #F9F9F9;
  padding: 20px;
  border-radius: 16px;
}

.spec-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  gap: 4px;
}

.spec-icon {
  font-size: 24px;
}

.spec-value {
  font-size: 18px;
  font-weight: 700;
  color: #1B1B1B;
}

.spec-label {
  font-size: 12px;
  color: rgba(27, 27, 27, 0.5);
}

/* Description */
.description-section h3,
.amenities-section h3 {
  font-size: 18px;
  font-weight: 600;
  margin: 0 0 12px;
  color: #1B1B1B;
}

.description-section p {
  font-size: 14px;
  line-height: 1.6;
  color: rgba(27, 27, 27, 0.7);
  margin: 0;
}

/* Amenities */
.amenities-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
  gap: 12px;
}

.amenity-item {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  color: #1B1B1B;
}

.amenity-icon {
  font-size: 16px;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.amenity-icon-img {
  width: 24px;
  height: 24px;
  object-fit: contain;
}

/* Price Card */
.price-card {
  background: #FFFFFF;
  border: 1px solid rgba(27, 27, 27, 0.08);
  border-radius: 24px;
  padding: 24px;
  position: sticky;
  top: 24px;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.06);
}

.price-main {
  display: flex;
  flex-direction: column;
  gap: 4px;
  margin-bottom: 8px;
}

.price-label {
  font-size: 13px;
  color: rgba(27, 27, 27, 0.5);
}

.price-value {
  font-size: 28px;
  font-weight: 700;
  color: #1B1B1B;
}

.price-secondary {
  font-size: 14px;
  color: rgba(27, 27, 27, 0.6);
  margin-bottom: 20px;
}

.btn-request {
  width: 100%;
  padding: 16px;
  background: #FF5B3C;
  color: white;
  border: none;
  border-radius: 100px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-request:hover {
  background: #E54E32;
}

/* Modal */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 24px;
  width: 100%;
  max-width: 500px;
  overflow: hidden;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 24px;
  border-bottom: 1px solid rgba(27, 27, 27, 0.06);
}

.modal-header h3 {
  margin: 0;
  font-size: 18px;
  font-weight: 600;
}

.modal-close {
  width: 32px;
  height: 32px;
  border: none;
  background: rgba(27, 27, 27, 0.04);
  border-radius: 50%;
  font-size: 20px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal-body {
  padding: 24px;
}

.modal-body label {
  display: block;
  font-size: 14px;
  font-weight: 500;
  margin-bottom: 8px;
  color: #1B1B1B;
}

.modal-body textarea {
  width: 100%;
  padding: 12px;
  border: 1px solid rgba(27, 27, 27, 0.1);
  border-radius: 12px;
  font-size: 14px;
  resize: vertical;
  font-family: inherit;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  padding: 16px 24px;
  border-top: 1px solid rgba(27, 27, 27, 0.06);
}

.btn-secondary {
  padding: 12px 20px;
  background: transparent;
  border: 1px solid rgba(27, 27, 27, 0.2);
  border-radius: 100px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
}

.btn-primary {
  padding: 12px 24px;
  background: #1B1B1B;
  color: white;
  border: none;
  border-radius: 100px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
}

.btn-primary:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Responsive */
@media (max-width: 900px) {
  .main-info {
    grid-template-columns: 1fr;
  }

  .price-card {
    position: fixed;
    bottom: 0;
    left: 0;
    right: 0;
    border-radius: 24px 24px 0 0;
    z-index: 100;
  }
}
</style>
