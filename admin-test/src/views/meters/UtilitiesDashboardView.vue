<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '@/services/api'
import type { Meter, MeterReading, MeterTariff, MeterType } from '@/types/meter'

const router = useRouter()

// Data
const properties = ref<{ id: string; number?: string; address?: string; region?: string }[]>([])
const meterTypes = ref<MeterType[]>([])
const metersByProperty = ref<Record<string, Meter[]>>({})
const recentReadings = ref<MeterReading[]>([])
const tariffs = ref<MeterTariff[]>([])

// Loading
const loading = ref(true)
const loadingProgress = ref(0)

// Computed stats
const totalProperties = computed(() => properties.value.length)
const totalMeters = computed(() => Object.values(metersByProperty.value).flat().length)
const propertiesWithMeters = computed(() =>
  Object.entries(metersByProperty.value).filter(([, meters]) => meters.length > 0).length
)
const propertiesWithoutMeters = computed(() => totalProperties.value - propertiesWithMeters.value)
const activeTariffs = computed(() => tariffs.value.filter(t => t.isActual).length)
const totalReadingsCount = computed(() => recentReadings.value.length)

// Property cards with meter data
const propertyCards = computed(() => {
  return properties.value.map(p => {
    const meters = metersByProperty.value[p.id] || []
    const meterTypeNames = meters.map(m => m.meterName).filter((v, i, a) => a.indexOf(v) === i)
    return {
      ...p,
      meters,
      meterCount: meters.length,
      meterTypeNames,
      hasMeters: meters.length > 0
    }
  })
})

// Get meter type icon
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

// Navigate to communal with property context
function goToMeters() {
  router.push({ name: 'communal-meters' })
}

function goToProperty(propertyId: string) {
  router.push({ name: 'communal-meters', query: { propertyId } })
}

// Load all data
async function loadDashboard() {
  loading.value = true
  loadingProgress.value = 0

  try {
    // Step 1: Load properties and meter types
    const [propsRes, typesRes] = await Promise.all([
      api.getMyRealEstates({ page: 1, pageSize: 100 }),
      api.getMeterTypes()
    ])

    const items = (propsRes as { items?: { id: string; number?: string; address?: string; region?: string }[] }).items || []
    properties.value = items
    meterTypes.value = typesRes
    loadingProgress.value = 30

    // Step 2: Load meters for each property (batch)
    const meterResults: Record<string, Meter[]> = {}
    const batchSize = 5
    for (let i = 0; i < items.length; i += batchSize) {
      const batch = items.slice(i, i + batchSize)
      const results = await Promise.all(
        batch.map(p => api.getMeters(p.id).catch(() => [] as Meter[]))
      )
      batch.forEach((p, idx) => {
        meterResults[p.id] = results[idx] || []
      })
      loadingProgress.value = 30 + Math.round((i / items.length) * 40)
    }
    metersByProperty.value = meterResults
    loadingProgress.value = 70

    // Step 3: Load recent readings for the first property that has meters
    const firstWithMeters = items.find(p => (meterResults[p.id] || []).length > 0)
    if (firstWithMeters) {
      try {
        const readingsRes = await api.getMeterReadings({
          realEstateId: firstWithMeters.id,
          page: 1,
          pageSize: 10
        })
        recentReadings.value = readingsRes.items || []
      } catch {
        recentReadings.value = []
      }
    }
    loadingProgress.value = 85

    // Step 4: Load tariffs
    try {
      const tariffsRes = await api.getMeterTariffs({ page: 1, pageSize: 50 })
      tariffs.value = tariffsRes.items || []
    } catch {
      tariffs.value = []
    }
    loadingProgress.value = 100
  } catch (e) {
    console.error('Failed to load dashboard:', e)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadDashboard()
})
</script>

