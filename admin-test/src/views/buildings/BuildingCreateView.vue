<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { api } from '@/services/api'
import { compressImage } from '@/utils/imageCompression'
import type { Region, District, Language } from '@/types/common'
import GoogleMap from '@/components/GoogleMap.vue'

const router = useRouter()
const route = useRoute()

const isEditMode = computed(() => route.name === 'building-edit')
const buildingId = computed(() => route.params.id as string)

// Form state
const currentStep = ref(0)  // Start at step 0 (type selection)
const totalSteps = 5  // 0: Type, 1: Basic, 2: Details, 3: Location, 4: Media
const isLoading = ref(false)
const isSaving = ref(false)
const errorMessage = ref('')

// Step 1: Basic Info
const buildingNumber = ref('')
const regionId = ref('')
const districtId = ref('')
const isCommercial = ref(false)
const isLiving = ref(false)
const complexId = ref('')

// Step 2: Details
const totalArea = ref<number | null>(null)
const floorsCount = ref<number | null>(null)

// Step 3: Location
const address = ref('')
const latitude = ref<number | null>(null)
const longitude = ref<number | null>(null)

// Step 4: Media
const descriptionRu = ref('')
const descriptionUz = ref('')
const descriptionEn = ref('')
const activeDescriptionTab = ref<'ru' | 'uz' | 'en'>('ru')
const imageKeys = ref<string[]>([])  // Uploaded image keys from API
const imageFiles = ref<{ file: File; preview: string; uploading: boolean }[]>([])  // Files to upload with preview
const existingImages = ref<{ key: string; url: string }[]>([])  // For displaying existing images in edit mode
const isUploadingImages = ref(false)

// Reference data
const regions = ref<Region[]>([])
const districts = ref<District[]>([])
const complexes = ref<any[]>([])
const languages = ref<Language[]>([])
const isLoadingDistricts = ref(false)

onMounted(async () => {
  isLoading.value = true
  try {
    // Load regions
    const regionsResponse = await api.getRegions({ pageSize: 100 })
    regions.value = regionsResponse.items || []

    // Load complexes
    const complexesResponse = await api.getComplexes({ pageSize: 100 })
    complexes.value = complexesResponse.items || []

    // Load languages (for descriptions)
    const languagesResponse = await api.getLanguages({ pageSize: 10 })
    languages.value = languagesResponse.items || []

    // If edit mode, load building data
    if (isEditMode.value && buildingId.value) {
      const building = await api.getBuildingById(buildingId.value)
      buildingNumber.value = building.number || ''
      regionId.value = building.regionId || ''
      districtId.value = building.districtId || ''
      isCommercial.value = building.isCommercial || false
      isLiving.value = building.isLiving || false
      complexId.value = building.complexId || ''
      totalArea.value = building.totalArea || null
      floorsCount.value = building.floorsCount || null
      address.value = building.address || ''
      latitude.value = building.latitude || null
      longitude.value = building.longitude || null

      // Handle descriptions
      if (building.descriptions) {
        building.descriptions.forEach((d: any) => {
          if (d.languageShortCode === 'ru' || d.language === 'ru') descriptionRu.value = d.value
          if (d.languageShortCode === 'uz' || d.language === 'uz') descriptionUz.value = d.value
          if (d.languageShortCode === 'en' || d.language === 'en') descriptionEn.value = d.value
        })
      }
      imageKeys.value = building.images || []

      // Load existing images - images are now full URLs
      if (building.images && building.images.length > 0) {
        for (const imageUrl of building.images) {
          existingImages.value.push({ key: imageUrl, url: imageUrl })
        }
      }

      // Load districts for selected region
      if (regionId.value) {
        await loadDistricts()
      }

      // Skip type selection step in edit mode
      currentStep.value = 1
    }
  } catch (error) {
    console.error('Error loading data:', error)
    errorMessage.value = 'Error loading data'
  } finally {
    isLoading.value = false
  }
})

