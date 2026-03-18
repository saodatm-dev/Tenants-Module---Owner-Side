<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '@/services/api'
import type { ContractTemplate } from '@/types/contractTemplate'
import { CONTRACT_TYPES } from '@/types/contractTemplate'

const router = useRouter()

// State
const templates = ref<ContractTemplate[]>([])
const loading = ref(false)
const error = ref('')

// Filters
const selectedType = ref('')
const searchQuery = ref('')

// Filtered list
const filteredTemplates = computed(() => {
  let result = templates.value
  if (searchQuery.value) {
    const q = searchQuery.value.toLowerCase()
    result = result.filter(t => t.name.toLowerCase().includes(q))
  }
  return result
})

// Load templates
async function loadTemplates() {
  loading.value = true
  error.value = ''
  try {
    // TODO: Enable when backend is ready
    // templates.value = await api.getContractTemplates(selectedType.value || undefined)
    templates.value = []
  } catch (e: unknown) {
    const err = e as Error
    error.value = err.message || 'Failed to load templates'
    console.error('Failed to load templates:', e)
  } finally {
    loading.value = false
  }
}

// Get type label
function getTypeLabel(typeKey: string): string {
  return CONTRACT_TYPES.find(t => t.key === typeKey)?.label || typeKey
}

// Get variable count from JSON
function getVariableCount(variablesJson: string): number {
  try {
    return JSON.parse(variablesJson)?.length || 0
  } catch {
    return 0
  }
}

// Format date
function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString('en-US', {
    year: 'numeric', month: 'short', day: 'numeric'
  })
}

// Navigation
function navigateToCreate() {
  router.push('/templates/new')
}

function navigateToDetail(id: string) {
  router.push(`/templates/${id}`)
}

function navigateToEdit(id: string, event: Event) {
  event.stopPropagation()
  router.push(`/templates/${id}/edit`)
}

// Lifecycle
onMounted(() => {
  loadTemplates()
})
</script>

<template>
  <div class="templates-list-view">
    <div class="page-header">
      <div class="header-content">
        <h1>Contract Templates</h1>
        <p class="subtitle">Manage your company's contract templates</p>
      </div>
      <button class="btn btn-primary" @click="navigateToCreate">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M12 5v14"></path>
          <path d="M5 12h14"></path>
        </svg>
        New Template
      </button>
    </div>

    <!-- Filters -->
    <div class="filters-section">
      <div class="filter-group">
        <label>Contract Type</label>
        <select v-model="selectedType" @change="loadTemplates">
          <option value="">All Types</option>
          <option v-for="ct in CONTRACT_TYPES" :key="ct.value" :value="ct.value">
            {{ ct.label }}
          </option>
        </select>
      </div>
      <div class="filter-group">
        <label>Search</label>
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search templates..."
        />
      </div>
    </div>

    <!-- Loading state -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Loading templates...</p>
    </div>

    <!-- Error state -->
    <div v-else-if="error" class="error-state">
      <p>{{ error }}</p>
      <button class="btn btn-secondary" @click="loadTemplates">Retry</button>
    </div>

    <!-- Empty state -->
    <div v-else-if="filteredTemplates.length === 0" class="empty-state">
      <div class="empty-icon">
        <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" opacity="0.4">
          <path d="M14 2H6C4.89543 2 4 2.89543 4 4V20C4 21.1046 4.89543 22 6 22H18C19.1046 22 20 21.1046 20 20V8L14 2Z" stroke-linecap="round" stroke-linejoin="round"/>
          <path d="M14 2V8H20" stroke-linecap="round" stroke-linejoin="round"/>
          <path d="M8 13H16M8 17H13" stroke-linecap="round"/>
        </svg>
      </div>
      <h3>No Templates</h3>
      <p>Create your first contract template</p>
      <button class="btn btn-primary" @click="navigateToCreate">
        New Template
      </button>
    </div>

    <!-- Templates grid -->
    <div v-else class="templates-grid">
      <div
        v-for="template in filteredTemplates"
        :key="template.id"
        class="template-card"
        @click="navigateToDetail(template.id)"
      >
        <div class="card-icon">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
            <path d="M14 2H6C4.89543 2 4 2.89543 4 4V20C4 21.1046 4.89543 22 6 22H18C19.1046 22 20 21.1046 20 20V8L14 2Z" stroke-linecap="round" stroke-linejoin="round"/>
            <path d="M14 2V8H20" stroke-linecap="round" stroke-linejoin="round"/>
            <path d="M8 13H16M8 17H13" stroke-linecap="round"/>
          </svg>
        </div>
        <div class="card-body">
          <h3 class="template-name">{{ template.name }}</h3>
          <div class="template-meta">
            <span class="type-badge" :class="'type-' + template.type.toLowerCase()">
              {{ getTypeLabel(template.type) }}
            </span>
            <span v-if="template.isSystem" class="system-badge">
              <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"/>
              </svg>
              Platform
            </span>
          </div>
          <div class="template-info">
            <span class="info-item">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M4 7h16M4 12h16M4 17h10" stroke-linecap="round"/>
              </svg>
              {{ getVariableCount(template.variablesJson) }} variables
            </span>
            <span class="info-item">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="12" cy="12" r="10"/>
                <path d="M12 6v6l4 2"/>
              </svg>
              {{ formatDate(template.createdAt) }}
            </span>
          </div>
        </div>
        <div class="card-actions" @click.stop>
          <button class="btn-icon" @click="navigateToEdit(template.id, $event)" title="Edit">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/>
              <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/>
            </svg>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.templates-list-view {
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

.filter-group select,
.filter-group input {
  padding: 0.625rem 1rem;
  font-size: 0.875rem;
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-sm);
  color: var(--text-primary);
  min-width: 180px;
  transition: all var(--transition-fast);
}

