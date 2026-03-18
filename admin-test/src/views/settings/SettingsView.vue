<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { api } from '@/services/api'
import type { UserProfile, CompanyUser, CompanyUsersResponse } from '@/types/settings'

// Tabs
type Tab = 'profile' | 'team' | 'didox'
const activeTab = ref<Tab>('profile')

// Profile state
const profile = ref<UserProfile | null>(null)
const profileLoading = ref(true)
const profileSaving = ref(false)
const profilePhoto = ref<string | null>(null)

// Edit form
const editForm = ref({
  firstName: '',
  lastName: '',
  middleName: '',
  profilePicture: null as string | null
})

// Company users state
const companyUsers = ref<CompanyUser[]>([])
const companyUsersLoading = ref(false)


// Check if business account
const isBusinessAccount = computed(() => profile.value && !profile.value.isIndividual)

// Didox state
const didoxConnected = ref(false)
const didoxLogin = ref('')
const didoxFormLogin = ref('')
const didoxFormPassword = ref('')
const didoxLoading = ref(false)
const didoxActionLoading = ref(false)

// Load profile
async function loadProfile() {
  profileLoading.value = true
  try {
    profile.value = await api.getUserProfile()
    editForm.value = {
      firstName: profile.value.firstName || '',
      lastName: profile.value.lastName || '',
      middleName: profile.value.middleName || '',
      profilePicture: profile.value.photo
    }
    // Load profile photo - photo is now a full URL from API
    if (profile.value.photo) {
      profilePhoto.value = profile.value.photo
    }
  } catch (e) {
    console.error('Failed to load profile:', e)
  } finally {
    profileLoading.value = false
  }
}

// Save profile
async function saveProfile() {
  if (profileSaving.value) return
  profileSaving.value = true
  try {
    await api.updateUserProfile({
      firstName: editForm.value.firstName,
      lastName: editForm.value.lastName,
      middleName: editForm.value.middleName,
      profilePicture: editForm.value.profilePicture
    })
    await loadProfile()
    alert('Profile updated successfully!')
  } catch (e) {
    console.error('Failed to save profile:', e)
    alert('Failed to update profile')
  } finally {
    profileSaving.value = false
  }
}

// Upload new photo
async function handlePhotoUpload(event: Event) {
  const input = event.target as HTMLInputElement
  if (!input.files?.length) return

  const file = input.files[0]
  if (!file) return

  try {
    const key = await api.uploadFile(file)
    editForm.value.profilePicture = key
    // Preview the new photo
    profilePhoto.value = URL.createObjectURL(file)
  } catch (e) {
    console.error('Failed to upload photo:', e)
    alert('Failed to upload photo')
  }
}

// Load company users
async function loadCompanyUsers() {
  companyUsersLoading.value = true
  try {
    const response: CompanyUsersResponse = await api.getCompanyUsers(1, 50)
    companyUsers.value = response.items
    // Load user photos

  } catch (e) {
    console.error('Failed to load company users:', e)
  } finally {
    companyUsersLoading.value = false
  }
}

function getUserPhoto(userId: string): string | null {
  const user = companyUsers.value.find(u => u.userId === userId)
  return user?.photo || null
}

function formatDate(dateStr: string): string {
  const date = new Date(dateStr)
  return date.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' })
}

function formatPhone(phone: string): string {
  if (!phone) return ''
  // Format: +998 (XX) XXX-XX-XX
  if (phone.startsWith('998') && phone.length === 12) {
    return `+${phone.slice(0, 3)} (${phone.slice(3, 5)}) ${phone.slice(5, 8)}-${phone.slice(8, 10)}-${phone.slice(10)}`
  }
  return phone
}

// Toggle user active status
const togglingUser = ref<string | null>(null)

async function toggleUserStatus(user: CompanyUser) {
  if (togglingUser.value || user.isOwner) return

  togglingUser.value = user.userId
  try {
    if (user.isActive) {
      await api.deactivateCompanyUser(user.userId)
    } else {
      await api.activateCompanyUser(user.userId)
    }
    // Update local state
    user.isActive = !user.isActive
  } catch (e) {
    console.error('Failed to toggle user status:', e)
    alert('Failed to update user status')
  } finally {
    togglingUser.value = null
  }
}

// Didox functions
async function loadDidoxStatus() {
  didoxLoading.value = true
  try {
    const status = await api.getDidoxStatus()
    didoxConnected.value = status.isConnected
    didoxLogin.value = status.login || ''
  } catch (e) {
    console.error('Failed to load Didox status:', e)
  } finally {
    didoxLoading.value = false
  }
}