// Image upload handlers with compression
async function handleFileSelect(event: Event) {
  const input = event.target as HTMLInputElement
  if (!input.files?.length) return

  const files = Array.from(input.files)
  isUploadingImages.value = true

  try {
    for (const file of files) {
      // Create preview
      const preview = URL.createObjectURL(file)
      const imageItem = { file, preview, uploading: true }
      imageFiles.value.push(imageItem)

      // Compress and upload file
      try {
        const compressedFile = await compressImage(file)
        const key = await api.uploadFile(compressedFile)
        imageKeys.value.push(key)
        imageItem.uploading = false
      } catch (error) {
        console.error('Error uploading file:', error)
        // Remove failed upload from preview
        const index = imageFiles.value.indexOf(imageItem)
        if (index > -1) {
          URL.revokeObjectURL(imageItem.preview)
          imageFiles.value.splice(index, 1)
        }
      }
    }
  } finally {
    isUploadingImages.value = false
    input.value = '' // Reset input
  }
}

function removeImage(index: number) {
  // Revoke preview URL
  if (imageFiles.value[index]) {
    URL.revokeObjectURL(imageFiles.value[index].preview)
    imageFiles.value.splice(index, 1)
  }
  // Remove key
  if (imageKeys.value[index]) {
    imageKeys.value.splice(index, 1)
  }
}

function removeExistingImage(index: number) {
  const item = existingImages.value[index]
  if (item) {
    URL.revokeObjectURL(item.url)
    // Remove from imageKeys
    const keyIndex = imageKeys.value.indexOf(item.key)
    if (keyIndex > -1) {
      imageKeys.value.splice(keyIndex, 1)
    }
    existingImages.value.splice(index, 1)
  }
}

// Load districts when region changes
async function loadDistricts() {
  if (!regionId.value) {
    districts.value = []
    return
  }

  isLoadingDistricts.value = true
  try {
    const response = await api.getDistricts({ regionId: regionId.value, pageSize: 100 })
    districts.value = response.items || []
  } catch (error) {
    console.error('Error loading districts:', error)
  } finally {
    isLoadingDistricts.value = false
  }
}

// Watch region changes
watch(regionId, () => {
  districtId.value = ''
  loadDistricts()
})

function nextStep() {
  if (currentStep.value < totalSteps - 1) {
    currentStep.value++
  }
}

function prevStep() {
  if (currentStep.value > 0) {
    currentStep.value--
  }
}

function selectBuildingType(type: 'commercial' | 'residential' | 'mixed') {
  if (type === 'commercial') {
    isCommercial.value = true
    isLiving.value = false
  } else if (type === 'residential') {
    isCommercial.value = false
    isLiving.value = true
  } else {
    isCommercial.value = true
    isLiving.value = true
  }
  // Auto-proceed to next step
  nextStep()
}

function canProceed(): boolean {
  switch (currentStep.value) {
    case 0:
      return (isCommercial.value || isLiving.value)
    case 1:
      return true  // Basic info is optional
    default:
      return true
  }
}

async function handleSubmit() {
  if (!canProceed()) return

  isSaving.value = true
  errorMessage.value = ''

  try {
    // Build descriptions with languageId
    const descriptions = []
    const ruLang = languages.value.find(l => l.shortCode === 'ru')
    const uzLang = languages.value.find(l => l.shortCode === 'uz')
    const enLang = languages.value.find(l => l.shortCode === 'en')

    if (descriptionRu.value && ruLang) {
      descriptions.push({ languageId: ruLang.id, languageShortCode: 'ru', value: descriptionRu.value })
    }
    if (descriptionUz.value && uzLang) {
      descriptions.push({ languageId: uzLang.id, languageShortCode: 'uz', value: descriptionUz.value })
    }
    if (descriptionEn.value && enLang) {
      descriptions.push({ languageId: enLang.id, languageShortCode: 'en', value: descriptionEn.value })
    }

    const data = {
      isCommercial: isCommercial.value,
      isLiving: isLiving.value,
      number: buildingNumber.value || null,
      regionId: regionId.value || null,
      districtId: districtId.value || null,
      complexId: complexId.value || null,
      totalArea: totalArea.value,
      floorsCount: floorsCount.value,
      latitude: latitude.value,
      longitude: longitude.value,
      address: address.value || null,
      descriptions: descriptions.length ? descriptions : null,
      images: imageKeys.value.length ? imageKeys.value : null,
    }

    if (isEditMode.value) {
      await api.updateBuilding({ id: buildingId.value, ...data })
      router.push(`/buildings/${buildingId.value}`)
    } else {
      const newBuildingId = await api.createBuilding(data)
      router.push(`/buildings/${newBuildingId}`)
    }
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : 'Error saving'
  } finally {
    isSaving.value = false
  }
}

