<script setup lang="ts">
import { ref, onBeforeUnmount } from 'vue'
import { api } from '@/services/api'
import { compressImage } from '@/utils/imageCompression'
import type { PropertyTypeConfig } from '@/configs/propertyTypeFlows'

interface ImageFile {
  file: File
  preview: string
  uploading: boolean
  key?: string
}

const props = defineProps<{
  propertyType: PropertyTypeConfig
  existingPlanUrl?: string | null
  existingImages?: { id: string; image: string; url: string }[]
}>()

// Use propertyType in template
const propertyType = props.propertyType

const emit = defineEmits<{
  (e: 'update:planKey', key: string | null): void
  (e: 'update:imageKeys', keys: string[]): void
  (e: 'delete:existingImage', id: string): void
}>()

// Local state for new uploads
const imageFiles = ref<ImageFile[]>([])
const planFile = ref<{ file: File; preview: string; uploading: boolean } | null>(null)
const uploadedImageKeys = ref<string[]>([])
const uploadedPlanKey = ref<string | null>(null)

// Drag state
const isDragging = ref(false)

// Handle image selection
async function handleImageSelect(event: Event) {
  const input = event.target as HTMLInputElement
  if (!input.files) return
  await processFiles(Array.from(input.files))
  input.value = ''
}

// Handle drag and drop
function handleDrop(event: DragEvent) {
  event.preventDefault()
  isDragging.value = false

  if (event.dataTransfer?.files) {
    processFiles(Array.from(event.dataTransfer.files))
  }
}

async function processFiles(files: File[]) {
  for (const file of files) {
    if (!file.type.startsWith('image/')) continue

    const preview = URL.createObjectURL(file)
    const imageItem: ImageFile = { file, preview, uploading: true }
    imageFiles.value.push(imageItem)

    try {
      const compressedFile = await compressImage(file)
      const key = await api.uploadFile(compressedFile)
      imageItem.uploading = false
      imageItem.key = key
      uploadedImageKeys.value.push(key)
      emit('update:imageKeys', uploadedImageKeys.value)
    } catch (e) {
      console.error('Failed to upload image:', e)
      const index = imageFiles.value.indexOf(imageItem)
      if (index > -1) {
        URL.revokeObjectURL(imageItem.preview)
        imageFiles.value.splice(index, 1)
      }
    }
  }
}

function removeImage(index: number) {
  const item = imageFiles.value[index]
  if (item) {
    URL.revokeObjectURL(item.preview)
    if (item.key) {
      const keyIndex = uploadedImageKeys.value.indexOf(item.key)
      if (keyIndex > -1) {
        uploadedImageKeys.value.splice(keyIndex, 1)
        emit('update:imageKeys', uploadedImageKeys.value)
      }
    }
  }
  imageFiles.value.splice(index, 1)
}

function removeExistingImage(id: string) {
  emit('delete:existingImage', id)
}

// Plan handling
async function handlePlanSelect(event: Event) {
  const input = event.target as HTMLInputElement
  const file = input.files?.[0]
  if (!file) return

  const preview = URL.createObjectURL(file)
  planFile.value = { file, preview, uploading: true }
  input.value = ''

  try {
    const compressedFile = await compressImage(file)
    const key = await api.uploadFile(compressedFile)
    uploadedPlanKey.value = key
    planFile.value.uploading = false
    emit('update:planKey', key)
  } catch (e) {
    console.error('Failed to upload plan:', e)
    URL.revokeObjectURL(preview)
    planFile.value = null
  }
}

function removePlan() {
  if (planFile.value) {
    URL.revokeObjectURL(planFile.value.preview)
    planFile.value = null
  }
  uploadedPlanKey.value = null
  emit('update:planKey', null)
}

// Cleanup
onBeforeUnmount(() => {
  imageFiles.value.forEach(img => URL.revokeObjectURL(img.preview))
  if (planFile.value) URL.revokeObjectURL(planFile.value.preview)
})
</script>

