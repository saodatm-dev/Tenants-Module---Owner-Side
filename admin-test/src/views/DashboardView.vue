<script setup lang="ts">
import { ref, onMounted, onActivated } from 'vue'
import { api, type DashboardStatistics } from '@/services/api'

const stats = ref<DashboardStatistics | null>(null)
const loading = ref(true)
const error = ref<string | null>(null)

async function fetchStats() {
  try {
    loading.value = true
    error.value = null
    stats.value = await api.getDashboardStatistics()
  } catch (e) {
    error.value = 'Failed to load statistics'
    console.error('Dashboard stats error:', e)
  } finally {
    loading.value = false
  }
}

onMounted(() => { fetchStats() })
onActivated(() => { fetchStats() })
</script>

<template>
  <div class="dashboard">

    <!-- Loading -->
    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      <p>Loading...</p>
    </div>

    <!-- Error -->
    <div v-else-if="error" class="error-block">
      <p>{{ error }}</p>
      <button class="btn-retry" @click="fetchStats">Try Again</button>
    </div>

    <!-- Content -->
    <div v-else-if="stats" class="content">
      <!-- Primary number -->
      <div class="hero-stat">
        <div class="hero-number">{{ stats.realEstates.total }}</div>
        <div class="hero-label">Total Properties</div>
        <div class="hero-breakdown">
          <span>{{ stats.realEstates.active }} active</span>
          <span class="dot">·</span>
          <span>{{ stats.realEstates.booked }} booked</span>
          <span class="dot">·</span>
          <span>{{ stats.realEstates.rented }} rented</span>
        </div>
      </div>

      <div class="divider"></div>

      <!-- Occupancy -->
      <div class="occupancy-row">
        <div class="occupancy-left">
          <span class="occupancy-value">{{ stats.occupancyRate }}%</span>
          <span class="occupancy-label">Occupancy Rate</span>
        </div>
        <div class="occupancy-bar-track">
          <div class="occupancy-bar-fill" :style="{ width: stats.occupancyRate + '%' }"></div>
        </div>
      </div>

      <div class="divider"></div>

      <!-- Secondary stats -->
      <div class="stats-row">
        <div class="stat-item">
          <span class="stat-num">{{ stats.listings.total }}</span>
          <span class="stat-label">Listings</span>
          <span class="stat-sub" v-if="stats.listings.active > 0">{{ stats.listings.active }} active</span>
        </div>
        <div class="stat-item">
          <span class="stat-num">{{ stats.listingRequests.total }}</span>
          <span class="stat-label">Requests</span>
          <span class="stat-sub" v-if="stats.listingRequests.pending > 0">{{ stats.listingRequests.pending }} pending</span>
          <span class="stat-sub muted" v-else>All processed</span>
        </div>
        <div class="stat-item">
          <span class="stat-num">{{ stats.leasesCount }}</span>
          <span class="stat-label">Active Leases</span>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.dashboard {
  max-width: 720px;
  margin: 0 auto;
  padding: 48px 24px;
  min-height: calc(80vh - 64px);
  display: flex;
  flex-direction: column;
  justify-content: center;
}

/* Header */
.dash-header {
  margin-bottom: 48px;
}

.greeting {
  font-size: 14px;
  color: rgba(27, 27, 27, 0.4);
  font-weight: 500;
}

.dash-header h1 {
  font-size: 32px;
  font-weight: 800;
  color: #1B1B1B;
  margin: 4px 0 0;
  letter-spacing: -0.5px;
}

.subtitle {
  font-size: 14px;
  color: rgba(27, 27, 27, 0.4);
  margin-top: 4px;
}

/* Loading */
.loading {
  text-align: center;
  padding: 80px 0;
  color: rgba(27, 27, 27, 0.4);
}

