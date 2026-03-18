<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { api } from '@/services/api'
import type { TemplateVariable, CreateTemplateData, UpdateTemplateData } from '@/types/contractTemplate'
import { CONTRACT_TYPES } from '@/types/contractTemplate'

const props = defineProps<{
  editMode?: boolean
}>()

const router = useRouter()
const route = useRoute()

// State
const loading = ref(false)
const saving = ref(false)
const error = ref('')
const activeTab = ref<'editor' | 'preview'>('editor')

// Form fields
const name = ref('')
const selectedType = ref(0)
const htmlContent = ref('')
const variables = ref<TemplateVariable[]>([])

// Computed
const isEdit = computed(() => props.editMode || route.name === 'template-edit')
const templateId = computed(() => route.params.id as string)
const pageTitle = computed(() => isEdit.value ? 'Edit Template' : 'New Template')

const previewHtml = computed(() => {
  let html = htmlContent.value
  variables.value.forEach(v => {
    const regex = new RegExp(`\\{\\{${v.name}\\}\\}`, 'g')
    html = html.replace(regex, `<span style="background:#FFF3CD;padding:2px 6px;border-radius:4px;font-weight:500;">{{${v.name}}}</span>`)
  })
  return html
})

// Load template for editing
async function loadTemplate() {
  if (!isEdit.value || !templateId.value) return
  loading.value = true
  try {
    // TODO: Enable when backend is ready
    // const template = await api.getContractTemplate(templateId.value)
    // name.value = template.name
    // selectedType.value = CONTRACT_TYPES.find(t => t.key === template.type)?.value ?? 0
    // htmlContent.value = template.htmlContent
    // try {
    //   variables.value = JSON.parse(template.variablesJson) || []
    // } catch {
    //   variables.value = []
    // }
  } catch (e: unknown) {
    const err = e as Error
    error.value = err.message || 'Failed to load template'
  } finally {
    loading.value = false
  }
}

// Add variable
function addVariable() {
  variables.value.push({
    name: '',
    label: '',
    type: 'text',
    required: true
  })
}

// Remove variable
function removeVariable(index: number) {
  variables.value.splice(index, 1)
}

// Insert variable into HTML
function insertVariable(varName: string) {
  htmlContent.value += `{{${varName}}}`
}

// Save
async function handleSave() {
  if (!name.value.trim()) {
    error.value = 'Template name is required'
    return
  }

  saving.value = true
  error.value = ''

  const variablesJson = JSON.stringify(variables.value)

  try {
    if (isEdit.value && templateId.value) {
      // TODO: Enable when backend is ready
      // const data: UpdateTemplateData = {
      //   name: name.value,
      //   htmlContent: htmlContent.value,
      //   variablesJson,
      // }
      // await api.updateContractTemplate(templateId.value, data)
      // router.push(`/templates/${templateId.value}`)
    } else {
      // TODO: Enable when backend is ready
      // const data: CreateTemplateData = {
      //   name: name.value,
      //   type: selectedType.value,
      //   htmlContent: htmlContent.value,
      //   variablesJson,
      // }
      // const id = await api.createContractTemplate(data)
      // router.push(`/templates/${id}`)
    }
  } catch (e: unknown) {
    const err = e as Error
    error.value = err.message || 'Failed to save template'
  } finally {
    saving.value = false
  }
}

function goBack() {
  router.push('/templates')
}

// Lifecycle
onMounted(() => {
  loadTemplate()
})
</script>