<template>
  <div class="dashboard">
    <!-- Header -->
    <div class="dash-header">
      <div>
        <h1>Communal Services</h1>
        <p class="subtitle">Overview of meters, readings & tariffs across all your properties</p>
      </div>
      <button class="btn btn-primary" @click="goToMeters">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M13 2L3 14h9l-1 8 10-12h-9l1-8z" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
        Manage Meters
      </button>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="loading-screen">
      <div class="loading-card">
        <div class="spinner"></div>
        <p class="loading-text">Loading dashboard...</p>
        <div class="progress-bar">
          <div class="progress-fill" :style="{ width: loadingProgress + '%' }"></div>
        </div>
      </div>
    </div>

    <!-- Dashboard Content -->
    <template v-else>
      <!-- Stats Cards -->
      <div class="stats-grid">
        <div class="stat-card">
          <div class="stat-icon properties-icon">
            <svg width="22" height="22" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M21 10.978V19a2 2 0 01-2 2H5a2 2 0 01-2-2v-8.022a2 2 0 01.772-1.579l7-5.444a2 2 0 012.456 0l7 5.444A2 2 0 0121 10.978z" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
          </div>
          <div class="stat-info">
            <span class="stat-value">{{ totalProperties }}</span>
            <span class="stat-label">Properties</span>
          </div>
        </div>

        <div class="stat-card">
          <div class="stat-icon meters-icon">
            <svg width="22" height="22" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M13 2L3 14h9l-1 8 10-12h-9l1-8z" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
          </div>
          <div class="stat-info">
            <span class="stat-value">{{ totalMeters }}</span>
            <span class="stat-label">Active Meters</span>
          </div>
        </div>

        <div class="stat-card">
          <div class="stat-icon readings-icon">
            <svg width="22" height="22" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M3 3v18h18" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
              <path d="M7 16l4-7 4 4 5-9" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
          </div>
          <div class="stat-info">
            <span class="stat-value">{{ totalReadingsCount }}</span>
            <span class="stat-label">Recent Readings</span>
          </div>
        </div>

        <div class="stat-card">
          <div class="stat-icon tariffs-icon">
            <svg width="22" height="22" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M20.59 13.41l-7.17 7.17a2 2 0 01-2.83 0L2 12V2h10l8.59 8.59a2 2 0 010 2.82z" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
              <circle cx="7" cy="7" r="1.5" fill="currentColor"/>
            </svg>
          </div>
          <div class="stat-info">
            <span class="stat-value">{{ activeTariffs }}</span>
            <span class="stat-label">Active Tariffs</span>
          </div>
        </div>
      </div>

      <!-- Action Alerts -->
      <div v-if="propertiesWithoutMeters > 0" class="alert-banner">
        <div class="alert-icon">
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M10.29 3.86L1.82 18a2 2 0 001.71 3h16.94a2 2 0 001.71-3L13.71 3.86a2 2 0 00-3.42 0z" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
            <path d="M12 9v4" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"/>
            <circle cx="12" cy="17" r="0.5" fill="currentColor"/>
          </svg>
        </div>
        <div class="alert-content">
          <strong>{{ propertiesWithoutMeters }} {{ propertiesWithoutMeters === 1 ? 'property' : 'properties' }} without meters</strong>
          <span>Add meters to start tracking utility consumption</span>
        </div>
        <button class="alert-action" @click="goToMeters">Add Meters →</button>
      </div>

      <!-- Properties Grid -->
      <div class="section">
        <div class="section-header">
          <h2>Properties</h2>
          <span class="badge">{{ totalProperties }}</span>
        </div>

        <div v-if="propertyCards.length === 0" class="empty-state">
          <svg width="48" height="48" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M21 10.978V19a2 2 0 01-2 2H5a2 2 0 01-2-2v-8.022a2 2 0 01.772-1.579l7-5.444a2 2 0 012.456 0l7 5.444A2 2 0 0121 10.978z" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
          <h3>No Properties</h3>
          <p>Add a real estate property to get started</p>
        </div>

        <div v-else class="property-grid">
          <div
            v-for="prop in propertyCards"
            :key="prop.id"
            class="property-card"
            :class="{ 'no-meters': !prop.hasMeters }"
            @click="goToProperty(prop.id)"
          >
            <div class="property-header">
              <div class="property-icon">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                  <path d="M21 10.978V19a2 2 0 01-2 2H5a2 2 0 01-2-2v-8.022a2 2 0 01.772-1.579l7-5.444a2 2 0 012.456 0l7 5.444A2 2 0 0121 10.978z" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
              </div>
              <div class="property-title">
                <h3>{{ prop.number || prop.address || prop.id.substring(0, 8) + '...' }}</h3>
                <span v-if="prop.address && prop.number" class="property-address">{{ prop.address }}</span>
              </div>
            </div>

            <div class="property-meters">
              <template v-if="prop.hasMeters">
                <div class="meter-chips">
                  <span
                    v-for="name in prop.meterTypeNames"
                    :key="name"
                    class="meter-chip"
                    :class="'chip-' + name.toLowerCase().replace(/\s/g, '')"
                  >
                    <img
                      v-if="getMeterTypeIcon(undefined, name)"
                      :src="getMeterTypeIcon(undefined, name)"
                      class="chip-icon"
                      alt=""
                    />
                    {{ name }}
                  </span>
                </div>
                <div class="meter-count">
                  <span class="count-num">{{ prop.meterCount }}</span>
                  <span class="count-label">{{ prop.meterCount === 1 ? 'meter' : 'meters' }}</span>
                </div>
              </template>
              <template v-else>
                <div class="no-meters-msg">
                  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="1.5"/>
                    <path d="M12 8v4" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"/>
                    <circle cx="12" cy="16" r="0.5" fill="currentColor"/>
                  </svg>
                  No meters installed
                </div>
              </template>
            </div>

            <div class="property-footer">
              <span class="view-link">View details →</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Readings -->
      <div v-if="recentReadings.length > 0" class="section">
        <div class="section-header">
          <h2>Recent Readings</h2>
          <button class="link-btn" @click="goToMeters">View All</button>
        </div>

        <div class="readings-table-wrap">
          <table class="readings-table">
            <thead>
              <tr>
                <th>Date</th>
                <th>Meter</th>
                <th>Previous</th>
                <th>Current</th>
                <th>Consumption</th>
                <th>Source</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(reading, idx) in recentReadings.slice(0, 8)" :key="idx">
                <td class="text-secondary">{{ reading.readingDate }}</td>
                <td>
                  <div class="reading-meter">
                    <img
                      v-if="getMeterTypeIcon(undefined, reading.meterName)"
                      :src="getMeterTypeIcon(undefined, reading.meterName)"
                      class="reading-icon"
                      alt=""
                    />
                    {{ reading.meterName }}
                  </div>
                </td>
                <td class="text-mono">{{ formatNumber(reading.previousValue) }}</td>
                <td class="text-mono">{{ formatNumber(reading.value) }}</td>
                <td class="text-mono consumption-cell">{{ formatNumber(reading.consumption) }}</td>
                <td>
                  <span class="source-badge" :class="reading.isManual ? 'manual' : 'auto'">
                    {{ reading.isManual ? 'Manual' : 'Auto' }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Meter Types Overview -->
      <div v-if="meterTypes.length > 0" class="section">
        <div class="section-header">
          <h2>Meter Types</h2>
          <span class="badge">{{ meterTypes.length }}</span>
        </div>

        <div class="meter-types-grid">
          <div v-for="mt in meterTypes" :key="mt.id" class="meter-type-card">
            <img v-if="mt.icon" :src="mt.icon" class="mt-icon" alt="" />
            <div class="mt-info">
              <span class="mt-name">{{ mt.name }}</span>
              <span v-if="mt.unit" class="mt-unit">{{ mt.unit }}</span>
            </div>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>

<style scoped>
.dashboard {
  max-width: 1200px;
  margin: 0 auto;
  padding: 32px 24px;
}

/* Header */
.dash-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 32px;
}