async function connectDidox() {
  if (!didoxFormLogin.value || !didoxFormPassword.value) return
  didoxActionLoading.value = true
  try {
    await api.connectDidox(didoxFormLogin.value, didoxFormPassword.value)
    didoxFormLogin.value = ''
    didoxFormPassword.value = ''
    await loadDidoxStatus()
  } catch (e: unknown) {
    alert((e as Error).message || 'Failed to connect to Didox')
  } finally {
    didoxActionLoading.value = false
  }
}

async function disconnectDidox() {
  if (!confirm('Disconnect from Didox?')) return
  didoxActionLoading.value = true
  try {
    await api.disconnectDidox()
    await loadDidoxStatus()
  } catch (e: unknown) {
    alert((e as Error).message || 'Failed to disconnect')
  } finally {
    didoxActionLoading.value = false
  }
}

// Switch tab
function switchTab(tab: Tab) {
  activeTab.value = tab
  if (tab === 'team' && companyUsers.value.length === 0) {
    loadCompanyUsers()
  }
  if (tab === 'didox') {
    loadDidoxStatus()
  }
}

onMounted(() => {
  loadProfile()
})
</script>

<template>
  <div class="settings-page">
    <div class="settings-header">
      <h1>Settings</h1>
    </div>

    <!-- Tabs -->
    <div class="settings-tabs">
      <button
        class="tab-btn"
        :class="{ active: activeTab === 'profile' }"
        @click="switchTab('profile')"
      >
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
          <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/>
          <circle cx="12" cy="7" r="4"/>
        </svg>
        Profile
      </button>
      <button
        v-if="isBusinessAccount"
        class="tab-btn"
        :class="{ active: activeTab === 'team' }"
        @click="switchTab('team')"
      >
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
          <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
          <circle cx="9" cy="7" r="4"/>
          <path d="M23 21v-2a4 4 0 0 0-3-3.87"/>
          <path d="M16 3.13a4 4 0 0 1 0 7.75"/>
        </svg>
        Team
      </button>
      <button
        class="tab-btn"
        :class="{ active: activeTab === 'didox' }"
        @click="switchTab('didox')"
      >
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
          <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"/>
        </svg>
        Didox
      </button>
    </div>

    <!-- Profile Tab -->
    <div v-if="activeTab === 'profile'" class="tab-content">
      <div v-if="profileLoading" class="loading-state">
        <div class="spinner"></div>
        <span>Loading profile...</span>
      </div>

      <div v-else class="profile-section">
        <div class="profile-card">
          <div class="profile-photo-section">
            <div
              class="profile-photo"
              :style="profilePhoto ? { backgroundImage: `url(${profilePhoto})` } : {}"
            >
              <span v-if="!profilePhoto">{{ (editForm.firstName || 'U').charAt(0) }}</span>
            </div>
            <label class="photo-upload-btn">
              <input type="file" accept="image/*" @change="handlePhotoUpload" hidden />
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/>
                <polyline points="17 8 12 3 7 8"/>
                <line x1="12" y1="3" x2="12" y2="15"/>
              </svg>
              Upload Photo
            </label>
          </div>

          <div class="profile-form">
            <div class="form-group">
              <label>First Name</label>
              <input v-model="editForm.firstName" type="text" placeholder="First name" />
            </div>
            <div class="form-group">
              <label>Last Name</label>
              <input v-model="editForm.lastName" type="text" placeholder="Last name" />
            </div>
            <div class="form-group">
              <label>Middle Name</label>
              <input v-model="editForm.middleName" type="text" placeholder="Middle name (optional)" />
            </div>
            <div class="form-group" v-if="profile?.phoneNumber">
              <label>Phone Number</label>
              <input :value="profile.phoneNumber" type="text" disabled />
            </div>
            <div class="form-group" v-if="profile?.companyName">
              <label>Company</label>
              <input :value="profile.companyName" type="text" disabled />
            </div>
          </div>

          <button class="save-btn" :disabled="profileSaving" @click="saveProfile">
            <span v-if="profileSaving">Saving...</span>
            <span v-else>Save Changes</span>
          </button>
        </div>
      </div>
    </div>

    <!-- Team Tab -->
    <div v-if="activeTab === 'team'" class="tab-content">
      <div v-if="companyUsersLoading" class="loading-state">
        <div class="spinner"></div>
        <span>Loading team members...</span>
      </div>

      <div v-else class="team-section">
        <div class="team-header">
          <h2>Team Members</h2>
          <span class="team-count">{{ companyUsers.length }} members</span>
        </div>

        <div class="team-list">
          <div v-for="user in companyUsers" :key="user.userId" class="team-member">
            <div
              class="member-avatar"
              :style="getUserPhoto(user.userId) ? { backgroundImage: `url(${getUserPhoto(user.userId)})` } : {}"
            >
              <span v-if="!getUserPhoto(user.userId)">{{ (user.fullName || user.phoneNumber || '?').charAt(0) }}</span>
            </div>
            <div class="member-info">
              <div class="member-name">
                {{ user.fullName || formatPhone(user.phoneNumber) || 'Unknown' }}
                <span v-if="user.isOwner" class="owner-badge">Owner</span>
              </div>
              <div class="member-details">
                <span class="role">{{ user.roleName }}</span>
                <span class="joined">Joined {{ formatDate(user.joinedAt) }}</span>
              </div>
            </div>
            <div class="member-actions" v-if="!user.isOwner">
              <button
                v-if="!user.isActive"
                class="btn btn-success"
                :disabled="togglingUser === user.userId"
                @click="toggleUserStatus(user)"
              >
                <span v-if="togglingUser === user.userId" class="btn-spinner"></span>
                <span v-else>Activate</span>
              </button>
              <button
                v-else
                class="btn btn-danger"
                :disabled="togglingUser === user.userId"
                @click="toggleUserStatus(user)"
              >
                <span v-if="togglingUser === user.userId" class="btn-spinner"></span>
                <span v-else>Deactivate</span>
              </button>
            </div>
            <div class="member-status owner-status" v-else>
              Owner
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Didox Tab -->
    <div v-if="activeTab === 'didox'" class="tab-content">
      <div v-if="didoxLoading" class="loading-state">
        <div class="spinner"></div>
        <span>Checking Didox status...</span>
      </div>

      <div v-else class="didox-section">
        <!-- Connected State -->
        <div v-if="didoxConnected" class="didox-card didox-connected">
          <div class="didox-status-header">
            <div class="didox-status-icon connected">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M20 6L9 17l-5-5" stroke-linecap="round" stroke-linejoin="round"/>
              </svg>
            </div>
            <div class="didox-status-info">
              <h3>Connected to Didox</h3>
              <p>Login: <strong>{{ didoxLogin }}</strong></p>
            </div>
          </div>
          <button class="disconnect-btn" @click="disconnectDidox" :disabled="didoxActionLoading">
            {{ didoxActionLoading ? 'Disconnecting...' : 'Disconnect' }}
          </button>
        </div>

        <!-- Disconnected State -->
        <div v-else class="didox-card">
          <div class="didox-status-header">
            <div class="didox-status-icon disconnected">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="12" cy="12" r="10"/>
                <path d="M15 9l-6 6M9 9l6 6" stroke-linecap="round"/>
              </svg>
            </div>
            <div class="didox-status-info">
              <h3>Not Connected</h3>
              <p>Enter your Didox credentials to enable e-signing</p>
            </div>
          </div>
          <div class="didox-form">
            <div class="form-group">
              <label>Login</label>
              <input v-model="didoxFormLogin" type="text" placeholder="Didox login" />
            </div>
            <div class="form-group">
              <label>Password</label>
              <input v-model="didoxFormPassword" type="password" placeholder="Didox password" />
            </div>
            <button class="save-btn" @click="connectDidox" :disabled="didoxActionLoading || !didoxFormLogin || !didoxFormPassword">
              {{ didoxActionLoading ? 'Connecting...' : 'Connect to Didox' }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.settings-page {
  padding: 32px;
  max-width: 800px;
}

.settings-header h1 {
  font-size: 28px;
  font-weight: 600;
  color: #19191C;
  margin: 0 0 24px 0;
}

/* Tabs */
.settings-tabs {
  display: flex;
  gap: 8px;
  margin-bottom: 24px;
  border-bottom: 1px solid #E5E5E5;
  padding-bottom: 0;
}

.tab-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 20px;
  border: none;
  background: none;
  font-size: 14px;
  font-weight: 500;
  color: #808286;
  cursor: pointer;
  border-bottom: 2px solid transparent;
  margin-bottom: -1px;
  transition: all 0.2s;
}

