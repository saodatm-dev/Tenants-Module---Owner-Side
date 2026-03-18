<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { computed, ref, onMounted } from 'vue'
import { api } from '@/services/api'
import type { Account } from '@/types/auth'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

// Account data
const accounts = ref<Account[]>([])
const showAccountMenu = ref(false)
const loadingAccounts = ref(false)
const creatingAccount = ref(false)

// Check if user already has an owner account (accountType 1)
const hasOwnerAccount = computed(() => accounts.value.some(a => a.accountType === 1))

// Current account info
const currentAccountName = computed(() => {
  return 'Мой аккаунт'
})

// Navigation items for client
const navItems = [
  { name: 'Главная', path: '/marketplace', iconId: 'home' },
  { name: 'Мои заявки', path: '/my-requests', iconId: 'orders' },
  { name: 'Настройки', path: '/settings', iconId: 'settings' },
]

function isActive(path: string): boolean {
  return route.path === path || route.path.startsWith(path + '/')
}

// Load accounts
async function loadAccounts() {
  loadingAccounts.value = true
  try {
    const response = await api.getMyAccounts()
    accounts.value = response || []
  } catch (e) {
    console.error('Failed to load accounts:', e)
  } finally {
    loadingAccounts.value = false
  }
}

// Check if account is owner based on accountType: 0 = Owner, 1 = Client
function isOwnerAccount(account: Account): boolean {
  return account.accountType === 1
}

// Get display name for an account from available fields
function getAccountDisplayName(account: Account): string {
  if (account.companyName) return account.companyName
  const parts = [account.firstName, account.lastName].filter(Boolean)
  return parts.length > 0 ? parts.join(' ') : 'Account'
}

// Switch account
async function switchAccount(account: Account) {
  try {
    const response = await api.changeAccount(account.key)
    authStore.setAuthData(response)
    showAccountMenu.value = false
    // Redirect based on accountType: 1 = Owner, 0 = Client
    if (account.accountType === 1) {
      window.location.href = '/dashboard'
    } else {
      window.location.href = '/marketplace'
    }
  } catch (e) {
    console.error('Failed to switch account:', e)
  }
}

// Create an owner account and switch to it
async function createOwnerAccount() {
  if (creatingAccount.value) return
  creatingAccount.value = true
  try {
    const response = await api.createOwnerAccount()
    authStore.setAuthData(response)
    window.location.href = '/dashboard'
  } catch (e) {
    console.error('Failed to create owner account:', e)
    alert('Failed to create owner account')
  } finally {
    creatingAccount.value = false
  }
}

// Logout
async function handleLogout() {
  await authStore.logout()
  router.push('/login')
}

onMounted(() => {
  loadAccounts()
})
</script>

