<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { api } from '@/services/api'
import type { Meter, MeterType } from '@/types/meter'

const props = defineProps<{
  meter: Meter | null
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
  realEstateId: '',
  meterTypeId: '',
  initialReading: '',
  serialNumber: '',
  installationDate: '',
  verificationDate: '',
  nextVerificationDate: ''
})

onMounted(() => {
  if (props.meter) {
    form.value = {
      realEstateId: props.selectedPropertyId || '',
      meterTypeId: props.meter.meterTypeId,
      initialReading: props.meter.initialReading?.toString() || '',
      serialNumber: props.meter.serialNumber || '',
      installationDate: props.meter.installationDate || '',
      verificationDate: props.meter.verificationDate || '',
      nextVerificationDate: props.meter.nextVerificationDate || ''
    }
  } else if (props.selectedPropertyId) {
    form.value.realEstateId = props.selectedPropertyId
  }
})

async function handleSubmit() {
  if (!props.meter && (!form.value.realEstateId || !form.value.meterTypeId)) {
    errorMsg.value = 'Please select a property and meter type'
    return
  }
  if (props.meter && !form.value.meterTypeId) {
    errorMsg.value = 'Please select a meter type'
    return
  }

  saving.value = true
  errorMsg.value = ''

  try {
    if (props.meter) {
      // Edit mode
      await api.updateMeter({
        id: props.meter.id,
        meterTypeId: form.value.meterTypeId,
        serialNumber: form.value.serialNumber || undefined,
        installationDate: form.value.installationDate || undefined,
        verificationDate: form.value.verificationDate || undefined,
        nextVerificationDate: form.value.nextVerificationDate || undefined,
        initialReading: form.value.initialReading ? parseFloat(form.value.initialReading) : undefined
      })
    } else {
      // Create mode
      await api.createMeter({
        realEstateId: form.value.realEstateId,
        meterTypeId: form.value.meterTypeId,
        initialReading: form.value.initialReading ? parseFloat(form.value.initialReading) : undefined,
        serialNumber: form.value.serialNumber || undefined,
        installationDate: form.value.installationDate || undefined,
        verificationDate: form.value.verificationDate || undefined,
        nextVerificationDate: form.value.nextVerificationDate || undefined
      })
    }

    emit('saved')
  } catch (e: unknown) {
    errorMsg.value = (e as Error).message || 'Failed to save meter'
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <div class="modal-overlay" @click.self="emit('close')">
    <div class="modal-panel">
      <div class="modal-header">
        <h2>{{ meter ? 'Edit Meter' : 'Add Meter' }}</h2>
        <button class="close-btn" @click="emit('close')">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M18 6L6 18M6 6l12 12"/>
          </svg>
        </button>
      </div>

      <form class="modal-body" @submit.prevent="handleSubmit">
        <div v-if="errorMsg" class="error-banner">{{ errorMsg }}</div>

        <div v-if="!meter" class="form-group">
          <label>Property *</label>
          <select v-model="form.realEstateId" required>
            <option value="" disabled>Select property</option>
            <option v-for="p in properties" :key="p.id" :value="p.id">
              {{ p.number || p.address || p.id.substring(0, 8) }}
            </option>
          </select>
        </div>

        <div class="form-group">
          <label>Meter Type *</label>
          <select v-model="form.meterTypeId" required>
            <option value="" disabled>Select type</option>
            <option v-for="mt in meterTypes" :key="mt.id" :value="mt.id">
              {{ mt.name }}
            </option>
          </select>
        </div>

        <div class="form-group">
          <label>Initial Reading</label>
          <input
            v-model="form.initialReading"
            type="number"
            step="0.01"
            min="0"
            placeholder="0.00"
          />
        </div>

        <div class="form-group">
          <label>Serial Number</label>
          <input
            v-model="form.serialNumber"
            type="text"
            placeholder="Optional"
          />
        </div>

        <div class="form-group">
          <label>Installation Date</label>
          <input
            v-model="form.installationDate"
            type="date"
          />
        </div>

        <div class="form-row">
          <div class="form-group">
            <label>Verification Date</label>
            <input
              v-model="form.verificationDate"
              type="date"
            />
          </div>
          <div class="form-group">
            <label>Next Verification</label>
            <input
              v-model="form.nextVerificationDate"
              type="date"
            />
          </div>
        </div>

        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" @click="emit('close')">Cancel</button>
          <button type="submit" class="btn btn-primary" :disabled="saving">
            {{ saving ? 'Saving...' : (meter ? 'Update' : 'Create') }}
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
  max-width: 480px;
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

.form-row {
  display: flex;
  gap: 12px;
}

.form-row .form-group {
  flex: 1;
}

.form-group select,
.form-group input {
  width: 100%;
  padding: 12px 14px;
  border: 1px solid rgba(27, 27, 27, 0.1);
  border-radius: 10px;
  font-size: 14px;
  background: #FAFAFA;
  color: #1B1B1B;
  transition: all 0.2s;
  box-sizing: border-box;
}

.form-group select:focus,
.form-group input:focus {
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
</style>
