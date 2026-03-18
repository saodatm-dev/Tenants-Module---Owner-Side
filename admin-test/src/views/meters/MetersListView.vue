<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { api } from '@/services/api'
import type { Meter, MeterReading, MeterTariff, MeterType } from '@/types/meter'
import MeterCreateModal from './MeterCreateModal.vue'
import MeterReadingModal from './MeterReadingModal.vue'

// Tab state
const activeTab = ref<'meters' | 'readings' | 'tariffs'>('meters')

// Loading & error
const loading = ref(false)
const error = ref('')

// Data
const meters = ref<Meter[]>([])
const meterTypes = ref<MeterType[]>([])
const readings = ref<MeterReading[]>([])
const tariffs = ref<MeterTariff[]>([])
const properties = ref<{ id: string; number?: string; address?: string }[]>([])

// Property selector (required for meters & readings)
const selectedPropertyId = ref('')

// Filters
const selectedMeterId = ref('')

// Modals
const showMeterModal = ref(false)
const editingMeter = ref<Meter | null>(null)
const showReadingModal = ref(false)

// Season labels
const seasonLabels: Record<number, string> = {
  0: 'All Year',
  1: 'Summer',
  2: 'Winter'
}

// Get meter type icon by meterTypeId or meterName
function getMeterTypeIcon(meterTypeId?: string, meterName?: string): string | undefined {
  if (meterTypeId) {
    const mt = meterTypes.value.find(t => t.id === meterTypeId)
    if (mt?.icon) return mt.icon
  }
  if (meterName) {
    const mt = meterTypes.value.find(t => t.name === meterName)
    if (mt?.icon) return mt.icon
  }
  return undefined
}

// Format number
function formatNumber(val?: number): string {
  if (val === null || val === undefined) return '—'
  return new Intl.NumberFormat('ru-RU', { minimumFractionDigits: 0, maximumFractionDigits: 2 }).format(val)
}

// Format price in UZS
function formatPrice(val?: number): string {
  if (val === null || val === undefined) return '—'
  return new Intl.NumberFormat('ru-RU', {
    style: 'currency',
    currency: 'UZS',
    maximumFractionDigits: 2
  }).format(val)
}

// Load functions
async function loadMeterTypes() {
  try {
    meterTypes.value = await api.getMeterTypes()
  } catch (e) {
    console.error('Failed to load meter types:', e)
  }
}

async function loadProperties() {
  try {
    const res = await api.getMyRealEstates({ page: 1, pageSize: 100 })
    const items = (res as { items?: { id: string; number?: string; address?: string; region?: string }[] }).items || []
    properties.value = items.map((p) => ({
      id: p.id,
      number: p.number,
      address: p.address || p.region || ''
    }))
    // Auto-select first property
    if (properties.value.length > 0 && !selectedPropertyId.value) {
      selectedPropertyId.value = properties.value[0]?.id ?? ''
    }
  } catch (e) {
    console.error('Failed to load properties:', e)
  }
}

async function loadMeters() {
  if (!selectedPropertyId.value) {
    meters.value = []
    return
  }
  loading.value = true
  error.value = ''
  try {
    meters.value = await api.getMeters(selectedPropertyId.value)
  } catch (e: unknown) {
    error.value = (e as Error).message || 'Failed to load meters'
  } finally {
    loading.value = false
  }
}

async function loadReadings() {
  if (!selectedPropertyId.value) {
    readings.value = []
    return
  }
  loading.value = true
  error.value = ''
  try {
    const params: { realEstateId: string; meterId?: string; page?: number; pageSize?: number } = {
      realEstateId: selectedPropertyId.value,
      page: 1,
      pageSize: 50
    }
    if (selectedMeterId.value) params.meterId = selectedMeterId.value
    const res = await api.getMeterReadings(params)
    readings.value = res.items || []
  } catch (e: unknown) {
    error.value = (e as Error).message || 'Failed to load readings'
  } finally {
    loading.value = false
  }
}

async function loadTariffs() {
  loading.value = true
  error.value = ''
  try {
    const res = await api.getMeterTariffs({ page: 1, pageSize: 50 })
    tariffs.value = res.items || []
  } catch (e: unknown) {
    error.value = (e as Error).message || 'Failed to load tariffs'
  } finally {
    loading.value = false
  }
}

// Load data based on active tab
function loadTabData() {
  if (activeTab.value === 'meters') loadMeters()
  else if (activeTab.value === 'readings') loadReadings()
  else if (activeTab.value === 'tariffs') loadTariffs()
}

// Watch tab changes
watch(activeTab, () => loadTabData())

