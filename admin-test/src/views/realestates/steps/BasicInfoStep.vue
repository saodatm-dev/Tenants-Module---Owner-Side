<script setup lang="ts">
import { computed } from 'vue'
import type { PropertyTypeConfig } from '@/configs/propertyTypeFlows'
import type { Renovation } from '@/types/realestate'

const props = defineProps<{
  propertyType: PropertyTypeConfig
  modelValue: {
    number?: string
    totalArea: number
    livingArea?: number
    roomsCount?: number
    renovationId?: string
    ceilingHeight?: number
    totalFloors?: number
    aboveFloors?: number
    belowFloors?: number
    cadastralNumber?: string
  }
  renovations: Renovation[]
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: typeof props.modelValue): void
}>()

// SVG icons for property types
const PROPERTY_ICONS: Record<string, string> = {
  'Apartment': `<svg width="20" height="20" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M8 44V10L24 4L40 10V44" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    <path d="M8 44H40" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
    <rect x="14" y="14" width="6" height="6" rx="1" stroke="currentColor" stroke-width="2"/>
    <rect x="28" y="14" width="6" height="6" rx="1" stroke="currentColor" stroke-width="2"/>
    <rect x="14" y="26" width="6" height="6" rx="1" stroke="currentColor" stroke-width="2"/>
    <rect x="28" y="26" width="6" height="6" rx="1" stroke="currentColor" stroke-width="2"/>
    <path d="M20 44V36H28V44" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
  </svg>`,
  'House': `<svg width="20" height="20" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M6 22L24 6L42 22" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    <path d="M10 20V42H38V20" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    <path d="M20 42V30H28V42" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    <rect x="30" y="24" width="5" height="5" rx="1" stroke="currentColor" stroke-width="2"/>
  </svg>`,
  'Office': `<svg width="20" height="20" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
    <rect x="6" y="8" width="36" height="36" rx="2" stroke="currentColor" stroke-width="2"/>
    <path d="M6 16H42" stroke="currentColor" stroke-width="2"/>
    <rect x="12" y="22" width="8" height="6" rx="1" stroke="currentColor" stroke-width="2"/>
    <rect x="28" y="22" width="8" height="6" rx="1" stroke="currentColor" stroke-width="2"/>
    <rect x="12" y="34" width="8" height="6" rx="1" stroke="currentColor" stroke-width="2"/>
    <rect x="28" y="34" width="8" height="6" rx="1" stroke="currentColor" stroke-width="2"/>
  </svg>`,
  'Retail': `<svg width="20" height="20" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M6 18L10 8H38L42 18" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    <path d="M6 18H42V42H6V18Z" stroke="currentColor" stroke-width="2" stroke-linejoin="round"/>
    <path d="M6 18C6 20 8 22 12 22C16 22 18 20 18 18" stroke="currentColor" stroke-width="2"/>
    <path d="M18 18C18 20 20 22 24 22C28 22 30 20 30 18" stroke="currentColor" stroke-width="2"/>
    <path d="M30 18C30 20 32 22 36 22C40 22 42 20 42 18" stroke="currentColor" stroke-width="2"/>
    <path d="M18 42V30H30V42" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
  </svg>`,
  'Business Center': `<svg width="20" height="20" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
    <rect x="8" y="12" width="14" height="32" rx="1" stroke="currentColor" stroke-width="2"/>
    <rect x="26" y="4" width="14" height="40" rx="1" stroke="currentColor" stroke-width="2"/>
    <path d="M12 18H18" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
    <path d="M12 24H18" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
    <path d="M12 30H18" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
    <path d="M30 12H36" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
    <path d="M30 18H36" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
    <path d="M30 24H36" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
    <path d="M30 30H36" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
  </svg>`,
  'Shopping Center': `<svg width="20" height="20" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M4 16L24 6L44 16" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    <path d="M8 16V42H40V16" stroke="currentColor" stroke-width="2"/>
    <path d="M8 42H40" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
    <circle cx="24" cy="12" r="2" stroke="currentColor" stroke-width="2"/>
    <path d="M14 24H20V34H14V24Z" stroke="currentColor" stroke-width="2"/>
    <path d="M28 24H34V34H28V24Z" stroke="currentColor" stroke-width="2"/>
    <path d="M20 42V36H28V42" stroke="currentColor" stroke-width="2"/>
  </svg>`,
  'Land Plot': `<svg width="20" height="20" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M6 36L14 28L22 34L32 22L42 30" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    <path d="M4 40H44" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
    <circle cx="36" cy="14" r="4" stroke="currentColor" stroke-width="2"/>
    <path d="M14 20C14 20 16 16 16 14C16 12 14 10 12 10C10 10 8 12 8 14C8 16 10 20 10 20" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    <path d="M10 20L14 20" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
  </svg>`,
  'Warehouse': `<svg width="20" height="20" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M4 20L24 8L44 20" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    <path d="M8 18V40H40V18" stroke="currentColor" stroke-width="2"/>
    <path d="M8 40H40" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
    <path d="M16 40V28H32V40" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    <path d="M16 34H32" stroke="currentColor" stroke-width="2"/>
  </svg>`
}

