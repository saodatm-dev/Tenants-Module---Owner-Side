<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { api } from '@/services/api'
import type { ContractTemplateDetail, TemplateVariable } from '@/types/contractTemplate'
import { CONTRACT_TYPES } from '@/types/contractTemplate'

const router = useRouter()
const route = useRoute()

// State
const template = ref<ContractTemplateDetail | null>(null)
const loading = ref(false)
const error = ref('')

const templateId = computed(() => route.params.id as string)

const parsedVariables = computed<TemplateVariable[]>(() => {
  if (!template.value?.variablesJson) return []
  try {
    return JSON.parse(template.value.variablesJson) || []
  } catch {
    return []
  }
})

const typeLabel = computed(() => {
  if (!template.value) return ''
  return CONTRACT_TYPES.find(t => t.key === template.value!.type)?.label || template.value.type
})

const previewHtml = computed(() => {
  if (!template.value) return ''
  let html = template.value.htmlContent
  parsedVariables.value.forEach(v => {
    const regex = new RegExp(`\\{\\{${v.name}\\}\\}`, 'g')
    html = html.replace(regex, `<span style="background:#FFF3CD;padding:2px 6px;border-radius:4px;font-weight:500;font-family:sans-serif;font-size:0.8125rem;">{{${v.name}}}</span>`)
  })
  return html
})

async function loadTemplate() {
  loading.value = true
  error.value = ''
  try {
    // TODO: Enable when backend is ready
    // template.value = await api.getContractTemplate(templateId.value)
    template.value = null
  } catch (e: unknown) {
    const err = e as Error
    error.value = err.message || 'Failed to load template'
  } finally {
    loading.value = false
  }
}

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString('en-US', {
    year: 'numeric', month: 'long', day: 'numeric'
  })
}

function navigateToEdit() {
  router.push(`/templates/${templateId.value}/edit`)
}

function formatVarName(name: string): string {
  return `{{${name}}}`
}

function goBack() {
  router.push('/templates')
}

onMounted(() => {
  loadTemplate()
})
</script>

<template>
  <div class="template-detail-view">
    <div class="page-header">
      <div class="header-left">
        <button class="btn-back" @click="goBack">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M19 12H5" stroke-linecap="round"/>
            <path d="M12 19l-7-7 7-7" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
        </button>
        <div v-if="template" class="header-content">
          <h1>{{ template.name }}</h1>
          <div class="header-meta">
            <span class="type-badge" :class="'type-' + template.type.toLowerCase()">
              {{ typeLabel }}
            </span>
            <span v-if="template.isSystem" class="system-badge">
              <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"/>
              </svg>
              Platform Template
            </span>
            <span class="date-text">Created {{ formatDate(template.createdAt) }}</span>
          </div>
        </div>
      </div>
      <div v-if="template && !template.isSystem" class="header-actions">
        <button class="btn btn-primary" @click="navigateToEdit">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/>
            <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/>
          </svg>
          Edit
        </button>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Loading template...</p>
    </div>

    <!-- Error -->
    <div v-else-if="error" class="error-state">
      <p>{{ error }}</p>
      <button class="btn btn-secondary" @click="loadTemplate">Retry</button>
    </div>

    <!-- Detail Content -->
    <div v-else-if="template" class="detail-layout">
      <!-- Variables Section -->
      <div class="detail-panel variables-panel">
        <h3 class="panel-title">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M4 7h16M4 12h16M4 17h10" stroke-linecap="round"/>
          </svg>
          Variables
          <span class="count-badge">{{ parsedVariables.length }}</span>
        </h3>
        <div v-if="parsedVariables.length === 0" class="no-variables">
          <p>No variables defined</p>
        </div>
        <table v-else class="variables-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Label</th>
              <th>Type</th>
              <th>Required</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="v in parsedVariables" :key="v.name">
              <td class="var-name-cell"><code>{{ formatVarName(v.name) }}</code></td>
              <td>{{ v.label || '—' }}</td>
              <td>
                <span class="var-type-badge">{{ v.type }}</span>
              </td>
              <td>
                <svg v-if="v.required" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#16A05E" stroke-width="2.5">
                  <path d="M20 6L9 17l-5-5" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
                <span v-else class="text-muted">—</span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Preview Section -->
      <div class="detail-panel preview-panel">
        <h3 class="panel-title">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/>
            <circle cx="12" cy="12" r="3"/>
          </svg>
          Preview
        </h3>
        <div class="preview-content" v-html="previewHtml"></div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.template-detail-view {
  padding: 0;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: var(--spacing-xl);
}

