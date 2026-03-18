<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '@/services/api'
import type { RealEstate } from '@/types/realestate'
import type { Building } from '@/types/building'
import TabBar from '@/components/TabBar.vue'
import SidebarBanner from '@/components/SidebarBanner.vue'

const router = useRouter()

// Tab state
const tabs = [
  { id: 'buildings', label: 'Здания' },
  { id: 'properties', label: 'Помещения' },
  { id: 'floorplans', label: 'Планировка' }
]
const activeTab = ref('buildings')

// Data state
const buildings = ref<Building[]>([])
const realEstates = ref<RealEstate[]>([])
const loading = ref(false)
const error = ref('')
const searchQuery = ref('')



// Group buildings by city/region
const buildingsByCity = computed(() => {
  const groups: Record<string, Building[]> = {}

  for (const building of buildings.value) {
    const city = building.regionName || building.districtName || 'Ташкент'
    if (!groups[city]) {
      groups[city] = []
    }
    groups[city].push(building)
  }

  return groups
})

// Get building name by ID
function getBuildingNameById(buildingId?: string): string {
  if (!buildingId) return 'Standalone'
  const building = buildings.value.find(b => b.id === buildingId)
  return building?.number || building?.address || 'Building'
}

// Group real estates by building name
const realEstatesByBuilding = computed(() => {
  const groups: Record<string, { buildingName: string; items: RealEstate[] }> = {}

  for (const re of realEstates.value) {
    // Try to get actual building name from buildings data
    const buildingName = getBuildingNameById(re.buildingId) || re.buildingNumber || 'Standalone'
    if (!groups[buildingName]) {
      groups[buildingName] = { buildingName, items: [] }
    }
    groups[buildingName].items.push(re)
  }

  return groups
})

// Sidebar preview images (first 3 with images)
const sidebarPreviews = computed(() => {
  const previews: { src: string; label?: string }[] = []

  for (const re of realEstates.value.slice(0, 3)) {
    const imgSrc = re.images?.[0]
    if (imgSrc) {
      previews.push({
        src: imgSrc,
        label: re.realEstateTypeName || 'Property'
      })
    }
  }

  return previews
})

// Load buildings
async function loadBuildings() {
  loading.value = true
  error.value = ''
  try {
    const response = await api.getBuildings({ pageSize: 50 })
    buildings.value = response.items || []
  } catch (e: any) {
    error.value = e.message || 'Error loading buildings'
  } finally {
    loading.value = false
  }
}



// Load real estates
async function loadRealEstates() {
  loading.value = true
  error.value = ''
  try {
    const params: any = { page: 1, pageSize: 50 }
    if (searchQuery.value) {
      params.filter = searchQuery.value
    }
    const response = await api.getRealEstates(params)
    realEstates.value = (response.items || []).map((item: any) => ({
      ...item,
      totalArea: item.totalArea,
      regionName: item.region,
      districtName: item.district,
      floorNumber: item.floor,
      realEstateTypeName: item.type
    }))
  } catch (e: any) {
    error.value = e.message || 'Error loading properties'
  } finally {
    loading.value = false
  }
}



// Computed: Real estates with plans
const realEstatesWithPlans = computed(() => {
  return realEstates.value.filter(re => re.plan)
})

// Get plan image helper
function getPlanImage(re: RealEstate): string | undefined {
  return re.plan || undefined
}

// Get image helpers
function getBuildingImage(buildingIdOrObj: Building | string): string | undefined {
  const b = typeof buildingIdOrObj === 'string'
    ? buildings.value.find(bld => bld.id === buildingIdOrObj)
    : buildingIdOrObj
  return b?.images?.[0]
}

function getRealEstateImage(re: RealEstate): string | undefined {
  return re.images?.[0]
}

// Navigation
function navigateToBuilding(id: string) {
  router.push(`/buildings/${id}`)
}