<template>
  <aside class="client-sidebar">
    <!-- Logo -->
    <div class="sidebar-logo">
      <svg width="32" height="24" viewBox="0 0 138 101" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M0.859375 99.5781V1.57812L26.6094 51.0781L26.8594 51.3281L52.6094 1.57812V99.5781H38.3594V44.5781L26.8594 67.0781H26.6094L14.8594 43.8281V99.5781H0.859375Z" fill="#FC4D3D"/>
        <path d="M60.3203 99.5781L86.8203 1.57812H101.07L127.57 99.5781H112.82L107.32 78.3281H80.3203L74.8203 99.5781H60.3203ZM83.5703 66.3281H104.07L93.8203 26.5781L83.5703 66.3281Z" fill="#19191C"/>
        <path d="M124.523 14.5781C124.523 11.0781 125.523 8.32812 127.523 6.32812C129.59 4.26146 132.34 3.22812 135.773 3.22812V0.078125C130.84 0.078125 126.84 1.57812 123.773 4.57812C120.773 7.51146 119.273 11.4448 119.273 16.3781C119.273 21.2448 120.773 25.1781 123.773 28.1781C126.84 31.1115 130.84 32.5781 135.773 32.5781V29.4281C132.34 29.4281 129.59 28.4281 127.523 26.4281C125.523 24.3615 124.523 21.5781 124.523 18.0781" fill="#FC4D3D"/>
        <path d="M137.023 76.3281C137.023 79.8281 136.023 82.5781 134.023 84.5781C131.956 86.6448 129.206 87.6781 125.773 87.6781V90.8281C130.706 90.8281 134.706 89.3281 137.773 86.3281C140.773 83.3948 142.273 79.4615 142.273 74.5281C142.273 69.6615 140.773 65.7281 137.773 62.7281C134.706 59.7948 130.706 58.3281 125.773 58.3281V61.4781C129.206 61.4781 131.956 62.4781 134.023 64.4781C136.023 66.5448 137.023 69.3281 137.023 72.8281" fill="#19191C"/>
      </svg>
    </div>

    <!-- User Profile -->
    <div class="user-profile" @click="showAccountMenu = !showAccountMenu">
      <div class="user-avatar">
        <span>{{ currentAccountName.charAt(0) }}</span>
      </div>
      <div class="user-info">
        <span class="user-name">{{ currentAccountName }}</span>
        <span class="user-role">Арендатор</span>
      </div>
      <svg class="dropdown-icon" :class="{ rotated: showAccountMenu }" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <path d="m6 9 6 6 6-6"/>
      </svg>
    </div>

    <!-- Account Dropdown -->
    <div v-if="showAccountMenu" class="account-dropdown">
      <div v-if="loadingAccounts" class="account-loading">
        Загрузка...
      </div>
      <div v-else>
        <div
          v-for="account in accounts"
          :key="account.key"
          class="account-item"
          @click="switchAccount(account)"
        >
          <div class="account-avatar" :class="{ owner: isOwnerAccount(account) }">
            {{ getAccountDisplayName(account).charAt(0) }}
          </div>
          <div class="account-details">
            <span class="account-name">{{ getAccountDisplayName(account) }}</span>
            <span class="account-type">{{ isOwnerAccount(account) ? 'Владелец' : 'Арендатор' }}</span>
          </div>
        </div>
      </div>
      <!-- Create Owner Account button -->
      <div v-if="!hasOwnerAccount" class="create-account-section">
        <button
          class="create-account-btn"
          :disabled="creatingAccount"
          @click.stop="createOwnerAccount"
        >
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M12 5v14M5 12h14" stroke-linecap="round"/>
          </svg>
          {{ creatingAccount ? 'Создание...' : 'Создать аккаунт владельца' }}
        </button>
      </div>
      <div class="account-actions">
        <button class="logout-btn" @click="handleLogout">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"/>
            <polyline points="16 17 21 12 16 7"/>
            <line x1="21" y1="12" x2="9" y2="12"/>
          </svg>
          Выйти
        </button>
      </div>
    </div>

    <!-- Navigation -->
    <nav class="sidebar-nav">
      <router-link
        v-for="item in navItems"
        :key="item.path"
        :to="item.path"
        class="nav-item"
        :class="{ active: isActive(item.path) }"
      >
        <!-- Home Icon -->
        <svg v-if="item.iconId === 'home'" class="nav-icon" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
          <path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z"/>
          <polyline points="9 22 9 12 15 12 15 22"/>
        </svg>

        <!-- Orders Icon -->
        <svg v-else-if="item.iconId === 'orders'" class="nav-icon" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
          <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/>
          <polyline points="14 2 14 8 20 8"/>
          <line x1="12" y1="18" x2="12" y2="12"/>
          <line x1="9" y1="15" x2="15" y2="15"/>
        </svg>

        <!-- Settings Icon -->
        <svg v-else-if="item.iconId === 'settings'" class="nav-icon" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
          <circle cx="12" cy="12" r="3"/>
          <path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 0 1 0 2.83 2 2 0 0 1-2.83 0l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-2 2 2 2 0 0 1-2-2v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 0 1-2.83 0 2 2 0 0 1 0-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1-2-2 2 2 0 0 1 2-2h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 0 1 0-2.83 2 2 0 0 1 2.83 0l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 2-2 2 2 0 0 1 2 2v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 0 1 2.83 0 2 2 0 0 1 0 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 2 2 2 2 0 0 1-2 2h-.09a1.65 1.65 0 0 0-1.51 1z"/>
        </svg>

        <span class="nav-label">{{ item.name }}</span>
      </router-link>
    </nav>
  </aside>
