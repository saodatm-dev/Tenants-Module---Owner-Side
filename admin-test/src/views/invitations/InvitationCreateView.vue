<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { api } from '@/services/api'
import type { CreateInvitationCommand, UpdateInvitationCommand, Invitation } from '@/types/invitation'

const route = useRoute()
const router = useRouter()

// Check if we're in edit mode
const isEditMode = computed(() => route.name === 'invitation-edit')
const invitationId = computed(() => route.params.id as string | undefined)

// Form state
const form = ref<{
  phoneNumber: string
  roleId: string
}>({
  phoneNumber: '',
  roleId: ''
})

// Original invitation data for edit mode
const originalInvitation = ref<Invitation | null>(null)

const loading = ref(false)
const saving = ref(false)
const error = ref('')

// TODO: Fetch roles from API if available
const roles = ref<{ id: string; name: string }[]>([
  // Placeholder - replace with API call if roles endpoint exists
])

// Load invitation data if in edit mode
async function loadInvitation() {
  if (!isEditMode.value || !invitationId.value) return

  loading.value = true
  error.value = ''
  try {
    const invitation = await api.getInvitationById(invitationId.value)
    originalInvitation.value = invitation
    form.value = {
      phoneNumber: invitation.phoneNumber || '',
      roleId: '' // Role ID is not returned in GET, only roleName
    }
  } catch (e: unknown) {
    const err = e as Error
    error.value = err.message || 'Error loading invitation'
    console.error('Failed to load invitation:', e)
  } finally {
    loading.value = false
  }
}

// Form validation
const isValid = computed(() => {
  if (isEditMode.value) {
    return form.value.phoneNumber.trim() !== ''
  }
  return form.value.phoneNumber.trim() !== '' && form.value.roleId.trim() !== ''
})

// Submit form
async function handleSubmit() {
  if (!isValid.value) {
    if (!isEditMode.value && !form.value.roleId) {
      alert('Please enter a role ID')
      return
    }
    alert('Please enter a phone number')
    return
  }

  saving.value = true
  error.value = ''

  try {
    if (isEditMode.value && invitationId.value) {
      // Update existing invitation
      const data: UpdateInvitationCommand = {
        id: invitationId.value,
        phoneNumber: form.value.phoneNumber
      }
      await api.updateInvitation(data)
      router.push(`/invitations/${invitationId.value}`)
    } else {
      // Create new invitation
      const data: CreateInvitationCommand = {
        phoneNumber: form.value.phoneNumber,
        roleId: form.value.roleId
      }
      const newId = await api.createInvitation(data)
      router.push(`/invitations/${newId}`)
    }
  } catch (e: unknown) {
    const err = e as Error
    error.value = err.message || 'Error saving invitation'
    console.error('Failed to save invitation:', e)
  } finally {
    saving.value = false
  }
}

function handleCancel() {
  if (isEditMode.value && invitationId.value) {
    router.push(`/invitations/${invitationId.value}`)
  } else {
    router.push('/invitations')
  }
}

onMounted(() => {
  loadInvitation()
})
</script>