function navigateToRealEstate(id: string) {
  router.push(`/realestates/${id}`)
}

function navigateToCreate() {
  if (activeTab.value === 'buildings') {
    router.push('/buildings/new')
  } else {
    router.push('/realestates/new')
  }
}

function navigateToFloorplan(id: string) {
  router.push(`/realestates/${id}/floorplan`)
}

function handleSidebarAction() {
  router.push('/realestates/new')
}

// Watch tab changes and load data
watch(activeTab, async (newTab) => {
  if (newTab === 'buildings' && buildings.value.length === 0) {
    await loadBuildings()
  } else if ((newTab === 'properties' || newTab === 'floorplans') && realEstates.value.length === 0) {
    await loadRealEstates()
  }
})

// Lifecycle
onMounted(async () => {
  await Promise.all([loadBuildings(), loadRealEstates()])
})
</script>

<template>
  <div class="my-realestate-view">
    <!-- Centered Header with Search and Tabs -->
    <div class="view-header">
      <div class="search-bar">
        <svg class="search-icon" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <circle cx="11" cy="11" r="8"></circle>
          <path d="m21 21-4.35-4.35"></path>
        </svg>
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Фильтр и поиск"
          @keyup.enter="activeTab === 'buildings' ? loadBuildings() : loadRealEstates()"
        />
      </div>

      <!-- Tab Bar -->
      <TabBar :tabs="tabs" v-model="activeTab" />
    </div>

    <!-- Content Layout -->
    <div class="content-layout">
      <!-- Main Content -->
      <div class="main-content">
        <!-- Loading State -->
        <div v-if="loading" class="loading-state">
          <div class="spinner"></div>
          <p>Загрузка...</p>
        </div>

        <!-- Error State -->
        <div v-else-if="error" class="error-state">
          <p>{{ error }}</p>
          <button class="btn btn-secondary" @click="activeTab === 'buildings' ? loadBuildings() : loadRealEstates()">
            Повторить
          </button>
        </div>

        <!-- ============ BUILDINGS TAB ============ -->
        <template v-else-if="activeTab === 'buildings'">
          <div v-if="Object.keys(buildingsByCity).length === 0" class="empty-state">
            <div class="empty-icon">🏢</div>
            <h3>Нет зданий</h3>
            <p>Добавьте первое здание</p>
            <button class="btn btn-primary" @click="navigateToCreate">+ Добавить</button>
          </div>

          <div v-else class="city-sections">
            <section v-for="(cityBuildings, city) in buildingsByCity" :key="city" class="city-section">
              <h2 class="section-title">{{ city }}</h2>
              <div class="buildings-grid">
                <article
                  v-for="building in cityBuildings"
                  :key="building.id"
                  class="building-card"
                  @click="navigateToBuilding(building.id)"
                >
                  <div class="card-image">
                    <img v-if="getBuildingImage(building)" :src="getBuildingImage(building)" :alt="building.number" />
                    <div v-else class="card-placeholder">🏢</div>
                  </div>
                  <div class="card-body">
                    <h4 class="card-title">{{ building.number || 'Здание' }}</h4>
                    <p class="card-meta">from {{ building.totalArea || '—' }} м² • {{ building.floorsCount || '—' }} plans</p>
                    <p class="card-price">112 000 000 UZS <span class="price-period">/mo</span></p>
                  </div>
                </article>
              </div>
            </section>
          </div>
        </template>

        <!-- ============ PROPERTIES TAB ============ -->
        <template v-else-if="activeTab === 'properties'">
          <div v-if="Object.keys(realEstatesByBuilding).length === 0" class="empty-state">
            <div class="empty-icon">🏠</div>
            <h3>Нет помещений</h3>
            <p>Добавьте первое помещение</p>
            <button class="btn btn-primary" @click="navigateToCreate">+ Добавить</button>
          </div>

          <div v-else class="building-sections">
            <section v-for="(group, buildingKey) in realEstatesByBuilding" :key="buildingKey" class="building-section">
              <div class="section-header">
                <h2 class="section-title">{{ group.buildingName }}</h2>
                <span class="section-link">Посмотреть все +{{ group.items.length }}</span>
              </div>
              <div class="properties-grid">
                <article
                  v-for="re in group.items"
                  :key="re.id"
                  class="property-card"
                  @click="navigateToRealEstate(re.id)"
                >
                  <div class="card-image">
                    <img v-if="getRealEstateImage(re)" :src="getRealEstateImage(re)" :alt="re.realEstateTypeName" />
                    <div v-else class="card-placeholder">🏠</div>
                  </div>
                  <div class="card-body">
                    <h4 class="card-title">{{ re.number || re.realEstateTypeName || 'Property' }}</h4>
                    <p class="card-meta">{{ re.totalArea || '—' }} м² • {{ re.roomsCount || '—' }} rooms</p>
                    <p class="card-price">
                      Negotiable
                      <span class="price-period">/mo</span>
                    </p>
                  </div>
                </article>
              </div>
            </section>
          </div>
        </template>

        <!-- ============ FLOORPLANS TAB ============ -->
        <template v-else-if="activeTab === 'floorplans'">
          <div v-if="realEstatesWithPlans.length === 0" class="empty-state">
            <div class="empty-icon">📐</div>
            <h3>Нет планировок</h3>
            <p>У объектов нет загруженных планировок</p>
            <button class="btn btn-primary" @click="navigateToCreate">+ Добавить</button>
          </div>

          <div v-else class="floorplan-sections">
            <section class="floorplan-section">
              <div class="section-header">
                <div class="section-icon">📐</div>
                <h2 class="section-title">Планировки объектов</h2>
                <span class="section-link">{{ realEstatesWithPlans.length }} планировок</span>
              </div>
              <div class="floorplan-grid">
                <article
                  v-for="re in realEstatesWithPlans"
                  :key="re.id"
                  class="floorplan-card"
                  @click="navigateToFloorplan(re.id)"
                >
                  <div class="floorplan-image">
                    <img v-if="getPlanImage(re)" :src="getPlanImage(re)" :alt="re.realEstateTypeName || 'Планировка'" />
                    <div v-else class="floorplan-loading">
                      <div class="spinner small"></div>
                    </div>
                  </div>
                  <div class="floorplan-info">
                    <h4>{{ re.number || re.realEstateTypeName || 'Property' }}</h4>
                    <p>{{ re.districtName || re.regionName || 'Tashkent' }}</p>
                    <div class="floorplan-meta">
                      <span v-if="re.totalArea">📐 {{ re.totalArea }} м²</span>
                      <span v-if="re.roomsCount">� {{ re.roomsCount }} комнат</span>
                    </div>
                  </div>
                </article>
              </div>
            </section>
          </div>
        </template>
      </div>

      <!-- Sidebar -->
      <aside class="sidebar">
        <SidebarBanner
          :images="sidebarPreviews"
          title="Пора выйти на рынок"
          subtitle="Опубликуйте объекты и получайте заявки"
          button-text="Выбрать объект"
          @action="handleSidebarAction"
        />
      </aside>
    </div>

    <!-- Bottom Floating Toolbar -->
    <div class="bottom-toolbar">
      <button class="toolbar-btn primary" @click="navigateToCreate">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M12 5v14"></path>
          <path d="M5 12h14"></path>
        </svg>
        Добавить
      </button>
      <button class="toolbar-btn secondary">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path>
          <circle cx="12" cy="10" r="3"></circle>
        </svg>
        На карте
      </button>
      <div class="toolbar-divider"></div>
      <button class="toolbar-btn icon-only">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <rect x="3" y="3" width="7" height="7"></rect>
          <rect x="14" y="3" width="7" height="7"></rect>
          <rect x="14" y="14" width="7" height="7"></rect>
          <rect x="3" y="14" width="7" height="7"></rect>
        </svg>
      </button>
      <button class="toolbar-btn icon-only active">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <line x1="8" y1="6" x2="21" y2="6"></line>
          <line x1="8" y1="12" x2="21" y2="12"></line>
          <line x1="8" y1="18" x2="21" y2="18"></line>
          <line x1="3" y1="6" x2="3.01" y2="6"></line>
          <line x1="3" y1="12" x2="3.01" y2="12"></line>
          <line x1="3" y1="18" x2="3.01" y2="18"></line>
        </svg>
      </button>
    </div>
  </div>