// Handle location selection from map
function onLocationUpdate(location: { lat: number; lng: number }) {
  latitude.value = location.lat
  longitude.value = location.lng
}

// Translation functionality
const isTranslating = ref(false)
const translationError = ref('')

async function translateText(text: string, sourceLang: string, targetLang: string): Promise<string> {
  if (!text.trim()) return ''

  // Use free Google Translate endpoint (unofficial but works without API key)
  const url = `https://translate.googleapis.com/translate_a/single?client=gtx&sl=${sourceLang}&tl=${targetLang}&dt=t&q=${encodeURIComponent(text)}`

  const response = await fetch(url)

  if (!response.ok) {
    throw new Error('Translation failed')
  }

  const data = await response.json()
  // Response format: [[["translated text","original text",null,null,10]],null,"source_lang"]
  // Extract all translated segments and join them
  if (Array.isArray(data) && Array.isArray(data[0])) {
    return data[0].map((segment: string[]) => segment[0]).join('')
  }
  return ''
}

// Auto-detect source language and translate to empty fields
async function autoTranslateDescriptions() {
  // Determine which field has text (source) and which are empty (targets)
  let sourceLang: 'ru' | 'uz' | 'en' | null = null
  let sourceText = ''
  const targets: { lang: 'ru' | 'uz' | 'en'; isEmpty: boolean }[] = []

  // Check Russian
  if (descriptionRu.value.trim()) {
    sourceLang = 'ru'
    sourceText = descriptionRu.value
  }
  targets.push({ lang: 'ru', isEmpty: !descriptionRu.value.trim() })

  // Check Uzbek
  if (descriptionUz.value.trim() && !sourceLang) {
    sourceLang = 'uz'
    sourceText = descriptionUz.value
  }
  targets.push({ lang: 'uz', isEmpty: !descriptionUz.value.trim() })

  // Check English
  if (descriptionEn.value.trim() && !sourceLang) {
    sourceLang = 'en'
    sourceText = descriptionEn.value
  }
  targets.push({ lang: 'en', isEmpty: !descriptionEn.value.trim() })

  // Validate
  if (!sourceLang || !sourceText.trim()) {
    translationError.value = 'Enter text in at least one field'
    return
  }

  // Get empty targets
  const emptyTargets = targets.filter(t => t.isEmpty && t.lang !== sourceLang)

  if (emptyTargets.length === 0) {
    translationError.value = 'All fields are already filled'
    return
  }

  isTranslating.value = true
  translationError.value = ''

  try {
    // Translate to empty fields only
    const translations = await Promise.all(
      emptyTargets.map(t => translateText(sourceText, sourceLang!, t.lang))
    )

    // Update the empty fields
    for (let i = 0; i < emptyTargets.length; i++) {
      const target = emptyTargets[i]
      const translated = translations[i] ?? ''
      if (target?.lang === 'ru') descriptionRu.value = translated
      else if (target?.lang === 'uz') descriptionUz.value = translated
      else if (target?.lang === 'en') descriptionEn.value = translated
    }
  } catch (error) {
    console.error('Translation error:', error)
    translationError.value = 'Translation error. Please try again later.'
  } finally {
    isTranslating.value = false
  }
}

function goBack() {
  if (isEditMode.value && buildingId.value) {
    router.push(`/buildings/${buildingId.value}`)
  } else {
    router.push('/buildings')
  }
}
</script>