function getIcon(name: string): string {
  return PROPERTY_ICONS[name] || ''
}

const form = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

function updateField<K extends keyof typeof props.modelValue>(field: K, value: typeof props.modelValue[K]) {
  emit('update:modelValue', { ...props.modelValue, [field]: value })
}

// Determine which fields to show based on property type
const showLivingArea = computed(() =>
  ['Apartment', 'House'].includes(props.propertyType.name)
)

const showRoomsCount = computed(() =>
  ['Apartment', 'House'].includes(props.propertyType.name)
)

const showRenovation = computed(() =>
  !['Land Plot'].includes(props.propertyType.name)
)

const showCeilingHeight = computed(() =>
  !['Land Plot'].includes(props.propertyType.name)
)

const showTotalFloors = computed(() =>
  !['Land Plot'].includes(props.propertyType.name)
)

const showCadastral = computed(() =>
  ['Office', 'Retail', 'Business Center', 'Shopping Center', 'Land Plot', 'Warehouse'].includes(props.propertyType.name)
)
</script>

<template>
  <div class="basic-info-step">
    <div class="step-header">
      <span class="type-badge">
        <span class="icon" v-html="getIcon(propertyType.name)"></span>
        {{ propertyType.name }}
      </span>
      <h2>Tell us about your {{ propertyType.name.toLowerCase() }}</h2>
      <p class="subtitle">Enter the basic details of your property</p>
    </div>

    <div class="form-section">
      <!-- Property Name -->
      <div class="form-group">
        <label for="property-name">Property Name</label>
        <input
          id="property-name"
          type="text"
          :value="form.number"
          @input="updateField('number', ($event.target as HTMLInputElement).value)"
          placeholder="e.g., My Apartment, Unit 4B"
        />
        <span class="hint">Give your property a memorable name</span>
      </div>

      <!-- Total Area (Required) -->
      <div class="form-group required">
        <label for="total-area">Total Area (m²)</label>
        <input
          id="total-area"
          type="number"
          :value="form.totalArea"
          @input="updateField('totalArea', parseFloat(($event.target as HTMLInputElement).value) || 0)"
          min="0"
          step="0.1"
          placeholder="0"
        />
      </div>

      <!-- Living Area (Residential only) -->
      <div v-if="showLivingArea" class="form-group">
        <label for="living-area">Living Area (m²)</label>
        <input
          id="living-area"
          type="number"
          :value="form.livingArea"
          @input="updateField('livingArea', parseFloat(($event.target as HTMLInputElement).value) || undefined)"
          min="0"
          step="0.1"
          placeholder="0"
        />
      </div>

      <!-- Rooms Count (Residential only) -->
      <div v-if="showRoomsCount" class="form-group">
        <label for="rooms-count">Number of Rooms</label>
        <div class="counter-input">
          <button
            type="button"
            class="counter-btn"
            :disabled="!form.roomsCount || form.roomsCount <= 0"
            @click="updateField('roomsCount', Math.max(0, (form.roomsCount || 0) - 1))"
          >−</button>
          <span class="counter-value">{{ form.roomsCount || 0 }}</span>
          <button
            type="button"
            class="counter-btn"
            @click="updateField('roomsCount', (form.roomsCount || 0) + 1)"
          >+</button>
        </div>
      </div>

      <!-- Renovation Level -->
      <div v-if="showRenovation" class="form-group">
        <label for="renovation">Renovation Level</label>
        <select
          id="renovation"
          :value="form.renovationId"
          @change="updateField('renovationId', ($event.target as HTMLSelectElement).value || undefined)"
        >
          <option value="">Not specified</option>
          <option v-for="ren in renovations" :key="ren.id" :value="ren.id">
            {{ ren.name }}
          </option>
        </select>
      </div>

      <!-- Ceiling Height -->
      <div v-if="showCeilingHeight" class="form-group">
        <label for="ceiling-height">Ceiling Height (m)</label>
        <input
          id="ceiling-height"
          type="number"
          :value="form.ceilingHeight"
          @input="updateField('ceilingHeight', parseFloat(($event.target as HTMLInputElement).value) || undefined)"
          min="0"
          step="0.1"
          placeholder="2.7"
        />
      </div>

      <!-- Total Floors (House only) -->
      <div v-if="showTotalFloors" class="form-group">
        <label for="total-floors">Total Floors</label>
        <div class="counter-input">
          <button
            type="button"
            class="counter-btn"
            :disabled="!form.totalFloors || form.totalFloors <= 1"
            @click="updateField('totalFloors', Math.max(1, (form.totalFloors || 1) - 1))"
          >−</button>
          <span class="counter-value">{{ form.totalFloors || 1 }}</span>
          <button
            type="button"
            class="counter-btn"
            @click="updateField('totalFloors', (form.totalFloors || 1) + 1)"
          >+</button>
        </div>
      </div>

      <!-- Above Ground Floors -->
      <div v-if="showTotalFloors" class="form-group">
        <label for="above-floors">Above Ground Floors</label>
        <input
          id="above-floors"
          type="number"
          :value="form.aboveFloors"
          @input="updateField('aboveFloors', parseInt(($event.target as HTMLInputElement).value) || undefined)"
          min="0"
          placeholder="0"
        />
      </div>

      <!-- Below Ground Floors -->
      <div v-if="showTotalFloors" class="form-group">
        <label for="below-floors">Below Ground Floors</label>
        <input
          id="below-floors"
          type="number"
          :value="form.belowFloors"
          @input="updateField('belowFloors', parseInt(($event.target as HTMLInputElement).value) || undefined)"
          min="0"
          placeholder="0"
        />
      </div>

      <!-- Cadastral Number -->
      <div v-if="showCadastral" class="form-group">
        <label for="cadastral">Cadastral Number</label>
        <input
          id="cadastral"
          type="text"
          :value="form.cadastralNumber"
          @input="updateField('cadastralNumber', ($event.target as HTMLInputElement).value || undefined)"
          placeholder="Registration number"
        />
      </div>
    </div>
  </div>
