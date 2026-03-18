<script setup lang="ts">
import { ref, onMounted, computed, nextTick } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { api } from '@/services/api'
import type { RealEstate, RealEstateUnit, RealEstateType, Renovation } from '@/types/realestate'

const router = useRouter()
const route = useRoute()

const realEstateId = computed(() => route.params.id as string)

// State
const realEstate = ref<RealEstate | null>(null)
const units = ref<RealEstateUnit[]>([])
const selectedUnit = ref<RealEstateUnit | null>(null)
const loading = ref(false)
const error = ref('')
const planImageUrl = ref<string | null>(null)

// Floor management
const floors = ref<{ id: string; number: number; freeCount: number; occupiedCount: number }[]>([])
const selectedFloorIndex = ref(0)

// Reference data for unit creation
const realEstateTypes = ref<RealEstateType[]>([])
const renovations = ref<Renovation[]>([])

// UI State
const showUnitPanel = ref(false)
const showCreateForm = ref(false)
const savingUnit = ref(false)

// Drawing Mode State
const isDrawingMode = ref(false)
const drawingPoints = ref<{ x: number; y: number }[]>([])
const planContainerRef = ref<HTMLElement | null>(null)
const planImageRef = ref<HTMLImageElement | null>(null)

// Unit form data
const unitForm = ref({
  realEstateTypeId: '',
  totalArea: 0,
  renovationId: '',
  floorNumber: undefined as number | undefined,
  number: '',
  livingArea: undefined as number | undefined,
  ceilingHeight: undefined as number | undefined,
  roomsCount: undefined as number | undefined,
  coordinates: '' // Store polygon coordinates as JSON string
})

// Load real estate and its units
async function loadData() {
  loading.value = true
  error.value = ''
  try {
    realEstate.value = await api.getRealEstateById(realEstateId.value)

    // Plan image is now a direct URL from API
    console.log('Real estate plan value:', realEstate.value?.plan)
    console.log('Real estate full response:', JSON.stringify(realEstate.value, null, 2))
    if (realEstate.value?.plan) {
      planImageUrl.value = realEstate.value.plan
      console.log('planImageUrl set to:', planImageUrl.value)
    }

    try {
      const unitsResponse = await api.getRealEstateUnits({
        realEstateId: realEstateId.value,
        pageSize: 100
      })
      units.value = unitsResponse?.items || []
    } catch (e) {
      console.warn('Units API not available:', e)
      units.value = []
    }

    floors.value = [{
      id: '1',
      number: 1,
      freeCount: units.value.filter(u => u.status === 'Active').length || 0,
      occupiedCount: units.value.filter(u => u.status !== 'Active').length || units.value.length
    }]

    try {
      const [typesRes, renovationsRes] = await Promise.all([
        api.getRealEstateTypes(),
        api.getRenovations({ pageSize: 50 })
      ])
      realEstateTypes.value = typesRes || []
      renovations.value = renovationsRes?.items || renovationsRes || []
    } catch (e) {
      console.error('Failed to load reference data:', e)
    }
  } catch (e: unknown) {
    error.value = (e as Error).message || 'Error loading data'
  } finally {
    loading.value = false
  }
}

// Select a unit
function selectUnit(unit: RealEstateUnit) {
  if (isDrawingMode.value) return
  selectedUnit.value = unit
  showUnitPanel.value = true
  showCreateForm.value = false
}

// Close unit panel
function closeUnitPanel() {
  selectedUnit.value = null
  showUnitPanel.value = false
  showCreateForm.value = false
  exitDrawingMode()
}

// Start drawing mode for unit creation
function startDrawingMode() {
  isDrawingMode.value = true
  drawingPoints.value = []
  showUnitPanel.value = false
  showCreateForm.value = false
  selectedUnit.value = null
}

// Exit drawing mode
function exitDrawingMode() {
  isDrawingMode.value = false
  drawingPoints.value = []
}

// Handle click on plan to add drawing point
function handlePlanClick(event: MouseEvent) {
  if (!isDrawingMode.value) {
    closeUnitPanel()
    return
  }

  const container = planContainerRef.value
  const image = planImageRef.value
  if (!container || !image) return

  const rect = image.getBoundingClientRect()

  // Calculate position relative to image (as percentage)
  const x = ((event.clientX - rect.left) / rect.width) * 100
  const y = ((event.clientY - rect.top) / rect.height) * 100

  // Add point (clamp to 0-100)
  drawingPoints.value.push({
    x: Math.max(0, Math.min(100, x)),
    y: Math.max(0, Math.min(100, y))
  })
}

