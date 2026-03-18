<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { api } from '@/services/api'
import type { Account } from '@/types/auth'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const userInfo = computed(() => authStore.userInfo)

// Account switching state
const accounts = ref<Account[]>([])
const showAccountDropdown = ref(false)
const switchingAccount = ref(false)
const creatingAccount = ref(false)

// Check if user already has a client account (accountType 0)
const hasClientAccount = computed(() => accounts.value.some(a => a.accountType === 0))

interface NavItem {
  name: string
  path: string
  iconId: string
  disabled?: boolean
}

const navItems: NavItem[] = [
  { name: 'Dashboard', path: '/dashboard', iconId: 'dashboard' },
  { name: 'Real Estate', path: '/realestates', iconId: 'realestate' },
  { name: 'Requests', path: '/requests', iconId: 'orders' },
  { name: 'Listings', path: '/listings', iconId: 'monitoring' },
  { name: 'Invitations', path: '/invitations', iconId: 'invitations' },
  { name: 'Templates', path: '/templates', iconId: 'templates' },
  { name: 'Contracts', path: '/contracts', iconId: 'contracts' },
  { name: 'Communal', path: '/communal', iconId: 'communal' },
  // { name: 'Create Listing', path: '/listing-wizard', iconId: 'create-listing' }, // Temporarily hidden
  { name: 'Settings', path: '/settings', iconId: 'settings' },
]

function isActive(path: string): boolean {
  return route.path === path || route.path.startsWith(path + '/')
}

// Load available accounts
async function loadAccounts() {
  try {
    accounts.value = await api.getMyAccounts()
    // Load photos via presigned URLs

  } catch (e) {
    console.error('Failed to load accounts:', e)
  }
}

// Get account photo URL - photo is now a direct URL from API
function getAccountPhoto(account: Account): string | null {
  return account.photo || null
}

// Get current account from list
const currentAccount = computed(() => accounts.value.find(a => a.isCurrent))

// Get current account photo
const currentAccountPhoto = computed(() => {
  const current = currentAccount.value
  if (!current) return null
  return getAccountPhoto(current)
})

// Get display name for an account from available fields
function getAccountDisplayName(account: Account): string {
  if (account.companyName) return account.companyName
  const parts = [account.firstName, account.lastName].filter(Boolean)
  return parts.length > 0 ? parts.join(' ') : 'Account'
}

// Get first name from full name
function getFirstName(fullName: string | undefined | null): string {
  if (!fullName) return 'User'
  const firstName = fullName.split(' ')[0]
  return firstName || 'User'
}

// Switch to another account
async function switchAccount(account: Account) {
  if (account.isCurrent || switchingAccount.value) return

  switchingAccount.value = true
  try {
    const response = await api.changeAccount(account.key)
    // Store new tokens
    authStore.setAuthData(response)
    // Redirect based on accountType: 1 = Owner, 0 = Client
    if (account.accountType === 1) {
      window.location.href = '/dashboard'
    } else {
      window.location.href = '/marketplace'
    }
  } catch (e) {
    console.error('Failed to switch account:', e)
    alert('Failed to switch account')
  } finally {
    switchingAccount.value = false
    showAccountDropdown.value = false
  }
}

// Create a client account and switch to it
async function createClientAccount() {
  if (creatingAccount.value) return
  creatingAccount.value = true
  try {
    const response = await api.createClientAccount()
    authStore.setAuthData(response)
    window.location.href = '/marketplace'
  } catch (e) {
    console.error('Failed to create client account:', e)
    alert('Failed to create client account')
  } finally {
    creatingAccount.value = false
  }
}

// Get account type label based on accountType: 0 = Client, 1 = Owner
function getAccountTypeLabel(account: Account): string {
  return account.accountType === 1 ? 'Owner' : 'Client'
}

async function handleLogout() {
  await authStore.logout()
  router.push('/login')
}

onMounted(() => {
  loadAccounts()
})
</script>

