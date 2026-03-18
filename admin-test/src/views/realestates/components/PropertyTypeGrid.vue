<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { api } from '@/services/api'
import type { RealEstateType } from '@/types/realestate'

const props = defineProps<{
  modelValue?: string
  compact?: boolean
}>()

const emit = defineEmits<{
  'update:modelValue': [value: string]
  'select': [type: RealEstateType]
}>()

const types = ref<RealEstateType[]>([])
const loading = ref(true)

// Icon mapping for property types
const typeIcons: Record<string, string> = {
  'apartment': '🏢',
  'house': '🏠',
  'office': '🏛️',
  'warehouse': '🏭',
  'commercial': '🏪',
  'land': '🌍',
  'garage': '🚗',
  'parking': '🅿️',
  'default': '🏠'
}

function getIcon(typeName: string): string {
  const key = typeName.toLowerCase()
  for (const [pattern, icon] of Object.entries(typeIcons)) {
    if (key.includes(pattern)) return icon
  }
  return typeIcons['default']!
}

const selectedType = computed(() =>
  types.value.find(t => t.id === props.modelValue)
)

function selectType(type: RealEstateType) {
  emit('update:modelValue', type.id)
  emit('select', type)
}

onMounted(async () => {
  try {
    types.value = await api.getRealEstateTypes() || []
  } catch (e) {
    console.error('Failed to load property types:', e)
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div class="property-type-grid" :class="{ compact: compact }">
    <div v-if="loading" class="loading-grid">
      <div v-for="i in 6" :key="i" class="type-card skeleton"></div>
    </div>

    <div v-else class="types-grid">
      <button
        v-for="type in types"
        :key="type.id"
        class="type-card"
        :class="{ selected: modelValue === type.id }"
        @click="selectType(type)"
        type="button"
      >
        <span class="type-icon">{{ getIcon(type.name) }}</span>
        <span class="type-name">{{ type.name }}</span>
        <div class="check-indicator" v-if="modelValue === type.id">✓</div>
      </button>
    </div>
  </div>
</template>

<style scoped>
.property-type-grid {
  width: 100%;
}

.types-grid,
.loading-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(140px, 1fr));
  gap: 12px;
}

.compact .types-grid,
.compact .loading-grid {
  grid-template-columns: repeat(auto-fill, minmax(100px, 1fr));
  gap: 8px;
}

.type-card {
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 8px;
  padding: 20px 16px;
  background: #ffffff;
  border: 2px solid #e5e7eb;
  border-radius: 16px;
  cursor: pointer;
  transition: all 0.2s ease;
  min-height: 100px;
}

.compact .type-card {
  padding: 12px 8px;
  min-height: 70px;
  border-radius: 12px;
}

.type-card:hover {
  border-color: #FF5B3C;
  background: #fff5f3;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(255, 91, 60, 0.15);
}

.type-card.selected {
  border-color: #FF5B3C;
  background: linear-gradient(135deg, #fff5f3 0%, #ffe8e3 100%);
  box-shadow: 0 4px 16px rgba(255, 91, 60, 0.25);
}

.type-icon {
  font-size: 28px;
  line-height: 1;
}

.compact .type-icon {
  font-size: 22px;
}

.type-name {
  font-size: 13px;
  font-weight: 500;
  color: #374151;
  text-align: center;
  line-height: 1.3;
}

.compact .type-name {
  font-size: 11px;
}

.check-indicator {
  position: absolute;
  top: 8px;
  right: 8px;
  width: 20px;
  height: 20px;
  background: #FF5B3C;
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  font-weight: bold;
}

.compact .check-indicator {
  width: 16px;
  height: 16px;
  font-size: 10px;
  top: 4px;
  right: 4px;
}

/* Skeleton loading */
.type-card.skeleton {
  background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
}

@keyframes shimmer {
  0% { background-position: 200% 0; }
  100% { background-position: -200% 0; }
}
</style>
