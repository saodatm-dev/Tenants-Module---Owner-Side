<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '@/services/api'
import type { CreatePartyData } from '@/types/contract'
import { CONTRACT_PARTY_ROLES } from '@/types/contract'
import { CONTRACT_TYPES } from '@/types/contractTemplate'

const router = useRouter()

// State
const step = ref(1)
const saving = ref(false)
const error = ref('')

// Step 1: Contract Info
const selectedType = ref(0)
const contractNumber = ref('')
const contractDate = ref(new Date().toISOString().split('T')[0])
const documentName = ref('')

// Step 2: Parties
const parties = ref<CreatePartyData[]>([])

// Step labels
const steps = ['Contract Info', 'Parties', 'Review']

const canProceed = computed(() => {
  if (step.value === 1) return contractNumber.value.trim() !== '' && contractDate.value !== ''
  if (step.value === 2) return parties.value.length > 0 && parties.value.every(p => p.tin && p.name)
  return true
})

function addParty() {
  parties.value.push({
    userId: crypto.randomUUID(),
    tin: '',
    role: 0,
    name: '',
    address: ''
  })
}

function removeParty(idx: number) {
  parties.value.splice(idx, 1)
}

function nextStep() {
  if (step.value < 3) step.value++
}

function prevStep() {
  if (step.value > 1) step.value--
}

function getRoleLabel(role: number): string {
  return CONTRACT_PARTY_ROLES.find(r => r.value === role)?.label || String(role)
}

function getTypeLabel(type: number): string {
  return CONTRACT_TYPES.find(t => t.value === type)?.label || String(type)
}

async function handleCreate() {
  saving.value = true
  error.value = ''
  try {
    // TODO: Enable when backend is ready
    // const id = await api.createContract({
    //   type: selectedType.value,
    //   contractNumber: contractNumber.value,
    //   contractDate: contractDate.value || '',
    //   documentName: documentName.value || undefined,
    //   parties: parties.value
    // })
    // router.push(`/contracts/${id}`)
  } catch (e: unknown) {
    error.value = (e as Error).message || 'Failed to create contract'
  } finally {
    saving.value = false
  }
}

function goBack() {
  router.push('/contracts')
}
</script>