// Complete polygon and open form
function completePolygon() {
  if (drawingPoints.value.length < 3) {
    alert('Нужно минимум 3 точки для создания области')
    return
  }

  // Store coordinates as JSON
  unitForm.value = {
    realEstateTypeId: realEstateTypes.value[0]?.id || '',
    totalArea: 0,
    renovationId: '',
    floorNumber: floors.value[selectedFloorIndex.value]?.number || 1,
    number: '',
    livingArea: undefined,
    ceilingHeight: undefined,
    roomsCount: undefined,
    coordinates: JSON.stringify(drawingPoints.value)
  }

  isDrawingMode.value = false
  showCreateForm.value = true
  showUnitPanel.value = true
}

// Undo last point
function undoLastPoint() {
  if (drawingPoints.value.length > 0) {
    drawingPoints.value.pop()
  }
}

// Clear all points
function clearDrawing() {
  drawingPoints.value = []
}

// Get polygon path for SVG
const polygonPath = computed(() => {
  if (drawingPoints.value.length < 2) return ''
  return drawingPoints.value.map((p, i) =>
    `${i === 0 ? 'M' : 'L'} ${p.x} ${p.y}`
  ).join(' ') + (drawingPoints.value.length > 2 ? ' Z' : '')
})

// Open create form (now starts drawing mode)
function openCreateForm() {
  startDrawingMode()
}

// Save unit
async function saveUnit() {
  savingUnit.value = true
  try {
    const unitNumber = unitForm.value.number || `Unit-${Date.now()}`

    // Parse coordinates from JSON string to array of {x, y} objects
    let coordinatesArray: { x: number; y: number }[] | undefined
    if (unitForm.value.coordinates) {
      try {
        coordinatesArray = JSON.parse(unitForm.value.coordinates)
      } catch (e) {
        console.error('Failed to parse coordinates:', e)
      }
    }

    await api.createRealEstateUnit({
      realEstateId: realEstateId.value,
      realEstateTypeId: unitForm.value.realEstateTypeId,
      totalArea: unitForm.value.totalArea,
      renovationId: unitForm.value.renovationId || undefined,
      floorNumber: unitForm.value.floorNumber,
      roomNumber: unitNumber,
      ceilingHeight: unitForm.value.ceilingHeight,
      coordinates: coordinatesArray  // Polygon coordinates as array of {x, y}
    })

    console.log('Unit created with coordinates:', unitForm.value.coordinates)

    await loadData()
    showCreateForm.value = false
    showUnitPanel.value = false
    drawingPoints.value = []
  } catch (e: unknown) {
    alert((e as Error).message || 'Error saving unit')
  } finally {
    savingUnit.value = false
  }
}

// Delete unit
async function deleteUnit() {
  if (!selectedUnit.value) return
  if (!confirm('Вы уверены, что хотите удалить этот юнит?')) return

  try {
    await api.deleteRealEstateUnit(selectedUnit.value.id)
    await loadData()
    closeUnitPanel()
  } catch (e: unknown) {
    alert((e as Error).message || 'Error deleting unit')
  }
}

// Get type name by ID
function getTypeName(typeId?: string): string {
  if (!typeId) return 'Не указан'
  const type = realEstateTypes.value.find(t => t.id === typeId)
  return type?.name || 'Не указан'
}

// Get unit status class
function getUnitStatusClass(unit: RealEstateUnit): string {
  if (selectedUnit.value?.id === unit.id) return 'selected'
  if (unit.status === 'Active') return 'available'
  return 'occupied'
}

// Parse unit coordinates from coordinates field (or fallback to reason for legacy data)
function parseUnitCoordinates(unit: RealEstateUnit): { x: number; y: number }[] {
  // First try new coordinates field (array of {x, y} objects)
  if (unit.coordinates && Array.isArray(unit.coordinates) && unit.coordinates.length > 0) {
    return unit.coordinates
  }
  // Fallback to legacy reason field (JSON string)
  if (unit.reason) {
    try {
      const coords = JSON.parse(unit.reason)
      if (Array.isArray(coords) && coords.length > 0 && typeof coords[0].x === 'number') {
        return coords
      }
    } catch {
      // Not valid JSON coordinates
    }
  }
  return []
}