</template>

<style scoped>
.basic-info-step {
  padding: 20px 0;
}

.step-header {
  text-align: center;
  margin-bottom: 36px;
}

.type-badge {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  background: linear-gradient(135deg, #fff5f3 0%, #ffe8e4 100%);
  color: #FF5B3C;
  padding: 8px 16px;
  border-radius: 20px;
  font-size: 14px;
  font-weight: 600;
  margin-bottom: 16px;
}

.type-badge .icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 20px;
  height: 20px;
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

.form-section {
  max-width: 500px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-size: 14px;
  font-weight: 600;
  color: #374151;
}

.form-group.required label::after {
  content: ' *';
  color: #ef4444;
}

.form-group input,
.form-group select {
  padding: 14px 16px;
  border: 2px solid #e5e7eb;
  border-radius: 12px;
  font-size: 16px;
  transition: all 0.2s;
  background: #fff;
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: #FF5B3C;
  box-shadow: 0 0 0 4px rgba(255, 91, 60, 0.1);
}

.form-group input::placeholder {
  color: #9ca3af;
}

.hint {
  font-size: 12px;
  color: #9ca3af;
}

/* Counter Input (for rooms, floors) */
.counter-input {
  display: flex;
  align-items: center;
  gap: 16px;
  background: #f9fafb;
  border-radius: 12px;
  padding: 8px;
  width: fit-content;
}

.counter-btn {
  width: 44px;
  height: 44px;
  border-radius: 50%;
  border: 2px solid #e5e7eb;
  background: #fff;
  font-size: 20px;
  font-weight: 600;
  color: #FF5B3C;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
}

.counter-btn:hover:not(:disabled) {
  background: #FF5B3C;
  color: #fff;
  border-color: #FF5B3C;
}

.counter-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.counter-value {
  font-size: 24px;
  font-weight: 700;
  color: #1a1a2e;
  min-width: 40px;
  text-align: center;
}
</style>
