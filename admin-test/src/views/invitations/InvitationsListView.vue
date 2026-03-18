<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '@/services/api'
import type { Invitation, GetInvitationsResponse } from '@/types/invitation'
import { InvitationStatus, getInvitationStatusLabel, getInvitationStatusColor } from '@/types/invitation'

const router = useRouter()

// State
const invitations = ref<Invitation[]>([])
const loading = ref(false)
const error = ref('')

// Filters
const filters = ref({
  filter: ''
})

// Pagination
const pagination = ref({
  page: 1,
  pageSize: 10,
  totalCount: 0,
  hasNextPage: false,
  hasPreviousPage: false
})

// Load invitations
async function loadInvitations() {
  loading.value = true
  error.value = ''
  try {
    const params: { page: number; pageSize: number; filter?: string } = {
      page: pagination.value.page,
      pageSize: pagination.value.pageSize
    }
    if (filters.value.filter) {
      params.filter = filters.value.filter
    }
    const response: GetInvitationsResponse = await api.getInvitations(params)
    invitations.value = response.items || []
    pagination.value.totalCount = response.totalCount
    pagination.value.hasNextPage = response.hasNextPage
    pagination.value.hasPreviousPage = response.hasPreviousPage
  } catch (e: unknown) {
    const err = e as Error
    error.value = err.message || 'Error loading invitations'
    console.error('Failed to load invitations:', e)
  } finally {
    loading.value = false
  }
}

// Format date
function formatDate(dateStr?: string): string {
  if (!dateStr) return '-'
  return new Date(dateStr).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

// Navigation
function navigateToCreate() {
  router.push('/invitations/new')
}

function navigateToDetail(id: string) {
  router.push(`/invitations/${id}`)
}

// Actions
async function handleAccept(id: string) {
  if (!confirm('Accept this invitation?')) return
  try {
    await api.acceptInvitation(id)
    await loadInvitations()
  } catch (e: unknown) {
    const err = e as Error
    alert(err.message || 'Error accepting invitation')
  }
}

async function handleCancel(id: string) {
  if (!confirm('Cancel this invitation?')) return
  try {
    await api.cancelInvitation(id)
    await loadInvitations()
  } catch (e: unknown) {
    const err = e as Error
    alert(err.message || 'Error cancelling invitation')
  }
}

async function handleReject(id: string) {
  const reason = prompt('Enter rejection reason (optional):')
  if (reason === null) return // User cancelled prompt
  try {
    await api.rejectInvitation({ id, reason: reason || undefined })
    await loadInvitations()
  } catch (e: unknown) {
    const err = e as Error
    alert(err.message || 'Error rejecting invitation')
  }
}

async function handleDelete(id: string) {
  if (!confirm('Are you sure you want to delete this invitation?')) return
  try {
    await api.deleteInvitation(id)
    invitations.value = invitations.value.filter(inv => inv.id !== id)
  } catch (e: unknown) {
    const err = e as Error
    alert(err.message || 'Delete error')
  }
}

// Check if action is available based on status
function canAccept(invitation: Invitation): boolean {
  return invitation.status === InvitationStatus.Sent || invitation.status === InvitationStatus.Received
}

function canCancel(invitation: Invitation): boolean {
  return invitation.status === InvitationStatus.Sent || invitation.status === InvitationStatus.Received
}

function canReject(invitation: Invitation): boolean {
  return invitation.status === InvitationStatus.Sent || invitation.status === InvitationStatus.Received
}

// Lifecycle
onMounted(() => {
  loadInvitations()
})
</script>

<template>
  <div class="invitations-list-view">
    <div class="page-header">
      <div class="header-content">
        <h1>Invitations</h1>
        <p class="subtitle">Manage your invitations</p>
      </div>
      <button class="btn btn-primary" @click="navigateToCreate">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M12 5v14"></path>
          <path d="M5 12h14"></path>
        </svg>
        Create Invitation
      </button>
    </div>

    <!-- Filters -->
    <div class="filters-section">
      <div class="filter-group">
        <label>Search</label>
        <input
          v-model="filters.filter"
          type="text"
          placeholder="Search by phone number or name..."
          @keyup.enter="loadInvitations"
        />
      </div>
      <button class="btn btn-secondary" @click="loadInvitations">
        Apply
      </button>
    </div>

    <!-- Loading state -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Loading invitations...</p>
    </div>

    <!-- Error state -->
    <div v-else-if="error" class="error-state">
      <p>{{ error }}</p>
      <button class="btn btn-secondary" @click="loadInvitations">Retry</button>
    </div>

    <!-- Empty state -->
    <div v-else-if="invitations.length === 0" class="empty-state">
      <div class="empty-icon">📧</div>
      <h3>No Invitations</h3>
      <p>Create your first invitation to get started</p>
      <button class="btn btn-primary" @click="navigateToCreate">
        Create Invitation
      </button>
    </div>

    <!-- Invitations grid -->
    <div v-else class="invitations-grid">
      <div
        v-for="invitation in invitations"
        :key="invitation.id"
        class="invitation-card"
        @click="navigateToDetail(invitation.id)"
      >
        <div class="card-header">
          <div class="card-icon">📧</div>
          <span :class="['status-badge', getInvitationStatusColor(invitation.status)]">
            {{ getInvitationStatusLabel(invitation.status) }}
          </span>
        </div>
        <div class="card-content">
          <h3 class="card-title">
            {{ invitation.fullName || invitation.phoneNumber }}
          </h3>
          <div class="card-details">
            <span class="detail" v-if="invitation.phoneNumber">
              <span class="icon">📱</span>
              {{ invitation.phoneNumber }}
            </span>
            <span class="detail" v-if="invitation.roleName">
              <span class="icon">👤</span>
              {{ invitation.roleName }}
            </span>
            <span class="detail" v-if="invitation.expiredTime">
              <span class="icon">⏰</span>
              Expires: {{ formatDate(invitation.expiredTime) }}
            </span>
          </div>
        </div>
        <div class="card-actions" @click.stop>
          <button
            v-if="canAccept(invitation)"
            class="btn-action btn-accept"
            @click="handleAccept(invitation.id)"
            title="Accept"
          >
            ✓
          </button>
          <button
            v-if="canReject(invitation)"
            class="btn-action btn-reject"
            @click="handleReject(invitation.id)"
            title="Reject"
          >
            ✗
          </button>
          <button
            v-if="canCancel(invitation)"
            class="btn-action btn-cancel"
            @click="handleCancel(invitation.id)"
            title="Cancel"
          >
            ⦸
          </button>
          <button class="btn-icon btn-danger" @click="handleDelete(invitation.id)" title="Delete">
            🗑️
          </button>
        </div>
      </div>
    </div>

    <!-- Pagination -->
    <div v-if="pagination.hasNextPage || pagination.hasPreviousPage" class="pagination">
      <button
        class="btn btn-secondary"
        :disabled="!pagination.hasPreviousPage"
        @click="pagination.page--; loadInvitations()"
      >
        Previous
      </button>
      <span class="page-info">
        Page {{ pagination.page }}
      </span>
      <button
        class="btn btn-secondary"
        :disabled="!pagination.hasNextPage"
        @click="pagination.page++; loadInvitations()"
      >
        Next
      </button>
    </div>
  </div>
</template>

<style scoped>
.invitations-list-view {
  padding: 20px 24px;
  max-width: 1400px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.header-content h1 {
  font-size: 24px;
  font-weight: 700;
  color: #1B1B1B;
  margin: 0;
}

.subtitle {
  color: rgba(27, 27, 27, 0.5);
  margin: 4px 0 0 0;
  font-size: 14px;
}

/* Buttons - Dark pill style */
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

.btn-secondary:hover {
  background: #EBEBEB;
}

.btn-secondary:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Filters */
.filters-section {
  display: flex;
  gap: 16px;
  align-items: flex-end;
  margin-bottom: 24px;
  padding: 16px;
  background: #FFFFFF;
  border-radius: 16px;
  border: 0.5px solid rgba(27, 27, 27, 0.04);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
  flex: 1;
}

.filter-group label {
  font-size: 12px;
  font-weight: 600;
  color: rgba(27, 27, 27, 0.5);
}

.filter-group input {
  padding: 10px 14px;
  border: 1px solid rgba(27, 27, 27, 0.08);
  border-radius: 8px;
  font-size: 14px;
  background: #F7F7F7;
  color: #1B1B1B;
}

.filter-group input:focus {
  outline: none;
  border-color: #FF5B3C;
  box-shadow: 0 0 0 3px rgba(255, 91, 60, 0.1);
}

/* States */
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
  margin: 0 0 6px 0;
  color: #1B1B1B;
}

.empty-state p {
  color: rgba(27, 27, 27, 0.5);
  margin: 0 0 16px;
  font-size: 14px;
}

/* Grid */
.invitations-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 20px;
}

