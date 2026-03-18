<script setup lang="ts">
interface Tab {
  id: string
  label: string
  icon?: string
}

defineProps<{
  tabs: Tab[]
  modelValue: string
}>()

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

function selectTab(tabId: string) {
  emit('update:modelValue', tabId)
}
</script>

<template>
  <div class="tab-bar">
    <button
      v-for="tab in tabs"
      :key="tab.id"
      class="tab-item"
      :class="{ active: modelValue === tab.id }"
      @click="selectTab(tab.id)"
    >
      <span v-if="tab.icon" class="tab-icon">{{ tab.icon }}</span>
      {{ tab.label }}
    </button>
  </div>
</template>

<style scoped>
/* TabBar - Figma Exact Values */
.tab-bar {
  display: inline-flex;
  gap: 4px;
  padding: 4px;
  background: #F6F6F6;
  border-radius: 100px;
  height: 36px;
}

.tab-item {
  padding: 0 16px;
  height: 28px;
  font-size: 13px;
  font-weight: 500;
  line-height: 28px;
  color: rgba(27, 27, 27, 0.5);
  background: transparent;
  border: none;
  border-radius: 100px;
  cursor: pointer;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  gap: 6px;
  white-space: nowrap;
}

.tab-item:hover:not(.active) {
  color: #1B1B1B;
  background: rgba(27, 27, 27, 0.04);
}

.tab-item.active {
  background: #FFFFFF;
  color: #1B1B1B;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.06);
}

.tab-icon {
  font-size: 14px;
}
</style>