.dash-header h1 {
  font-size: 28px;
  font-weight: 800;
  color: #1B1B1B;
  letter-spacing: -0.5px;
  margin: 0;
}

.subtitle {
  color: rgba(27, 27, 27, 0.5);
  font-size: 14px;
  margin-top: 4px;
}

.btn {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 10px 20px;
  border-radius: 10px;
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
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

/* Loading */
.loading-screen {
  display: flex;
  justify-content: center;
  padding: 80px 0;
}

.loading-card {
  text-align: center;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 3px solid rgba(27, 27, 27, 0.08);
  border-top-color: #1B1B1B;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
  margin: 0 auto 16px;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.loading-text {
  color: rgba(27, 27, 27, 0.5);
  font-size: 14px;
  margin-bottom: 16px;
}

.progress-bar {
  width: 200px;
  height: 3px;
  background: rgba(27, 27, 27, 0.06);
  border-radius: 2px;
  overflow: hidden;
  margin: 0 auto;
}

.progress-fill {
  height: 100%;
  background: #1B1B1B;
  border-radius: 2px;
  transition: width 0.3s ease;
}

/* Stats Grid */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
  margin-bottom: 24px;
}

.stat-card {
  background: #fff;
  border: 1px solid rgba(27, 27, 27, 0.06);
  border-radius: 16px;
  padding: 20px;
  display: flex;
  align-items: center;
  gap: 16px;
  transition: all 0.2s;
}

.stat-card:hover {
  border-color: rgba(27, 27, 27, 0.1);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.04);
}