</template>

<style scoped>
.client-sidebar {
  width: 260px;
  min-width: 260px;
  height: 100vh;
  background: #FFFFFF;
  border-right: 1px solid rgba(27, 27, 27, 0.06);
  display: flex;
  flex-direction: column;
  padding: 20px 16px;
  position: sticky;
  top: 0;
  z-index: 100;
}

.sidebar-logo {
  padding: 8px 12px;
  margin-bottom: 24px;
}

/* User Profile */
.user-profile {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px;
  border-radius: 16px;
  cursor: pointer;
  transition: background 0.2s;
  margin-bottom: 8px;
}

.user-profile:hover {
  background: rgba(27, 27, 27, 0.04);
}

.user-avatar {
  width: 40px;
  height: 40px;
  border-radius: 12px;
  background: linear-gradient(135deg, #FF5B3C 0%, #FF8A65 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 600;
  font-size: 16px;
}

.user-info {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.user-name {
  font-size: 14px;
  font-weight: 600;
  color: #1B1B1B;
}

.user-role {
  font-size: 12px;
  color: rgba(27, 27, 27, 0.5);
}

.dropdown-icon {
  color: rgba(27, 27, 27, 0.4);
  transition: transform 0.2s;
}

.dropdown-icon.rotated {
  transform: rotate(180deg);
}

/* Account Dropdown */
.account-dropdown {
  background: #FFFFFF;
  border: 1px solid rgba(27, 27, 27, 0.08);
  border-radius: 16px;
  margin-bottom: 16px;
  padding: 8px;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.08);
}

.account-loading {
  padding: 16px;
  text-align: center;
  color: rgba(27, 27, 27, 0.5);
  font-size: 13px;
}

.account-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 10px 12px;
  border-radius: 12px;
  cursor: pointer;
  transition: background 0.2s;
}

.account-item:hover {
  background: rgba(27, 27, 27, 0.04);
}

.account-avatar {
  width: 36px;
  height: 36px;
  border-radius: 10px;
  background: linear-gradient(135deg, #42A5F5 0%, #64B5F6 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 600;
  font-size: 14px;
}

.account-avatar.owner {
  background: linear-gradient(135deg, #FF5B3C 0%, #FF8A65 100%);
}

.account-details {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.account-name {
  font-size: 13px;
  font-weight: 600;
  color: #1B1B1B;
}

.account-type {
  font-size: 11px;
  color: rgba(27, 27, 27, 0.5);
}

.account-actions {
  border-top: 1px solid rgba(27, 27, 27, 0.06);
  padding-top: 8px;
  margin-top: 4px;
}

.logout-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  width: 100%;
  padding: 10px 12px;
  border: none;
  background: transparent;
  border-radius: 12px;
  color: #F44336;
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  transition: background 0.2s;
}

.logout-btn:hover {
  background: rgba(244, 67, 54, 0.08);
}

/* Navigation */
.sidebar-nav {
  display: flex;
  flex-direction: column;
  gap: 4px;
  flex: 1;
}

.nav-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 16px;
  border-radius: 14px;
  text-decoration: none;
  color: #1B1B1B;
  font-size: 14px;
  font-weight: 500;
  transition: all 0.2s;
}

.nav-item:hover {
  background: rgba(27, 27, 27, 0.04);
}

.nav-item.active {
  background: rgba(255, 91, 60, 0.08);
  color: #FF5B3C;
}

.nav-icon {
  width: 20px;
  height: 20px;
  flex-shrink: 0;
}

.nav-label {
  font-size: 14px;
}

.create-account-section {
  border-top: 1px solid rgba(27, 27, 27, 0.06);
  padding: 8px;
}

.create-account-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  width: 100%;
  padding: 10px 12px;
  border: none;
  background: transparent;
  border-radius: 8px;
  font-size: 13px;
  font-weight: 500;
  color: #FF5B3C;
  cursor: pointer;
  transition: background 0.2s;
}

.create-account-btn:hover:not(:disabled) {
  background: rgba(255, 91, 60, 0.08);
}

.create-account-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.create-account-btn svg {
  flex-shrink: 0;
}
</style>
