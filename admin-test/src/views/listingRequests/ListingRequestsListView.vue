<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { api } from '@/services/api'
import type { ListingRequest, GetListingRequestsResponse } from '@/types/listingRequest'
import { ListingRequestStatus, getListingRequestStatusLabel, getListingRequestStatusColor } from '@/types/listingRequest'

// State
const listingRequests = ref<ListingRequest[]>([])
const loading = ref(false)
const error = ref('')



// Pagination
const pagination = ref({
  page: 1,
  pageSize: 10,
  totalCount: 0,
  hasNextPage: false,
  hasPreviousPage: false
})

// Get client photo URL - photo is now a direct URL from API
function getClientPhotoUrl(clientId: string): string | null {
  const request = listingRequests.value.find(r => r.clientId === clientId)
  return request?.clientPhoto || null
}



// Load listing requests
async function loadListingRequests() {
  loading.value = true
  error.value = ''
  try {
    const params = {
      page: pagination.value.page,
      pageSize: pagination.value.pageSize
    }
    const response: GetListingRequestsResponse = await api.getOwnerListingRequests(params)
    listingRequests.value = response.items || []
    pagination.value.totalCount = response.totalCount
    pagination.value.hasNextPage = response.hasNextPage
    pagination.value.hasPreviousPage = response.hasPreviousPage
    // Load client photos after fetching requests

  } catch (e: unknown) {
    const err = e as Error
    error.value = err.message || 'Error loading listing requests'
    console.error('Failed to load listing requests:', e)
  } finally {
    loading.value = false
  }
}

// Actions
async function handleAccept(id: string) {
  if (!confirm('Accept this listing request?')) return
  try {
    await api.acceptListingRequest(id)
    await loadListingRequests()
  } catch (e: unknown) {
    const err = e as Error
    alert(err.message || 'Error accepting listing request')
  }
}

async function handleReject(id: string) {
  const reason = prompt('Enter rejection reason:')
  if (reason === null) return // User cancelled prompt
  if (!reason.trim()) {
    alert('Rejection reason is required')
    return
  }
  try {
    await api.rejectListingRequest(id, reason)
    await loadListingRequests()
  } catch (e: unknown) {
    const err = e as Error
    alert(err.message || 'Error rejecting listing request')
  }
}

// Check if action is available based on status
function canAccept(request: ListingRequest): boolean {
  return request.status === ListingRequestStatus.Sent || request.status === ListingRequestStatus.Received
}

function canReject(request: ListingRequest): boolean {
  return request.status === ListingRequestStatus.Sent || request.status === ListingRequestStatus.Received
}

// Helper to generate initials from name
function getInitials(name: string): string {
  if (!name) return '?'
  const parts = name.trim().split(' ').filter(p => p.length > 0)
  if (parts.length >= 2) {
    const first = parts[0]?.[0] ?? ''
    const last = parts[parts.length - 1]?.[0] ?? ''
    return (first + last).toUpperCase()
  }
  return name.substring(0, 2).toUpperCase()
}

// Lifecycle
onMounted(() => {
  loadListingRequests()
})
</script>

<template>
  <div class="listing-requests-view">
    <div class="page-header">
      <div class="header-content">
        <h1>Listing Requests</h1>
        <p class="subtitle">Manage incoming rental requests from clients</p>
      </div>
    </div>

    <!-- Loading state -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Loading listing requests...</p>
    </div>

    <!-- Error state -->
    <div v-else-if="error" class="error-state">
      <p>{{ error }}</p>
      <button class="btn btn-secondary" @click="loadListingRequests">Retry</button>
    </div>

    <!-- Empty state -->
    <div v-else-if="listingRequests.length === 0" class="empty-state">
      <div class="empty-icon">📋</div>
      <h3>No Listing Requests</h3>
      <p>You don't have any listing requests yet</p>
    </div>

    <!-- Requests list -->
    <div v-else class="requests-list">
      <div
        v-for="request in listingRequests"
        :key="request.id"
        class="request-row"
      >
        <!-- Thumbnail -->
        <div class="row-thumb">
          <img v-if="getClientPhotoUrl(request.clientId)" :src="getClientPhotoUrl(request.clientId)!" alt="" />
          <span v-else class="thumb-initials">{{ getInitials(request.client) }}</span>
        </div>

        <!-- Property Info -->
        <div class="row-property">
          <span class="property-title">{{ request.name }}</span>
          <span v-if="request.clientPhone" class="property-detail">{{ request.clientPhone }}</span>
        </div>

        <!-- Client -->
        <div class="row-client">
          <span class="client-name">{{ request.client }}</span>
          <span v-if="request.clientCompany" class="client-sub">{{ request.clientCompany }}</span>
        </div>

        <!-- Message/Content -->
        <div v-if="request.content" class="row-content">
          <span class="content-text">{{ request.content.length > 60 ? request.content.slice(0, 60) + '...' : request.content }}</span>
        </div>

        <!-- Status -->
        <span :class="['row-status', getListingRequestStatusColor(request.status)]">
          {{ getListingRequestStatusLabel(request.status) }}
        </span>

        <!-- Actions -->
        <div class="row-actions">
          <button v-if="canAccept(request)" class="action-btn accept" @click="handleAccept(request.id)">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
              <path d="M20 6L9 17l-5-5" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
            Accept
          </button>
          <button v-if="canReject(request)" class="action-btn reject" @click="handleReject(request.id)">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
              <path d="M18 6L6 18M6 6l12 12" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
            Reject
          </button>
        </div>
      </div>
    </div>

    <!-- Pagination -->
    <div v-if="pagination.hasNextPage || pagination.hasPreviousPage" class="pagination">
      <button
        class="btn btn-secondary"
        :disabled="!pagination.hasPreviousPage"
        @click="pagination.page--; loadListingRequests()"
      >
        Previous
      </button>
      <span class="page-info">
        Page {{ pagination.page }}
      </span>
      <button
        class="btn btn-secondary"
        :disabled="!pagination.hasNextPage"
        @click="pagination.page++; loadListingRequests()"
      >
        Next
      </button>
    </div>
  </div>