<template>
  <div class="template-form-view">
    <div class="page-header">
      <div class="header-left">
        <button class="btn-back" @click="goBack">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M19 12H5" stroke-linecap="round"/>
            <path d="M12 19l-7-7 7-7" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
        </button>
        <div class="header-content">
          <h1>{{ pageTitle }}</h1>
          <p class="subtitle">{{ isEdit ? 'Update your contract template' : 'Define a reusable contract template' }}</p>
        </div>
      </div>
      <div class="header-actions">
        <button class="btn btn-secondary" @click="goBack">Cancel</button>
        <button class="btn btn-primary" @click="handleSave" :disabled="saving">
          <div v-if="saving" class="spinner" style="width:16px;height:16px;"></div>
          {{ saving ? 'Saving...' : (isEdit ? 'Update' : 'Create') }}
        </button>
      </div>
    </div>

    <!-- Error -->
    <div v-if="error" class="alert alert-error">
      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <circle cx="12" cy="12" r="10"/>
        <path d="M15 9l-6 6M9 9l6 6"/>
      </svg>
      {{ error }}
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Loading template...</p>
    </div>

    <!-- Form -->
    <div v-else class="form-layout">
      <!-- Left Panel: Metadata & Variables -->
      <div class="panel panel-left">
        <div class="panel-section">
          <h3 class="section-title">Template Info</h3>
          <div class="form-group">
            <label class="form-label">Name</label>
            <input v-model="name" class="form-input" placeholder="e.g. Delegation Agreement" />
          </div>
          <div class="form-group">
            <label class="form-label">Contract Type</label>
            <select v-model="selectedType" class="form-input" :disabled="isEdit">
              <option v-for="ct in CONTRACT_TYPES" :key="ct.value" :value="ct.value">
                {{ ct.label }}
              </option>
            </select>
          </div>
        </div>

        <div class="panel-section">
          <div class="section-header">
            <h3 class="section-title">Variables</h3>
            <button class="btn-add" @click="addVariable">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                <path d="M12 5v14M5 12h14" stroke-linecap="round"/>
              </svg>
              Add
            </button>
          </div>
          <p v-if="variables.length === 0" class="hint-text">
            Add variables that will be replaced when using this template.
          </p>
          <div class="variables-list">
            <div v-for="(variable, idx) in variables" :key="idx" class="variable-row">
              <div class="var-fields">
                <input
                  v-model="variable.name"
                  class="var-input"
                  placeholder="variable_name"
                />
                <input
                  v-model="variable.label"
                  class="var-input"
                  placeholder="Display Label"
                />
                <select v-model="variable.type" class="var-select">
                  <option value="text">Text</option>
                  <option value="number">Number</option>
                  <option value="date">Date</option>
                </select>
                <label class="var-checkbox">
                  <input type="checkbox" v-model="variable.required" />
                  <span>Req</span>
                </label>
              </div>
              <div class="var-actions">
                <button class="btn-insert" @click="insertVariable(variable.name)" title="Insert into HTML" :disabled="!variable.name">
                  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M15 3h6v6M9 21H3v-6M21 3l-7 7M3 21l7-7" stroke-linecap="round" stroke-linejoin="round"/>
                  </svg>
                </button>
                <button class="btn-remove" @click="removeVariable(idx)" title="Remove">
                  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M18 6L6 18M6 6l12 12" stroke-linecap="round"/>
                  </svg>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Right Panel: HTML Editor & Preview -->
      <div class="panel panel-right">
        <div class="editor-tabs">
          <button
            class="tab-btn"
            :class="{ active: activeTab === 'editor' }"
            @click="activeTab = 'editor'"
          >
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M16 18l6-6-6-6M8 6l-6 6 6 6" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
            Editor
          </button>
          <button
            class="tab-btn"
            :class="{ active: activeTab === 'preview' }"
            @click="activeTab = 'preview'"
          >
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/>
              <circle cx="12" cy="12" r="3"/>
            </svg>
            Preview
          </button>
        </div>

        <div v-if="activeTab === 'editor'" class="editor-container">
          <textarea
            v-model="htmlContent"
            class="html-editor"
            placeholder="<h1>Contract Title</h1>
<p>This agreement is entered between {{owner_name}} and {{tenant_name}}...</p>"
            spellcheck="false"
          ></textarea>
        </div>

        <div v-else class="preview-container">
          <div class="preview-content" v-html="previewHtml"></div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.template-form-view {
  padding: 0;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-xl);
}

.header-left {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
}

.btn-back {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-sm);
  cursor: pointer;
  color: var(--text-secondary);
  transition: all var(--transition-fast);
}

.btn-back:hover {
  background: var(--bg-secondary);
  color: var(--text-primary);
}

.header-content h1 {
  font-size: 1.5rem;
  font-weight: 700;
  margin-bottom: 0.125rem;
}