<template>
  <aside class="sidebar">
    <!-- Logo -->
    <div class="sidebar-logo">
      <svg xmlns="http://www.w3.org/2000/svg" width="123" height="22" viewBox="0 0 123 22" fill="none">
        <g clip-path="url(#clip0_sidebar)">
          <path d="M0 6.72256V21.7456H5.92605V10.0847L11.8521 6.72256V0L0 6.72256Z" fill="#FC4D3D"/>
          <path d="M17.7781 3.36214L11.8521 6.72256V21.7456H17.7781V10.0847L23.7041 6.72256V0L17.7781 3.36214Z" fill="#FC4D3D"/>
          <path d="M23.7041 21.7457H29.6319V10.0848L23.7041 6.72266V21.7457Z" fill="#FC4D3D"/>
          <path d="M29.6318 3.36214V10.0847L35.5579 6.72256V0L29.6318 3.36214Z" fill="#FC4D3D"/>
          <path d="M105.535 3.10087C105.6 3.08024 105.659 3.05789 105.72 3.03211C106.014 2.90663 106.24 2.72099 106.397 2.47863C106.557 2.23971 106.634 1.95265 106.634 1.62091C106.634 1.1104 106.459 0.713337 106.107 0.428002C105.757 0.142667 105.278 0 104.667 0H102.87V4.72178H103.376V3.22463H104.667C104.802 3.22463 104.933 3.21775 105.055 3.19884L106.151 4.72178H106.711L105.535 3.09915V3.10087ZM104.655 2.80006H103.378V0.43144H104.655C105.138 0.43144 105.502 0.534573 105.75 0.742558C106 0.947105 106.124 1.24275 106.124 1.62091C106.124 1.99906 106 2.28268 105.75 2.48894C105.5 2.69693 105.136 2.80006 104.655 2.80006Z" fill="#2B2B2B"/>
          <path d="M111.853 4.29034V4.72349H108.455V0H111.751V0.43144H108.964V2.11079H111.449V2.53707H108.964V4.29034H111.853Z" fill="#2B2B2B"/>
          <path d="M117.684 0V4.72349H117.266L114.175 0.892101V4.72349H113.666V0H114.084L117.182 3.82967V0H117.684Z" fill="#2B2B2B"/>
          <path d="M120.802 4.72349V0.43144H119.112V0H123V0.43144H121.31V4.72178H120.801L120.802 4.72349Z" fill="#2B2B2B"/>
          <path d="M114.433 21.7454H112.169V9.82666H114.257L120.736 17.8281V9.82666H123V21.7454H120.911L114.433 13.7595V21.7454Z" fill="#2B2B2B"/>
          <path d="M103.696 9.57227C105.546 9.57227 107.073 10.1687 108.279 11.3599C109.507 12.5287 110.123 14.0053 110.123 15.7878C110.123 17.5702 109.509 19.0399 108.279 20.2311C107.063 21.4119 105.535 22.0015 103.696 22.0015C101.857 22.0015 100.354 21.4119 99.1157 20.2311C97.8979 19.0278 97.2891 17.5462 97.2891 15.7878C97.2891 14.0293 97.8979 12.5408 99.1157 11.3599C100.344 10.1687 101.871 9.57227 103.696 9.57227ZM100.871 12.9258C100.122 13.6856 99.7473 14.6395 99.7473 15.786C99.7473 16.9325 100.122 17.8848 100.871 18.6463C101.619 19.4077 102.562 19.7876 103.696 19.7876C104.83 19.7876 105.773 19.4077 106.522 18.6463C107.283 17.8745 107.663 16.9205 107.663 15.786C107.663 14.6516 107.281 13.6976 106.522 12.9258C105.761 12.154 104.82 11.7673 103.696 11.7673C102.573 11.7673 101.63 12.154 100.871 12.9258Z" fill="#2B2B2B"/>
          <path d="M89.1581 21.7454H84.1733V9.82666H89.1581C90.983 9.82666 92.5051 10.4008 93.7229 11.5473C94.9511 12.6938 95.567 14.1067 95.567 15.786C95.567 17.4654 94.9529 18.8783 93.7229 20.0248C92.5051 21.1713 90.9847 21.7454 89.1581 21.7454ZM89.1581 11.9374H86.5249V19.6329H89.1581C90.2813 19.6329 91.2244 19.2633 91.9837 18.5259C92.7448 17.7662 93.1245 16.8517 93.1245 15.786C93.1245 14.7203 92.7431 13.7818 91.9837 13.0444C91.2227 12.307 90.2813 11.9374 89.1581 11.9374Z" fill="#2B2B2B"/>
          <path d="M75.5792 16.8758L70.9111 9.82666H73.6301L76.7374 14.7306L79.9498 9.82666H82.6547L77.9325 16.807V21.7454H75.5792V16.8758Z" fill="#2B2B2B"/>
          <path d="M61.7536 21.7454H59.2778L64.5443 9.82666H66.4584L71.7248 21.7454H69.2508L68.285 19.4988H62.7194L61.7536 21.7454ZM67.3717 17.4035L65.5101 13.0788L63.6309 17.4035H67.3699H67.3717Z" fill="#2B2B2B"/>
          <path d="M46.6086 21.7454H44.2554V9.82666H46.6086L51.0142 17.3519L55.4041 9.82666H57.7556V21.7454H55.4041V14.1514L51.7876 20.3995H50.2252L46.6086 14.1686V21.7454Z" fill="#2B2B2B"/>
        </g>
        <defs>
          <clipPath id="clip0_sidebar">
            <rect width="123" height="22" fill="white"/>
          </clipPath>
        </defs>
      </svg>
    </div>

    <!-- User Profile with Account Switcher -->
    <div class="sidebar-profile" @click="showAccountDropdown = !showAccountDropdown">
      <div
        class="profile-avatar"
        :style="currentAccountPhoto ? { backgroundImage: `url(${currentAccountPhoto})` } : {}"
      >
        <span v-if="!currentAccountPhoto">{{ currentAccount ? getAccountDisplayName(currentAccount).charAt(0) : '👤' }}</span>
      </div>
      <div class="profile-info">
        <div class="profile-name">
          {{ currentAccount ? getAccountDisplayName(currentAccount) : getFirstName(userInfo?.firstName) }}
        </div>
        <div class="profile-role" v-if="currentAccount">
          {{ getAccountTypeLabel(currentAccount) }}
        </div>
      </div>
      <div class="profile-toggle" v-if="accounts.length > 1">
        <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg" :class="{ rotated: showAccountDropdown }">
          <path d="M18 9L12 15L6 9" stroke="#19191C" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </div>
    </div>

    <!-- Account Dropdown -->
    <div v-if="showAccountDropdown && (accounts.length > 1 || !hasClientAccount)" class="account-dropdown">
      <div
        v-for="account in accounts"
        :key="account.key"
        class="account-item"
        :class="{ current: account.isCurrent, switching: switchingAccount }"
        @click.stop="switchAccount(account)"
      >
        <div
          class="account-avatar"
          :style="getAccountPhoto(account) ? { backgroundImage: `url(${getAccountPhoto(account)})`, backgroundSize: 'cover' } : {}"
        >
          <span v-if="!getAccountPhoto(account)">{{ getAccountDisplayName(account).charAt(0) || '?' }}</span>
        </div>
        <div class="account-info">
          <div class="account-name">{{ getAccountDisplayName(account) }}</div>
          <div class="account-type">{{ getAccountTypeLabel(account) }}</div>
        </div>
        <div v-if="account.isCurrent" class="account-check">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
            <path d="M20 6L9 17l-5-5" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
        </div>
      </div>
      <!-- Create Client Account button -->
      <div v-if="!hasClientAccount" class="create-account-section">
        <button
          class="create-account-btn"
          :disabled="creatingAccount"
          @click.stop="createClientAccount"
        >
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M12 5v14M5 12h14" stroke-linecap="round"/>
          </svg>
          {{ creatingAccount ? 'Creating...' : 'Create Client Account' }}
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
        <!-- Dashboard Icon (Главная) -->
        <svg v-if="item.iconId === 'dashboard'" class="nav-icon" width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M0.750861 4.14962C0.750938 2.84032 0.750976 2.18566 1.0457 1.70476C1.21062 1.43566 1.43688 1.20942 1.70598 1.04452C2.18691 0.749818 2.84156 0.749818 4.15086 0.749818H4.35C5.65937 0.749818 6.31406 0.749818 6.795 1.04454C7.06411 1.20945 7.29037 1.43571 7.45528 1.70482C7.75 2.18576 7.75 2.84045 7.75 4.14982V15.3503C7.75 16.6597 7.75 17.3143 7.45528 17.7953C7.29037 18.0644 7.06411 18.2906 6.795 18.4556C6.31406 18.7503 5.65937 18.7503 4.35 18.7503H4.1502C2.84076 18.7503 2.18604 18.7503 1.70509 18.4555C1.43597 18.2906 1.2097 18.0643 1.0448 17.7952C0.750085 17.3142 0.750123 16.6595 0.7502 15.3501L0.750861 4.14962Z" stroke="currentColor" stroke-width="1.5"/>
          <path d="M11.75 4.15001C11.75 2.84047 11.75 2.18569 12.0448 1.70472C12.2097 1.43559 12.436 1.20933 12.7052 1.04443C13.1862 0.749723 13.841 0.749819 15.1505 0.750012L15.3505 0.750041C16.6597 0.750234 17.3143 0.75033 17.7952 1.04507C18.0642 1.20999 18.2905 1.43624 18.4553 1.70534C18.75 2.18624 18.75 2.84084 18.75 4.15004V9.35027C18.75 10.6596 18.75 11.3143 18.4553 11.7953C18.2904 12.0644 18.0641 12.2906 17.795 12.4556C17.3141 12.7503 16.6594 12.7503 15.35 12.7503H15.15C13.8406 12.7503 13.1859 12.7503 12.705 12.4556C12.4359 12.2906 12.2096 12.0644 12.0447 11.7953C11.75 11.3143 11.75 10.6596 11.75 9.35027V4.15001Z" stroke="currentColor" stroke-width="1.5"/>
        </svg>

        <!-- Orders Icon (Заявки) -->
        <svg v-else-if="item.iconId === 'orders'" class="nav-icon" width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M16.75 15.75H2.75M4.75 18.75H14.75M9.75 3.75V6.75M9.75 9.75V6.75M9.75 6.75H12.75M9.75 6.75H6.75M3.95 0.75H15.55C16.6701 0.75 17.2302 0.75 17.658 0.967987C18.0343 1.15973 18.3403 1.46569 18.532 1.84202C18.75 2.26984 18.75 2.82989 18.75 3.95V9.55C18.75 10.6701 18.75 11.2302 18.532 11.658C18.3403 12.0343 18.0343 12.3403 17.658 12.532C17.2302 12.75 16.6701 12.75 15.55 12.75H3.95C2.8299 12.75 2.26984 12.75 1.84202 12.532C1.46569 12.3403 1.15973 12.0343 0.967987 11.658C0.75 11.2302 0.75 10.6701 0.75 9.55V3.95C0.75 2.8299 0.75 2.26984 0.967987 1.84202C1.15973 1.46569 1.46569 1.15973 1.84202 0.967987C2.26984 0.75 2.82989 0.75 3.95 0.75Z" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"/>
        </svg>

        <!-- Real Estate Icon (Недвижимость) -->
        <svg v-else-if="item.iconId === 'realestate'" class="nav-icon" width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M21 10.9782V19C21 20.1046 20.1068 21 19.0023 21H4.99869C3.89412 21 3 20.1046 3 19V10.9782C3 10.361 3.28494 9.77838 3.77212 9.39946L10.7721 3.95502C11.4943 3.39329 12.5057 3.39329 13.2279 3.95502L20.2279 9.39946C20.7151 9.77838 21 10.361 21 10.9782Z" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>

        <!-- Tenants Icon (Арендаторы) -->
        <svg v-else-if="item.iconId === 'tenants'" class="nav-icon" width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path fill-rule="evenodd" clip-rule="evenodd" d="M12 3C14.2782 3 16.125 4.84683 16.125 7.125C16.125 9.40317 14.2782 11.25 12 11.25C9.72183 11.25 7.875 9.40317 7.875 7.125C7.875 4.84683 9.72183 3 12 3Z" stroke="currentColor" stroke-width="1.5"/>
          <path fill-rule="evenodd" clip-rule="evenodd" d="M12 12C15.7279 12 18.75 14.0147 18.75 16.5C18.75 18.9853 15.7279 21 12 21C8.27208 21 5.25 18.9853 5.25 16.5C5.25 14.0147 8.27208 12 12 12Z" stroke="currentColor" stroke-width="1.5"/>
        </svg>

        <!-- Monitoring Icon (Мониторинг) -->
        <svg v-else-if="item.iconId === 'monitoring'" class="nav-icon" width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M19 18H5M7 21H17M6.2 3H17.8C18.9201 3 19.4802 3 19.908 3.21799C20.2843 3.40973 20.5903 3.71569 20.782 4.09202C21 4.51984 21 5.07989 21 6.2V11.8C21 12.9201 21 13.4802 20.782 13.908C20.5903 14.2843 20.2843 14.5903 19.908 14.782C19.4802 15 18.9201 15 17.8 15H6.2C5.0799 15 4.51984 15 4.09202 14.782C3.71569 14.5903 3.40973 14.2843 3.21799 13.908C3 13.4802 3 12.9201 3 11.8V6.2C3 5.0799 3 4.51984 3.21799 4.09202C3.40973 3.71569 3.71569 3.40973 4.09202 3.21799C4.51984 3 5.07989 3 6.2 3Z" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"/>
        </svg>

        <!-- Create Listing Icon (Plus in square) -->
        <svg v-else-if="item.iconId === 'create-listing'" class="nav-icon" width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <rect x="3" y="3" width="18" height="18" rx="3" stroke="currentColor" stroke-width="1.5"/>
          <path d="M12 8V16M8 12H16" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"/>
        </svg>

        <!-- Invitations Icon (Envelope) -->
        <svg v-else-if="item.iconId === 'invitations'" class="nav-icon" width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M3 8L10.89 13.26C11.2187 13.4793 11.6049 13.5963 12 13.5963C12.3951 13.5963 12.7813 13.4793 13.11 13.26L21 8M5 19H19C19.5304 19 20.0391 18.7893 20.4142 18.4142C20.7893 18.0391 21 17.5304 21 17V7C21 6.46957 20.7893 5.96086 20.4142 5.58579C20.0391 5.21071 19.5304 5 19 5H5C4.46957 5 3.96086 5.21071 3.58579 5.58579C3.21071 5.96086 3 6.46957 3 7V17C3 17.5304 3.21071 18.0391 3.58579 18.4142C3.96086 18.7893 4.46957 19 5 19Z" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>

        <!-- Communal Icon (Коммунальные) -->
        <svg v-else-if="item.iconId === 'communal'" class="nav-icon" width="20" height="20" viewBox="0 0 26 26" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M24.825 16.8235L24.6001 3.6001L23.7617 1.62098L21.6001 0.560397L8.55898 0.560396L5.37692 11.5205C5.37692 11.5205 9.3401 12.3922 11.0974 14.1003C12.9234 15.8752 13.8622 20.0058 13.8622 20.0058L24.8224 16.8238L24.825 16.8235Z" fill="url(#paint0_linear_communal)"/>
          <path d="M13.1958 20.6723L13.6384 21.0775C13.6963 21.0142 13.74 20.9392 13.7665 20.8576L13.1958 20.6723ZM4.71056 12.187L4.52523 11.6164L4.37743 11.6644L4.27151 11.7781L4.71056 12.187ZM0.161048 16.1912C-0.0648056 16.4336 -0.0513272 16.8133 0.191153 17.0391C0.433633 17.265 0.813293 17.2515 1.03915 17.009L0.600098 16.6001L0.161048 16.1912ZM9.15754 24.195C8.93378 24.4394 8.95053 24.8189 9.19495 25.0427C9.43937 25.2664 9.8189 25.2497 10.0427 25.0052L9.6001 24.6001L9.15754 24.195ZM2.78414 24.1641L3.05653 23.6295L2.78414 24.1641ZM1.03607 22.4161L1.57068 22.1437L1.03607 22.4161ZM24.1641 22.4161L23.6295 22.1437L24.1641 22.4161ZM22.4161 24.1641L22.1437 23.6295L22.4161 24.1641ZM22.4161 1.03607L22.1437 1.57068L22.4161 1.03607ZM24.1641 2.78414L23.6295 3.05653L24.1641 2.78414ZM2.78414 1.03607L3.05653 1.57068L2.78414 1.03607ZM1.03607 2.78414L1.57068 3.05653L1.03607 2.78414ZM13.1958 20.6723C13.7665 20.8576 13.7666 20.8573 13.7667 20.8569C13.7668 20.8568 13.7669 20.8564 13.767 20.8561C13.7672 20.8556 13.7674 20.855 13.7676 20.8544C13.768 20.8531 13.7684 20.8516 13.769 20.8499C13.77 20.8465 13.7713 20.8424 13.7728 20.8374C13.7758 20.8276 13.7796 20.8147 13.7841 20.7988C13.793 20.7671 13.8046 20.7234 13.8175 20.6686C13.8433 20.559 13.8745 20.4044 13.9003 20.2111C13.9518 19.8248 13.9822 19.2812 13.9039 18.632C13.7461 17.324 13.1502 15.6164 11.4583 13.9245L11.0341 14.3488L10.6098 14.7731C12.1 16.2632 12.5849 17.7174 12.7125 18.7757C12.7769 19.3095 12.7511 19.7502 12.7108 20.0524C12.6907 20.2033 12.667 20.319 12.6494 20.3937C12.6406 20.4309 12.6334 20.4579 12.629 20.4737C12.6267 20.4816 12.6252 20.4867 12.6245 20.489C12.6242 20.4901 12.624 20.4905 12.6242 20.4902C12.6242 20.49 12.6243 20.4897 12.6245 20.4892C12.6246 20.4889 12.6247 20.4886 12.6248 20.4882C12.6248 20.488 12.6249 20.4877 12.625 20.4876C12.6251 20.4873 12.6252 20.487 13.1958 20.6723ZM11.0341 14.3488L11.4583 13.9245C9.7665 12.2327 8.05891 11.6368 6.75091 11.479C6.10163 11.4007 5.55812 11.4311 5.17179 11.4826C4.97849 11.5084 4.8239 11.5396 4.71429 11.5654C4.65946 11.5783 4.61578 11.5899 4.58404 11.5988C4.56817 11.6033 4.55527 11.6071 4.54544 11.6101C4.54052 11.6116 4.53637 11.6128 4.533 11.6139C4.53132 11.6144 4.52982 11.6149 4.52853 11.6153C4.52788 11.6155 4.52728 11.6157 4.52673 11.6159C4.52646 11.616 4.52608 11.6161 4.52595 11.6162C4.52558 11.6163 4.52523 11.6164 4.71056 12.187C4.89588 12.7577 4.89555 12.7578 4.89524 12.7579C4.89515 12.7579 4.89484 12.758 4.89466 12.7581C4.89431 12.7582 4.89399 12.7583 4.89373 12.7584C4.8932 12.7586 4.89285 12.7587 4.89269 12.7587C4.89236 12.7588 4.89277 12.7587 4.89389 12.7584C4.89614 12.7577 4.90127 12.7561 4.90919 12.7539C4.92502 12.7495 4.95194 12.7422 4.98922 12.7335C5.06385 12.7159 5.17955 12.6922 5.33048 12.6721C5.63263 12.6318 6.07338 12.606 6.60722 12.6704C7.66549 12.798 9.11966 13.2829 10.6098 14.7731L11.0341 14.3488ZM4.71056 12.187L4.27151 11.7781L0.161048 16.1912L0.600098 16.6001L1.03915 17.009L5.1496 12.596L4.71056 12.187ZM9.6001 24.6001L10.0427 25.0052L13.6384 21.0775L13.1958 20.6723L12.7533 20.2672L9.15754 24.195L9.6001 24.6001ZM7.0001 0.600098V1.2001H18.2001V0.600098V9.76324e-05H7.0001V0.600098ZM24.6001 7.0001H24.0001V18.2001H24.6001H25.2001V7.0001H24.6001ZM18.2001 24.6001V24.0001H7.0001V24.6001V25.2001H18.2001V24.6001ZM0.600098 18.2001H1.2001V7.0001H0.600098H9.76324e-05V18.2001H0.600098ZM7.0001 24.6001V24.0001C5.87009 24.0001 5.05867 23.9996 4.42176 23.9476C3.79186 23.8961 3.38567 23.7972 3.05653 23.6295L2.78414 24.1641L2.51174 24.6987C3.03825 24.967 3.61993 25.0861 4.32405 25.1436C5.02115 25.2006 5.88989 25.2001 7.0001 25.2001V24.6001ZM0.600098 18.2001H9.76324e-05C9.76324e-05 19.3103 -0.000369012 20.179 0.056587 20.8761C0.114115 21.5803 0.2332 22.1619 0.501468 22.6885L1.03607 22.4161L1.57068 22.1437C1.40297 21.8145 1.30407 21.4083 1.2526 20.7784C1.20056 20.1415 1.2001 19.3301 1.2001 18.2001H0.600098ZM2.78414 24.1641L3.05653 23.6295C2.41678 23.3036 1.89664 22.7834 1.57068 22.1437L1.03607 22.4161L0.501468 22.6885C0.942485 23.554 1.6462 24.2577 2.51174 24.6987L2.78414 24.1641ZM24.6001 18.2001H24.0001C24.0001 19.3301 23.9996 20.1415 23.9476 20.7784C23.8961 21.4083 23.7972 21.8145 23.6295 22.1437L24.1641 22.4161L24.6987 22.6885C24.967 22.1619 25.0861 21.5803 25.1436 20.8761C25.2006 20.179 25.2001 19.3103 25.2001 18.2001H24.6001ZM18.2001 24.6001V25.2001C19.3103 25.2001 20.179 25.2006 20.8762 25.1436C21.5803 25.0861 22.1619 24.967 22.6885 24.6987L22.4161 24.1641L22.1437 23.6295C21.8145 23.7972 21.4083 23.8961 20.7784 23.9476C20.1415 23.9996 19.3301 24.0001 18.2001 24.0001V24.6001ZM24.1641 22.4161L23.6295 22.1437C23.3036 22.7834 22.7834 23.3036 22.1437 23.6295L22.4161 24.1641L22.6885 24.6987C23.554 24.2577 24.2577 23.554 24.6987 22.6885L24.1641 22.4161ZM18.2001 0.600098V1.2001C19.3301 1.2001 20.1415 1.20056 20.7784 1.2526C21.4083 1.30407 21.8145 1.40297 22.1437 1.57068L22.4161 1.03607L22.6885 0.501468C22.1619 0.2332 21.5803 0.114115 20.8761 0.056587C20.179 -0.000369012 19.3103 9.76324e-05 18.2001 9.76324e-05V0.600098ZM24.6001 7.0001H25.2001C25.2001 5.88989 25.2006 5.02116 25.1436 4.32405C25.0861 3.61993 24.967 3.03825 24.6987 2.51174L24.1641 2.78414L23.6295 3.05653C23.7972 3.38567 23.8961 3.79186 23.9476 4.42176C23.9996 5.05867 24.0001 5.87009 24.0001 7.0001H24.6001ZM22.4161 1.03607L22.1437 1.57068C22.7834 1.89664 23.3036 2.41678 23.6295 3.05653L24.1641 2.78414L24.6987 2.51174C24.2577 1.6462 23.554 0.942485 22.6885 0.501468L22.4161 1.03607ZM7.0001 0.600098V9.76324e-05C5.88989 9.76324e-05 5.02116 -0.000369012 4.32405 0.056587C3.61993 0.114115 3.03825 0.2332 2.51174 0.501468L2.78414 1.03607L3.05653 1.57068C3.38567 1.40297 3.79186 1.30407 4.42176 1.2526C5.05867 1.20056 5.87009 1.2001 7.0001 1.2001V0.600098ZM0.600098 7.0001H1.2001C1.2001 5.87009 1.20056 5.05867 1.2526 4.42176C1.30407 3.79186 1.40297 3.38567 1.57068 3.05653L1.03607 2.78414L0.501468 2.51174C0.2332 3.03825 0.114115 3.61993 0.056587 4.32405C-0.000369012 5.02115 9.76324e-05 5.88989 9.76324e-05 7.0001H0.600098ZM2.78414 1.03607L2.51174 0.501468C1.6462 0.942485 0.942485 1.6462 0.501468 2.51174L1.03607 2.78414L1.57068 3.05653C1.89664 2.41678 2.41678 1.89664 3.05653 1.57068L2.78414 1.03607Z" fill="currentColor"/>
          <defs>
            <linearGradient id="paint0_linear_communal" x1="4.78316" y1="8.59192" x2="17.8045" y2="-4.42945" gradientUnits="userSpaceOnUse">
              <stop stop-color="#FC933D"/>
              <stop offset="1" stop-color="#FC463D"/>
            </linearGradient>
          </defs>
        </svg>

        <!-- Services Icon (Услуги) -->
        <svg v-else-if="item.iconId === 'services'" class="nav-icon" width="20" height="20" viewBox="0 0 26 26" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M10.4927 17.3669C10.1228 16.6348 9.07738 16.6348 8.70753 17.3669L5.14341 24.422C5.60977 24.5384 6.09772 24.6001 6.6001 24.6001H14.1467L10.4927 17.3669Z" fill="url(#paint0_linear_services)"/>
          <path d="M12.6001 10.6001V0.600098H6.6001C3.28639 0.600098 0.600098 3.28639 0.600098 6.6001V12.6001H10.6001C11.7047 12.6001 12.6001 11.7047 12.6001 10.6001Z" fill="url(#paint1_linear_services)"/>
          <path d="M24.6001 18.6001V6.6001C24.6001 5.05381 24.0152 3.64413 23.0545 2.58028C19.2474 4.31015 16.6001 8.14596 16.6001 12.6001C16.6001 17.0542 19.2474 20.89 23.0545 22.6199C24.0152 21.5561 24.6001 20.1464 24.6001 18.6001Z" fill="url(#paint2_linear_services)"/>
          <path d="M6.6001 0.600098H18.6001C20.3675 0.600098 21.9564 1.36429 23.0545 2.58028M6.6001 0.600098C3.28639 0.600098 0.600098 3.28639 0.600098 6.6001M6.6001 0.600098H12.6001V10.6001C12.6001 11.7047 11.7047 12.6001 10.6001 12.6001H0.600098V6.6001M0.600098 6.6001V18.6001C0.600098 21.4114 2.53362 23.7712 5.14341 24.422M5.14341 24.422C5.60977 24.5384 6.09772 24.6001 6.6001 24.6001H14.1467M5.14341 24.422L8.70753 17.3669C9.07738 16.6348 10.1228 16.6348 10.4927 17.3669L14.1467 24.6001M14.1467 24.6001H18.6001C20.3675 24.6001 21.9564 23.8359 23.0545 22.6199M23.0545 22.6199C24.0152 21.5561 24.6001 20.1464 24.6001 18.6001V6.6001C24.6001 5.05381 24.0152 3.64413 23.0545 2.58028M23.0545 22.6199C19.2474 20.89 16.6001 17.0542 16.6001 12.6001C16.6001 8.14596 19.2474 4.31015 23.0545 2.58028" stroke="currentColor" stroke-width="1.2"/>
          <defs>
            <linearGradient id="paint0_linear_services" x1="24.6001" y1="-2.09295" x2="24.6001" y2="28.8179" gradientUnits="userSpaceOnUse">
              <stop stop-color="#FC463D"/>
              <stop offset="1" stop-color="#FC933D"/>
            </linearGradient>
            <linearGradient id="paint1_linear_services" x1="24.6001" y1="-2.09295" x2="24.6001" y2="28.8179" gradientUnits="userSpaceOnUse">
              <stop stop-color="#FC463D"/>
              <stop offset="1" stop-color="#FC933D"/>
            </linearGradient>
            <linearGradient id="paint2_linear_services" x1="24.6001" y1="-2.09295" x2="24.6001" y2="28.8179" gradientUnits="userSpaceOnUse">
              <stop stop-color="#FC463D"/>
              <stop offset="1" stop-color="#FC933D"/>
            </linearGradient>
          </defs>
        </svg>

        <!-- Templates Icon (Contract Document) -->
        <svg v-else-if="item.iconId === 'templates'" class="nav-icon" width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M14 2H6C4.89543 2 4 2.89543 4 4V20C4 21.1046 4.89543 22 6 22H18C19.1046 22 20 21.1046 20 20V8L14 2Z" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
          <path d="M14 2V8H20" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
          <path d="M8 13H16M8 17H13" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"/>
        </svg>

        <!-- Contracts Icon (Handshake/Pen) -->
        <svg v-else-if="item.iconId === 'contracts'" class="nav-icon" width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M17 3a2.83 2.83 0 0 1 4 4L7.5 20.5 2 22l1.5-5.5L17 3z" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>

        <!-- Settings Icon -->
        <svg v-else-if="item.iconId === 'settings'" class="nav-icon" width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <circle cx="12" cy="12" r="3" stroke="currentColor" stroke-width="1.5"/>
          <path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 0 1-2.83 2.83l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-4 0v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 0 1-2.83-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1 0-4h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 0 1 2.83-2.83l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 4 0v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 0 1 2.83 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 0 4h-.09a1.65 1.65 0 0 0-1.51 1z" stroke="currentColor" stroke-width="1.5"/>
        </svg>

        <span class="nav-label">{{ item.name }}</span>
      </router-link>
    </nav>

    <!-- Bottom Actions -->
    <div class="sidebar-footer">
      <button class="nav-item logout-btn" @click="handleLogout">
        <svg class="nav-icon" width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M7 19H3C2.45 19 2 18.55 2 18V2C2 1.45 2.45 1 3 1H7M14 15L18 10L14 5M18 10H7" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
        <span class="nav-label">Logout</span>
      </button>
    </div>
  </aside>
</template>

<style scoped>
.sidebar {
  width: 256px;
  min-width: 256px;
  max-width: 256px;
  height: 100vh;
  position: sticky;
  top: 0;
  left: 0;
  background: #ffffff;
  border-right: 1px solid rgba(25, 25, 28, 0.08);
  display: flex;
  flex-direction: column;
  padding: 24px 16px;
  overflow-y: auto;
  z-index: 100;
}

.sidebar-logo {
  display: flex;
  padding: 6px;
  flex-direction: column;
  align-items: flex-start;
  gap: 10px;
  align-self: stretch;
  margin-bottom: 16px;
}

.sidebar-logo svg {
  height: 22px;
  width: 123px;
}

.sidebar-profile {
  display: flex;
  width: 248px;
  height: 44px;
  padding: 6px;
  align-items: center;
  gap: 10px;
  margin-bottom: 24px;
  cursor: pointer;
  transition: all 0.15s ease;
  border-radius: 22px;
  user-select: none;
}

.sidebar-profile:hover {
  background: rgba(25, 25, 28, 0.04);
}

.sidebar-profile:active {
  background: rgba(25, 25, 28, 0.08);
  transform: scale(0.98);
}

.profile-avatar {
  width: 32px;
  height: 32px;
  border-radius: 32px;
  background: lightgray 50% / cover no-repeat;
  color: #19191C;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  font-weight: 600;
  flex-shrink: 0;
  background-size: cover;
  background-position: center;
}

.profile-info {
  min-width: 0;
}

.profile-name {
  font-weight: 500;
  font-size: 14px;
  color: #19191C;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.profile-role {
  font-size: 11px;
  color: #808286;
  white-space: nowrap;
}

.profile-toggle {
  display: flex;
  align-items: center;
  width: 24px;
  height: 24px;
  flex-shrink: 0;
}

.profile-toggle svg {
  transition: transform 0.2s;
}

.profile-toggle svg.rotated {
  transform: rotate(180deg);
}

.sidebar-nav {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.nav-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 16px;
  border-radius: 12px;
  color: #19191C;
  text-decoration: none;
  font-size: 14px;
  font-weight: 500;
  transition: all 0.15s ease;
  background: transparent;
  border: none;
  cursor: pointer;
  width: 100%;
  text-align: left;
  user-select: none;
}

.nav-item:hover {
  background: rgba(25, 25, 28, 0.04);
}

.nav-item:active {
  background: rgba(25, 25, 28, 0.08);
  transform: scale(0.98);
}

.nav-item.active {
  background: rgba(252, 77, 61, 0.08);
  color: #FC4D3D;
}

.nav-item.active:active {
  background: rgba(252, 77, 61, 0.12);
  transform: scale(0.98);
}

.nav-item.disabled {
  cursor: default;
  pointer-events: none;
}

.nav-icon {
  width: 20px;
  height: 20px;
  flex-shrink: 0;
  color: #19191C;
}

.nav-item.active .nav-icon {
  color: #FC4D3D;
}

.nav-label {
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.sidebar-footer {
  margin-top: auto;
  padding-top: 16px;
  border-top: 1px solid rgba(25, 25, 28, 0.08);
}

.logout-btn {
  color: #19191C;
}

.logout-btn:hover {
  background: rgba(239, 68, 68, 0.08);
  color: #ef4444;
}

.logout-btn:active {
  background: rgba(239, 68, 68, 0.12);
  transform: scale(0.98);
}

.logout-btn:hover .nav-icon {
  color: #ef4444;
}

.account-dropdown {
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.12);
  margin-top: -16px;
  margin-bottom: 16px;
  overflow: hidden;
}

.account-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 16px;
  cursor: pointer;
  transition: background 0.2s;
}

.account-item:hover {
  background: rgba(25, 25, 28, 0.04);
}

.account-item.current {
  background: rgba(252, 77, 61, 0.05);
}

.account-item.switching {
  opacity: 0.6;
  pointer-events: none;
}

.account-avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: linear-gradient(135deg, #FC463D, #FC933D);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  font-weight: 600;
  flex-shrink: 0;
}

.account-info {
  flex: 1;
  min-width: 0;
}

.account-name {
  font-weight: 500;
  font-size: 14px;
  color: #19191C;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.account-type {
  font-size: 12px;
  color: #808286;
}

.account-check {
  color: #FC4D3D;
  font-weight: bold;
}

.create-account-section {
  border-top: 1px solid rgba(25, 25, 28, 0.08);
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
  color: #FC4D3D;
  cursor: pointer;
  transition: background 0.2s;
}

.create-account-btn:hover:not(:disabled) {
  background: rgba(252, 77, 61, 0.08);
}

.create-account-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.create-account-btn svg {
  flex-shrink: 0;
}

/* Settings Panel */
.settings-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.4);
  z-index: 1000;
  display: flex;
}