</template>

<style scoped>
.my-realestate-view {
  padding: 20px 24px;
  max-width: 1400px;
  margin: 0 auto;
  padding-bottom: 80px; /* Space for bottom toolbar */
}

/* Header - Figma: Centered vertical stack with search above tabs */
.view-header {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 16px;
  margin-bottom: 24px;
}

/* Search Bar - Figma: Width 412px, Height 44px, Radius 100px */
.search-bar {
  display: flex;
  align-items: center;
  gap: 10px;
  height: 44px;
  padding: 0 16px;
  background: #fff;
  border: 0.5px solid rgba(27, 27, 27, 0.04);
  border-radius: 100px;
  width: 412px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
}

.search-icon {
  color: rgba(27, 27, 27, 0.4);
  flex-shrink: 0;
}

.search-bar input {
  flex: 1;
  border: none;
  background: transparent;
  font-size: 13px;
  outline: none;
  color: #1B1B1B;
}

.search-bar input::placeholder {
  color: rgba(27, 27, 27, 0.4);
}

/* Content Layout */
.content-layout {
  display: grid;
  grid-template-columns: 1fr 300px;
  gap: 24px;
  align-items: start;
}

.main-content {
  min-height: 400px;
}

.sidebar {
  position: sticky;
  top: 20px;
}

