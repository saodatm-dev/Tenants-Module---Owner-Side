<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { api } from '@/services/api'

interface AmenityItem {
  id: string
  name: string
  iconUrl: string
}

interface AmenityCategory {
  id: string
  name: string
  amenities: AmenityItem[]
}

const props = defineProps<{
  modelValue: string[]
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: string[]): void
}>()

// State
const categories = ref<AmenityCategory[]>([])
const loading = ref(false)

// Load amenities grouped by category from API
async function loadCategories() {
  loading.value = true
  try {
    const res = await api.getAmenityCategoriesWithAmenities()
    categories.value = res || []
  } catch (e) {
    console.error('Failed to load amenity categories:', e)
  } finally {
    loading.value = false
  }
}

// Selection handlers
function toggleAmenity(amenityId: string) {
  const current = [...props.modelValue]
  const index = current.indexOf(amenityId)
  if (index >= 0) {
    current.splice(index, 1)
  } else {
    current.push(amenityId)
  }
  emit('update:modelValue', current)
}

function isAmenitySelected(amenityId: string): boolean {
  return props.modelValue.includes(amenityId)
}

// Lifecycle
onMounted(() => {
  loadCategories()
})
</script>

<template>
  <div class="amenities-step">
    <!-- Loading -->
    <div v-if="loading" class="loading-state">
      <div class="spinner-small"></div>
      <span>Loading amenities...</span>
    </div>

    <!-- Amenities as pill chips grouped by category -->
    <div v-else-if="categories.length > 0" class="categories-list">
      <div
        v-for="category in categories"
        :key="category.id"
        class="category-section"
      >
        <div class="category-label">{{ category.name }}</div>
        <div class="chips-row">
          <button
            v-for="amenity in category.amenities"
            :key="amenity.id"
            class="amenity-chip"
            :class="{ selected: isAmenitySelected(amenity.id) }"
            @click="toggleAmenity(amenity.id)"
          >
            <img v-if="amenity.iconUrl" :src="amenity.iconUrl" class="chip-icon" alt="" />
            <span class="chip-label">{{ amenity.name }}</span>
          </button>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-else class="empty-state">
      <p>No amenities available</p>
    </div>

    <!-- Selection Summary -->
    <div v-if="modelValue.length > 0" class="selection-summary">
      Selected: {{ modelValue.length }} amenity(ies)
    </div>
  </div>
</template>

<style scoped>
.amenities-step {
  padding: 0;
}

/* Loading */
.loading-state {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 40px 0;
  justify-content: center;
  color: #666;
}

.spinner-small {
  width: 20px;
  height: 20px;
  border: 2px solid #e5e7eb;
  border-top-color: #FF5B3C;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

/* Categories */
.categories-list {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.category-section {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.category-label {
  font-size: 15px;
  font-weight: 600;
  color: #1B1B1B;
}

/* Chips row — wrapping flow */
.chips-row {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

/* Pill chip */
.amenity-chip {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 12px 20px;
  border-radius: 24px;
  border: none;
  background: #F4F4F4;
  cursor: pointer;
  transition: background 0.15s ease;
  white-space: nowrap;
  font-family: inherit;
}

.amenity-chip:hover {
  background: #EBEBEB;
}

.amenity-chip.selected {
  background: #E8E8E8;
}

.chip-icon {
  width: 20px;
  height: 20px;
  object-fit: contain;
  flex-shrink: 0;
}

.chip-label {
  font-size: 14px;
  font-weight: 400;
  color: #1B1B1B;
  line-height: 1;
}

/* Empty State */
.empty-state {
  text-align: center;
  padding: 40px;
  color: #666;
}

/* Selection Summary */
.selection-summary {
  margin-top: 24px;
  padding: 12px 16px;
  background: rgba(255, 91, 60, 0.08);
  border-radius: 10px;
  color: #FF5B3C;
  font-weight: 500;
  text-align: center;
}

/* Responsive */
@media (max-width: 600px) {
  .chips-row {
    gap: 6px;
  }

  .amenity-chip {
    padding: 10px 16px;
    border-radius: 20px;
  }

  .chip-label {
    font-size: 13px;
  }
}
</style>
