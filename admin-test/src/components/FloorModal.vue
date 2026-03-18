<script setup lang="ts">
import { ref, computed, watch, onUnmounted } from 'vue'
import { api } from '@/services/api'
import type { Floor, CreateFloorCommand, UpdateFloorCommand } from '@/types/building'
import { FloorType, FloorTypeLabels } from '@/types/building'

const props = defineProps<{
  buildingId?: string
  realEstateId?: string
  floor?: Floor | null
  isOpen: boolean
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'saved'): void
}>()

const isLoading = ref(false)
const errorMessage = ref('')

// Form data
const formData = ref({
  number: 1,
  type: null as FloorType | null,
  label: '' as string,
  totalArea: null as number | null,
  ceilingHeight: null as number | null,
  plan: null as string | null
})

// Floor type options for dropdown
const floorTypeOptions = Object.entries(FloorTypeLabels).map(([value, label]) => ({
  value: Number(value) as FloorType,
  label
}))

// Floor plan image handling
const planFile = ref<File | null>(null)
const planPreviewUrl = ref<string | null>(null)

const isEditMode = computed(() => !!props.floor)
const modalTitle = computed(() => isEditMode.value ? 'Edit Floor' : 'Add Floor')

// Reset form when modal opens/closes or floor changes
watch(() => [props.isOpen, props.floor], () => {
  if (props.isOpen) {
    if (props.floor) {
      // Edit mode - populate form
      formData.value = {
        number: props.floor.number,
        type: props.floor.type ?? null,
        label: props.floor.label ?? '',
        totalArea: props.floor.totalArea ?? null,
        ceilingHeight: props.floor.ceilingHeight ?? null,
        plan: props.floor.plan ?? null
      }
      // Load existing plan image - plan is now a full URL
      if (props.floor.plan) {
        planPreviewUrl.value = props.floor.plan
      }
    } else {
      // Create mode - reset form
      formData.value = {
        number: 1,
        type: null,
        label: '',
        totalArea: null,
        ceilingHeight: null,
        plan: null
      }
      planFile.value = null
      planPreviewUrl.value = null
    }
    errorMessage.value = ''
  }
}, { immediate: true })

function handlePlanSelect(event: Event) {
  const input = event.target as HTMLInputElement
  const file = input.files?.[0]
  if (file) {
    planFile.value = file
    // Create preview
    if (planPreviewUrl.value) {
      URL.revokeObjectURL(planPreviewUrl.value)
    }
    planPreviewUrl.value = URL.createObjectURL(file)
  }
}

function removePlan() {
  planFile.value = null
  if (planPreviewUrl.value) {
    URL.revokeObjectURL(planPreviewUrl.value)
    planPreviewUrl.value = null
  }
  formData.value.plan = null
}

async function handleSubmit() {
  if (formData.value.number === null || formData.value.number === undefined) {
    errorMessage.value = 'Enter floor number'
    return
  }

  isLoading.value = true
  errorMessage.value = ''

  try {
    // Upload plan image if selected
    let planKey = formData.value.plan
    if (planFile.value) {
      planKey = await api.uploadFile(planFile.value)
    }

    if (isEditMode.value && props.floor) {
      // Update floor
      const updateData: UpdateFloorCommand = {
        id: props.floor.id,
        buildingId: props.buildingId || null,
        realEstateId: props.realEstateId || null,
        number: formData.value.number,
        type: formData.value.type,
        label: formData.value.label || null,
        totalArea: formData.value.totalArea,
        ceilingHeight: formData.value.ceilingHeight,
        plan: planKey
      }
      await api.updateFloor(updateData)
    } else {
      // Create floor
      const createData: CreateFloorCommand = {
        buildingId: props.buildingId || null,
        realEstateId: props.realEstateId || null,
        number: formData.value.number,
        type: formData.value.type,
        label: formData.value.label || null,
        totalArea: formData.value.totalArea,
        ceilingHeight: formData.value.ceilingHeight,
        plan: planKey
      }
      await api.createFloor(createData)
    }

    emit('saved')
    emit('close')
  } catch (error) {
    console.error('Failed to save floor:', error)
    errorMessage.value = error instanceof Error ? error.message : 'Error saving floor'
  } finally {
    isLoading.value = false
  }
}

function handleClose() {
  if (!isLoading.value) {
    emit('close')
  }
}

// Cleanup blob URLs on unmount
onUnmounted(() => {
  if (planPreviewUrl.value && planFile.value) {
    URL.revokeObjectURL(planPreviewUrl.value)
  }
})
</script>