/* States - Apple Glass Style */
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
  font-size: 40px;
  margin-bottom: 12px;
}

.empty-state h3 {
  font-size: 18px;
  margin: 0 0 6px;
  color: #1B1B1B;
}

.empty-state p {
  color: rgba(27, 27, 27, 0.5);
  margin: 0 0 16px;
  font-size: 14px;
}

/* Section Titles */
.city-sections,
.building-sections,
.floorplan-sections {
  display: flex;
  flex-direction: column;
  gap: 32px;
}

.section-title {
  font-size: 18px;
  font-weight: 700;
  color: #1B1B1B;
  margin: 0 0 16px;
}

.section-header {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 16px;
}

/* Section Icon - Figma: 40x40 circular thumbnail */
.section-icon {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  overflow: hidden;
}

.section-icon.building-photo {
  background: linear-gradient(135deg, #FC463D 0%, #FC933D 100%);
}

.section-icon.building-photo img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.section-header .section-title {
  margin: 0;
  flex: 1;
}

.section-link {
  font-size: 13px;
  color: rgba(27, 27, 27, 0.5);
  cursor: pointer;
}

.section-link:hover {
  color: #FF5B3C;
}

.section-stats {
  display: flex;
  gap: 12px;
  font-size: 13px;
  color: rgba(27, 27, 27, 0.5);
}

/* Grids */
.buildings-grid,
.properties-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 16px;
}

/* Cards - Figma: 24px radius, soft shadow */
.building-card,
.property-card {
  background: #FFFFFF;
  border-radius: 24px;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.2s ease;
  border: 0.5px solid rgba(27, 27, 27, 0.04);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
}

.building-card:hover,
.property-card:hover {
  transform: translateY(-3px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.08);
  border-color: #FF5B3C;
}

.card-image {
  height: 140px;
  background: #F6F6F6;
  overflow: hidden;
  border-radius: 20px 20px 0 0;
}