// Watch property selection — reload meters & readings
watch(selectedPropertyId, () => {
  if (activeTab.value === 'meters' || activeTab.value === 'readings') {
    loadTabData()
  }
})

// Watch meter filter for readings
watch(selectedMeterId, () => {
  if (activeTab.value === 'readings') loadReadings()
})

// Meter CRUD
function openCreateMeter() {
  editingMeter.value = null
  showMeterModal.value = true
}

async function openEditMeter(meter: Meter) {
  try {
    const detail = await api.getMeterById(meter.id)
    editingMeter.value = { ...detail, id: meter.id }
    showMeterModal.value = true
  } catch (e) {
    console.error('Failed to load meter details:', e)
    editingMeter.value = meter
    showMeterModal.value = true
  }
}

async function deactivateMeter(meter: Meter) {
  if (!confirm(`Deactivate meter "${meter.meterName} — ${meter.serialNumber}"?`)) return
  try {
    await api.deleteMeter(meter.id)
    loadMeters()
  } catch (e) {
    console.error('Failed to deactivate meter:', e)
    alert('Failed to deactivate meter')
  }
}

function handleMeterSaved() {
  showMeterModal.value = false
  loadMeters()
}

// Reading
function openSubmitReading() {
  showReadingModal.value = true
}

function handleReadingSaved() {
  showReadingModal.value = false
  loadReadings()
}

// Init
onMounted(async () => {
  await Promise.all([loadMeterTypes(), loadProperties()])
  loadTabData()
})
</script>

