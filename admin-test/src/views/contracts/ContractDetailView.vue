<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { api } from '@/services/api'
import type { ContractDetail } from '@/types/contract'
import { CONTRACT_STATUSES } from '@/types/contract'
import { CONTRACT_TYPES } from '@/types/contractTemplate'

const router = useRouter()
const route = useRoute()

const contract = ref<ContractDetail | null>(null)
const loading = ref(false)
const error = ref('')
const actionLoading = ref('')
const showDocPanel = ref(false)
const docTab = ref<'template' | 'editor'>('template')
const editorHtml = ref('')
const selectedTemplateId = ref('')
const templateVariablesJson = ref('{}')

const contractId = computed(() => route.params.id as string)

const statusConfig = computed(() => {
  if (!contract.value) return { label: '', color: 'gray' }
  return CONTRACT_STATUSES.find(s => s.value === contract.value!.status) || { label: contract.value.status, color: 'gray' }
})

const typeLabel = computed(() => {
  if (!contract.value) return ''
  return CONTRACT_TYPES.find(t => t.key === contract.value!.type)?.label || contract.value.type
})

async function loadContract() {
  loading.value = true
  error.value = ''
  try {
    // TODO: Enable when backend is ready
    // contract.value = await api.getContractById(contractId.value)
    contract.value = null
  } catch (e: unknown) {
    error.value = (e as Error).message || 'Failed to load contract'
  } finally {
    loading.value = false
  }
}

async function handleSign() {
  if (!confirm('Sign this contract?')) return
  actionLoading.value = 'sign'
  try {
    // TODO: Enable when backend is ready
    // await api.signContract(contractId.value)
    // await loadContract()
  } catch (e: unknown) {
    alert((e as Error).message || 'Failed to sign')
  } finally {
    actionLoading.value = ''
  }
}

async function handleCancel() {
  if (!confirm('Cancel this contract? This cannot be undone.')) return
  actionLoading.value = 'cancel'
  try {
    // TODO: Enable when backend is ready
    // await api.cancelContract(contractId.value)
    // await loadContract()
  } catch (e: unknown) {
    alert((e as Error).message || 'Failed to cancel')
  } finally {
    actionLoading.value = ''
  }
}

async function handleSync() {
  actionLoading.value = 'sync'
  try {
    // TODO: Enable when backend is ready
    // const result = await api.syncContractStatus(contractId.value)
    // alert(`Didox status: ${result.didoxStatus}\nContract status: ${result.status}\nSigned: ${result.isSigned}`)
    // await loadContract()
  } catch (e: unknown) {
    alert((e as Error).message || 'Failed to sync')
  } finally {
    actionLoading.value = ''
  }
}

async function handleSaveFromEditor() {
  if (!editorHtml.value.trim()) return
  actionLoading.value = 'doc'
  try {
    // TODO: Enable when backend is ready
    // await api.saveDocumentFromEditor(contractId.value, editorHtml.value)
    // showDocPanel.value = false
    // await loadContract()
  } catch (e: unknown) {
    alert((e as Error).message || 'Failed to save document')
  } finally {
    actionLoading.value = ''
  }
}

async function handleSaveFromTemplate() {
  if (!selectedTemplateId.value) return
  actionLoading.value = 'doc'
  try {
    // TODO: Enable when backend is ready
    // await api.saveDocumentFromTemplate(contractId.value, selectedTemplateId.value, templateVariablesJson.value)
    // showDocPanel.value = false
    // await loadContract()
  } catch (e: unknown) {
    alert((e as Error).message || 'Failed to save document')
  } finally {
    actionLoading.value = ''
  }
}

function formatDate(dateStr: string | undefined): string {
  if (!dateStr) return '—'
  return new Date(dateStr).toLocaleDateString('en-US', {
    year: 'numeric', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit'
  })
}

function goBack() {
  router.push('/contracts')
}

onMounted(() => {
  loadContract()
})
</script>