<template>
  <div class="contract-create-view">
    <div class="page-header">
      <div class="header-left">
        <button class="btn-back" @click="goBack">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M19 12H5" stroke-linecap="round"/>
            <path d="M12 19l-7-7 7-7" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
        </button>
        <div class="header-content">
          <h1>New Contract</h1>
          <p class="subtitle">Create a new contract</p>
        </div>
      </div>
    </div>

    <!-- Step Indicator -->
    <div class="steps-bar">
      <div v-for="(s, i) in steps" :key="i" class="step-item" :class="{ active: step === i + 1, done: step > i + 1 }">
        <div class="step-number">
          <svg v-if="step > i + 1" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3">
            <path d="M20 6L9 17l-5-5" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
          <span v-else>{{ i + 1 }}</span>
        </div>
        <span class="step-label">{{ s }}</span>
      </div>
    </div>

    <!-- Error -->
    <div v-if="error" class="alert alert-error">{{ error }}</div>

    <!-- Step 1: Contract Info -->
    <div v-if="step === 1" class="step-panel">
      <h3 class="panel-title">Contract Information</h3>
      <div class="form-grid">
        <div class="form-group">
          <label class="form-label">Contract Type</label>
          <select v-model="selectedType" class="form-input">
            <option v-for="ct in CONTRACT_TYPES" :key="ct.value" :value="ct.value">{{ ct.label }}</option>
          </select>
        </div>
        <div class="form-group">
          <label class="form-label">Contract Number *</label>
          <input v-model="contractNumber" class="form-input" placeholder="e.g. DLG-2026-001" />
        </div>
        <div class="form-group">
          <label class="form-label">Contract Date *</label>
          <input v-model="contractDate" type="date" class="form-input" />
        </div>
        <div class="form-group">
          <label class="form-label">Document Name</label>
          <input v-model="documentName" class="form-input" placeholder="Optional document title" />
        </div>
      </div>
    </div>

    <!-- Step 2: Parties -->
    <div v-if="step === 2" class="step-panel">
      <div class="section-header">
        <h3 class="panel-title">Contract Parties</h3>
        <button class="btn-add" @click="addParty">
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
            <path d="M12 5v14M5 12h14" stroke-linecap="round"/>
          </svg>
          Add Party
        </button>
      </div>
      <p v-if="parties.length === 0" class="hint-text">Add at least one party to the contract.</p>
      <div class="parties-list">
        <div v-for="(party, idx) in parties" :key="idx" class="party-card">
          <div class="party-card-header">
            <span class="party-label">Party {{ idx + 1 }}</span>
            <button class="btn-remove" @click="removeParty(idx)">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M18 6L6 18M6 6l12 12" stroke-linecap="round"/>
              </svg>
            </button>
          </div>
          <div class="party-form-grid">
            <div class="form-group">
              <label class="form-label">Name *</label>
              <input v-model="party.name" class="form-input" placeholder="Full name or company" />
            </div>
            <div class="form-group">
              <label class="form-label">TIN *</label>
              <input v-model="party.tin" class="form-input" placeholder="Tax ID number" />
            </div>
            <div class="form-group">
              <label class="form-label">Role</label>
              <select v-model="party.role" class="form-input">
                <option v-for="r in CONTRACT_PARTY_ROLES" :key="r.value" :value="r.value">{{ r.label }}</option>
              </select>
            </div>
            <div class="form-group">
              <label class="form-label">Address</label>
              <input v-model="party.address" class="form-input" placeholder="Optional" />
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Step 3: Review -->
    <div v-if="step === 3" class="step-panel">
      <h3 class="panel-title">Review Contract</h3>
      <div class="review-section">
        <div class="review-grid">
          <div class="review-item">
            <span class="review-label">Type</span>
            <span class="review-value">{{ getTypeLabel(selectedType) }}</span>
          </div>
          <div class="review-item">
            <span class="review-label">Number</span>
            <span class="review-value">{{ contractNumber }}</span>
          </div>
          <div class="review-item">
            <span class="review-label">Date</span>
            <span class="review-value">{{ contractDate }}</span>
          </div>
          <div class="review-item" v-if="documentName">
            <span class="review-label">Document</span>
            <span class="review-value">{{ documentName }}</span>
          </div>
        </div>
      </div>
      <div class="review-section">
        <h4 class="review-subtitle">Parties ({{ parties.length }})</h4>
        <div v-for="(party, idx) in parties" :key="idx" class="review-party">
          <span class="review-party-name">{{ party.name }}</span>
          <span class="review-party-tin">TIN: {{ party.tin }}</span>
          <span class="role-badge" :class="'role-' + CONTRACT_PARTY_ROLES.find(r => r.value === party.role)?.key?.toLowerCase()">
            {{ getRoleLabel(party.role) }}
          </span>
        </div>
      </div>
    </div>

    <!-- Navigation -->
    <div class="step-navigation">
      <button v-if="step > 1" class="btn btn-secondary" @click="prevStep">Back</button>
      <div class="nav-spacer"></div>
      <button v-if="step < 3" class="btn btn-primary" @click="nextStep" :disabled="!canProceed">
        Next
        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M5 12h14M12 5l7 7-7 7" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </button>
      <button v-if="step === 3" class="btn btn-primary" @click="handleCreate" :disabled="saving">
        {{ saving ? 'Creating...' : 'Create Contract' }}
      </button>
    </div>
  </div>
</template>

<style scoped>
.contract-create-view { padding: 0; }

.page-header {
  display: flex; justify-content: space-between; align-items: center;
  margin-bottom: var(--spacing-xl);
}

.header-left { display: flex; align-items: center; gap: var(--spacing-md); }

.btn-back {
  width: 40px; height: 40px; display: flex; align-items: center; justify-content: center;
  background: var(--bg-card); border: 1px solid var(--border-color);
  border-radius: var(--radius-sm); cursor: pointer;
  color: var(--text-secondary); transition: all var(--transition-fast);
}
.btn-back:hover { background: var(--bg-secondary); color: var(--text-primary); }

.header-content h1 { font-size: 1.5rem; font-weight: 700; margin-bottom: 0.125rem; }
.subtitle { color: var(--text-secondary); font-size: 0.8125rem; }

/* Steps Bar */
.steps-bar {
  display: flex; gap: var(--spacing-xl);
  margin-bottom: var(--spacing-xl);
  padding: var(--spacing-md) var(--spacing-lg);
  background: var(--bg-card); border-radius: var(--radius-lg);
  box-shadow: var(--shadow-card);
}