// Get SVG polygon path for a unit's saved area
function getUnitPolygonPath(unit: RealEstateUnit): string {
  const coords = parseUnitCoordinates(unit)
  if (coords.length < 3) return ''
  return coords.map((p, i) =>
    `${i === 0 ? 'M' : 'L'} ${p.x} ${p.y}`
  ).join(' ') + ' Z'
}

// Check if unit has saved polygon
function unitHasPolygon(unit: RealEstateUnit): boolean {
  return parseUnitCoordinates(unit).length >= 3
}

// Lifecycle
onMounted(() => {
  loadData()
})


</script>

<template>
  <div class="floorplan-view" :class="{ 'drawing-mode': isDrawingMode }">
    <!-- Back Button -->
    <button class="back-button" @click="isDrawingMode ? exitDrawingMode() : router.back()">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <path d="M19 12H5M12 19l-7-7 7-7"/>
      </svg>
      {{ isDrawingMode ? 'Отмена' : 'Назад' }}
    </button>

    <!-- Drawing Mode Toolbar -->
    <div v-if="isDrawingMode" class="drawing-toolbar">
      <div class="toolbar-info">
        <span class="icon">✏️</span>
        <span>Рисование области: кликните на план чтобы добавить точки ({{ drawingPoints.length }} точек)</span>
      </div>
      <div class="toolbar-actions">
        <button class="tool-btn" @click="undoLastPoint" :disabled="drawingPoints.length === 0">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M3 10h10a4 4 0 0 1 4 4v7m-7-11 5 5-5 5"/>
          </svg>
          Отменить точку
        </button>
        <button class="tool-btn" @click="clearDrawing" :disabled="drawingPoints.length === 0">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M3 6h18M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6"/>
          </svg>
          Очистить
        </button>
        <button class="tool-btn primary" @click="completePolygon" :disabled="drawingPoints.length < 3">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <polyline points="20 6 9 17 4 12"/>
          </svg>
          Готово ({{ drawingPoints.length }}/3)
        </button>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading-overlay">
      <div class="spinner"></div>
    </div>

    <!-- Error -->
    <div v-else-if="error" class="error-state">
      <p>{{ error }}</p>
      <button class="btn btn-secondary" @click="loadData">Повторить</button>
    </div>

    <!-- Main Content -->
    <div v-else class="main-content">
      <!-- Floor Selector (Left Panel) -->
      <aside class="floor-selector" v-if="!isDrawingMode">
        <div
          v-for="(floor, index) in floors"
          :key="floor.id"
          class="floor-item"
          :class="{ active: selectedFloorIndex === index }"
          @click="selectedFloorIndex = index"
        >
          <div class="floor-thumbnail">
            <img v-if="planImageUrl" :src="planImageUrl" alt="Floor plan" />
            <div v-else class="floor-placeholder">📐</div>
          </div>
          <div class="floor-label">{{ floor.number }} этаж</div>
          <div class="floor-stats">
            <span class="stat free">свободно<br><strong>{{ floor.freeCount }}</strong></span>
            <span class="stat occupied">занято<br><strong>{{ floor.occupiedCount }}</strong></span>
          </div>
        </div>

        <button class="add-floor-btn">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M12 5v14M5 12h14"/>
          </svg>
          Добавить этаж
        </button>
      </aside>

      <!-- Floor Plan Canvas -->
      <div
        ref="planContainerRef"
        class="plan-canvas"
        :class="{ 'is-drawing': isDrawingMode }"
        @click="handlePlanClick"
      >
        <div class="plan-container">
          <img
            v-if="planImageUrl"
            ref="planImageRef"
            :src="planImageUrl"
            alt="Планировка"
            class="plan-image"
          />
          <div v-else class="no-plan">
            <span class="icon">📐</span>
            <p>Планировка не загружена</p>
          </div>

          <!-- Drawing Overlay -->
          <svg v-if="isDrawingMode && planImageUrl" class="drawing-overlay" viewBox="0 0 100 100" preserveAspectRatio="none">
            <!-- Polygon shape -->
            <path
              v-if="drawingPoints.length > 1"
              :d="polygonPath"
              fill="rgba(255, 91, 60, 0.3)"
              stroke="#FF5B3C"
              stroke-width="0.3"
              stroke-dasharray="1"
            />

            <!-- Points -->
            <circle
              v-for="(point, index) in drawingPoints"
              :key="index"
              :cx="point.x"
              :cy="point.y"
              r="1"
              fill="#FF5B3C"
              stroke="#fff"
              stroke-width="0.3"
            />

            <!-- Point numbers -->
            <text
              v-for="(point, index) in drawingPoints"
              :key="'label-' + index"
              :x="point.x + 1.5"
              :y="point.y + 0.5"
              fill="#1B1B1B"
              font-size="2"
              font-weight="bold"
            >{{ index + 1 }}</text>
          </svg>

          <!-- Saved Unit Polygons (from coordinates field) -->
          <svg v-if="!isDrawingMode && planImageUrl" class="unit-polygons-overlay" viewBox="0 0 100 100" preserveAspectRatio="none">
            <g v-for="unit in units" :key="'polygon-' + unit.id">
              <!-- Unit polygon area -->
              <path
                v-if="unitHasPolygon(unit)"
                :d="getUnitPolygonPath(unit)"
                :fill="selectedUnit?.id === unit.id ? 'rgba(255, 91, 60, 0.5)' : unit.status === 'Active' ? 'rgba(34, 197, 94, 0.3)' : 'rgba(255, 91, 60, 0.3)'"
                :stroke="selectedUnit?.id === unit.id ? '#FF5B3C' : unit.status === 'Active' ? '#22C55E' : '#FF5B3C'"
                stroke-width="0.3"
                class="unit-polygon"
                @click.stop="selectUnit(unit)"
              />
              <!-- Unit label in center of polygon -->
              <text
                v-if="unitHasPolygon(unit)"
                :x="parseUnitCoordinates(unit).reduce((sum, p) => sum + p.x, 0) / parseUnitCoordinates(unit).length"
                :y="parseUnitCoordinates(unit).reduce((sum, p) => sum + p.y, 0) / parseUnitCoordinates(unit).length"
                fill="#1B1B1B"
                font-size="3"
                font-weight="bold"
                text-anchor="middle"
                dominant-baseline="middle"
                class="unit-label"
                @click.stop="selectUnit(unit)"
              >{{ unit.number || '—' }}</text>
            </g>
          </svg>

          <!-- Unit Markers (fallback for units without polygon) -->
          <div v-if="!isDrawingMode" class="unit-markers-overlay">
            <div
              v-for="(unit, index) in units.filter(u => !unitHasPolygon(u))"
              :key="unit.id"
              class="unit-marker"
              :class="getUnitStatusClass(unit)"
              :style="{
                left: `${20 + (index % 5) * 18}%`,
                top: `${25 + Math.floor(index / 5) * 30}%`
              }"
              @click.stop="selectUnit(unit)"
            >
              <span class="unit-number">{{ unit.number || (index + 1) }}</span>
              <svg v-if="unit.status === 'Active'" class="check-icon" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3">
                <polyline points="20 6 9 17 4 12"></polyline>
              </svg>
            </div>
          </div>
        </div>
      </div>

      <!-- Unit Panel -->
      <aside v-if="showUnitPanel && !isDrawingMode" class="unit-panel" @click.stop>
        <!-- Unit Details -->
        <template v-if="selectedUnit && !showCreateForm">
          <div class="panel-header">
            <h3>Юнит {{ selectedUnit.number || '—' }}</h3>
            <button class="close-btn" @click="closeUnitPanel">×</button>
          </div>

          <div class="panel-content">
            <div class="status-badge" :class="selectedUnit.status?.toLowerCase() || 'draft'">
              {{ selectedUnit.status === 'Active' ? 'Свободно' : 'Занято' }}
            </div>

            <div class="info-grid">
              <div class="info-item">
                <span class="label">Тип</span>
                <span class="value">{{ getTypeName(selectedUnit.realEstateTypeId) }}</span>
              </div>
              <div class="info-item">
                <span class="label">Площадь</span>
                <span class="value">{{ selectedUnit.totalArea || '—' }} м²</span>
              </div>
              <div class="info-item" v-if="selectedUnit.roomsCount">
                <span class="label">Комнат</span>
                <span class="value">{{ selectedUnit.roomsCount }}</span>
              </div>
              <div class="info-item" v-if="selectedUnit.floorNumber">
                <span class="label">Этаж</span>
                <span class="value">{{ selectedUnit.floorNumber }}</span>
              </div>
            </div>

            <div class="panel-actions">
              <button class="btn btn-primary" @click="showCreateForm = true">Редактировать</button>
              <button class="btn btn-danger" @click="deleteUnit">Удалить</button>
            </div>
          </div>
        </template>

        <!-- Create/Edit Form -->
        <template v-else-if="showCreateForm">
          <div class="panel-header">
            <h3>{{ selectedUnit ? 'Редактировать' : 'Новый юнит' }}</h3>
            <button class="close-btn" @click="closeUnitPanel">×</button>
          </div>

          <div class="panel-content">
            <!-- Coordinates Preview -->
            <div v-if="unitForm.coordinates" class="coordinates-preview">
              <div class="preview-label">📍 Область на плане</div>
              <div class="preview-points">{{ JSON.parse(unitForm.coordinates).length }} точек определено</div>
            </div>

            <div class="form-group">
              <label>Тип</label>
              <select v-model="unitForm.realEstateTypeId">
                <option value="">Выберите тип</option>
                <option v-for="type in realEstateTypes" :key="type.id" :value="type.id">
                  {{ type.name }}
                </option>
              </select>
            </div>

            <div class="form-group">
              <label>Номер</label>
              <input type="text" v-model="unitForm.number" placeholder="501-A" />
            </div>

            <div class="form-row">
              <div class="form-group">
                <label>Площадь (м²)</label>
                <input type="number" v-model.number="unitForm.totalArea" min="0" step="0.1" />
              </div>
              <div class="form-group">
                <label>Комнат</label>
                <input type="number" v-model.number="unitForm.roomsCount" min="0" />
              </div>
            </div>

            <div class="form-group">
              <label>Ремонт</label>
              <select v-model="unitForm.renovationId">
                <option value="">Не указан</option>
                <option v-for="ren in renovations" :key="ren.id" :value="ren.id">
                  {{ ren.name }}
                </option>
              </select>
            </div>

            <div class="panel-actions">
              <button
                class="btn btn-primary"
                @click="saveUnit"
                :disabled="savingUnit || !unitForm.totalArea"
              >
                {{ savingUnit ? 'Сохранение...' : 'Сохранить' }}
              </button>
              <button class="btn btn-secondary" @click="closeUnitPanel">Отмена</button>
            </div>
          </div>
        </template>
      </aside>
    </div>

    <!-- Bottom Toolbar -->
    <div v-if="!isDrawingMode" class="bottom-toolbar">
      <button class="toolbar-btn">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <rect x="3" y="3" width="7" height="7"></rect>
          <rect x="14" y="3" width="7" height="7"></rect>
          <rect x="14" y="14" width="7" height="7"></rect>
          <rect x="3" y="14" width="7" height="7"></rect>
        </svg>
        Выделить этаж
      </button>
      <button class="toolbar-btn">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <polygon points="22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3"></polygon>
        </svg>
        Фильтр
      </button>
      <button class="toolbar-btn primary" @click="openCreateForm">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M12 5v14M5 12h14"/>
        </svg>
        Добавить
      </button>
    </div>
  </div>
