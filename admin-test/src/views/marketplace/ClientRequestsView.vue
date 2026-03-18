<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '@/services/api'
import type { ClientListingRequest, GetClientListingRequestsResponse } from '@/types/marketplace'
import { ClientListingRequestStatus, getClientRequestStatusLabel, getClientRequestStatusColor } from '@/types/marketplace'

const router = useRouter()

// Data state
const requests = ref<ClientListingRequest[]>([])
const loading = ref(false)
const error = ref('')
const page = ref(1)
const pageSize = ref(10)
const totalCount = ref(0)
const hasNextPage = ref(false)
const hasPreviousPage = ref(false)

// Computed
const totalPages = computed(() => Math.ceil(totalCount.value / pageSize.value))

// Load requests
async function loadRequests() {
  loading.value = true
  error.value = ''
  try {
    const response = await api.getClientListingRequests({
      page: page.value,
      pageSize: pageSize.value
    })
    requests.value = response.items || []
    totalCount.value = response.totalCount
    hasNextPage.value = response.hasNextPage
    hasPreviousPage.value = response.hasPreviousPage
  } catch (e: any) {
    error.value = e.message || 'Ошибка загрузки заявок'
    console.error('Failed to load requests:', e)
  } finally {
    loading.value = false
  }
}

// Cancel request
async function cancelRequest(id: string) {
  if (!confirm('Вы уверены что хотите отменить заявку?')) return

  try {
    await api.cancelListingRequest(id)
    // Refresh list
    await loadRequests()
  } catch (e: any) {
    alert('Ошибка при отмене заявки: ' + e.message)
  }
}

// Check if request can be cancelled
function canCancel(request: ClientListingRequest): boolean {
  return request.status === ClientListingRequestStatus.Sent ||
         request.status === ClientListingRequestStatus.Received
}

// Pagination
function goToPage(newPage: number) {
  if (newPage >= 1 && newPage <= totalPages.value) {
    page.value = newPage
    loadRequests()
  }
}

onMounted(() => {
  loadRequests()
})
</script>

<template>
  <div class="client-requests-view">
    <!-- Header -->
    <div class="view-header">
      <h1 class="view-title">Мои заявки</h1>
      <p class="view-subtitle">Здесь отображаются ваши заявки на аренду</p>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Загрузка заявок...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="error-state">
      <p>{{ error }}</p>
      <button class="btn btn-secondary" @click="loadRequests">
        Повторить
      </button>
    </div>

    <!-- Empty State -->
    <div v-else-if="requests.length === 0" class="empty-state">
      <div class="empty-icon">📋</div>
      <h3>Нет заявок</h3>
      <p>Вы еще не отправляли заявки на аренду</p>
      <button class="btn btn-primary" @click="router.push('/marketplace')">
        Найти помещение
      </button>
    </div>

    <!-- Requests Grid -->
    <div v-else class="requests-grid">
      <article
        v-for="request in requests"
        :key="request.id"
        class="request-card"
      >
        <div class="card-header">
          <span class="status-badge" :class="getClientRequestStatusColor(request.status)">
            {{ getClientRequestStatusLabel(request.status) }}
          </span>
        </div>

        <div class="card-body">
          <h4 class="card-title">{{ request.name }}</h4>
          <p class="card-owner">Владелец: {{ request.owner }}</p>
          <p class="card-content">{{ request.content }}</p>
        </div>

        <div class="card-actions">
          <button
            v-if="canCancel(request)"
            class="btn-action btn-cancel"
            @click="cancelRequest(request.id)"
            title="Отменить"
          >
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <line x1="18" y1="6" x2="6" y2="18"></line>
              <line x1="6" y1="6" x2="18" y2="18"></line>
            </svg>
            Отменить
          </button>
        </div>
      </article>
    </div>

    <!-- Pagination -->
    <div v-if="requests.length > 0 && totalPages > 1" class="pagination">
      <button
        class="pagination-btn"
        :disabled="!hasPreviousPage"
        @click="goToPage(page - 1)"
      >
        ← Назад
      </button>
      <span class="pagination-info">
        Страница {{ page }} из {{ totalPages }}
      </span>
      <button
        class="pagination-btn"
        :disabled="!hasNextPage"
        @click="goToPage(page + 1)"
      >
        Далее →
      </button>
    </div>
  </div>
</template>

<style scoped>
.client-requests-view {
  padding: 20px 24px;
  max-width: 1200px;
  margin: 0 auto;
}

/* Header */
.view-header {
  margin-bottom: 32px;
}

.view-title {
  font-size: 28px;
  font-weight: 700;
  color: #1B1B1B;
  margin: 0 0 8px;
}

.view-subtitle {
  font-size: 14px;
  color: rgba(27, 27, 27, 0.5);
  margin: 0;
}

/* States */
.loading-state,
.error-state,
.empty-state {
  text-align: center;
  padding: 80px 20px;
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
  font-size: 48px;
  margin-bottom: 16px;
}

.empty-state h3 {
  font-size: 20px;
  margin: 0 0 8px;
  color: #1B1B1B;
}

.empty-state p,
.error-state p {
  color: rgba(27, 27, 27, 0.5);
  margin: 0 0 20px;
  font-size: 14px;
}

.btn-primary {
  padding: 12px 24px;
  background: #1B1B1B;
  color: white;
  border: none;
  border-radius: 100px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-primary:hover {
  background: #333;
}

.btn-secondary {
  padding: 10px 20px;
  border: 1px solid #1B1B1B;
  background: transparent;
  border-radius: 100px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
}

/* Requests Grid */
.requests-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 20px;
}

.request-card {
  background: #FFFFFF;
  border-radius: 20px;
  overflow: hidden;
  border: 0.5px solid rgba(27, 27, 27, 0.04);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
  transition: all 0.2s;
}

.request-card:hover {
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.08);
}

.card-header {
  padding: 16px;
  border-bottom: 1px solid rgba(27, 27, 27, 0.04);
}

.status-badge {
  display: inline-block;
  padding: 6px 12px;
  border-radius: 100px;
  font-size: 12px;
  font-weight: 600;
}

.status-sent {
  background: rgba(33, 150, 243, 0.1);
  color: #2196F3;
}

.status-received {
  background: rgba(156, 39, 176, 0.1);
  color: #9C27B0;
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

.card-body {
  padding: 16px;
}

.card-title {
  font-size: 16px;
  font-weight: 600;
  color: #1B1B1B;
  margin: 0 0 8px;
}

.card-owner {
  font-size: 13px;
  color: rgba(27, 27, 27, 0.6);
  margin: 0 0 8px;
}

.card-content {
  font-size: 14px;
  color: rgba(27, 27, 27, 0.7);
  margin: 0;
  line-height: 1.5;
}

.card-actions {
  padding: 12px 16px;
  border-top: 1px solid rgba(27, 27, 27, 0.04);
  display: flex;
  gap: 8px;
}

.btn-action {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 14px;
  border: none;
  border-radius: 100px;
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-cancel {
  background: rgba(244, 67, 54, 0.1);
  color: #F44336;
}

.btn-cancel:hover {
  background: rgba(244, 67, 54, 0.2);
}

/* Pagination */
.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 16px;
  margin-top: 32px;
}

.pagination-btn {
  padding: 10px 20px;
  background: #FFFFFF;
  border: 1px solid rgba(27, 27, 27, 0.1);
  border-radius: 100px;
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.pagination-btn:hover:not(:disabled) {
  background: #1B1B1B;
  color: white;
}

.pagination-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination-info {
  font-size: 13px;
  color: rgba(27, 27, 27, 0.6);
}
</style>