<template>
  <div class="invitation-create-view">
    <!-- Back button -->
    <button class="btn btn-back" @click="handleCancel">
      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <path d="M19 12H5M12 19l-7-7 7-7"/>
      </svg>
      {{ isEditMode ? 'Cancel' : 'Back to Invitations' }}
    </button>

    <div class="form-card">
      <div class="form-header">
        <h1>{{ isEditMode ? 'Edit Invitation' : 'Create Invitation' }}</h1>
        <p class="subtitle">{{ isEditMode ? 'Update invitation details' : 'Send a new invitation' }}</p>
      </div>

      <!-- Loading state -->
      <div v-if="loading" class="loading-state">
        <div class="spinner"></div>
        <p>Loading invitation...</p>
      </div>

      <!-- Form -->
      <form v-else @submit.prevent="handleSubmit" class="invitation-form">
        <!-- Error message -->
        <div v-if="error" class="error-banner">
          {{ error }}
        </div>

        <div class="form-group">
          <label for="phoneNumber">Phone Number <span class="required">*</span></label>
          <input
            id="phoneNumber"
            v-model="form.phoneNumber"
            type="tel"
            placeholder="+998 90 123 45 67"
            :disabled="saving"
            required
          />
          <span class="help-text">Phone number to send the invitation to</span>
        </div>

        <div v-if="!isEditMode" class="form-group">
          <label for="roleId">Role ID <span class="required">*</span></label>
          <input
            id="roleId"
            v-model="form.roleId"
            type="text"
            placeholder="e.g., 3fa85f64-5717-4562-b3fc-2c963f66afa6"
            :disabled="saving"
            required
          />
          <span class="help-text">GUID of the role to assign to the invited user</span>
        </div>

        <!-- Show current role name in edit mode -->
        <div v-if="isEditMode && originalInvitation?.roleName" class="form-group">
          <label>Current Role</label>
          <div class="readonly-value">{{ originalInvitation.roleName }}</div>
          <span class="help-text">Role cannot be changed after creation</span>
        </div>

        <div class="form-actions">
          <button type="button" class="btn btn-secondary" @click="handleCancel" :disabled="saving">
            Cancel
          </button>
          <button type="submit" class="btn btn-primary" :disabled="saving || !isValid">
            <span v-if="saving" class="btn-spinner"></span>
            {{ saving ? 'Saving...' : (isEditMode ? 'Update Invitation' : 'Create Invitation') }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped>
.invitation-create-view {
  padding: 20px 24px;
  max-width: 600px;
  margin: 0 auto;
}

.btn-back {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 10px 16px;
  background: transparent;
  border: none;
  color: rgba(27, 27, 27, 0.7);
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  margin-bottom: 24px;
  border-radius: 8px;
  transition: all 0.2s;
}

.btn-back:hover {
  background: rgba(27, 27, 27, 0.04);
  color: #1B1B1B;
}

.form-card {
  background: #FFFFFF;
  border-radius: 24px;
  padding: 32px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
}

.form-header {
  margin-bottom: 32px;
}

.form-header h1 {
  font-size: 24px;
  font-weight: 700;
  color: #1B1B1B;
  margin: 0 0 4px 0;
}

.subtitle {
  color: rgba(27, 27, 27, 0.5);
  margin: 0;
  font-size: 14px;
}

.loading-state {
  text-align: center;
  padding: 40px 20px;
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

.error-banner {
  background: rgba(244, 67, 54, 0.1);
  color: #F44336;
  padding: 12px 16px;
  border-radius: 8px;
  margin-bottom: 24px;
  font-size: 14px;
}

.invitation-form {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.form-group label {
  font-size: 14px;
  font-weight: 600;
  color: #1B1B1B;
}

.required {
  color: #F44336;
}

.form-group input {
  padding: 12px 16px;
  border: 1px solid rgba(27, 27, 27, 0.12);
  border-radius: 12px;
  font-size: 15px;
  background: #FAFAFA;
  color: #1B1B1B;
  transition: all 0.2s;
}

.form-group input:focus {
  outline: none;
  border-color: #FF5B3C;
  background: #FFFFFF;
  box-shadow: 0 0 0 3px rgba(255, 91, 60, 0.1);
}

.form-group input:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.readonly-value {
  padding: 12px 16px;
  background: #F0F0F0;
  border-radius: 12px;
  font-size: 15px;
  color: #666;
}

.help-text {
  font-size: 12px;
  color: rgba(27, 27, 27, 0.5);
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  margin-top: 8px;
  padding-top: 24px;
  border-top: 1px solid rgba(27, 27, 27, 0.08);
}

.btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  padding: 12px 24px;
  border-radius: 100px;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  border: none;
  transition: all 0.2s;
}

.btn-primary {
  background: #1B1B1B;
  color: #fff;
  min-width: 160px;
}

.btn-primary:hover:not(:disabled) {
  background: #333;
}

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-secondary {
  background: #F7F7F7;
  color: #1B1B1B;
}

.btn-secondary:hover:not(:disabled) {
  background: #EBEBEB;
}

.btn-spinner {
  width: 16px;
  height: 16px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top-color: #fff;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

@media (max-width: 480px) {
  .form-actions {
    flex-direction: column-reverse;
  }

  .btn {
    width: 100%;
  }
}
</style>