.settings-panel {
  width: 380px;
  max-width: 90vw;
  height: 100%;
  background: white;
  box-shadow: 4px 0 24px rgba(0, 0, 0, 0.15);
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.settings-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 20px 24px;
  border-bottom: 1px solid rgba(0, 0, 0, 0.06);
}

.settings-header h2 {
  font-size: 20px;
  font-weight: 600;
  color: #19191C;
  margin: 0;
}

.close-btn {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  border: none;
  background: transparent;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #808286;
  transition: all 0.2s;
}

.close-btn:hover {
  background: rgba(0, 0, 0, 0.05);
  color: #19191C;
}

.settings-section {
  padding: 20px 24px;
  border-bottom: 1px solid rgba(0, 0, 0, 0.06);
}

.section-label {
  font-size: 12px;
  font-weight: 600;
  color: #808286;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin-bottom: 12px;
}

.current-account-card {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 16px;
  background: linear-gradient(135deg, rgba(252, 77, 61, 0.08), rgba(252, 147, 61, 0.08));
  border-radius: 16px;
}

.account-avatar.large {
  width: 56px;
  height: 56px;
  font-size: 20px;
}

.account-details {
  flex: 1;
  min-width: 0;
}

.account-details .account-name {
  font-size: 16px;
  font-weight: 600;
}