.invitation-card {
  background: #FFFFFF;
  border-radius: 24px;
  padding: 20px;
  cursor: pointer;
  transition: all 0.2s ease;
  position: relative;
  border: 0.5px solid rgba(27, 27, 27, 0.04);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
}

.invitation-card:hover {
  transform: translateY(-3px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.08);
  border-color: #FF5B3C;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.card-icon {
  font-size: 24px;
}

.status-badge {
  padding: 4px 12px;
  border-radius: 100px;
  font-size: 12px;
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

.card-content {
  margin-bottom: 12px;
}

.card-title {
  font-size: 16px;
  font-weight: 600;
  color: #1B1B1B;
  margin: 0 0 8px 0;
}

.card-details {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.detail {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 12px;
  color: rgba(27, 27, 27, 0.5);
}

.detail .icon {
  font-size: 12px;
}

.card-actions {
  display: flex;
  gap: 8px;
  opacity: 0;
  transition: opacity 0.2s;
}

.invitation-card:hover .card-actions {
  opacity: 1;
}

.btn-action {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  border: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  transition: all 0.2s;
}

.btn-accept {
  background: rgba(76, 175, 80, 0.1);
  color: #4CAF50;
}

.btn-accept:hover {
  background: rgba(76, 175, 80, 0.2);
}

.btn-reject {
  background: rgba(244, 67, 54, 0.1);
  color: #F44336;
}

.btn-reject:hover {
  background: rgba(244, 67, 54, 0.2);
}

.btn-cancel {
  background: rgba(158, 158, 158, 0.1);
  color: #9E9E9E;
}

.btn-cancel:hover {
  background: rgba(158, 158, 158, 0.2);
}

.btn-icon {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  border: none;
  background: #FFFFFF;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.12);
  transition: all 0.2s;
}

.btn-icon:hover {
  transform: scale(1.1);
}

.btn-icon.btn-danger:hover {
  background: #FEF2F2;
}

/* Pagination */
.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 16px;
  margin-top: 24px;
}

.page-info {
  font-size: 14px;
  color: rgba(27, 27, 27, 0.5);
}

/* Responsive */
@media (max-width: 640px) {
  .page-header {
    flex-direction: column;
    align-items: stretch;
    gap: 16px;
  }

  .filters-section {
    flex-direction: column;
    align-items: stretch;
  }

  .invitations-grid {
    grid-template-columns: 1fr;
  }
}
</style>