<template>
  <div class="building-create-page">
    <!-- Header -->
    <div class="page-header">
      <button class="btn btn-ghost" @click="goBack">
        ← Back
      </button>
      <h1 class="page-title">{{ isEditMode ? 'Edit Building' : 'New Building' }}</h1>
    </div>

    <!-- Progress Steps (only show after type selection) -->
    <div v-if="currentStep > 0" class="steps-progress">
      <div
        v-for="step in 4"
        :key="step"
        class="step"
        :class="{ active: step === currentStep, completed: step < currentStep }"
      >
        <div class="step-number">{{ step }}</div>
        <div class="step-label">
          {{ step === 1 ? 'Basic' : step === 2 ? 'Details' : step === 3 ? 'Location' : 'Media' }}
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="loading-state">
      <div class="spinner spinner-lg"></div>
    </div>

    <!-- Form -->
    <div v-else class="form-container">
      <div v-if="errorMessage" class="alert alert-error mb-lg">
        {{ errorMessage }}
      </div>

      <!-- Step 0: Building Type Selection -->
      <div v-show="currentStep === 0" class="type-selection-step">
        <div class="type-selection-header">
          <h2>What type of building are you adding?</h2>
          <p>Select the building type to get started</p>
        </div>

        <div class="type-cards">
          <!-- Commercial Building -->
          <div
            class="type-card"
            :class="{ selected: isCommercial && !isLiving }"
            @click="selectBuildingType('commercial')"
          >
            <div class="type-card-icon">🏢</div>
            <div class="type-card-content">
              <h3>Commercial</h3>
              <p>Office buildings, shopping centers, business properties</p>
            </div>
            <div class="type-card-arrow">→</div>
          </div>

          <!-- Residential Building -->
          <div
            class="type-card"
            :class="{ selected: isLiving && !isCommercial }"
            @click="selectBuildingType('residential')"
          >
            <div class="type-card-icon">🏠</div>
            <div class="type-card-content">
              <h3>Residential</h3>
              <p>Apartment buildings, housing complexes, residential towers</p>
            </div>
            <div class="type-card-arrow">→</div>
          </div>

          <!-- Mixed Use Building -->
          <div
            class="type-card"
            :class="{ selected: isCommercial && isLiving }"
            @click="selectBuildingType('mixed')"
          >
            <div class="type-card-icon">🏗️</div>
            <div class="type-card-content">
              <h3>Mixed Use</h3>
              <p>Buildings with both commercial and residential spaces</p>
            </div>
            <div class="type-card-arrow">→</div>
          </div>
        </div>
      </div>

      <!-- Step 1: Basic Info -->
      <div v-show="currentStep === 1" class="form-step">
        <h2 class="step-title">Basic Information</h2>

        <div class="form-group">
          <label class="form-label">Building Number</label>
          <input
            v-model="buildingNumber"
            type="text"
            class="form-input"
            placeholder="e.g.: Block A, Building 1"
          />
        </div>

        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Region *</label>
            <select v-model="regionId" class="form-input">
              <option value="">Select region</option>
              <option v-for="region in regions" :key="region.id" :value="region.id">
                {{ region.name }}
              </option>
            </select>
          </div>

          <div class="form-group">
            <label class="form-label">District *</label>
            <select v-model="districtId" class="form-input" :disabled="!regionId">
              <option value="">Select district</option>
              <option v-for="district in districts" :key="district.id" :value="district.id">
                {{ district.name }}
              </option>
            </select>
          </div>
        </div>

        <div class="form-group">
          <label class="form-label">Complex (optional)</label>
          <select v-model="complexId" class="form-input">
            <option value="">Not part of complex</option>
            <option v-for="complex in complexes" :key="complex.id" :value="complex.id">
              {{ complex.name }}
            </option>
          </select>
        </div>

        <div class="form-group">
          <label class="form-label">Building Type *</label>
          <div class="checkbox-group">
            <label class="checkbox-item">
              <input type="checkbox" v-model="isCommercial" />
              <span>🏬 Commercial</span>
            </label>
            <label class="checkbox-item">
              <input type="checkbox" v-model="isLiving" />
              <span>🏠 Residential</span>
            </label>
          </div>
          <p v-if="!isCommercial && !isLiving" class="form-hint text-error">
            Select at least one type
          </p>
        </div>
      </div>

      <!-- Step 2: Details -->
      <div v-show="currentStep === 2" class="form-step">
        <h2 class="step-title">Additional Details</h2>

        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Total Area (m²)</label>
            <input
              v-model.number="totalArea"
              type="number"
              class="form-input"
              placeholder="0"
              min="0"
            />
          </div>

          <div class="form-group">
            <label class="form-label">Number of Floors</label>
            <input
              v-model.number="floorsCount"
              type="number"
              class="form-input"
              placeholder="0"
              min="0"
            />
          </div>
        </div>
      </div>

      <!-- Step 3: Location -->
      <div v-show="currentStep === 3" class="form-step">
        <h2 class="step-title">Location</h2>

        <div class="form-group">
          <label class="form-label">Address</label>
          <input
            v-model="address"
            type="text"
            class="form-input"
            placeholder="Street Name, Building 1"
          />
        </div>

        <!-- Google Map -->
        <div class="form-group">
          <label class="form-label">Select on map</label>
          <GoogleMap
            :latitude="latitude"
            :longitude="longitude"
            :editable="true"
            height="350px"
            @update:location="onLocationUpdate"
          />
        </div>

        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Latitude</label>
            <input
              v-model.number="latitude"
              type="number"
              class="form-input"
              placeholder="41.311081"
              step="0.000001"
            />
          </div>

          <div class="form-group">
            <label class="form-label">Longitude</label>
            <input
              v-model.number="longitude"
              type="number"
              class="form-input"
              placeholder="69.240562"
              step="0.000001"
            />
          </div>
        </div>
      </div>

      <!-- Step 4: Media -->
      <div v-show="currentStep === 4" class="form-step">
        <h2 class="step-title">Description and Photos</h2>

        <!-- Description Card -->
        <div class="description-card">
          <div class="description-header">
            <div class="description-title">
              <span class="description-icon">📝</span>
              <h3>Building Description</h3>
            </div>
            <button
              type="button"
              class="btn-translate"
              :disabled="isTranslating || (!descriptionRu && !descriptionUz && !descriptionEn)"
              @click="autoTranslateDescriptions"
            >
              <span v-if="isTranslating" class="spinner spinner-sm"></span>
              <span v-else>🌐</span>
              {{ isTranslating ? 'Translating...' : 'Auto-translate' }}
            </button>
          </div>

          <!-- Translation Error -->
          <div v-if="translationError" class="translation-error">
            ⚠️ {{ translationError }}
          </div>

          <!-- Language Tabs -->
          <div class="language-tabs">
            <button
              type="button"
              class="language-tab"
              :class="{ active: activeDescriptionTab === 'ru' }"
              @click="activeDescriptionTab = 'ru'"
            >
              <span class="tab-flag ru">RU</span>
              <span class="tab-label">Русский</span>
              <span v-if="descriptionRu" class="tab-indicator">✓</span>
            </button>
            <button
              type="button"
              class="language-tab"
              :class="{ active: activeDescriptionTab === 'uz' }"
              @click="activeDescriptionTab = 'uz'"
            >
              <span class="tab-flag uz">UZ</span>
              <span class="tab-label">O'zbek</span>
              <span v-if="descriptionUz" class="tab-indicator">✓</span>
            </button>
            <button
              type="button"
              class="language-tab"
              :class="{ active: activeDescriptionTab === 'en' }"
              @click="activeDescriptionTab = 'en'"
            >
              <span class="tab-flag en">EN</span>
              <span class="tab-label">English</span>
              <span v-if="descriptionEn" class="tab-indicator">✓</span>
            </button>
          </div>

          <!-- Tab Content -->
          <div class="description-content">
            <div v-show="activeDescriptionTab === 'ru'" class="description-tab-content">
              <textarea
                v-model="descriptionRu"
                class="description-textarea"
                placeholder="Введите описание здания на русском языке..."
                rows="5"
              ></textarea>
              <div class="textarea-footer">
                <span class="char-count">{{ descriptionRu.length }} символов</span>
              </div>
            </div>

            <div v-show="activeDescriptionTab === 'uz'" class="description-tab-content">
              <textarea
                v-model="descriptionUz"
                class="description-textarea"
                placeholder="Bino tavsifini o'zbek tilida kiriting..."
                rows="5"
              ></textarea>
              <div class="textarea-footer">
                <span class="char-count">{{ descriptionUz.length }} belgi</span>
              </div>
            </div>

            <div v-show="activeDescriptionTab === 'en'" class="description-tab-content">
              <textarea
                v-model="descriptionEn"
                class="description-textarea"
                placeholder="Enter building description in English..."
                rows="5"
              ></textarea>
              <div class="textarea-footer">
                <span class="char-count">{{ descriptionEn.length }} characters</span>
              </div>
            </div>
          </div>

          <div class="description-hint">
            💡 Enter description in any language and click "Auto-translate" to fill others
          </div>
        </div>

        <div class="form-group">
          <label class="form-label">Photos</label>

          <!-- Image Preview Grid -->
          <div v-if="existingImages.length > 0 || imageFiles.length > 0" class="image-preview-grid">
            <!-- Existing images from API (in edit mode) -->
            <div
              v-for="(img, index) in existingImages"
              :key="'existing-' + index"
              class="image-preview-item"
            >
              <img :src="img.url" alt="Existing photo" />
              <button class="image-remove-btn" @click="removeExistingImage(index)">×</button>
            </div>

            <!-- Newly uploaded images -->
            <div
              v-for="(img, index) in imageFiles"
              :key="'new-' + index"
              class="image-preview-item"
            >
              <img :src="img.preview" alt="Preview" />
              <div v-if="img.uploading" class="image-uploading">
                <span class="spinner"></span>
              </div>
              <button v-else class="image-remove-btn" @click="removeImage(index)">×</button>
            </div>
          </div>

          <!-- Upload Zone -->
          <label class="upload-zone" :class="{ disabled: isUploadingImages }">
            <input
              type="file"
              accept="image/*"
              multiple
              class="hidden-input"
              @change="handleFileSelect"
              :disabled="isUploadingImages"
            />
            <span v-if="isUploadingImages">
              <span class="spinner"></span> Uploading...
            </span>
            <span v-else>
              📁 Click to select images
            </span>
            <p class="form-hint">Supported: JPG, PNG, WebP</p>
          </label>
        </div>
      </div>

      <!-- Actions (not shown on Step 0) -->
      <div v-if="currentStep > 0" class="form-actions">
        <button
          v-if="currentStep > 1"
          class="btn btn-ghost"
          @click="prevStep"
        >
          ← Back
        </button>
        <div class="spacer"></div>
        <button
          v-if="currentStep < 4"
          class="btn btn-primary"
          :disabled="!canProceed()"
          @click="nextStep"
        >
          Next →
        </button>
        <button
          v-else
          class="btn btn-primary"
          :disabled="isSaving"
          @click="handleSubmit"
        >
          <span v-if="isSaving" class="spinner"></span>
          <span v-else>{{ isEditMode ? 'Save' : 'Create Building' }}</span>
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.building-create-page {
  max-width: 700px;
}

