<script setup lang="ts">
import { ref } from 'vue'
import { api } from '@/services/api'
import type { Meter, MeterType } from '@/types/meter'

const props = defineProps<{
  meters: Meter[]
  meterTypes: MeterType[]
  properties: { id: string; number?: string; address?: string }[]
  selectedPropertyId?: string
}>()

const emit = defineEmits<{
  close: []
  saved: []
}>()

const saving = ref(false)
const errorMsg = ref('')

const form = ref({
  meterId: '',
  value: '',
  previousValue: '',
  readingDate: new Date().toISOString().split('T')[0],
  isManual: true,
  note: ''
})

// Not needed — meters already have meterName from backend
// function getMeterTypeName(typeId: string): string {
//   return props.meterTypes.find(t => t.id === typeId)?.name || 'Unknown'
// }

async function handleSubmit() {
  if (!form.value.meterId || !form.value.value) {
    errorMsg.value = 'Please select a meter and enter the current reading'
    return
  }

  saving.value = true
  errorMsg.value = ''

  try {
    await api.upsertMeterReading({
      meterId: form.value.meterId,
      value: parseFloat(form.value.value),
      previousValue: form.value.previousValue ? parseFloat(form.value.previousValue) : undefined,
      readingDate: form.value.readingDate || undefined,
      isManual: form.value.isManual,
      note: form.value.note || undefined
    })
    emit('saved')
  } catch (e: unknown) {
    errorMsg.value = (e as Error).message || 'Failed to submit reading'
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <div class="modal-overlay" @click.self="emit('close')">
    <div class="modal-panel">
      <div class="modal-header">
        <h2>Submit Meter Reading</h2>
        <button class="close-btn" @click="emit('close')">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M18 6L6 18M6 6l12 12"/>
          </svg>
        </button>
      </div>

      <form class="modal-body" @submit.prevent="handleSubmit">
        <div v-if="errorMsg" class="error-banner">{{ errorMsg }}</div>

        <div class="form-group">
          <label>Meter *</label>
          <select v-model="form.meterId" required>
            <option value="" disabled>Select meter</option>
            <option v-for="m in meters" :key="m.id" :value="m.id">
              {{ m.meterName }} — {{ m.serialNumber }}
            </option>
          </select>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label>Previous Reading</label>
            <input
              v-model="form.previousValue"
              type="number"
              step="0.01"
              min="0"
              placeholder="Optional"
            />
          </div>

          <div class="form-group">
            <label>Current Reading *</label>
            <input
              v-model="form.value"
              type="number"
              step="0.01"
              min="0"
              placeholder="0.00"
              required
            />
          </div>
        </div>

        <div class="form-group">
          <label>Reading Date</label>
          <input
            v-model="form.readingDate"
            type="date"
          />
        </div>

        <div class="form-group">
          <label>Note</label>
          <textarea
            v-model="form.note"
            rows="2"
            placeholder="Optional note..."
          ></textarea>
        </div>

        <div class="form-group toggle-group">
          <label class="toggle-label">
            <span>Manual Reading</span>
            <button
              type="button"
              class="toggle-btn"
              :class="{ active: form.isManual }"
              @click="form.isManual = !form.isManual"
            >
              <span class="toggle-knob"></span>
            </button>
          </label>
        </div>

        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" @click="emit('close')">Cancel</button>
          <button type="submit" class="btn btn-primary" :disabled="saving">
            {{ saving ? 'Saving...' : 'Submit Reading' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  backdrop-filter: blur(4px);
}

.modal-panel {
  background: #FFFFFF;
  border-radius: 24px;
  width: 100%;
  max-width: 520px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 24px 80px rgba(0, 0, 0, 0.15);
  animation: slideUp 0.25s ease-out;
}

@keyframes slideUp {
  from { opacity: 0; transform: translateY(20px); }
  to { opacity: 1; transform: translateY(0); }
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 24px 24px 0;
}

.modal-header h2 {
  font-size: 20px;
  font-weight: 700;
  color: #1B1B1B;
  margin: 0;
}

.close-btn {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  border: none;
  background: #F7F7F7;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  color: rgba(27, 27, 27, 0.5);
  transition: all 0.2s;
}

.close-btn:hover {
  background: #EBEBEB;
  color: #1B1B1B;
}

.modal-body {
  padding: 24px;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  font-size: 13px;
  font-weight: 600;
  color: rgba(27, 27, 27, 0.6);
  margin-bottom: 8px;
}

.form-group select,
.form-group input,
.form-group textarea {
  width: 100%;
  padding: 12px 14px;
  border: 1px solid rgba(27, 27, 27, 0.1);
  border-radius: 10px;
  font-size: 14px;
  background: #FAFAFA;
  color: #1B1B1B;
  transition: all 0.2s;
  box-sizing: border-box;
  font-family: inherit;
}

.form-group textarea {
  resize: vertical;
  min-height: 60px;
}

.form-group select:focus,
.form-group input:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #FF5B3C;
  box-shadow: 0 0 0 3px rgba(255, 91, 60, 0.1);
  background: #FFFFFF;
}

.error-banner {
  background: rgba(239, 68, 68, 0.08);
  color: #DC2626;
  padding: 12px 16px;
  border-radius: 10px;
  font-size: 13px;
  margin-bottom: 20px;
}

.modal-footer {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
  padding-top: 8px;
}

.btn {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 10px 20px;
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

.btn-primary:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-secondary {
  background: #F7F7F7;
  color: #1B1B1B;
}

.btn-secondary:hover {
  background: #EBEBEB;
}

.toggle-group {
  margin-bottom: 12px;
}

.toggle-label {
  display: flex;
  align-items: center;
  justify-content: space-between;
  cursor: pointer;
}

.toggle-label span {
  font-size: 13px;
  font-weight: 600;
  color: rgba(27, 27, 27, 0.6);
}

.toggle-btn {
  position: relative;
  width: 44px;
  height: 24px;
  border-radius: 12px;
  border: none;
  background: rgba(27, 27, 27, 0.12);
  cursor: pointer;
  transition: background 0.2s;
  padding: 0;
}

.toggle-btn.active {
  background: #FF5B3C;
}

.toggle-knob {
  position: absolute;
  top: 2px;
  left: 2px;
  width: 20px;
  height: 20px;
  border-radius: 50%;
  background: white;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.2);
  transition: transform 0.2s;
}

.toggle-btn.active .toggle-knob {
  transform: translateX(20px);
}
</style>