<template>
  <div class="meters-view">
    <div class="page-header">
      <div class="header-content">
        <h1>Communal Services</h1>
        <p class="subtitle">Meters, readings & tariffs</p>
      </div>
      <button v-if="activeTab === 'meters'" class="btn btn-primary" @click="openCreateMeter">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M12 5v14"></path><path d="M5 12h14"></path>
        </svg>
        Add Meter
      </button>
      <button v-if="activeTab === 'readings'" class="btn btn-primary" @click="openSubmitReading">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M12 5v14"></path><path d="M5 12h14"></path>
        </svg>
        Submit Reading
      </button>
    </div>

    <!-- Property Selector (for meters & readings) -->
    <div v-if="activeTab !== 'tariffs'" class="property-selector">
      <div class="selector-label">
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
          <path d="M3 9l9-7 9 7v11a2 2 0 01-2 2H5a2 2 0 01-2-2z"/><polyline points="9 22 9 12 15 12 15 22"/>
        </svg>
        Property
      </div>
      <select v-model="selectedPropertyId">
        <option value="" disabled>Select property...</option>
        <option v-for="p in properties" :key="p.id" :value="p.id">
          {{ p.number || p.address || p.id.substring(0, 8) + '...' }}
        </option>
      </select>
    </div>

    <!-- Tabs -->
    <div class="tabs-section">
      <button
        class="tab-btn"
        :class="{ active: activeTab === 'meters' }"
        @click="activeTab = 'meters'"
      >
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
          <circle cx="12" cy="12" r="10"/><path d="M12 6v6l4 2"/>
        </svg>
        My Meters
      </button>
      <button
        class="tab-btn"
        :class="{ active: activeTab === 'readings' }"
        @click="activeTab = 'readings'"
      >
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
          <path d="M3 3v18h18"/><path d="M7 16l4-7 4 4 5-9"/>
        </svg>
        Readings
      </button>
      <button
        class="tab-btn"
        :class="{ active: activeTab === 'tariffs' }"
        @click="activeTab = 'tariffs'"
      >
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
          <rect x="2" y="5" width="20" height="14" rx="2"/><path d="M2 10h20"/>
        </svg>
        Tariffs
      </button>
    </div>

    <!-- No property selected state -->
    <div v-if="activeTab !== 'tariffs' && !selectedPropertyId" class="empty-state">
      <div class="empty-icon-svg">
        <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
          <path d="M3 9l9-7 9 7v11a2 2 0 01-2 2H5a2 2 0 01-2-2z"/><polyline points="9 22 9 12 15 12 15 22"/>
        </svg>
      </div>
      <h3>Select a Property</h3>
      <p>Choose a property above to view its meters and readings</p>
    </div>

    <!-- Filter for readings -->
    <div v-else-if="activeTab === 'readings'" class="filters-section">
      <div class="filter-group">
        <label>Filter by Meter</label>
        <select v-model="selectedMeterId">
          <option value="">All Meters</option>
          <option v-for="m in meters" :key="m.id" :value="m.id">
            {{ m.meterName }} — {{ m.serialNumber }}
          </option>
        </select>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Loading...</p>
    </div>

    <!-- Error -->
    <div v-else-if="error" class="error-state">
      <p>{{ error }}</p>
      <button class="btn btn-secondary" @click="loadTabData()">Retry</button>
    </div>

    <!-- ==================== METERS TAB ==================== -->
    <template v-else-if="activeTab === 'meters' && selectedPropertyId">
      <div v-if="meters.length === 0" class="empty-state">
        <div class="empty-icon-svg">
          <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
            <polygon points="13 2 3 14 12 14 11 22 21 10 12 10 13 2"/>
          </svg>
        </div>
        <h3>No Meters Yet</h3>
        <p>Add your first utility meter to start tracking consumption</p>
        <button class="btn btn-primary" @click="openCreateMeter">Add Meter</button>
      </div>

      <div v-else class="data-table-wrap">
        <table class="data-table">
          <thead>
            <tr>
              <th>Meter Type</th>
              <th>Serial Number</th>
              <th>Initial Reading</th>
              <th>Installation Date</th>
              <th>Verification Date</th>
              <th>Next Verification</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="meter in meters" :key="meter.id">
              <td>
                <div class="meter-type-cell">
                  <span class="meter-type-badge" :class="'type-' + meter.meterName.toLowerCase().replace(/\s/g, '')">
                    <img v-if="getMeterTypeIcon(meter.meterTypeId, meter.meterName)" :src="getMeterTypeIcon(meter.meterTypeId, meter.meterName)" class="badge-icon" alt="" />
                    {{ meter.meterName }}
                  </span>
                </div>
              </td>
              <td class="text-secondary">{{ meter.serialNumber || '—' }}</td>
              <td class="text-mono">{{ formatNumber(meter.initialReading) }}</td>
              <td class="text-secondary">{{ meter.installationDate || '—' }}</td>
              <td class="text-secondary">{{ meter.verificationDate || '—' }}</td>
              <td class="text-secondary">{{ meter.nextVerificationDate || '—' }}</td>
              <td>
                <div class="action-btns">
                  <button class="action-btn edit-btn" title="Edit" @click="openEditMeter(meter)">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                      <path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7"/>
                      <path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z"/>
                    </svg>
                  </button>
                  <button class="action-btn deactivate-btn" title="Deactivate" @click="deactivateMeter(meter)">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                      <path d="M18.36 6.64a9 9 0 11-12.73 0"/>
                      <line x1="12" y1="2" x2="12" y2="12"/>
                    </svg>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </template>

    <!-- ==================== READINGS TAB ==================== -->
    <template v-else-if="activeTab === 'readings' && selectedPropertyId">
      <div v-if="readings.length === 0" class="empty-state">
        <div class="empty-icon-svg">
          <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
            <path d="M3 3v18h18"/><path d="M7 16l4-7 4 4 5-9"/>
          </svg>
        </div>
        <h3>No Readings</h3>
        <p>Submit your first meter reading</p>
        <button class="btn btn-primary" @click="openSubmitReading">Submit Reading</button>
      </div>

      <div v-else class="data-table-wrap">
        <table class="data-table">
          <thead>
            <tr>
              <th>Date</th>
              <th>Meter</th>
              <th>Serial Number</th>
              <th>Previous</th>
              <th>Current</th>
              <th>Consumption</th>
              <th>Manual</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(reading, index) in readings" :key="index">
              <td class="text-secondary">{{ reading.readingDate }}</td>
              <td>
                <span class="meter-type-badge small">
                  <img v-if="getMeterTypeIcon(undefined, reading.meterName)" :src="getMeterTypeIcon(undefined, reading.meterName)" class="badge-icon small" alt="" />
                  {{ reading.meterName }}
                </span>
              </td>
              <td class="text-secondary">{{ reading.serialNumber || '—' }}</td>
              <td class="text-mono">{{ formatNumber(reading.previousValue) }}</td>
              <td class="text-mono text-bold">{{ formatNumber(reading.value) }}</td>
              <td>
                <span class="consumption-badge">{{ formatNumber(reading.consumption) }}</span>
              </td>
              <td class="text-secondary text-sm">{{ reading.isManual ? 'Yes' : 'Auto' }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </template>

    <!-- ==================== TARIFFS TAB ==================== -->
    <template v-else-if="activeTab === 'tariffs'">
      <div v-if="tariffs.length === 0" class="empty-state">
        <div class="empty-icon-svg">
          <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
            <rect x="2" y="5" width="20" height="14" rx="2"/><path d="M2 10h20"/>
          </svg>
        </div>
        <h3>No Tariffs</h3>
        <p>Tariffs are managed by administration</p>
      </div>

      <div v-else class="tariffs-grid">
        <div v-for="tariff in tariffs" :key="tariff.id" class="tariff-card">
          <div class="tariff-header">
            <span class="meter-type-badge">
              <img v-if="getMeterTypeIcon(tariff.meterTypeId)" :src="getMeterTypeIcon(tariff.meterTypeId)" class="badge-icon" alt="" />
              {{ tariff.meterType }}
            </span>
            <span class="tariff-status" :class="{ active: tariff.isActual }">
              {{ tariff.isActual ? 'Active' : 'Inactive' }}
            </span>
          </div>
          <div class="tariff-price">
            {{ formatPrice(tariff.price) }}
            <span class="price-unit">/ unit</span>
          </div>
          <div class="tariff-details">
            <div class="tariff-detail" v-if="tariff.season !== undefined">
              <span class="detail-label">Season</span>
              <span class="detail-value">{{ seasonLabels[tariff.season] || 'All Year' }}</span>
            </div>
            <div class="tariff-detail" v-if="tariff.minLimit !== null && tariff.minLimit !== undefined">
              <span class="detail-label">Min Limit</span>
              <span class="detail-value">{{ formatNumber(tariff.minLimit) }}</span>
            </div>
            <div class="tariff-detail" v-if="tariff.maxLimit !== null && tariff.maxLimit !== undefined">
              <span class="detail-label">Max Limit</span>
              <span class="detail-value">{{ formatNumber(tariff.maxLimit) }}</span>
            </div>
            <div class="tariff-detail" v-if="tariff.socialNormLimit !== null && tariff.socialNormLimit !== undefined">
              <span class="detail-label">Social Norm</span>
              <span class="detail-value">{{ formatNumber(tariff.socialNormLimit) }}</span>
            </div>
            <div class="tariff-detail" v-if="tariff.fixedPrice !== null && tariff.fixedPrice !== undefined">
              <span class="detail-label">Fixed Price</span>
              <span class="detail-value">{{ formatPrice(tariff.fixedPrice) }}</span>
            </div>
          </div>
        </div>
      </div>
    </template>

    <!-- Modals -->
    <MeterCreateModal
      v-if="showMeterModal"
      :meter="editingMeter"
      :meter-types="meterTypes"
      :properties="properties"
      :selected-property-id="selectedPropertyId"
      @close="showMeterModal = false"
      @saved="handleMeterSaved"
    />

    <MeterReadingModal
      v-if="showReadingModal"
      :meters="meters"
      :meter-types="meterTypes"
      :properties="properties"
      :selected-property-id="selectedPropertyId"
      @close="showReadingModal = false"
      @saved="handleReadingSaved"
    />
  </div>
</template>

<style scoped>
.meters-view {
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

/* Property Selector */
.property-selector {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 20px;
  padding: 12px 16px;
  background: #FFFFFF;
  border-radius: 14px;
  border: 0.5px solid rgba(27, 27, 27, 0.06);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
}

.selector-label {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 13px;
  font-weight: 600;
  color: rgba(27, 27, 27, 0.5);
  white-space: nowrap;
}

.property-selector select {
  flex: 1;
  padding: 10px 14px;
  border: 1px solid rgba(27, 27, 27, 0.08);
  border-radius: 10px;
  font-size: 14px;
  background: #F7F7F7;
  color: #1B1B1B;
  max-width: 400px;
}

.property-selector select:focus {
  outline: none;
  border-color: #FF5B3C;
  box-shadow: 0 0 0 3px rgba(255, 91, 60, 0.1);
}

/* Buttons */
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

/* Tabs */
.tabs-section {
  display: flex;
  gap: 4px;
  margin-bottom: 24px;
  background: #F7F7F7;
  padding: 4px;
  border-radius: 12px;
  width: fit-content;
}

.tab-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 20px;
  border: none;
  border-radius: 10px;
  background: transparent;
  color: rgba(27, 27, 27, 0.5);
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.tab-btn:hover {
  color: #1B1B1B;
  background: rgba(255, 255, 255, 0.5);
}

.tab-btn.active {
  background: #FFFFFF;
  color: #1B1B1B;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);
  font-weight: 600;
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

.filter-group select {
  padding: 10px 14px;
  border: 1px solid rgba(27, 27, 27, 0.08);
  border-radius: 8px;
  font-size: 14px;
  min-width: 280px;
  background: #F7F7F7;
  color: #1B1B1B;
}

.filter-group select:focus {
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
  font-size: 48px;
  margin-bottom: 12px;
}

.empty-icon-svg {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 80px;
  height: 80px;
  margin: 0 auto 16px;
  border-radius: 50%;
  background: rgba(255, 91, 60, 0.08);
  color: #FF5B3C;
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

/* Data Table */
.data-table-wrap {
  background: #FFFFFF;
  border-radius: 20px;
  border: 0.5px solid rgba(27, 27, 27, 0.04);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
  overflow: hidden;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
}

.data-table th {
  text-align: left;
  padding: 14px 20px;
  font-size: 12px;
  font-weight: 600;
  color: rgba(27, 27, 27, 0.4);
  text-transform: uppercase;
  letter-spacing: 0.5px;
  border-bottom: 1px solid rgba(27, 27, 27, 0.06);
  background: #FAFAFA;
}

.data-table td {
  padding: 16px 20px;
  font-size: 14px;
  color: #1B1B1B;
  border-bottom: 1px solid rgba(27, 27, 27, 0.04);
}

.data-table tbody tr {
  transition: background 0.15s;
}

.data-table tbody tr:hover {
  background: rgba(255, 91, 60, 0.02);
}

.data-table tbody tr:last-child td {
  border-bottom: none;
}

.action-btns {
  display: flex;
  gap: 6px;
}

.action-btn {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  border: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
  background: transparent;
  color: #1B1B1B;
}

.action-btn:hover {
  background: #F7F7F7;
}

.edit-btn:hover {
  background: rgba(59, 130, 246, 0.1);
  color: #3B82F6;
}

.deactivate-btn:hover {
  background: rgba(239, 68, 68, 0.1);
  color: #DC2626;
}

.text-secondary {
  color: rgba(27, 27, 27, 0.5);
}

.text-mono {
  font-family: 'SF Mono', 'Fira Code', monospace;
  font-size: 13px;
}

.text-bold {
  font-weight: 600;
  color: #1B1B1B;
}

.text-sm {
  font-size: 12px;
}

/* Badges */
.meter-type-badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 5px 12px;
  border-radius: 100px;
  font-size: 12px;
  font-weight: 600;
  background: rgba(255, 91, 60, 0.1);
  color: #FF5B3C;
}

.meter-type-badge.small {
  gap: 4px;
  padding: 3px 8px;
  font-size: 11px;
}

.badge-icon {
  width: 16px;
  height: 16px;
  flex-shrink: 0;
  filter: brightness(0) saturate(100%) invert(42%) sepia(93%) saturate(1232%) hue-rotate(343deg) brightness(101%) contrast(101%);
}

.badge-icon.small {
  width: 13px;
  height: 13px;
}

.consumption-badge {
  display: inline-flex;
  align-items: center;
  padding: 4px 10px;
  border-radius: 8px;
  font-size: 13px;
  font-weight: 600;
  background: rgba(16, 185, 129, 0.1);
  color: #059669;
  font-family: 'SF Mono', 'Fira Code', monospace;
}

/* Tariffs Grid */
.tariffs-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 16px;
}

.tariff-card {
  background: #FFFFFF;
  border-radius: 20px;
  padding: 24px;
  border: 0.5px solid rgba(27, 27, 27, 0.04);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
  transition: all 0.2s;
}

.tariff-card:hover {
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.08);
  transform: translateY(-2px);
}

.tariff-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.tariff-status {
  font-size: 11px;
  font-weight: 600;
  padding: 4px 10px;
  border-radius: 100px;
  background: rgba(27, 27, 27, 0.06);
  color: rgba(27, 27, 27, 0.4);
}

.tariff-status.active {
  background: rgba(16, 185, 129, 0.1);
  color: #059669;
}

.tariff-price {
  font-size: 28px;
  font-weight: 700;
  color: #1B1B1B;
  margin-bottom: 20px;
  line-height: 1;
}

.price-unit {
  font-size: 14px;
  font-weight: 400;
  color: rgba(27, 27, 27, 0.4);
}

.tariff-details {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.tariff-detail {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.detail-label {
  font-size: 13px;
  color: rgba(27, 27, 27, 0.5);
}

.detail-value {
  font-size: 13px;
  font-weight: 600;
  color: #1B1B1B;
}

/* Responsive */
@media (max-width: 768px) {
  .tabs-section {
    width: 100%;
  }

  .tab-btn {
    flex: 1;
    justify-content: center;
    padding: 10px 12px;
    font-size: 13px;
  }

  .property-selector {
    flex-direction: column;
    align-items: stretch;
  }

  .property-selector select {
    max-width: unset;
  }

  .tariffs-grid {
    grid-template-columns: 1fr;
  }
}
</style>