.tab-btn:hover {
  color: #19191C;
}

.tab-btn.active {
  color: #FF5B3C;
  border-bottom-color: #FF5B3C;
}

.tab-btn svg {
  flex-shrink: 0;
}

/* Loading */
.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 60px 20px;
  gap: 16px;
  color: #808286;
}

.spinner {
  width: 32px;
  height: 32px;
  border: 3px solid #E5E5E5;
  border-top-color: #FF5B3C;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

/* Profile Section */
.profile-card {
  background: white;
  border-radius: 16px;
  padding: 32px;
  box-shadow: 0 1px 3px rgba(0,0,0,0.08);
}

.profile-photo-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 32px;
}

.profile-photo {
  width: 100px;
  height: 100px;
  border-radius: 50%;
  background: linear-gradient(135deg, #FC463D, #FC933D);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 36px;
  font-weight: 600;
  background-size: cover;
  background-position: center;
  margin-bottom: 16px;
}

.photo-upload-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 16px;
  background: #F5F5F5;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 500;
  color: #19191C;
  cursor: pointer;
  transition: background 0.2s;
}

.photo-upload-btn:hover {
  background: #EBEBEB;
}

.profile-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
  margin-bottom: 24px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-size: 13px;
  font-weight: 500;
  color: #808286;
}

