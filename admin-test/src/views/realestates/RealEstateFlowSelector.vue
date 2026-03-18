<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import FlowAQuickEntry from './flows/FlowAQuickEntry.vue'
import FlowBHubSpoke from './flows/FlowBHubSpoke.vue'
import FlowCConversational from './flows/FlowCConversational.vue'
import FlowDUnitsShowcase from './flows/FlowDUnitsShowcase.vue'
import FlowEUysotStyle from './flows/FlowEUysotStyle.vue'

const router = useRouter()

type FlowType = 'quick' | 'dashboard' | 'guided' | 'units' | 'uysot'

const flows: { id: FlowType; label: string; icon: string; description: string }[] = [
  { id: 'quick', label: 'Quick Entry', icon: '⚡', description: 'Fastest way to add a property' },
  { id: 'dashboard', label: 'Dashboard', icon: '📊', description: 'Fill sections in any order' },
  { id: 'guided', label: 'Guided', icon: '💬', description: 'Step-by-step questions' },
  { id: 'units', label: 'Units', icon: '🏢', description: 'Multi-unit property with glass UI' },
  { id: 'uysot', label: 'Competitors', icon: '🏡', description: '8 property types with full details' }
]

// Remember user's preference
const savedFlow = localStorage.getItem('preferred_realestate_flow') as FlowType | null
const selectedFlow = ref<FlowType>(savedFlow || 'quick')

function selectFlow(flow: FlowType) {
  selectedFlow.value = flow
  localStorage.setItem('preferred_realestate_flow', flow)
}

const currentFlowComponent = computed(() => {
  switch (selectedFlow.value) {
    case 'quick': return FlowAQuickEntry
    case 'dashboard': return FlowBHubSpoke
    case 'guided': return FlowCConversational
    case 'units': return FlowDUnitsShowcase
    case 'uysot': return FlowEUysotStyle
    default: return FlowAQuickEntry
  }
})
</script>

<template>
  <div class="flow-selector-view">
    <!-- Header -->
    <div class="page-header">
      <button class="btn-back" @click="router.back()">← Back</button>
      <h1>Add Property</h1>
    </div>

    <!-- Flow Tabs -->
    <div class="flow-tabs">
      <button
        v-for="flow in flows"
        :key="flow.id"
        class="flow-tab"
        :class="{ active: selectedFlow === flow.id }"
        @click="selectFlow(flow.id)"
      >
        <span class="flow-icon">{{ flow.icon }}</span>
        <span class="flow-label">{{ flow.label }}</span>
      </button>
    </div>

    <!-- Flow Description -->
    <p class="flow-description">
      {{ flows.find(f => f.id === selectedFlow)?.description }}
    </p>

    <!-- Flow Content -->
    <div class="flow-content">
      <component :is="currentFlowComponent" />
    </div>
  </div>
</template>

<style scoped>
.flow-selector-view {
  padding: 24px;
  max-width: 900px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-bottom: 24px;
}

.btn-back {
  padding: 8px 16px;
  background: #f3f4f6;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-size: 14px;
  color: #374151;
  transition: background 0.2s;
}

.btn-back:hover {
  background: #e5e7eb;
}

h1 {
  font-size: 24px;
  font-weight: 700;
  color: #1f2937;
  margin: 0;
}

.flow-tabs {
  display: flex;
  gap: 8px;
  padding: 6px;
  background: #f3f4f6;
  border-radius: 16px;
  margin-bottom: 12px;
}

.flow-tab {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  padding: 12px 16px;
  background: transparent;
  border: none;
  border-radius: 12px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  color: #6b7280;
  transition: all 0.2s ease;
}

.flow-tab:hover {
  background: rgba(255, 255, 255, 0.5);
  color: #374151;
}

.flow-tab.active {
  background: #fff;
  color: #1f2937;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
}

.flow-icon {
  font-size: 18px;
}

.flow-label {
  font-weight: 600;
}

.flow-description {
  text-align: center;
  color: #6b7280;
  font-size: 13px;
  margin-bottom: 24px;
}

.flow-content {
  background: #fff;
  border-radius: 20px;
  padding: 24px;
  box-shadow: 0 4px 24px rgba(0, 0, 0, 0.06);
  min-height: 400px;
}
</style>