<template>
  <div class="media-step">
    <div class="step-header">
      <h2>Add photos of your {{ propertyType.name.toLowerCase() }}</h2>
      <p class="subtitle">Great photos help your property stand out</p>
    </div>

    <div class="media-sections">
      <!-- Floor Plan Section -->
      <div class="media-section">
        <div class="section-header">
          <h3>📋 Floor Plan</h3>
          <span class="optional-badge">Optional</span>
        </div>

        <div
          class="upload-zone plan-zone"
          @click="!planFile && !existingPlanUrl && ($refs.planInput as HTMLInputElement).click()"
        >
          <input
            ref="planInput"
            type="file"
            accept="image/*"
            class="hidden-input"
            @change="handlePlanSelect"
          />

          <template v-if="!planFile && !existingPlanUrl">
            <span class="upload-icon">📐</span>
            <span class="upload-text">Upload floor plan</span>
            <span class="upload-hint">Click to browse</span>
          </template>

          <template v-else>
            <div class="plan-preview">
              <img :src="planFile?.preview || existingPlanUrl || ''" alt="Floor plan" />
              <div v-if="planFile?.uploading" class="uploading-overlay">
                <div class="spinner"></div>
              </div>
              <button class="remove-btn" @click.stop="removePlan">×</button>
            </div>
          </template>
        </div>
      </div>

      <!-- Property Photos Section -->
      <div class="media-section">
        <div class="section-header">
          <h3>📷 Property Photos</h3>
        </div>

        <div
          class="photos-dropzone"
          :class="{ dragging: isDragging }"
          @dragover.prevent="isDragging = true"
          @dragleave.prevent="isDragging = false"
          @drop="handleDrop"
        >
          <!-- Existing images -->
          <div
            v-for="img in existingImages"
            :key="'existing-' + img.id"
            class="photo-item"
          >
            <img :src="img.url" alt="Photo" />
            <button class="remove-btn" @click="removeExistingImage(img.id)">×</button>
          </div>

          <!-- New uploads -->
          <div
            v-for="(img, index) in imageFiles"
            :key="'new-' + index"
            class="photo-item"
          >
            <img :src="img.preview" alt="Photo" />
            <div v-if="img.uploading" class="uploading-overlay">
              <div class="spinner small"></div>
            </div>
            <button class="remove-btn" @click="removeImage(index)">×</button>
          </div>

          <!-- Add more button -->
          <div class="add-photo-btn" @click="($refs.imagesInput as HTMLInputElement).click()">
            <input
              ref="imagesInput"
              type="file"
              accept="image/*"
              multiple
              class="hidden-input"
              @change="handleImageSelect"
            />
            <span class="add-icon">+</span>
            <span class="add-text">Add photos</span>
          </div>
        </div>

        <p class="drop-hint">
          {{ isDragging ? 'Drop photos here!' : 'Drag and drop photos here or click to browse' }}
        </p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.media-step {
  padding: 20px 0;
}

.step-header {
  text-align: center;
  margin-bottom: 32px;
}

.step-header h2 {
  font-size: 28px;
  font-weight: 700;
  color: #1a1a2e;
  margin: 0 0 8px 0;
}

.subtitle {
  color: #6b7280;
  font-size: 16px;
  margin: 0;
}

.media-sections {
  max-width: 700px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  gap: 32px;
}

.media-section {
  background: #fff;
  border-radius: 16px;
  padding: 24px;
  border: 2px solid #e5e7eb;
}

.section-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 16px;
}

.section-header h3 {
  font-size: 18px;
  font-weight: 600;
  color: #1a1a2e;
  margin: 0;
}

.optional-badge {
  font-size: 11px;
  font-weight: 600;
  color: #9ca3af;
  background: #f3f4f6;
  padding: 4px 8px;
  border-radius: 4px;
  text-transform: uppercase;
}

/* Upload Zones */
.upload-zone {
  border: 2px dashed #d1d5db;
  border-radius: 12px;
  padding: 40px;
  text-align: center;
  cursor: pointer;
  transition: all 0.2s;
  background: #fafafa;
}

.upload-zone:hover {
  border-color: #FF5B3C;
  background: #fff8f7;
}

.upload-icon {
  font-size: 48px;
  display: block;
  margin-bottom: 12px;
}

.upload-text {
  font-size: 16px;
  font-weight: 600;
  color: #374151;
  display: block;
  margin-bottom: 4px;
}

.upload-hint {
  font-size: 13px;
  color: #9ca3af;
}

.plan-preview {
  position: relative;
  display: inline-block;
}

.plan-preview img {
  max-width: 100%;
  max-height: 200px;
  border-radius: 8px;
}

/* Photos Grid */
.photos-dropzone {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
  gap: 12px;
  padding: 16px;
  background: #fafafa;
  border-radius: 12px;
  border: 2px dashed #d1d5db;
  min-height: 160px;
  transition: all 0.2s;
}

.photos-dropzone.dragging {
  border-color: #FF5B3C;
  background: #fff5f3;
}

.photo-item {
  position: relative;
  aspect-ratio: 1;
  border-radius: 10px;
  overflow: hidden;
  background: #e5e7eb;
}

.photo-item img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.add-photo-btn {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  aspect-ratio: 1;
  border: 2px dashed #d1d5db;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.2s;
  background: #fff;
}

.add-photo-btn:hover {
  border-color: #FF5B3C;
  background: #fff8f7;
}

.add-icon {
  font-size: 28px;
  color: #FF5B3C;
  font-weight: 300;
}

.add-text {
  font-size: 11px;
  color: #6b7280;
  margin-top: 4px;
}

.drop-hint {
  text-align: center;
  color: #9ca3af;
  font-size: 13px;
  margin: 12px 0 0 0;
}

/* Uploading Overlay */
.uploading-overlay {
  position: absolute;
  inset: 0;
  background: rgba(255, 255, 255, 0.8);
  display: flex;
  align-items: center;
  justify-content: center;
}

.spinner {
  width: 32px;
  height: 32px;
  border: 3px solid #e5e7eb;
  border-top-color: #FF5B3C;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

.spinner.small {
  width: 24px;
  height: 24px;
  border-width: 2px;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

/* Remove Button */
.remove-btn {
  position: absolute;
  top: 6px;
  right: 6px;
  width: 24px;
  height: 24px;
  border: none;
  background: rgba(0, 0, 0, 0.6);
  color: #fff;
  border-radius: 50%;
  font-size: 16px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background 0.2s;
}

.remove-btn:hover {
  background: #dc2626;
}

.hidden-input {
  display: none;
}
</style>