</template>

<style scoped>
.floorplan-view {
  position: relative;
  width: 100%;
  height: 100vh;
  background: #F5F5F5;
  overflow: hidden;
}

.floorplan-view.drawing-mode {
  cursor: crosshair;
}

/* Back Button */
.back-button {
  position: absolute;
  top: 16px;
  left: 16px;
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 16px;
  background: #fff;
  border: none;
  border-radius: 8px;
  color: #1B1B1B;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  z-index: 100;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
  transition: all 0.2s;
}

.back-button:hover {
  background: #f5f5f5;
}

/* Drawing Toolbar */
.drawing-toolbar {
  position: absolute;
  top: 16px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 12px 20px;
  background: #1B1B1B;
  border-radius: 12px;
  z-index: 100;
  box-shadow: 0 4px 20px rgba(0,0,0,0.3);
}

.toolbar-info {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #fff;
  font-size: 13px;
}

.toolbar-info .icon {
  font-size: 16px;
}

.toolbar-actions {
  display: flex;
  align-items: center;
  gap: 8px;
}

.tool-btn {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 12px;
  background: rgba(255,255,255,0.1);
  border: none;
  border-radius: 8px;
  color: #fff;
  font-size: 12px;
  cursor: pointer;
  transition: all 0.2s;
}