.page-header {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 2rem;
}

.page-title {
  font-size: 1.5rem;
  font-weight: 700;
  margin: 0;
}

/* Type Selection Step */
.type-selection-step {
  min-height: 400px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2rem;
}

.type-selection-header {
  text-align: center;
  margin-bottom: 2.5rem;
}

.type-selection-header h2 {
  font-size: 1.75rem;
  font-weight: 700;
  margin: 0 0 0.5rem;
  color: var(--text-primary);
}

.type-selection-header p {
  font-size: 1rem;
  color: var(--text-muted);
  margin: 0;
}

.type-cards {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  width: 100%;
  max-width: 500px;
}

.type-card {
  display: flex;
  align-items: center;
  gap: 1.25rem;
  padding: 1.5rem;
  background: var(--color-surface, var(--bg-card));
  border: 2px solid var(--border-color);
  border-radius: var(--radius-xl);
  cursor: pointer;
  transition: all 0.25s ease;
}

.type-card:hover {
  border-color: var(--color-brand);
  transform: translateX(8px);
  box-shadow: 0 8px 24px rgba(255, 91, 60, 0.15);
}

.type-card.selected {
  border-color: var(--color-brand);
  background: rgba(255, 91, 60, 0.08);
}

.type-card-icon {
  width: 64px;
  height: 64px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, rgba(255, 91, 60, 0.1) 0%, rgba(255, 138, 108, 0.1) 100%);
  border-radius: var(--radius-lg);
  font-size: 2rem;
  flex-shrink: 0;
}

