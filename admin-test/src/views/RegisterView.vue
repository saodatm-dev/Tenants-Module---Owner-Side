<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const isLoading = ref(false)
const errorMessage = ref('')

// Manual OneID code entry
const showOneIdCodeModal = ref(false)
const oneIdCode = ref('')

// OneID Configuration from environment variables
const ONEID_CONFIG = {
  authUrl: 'https://sso.egov.uz/sso/oauth/Authorization.do',
  clientId: import.meta.env.VITE_ONEID_CLIENT_ID || 'maydon_uz',
  scope: import.meta.env.VITE_ONEID_SCOPE || 'maydon_uz',
  redirectUri: import.meta.env.VITE_ONEID_REDIRECT_URI || `${window.location.origin}/callback`
}

// Generate OneID authorization URL
function getOneIdAuthUrl(): string {
  const state = crypto.randomUUID()
  localStorage.setItem('oauth_state', state)

  const params = new URLSearchParams({
    response_type: 'one_code',
    client_id: ONEID_CONFIG.clientId,
    redirect_uri: ONEID_CONFIG.redirectUri,
    scope: ONEID_CONFIG.scope,
    state: state,
  })

  return `${ONEID_CONFIG.authUrl}?${params.toString()}`
}

function goToPhoneRegister() {
  router.push('/register/phone')
}

function goToEImzoRegister() {
  router.push('/register/eimzo')
}

function goToOneIdRegister() {
  localStorage.setItem('auth_mode', 'register')
  window.location.href = getOneIdAuthUrl()
}