<template>
  <div class="contract-detail-view">
    <div class="page-header">
      <div class="header-left">
        <button class="btn-back" @click="goBack">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M19 12H5" stroke-linecap="round"/>
            <path d="M12 19l-7-7 7-7" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
        </button>
        <div v-if="contract" class="header-content">
          <h1>{{ contract.contractNumber }}</h1>
          <div class="header-meta">
            <span class="type-badge" :class="'type-' + contract.type.toLowerCase()">{{ typeLabel }}</span>
            <span class="status-badge" :class="'status-' + statusConfig.color">{{ statusConfig.label }}</span>
            <span class="date-text">{{ formatDate(contract.contractDate) }}</span>
          </div>
        </div>
      </div>
      <div v-if="contract" class="header-actions">
        <button
          v-if="contract.status !== 'FullySigned' && contract.status !== 'Cancelled'"
          class="btn btn-primary"
          @click="handleSign"
          :disabled="actionLoading === 'sign'"
        >
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M20 6L9 17l-5-5" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
          {{ actionLoading === 'sign' ? 'Signing...' : 'Sign' }}
        </button>
        <button
          v-if="contract.didoxDocumentId"
          class="btn btn-secondary"
          @click="handleSync"
          :disabled="actionLoading === 'sync'"
        >
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M23 4v6h-6M1 20v-6h6" stroke-linecap="round" stroke-linejoin="round"/>
            <path d="M3.51 9a9 9 0 0 1 14.85-3.36L23 10M1 14l4.64 4.36A9 9 0 0 0 20.49 15" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
          {{ actionLoading === 'sync' ? 'Syncing...' : 'Sync' }}
        </button>
        <button
          class="btn btn-ghost"
          @click="showDocPanel = !showDocPanel"
        >
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" stroke-linecap="round" stroke-linejoin="round"/>
            <path d="M14 2v6h6M12 18v-6M9 15h6" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
          Add Document
        </button>
        <button
          v-if="contract.status !== 'FullySigned' && contract.status !== 'Cancelled'"
          class="btn btn-danger"
          @click="handleCancel"
          :disabled="actionLoading === 'cancel'"
        >
          {{ actionLoading === 'cancel' ? 'Cancelling...' : 'Cancel Contract' }}
        </button>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Loading contract...</p>
    </div>

    <!-- Error -->
    <div v-else-if="error" class="error-state">
      <p>{{ error }}</p>
      <button class="btn btn-secondary" @click="loadContract">Retry</button>
    </div>

    <template v-else-if="contract">
      <!-- Info Cards -->
      <div class="info-cards">
        <div class="info-card">
          <div class="info-label">Signatures</div>
          <div class="info-value">
            <span class="sig-count">{{ contract.signedCount }}</span>
            <span class="sig-separator">/</span>
            <span class="sig-total">{{ contract.partiesCount }}</span>
          </div>
        </div>
        <div class="info-card">
          <div class="info-label">Document</div>
          <div class="info-value">{{ contract.documentName || 'None' }}</div>
        </div>
        <div v-if="contract.didoxDocumentId" class="info-card">
          <div class="info-label">Didox</div>
          <div class="info-value">
            <a v-if="contract.didoxDocumentUrl" :href="contract.didoxDocumentUrl" target="_blank" class="didox-link">
              View in Didox
              <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M18 13v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h6M15 3h6v6M10 14L21 3" stroke-linecap="round" stroke-linejoin="round"/>
              </svg>
            </a>
            <span v-else class="didox-id">{{ contract.didoxDocumentId }}</span>
          </div>
        </div>
      </div>

      <!-- Parties -->
      <div class="detail-panel">
        <h3 class="panel-title">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" stroke-linecap="round" stroke-linejoin="round"/>
            <circle cx="9" cy="7" r="4" stroke-linecap="round" stroke-linejoin="round"/>
            <path d="M23 21v-2a4 4 0 0 0-3-3.87M16 3.13a4 4 0 0 1 0 7.75" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
          Parties
          <span class="count-badge">{{ contract.parties.length }}</span>
        </h3>
        <table class="parties-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>TIN</th>
              <th>Role</th>
              <th>Status</th>
              <th>Signed At</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="party in contract.parties" :key="party.id">
              <td>
                <div class="party-name">{{ party.name }}</div>
                <div v-if="party.address" class="party-address">{{ party.address }}</div>
              </td>
              <td class="tin-cell">{{ party.tin }}</td>
              <td>
                <span class="role-badge" :class="'role-' + party.role.toLowerCase()">{{ party.role }}</span>
              </td>
              <td>
                <span v-if="party.isSigned" class="signed-yes">
                  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                    <path d="M20 6L9 17l-5-5" stroke-linecap="round" stroke-linejoin="round"/>
                  </svg>
                  Signed
                </span>
                <span v-else class="signed-no">Pending</span>
              </td>
              <td class="date-cell">{{ party.isSigned ? formatDate(party.signedAt) : '—' }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Document Creation Panel -->
      <div v-if="showDocPanel" class="detail-panel doc-panel">
        <h3 class="panel-title">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
          Create Document
        </h3>
        <div class="doc-tabs">
          <button class="tab-btn" :class="{ active: docTab === 'template' }" @click="docTab = 'template'">From Template</button>
          <button class="tab-btn" :class="{ active: docTab === 'editor' }" @click="docTab = 'editor'">From Editor</button>
        </div>

        <div v-if="docTab === 'template'" class="doc-form">
          <div class="form-group">
            <label class="form-label">Template ID</label>
            <input v-model="selectedTemplateId" class="form-input" placeholder="Template UUID" />
          </div>
          <div class="form-group">
            <label class="form-label">Variables (JSON)</label>
            <textarea v-model="templateVariablesJson" class="form-input" rows="4" placeholder='{"owner_name": "John", "tenant_name": "Jane"}'></textarea>
          </div>
          <button class="btn btn-primary" @click="handleSaveFromTemplate" :disabled="actionLoading === 'doc'">
            {{ actionLoading === 'doc' ? 'Saving...' : 'Create from Template' }}
          </button>
        </div>

        <div v-if="docTab === 'editor'" class="doc-form">
          <div class="form-group">
            <label class="form-label">HTML Content</label>
            <textarea v-model="editorHtml" class="html-editor" rows="12" placeholder="<h1>Contract Document</h1>..."></textarea>
          </div>
          <button class="btn btn-primary" @click="handleSaveFromEditor" :disabled="actionLoading === 'doc'">
            {{ actionLoading === 'doc' ? 'Saving...' : 'Create from Editor' }}
          </button>
        </div>
      </div>
    </template>
  </div>
</template>

<style scoped>
.contract-detail-view { padding: 0; }

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: var(--spacing-xl);
  flex-wrap: wrap;
  gap: var(--spacing-md);
}