</template>

<style scoped>
.listing-requests-view {
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

/* Compact Row List */
.requests-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
  max-width: 800px;
  margin: 0 auto;
}

.request-row {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 14px 18px;
  background: #FFFFFF;
  border-radius: 14px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);
  transition: box-shadow 0.15s, transform 0.15s;
}

.request-row:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  transform: translateY(-1px);
}

/* Thumbnail */
.row-thumb {
  width: 48px;
  height: 48px;
  border-radius: 12px;
  overflow: hidden;
  flex-shrink: 0;
  background: #F5F5F5;
}

.row-thumb img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.thumb-initials {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #FF5B3C, #FF8A65);
  color: white;
  font-size: 16px;
  font-weight: 600;
}

/* Property Column */
.row-property {
  flex: 1;
  min-width: 100px;
  display: flex;
  flex-direction: column;
  gap: 3px;
}

.property-title {
  font-size: 14px;
  font-weight: 600;
  color: #1B1B1B;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.property-detail {
  font-size: 12px;
  color: rgba(27, 27, 27, 0.5);
}

/* Client Column */
.row-client {
  flex: 1;
  min-width: 100px;
  display: flex;
  flex-direction: column;
  gap: 3px;
}

.client-name {
  font-size: 14px;
  font-weight: 500;
  color: #1B1B1B;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.client-sub {
  font-size: 12px;
  color: rgba(27, 27, 27, 0.5);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

/* Content/Message Column */
.row-content {
  flex: 1.5;
  min-width: 100px;
}

.content-text {
  font-size: 13px;
  color: rgba(27, 27, 27, 0.6);
  line-height: 1.4;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* Status Badge */
.row-status {
  padding: 6px 14px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
  white-space: nowrap;
  flex-shrink: 0;
}

.status-sent {
  background: rgba(33, 150, 243, 0.12);
  color: #2196F3;
}

.status-received {
  background: rgba(255, 152, 0, 0.12);
  color: #FF9800;
}

.status-accepted {
  background: rgba(76, 175, 80, 0.12);
  color: #4CAF50;
}

.status-canceled {
  background: rgba(158, 158, 158, 0.12);
  color: #9E9E9E;
}

.status-rejected {
  background: rgba(244, 67, 54, 0.12);
  color: #F44336;
}

.status-unknown {
  background: rgba(158, 158, 158, 0.12);
  color: #9E9E9E;
}

/* Actions */
.row-actions {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-shrink: 0;
}

.action-btn {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 8px 16px;
  border: none;
  border-radius: 8px;
  font-size: 13px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.15s;
}

.action-btn.accept {
  background: #1B1B1B;
  color: white;
}

.action-btn.accept:hover {
  background: #333333;
}

.action-btn.reject {
  background: #F5F5F5;
  color: #1B1B1B;
}

.action-btn.reject:hover {
  background: #E0E0E0;
}

.detail {
  display: flex;
  align-items: flex-start;
  gap: 6px;
  font-size: 12px;
  color: rgba(27, 27, 27, 0.5);
}

.detail .icon {
  font-size: 12px;
  flex-shrink: 0;
}

.content-preview {
  line-height: 1.4;
}

.card-actions {
  display: flex;
  gap: 8px;
  opacity: 0;
  transition: opacity 0.2s;
}

.request-card:hover .card-actions {
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

  .requests-grid {
    grid-template-columns: 1fr;
  }
}
</style>
