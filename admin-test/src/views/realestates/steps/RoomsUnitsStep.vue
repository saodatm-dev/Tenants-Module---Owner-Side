<script setup lang="ts">
import { ref, computed } from 'vue'
import type { PropertyTypeConfig } from '@/configs/propertyTypeFlows'
import type { RoomType, Renovation } from '@/types/realestate'

interface RoomEntry {
  roomTypeId: string
  area: number
}

interface UnitEntry {
  totalArea: number
  floorNumber?: number
  renovationId?: string
  rooms: RoomEntry[]
  expanded: boolean
}

const props = defineProps<{
  propertyType: PropertyTypeConfig
  modelValue: {
    rooms: RoomEntry[]
    units: UnitEntry[]
  }
  roomTypes: RoomType[]
  renovations: Renovation[]
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: typeof props.modelValue): void
}>()

// Tab state for types that support both
const activeTab = ref<'rooms' | 'units'>(props.propertyType.canHaveUnits ? 'units' : 'rooms')

const showUnitsTab = computed(() => props.propertyType.canHaveUnits)

// Room management
function addRoom() {
  const newRooms = [...props.modelValue.rooms, { roomTypeId: '', area: 0 }]
  emit('update:modelValue', { ...props.modelValue, rooms: newRooms })
}

function updateRoom(index: number, field: keyof RoomEntry, value: string | number) {
  const newRooms = [...props.modelValue.rooms]
  newRooms[index] = { ...newRooms[index], [field]: value } as RoomEntry
  emit('update:modelValue', { ...props.modelValue, rooms: newRooms })
}

function removeRoom(index: number) {
  const newRooms = props.modelValue.rooms.filter((_, i) => i !== index)
  emit('update:modelValue', { ...props.modelValue, rooms: newRooms })
}

// Unit management
function addUnit() {
  const newUnit: UnitEntry = {
    totalArea: 0,
    floorNumber: undefined,
    renovationId: undefined,
    rooms: [],
    expanded: true
  }
  const newUnits = [...props.modelValue.units, newUnit]
  emit('update:modelValue', { ...props.modelValue, units: newUnits })
}

function updateUnit(index: number, field: keyof UnitEntry, value: number | string | boolean | undefined) {
  const newUnits = [...props.modelValue.units]
  newUnits[index] = { ...newUnits[index], [field]: value } as UnitEntry
  emit('update:modelValue', { ...props.modelValue, units: newUnits })
}

function removeUnit(index: number) {
  const newUnits = props.modelValue.units.filter((_, i) => i !== index)
  emit('update:modelValue', { ...props.modelValue, units: newUnits })
}

function toggleUnit(index: number) {
  const newUnits = [...props.modelValue.units]
  const currentUnit = newUnits[index]
  if (currentUnit) {
    newUnits[index] = { ...currentUnit, expanded: !currentUnit.expanded }
  }
  emit('update:modelValue', { ...props.modelValue, units: newUnits })
}

function addRoomToUnit(unitIndex: number) {
  const newUnits = [...props.modelValue.units]
  const unit = newUnits[unitIndex]
  if (unit) {
    unit.rooms = [...unit.rooms, { roomTypeId: '', area: 0 }]
  }
  emit('update:modelValue', { ...props.modelValue, units: newUnits })
}

function updateUnitRoom(unitIndex: number, roomIndex: number, field: keyof RoomEntry, value: string | number) {
  const newUnits = [...props.modelValue.units]
  const unit = newUnits[unitIndex]
  if (unit && unit.rooms[roomIndex]) {
    unit.rooms[roomIndex] = { ...unit.rooms[roomIndex], [field]: value } as RoomEntry
  }
  emit('update:modelValue', { ...props.modelValue, units: newUnits })
}

function removeRoomFromUnit(unitIndex: number, roomIndex: number) {
  const newUnits = [...props.modelValue.units]
  const unit = newUnits[unitIndex]
  if (unit) {
    unit.rooms = unit.rooms.filter((_, i) => i !== roomIndex)
  }
  emit('update:modelValue', { ...props.modelValue, units: newUnits })
}
</script>