.form-group input {
  padding: 12px 16px;
  border: 1px solid #E5E5E5;
  border-radius: 10px;
  font-size: 15px;
  color: #19191C;
  transition: border-color 0.2s;
}

.form-group input:focus {
  outline: none;
  border-color: #FF5B3C;
}

.form-group input:disabled {
  background: #F9F9F9;
  color: #808286;
}

.save-btn {
  width: 100%;
  padding: 14px;
  background: #1B1B1B;
  color: white;
  border: none;
  border-radius: 100px;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s;
}

.save-btn:hover:not(:disabled) {
  background: #333;
}

.save-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* Team Section */
.team-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 20px;
}

.team-header h2 {
  font-size: 18px;
  font-weight: 600;
  color: #19191C;
  margin: 0;
}

.team-count {
  font-size: 14px;
  color: #808286;
}

.team-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.team-member {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 16px;
  background: white;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0,0,0,0.08);
}

.member-avatar {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  background: linear-gradient(135deg, #FC463D, #FC933D);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 18px;
  font-weight: 600;
  background-size: cover;
  background-position: center;
  flex-shrink: 0;
}

.member-info {
  flex: 1;
  min-width: 0;
}

.member-name {
  font-size: 15px;
  font-weight: 600;
  color: #19191C;
  display: flex;
  align-items: center;
  gap: 8px;
}

.owner-badge {
  font-size: 11px;
  font-weight: 600;
  color: #FF5B3C;
  background: rgba(255, 91, 60, 0.1);
  padding: 2px 8px;
  border-radius: 12px;
}

.member-details {
  display: flex;
  gap: 12px;
  font-size: 13px;
  color: #808286;
  margin-top: 4px;
}

.member-actions {
  display: flex;
  gap: 8px;
}

/* Buttons - Matching Listings/RealEstates style */
.btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  padding: 8px 16px;
  border-radius: 8px;
  font-weight: 600;
  font-size: 13px;
  cursor: pointer;
  border: none;
  transition: all 0.2s;
  min-width: 90px;
}

.btn-success {
  background: #1B1B1B;
  color: #fff;
}

.btn-success:hover:not(:disabled) {
  background: #333;
}

.btn-danger {
  background: #F7F7F7;
  color: #1B1B1B;
}

.btn-danger:hover:not(:disabled) {
  background: #EBEBEB;
}

.btn:disabled {
  cursor: not-allowed;
  opacity: 0.6;
}

.member-status.owner-status {
  font-size: 13px;
  font-weight: 600;
  padding: 8px 16px;
  border-radius: 8px;
  background: rgba(255, 91, 60, 0.1);
  color: #FF5B3C;
}

.btn-spinner {
  display: inline-block;
  width: 12px;
  height: 12px;
  border: 2px solid currentColor;
  border-top-color: transparent;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

/* Didox Section */
.didox-card {
  background: white;
  border-radius: 16px;
  padding: 32px;
  box-shadow: 0 1px 3px rgba(0,0,0,0.08);
}

.didox-status-header {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-bottom: 24px;
}

.didox-status-icon {
  width: 48px;
  height: 48px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.didox-status-icon.connected {
  background: rgba(22, 160, 94, 0.1);
  color: #16A05E;
}

.didox-status-icon.disconnected {
  background: rgba(239, 68, 68, 0.1);
  color: #EF4444;
}

.didox-status-info h3 {
  font-size: 16px;
  font-weight: 600;
  color: #19191C;
  margin: 0 0 4px 0;
}

.didox-status-info p {
  font-size: 14px;
  color: #808286;
  margin: 0;
}

.didox-status-info strong {
  color: #19191C;
  font-family: 'SF Mono', 'Fira Code', monospace;
}

.didox-form {
  display: flex;
  flex-direction: column;
  gap: 16px;
  max-width: 400px;
}

.disconnect-btn {
  padding: 10px 20px;
  background: #F7F7F7;
  border: 1px solid #E5E5E5;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 500;
  color: #EF4444;
  cursor: pointer;
  transition: all 0.2s;
}

.disconnect-btn:hover:not(:disabled) {
  background: rgba(239, 68, 68, 0.1);
  border-color: #EF4444;
}

.disconnect-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
</style>
