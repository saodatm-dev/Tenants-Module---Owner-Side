<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '@/services/api'
import type { ContractListItem } from '@/types/contract'
import { CONTRACT_STATUSES } from '@/types/contract'
import { CONTRACT_TYPES } from '@/types/contractTemplate'

const router = useRouter()

// State
const contracts = ref<ContractListItem[]>([])
const loading = ref(false)
const error = ref('')

// Filters
const selectedType = ref('')
const selectedStatus = ref('')

const filteredContracts = computed(() => contracts.value)

async function loadContracts() {
  loading.value = true
  error.value = ''
  try {
    // TODO: Enable when backend is ready
    // contracts.value = await api.getMyContracts(selectedType.value || undefined, selectedStatus.value || undefined)
    contracts.value = []
  } catch (e: unknown) {
    const err = e as Error
    error.value = err.message || 'Failed to load contracts'
  } finally {
    loading.value = false
  }
}

function getStatusConfig(status: string) {
  return CONTRACT_STATUSES.find(s => s.value === status) || { value: status, label: status, color: 'gray' }
}

function getTypeLabel(type: string): string {
  return CONTRACT_TYPES.find(t => t.key === type)?.label || type
}

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString('en-US', {
    year: 'numeric', month: 'short', day: 'numeric'
  })
}

function navigateToCreate() {
  router.push('/contracts/new')
}

function navigateToDetail(id: string) {
  router.push(`/contracts/${id}`)
}

onMounted(() => {
  loadContracts()
})
</script>

<template>
  <div class="contracts-list-view">
    <div class="page-header">
      <div class="header-content">
        <h1>Contracts</h1>
        <p class="subtitle">Manage your contracts and signatures</p>
      </div>
      <button class="btn btn-primary" @click="navigateToCreate">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M12 5v14M5 12h14" stroke-linecap="round"/>
        </svg>
        New Contract
      </button>
    </div>

    <!-- Filters -->
    <div class="filters-section">
      <div class="filter-group">
        <label>Contract Type</label>
        <select v-model="selectedType" @change="loadContracts">
          <option value="">All Types</option>
          <option v-for="ct in CONTRACT_TYPES" :key="ct.value" :value="ct.value">
            {{ ct.label }}
          </option>
        </select>
      </div>
      <div class="filter-group">
        <label>Status</label>
        <select v-model="selectedStatus" @change="loadContracts">
          <option value="">All Statuses</option>
          <option v-for="s in CONTRACT_STATUSES" :key="s.value" :value="s.value">
            {{ s.label }}
          </option>
        </select>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Loading contracts...</p>
    </div>

    <!-- Error -->
    <div v-else-if="error" class="error-state">
      <p>{{ error }}</p>
      <button class="btn btn-secondary" @click="loadContracts">Retry</button>
    </div>

    <!-- Empty -->
    <div v-else-if="filteredContracts.length === 0" class="empty-state">
      <div class="empty-icon">
        <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" opacity="0.4">
          <path d="M9 12h6M9 16h6M17 21H7a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h5.586a1 1 0 0 1 .707.293l5.414 5.414a1 1 0 0 1 .293.707V19a2 2 0 0 1-2 2z" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </div>
      <h3>No Contracts</h3>
      <p>Create your first contract</p>
      <button class="btn btn-primary" @click="navigateToCreate">New Contract</button>
    </div>

    <!-- Contracts Table -->
    <div v-else class="table-container">
      <table class="contracts-table">
        <thead>
          <tr>
            <th>Contract</th>
            <th>Type</th>
            <th>Status</th>
            <th>Signatures</th>
            <th>Document</th>
            <th>Created</th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="contract in filteredContracts"
            :key="contract.id"
            @click="navigateToDetail(contract.id)"
            class="table-row"
          >
            <td>
              <div class="contract-number">{{ contract.contractNumber }}</div>
              <div class="contract-date">{{ formatDate(contract.contractDate) }}</div>
            </td>
            <td>
              <span class="type-badge" :class="'type-' + contract.type.toLowerCase()">
                {{ getTypeLabel(contract.type) }}
              </span>
            </td>
            <td>
              <span class="status-badge" :class="'status-' + getStatusConfig(contract.status).color">
                {{ getStatusConfig(contract.status).label }}
              </span>
            </td>
            <td>
              <div class="signature-progress">
                <div class="progress-bar">
                  <div
                    class="progress-fill"
                    :class="{ 'progress-complete': contract.signedCount === contract.partiesCount && contract.partiesCount > 0 }"
                    :style="{ width: contract.partiesCount > 0 ? (contract.signedCount / contract.partiesCount * 100) + '%' : '0%' }"
                  ></div>
                </div>
                <span class="progress-text">{{ contract.signedCount }}/{{ contract.partiesCount }}</span>
              </div>
            </td>
            <td>
              <span v-if="contract.documentName" class="doc-name">{{ contract.documentName }}</span>
              <span v-else class="no-doc">No document</span>
            </td>
            <td class="date-cell">{{ formatDate(contract.createdAt) }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<style scoped>
.contracts-list-view {
  padding: 0;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: var(--spacing-xl);
}

.header-content h1 {
  font-size: 1.75rem;
  font-weight: 700;
  margin-bottom: 0.25rem;
}

.subtitle {
  color: var(--text-secondary);
  font-size: 0.875rem;
}

/* Filters */
.filters-section {
  display: flex;
  gap: var(--spacing-md);
  margin-bottom: var(--spacing-xl);
  flex-wrap: wrap;
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 0.375rem;
}

.filter-group label {
  font-size: 0.75rem;
  font-weight: 500;
  color: var(--text-muted);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.filter-group select {
  padding: 0.625rem 1rem;
  font-size: 0.875rem;
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-sm);
  color: var(--text-primary);
  min-width: 160px;
  transition: all var(--transition-fast);
}

.filter-group select:focus {
  outline: none;
  border-color: var(--color-brand);
  box-shadow: 0 0 0 3px var(--border-focus);
}

/* States */
.loading-state, .error-state, .empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  text-align: center;
  gap: var(--spacing-md);
}