.stat-icon {
  width: 48px;
  height: 48px;
  border-radius: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.properties-icon,
.meters-icon,
.readings-icon,
.tariffs-icon {
  background: rgba(27, 27, 27, 0.06);
  color: #1B1B1B;
}

.stat-info {
  display: flex;
  flex-direction: column;
}

.stat-value {
  font-size: 28px;
  font-weight: 800;
  color: #1B1B1B;
  line-height: 1;
  letter-spacing: -0.5px;
}

.stat-label {
  font-size: 13px;
  color: rgba(27, 27, 27, 0.45);
  margin-top: 4px;
  font-weight: 500;
}

/* Alert Banner */
.alert-banner {
  display: flex;
  align-items: center;
  gap: 14px;
  padding: 14px 20px;
  background: rgba(245, 158, 11, 0.06);
  border: 1px solid rgba(245, 158, 11, 0.15);
  border-radius: 14px;
  margin-bottom: 24px;
}

.alert-icon {
  width: 36px;
  height: 36px;
  border-radius: 10px;
  background: rgba(245, 158, 11, 0.12);
  color: #D97706;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.alert-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.alert-content strong {
  font-size: 14px;
  color: #92400E;
}

.alert-content span {
  font-size: 12px;
  color: #B45309;
}

.alert-action {
  background: none;
  border: none;
  color: #D97706;
  font-weight: 600;
  font-size: 13px;
  cursor: pointer;
  white-space: nowrap;
  padding: 6px 12px;
  border-radius: 8px;
  transition: all 0.2s;
}

.alert-action:hover {
  background: rgba(245, 158, 11, 0.1);
}

/* Sections */
.section {
  margin-bottom: 32px;
}

.section-header {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 16px;
}

.section-header h2 {
  font-size: 18px;
  font-weight: 700;
  color: #1B1B1B;
  margin: 0;
}

.badge {
  background: rgba(27, 27, 27, 0.06);
  color: rgba(27, 27, 27, 0.5);
  font-size: 12px;
  font-weight: 600;
  padding: 2px 10px;
  border-radius: 20px;
}

.link-btn {
  margin-left: auto;
  background: none;
  border: none;
  color: rgba(27, 27, 27, 0.4);
  font-size: 13px;
  font-weight: 600;
  cursor: pointer;
  transition: color 0.2s;
}

.link-btn:hover {
  color: #1B1B1B;
}

/* Property Grid */
.property-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 16px;
}

.property-card {
  background: #fff;
  border: 1px solid rgba(27, 27, 27, 0.06);
  border-radius: 16px;
  padding: 20px;
  cursor: pointer;
  transition: all 0.25s;
}

.property-card:hover {
  border-color: rgba(27, 27, 27, 0.12);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.06);
  transform: translateY(-2px);
}

.property-card.no-meters {
  border-style: dashed;
  border-color: rgba(27, 27, 27, 0.1);
}

.property-card.no-meters:hover {
  border-color: rgba(245, 158, 11, 0.3);
}

.property-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 16px;
}

.property-icon {
  width: 40px;
  height: 40px;
  border-radius: 12px;
  background: rgba(27, 27, 27, 0.04);
  display: flex;
  align-items: center;
  justify-content: center;
  color: rgba(27, 27, 27, 0.4);
  flex-shrink: 0;
}

.property-title h3 {
  font-size: 15px;
  font-weight: 700;
  color: #1B1B1B;
  margin: 0;
}

.property-address {
  font-size: 12px;
  color: rgba(27, 27, 27, 0.4);
}

.property-meters {
  margin-bottom: 14px;
  min-height: 36px;
}