.tool-btn:hover:not(:disabled) {
  background: rgba(255,255,255,0.2);
}

.tool-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.tool-btn.primary {
  background: #FF5B3C;
}

.tool-btn.primary:hover:not(:disabled) {
  background: #E54B2E;
}

/* Loading & Error */
.loading-overlay {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.9);
  z-index: 200;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 3px solid #e5e7eb;
  border-top-color: #FF5B3C;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.error-state {
  position: absolute;
  inset: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 16px;
}

/* Main Content Layout */
.main-content {
  display: flex;
  height: 100%;
  padding-top: 56px;
}

/* Floor Selector */
.floor-selector {
  width: 140px;
  padding: 16px 12px;
  display: flex;
  flex-direction: column;
  gap: 12px;
  background: #fff;
  border-right: 1px solid rgba(0, 0, 0, 0.06);
  overflow-y: auto;
}

.floor-item {
  padding: 8px;
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.2s;
  border: 1px solid transparent;
}

.floor-item:hover {
  background: rgba(0, 0, 0, 0.02);
}

.floor-item.active {
  background: rgba(255, 91, 60, 0.1);
  border-color: #FF5B3C;
}

.floor-thumbnail {
  width: 100%;
  height: 60px;
  background: #F5F5F5;
  border-radius: 8px;
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 8px;
}