.header-left {
  display: flex;
  align-items: flex-start;
  gap: var(--spacing-md);
}

.btn-back {
  width: 40px; height: 40px; min-width: 40px;
  display: flex; align-items: center; justify-content: center;
  background: var(--bg-card); border: 1px solid var(--border-color);
  border-radius: var(--radius-sm); cursor: pointer;
  color: var(--text-secondary); transition: all var(--transition-fast);
  margin-top: 0.25rem;
}
.btn-back:hover { background: var(--bg-secondary); color: var(--text-primary); }

.header-content h1 { font-size: 1.5rem; font-weight: 700; margin-bottom: 0.5rem; }

.header-meta { display: flex; align-items: center; gap: 0.75rem; flex-wrap: wrap; }

.header-actions { display: flex; gap: var(--spacing-sm); flex-wrap: wrap; }

.date-text { font-size: 0.8125rem; color: var(--text-muted); }

/* Type Badges */
.type-badge {
  display: inline-flex; align-items: center; padding: 0.25rem 0.625rem;
  font-size: 0.6875rem; font-weight: 600; border-radius: var(--radius-pill);
  text-transform: uppercase; letter-spacing: 0.03em;
}
.type-delegation { background: var(--color-brand-light); color: var(--color-brand); }
.type-lease { background: var(--color-success-light); color: var(--color-success); }
.type-handover { background: var(--color-warning-light); color: var(--color-warning); }
.type-communalbill { background: rgba(99, 102, 241, 0.1); color: #6366F1; }
.type-serviceact { background: rgba(139, 92, 246, 0.1); color: #8B5CF6; }
.type-reconciliation { background: rgba(20, 184, 166, 0.1); color: #14B8A6; }

/* Status Badges */
.status-badge {
  display: inline-flex; align-items: center; padding: 0.25rem 0.625rem;
  font-size: 0.6875rem; font-weight: 600; border-radius: var(--radius-pill);
}
.status-gray { background: rgba(107, 114, 128, 0.1); color: #6B7280; }
.status-amber { background: rgba(245, 158, 11, 0.1); color: #D97706; }
.status-blue { background: rgba(59, 130, 246, 0.1); color: #3B82F6; }
.status-green { background: rgba(22, 160, 94, 0.1); color: #16A05E; }
.status-red { background: rgba(239, 68, 68, 0.1); color: #EF4444; }

/* Info Cards */
.info-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
  gap: var(--spacing-md);
  margin-bottom: var(--spacing-xl);
}

.info-card {
  background: var(--bg-card);
  border-radius: var(--radius-lg);
  padding: var(--spacing-lg);
  box-shadow: var(--shadow-card);
}

.info-label {
  font-size: 0.6875rem;
  font-weight: 600;
  color: var(--text-muted);
  text-transform: uppercase;
  letter-spacing: 0.05em;
  margin-bottom: 0.5rem;
}

.info-value {
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--text-primary);
}

.sig-count { color: var(--color-brand); font-size: 1.5rem; }
.sig-separator { color: var(--text-muted); margin: 0 0.125rem; }
.sig-total { color: var(--text-muted); font-size: 1.5rem; font-weight: 400; }

.didox-link {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  color: var(--color-brand);
  font-size: 0.875rem;
  text-decoration: none;
}
.didox-link:hover { text-decoration: underline; }

.didox-id { font-size: 0.75rem; color: var(--text-muted); font-family: monospace; }

/* Panels */
.detail-panel {
  background: var(--bg-card);
  border-radius: var(--radius-xl);
  box-shadow: var(--shadow-card);
  padding: var(--spacing-lg);
  margin-bottom: var(--spacing-lg);
}

.panel-title {
  display: flex; align-items: center; gap: 0.5rem;
  font-size: 0.9375rem; font-weight: 600;
  margin-bottom: var(--spacing-lg); color: var(--text-primary);
}

.count-badge {
  display: inline-flex; align-items: center; justify-content: center;
  min-width: 22px; height: 22px; padding: 0 0.375rem;
  font-size: 0.6875rem; font-weight: 600;
  background: var(--bg-secondary); border-radius: var(--radius-pill); color: var(--text-muted);
}

/* Parties Table */
.parties-table { width: 100%; border-collapse: collapse; }

.parties-table th {
  text-align: left; padding: 0.625rem 0.75rem;
  font-size: 0.6875rem; font-weight: 600; color: var(--text-muted);
  text-transform: uppercase; letter-spacing: 0.05em;
  border-bottom: 1px solid var(--border-color);
}

.parties-table td {
  padding: 0.75rem; font-size: 0.8125rem;
  border-bottom: 1px solid var(--border-color); vertical-align: middle;
}

.parties-table tbody tr:last-child td { border-bottom: none; }
.parties-table tbody tr:hover { background: var(--bg-secondary); }

.party-name { font-weight: 500; }
.party-address { font-size: 0.75rem; color: var(--text-muted); margin-top: 0.125rem; }
.tin-cell { font-family: 'SF Mono', monospace; font-size: 0.75rem; color: var(--text-secondary); }

.role-badge {
  display: inline-flex; padding: 0.25rem 0.5rem;
  font-size: 0.6875rem; font-weight: 600;
  border-radius: var(--radius-pill);
}
.role-owner { background: var(--color-brand-light); color: var(--color-brand); }
.role-agent { background: rgba(99, 102, 241, 0.1); color: #6366F1; }
.role-tenant { background: var(--color-success-light); color: var(--color-success); }

.signed-yes {
  display: inline-flex; align-items: center; gap: 0.25rem;
  color: var(--color-success); font-weight: 500; font-size: 0.8125rem;
}
.signed-no { color: var(--text-muted); font-size: 0.8125rem; }
.date-cell { color: var(--text-muted); font-size: 0.75rem; }

/* Document Panel */
.doc-panel { margin-top: var(--spacing-lg); }

.doc-tabs {
  display: flex; gap: 0; margin-bottom: var(--spacing-lg);
  border-bottom: 1px solid var(--border-color);
}

.tab-btn {
  padding: 0.625rem 1rem; font-size: 0.8125rem; font-weight: 500;
  background: transparent; border: none; border-bottom: 2px solid transparent;
  cursor: pointer; color: var(--text-muted); transition: all var(--transition-fast);
}
.tab-btn.active { color: var(--text-primary); border-bottom-color: var(--color-brand); }
.tab-btn:hover:not(.active) { color: var(--text-primary); }

.doc-form { display: flex; flex-direction: column; gap: var(--spacing-md); }

.html-editor {
  font-family: 'SF Mono', 'Fira Code', monospace;
  font-size: 0.8125rem; line-height: 1.7;
  padding: var(--spacing-md); background: var(--bg-secondary);
  border: 1px solid var(--border-color); border-radius: var(--radius-sm);
  color: var(--text-primary); resize: vertical;
}
.html-editor:focus { outline: none; border-color: var(--color-brand); }

/* Danger button */
.btn-danger {
  padding: 0.5rem 1rem; font-size: 0.8125rem; font-weight: 600;
  background: transparent; border: 1px solid var(--color-error);
  color: var(--color-error); border-radius: var(--radius-sm);
  cursor: pointer; transition: all var(--transition-fast);
}
.btn-danger:hover { background: var(--color-error); color: white; }

/* States */
.loading-state, .error-state {
  display: flex; flex-direction: column; align-items: center;
  justify-content: center; padding: 4rem 2rem; gap: var(--spacing-md);
}
</style>