.subtitle {
  color: var(--text-secondary);
  font-size: 0.8125rem;
}

.header-actions {
  display: flex;
  gap: var(--spacing-sm);
}

/* Form Layout */
.form-layout {
  display: grid;
  grid-template-columns: 380px 1fr;
  gap: var(--spacing-lg);
  align-items: start;
}

.panel {
  background: var(--bg-card);
  border-radius: var(--radius-xl);
  box-shadow: var(--shadow-card);
  overflow: hidden;
}

.panel-section {
  padding: var(--spacing-lg);
  border-bottom: 1px solid var(--border-color);
}

.panel-section:last-child {
  border-bottom: none;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-md);
}

.section-title {
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--text-primary);
  margin-bottom: var(--spacing-md);
}

.section-header .section-title {
  margin-bottom: 0;
}

.hint-text {
  font-size: 0.8125rem;
  color: var(--text-muted);
  text-align: center;
  padding: var(--spacing-lg) 0;
}

.btn-add {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.75rem;
  font-size: 0.75rem;
  font-weight: 600;
  color: var(--color-brand);
  background: var(--color-brand-light);
  border: none;
  border-radius: var(--radius-pill);
  cursor: pointer;
  transition: all var(--transition-fast);
}

.btn-add:hover {
  background: rgba(255, 91, 60, 0.2);
}

/* Variables */
.variables-list {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.variable-row {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem;
  background: var(--bg-secondary);
  border-radius: var(--radius-sm);
}

.var-fields {
  display: grid;
  grid-template-columns: 1fr 1fr 70px 40px;
  gap: 0.375rem;
  flex: 1;
}

.var-input,
.var-select {
  padding: 0.5rem 0.625rem;
  font-size: 0.75rem;
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-xs);
  color: var(--text-primary);
  font-family: 'Inter', sans-serif;
}

.var-input:focus,
.var-select:focus {
  outline: none;
  border-color: var(--color-brand);
}

.var-checkbox {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.6875rem;
  color: var(--text-muted);
  cursor: pointer;
}

.var-checkbox input {
  accent-color: var(--color-brand);
}

.var-actions {
  display: flex;
  gap: 0.25rem;
}

.btn-insert,
.btn-remove {
  width: 28px;
  height: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: transparent;
  border: none;
  border-radius: var(--radius-xs);
  cursor: pointer;
  color: var(--text-muted);
  transition: all var(--transition-fast);
}

.btn-insert:hover {
  background: var(--color-brand-light);
  color: var(--color-brand);
}

.btn-remove:hover {
  background: var(--color-error-light);
  color: var(--color-error);
}

.btn-insert:disabled {
  opacity: 0.3;
  cursor: not-allowed;
}

/* Editor */
.editor-tabs {
  display: flex;
  gap: 0;
  padding: var(--spacing-sm) var(--spacing-md);
  border-bottom: 1px solid var(--border-color);
  background: var(--bg-secondary);
}

.tab-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.5rem 1rem;
  font-size: 0.8125rem;
  font-weight: 500;
  background: transparent;
  border: none;
  border-radius: var(--radius-xs);
  cursor: pointer;
  color: var(--text-muted);
  transition: all var(--transition-fast);
}

.tab-btn.active {
  background: var(--bg-card);
  color: var(--text-primary);
  box-shadow: var(--shadow-sm);
}

.tab-btn:hover:not(.active) {
  color: var(--text-primary);
}

.editor-container {
  padding: 0;
}

.html-editor {
  width: 100%;
  min-height: 500px;
  padding: var(--spacing-lg);
  font-family: 'SF Mono', 'Fira Code', 'Consolas', monospace;
  font-size: 0.8125rem;
  line-height: 1.7;
  color: var(--text-primary);
  background: var(--bg-card);
  border: none;
  resize: vertical;
}

.html-editor:focus {
  outline: none;
}

.html-editor::placeholder {
  color: var(--text-muted);
}

.preview-container {
  padding: var(--spacing-lg);
  min-height: 500px;
}

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
}

/* Loading */
.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  gap: var(--spacing-md);
}

@media (max-width: 900px) {
  .form-layout {
    grid-template-columns: 1fr;
  }
}
</style>
