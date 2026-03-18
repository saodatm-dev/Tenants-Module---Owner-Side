<script setup lang="ts">
import AppSidebar from '@/components/AppSidebar.vue'
import ClientSidebar from '@/components/ClientSidebar.vue'
import { useAuthStore } from '@/stores/auth'
import { api } from '@/services/api'
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import type { Account } from '@/types/auth'

const route = useRoute()
const authStore = useAuthStore()

// Current account data
const currentAccount = ref<Account | null>(null)
const accountsLoaded = ref(false)

// Check if sidebar should be hidden based on route meta
const hideSidebar = computed(() => !!route.meta.hideSidebar)

// Load current account to check accountType
async function loadCurrentAccount() {
  try {
    const accounts = await api.getMyAccounts()
    currentAccount.value = accounts.find((a: Account) => a.isCurrent) || null
    accountsLoaded.value = true
  } catch (e) {
    console.error('Failed to load accounts:', e)
    accountsLoaded.value = true
  }
}

// Determine if current account is owner based on accountType
// accountType: 0 = Client, 1 = Owner
const isOwner = computed(() => {
  // If accounts not loaded yet, default to owner (show AppSidebar)
  if (!accountsLoaded.value) return true
  // accountType 1 = Owner, accountType 0 = Client
  return currentAccount.value?.accountType === 1
})

onMounted(() => {
  if (authStore.isAuthenticated) {
    loadCurrentAccount()
  }
})
</script>

<template>
  <div class="main-layout">
    <template v-if="!hideSidebar">
      <ClientSidebar v-if="!isOwner" />
      <AppSidebar v-else />
    </template>
    <main class="main-content" :class="{ 'no-sidebar': hideSidebar }">
      <router-view v-slot="{ Component, route }">
        <component :is="Component" :key="route.path" />
      </router-view>
    </main>
  </div>
</template>

<style scoped>
.main-layout {
  display: flex;
  min-height: 100vh;
  width: 100%;
}

.main-content {
  flex: 1;
  min-width: 0;
  padding: 2rem;
  background: var(--color-background);
  overflow-y: auto;
}

.main-content.no-sidebar {
  padding: 0;
}
</style>