<template>
  <Teleport to="body">
    <Transition name="modal">
      <div v-if="isOpen" class="modal-backdrop" @click.self="handleClose">
        <div class="modal-container">
          <div class="modal-header">
            <h2 class="modal-title">{{ modalTitle }}</h2>
            <button class="modal-close" @click="handleClose" :disabled="isLoading">
              ✕
            </button>
          </div>

          <form @submit.prevent="handleSubmit" class="modal-body">
            <!-- Error Alert -->
            <div v-if="errorMessage" class="alert alert-error mb-md">
              {{ errorMessage }}
            </div>

            <!-- Floor Number -->
            <div class="form-group">
              <label class="form-label">Floor Number <span class="required">*</span></label>
              <input
                v-model.number="formData.number"
                type="number"
                class="form-input"
                placeholder="1"
                required
              />
            </div>

            <!-- Floor Type and Label Row -->
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Floor Type</label>
                <select v-model="formData.type" class="form-input">
                  <option :value="null">— Not specified —</option>
                  <option v-for="opt in floorTypeOptions" :key="opt.value" :value="opt.value">
                    {{ opt.label }}
                  </option>
                </select>
              </div>
              <div class="form-group">
                <label class="form-label">Label</label>
                <input
                  v-model="formData.label"
                  type="text"
                  class="form-input"
                  placeholder="e.g. B1, G, PH"
                  maxlength="20"
                />
              </div>
            </div>

            <!-- Area and Height Row -->
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Area (m²)</label>
                <input
                  v-model.number="formData.totalArea"
                  type="number"
                  step="0.01"
                  min="0"
                  class="form-input"
                  placeholder="100.5"
                />
              </div>
              <div class="form-group">
                <label class="form-label">Ceiling Height (m)</label>
                <input
                  v-model.number="formData.ceilingHeight"
                  type="number"
                  step="0.01"
                  min="0"
                  class="form-input"
                  placeholder="3.0"
                />
              </div>
            </div>



            <!-- Floor Plan Upload -->
            <div class="form-group">
              <label class="form-label">Floor Plan</label>
              <div class="plan-upload-area">
                <div v-if="planPreviewUrl" class="plan-preview">
                  <img :src="planPreviewUrl" alt="Floor plan" />
                  <button type="button" class="plan-remove" @click="removePlan">
                    🗑️ Delete
                  </button>
                </div>
                <label v-else class="plan-dropzone">
                  <input
                    type="file"
                    accept="image/*"
                    @change="handlePlanSelect"
                    class="sr-only"
                  />
                  <div class="dropzone-content">
                    <span class="dropzone-icon">📐</span>
                    <span class="dropzone-text">Click to upload floor plan</span>
                    <span class="dropzone-hint">PNG, JPG до 10MB</span>
                  </div>
                </label>
              </div>
            </div>

            <!-- Actions -->
            <div class="modal-actions">
              <button type="button" class="btn btn-secondary" @click="handleClose" :disabled="isLoading">
                Cancel
              </button>
              <button type="submit" class="btn btn-primary" :disabled="isLoading">
                <span v-if="isLoading" class="spinner"></span>
                {{ isEditMode ? 'Save' : 'Add' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.modal-backdrop {
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

.modal-container {
  background: var(--bg-primary);
  border-radius: var(--radius-xl);
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
  width: 100%;
  max-width: 500px;
  max-height: 90vh;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1.25rem 1.5rem;
  border-bottom: 1px solid var(--border-color);
}

.modal-title {
  font-size: 1.25rem;
  font-weight: 600;
  margin: 0;
  color: var(--text-primary);
}

.modal-close {
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: transparent;
  border: none;
  border-radius: var(--radius-md);
  color: var(--text-muted);
  cursor: pointer;
  transition: all var(--transition-fast);
  font-size: 1rem;
}

.modal-close:hover:not(:disabled) {
  background: var(--color-primary-light);
  color: var(--text-primary);
}

.modal-body {
  padding: 1.5rem;
  overflow-y: auto;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.form-textarea {
  resize: vertical;
  min-height: 80px;
}

.required {
  color: var(--color-error);
}

/* Plan Upload */
.plan-upload-area {
  border-radius: var(--radius-lg);
  overflow: hidden;
}

.plan-dropzone {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  background: var(--bg-input);
  border: 2px dashed var(--border-color);
  border-radius: var(--radius-lg);
  cursor: pointer;
  transition: all var(--transition-base);
}

.plan-dropzone:hover {
  border-color: var(--color-brand);
  background: var(--color-brand-light);
}

.dropzone-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  text-align: center;
}

.dropzone-icon {
  font-size: 2rem;
}

.dropzone-text {
  font-weight: 500;
  color: var(--text-primary);
}

.dropzone-hint {
  font-size: 0.8rem;
  color: var(--text-muted);
}

.plan-preview {
  position: relative;
  background: var(--bg-secondary);
  border-radius: var(--radius-lg);
  overflow: hidden;
}

.plan-preview img {
  width: 100%;
  max-height: 200px;
  object-fit: contain;
}

.plan-remove {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
  padding: 0.5rem 0.75rem;
  background: rgba(255, 255, 255, 0.9);
  border: none;
  border-radius: var(--radius-md);
  cursor: pointer;
  font-size: 0.8rem;
  transition: all var(--transition-fast);
}

.plan-remove:hover {
  background: var(--color-error-light);
  color: var(--color-error);
}

.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0, 0, 0, 0);
  border: 0;
}

/* Modal Actions */
.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  margin-top: 1.5rem;
  padding-top: 1.5rem;
  border-top: 1px solid var(--border-color);
}

/* Transitions */
.modal-enter-active,
.modal-leave-active {
  transition: all 0.25s ease;
}

.modal-enter-active .modal-container,
.modal-leave-active .modal-container {
  transition: all 0.25s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-from .modal-container,
.modal-leave-to .modal-container {
  transform: scale(0.95) translateY(-10px);
  opacity: 0;
}
</style>
