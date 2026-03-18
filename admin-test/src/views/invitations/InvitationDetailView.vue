<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { api } from '@/services/api'
import type { Invitation } from '@/types/invitation'
import { InvitationStatus, getInvitationStatusLabel, getInvitationStatusColor } from '@/types/invitation'

const route = useRoute()
const router = useRouter()

const invitation = ref<Invitation | null>(null)
const loading = ref(true)
const error = ref('')

const invitationId = computed(() => route.params.id as string)

async function loadInvitation() {
  loading.value = true
  error.value = ''
  try {
    invitation.value = await api.getInvitationById(invitationId.value)
  } catch (e: unknown) {
    const err = e as Error
    error.value = err.message || 'Error loading invitation'
    console.error('Failed to load invitation:', e)
  } finally {
    loading.value = false
  }
}

function formatDate(dateStr?: string): string {
  if (!dateStr) return '-'
  return new Date(dateStr).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

function navigateBack() {
  router.push('/invitations')
}

// Actions
async function handleAccept() {
  if (!confirm('Accept this invitation?')) return
  try {
    await api.acceptInvitation(invitationId.value)
    await loadInvitation()
  } catch (e: unknown) {
    const err = e as Error
    alert(err.message || 'Error accepting invitation')
  }
}

async function handleCancel() {
  if (!confirm('Cancel this invitation?')) return
  try {
    await api.cancelInvitation(invitationId.value)
    await loadInvitation()
  } catch (e: unknown) {
    const err = e as Error
    alert(err.message || 'Error cancelling invitation')
  }
}

async function handleReject() {
  const reason = prompt('Enter rejection reason (optional):')
  if (reason === null) return // User cancelled prompt
  try {
    await api.rejectInvitation({ id: invitationId.value, reason: reason || undefined })
    await loadInvitation()
  } catch (e: unknown) {
    const err = e as Error
    alert(err.message || 'Error rejecting invitation')
  }
}

async function handleDelete() {
  if (!confirm('Are you sure you want to delete this invitation?')) return
  try {
    await api.deleteInvitation(invitationId.value)
    router.push('/invitations')
  } catch (e: unknown) {
    const err = e as Error
    alert(err.message || 'Delete error')
  }
}

// Check if action is available based on status
function canAccept(): boolean {
  return invitation.value?.status === InvitationStatus.Sent || invitation.value?.status === InvitationStatus.Received
}

function canCancel(): boolean {
  return invitation.value?.status === InvitationStatus.Sent || invitation.value?.status === InvitationStatus.Received
}

function canReject(): boolean {
  return invitation.value?.status === InvitationStatus.Sent || invitation.value?.status === InvitationStatus.Received
}

onMounted(() => {
  loadInvitation()
})
</script>

<template>
  <div class="invitation-detail-view">
    <!-- Back button -->
    <button class="btn btn-back" @click="navigateBack">
      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <path d="M19 12H5M12 19l-7-7 7-7"/>
      </svg>
      Back to Invitations
    </button>

    <!-- Loading state -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Loading invitation...</p>
    </div>

    <!-- Error state -->
    <div v-else-if="error" class="error-state">
      <p>{{ error }}</p>
      <button class="btn btn-secondary" @click="loadInvitation">Retry</button>
    </div>

    <!-- Invitation details -->
    <div v-else-if="invitation" class="detail-card">
      <div class="detail-header">
        <div class="header-info">
          <h1>Invitation Details</h1>
          <span :class="['status-badge', getInvitationStatusColor(invitation.status)]">
            {{ getInvitationStatusLabel(invitation.status) }}
          </span>
        </div>
        <div class="header-actions">
          <button v-if="canAccept()" class="btn btn-success" @click="handleAccept">
            Accept
          </button>
          <button v-if="canReject()" class="btn btn-danger" @click="handleReject">
            Reject
          </button>
          <button v-if="canCancel()" class="btn btn-secondary" @click="handleCancel">
            Cancel
          </button>
          <button class="btn btn-danger-outline" @click="handleDelete">
            Delete
          </button>
        </div>
      </div>

      <div class="detail-content">
        <div class="detail-section">
          <h3>Recipient</h3>
          <div class="detail-grid">
            <div class="detail-item">
              <label>Phone Number</label>
              <span>{{ invitation.phoneNumber }}</span>
            </div>
            <div class="detail-item" v-if="invitation.fullName">
              <label>Full Name</label>
              <span>{{ invitation.fullName }}</span>
            </div>
            <div class="detail-item" v-if="invitation.roleName">
              <label>Role</label>
              <span>{{ invitation.roleName }}</span>
            </div>
          </div>
        </div>

        <div class="detail-section">
          <h3>Dates</h3>
          <div class="detail-grid">
            <div class="detail-item" v-if="invitation.expiredTime">
              <label>Expires</label>
              <span>{{ formatDate(invitation.expiredTime) }}</span>
            </div>
          </div>
        </div>

        <div class="detail-section" v-if="invitation.reason">
          <h3>Rejection Reason</h3>
          <p class="reason-text">{{ invitation.reason }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.invitation-detail-view {
  padding: 20px 24px;
  max-width: 800px;
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

.loading-state,
.error-state {
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

.detail-card {
  background: #FFFFFF;
  border-radius: 24px;
  padding: 32px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
}

.detail-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 32px;
  padding-bottom: 24px;
  border-bottom: 1px solid rgba(27, 27, 27, 0.08);
}

.header-info {
  display: flex;
  align-items: center;
  gap: 16px;
}

.header-info h1 {
  font-size: 24px;
  font-weight: 700;
  color: #1B1B1B;
  margin: 0;
}

.header-actions {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.btn {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 10px 18px;
  border-radius: 100px;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  border: none;
  transition: all 0.2s;
}

.btn-secondary {
  background: #F7F7F7;
  color: #1B1B1B;
}

.btn-secondary:hover {
  background: #EBEBEB;
}

.btn-success {
  background: #4CAF50;
  color: #fff;
}

.btn-success:hover {
  background: #45A049;
}

.btn-danger {
  background: #F44336;
  color: #fff;
}

.btn-danger:hover {
  background: #D32F2F;
}

.btn-danger-outline {
  background: transparent;
  color: #F44336;
  border: 1px solid #F44336;
}

.btn-danger-outline:hover {
  background: rgba(244, 67, 54, 0.1);
}

.status-badge {
  padding: 6px 16px;
  border-radius: 100px;
  font-size: 13px;
  font-weight: 600;
}

.status-sent {
  background: rgba(33, 150, 243, 0.1);
  color: #2196F3;
}

.status-received {
  background: rgba(255, 193, 7, 0.1);
  color: #FFC107;
}

.status-accepted {
  background: rgba(76, 175, 80, 0.1);
  color: #4CAF50;
}

.status-canceled {
  background: rgba(158, 158, 158, 0.1);
  color: #9E9E9E;
}

.status-rejected {
  background: rgba(244, 67, 54, 0.1);
  color: #F44336;
}

.status-unknown {
  background: rgba(158, 158, 158, 0.1);
  color: #9E9E9E;
}

.detail-section {
  margin-bottom: 24px;
}

.detail-section h3 {
  font-size: 14px;
  font-weight: 600;
  color: rgba(27, 27, 27, 0.5);
  margin: 0 0 12px 0;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.detail-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 16px;
}

.detail-item {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.detail-item label {
  font-size: 12px;
  color: rgba(27, 27, 27, 0.5);
}

.detail-item span {
  font-size: 15px;
  color: #1B1B1B;
  font-weight: 500;
}

.reason-text {
  font-size: 15px;
  color: #1B1B1B;
  line-height: 1.6;
  margin: 0;
  padding: 16px;
  background: #FEF2F2;
  border-radius: 12px;
  border-left: 4px solid #F44336;
}

@media (max-width: 640px) {
  .detail-header {
    flex-direction: column;
    gap: 16px;
  }

  .header-actions {
    flex-wrap: wrap;
  }

  .detail-grid {
    grid-template-columns: 1fr;
  }
}
</style>