.spinner {
  width: 32px;
  height: 32px;
  border: 2px solid rgba(27, 27, 27, 0.08);
  border-top-color: #1B1B1B;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
  margin: 0 auto 12px;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

/* Error */
.error-block {
  text-align: center;
  padding: 48px 0;
  color: rgba(27, 27, 27, 0.5);
}

.btn-retry {
  margin-top: 12px;
  padding: 8px 20px;
  background: #1B1B1B;
  color: #fff;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  font-size: 13px;
  cursor: pointer;
  transition: background 0.2s;
}

.btn-retry:hover {
  background: #333;
}

/* Content */
.content {
  display: flex;
  flex-direction: column;
}

/* Hero stat */
.hero-stat {
  padding: 8px 0;
}

.hero-number {
  font-size: 72px;
  font-weight: 800;
  color: #1B1B1B;
  line-height: 1;
  letter-spacing: -2px;
}

.hero-label {
  font-size: 15px;
  font-weight: 600;
  color: rgba(27, 27, 27, 0.5);
  margin-top: 8px;
}

.hero-breakdown {
  display: flex;
  align-items: center;
  gap: 4px;
  margin-top: 12px;
  font-size: 14px;
  color: rgba(27, 27, 27, 0.4);
  font-weight: 500;
}

.hero-breakdown .dot {
  color: rgba(27, 27, 27, 0.15);
  margin: 0 4px;
}

/* Divider */
.divider {
  height: 1px;
  background: rgba(27, 27, 27, 0.06);
  margin: 32px 0;
}

/* Occupancy */
.occupancy-row {
  display: flex;
  align-items: center;
  gap: 32px;
}

.occupancy-left {
  display: flex;
  align-items: baseline;
  gap: 10px;
  flex-shrink: 0;
}

.occupancy-value {
  font-size: 36px;
  font-weight: 800;
  color: #1B1B1B;
  letter-spacing: -1px;
  line-height: 1;
}

.occupancy-label {
  font-size: 14px;
  color: rgba(27, 27, 27, 0.4);
  font-weight: 500;
}

.occupancy-bar-track {
  flex: 1;
  height: 6px;
  background: rgba(27, 27, 27, 0.06);
  border-radius: 3px;
  overflow: hidden;
}

.occupancy-bar-fill {
  height: 100%;
  background: #1B1B1B;
  border-radius: 3px;
  transition: width 0.8s ease;
}

/* Stats row */
.stats-row {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 0;
}

.stat-item {
  display: flex;
  flex-direction: column;
  padding: 8px 0;
}

.stat-item:not(:last-child) {
  border-right: 1px solid rgba(27, 27, 27, 0.06);
  padding-right: 32px;
}

.stat-item:not(:first-child) {
  padding-left: 32px;
}

.stat-num {
  font-size: 36px;
  font-weight: 800;
  color: #1B1B1B;
  line-height: 1;
  letter-spacing: -1px;
}

.stat-label {
  font-size: 14px;
  font-weight: 600;
  color: rgba(27, 27, 27, 0.5);
  margin-top: 8px;
}

.stat-sub {
  font-size: 13px;
  color: rgba(27, 27, 27, 0.35);
  margin-top: 4px;
  font-weight: 500;
}

.stat-sub.muted {
  color: rgba(27, 27, 27, 0.25);
}

/* Responsive */
@media (max-width: 640px) {
  .dashboard {
    padding: 32px 16px;
  }

  .hero-number {
    font-size: 56px;
  }

  .occupancy-row {
    flex-direction: column;
    align-items: flex-start;
    gap: 16px;
  }

  .occupancy-bar-track {
    width: 100%;
  }

  .stats-row {
    grid-template-columns: 1fr;
    gap: 24px;
  }

  .stat-item:not(:last-child) {
    border-right: none;
    border-bottom: 1px solid rgba(27, 27, 27, 0.06);
    padding-right: 0;
    padding-bottom: 24px;
  }

  .stat-item:not(:first-child) {
    padding-left: 0;
  }
}
</style>