.empty-icon {
  width: 80px;
  height: 80px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--bg-secondary);
  border-radius: var(--radius-xl);
  margin-bottom: var(--spacing-sm);
}

.empty-state h3 { font-size: 1.125rem; font-weight: 600; }
.empty-state p { color: var(--text-muted); font-size: 0.875rem; }

/* Table */
.table-container {
  background: var(--bg-card);
  border-radius: var(--radius-xl);
  box-shadow: var(--shadow-card);
  overflow: hidden;
}

.contracts-table {
  width: 100%;
  border-collapse: collapse;
}

.contracts-table th {
  text-align: left;
  padding: 0.875rem 1rem;
  font-size: 0.6875rem;
  font-weight: 600;
  color: var(--text-muted);
  text-transform: uppercase;
  letter-spacing: 0.05em;
  border-bottom: 1px solid var(--border-color);
  background: var(--bg-secondary);
}

.contracts-table td {
  padding: 0.875rem 1rem;
  font-size: 0.8125rem;
  border-bottom: 1px solid var(--border-color);
  vertical-align: middle;
}

.table-row {
  cursor: pointer;
  transition: background var(--transition-fast);
}

.table-row:hover {
  background: var(--bg-secondary);
}

.table-row:last-child td {
  border-bottom: none;
}

.contract-number {
  font-weight: 600;
  font-size: 0.875rem;
  color: var(--text-primary);
}

.contract-date {
  font-size: 0.75rem;
  color: var(--text-muted);
  margin-top: 0.125rem;
}

/* Type Badges */
.type-badge {
  display: inline-flex;
  align-items: center;
  padding: 0.25rem 0.625rem;
  font-size: 0.6875rem;
  font-weight: 600;
  border-radius: var(--radius-pill);
  text-transform: uppercase;
  letter-spacing: 0.03em;
}

.type-delegation { background: var(--color-brand-light); color: var(--color-brand); }
.type-lease { background: var(--color-success-light); color: var(--color-success); }
.type-handover { background: var(--color-warning-light); color: var(--color-warning); }
.type-communalbill { background: rgba(99, 102, 241, 0.1); color: #6366F1; }
.type-serviceact { background: rgba(139, 92, 246, 0.1); color: #8B5CF6; }
.type-reconciliation { background: rgba(20, 184, 166, 0.1); color: #14B8A6; }

/* Status Badges */
.status-badge {
  display: inline-flex;
  align-items: center;
  padding: 0.25rem 0.625rem;
  font-size: 0.6875rem;
  font-weight: 600;
  border-radius: var(--radius-pill);
}

.status-gray { background: rgba(107, 114, 128, 0.1); color: #6B7280; }
.status-amber { background: rgba(245, 158, 11, 0.1); color: #D97706; }
.status-blue { background: rgba(59, 130, 246, 0.1); color: #3B82F6; }
.status-green { background: rgba(22, 160, 94, 0.1); color: #16A05E; }
.status-red { background: rgba(239, 68, 68, 0.1); color: #EF4444; }

/* Signature progress */
.signature-progress {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.progress-bar {
  width: 60px;
  height: 6px;
  background: var(--bg-secondary);
  border-radius: 3px;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: var(--color-brand);
  border-radius: 3px;
  transition: width 0.3s ease;
}

.progress-complete {
  background: var(--color-success);
}

.progress-text {
  font-size: 0.75rem;
  font-weight: 500;
  color: var(--text-secondary);
}

/* Document */
.doc-name {
  font-size: 0.8125rem;
  color: var(--text-primary);
}

.no-doc {
  font-size: 0.75rem;
  color: var(--text-muted);
  font-style: italic;
}

.date-cell {
  color: var(--text-muted);
  font-size: 0.75rem;
}
</style>