.header-left {
  display: flex;
  align-items: flex-start;
  gap: var(--spacing-md);
}

.btn-back {
  width: 40px;
  height: 40px;
  min-width: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-sm);
  cursor: pointer;
  color: var(--text-secondary);
  transition: all var(--transition-fast);
  margin-top: 0.25rem;
}

.btn-back:hover {
  background: var(--bg-secondary);
  color: var(--text-primary);
}

.header-content h1 {
  font-size: 1.5rem;
  font-weight: 700;
  margin-bottom: 0.5rem;
}

.header-meta {
  display: flex;
  align-items: center;
  gap: 0.75rem;
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

.type-delegation { background: var(--color-brand-light); color: var(--color-brand); }
.type-lease { background: var(--color-success-light); color: var(--color-success); }
.type-handover { background: var(--color-warning-light); color: var(--color-warning); }
.type-communalbill { background: rgba(99, 102, 241, 0.1); color: #6366F1; }
.type-serviceact { background: rgba(139, 92, 246, 0.1); color: #8B5CF6; }
.type-reconciliation { background: rgba(20, 184, 166, 0.1); color: #14B8A6; }

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

.date-text {
  font-size: 0.8125rem;
  color: var(--text-muted);
}

.header-actions {
  display: flex;
  gap: var(--spacing-sm);
}

/* Detail Layout */
.detail-layout {
  display: grid;
  grid-template-columns: 1fr;
  gap: var(--spacing-lg);
}

.detail-panel {
  background: var(--bg-card);
  border-radius: var(--radius-xl);
  box-shadow: var(--shadow-card);
  padding: var(--spacing-lg);
}

.panel-title {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9375rem;
  font-weight: 600;
  margin-bottom: var(--spacing-lg);
  color: var(--text-primary);
}

.count-badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-width: 22px;
  height: 22px;
  padding: 0 0.375rem;
  font-size: 0.6875rem;
  font-weight: 600;
  background: var(--bg-secondary);
  border-radius: var(--radius-pill);
  color: var(--text-muted);
}

/* Variables Table */
.no-variables {
  text-align: center;
  padding: var(--spacing-lg);
  color: var(--text-muted);
  font-size: 0.875rem;
}

.variables-table {
  width: 100%;
  border-collapse: collapse;
}

.variables-table th {
  text-align: left;
  padding: 0.625rem 0.75rem;
  font-size: 0.6875rem;
  font-weight: 600;
  color: var(--text-muted);
  text-transform: uppercase;
  letter-spacing: 0.05em;
  border-bottom: 1px solid var(--border-color);
}

.variables-table td {
  padding: 0.75rem;
  font-size: 0.8125rem;
  border-bottom: 1px solid var(--border-color);
}

.variables-table tbody tr:last-child td {
  border-bottom: none;
}

.variables-table tbody tr:hover {
  background: var(--bg-secondary);
}

.var-name-cell code {
  padding: 0.125rem 0.5rem;
  font-size: 0.75rem;
  background: var(--color-brand-light);
  color: var(--color-brand);
  border-radius: var(--radius-xs);
  font-family: 'SF Mono', 'Fira Code', monospace;
}

.var-type-badge {
  display: inline-flex;
  padding: 0.125rem 0.5rem;
  font-size: 0.6875rem;
  font-weight: 500;
  background: var(--bg-secondary);
  color: var(--text-secondary);
  border-radius: var(--radius-pill);
}

/* Preview */
.preview-content {
  font-family: 'Times New Roman', serif;
  font-size: 14px;
  line-height: 1.8;
  color: #333;
  max-width: 700px;
  margin: 0 auto;
  padding: 2rem;
  background: white;
  border: 1px solid #e5e5e5;
  border-radius: var(--radius-sm);
  box-shadow: var(--shadow-md);
  min-height: 200px;
}

/* States */
.loading-state,
.error-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  gap: var(--spacing-md);
}
</style>