.filter-group select:focus,
.filter-group input:focus {
  outline: none;
  border-color: var(--color-brand);
  box-shadow: 0 0 0 3px var(--border-focus);
}

/* States */
.loading-state,
.error-state,
.empty-state {
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

.empty-state h3 {
  font-size: 1.125rem;
  font-weight: 600;
}

.empty-state p {
  color: var(--text-muted);
  font-size: 0.875rem;
}

/* Templates grid */
.templates-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(340px, 1fr));
  gap: var(--spacing-lg);
}

.template-card {
  display: flex;
  align-items: flex-start;
  gap: var(--spacing-md);
  padding: var(--spacing-lg);
  background: var(--bg-card);
  border-radius: var(--radius-xl);
  box-shadow: var(--shadow-card);
  cursor: pointer;
  transition: all var(--transition-base);
  position: relative;
}

.template-card:hover {
  box-shadow: var(--shadow-card-hover);
  transform: translateY(-2px);
}

.card-icon {
  width: 44px;
  height: 44px;
  min-width: 44px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--color-brand-light);
  border-radius: var(--radius-md);
  color: var(--color-brand);
}

.card-body {
  flex: 1;
  min-width: 0;
}

.template-name {
  font-size: 0.9375rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.template-meta {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.75rem;
  flex-wrap: wrap;
}

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

.type-delegation {
  background: var(--color-brand-light);
  color: var(--color-brand);
}

.type-lease {
  background: var(--color-success-light);
  color: var(--color-success);
}

.type-handover {
  background: var(--color-warning-light);
  color: var(--color-warning);
}

.type-communalbill {
  background: rgba(99, 102, 241, 0.1);
  color: #6366F1;
}

.type-serviceact {
  background: rgba(139, 92, 246, 0.1);
  color: #8B5CF6;
}

.type-reconciliation {
  background: rgba(20, 184, 166, 0.1);
  color: #14B8A6;
}

.system-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.5rem;
  font-size: 0.6875rem;
  font-weight: 500;
  border-radius: var(--radius-pill);
  background: var(--color-primary-light);
  color: var(--text-secondary);
}

.template-info {
  display: flex;
  gap: 1rem;
}

.info-item {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.75rem;
  color: var(--text-muted);
}

.card-actions {
  position: absolute;
  top: var(--spacing-md);
  right: var(--spacing-md);
  display: flex;
  gap: 0.25rem;
  opacity: 0;
  transition: opacity var(--transition-fast);
}

.template-card:hover .card-actions {
  opacity: 1;
}

.btn-icon {
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--bg-secondary);
  border: none;
  border-radius: var(--radius-xs);
  cursor: pointer;
  color: var(--text-secondary);
  transition: all var(--transition-fast);
}

.btn-icon:hover {
  background: var(--color-primary-light);
  color: var(--text-primary);
}
</style>