<template>
  <div class="rooms-units-step">
    <div class="step-header">
      <h2>
        {{ showUnitsTab ? 'Add rooms or units' : 'Add rooms to your property' }}
      </h2>
      <p class="subtitle">
        {{ showUnitsTab
          ? 'You can add individual rooms or create separate units'
          : 'Define the rooms in your property (optional)'
        }}
      </p>
    </div>

    <!-- Tab Navigation (only for unit-capable types) -->
    <div v-if="showUnitsTab" class="tabs-nav">
      <button
        class="tab-btn"
        :class="{ active: activeTab === 'rooms' }"
        @click="activeTab = 'rooms'"
      >
        🏠 Rooms
      </button>
      <button
        class="tab-btn"
        :class="{ active: activeTab === 'units' }"
        @click="activeTab = 'units'"
      >
        🏢 Units
      </button>
    </div>

    <div class="content-section">
      <!-- Rooms Tab -->
      <div v-if="activeTab === 'rooms' || !showUnitsTab" class="rooms-section">
        <div v-if="modelValue.rooms.length === 0" class="empty-state">
          <span class="empty-icon">🛋️</span>
          <p>No rooms added yet</p>
          <button class="btn-add-first" @click="addRoom">+ Add your first room</button>
        </div>

        <div v-else class="rooms-list">
          <div v-for="(room, index) in modelValue.rooms" :key="index" class="room-card">
            <div class="room-fields">
              <select
                :value="room.roomTypeId"
                @change="updateRoom(index, 'roomTypeId', ($event.target as HTMLSelectElement).value)"
              >
                <option value="">Select room type...</option>
                <option v-for="rt in roomTypes" :key="rt.id" :value="rt.id">
                  {{ rt.name }}
                </option>
              </select>
              <input
                type="number"
                :value="room.area"
                @input="updateRoom(index, 'area', parseFloat(($event.target as HTMLInputElement).value) || 0)"
                placeholder="Area (m²)"
                min="0"
                step="0.1"
              />
            </div>
            <button class="btn-remove" @click="removeRoom(index)">×</button>
          </div>
        </div>

        <button v-if="modelValue.rooms.length > 0" class="btn-add" @click="addRoom">
          + Add another room
        </button>
      </div>

      <!-- Units Tab -->
      <div v-if="activeTab === 'units' && showUnitsTab" class="units-section">
        <div v-if="modelValue.units.length === 0" class="empty-state">
          <span class="empty-icon">🏢</span>
          <p>No units added yet</p>
          <button class="btn-add-first" @click="addUnit">+ Add your first unit</button>
        </div>

        <div v-else class="units-list">
          <div v-for="(unit, unitIndex) in modelValue.units" :key="unitIndex" class="unit-card">
            <div class="unit-header" @click="toggleUnit(unitIndex)">
              <span class="expand-icon">{{ unit.expanded ? '▼' : '▶' }}</span>
              <span class="unit-title">Unit {{ unitIndex + 1 }}</span>
              <span v-if="unit.totalArea > 0" class="unit-badge">{{ unit.totalArea }} m²</span>
              <button class="btn-remove" @click.stop="removeUnit(unitIndex)">×</button>
            </div>

            <div v-if="unit.expanded" class="unit-content">
              <div class="unit-fields">
                <div class="form-group">
                  <label>Area (m²)</label>
                  <input
                    type="number"
                    :value="unit.totalArea"
                    @input="updateUnit(unitIndex, 'totalArea', parseFloat(($event.target as HTMLInputElement).value) || 0)"
                    min="0"
                    step="0.1"
                  />
                </div>
                <div class="form-group">
                  <label>Floor</label>
                  <input
                    type="number"
                    :value="unit.floorNumber"
                    @input="updateUnit(unitIndex, 'floorNumber', parseInt(($event.target as HTMLInputElement).value) || undefined)"
                    min="0"
                  />
                </div>
                <div class="form-group">
                  <label>Renovation</label>
                  <select
                    :value="unit.renovationId"
                    @change="updateUnit(unitIndex, 'renovationId', ($event.target as HTMLSelectElement).value || undefined)"
                  >
                    <option value="">Not specified</option>
                    <option v-for="ren in renovations" :key="ren.id" :value="ren.id">
                      {{ ren.name }}
                    </option>
                  </select>
                </div>
              </div>

              <!-- Unit Rooms -->
              <div class="unit-rooms">
                <h4>Rooms in this unit</h4>
                <div class="rooms-list compact">
                  <div v-for="(room, roomIndex) in unit.rooms" :key="roomIndex" class="room-card compact">
                    <select
                      :value="room.roomTypeId"
                      @change="updateUnitRoom(unitIndex, roomIndex, 'roomTypeId', ($event.target as HTMLSelectElement).value)"
                    >
                      <option value="">Room type...</option>
                      <option v-for="rt in roomTypes" :key="rt.id" :value="rt.id">
                        {{ rt.name }}
                      </option>
                    </select>
                    <input
                      type="number"
                      :value="room.area"
                      @input="updateUnitRoom(unitIndex, roomIndex, 'area', parseFloat(($event.target as HTMLInputElement).value) || 0)"
                      placeholder="m²"
                      min="0"
                      step="0.1"
                    />
                    <button class="btn-remove-sm" @click="removeRoomFromUnit(unitIndex, roomIndex)">×</button>
                  </div>
                </div>
                <button class="btn-add-sm" @click="addRoomToUnit(unitIndex)">+ Add room</button>
              </div>
            </div>
          </div>
        </div>

        <button v-if="modelValue.units.length > 0" class="btn-add" @click="addUnit">
          + Add another unit
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.rooms-units-step {
  padding: 20px 0;
}