.floor-thumbnail img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.floor-placeholder {
  font-size: 24px;
  opacity: 0.4;
}

.floor-label {
  font-size: 12px;
  font-weight: 600;
  color: #1B1B1B;
  margin-bottom: 4px;
}

.floor-stats {
  display: flex;
  gap: 8px;
  font-size: 10px;
}

.floor-stats .stat {
  color: #757575;
}

.floor-stats .stat strong {
  display: block;
  font-size: 12px;
  color: #1B1B1B;
}

.add-floor-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  padding: 12px;
  background: transparent;
  border: 1px dashed rgba(0, 0, 0, 0.2);
  border-radius: 12px;
  color: #757575;
  font-size: 12px;
  cursor: pointer;
  transition: all 0.2s;
  margin-top: auto;
}

.add-floor-btn:hover {
  border-color: #FF5B3C;
  color: #FF5B3C;
}

/* Plan Canvas */
.plan-canvas {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 24px;
  overflow: auto;
}

.plan-canvas.is-drawing {
  padding-top: 80px;
}

.plan-container {
  position: relative;
  max-width: 100%;
  max-height: 100%;
}

.plan-image {
  max-width: 100%;
  max-height: calc(100vh - 200px);
  object-fit: contain;
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
  user-select: none;
}

.no-plan {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 80px;
  color: #757575;
}

.no-plan .icon {
  font-size: 64px;
  margin-bottom: 16px;
  opacity: 0.5;
}

/* Drawing Overlay */
.drawing-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
}

/* Unit Polygons Overlay */
.unit-polygons-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
}

.unit-polygon {
  cursor: pointer;
  transition: all 0.2s;
}

.unit-polygon:hover {
  fill-opacity: 0.6;
  stroke-width: 0.5;
}

.unit-label {
  cursor: pointer;
  pointer-events: all;
}

/* Unit Markers Overlay */
.unit-markers-overlay {
  position: absolute;
  inset: 0;
  pointer-events: none;
}

.unit-marker {
  position: absolute;
  width: 36px;
  height: 36px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  font-weight: 600;
  cursor: pointer;
  pointer-events: auto;
  transition: all 0.2s;
  transform: translate(-50%, -50%);
}

.unit-marker.available {
  background: #fff;
  border: 2px solid #1B1B1B;
  color: #1B1B1B;
}

.unit-marker.occupied {
  background: #FF5B3C;
  border: 2px solid #FF5B3C;
  color: #fff;
}

