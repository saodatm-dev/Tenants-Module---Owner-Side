<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { api } from '@/services/api'
import {
  buildPropertyTypeConfigs,
  type PropertyTypeConfig
} from '@/configs/propertyTypeFlows'

const props = defineProps<{
  selectedTypeId?: string
}>()

const emit = defineEmits<{
  (e: 'select', config: PropertyTypeConfig): void
}>()

const configs = ref<PropertyTypeConfig[]>([])
const loading = ref(false)

async function loadTypes() {
  loading.value = true
  try {
    const apiTypes = await api.getRealEstateTypes()
    configs.value = buildPropertyTypeConfigs(apiTypes)
  } catch (e) {
    console.error('Failed to load property types:', e)
  } finally {
    loading.value = false
  }
}

function selectType(config: PropertyTypeConfig) {
  emit('select', config)
}

function isSelected(config: PropertyTypeConfig): boolean {
  return props.selectedTypeId === config.id
}

onMounted(() => loadTypes())
</script>

<template>
  <div class="property-type-selector">
    <div class="selector-header">
      <h2>What type of property are you adding?</h2>
      <p class="subtitle">Choose the option that best describes your property</p>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Loading property types...</p>
    </div>

    <div v-else class="type-grid">
      <button
        v-for="config in configs"
        :key="config.id"
        class="type-card"
        :class="{ selected: isSelected(config) }"
        @click="selectType(config)"
      >
        <span class="type-icon">
          <img v-if="config.icon" :src="config.icon" :alt="config.name" class="type-icon-img" />
        </span>
        <span class="type-name">{{ config.name }}</span>
        <span class="type-description">{{ config.description }}</span>
        <div v-if="isSelected(config)" class="selected-indicator">
          <svg width="20" height="20" viewBox="0 0 20 20" fill="none">
            <circle cx="10" cy="10" r="10" fill="currentColor"/>
            <path d="M6 10L9 13L14 7" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
        </div>
      </button>
    </div>
  </div>
</template>

<style scoped>
.property-type-selector {
  padding: 20px 0;
}

.selector-header {
  text-align: center;
  margin-bottom: 32px;
}

.selector-header h2 {
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

/* Loading */
.loading-state {
  text-align: center;
  padding: 60px 24px;
  color: #6b7280;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 3px solid #e5e7eb;
  border-top-color: #FF5B3C;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
  margin: 0 auto 16px;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.type-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 16px;
  max-width: 900px;
  margin: 0 auto;
}

.type-card {
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 28px 20px;
  background: #fff;
  border: 2px solid #e5e7eb;
  border-radius: 16px;
  cursor: pointer;
  transition: all 0.2s ease;
  text-align: center;
}

.type-card:hover {
  border-color: #FFB3A0;
  background: #f8faff;
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(255, 91, 60, 0.1);
}

.type-card.selected {
  border-color: #FF5B3C;
  background: linear-gradient(135deg, #fff5f3 0%, #fff 100%);
  box-shadow: 0 4px 20px rgba(255, 91, 60, 0.15);
}

.type-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 56px;
  height: 56px;
  margin-bottom: 14px;
  border-radius: 16px;
  background: rgba(107, 114, 128, 0.06);
  transition: all 0.2s ease;
}

.type-card:hover .type-icon,
.type-card.selected .type-icon {
  background: rgba(255, 91, 60, 0.08);
}

.type-icon-img {
  width: 28px;
  height: 28px;
  /* Default: gray */
  filter: brightness(0) saturate(100%) invert(46%) sepia(7%) saturate(674%) hue-rotate(182deg) brightness(94%) contrast(89%);
  transition: filter 0.2s ease;
}

.type-card:hover .type-icon-img,
.type-card.selected .type-icon-img {
  /* Accent: #FF5B3C */
  filter: brightness(0) saturate(100%) invert(42%) sepia(93%) saturate(1232%) hue-rotate(343deg) brightness(101%) contrast(101%);
}

.type-card:hover .type-icon-img {
  transform: scale(1.1);
}

.type-name {
  font-size: 18px;
  font-weight: 600;
  color: #1a1a2e;
  margin-bottom: 4px;
}

.type-description {
  font-size: 13px;
  color: #6b7280;
  line-height: 1.4;
}

.selected-indicator {
  position: absolute;
  top: 12px;
  right: 12px;
  color: #FF5B3C;
}

/* Responsive */
@media (max-width: 600px) {
  .type-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 12px;
  }

  .type-card {
    padding: 20px 12px;
  }

  .type-icon {
    width: 48px;
    height: 48px;
  }

  .type-icon-img {
    width: 24px;
    height: 24px;
  }

  .type-name {
    font-size: 14px;
  }

  .type-description {
    font-size: 11px;
  }
}
</style>