.step-header {
  text-align: center;
  margin-bottom: 24px;
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

/* Tabs */
.tabs-nav {
  display: flex;
  gap: 8px;
  padding: 6px;
  background: #f3f4f6;
  border-radius: 12px;
  margin-bottom: 24px;
  max-width: 300px;
  margin-left: auto;
  margin-right: auto;
}

.tab-btn {
  flex: 1;
  padding: 12px 20px;
  background: transparent;
  border: none;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  color: #6b7280;
  cursor: pointer;
  transition: all 0.2s;
}

.tab-btn:hover {
  color: #FF5B3C;
}

.tab-btn.active {
  background: #fff;
  color: #FF5B3C;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
}

/* Content */
.content-section {
  max-width: 600px;
  margin: 0 auto;
}

/* Empty State */
.empty-state {
  text-align: center;
  padding: 48px 24px;
  background: #f9fafb;
  border-radius: 16px;
  border: 2px dashed #e5e7eb;
}

.empty-icon {
  font-size: 48px;
  display: block;
  margin-bottom: 16px;
}

.empty-state p {
  color: #6b7280;
  margin: 0 0 20px 0;
}

.btn-add-first {
  padding: 14px 28px;
  background: linear-gradient(135deg, #FF5B3C, #FF7B5C);
  color: #fff;
  border: none;
  border-radius: 12px;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
}

.btn-add-first:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(255, 91, 60, 0.3);
}

/* Room/Unit Cards */
.rooms-list, .units-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-bottom: 16px;
}

.room-card {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 16px;
  background: #fff;
  border: 2px solid #e5e7eb;
  border-radius: 12px;
  transition: border-color 0.2s;
}

.room-card:hover {
  border-color: #a5b4fc;
}

.room-card.compact {
  padding: 10px;
  gap: 8px;
}

.room-fields {
  display: flex;
  gap: 12px;
  flex: 1;
}

.room-fields select,
.room-fields input {
  padding: 12px 14px;
  border: 2px solid #e5e7eb;
  border-radius: 10px;
  font-size: 14px;
}

.room-fields select {
  flex: 2;
}

.room-fields input {
  flex: 1;
  width: 100px;
}

.unit-card {
  background: #fff;
  border: 2px solid #e5e7eb;
  border-radius: 16px;
  overflow: hidden;
}

.unit-header {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 16px 20px;
  background: #f9fafb;
  cursor: pointer;
  transition: background 0.2s;
}

.unit-header:hover {
  background: #f3f4f6;
}

.expand-icon {
  color: #6b7280;
  font-size: 12px;
}

.unit-title {
  font-weight: 600;
  color: #1a1a2e;
  flex: 1;
}

.unit-badge {
  background: #ffe8e4;
  color: #FF5B3C;
  padding: 4px 10px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 600;
}

.unit-content {
  padding: 20px;
  border-top: 1px solid #e5e7eb;
}

.unit-fields {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 16px;
  margin-bottom: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.form-group label {
  font-size: 12px;
  font-weight: 600;
  color: #6b7280;
}

.form-group input,
.form-group select {
  padding: 10px 12px;
  border: 2px solid #e5e7eb;
  border-radius: 8px;
  font-size: 14px;
}

.unit-rooms h4 {
  font-size: 13px;
  font-weight: 600;
  color: #6b7280;
  margin: 0 0 12px 0;
}

.rooms-list.compact {
  gap: 8px;
}

.room-card.compact select,
.room-card.compact input {
  padding: 8px 10px;
  font-size: 13px;
}

/* Buttons */
.btn-remove {
  width: 32px;
  height: 32px;
  border: none;
  background: #fee2e2;
  color: #dc2626;
  border-radius: 8px;
  font-size: 18px;
  cursor: pointer;
  transition: background 0.2s;
}

.btn-remove:hover {
  background: #fecaca;
}

.btn-remove-sm {
  width: 24px;
  height: 24px;
  border: none;
  background: #fee2e2;
  color: #dc2626;
  border-radius: 6px;
  font-size: 14px;
  cursor: pointer;
}

.btn-add {
  width: 100%;
  padding: 14px;
  background: #f3f4f6;
  border: 2px dashed #d1d5db;
  border-radius: 12px;
  font-size: 14px;
  font-weight: 600;
  color: #6b7280;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-add:hover {
  background: #e5e7eb;
  border-color: #9ca3af;
  color: #4b5563;
}

.btn-add-sm {
  padding: 8px 14px;
  background: #f3f4f6;
  border: 1px dashed #d1d5db;
  border-radius: 8px;
  font-size: 12px;
  font-weight: 600;
  color: #6b7280;
  cursor: pointer;
}

@media (max-width: 600px) {
  .unit-fields {
    grid-template-columns: 1fr;
  }

  .room-fields {
    flex-direction: column;
  }
}
</style>