.unit-marker.selected {
  background: #FF5B3C;
  border: 2px solid #1B1B1B;
  color: #fff;
  transform: translate(-50%, -50%) scale(1.15);
  box-shadow: 0 4px 12px rgba(255, 91, 60, 0.4);
}

.unit-marker:hover {
  transform: translate(-50%, -50%) scale(1.1);
}

.unit-marker .check-icon {
  position: absolute;
  bottom: -4px;
  right: -4px;
  background: #22C55E;
  border-radius: 50%;
  padding: 2px;
}

/* Unit Panel */
.unit-panel {
  width: 320px;
  background: #fff;
  border-left: 1px solid rgba(0, 0, 0, 0.06);
  display: flex;
  flex-direction: column;
  animation: slideIn 0.2s ease;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateX(20px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

.panel-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px 20px;
  border-bottom: 1px solid rgba(0, 0, 0, 0.06);
}

.panel-header h3 {
  font-size: 16px;
  font-weight: 600;
  margin: 0;
}

.close-btn {
  width: 28px;
  height: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #F5F5F5;
  border: none;
  border-radius: 50%;
  font-size: 18px;
  cursor: pointer;
  color: #757575;
}

.close-btn:hover {
  background: #EBEBEB;
}

.panel-content {
  padding: 20px;
  flex: 1;
  overflow-y: auto;
}

/* Coordinates Preview */
.coordinates-preview {
  background: rgba(255, 91, 60, 0.1);
  border: 1px solid rgba(255, 91, 60, 0.3);
  border-radius: 10px;
  padding: 12px;
  margin-bottom: 16px;
}

.preview-label {
  font-size: 12px;
  font-weight: 600;
  color: #FF5B3C;
  margin-bottom: 4px;
}

.preview-points {
  font-size: 13px;
  color: #1B1B1B;
}

.status-badge {
  display: inline-block;
  padding: 6px 12px;
  border-radius: 100px;
  font-size: 12px;
  font-weight: 600;
  margin-bottom: 16px;
}

.status-badge.active {
  background: rgba(34, 197, 94, 0.15);
  color: #16a34a;
}

.status-badge.draft,
.status-badge.inactive {
  background: rgba(255, 91, 60, 0.15);
  color: #FF5B3C;
}

.info-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
  margin-bottom: 24px;
}

.info-item .label {
  display: block;
  font-size: 11px;
  color: #757575;
  margin-bottom: 4px;
}

.info-item .value {
  font-size: 14px;
  font-weight: 600;
  color: #1B1B1B;
}

.panel-actions {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

/* Form */
.form-group {
  margin-bottom: 16px;
}

.form-group label {
  display: block;
  font-size: 12px;
  color: #757575;
  margin-bottom: 6px;
}

.form-group input,
.form-group select {
  width: 100%;
  padding: 10px 12px;
  border: 1px solid rgba(0, 0, 0, 0.1);
  border-radius: 10px;
  font-size: 14px;
  background: #fff;
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: #FF5B3C;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
}

/* Buttons */
.btn {
  padding: 12px 16px;
  border-radius: 10px;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  border: none;
  transition: all 0.2s;
  width: 100%;
}

.btn-primary {
  background: #1B1B1B;
  color: #fff;
}

.btn-primary:hover:not(:disabled) {
  background: #333;
}

.btn-primary:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-secondary {
  background: #F5F5F5;
  color: #1B1B1B;
}

.btn-secondary:hover {
  background: #EBEBEB;
}

.btn-danger {
  background: rgba(239, 68, 68, 0.1);
  color: #dc2626;
}

.btn-danger:hover {
  background: rgba(239, 68, 68, 0.2);
}

/* Bottom Toolbar */
.bottom-toolbar {
  position: fixed;
  bottom: 24px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  align-items: center;
  gap: 4px;
  padding: 6px;
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
  background: transparent;
  border: none;
  border-radius: 100px;
  font-size: 13px;
  font-weight: 500;
  color: rgba(255, 255, 255, 0.7);
  cursor: pointer;
  transition: all 0.2s;
}

.toolbar-btn:hover {
  color: #fff;
  background: rgba(255, 255, 255, 0.1);
}

.toolbar-btn.primary {
  background: #FF5B3C;
  color: #fff;
}

.toolbar-btn.primary:hover {
  background: #E54B2E;
}
</style>