.step-item {
  display: flex; align-items: center; gap: 0.5rem;
  color: var(--text-muted); font-size: 0.8125rem;
}

.step-number {
  width: 28px; height: 28px; display: flex; align-items: center; justify-content: center;
  border-radius: 50%; font-size: 0.75rem; font-weight: 600;
  background: var(--bg-secondary); color: var(--text-muted);
  transition: all var(--transition-fast);
}

.step-item.active .step-number { background: var(--color-brand); color: white; }
.step-item.active .step-label { color: var(--text-primary); font-weight: 600; }
.step-item.done .step-number { background: var(--color-success); color: white; }
.step-item.done .step-label { color: var(--color-success); }

/* Step Panel */
.step-panel {
  background: var(--bg-card); border-radius: var(--radius-xl);
  box-shadow: var(--shadow-card); padding: var(--spacing-xl);
  margin-bottom: var(--spacing-lg);
}

.panel-title { font-size: 1rem; font-weight: 600; margin-bottom: var(--spacing-lg); }

.section-header {
  display: flex; justify-content: space-between; align-items: center;
  margin-bottom: var(--spacing-lg);
}

.section-header .panel-title { margin-bottom: 0; }

.hint-text {
  font-size: 0.8125rem; color: var(--text-muted);
  text-align: center; padding: var(--spacing-xl) 0;
}

.btn-add {
  display: inline-flex; align-items: center; gap: 0.375rem;
  padding: 0.375rem 0.75rem; font-size: 0.75rem; font-weight: 600;
  color: var(--color-brand); background: var(--color-brand-light);
  border: none; border-radius: var(--radius-pill); cursor: pointer;
}
.btn-add:hover { background: rgba(255, 91, 60, 0.2); }

.form-grid {
  display: grid; grid-template-columns: 1fr 1fr;
  gap: var(--spacing-md);
}

/* Party Cards */
.parties-list { display: flex; flex-direction: column; gap: var(--spacing-md); }

.party-card {
  padding: var(--spacing-lg); background: var(--bg-secondary);
  border-radius: var(--radius-lg); border: 1px solid var(--border-color);
}

.party-card-header {
  display: flex; justify-content: space-between; align-items: center;
  margin-bottom: var(--spacing-md);
}

.party-label { font-size: 0.8125rem; font-weight: 600; color: var(--text-primary); }

.btn-remove {
  width: 28px; height: 28px; display: flex; align-items: center; justify-content: center;
  background: transparent; border: none; border-radius: var(--radius-xs); cursor: pointer;
  color: var(--text-muted); transition: all var(--transition-fast);
}
.btn-remove:hover { background: var(--color-error-light); color: var(--color-error); }

.party-form-grid {
  display: grid; grid-template-columns: 1fr 1fr;
  gap: var(--spacing-sm);
}

/* Review */
.review-section { margin-bottom: var(--spacing-lg); }

.review-grid {
  display: grid; grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
  gap: var(--spacing-md);
}

.review-item { display: flex; flex-direction: column; gap: 0.25rem; }
.review-label { font-size: 0.6875rem; font-weight: 600; color: var(--text-muted); text-transform: uppercase; letter-spacing: 0.05em; }
.review-value { font-size: 0.9375rem; font-weight: 500; color: var(--text-primary); }

.review-subtitle { font-size: 0.875rem; font-weight: 600; margin-bottom: var(--spacing-sm); }

.review-party {
  display: flex; align-items: center; gap: 0.75rem;
  padding: 0.625rem 0; border-bottom: 1px solid var(--border-color);
}
.review-party:last-child { border-bottom: none; }
.review-party-name { font-weight: 500; flex: 1; }
.review-party-tin { font-size: 0.75rem; color: var(--text-muted); font-family: monospace; }

.role-badge {
  display: inline-flex; padding: 0.25rem 0.5rem;
  font-size: 0.6875rem; font-weight: 600; border-radius: var(--radius-pill);
}
.role-owner { background: var(--color-brand-light); color: var(--color-brand); }
.role-agent { background: rgba(99, 102, 241, 0.1); color: #6366F1; }
.role-tenant { background: var(--color-success-light); color: var(--color-success); }

/* Navigation */
.step-navigation {
  display: flex; gap: var(--spacing-sm); align-items: center;
}
.nav-spacer { flex: 1; }

@media (max-width: 768px) {
  .form-grid, .party-form-grid { grid-template-columns: 1fr; }
}
</style>