.account-details .account-type {
  font-size: 13px;
}

.accounts-list {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.accounts-list .account-item {
  border-radius: 12px;
  padding: 12px;
}

.switch-arrow {
  color: #808286;
  flex-shrink: 0;
}

.settings-options {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.settings-option {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 14px 16px;
  border-radius: 12px;
  cursor: pointer;
  transition: background 0.2s;
  color: #19191C;
}

.settings-option:hover {
  background: rgba(0, 0, 0, 0.04);
}

.settings-option.disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.settings-option.disabled:hover {
  background: transparent;
}

.settings-option span {
  flex: 1;
  font-size: 14px;
  font-weight: 500;
}

.coming-soon {
  font-size: 11px;
  color: #808286;
  background: rgba(0, 0, 0, 0.05);
  padding: 4px 8px;
  border-radius: 20px;
  font-weight: 500;
}

.settings-footer {
  margin-top: auto;
  padding: 20px 24px;
  border-top: 1px solid rgba(0, 0, 0, 0.06);
}

.settings-footer .logout-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  width: 100%;
  padding: 14px;
  border: none;
  border-radius: 12px;
  background: rgba(239, 68, 68, 0.08);
  color: #ef4444;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.settings-footer .logout-btn:hover {
  background: rgba(239, 68, 68, 0.15);
}

/* Settings Panel Transitions */
.settings-panel-enter-active,
.settings-panel-leave-active {
  transition: opacity 0.3s ease;
}

.settings-panel-enter-active .settings-panel,
.settings-panel-leave-active .settings-panel {
  transition: transform 0.3s ease;
}

.settings-panel-enter-from,
.settings-panel-leave-to {
  opacity: 0;
}

.settings-panel-enter-from .settings-panel,
.settings-panel-leave-to .settings-panel {
  transform: translateX(-100%);
}
</style>