.card-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.card-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 36px;
  opacity: 0.25;
}

.card-body {
  padding: 12px;
}

.card-title {
  font-size: 14px;
  font-weight: 600;
  color: #1B1B1B;
  margin: 0 0 4px;
  line-height: 1.3;
}

.card-meta {
  font-size: 12px;
  color: rgba(27, 27, 27, 0.5);
  margin: 0 0 6px;
}

.card-price {
  font-size: 13px;
  font-weight: 700;
  color: #FF5B3C;
  margin: 0;
}

.price-period {
  font-weight: 400;
  color: rgba(27, 27, 27, 0.4);
}

/* Floorplan */
.floorplan-content {
  background: #fff;
  border-radius: 14px;
  overflow: hidden;
}

.floorplan-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 48px;
  color: rgba(27, 27, 27, 0.4);
}

.placeholder-icon {
  font-size: 40px;
  margin-bottom: 8px;
  opacity: 0.5;
}

.floorplan-placeholder p {
  font-size: 13px;
  margin: 0;
}

/* Floorplan Grid - New Styles */
.floorplan-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 20px;
}

.floorplan-card {
  background: #FFFFFF;
  border-radius: 20px;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.2s ease;
  border: 0.5px solid rgba(27, 27, 27, 0.04);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
}

.floorplan-card:hover {
  transform: translateY(-3px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.08);
  border-color: #FF5B3C;
}

.floorplan-image {
  height: 200px;
  background: #F6F6F6;
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
}

.floorplan-image img {
  width: 100%;
  height: 100%;
  object-fit: contain;
  background: white;
}

.floorplan-loading {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 100%;
}

.spinner.small {
  width: 24px;
  height: 24px;
  border-width: 2px;
}

.floorplan-info {
  padding: 16px;
}

.floorplan-info h4 {
  font-size: 15px;
  font-weight: 600;
  color: #1B1B1B;
  margin: 0 0 4px;
}

.floorplan-info p {
  font-size: 13px;
  color: rgba(27, 27, 27, 0.6);
  margin: 0 0 8px;
}

.floorplan-meta {
  display: flex;
  gap: 12px;
  font-size: 12px;
  color: rgba(27, 27, 27, 0.5);
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

/* Bottom Floating Toolbar - Figma: Centered bottom bar */
.bottom-toolbar {
  position: fixed;
  bottom: 24px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px;
  background: #1B1B1B;
  border-radius: 100px;
  box-shadow: 0 6px 24px rgba(0, 0, 0, 0.25);
  z-index: 100;
}

.toolbar-btn {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 10px 16px;
  border: none;
  border-radius: 100px;
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.toolbar-btn.primary {
  background: #fff;
  color: #1B1B1B;
}

.toolbar-btn.primary:hover {
  background: #f0f0f0;
}

.toolbar-btn.secondary {
  background: rgba(255, 255, 255, 0.1);
  color: #fff;
}

.toolbar-btn.secondary:hover {
  background: rgba(255, 255, 255, 0.2);
}

.toolbar-btn.icon-only {
  padding: 10px;
  background: transparent;
  color: rgba(255, 255, 255, 0.5);
}

.toolbar-btn.icon-only:hover {
  color: #fff;
}

.toolbar-btn.icon-only.active {
  color: #fff;
}

.toolbar-divider {
  width: 1px;
  height: 20px;
  background: rgba(255, 255, 255, 0.2);
  margin: 0 4px;
}

/* Responsive */
@media (max-width: 1024px) {
  .content-layout {
    grid-template-columns: 1fr;
  }

  .sidebar {
    display: none;
  }
}

@media (max-width: 640px) {
  .view-header {
    flex-direction: column;
    align-items: stretch;
  }

  .search-bar {
    width: 100%;
  }

  .buildings-grid,
  .properties-grid {
    grid-template-columns: 1fr 1fr;
  }
}
</style>
