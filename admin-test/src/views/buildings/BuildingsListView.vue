<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '@/services/api'
import type { Building } from '@/types/building'
import PropertyCard from '@/components/PropertyCard.vue'
import SidebarBanner from '@/components/SidebarBanner.vue'

const router = useRouter()
const buildings = ref<Building[]>([])
const isLoading = ref(false)
const errorMessage = ref('')
const searchQuery = ref('')

// Group buildings by city/region
const buildingsByCity = computed(() => {
  const groups: Record<string, Building[]> = {}

  // Filter by search query
  const filtered = searchQuery.value
    ? buildings.value.filter(b =>
        b.number?.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
        b.address?.toLowerCase().includes(searchQuery.value.toLowerCase())
      )
    : buildings.value

  for (const building of filtered) {
    const city = building.regionName || building.districtName || 'Other'
    if (!groups[city]) {
      groups[city] = []
    }
    groups[city].push(building)
  }

  return groups
})

// Sidebar preview images
const sidebarPreviews = computed(() => {
  const previews: { src: string; label?: string }[] = []

  for (const building of buildings.value.slice(0, 3)) {
    const imgSrc = building.images?.[0]
    if (imgSrc) {
      previews.push({
        src: imgSrc,
        label: building.number || 'Building'
      })
    }
  }

  return previews
})

onMounted(async () => {
  await loadBuildings()
})

async function loadBuildings() {
  isLoading.value = true
  errorMessage.value = ''
  try {
    const response = await api.getBuildings({ pageSize: 50 })
    buildings.value = response.items || []
  } catch (error) {
    console.error('Error loading buildings:', error)
    errorMessage.value = error instanceof Error ? error.message : 'Error loading buildings'
  } finally {
    isLoading.value = false
  }
}

function goToCreateBuilding() {
  router.push('/buildings/new')
}

function goToBuilding(id: string) {
  router.push(`/buildings/${id}`)
}

function getBuildingImage(building: Building): string | undefined {
  return building.images?.[0]
}
</script>

<template>
  <div class="buildings-page">
    <!-- Header with Search -->
    <div class="page-header">
      <div class="search-bar">
        <span class="search-icon">🔍</span>
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Фильтр и поиск"
        />
      </div>
    </div>

    <!-- Main Content Area -->
    <div class="content-layout">
      <!-- Main Content -->
      <div class="main-content">
        <!-- Loading State -->
        <div v-if="isLoading" class="loading-state">
          <div class="spinner spinner-lg"></div>
          <p>Loading...</p>
        </div>

        <!-- Empty State -->
        <div v-else-if="Object.keys(buildingsByCity).length === 0" class="empty-state">
          <div class="empty-state-icon">🏢</div>
          <h2>You don't have any buildings yet</h2>
          <p>Add your first building to start managing real estate</p>
          <button class="btn btn-primary btn-lg" @click="goToCreateBuilding">
            <span>+</span> Add Building
          </button>
        </div>

        <!-- Buildings Grid by City -->
        <div v-else class="city-groups">
          <div v-for="(cityBuildings, city) in buildingsByCity" :key="city" class="city-group">
            <h2 class="city-title">{{ city }}</h2>
            <div class="property-grid">
              <PropertyCard
                v-for="building in cityBuildings"
                :key="building.id"
                :image="getBuildingImage(building)"
                :title="building.number || 'Building'"
                :subtitle="`from ${building.totalArea || '—'} м² • ${building.floorsCount || '—'} floors`"
                :location="building.address"
                :price="null"
                price-label="/mo"
                :badges="[
                  ...(building.isCommercial ? [{ text: 'Commercial', type: 'info' as const }] : []),
                  ...(building.isLiving ? [{ text: 'Residential', type: 'success' as const }] : [])
                ]"
                :clickable="true"
                @click="goToBuilding(building.id)"
              />
            </div>
          </div>
        </div>
      </div>

      <!-- Sidebar -->
      <div class="sidebar-content">
        <SidebarBanner
          :images="sidebarPreviews"
          title="Пора выйти на рынок"
          subtitle="Опубликуйте объекты и получайте заявки"
          button-text="Выбрать объект"
          @action="goToCreateBuilding"
        />
      </div>
    </div>

    <!-- Floating Action Button -->
    <button class="fab" @click="goToCreateBuilding">
      <span>+</span>
      <span class="fab-label">Добавить</span>
    </button>
  </div>
</template>

<style scoped>
.buildings-page {
  padding: 24px;
  max-width: 1400px;
  margin: 0 auto;
  min-height: 100vh;
}

/* Header */
.page-header {
  margin-bottom: 24px;
}

.search-bar {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 20px;
  background: var(--bg-primary, #FFFFFF);
  border: 1px solid var(--border-color, rgba(27, 27, 27, 0.12));
  border-radius: 12px;
  max-width: 400px;
}

.search-icon {
  font-size: 16px;
  opacity: 0.5;
}

.search-bar input {
  flex: 1;
  border: none;
  background: transparent;
  font-size: 14px;
  color: var(--text-primary, #1B1B1B);
  outline: none;
}

.search-bar input::placeholder {
  color: var(--text-muted, rgba(27, 27, 27, 0.4));
}

/* Content Layout */
.content-layout {
  display: grid;
  grid-template-columns: 1fr 320px;
  gap: 32px;
  align-items: start;
}

.main-content {
  min-height: 400px;
}

.sidebar-content {
  position: sticky;
  top: 24px;
}

/* States */
.loading-state,
.empty-state {
  text-align: center;
  padding: 60px 20px;
  background: var(--bg-primary, #FFFFFF);
  border-radius: 16px;
}

.empty-state-icon {
  font-size: 48px;
  margin-bottom: 16px;
}

.empty-state h2 {
  font-size: 20px;
  color: var(--text-primary, #1B1B1B);
  margin: 0 0 8px;
}

.empty-state p {
  color: var(--text-secondary, rgba(27, 27, 27, 0.6));
  margin: 0 0 20px;
}

/* City Groups */
.city-groups {
  display: flex;
  flex-direction: column;
  gap: 32px;
}

.city-title {
  font-size: 20px;
  font-weight: 700;
  color: var(--text-primary, #1B1B1B);
  margin: 0 0 16px;
}

/* Property Grid */
.property-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
  gap: 16px;
}

/* Buttons */
.btn {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 12px 20px;
  border-radius: 10px;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  border: none;
  transition: all 0.2s;
}

.btn-primary {
  background: var(--color-primary, #1B1B1B);
  color: white;
}

.btn-primary:hover {
  background: var(--color-primary-hover, #333333);
  transform: translateY(-1px);
}

.btn-lg {
  padding: 14px 28px;
  font-size: 15px;
}

/* FAB */
.fab {
  position: fixed;
  bottom: 32px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 14px 28px;
  background: var(--color-primary, #1B1B1B);
  color: white;
  border: none;
  border-radius: 50px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.25);
  transition: all 0.2s;
  z-index: 100;
}

.fab:hover {
  transform: translateX(-50%) translateY(-2px);
  box-shadow: 0 12px 40px rgba(0, 0, 0, 0.3);
}

.fab span:first-child {
  font-size: 18px;
}

/* Responsive */
@media (max-width: 1024px) {
  .content-layout {
    grid-template-columns: 1fr;
  }

  .sidebar-content {
    display: none;
  }
}

@media (max-width: 768px) {
  .property-grid {
    grid-template-columns: 1fr;
  }
}
</style>