.type-card:hover .type-card-icon {
  background: linear-gradient(135deg, rgba(255, 91, 60, 0.2) 0%, rgba(255, 138, 108, 0.2) 100%);
}

.type-card-content {
  flex: 1;
}

.type-card-content h3 {
  font-size: 1.125rem;
  font-weight: 600;
  margin: 0 0 0.25rem;
  color: var(--text-primary);
}

.type-card-content p {
  font-size: 0.875rem;
  color: var(--text-muted);
  margin: 0;
  line-height: 1.4;
}

.type-card-arrow {
  font-size: 1.5rem;
  color: var(--text-muted);
  transition: all 0.25s;
  opacity: 0;
}

.type-card:hover .type-card-arrow {
  opacity: 1;
  color: var(--color-brand);
  transform: translateX(4px);
}

/* Description Card */
.description-card {
  background: var(--color-surface, var(--bg-card));
  border-radius: var(--radius-xl);
  padding: 1.5rem;
  margin-bottom: 1.5rem;
  border: 1px solid var(--border-color);
}

.description-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.description-title {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.description-icon {
  font-size: 1.5rem;
}

.description-title h3 {
  margin: 0;
  font-size: 1.125rem;
  font-weight: 600;
}

.btn-translate {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background: linear-gradient(135deg, #4A90D9 0%, #357ABD 100%);
  color: white;
  border: none;
  border-radius: var(--radius-lg);
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-translate:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(74, 144, 217, 0.3);
}

.btn-translate:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.translation-error {
  background: rgba(220, 53, 69, 0.1);
  color: #dc3545;
  padding: 0.75rem 1rem;
  border-radius: var(--radius-md);
  margin-bottom: 1rem;
  font-size: 0.875rem;
}

.language-tabs {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
  background: var(--bg-secondary, #f5f5f5);
  padding: 0.25rem;
  border-radius: var(--radius-lg);
}

.language-tab {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  padding: 0.75rem 1rem;
  background: transparent;
  border: none;
  border-radius: var(--radius-md);
  cursor: pointer;
  transition: all 0.2s;
  font-size: 0.875rem;
  color: var(--text-muted);
}

.language-tab:hover {
  color: var(--text-primary);
  background: rgba(255, 255, 255, 0.5);
}

.language-tab.active {
  background: white;
  color: var(--text-primary);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
}

.tab-flag {
  font-size: 0.6875rem;
  font-weight: 700;
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  color: white;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.tab-flag.ru {
  background: linear-gradient(135deg, #0039A6 0%, #0039A6 33%, #D52B1E 66%, #D52B1E 100%);
}

.tab-flag.uz {
  background: linear-gradient(135deg, #1EB53A 0%, #0099B5 100%);
}

.tab-flag.en {
  background: linear-gradient(135deg, #012169 0%, #C8102E 100%);
}

.tab-label {
  font-weight: 500;
}

.tab-indicator {
  width: 16px;
  height: 16px;
  border-radius: 50%;
  background: var(--color-success, #28a745);
  color: white;
  font-size: 0.625rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

.description-content {
  margin-bottom: 0.75rem;
}

.description-tab-content {
  animation: fadeIn 0.2s ease;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(-4px); }
  to { opacity: 1; transform: translateY(0); }
}

.description-textarea {
  width: 100%;
  padding: 1rem;
  border: 1px solid var(--border-color);
  border-radius: var(--radius-lg);
  font-size: 0.9375rem;
  line-height: 1.6;
  resize: vertical;
  min-height: 120px;
  transition: all 0.2s;
  font-family: inherit;
  background: var(--bg-primary, white);
}

.description-textarea:focus {
  outline: none;
  border-color: var(--color-brand);
  box-shadow: 0 0 0 3px rgba(255, 91, 60, 0.1);
}

.description-textarea::placeholder {
  color: var(--text-muted);
}

.textarea-footer {
  display: flex;
  justify-content: flex-end;
  margin-top: 0.5rem;
}

.char-count {
  font-size: 0.75rem;
  color: var(--text-muted);
}

.description-hint {
  font-size: 0.8125rem;
  color: var(--text-muted);
  text-align: center;
  padding: 0.75rem;
  background: rgba(255, 193, 7, 0.1);
  border-radius: var(--radius-md);
}

.steps-progress {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
}

.step {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1rem;
  background: var(--color-surface);
  border-radius: var(--radius-lg);
  flex: 1;
  opacity: 0.5;
  transition: all 0.2s;
}

.step.active {
  opacity: 1;
  background: rgba(255, 91, 60, 0.1);
}

.step.completed {
  opacity: 1;
}

.step-number {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  background: var(--color-border);
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  font-size: 0.875rem;
}

.step.active .step-number {
  background: var(--color-brand);
  color: white;
}

.step.completed .step-number {
  background: var(--color-success);
  color: white;
}

.step-label {
  font-size: 0.875rem;
  font-weight: 500;
}

.form-container {
  background: var(--color-surface);
  border-radius: var(--radius-xl);
  padding: 2rem;
}

.form-step {
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

.step-title {
  font-size: 1.25rem;
  font-weight: 600;
  margin: 0 0 1.5rem;
  color: var(--color-text);
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.checkbox-group {
  display: flex;
  gap: 1.5rem;
}

.checkbox-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
  font-size: 0.9rem;
}

.checkbox-item input[type="checkbox"] {
  width: 20px;
  height: 20px;
  accent-color: var(--color-brand);
}

.form-textarea {
  resize: vertical;
  min-height: 80px;
}

/* Label Row with Translate Button */
.label-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.label-row .form-label {
  margin-bottom: 0;
}

.btn-translate {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.375rem 0.75rem;
  font-size: 0.75rem;
  font-weight: 500;
  color: #4f46e5;
  background: rgba(79, 70, 229, 0.1);
  border: 1px solid rgba(79, 70, 229, 0.2);
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-translate:hover:not(:disabled) {
  background: rgba(79, 70, 229, 0.2);
  border-color: rgba(79, 70, 229, 0.4);
}

.btn-translate:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Translation Loading */
.translation-loading {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1rem;
  margin-bottom: 1rem;
  background: rgba(79, 70, 229, 0.08);
  border-radius: 8px;
  color: #4f46e5;
  font-size: 0.875rem;
  font-weight: 500;
}

.mb-md {
  margin-bottom: 1rem;
}

/* Auto-Translate Button */
.btn-translate-all {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  width: 100%;
  padding: 0.75rem 1rem;
  margin-bottom: 1.5rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: #4f46e5;
  background: linear-gradient(135deg, rgba(79, 70, 229, 0.1), rgba(124, 58, 237, 0.1));
  border: 1px solid rgba(79, 70, 229, 0.2);
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-translate-all:hover:not(:disabled) {
  background: linear-gradient(135deg, rgba(79, 70, 229, 0.2), rgba(124, 58, 237, 0.2));
  border-color: rgba(79, 70, 229, 0.4);
}

.btn-translate-all:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.upload-zone {
  border: 2px dashed var(--color-border);
  border-radius: var(--radius-lg);
  padding: 2rem;
  text-align: center;
  cursor: pointer;
  transition: all 0.2s;
}

.upload-zone:hover {
  border-color: var(--color-brand);
  background: rgba(255, 91, 60, 0.05);
}

.form-actions {
  display: flex;
  gap: 1rem;
  margin-top: 2rem;
  padding-top: 1.5rem;
  border-top: 1px solid var(--color-border);
}

.spacer {
  flex: 1;
}

.loading-state {
  text-align: center;
  padding: 4rem;
  background: var(--color-surface);
  border-radius: var(--radius-xl);
}

/* Image Upload Styles */
.hidden-input {
  display: none;
}

.upload-zone.disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.image-preview-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(100px, 1fr));
  gap: 0.75rem;
  margin-bottom: 1rem;
}

.image-preview-item {
  position: relative;
  aspect-ratio: 1;
  border-radius: var(--radius-md);
  overflow: hidden;
  background: var(--color-background);
}

.image-preview-item img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.image-uploading {
  position: absolute;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
}

.image-remove-btn {
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
  font-size: 1rem;
  line-height: 1;
  display: flex;
  align-items: center;
  justify-content: center;
}

.image-remove-btn:hover {
  background: rgba(239, 68, 68, 0.9);
}
</style>