.meter-chips {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
  margin-bottom: 10px;
}

.meter-chip {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 4px 10px;
  border-radius: 8px;
  font-size: 11px;
  font-weight: 600;
  background: rgba(27, 27, 27, 0.04);
  color: rgba(27, 27, 27, 0.7);
}

.chip-icon {
  width: 14px;
  height: 14px;
}

.meter-count {
  display: flex;
  align-items: baseline;
  gap: 4px;
}

.count-num {
  font-size: 20px;
  font-weight: 800;
  color: #1B1B1B;
}

.count-label {
  font-size: 12px;
  color: rgba(27, 27, 27, 0.4);
  font-weight: 500;
}

.no-meters-msg {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 13px;
  color: rgba(27, 27, 27, 0.35);
}

.property-footer {
  border-top: 1px solid rgba(27, 27, 27, 0.04);
  padding-top: 12px;
}

.view-link {
  font-size: 13px;
  font-weight: 600;
  color: rgba(27, 27, 27, 0.35);
  transition: color 0.2s;
}

.property-card:hover .view-link {
  color: #1B1B1B;
}

/* Readings Table */
.readings-table-wrap {
  background: #fff;
  border: 1px solid rgba(27, 27, 27, 0.06);
  border-radius: 16px;
  overflow: hidden;
}

.readings-table {
  width: 100%;
  border-collapse: collapse;
}

.readings-table th {
  padding: 14px 20px;
  text-align: left;
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: rgba(27, 27, 27, 0.4);
  background: rgba(27, 27, 27, 0.02);
  border-bottom: 1px solid rgba(27, 27, 27, 0.06);
}

.readings-table td {
  padding: 14px 20px;
  font-size: 14px;
  color: #1B1B1B;
  border-bottom: 1px solid rgba(27, 27, 27, 0.04);
}

.readings-table tbody tr:last-child td {
  border-bottom: none;
}

.readings-table tbody tr {
  transition: background 0.15s;
}

.readings-table tbody tr:hover {
  background: rgba(27, 27, 27, 0.01);
}

.reading-meter {
  display: flex;
  align-items: center;
  gap: 6px;
  font-weight: 600;
}

.reading-icon {
  width: 18px;
  height: 18px;
}

.text-secondary {
  color: rgba(27, 27, 27, 0.5);
}

.text-mono {
  font-family: 'SF Mono', 'Menlo', monospace;
  font-size: 13px;
}

.consumption-cell {
  font-weight: 600;
  color: #1B1B1B;
}

.source-badge {
  display: inline-flex;
  padding: 3px 10px;
  border-radius: 6px;
  font-size: 11px;
  font-weight: 600;
}

.source-badge.manual,
.source-badge.auto {
  background: rgba(27, 27, 27, 0.06);
  color: #1B1B1B;
}

/* Meter Types Grid */
.meter-types-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 12px;
}

.meter-type-card {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 14px 16px;
  background: #fff;
  border: 1px solid rgba(27, 27, 27, 0.06);
  border-radius: 12px;
  transition: all 0.2s;
}

.meter-type-card:hover {
  border-color: rgba(27, 27, 27, 0.1);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
}

.mt-icon {
  width: 32px;
  height: 32px;
  filter: brightness(0);
}

.mt-info {
  display: flex;
  flex-direction: column;
}

.mt-name {
  font-size: 14px;
  font-weight: 600;
  color: #1B1B1B;
}

.mt-unit {
  font-size: 12px;
  color: rgba(27, 27, 27, 0.4);
}

/* Empty State */
.empty-state {
  text-align: center;
  padding: 60px 20px;
  color: rgba(27, 27, 27, 0.4);
}

.empty-state svg {
  margin-bottom: 12px;
  opacity: 0.3;
}

.empty-state h3 {
  font-size: 16px;
  font-weight: 700;
  color: rgba(27, 27, 27, 0.5);
  margin: 0 0 4px;
}

.empty-state p {
  font-size: 13px;
  color: rgba(27, 27, 27, 0.35);
}

/* Responsive */
@media (max-width: 768px) {
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .dash-header {
    flex-direction: column;
    gap: 16px;
  }

  .property-grid {
    grid-template-columns: 1fr;
  }
}
</style>