// Manual OneID code entry for development
async function submitOneIdCode() {
  if (!oneIdCode.value.trim()) {
    errorMessage.value = 'Enter OneID code'
    return
  }

  isLoading.value = true
  errorMessage.value = ''

  try {
    // Try registration first
    try {
      await authStore.registerWithOneId(oneIdCode.value.trim())
    } catch {
      // If registration fails (user exists), try login
      await authStore.loginWithOneId(oneIdCode.value.trim())
    }

    showOneIdCodeModal.value = false
    oneIdCode.value = ''
    router.push('/realestates')
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : 'OneID registration error'
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <div class="auth-layout">
    <div class="auth-container fade-in">
      <!-- Register Card Container with 26px border-radius matching Figma -->
      <div class="login-card">
        <div class="auth-logo">
          <svg xmlns="http://www.w3.org/2000/svg" width="185" height="33" viewBox="0 0 123 22" fill="none">
            <g clip-path="url(#clip0_register)">
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
              <clipPath id="clip0_register">
                <rect width="123" height="22" fill="white"/>
              </clipPath>
            </defs>
          </svg>
        </div>

        <div v-if="errorMessage" class="alert alert-error mb-lg">
          {{ errorMessage }}
        </div>

        <div class="method-cards">
          <div class="method-card" @click="goToPhoneRegister">
            <div class="method-card-icon">
              <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <rect x="5" y="2" width="14" height="20" rx="2" ry="2"/>
                <path d="M12 18h.01"/>
              </svg>
            </div>
            <div class="method-card-content">
              <div class="method-card-title">Phone Number</div>
              <div class="method-card-desc">Registration via SMS</div>
            </div>
          </div>

          <div class="method-card" @click="goToEImzoRegister">
            <div class="method-card-icon">
             <svg width="800px" height="800px" viewBox="0 0 16 16" xmlns="http://www.w3.org/2000/svg">
    <path d="m 8.074219 0 c -1.203125 -0.0117188 -2.40625 0.285156 -3.492188 0.890625 c -0.480469 0.269531 -0.652343 0.878906 -0.382812 1.359375 c 0.269531 0.484375 0.878906 0.65625 1.359375 0.386719 c 1.550781 -0.867188 3.4375 -0.847657 4.972656 0.050781 c 1.53125 0.898438 2.46875 2.535156 2.46875 4.3125 v 1 c 0 0.550781 0.449219 1 1 1 s 1 -0.449219 1 -1 v -1 c 0 -0.019531 0 -0.039062 -0.003906 -0.054688 c -0.019532 -2.460937 -1.332032 -4.738281 -3.457032 -5.984374 c -1.070312 -0.628907 -2.265624 -0.9492192 -3.46875 -0.960938 z m -5.199219 2.832031 c -0.066406 0 -0.132812 0.007813 -0.195312 0.023438 c -0.257813 0.058593 -0.484376 0.21875 -0.625 0.445312 c -0.6875 1.109375 -1.054688 2.390625 -1.054688 3.699219 v 5.0625 c 0 0.550781 0.449219 1 1 1 s 1 -0.449219 1 -1 v -5.0625 c 0 -0.933594 0.261719 -1.851562 0.753906 -2.644531 c 0.292969 -0.46875 0.148438 -1.082031 -0.320312 -1.375 c -0.167969 -0.105469 -0.363282 -0.15625 -0.558594 -0.148438 z m 5.125 0.167969 c -2.199219 0 -4 1.800781 -4 4 v 1 c 0 0.550781 0.449219 1 1 1 s 1 -0.449219 1 -1 v -1 c 0 -1.117188 0.882812 -2 2 -2 s 2 0.882812 2 2 v 5 s 0.007812 0.441406 0.175781 0.941406 s 0.5 1.148438 1.117188 1.765625 c 0.390625 0.390625 1.023437 0.390625 1.414062 0 s 0.390625 -1.023437 0 -1.414062 c -0.382812 -0.382813 -0.550781 -0.734375 -0.632812 -0.984375 s -0.074219 -0.308594 -0.074219 -0.308594 v -5 c 0 -2.199219 -1.800781 -4 -4 -4 z m 0 3 c -0.550781 0 -1 0.449219 -1 1 v 5 s 0 0.59375 0.144531 1.320312 c 0.144531 0.726563 0.414063 1.652344 1.148438 2.386719 c 0.390625 0.390625 1.023437 0.390625 1.414062 0 s 0.390625 -1.023437 0 -1.414062 c -0.265625 -0.265625 -0.496093 -0.839844 -0.601562 -1.363281 c -0.105469 -0.523438 -0.105469 -0.929688 -0.105469 -0.929688 v -5 c 0 -0.550781 -0.449219 -1 -1 -1 z m -3 4 c -0.550781 0 -1 0.449219 -1 1 v 3 c 0 0.550781 0.449219 1 1 1 s 1 -0.449219 1 -1 v -3 c 0 -0.550781 -0.449219 -1 -1 -1 z m 9 0 c -0.550781 0 -1 0.449219 -1 1 s 0.449219 1 1 1 s 1 -0.449219 1 -1 s -0.449219 -1 -1 -1 z m 0 0" fill="#2e3434"/>
</svg>
            </div>
            <div class="method-card-content">
              <div class="method-card-title">E-IMZO</div>
              <div class="method-card-desc">Registration with digital signature (Professional)</div>
            </div>
          </div>

          <div class="method-card" @click="goToOneIdRegister">
            <div class="method-card-icon">
                            <svg width="800px" height="800px" viewBox="0 0 192 192" xmlns="http://www.w3.org/2000/svg" fill="none"><path stroke="#000000" stroke-linecap="round" stroke-linejoin="round" stroke-width="12" d="M96 62H68c-17.673 0-32.389 14.46-29.302 31.862C47.497 143.453 75.94 170 96 170m0-108h28c17.673 0 32.389 14.46 29.302 31.862C144.503 143.453 116.06 170 96 170"/><path fill="#000000" fill-rule="evenodd" d="M68 56c0-15.464 12.536-28 28-28s28 12.536 28 28c0 2.06-.222 4.067-.644 6h12.197c.294-1.957.447-3.96.447-6 0-22.091-17.909-40-40-40S56 33.909 56 56c0 2.04.153 4.043.447 6h12.197A28.105 28.105 0 0 1 68 56Zm-.367 98.946C70.417 141.836 82.06 132 96 132c12.903 0 23.838 8.427 27.601 20.077l8.982-9.608C125.816 129.136 111.975 120 96 120c-15.922 0-29.725 9.076-36.516 22.338L65.5 152l2.133 2.946Z" clip-rule="evenodd"/><circle cx="96" cy="94" r="14" stroke="#000000" stroke-width="12"/></svg>

            </div>
            <div class="method-card-content">
              <div class="method-card-title">OneID</div>
              <div class="method-card-desc">Registration via government portal</div>
            </div>
            <!-- Manual code entry button -->
            <button
              class="btn-code-entry"
              @click.stop="showOneIdCodeModal = true"
              title="Enter code manually"
            >
              📝
            </button>
          </div>
        </div>

        <div class="auth-footer">
          Already have an account?
          <router-link to="/login">Sign In</router-link>
        </div>
      </div>
    </div>

    <!-- OneID Code Entry Modal -->
    <div v-if="showOneIdCodeModal" class="modal-overlay" @click.self="showOneIdCodeModal = false">
      <div class="modal-content card">
        <h3 style="margin: 0 0 1rem;">Enter OneID Code</h3>
        <p class="text-muted" style="margin-bottom: 1rem; font-size: 0.875rem;">
          Paste the code from callback URL (code parameter)
        </p>

        <div v-if="errorMessage" class="alert alert-error mb-lg">
          {{ errorMessage }}
        </div>

        <div class="form-group">
          <input
            v-model="oneIdCode"
            type="text"
            class="form-input"
            placeholder="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
            :disabled="isLoading"
          />
        </div>

        <div style="display: flex; gap: 0.5rem;">
          <button
            class="btn btn-ghost"
            @click="showOneIdCodeModal = false"
            :disabled="isLoading"
          >
            Cancel
          </button>
          <button
            class="btn btn-primary"
            @click="submitOneIdCode"
            :disabled="isLoading || !oneIdCode.trim()"
          >
            <span v-if="isLoading" class="spinner"></span>
            <span v-else>Register</span>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Full-screen auth layout - no container constraints */
.auth-layout {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #FFFFFF;
  padding: 0;
}

.auth-container {
  width: 100%;
  max-width: 100%;
  padding: 0;
}

/* Login Card - Full width, centered content */
.login-card {
  background: #FFFFFF;
  width: 100%;
  max-width: 400px;
  margin: 0 auto;
  padding: 40px 32px;
}

.login-card .auth-logo {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  margin-bottom: 70px;
}

.login-card .auth-footer {
  text-align: center;
  margin-top: 24px;
  padding-top: 16px;
  color: var(--text-muted, #6b7280);
  font-size: 0.875rem;
}

.btn-code-entry {
  position: absolute;
  right: 0.5rem;
  top: 50%;
  transform: translateY(-50%);
  background: none;
  border: none;
  font-size: 1rem;
  cursor: pointer;
  padding: 0.25rem;
  border-radius: 4px;
  opacity: 0.6;
  transition: opacity 0.2s;
}

.btn-code-entry:hover {
  opacity: 1;
}

.method-card {
  position: relative;
}

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  max-width: 400px;
  width: 90%;
  animation: modalSlideIn 0.2s ease;
}

@keyframes modalSlideIn {
  from {
    opacity: 0;
    transform: translateY(-20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
