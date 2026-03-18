<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '@/services/api'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

// Steps: 1 = Enter Phone, 2 = Enter OTP, 3 = Set New Password
const currentStep = ref(1)
const isLoading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

// Form data
const phoneNumber = ref('')
const otpCode = ref('')
const sessionKey = ref('')
const password = ref('')
const confirmPassword = ref('')

const stepTitle = computed(() => {
  switch (currentStep.value) {
    case 1: return 'Forgot Password'
    case 2: return 'Verify Number'
    case 3: return 'Create New Password'
    default: return ''
  }
})

const stepDescription = computed(() => {
  switch (currentStep.value) {
    case 1: return 'Enter your phone number to reset password'
    case 2: return `Enter the 6-digit code sent to ${phoneNumber.value}`
    case 3: return 'Create a new secure password'
    default: return ''
  }
})

// Format phone number (remove spaces and special chars, keep only digits)
function formatPhoneNumber(phone: string): string {
  return phone.replace(/\D/g, '')
}

// Step 1: Send OTP to phone
async function sendOtp() {
  if (!phoneNumber.value) {
    errorMessage.value = 'Enter phone number'
    return
  }

  isLoading.value = true
  errorMessage.value = ''

  try {
    const formattedPhone = formatPhoneNumber(phoneNumber.value)
    // Use the existing OTP endpoint to send code
    await api.sendOtp(formattedPhone)
    currentStep.value = 2
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : 'Failed to send code'
  } finally {
    isLoading.value = false
  }
}

// Step 2: Verify OTP and get session key
async function verifyOtp() {
  if (otpCode.value.length !== 6) {
    errorMessage.value = 'Enter 6-digit code'
    return
  }

  isLoading.value = true
  errorMessage.value = ''

  try {
    const formattedPhone = formatPhoneNumber(phoneNumber.value)
    // Verify OTP using forgot-password endpoint (which returns session key)
    const key = await api.forgotPassword(formattedPhone, otpCode.value)
    sessionKey.value = key
    currentStep.value = 3
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : 'Invalid verification code'
  } finally {
    isLoading.value = false
  }
}

// Step 3: Set new password and auto-login
async function resetPassword() {
  if (!password.value) {
    errorMessage.value = 'Enter password'
    return
  }
  if (password.value.length < 6) {
    errorMessage.value = 'Password must be at least 6 characters'
    return
  }
  if (password.value !== confirmPassword.value) {
    errorMessage.value = 'Passwords do not match'
    return
  }

  isLoading.value = true
  errorMessage.value = ''

  try {
    // Reset password and get auth token
    const response = await api.forgotPasswordConfirm(sessionKey.value, password.value)
    // Auto-login with the returned token
    authStore.setAuthData(response)
    router.push('/realestates')
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : 'Password reset failed'
  } finally {
    isLoading.value = false
  }
}

function handleOtpInput(index: number, event: Event) {
  const input = event.target as HTMLInputElement
  const value = input.value

  const otpArray = otpCode.value.split('')
  otpArray[index] = value
  otpCode.value = otpArray.join('').slice(0, 6)

  if (value && index < 5) {
    const nextInput = document.querySelector(`#otp-${index + 1}`) as HTMLInputElement
    nextInput?.focus()
  }
}

function handleOtpKeydown(index: number, event: KeyboardEvent) {
  if (event.key === 'Backspace' && index > 0 && !otpCode.value[index]) {
    const prevInput = document.querySelector(`#otp-${index - 1}`) as HTMLInputElement
    prevInput?.focus()
  }
}

async function resendOtp() {
  isLoading.value = true
  errorMessage.value = ''
  try {
    const formattedPhone = formatPhoneNumber(phoneNumber.value)
    await api.sendOtp(formattedPhone)
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : 'Failed to send code'
  } finally {
    isLoading.value = false
  }
}

function goBack() {
  if (currentStep.value > 1) {
    currentStep.value--
    errorMessage.value = ''
  } else {
    router.push('/login')
  }
}
</script>

<template>
  <div class="auth-layout">
    <div class="auth-container fade-in">
      <div class="login-card">
        <div class="auth-logo">
          <svg xmlns="http://www.w3.org/2000/svg" width="185" height="33" viewBox="0 0 123 22" fill="none">
            <g clip-path="url(#clip0_forgot)">
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
              <clipPath id="clip0_forgot">
                <rect width="123" height="22" fill="white"/>
              </clipPath>
            </defs>
          </svg>
        </div>

        <div class="step-header">
          <h2 class="step-title">{{ stepTitle }}</h2>
          <p class="step-description">{{ stepDescription }}</p>
        </div>

        <div v-if="errorMessage" class="alert alert-error mb-lg">
          {{ errorMessage }}
        </div>

        <div v-if="successMessage" class="alert alert-success mb-lg">
          {{ successMessage }}
        </div>

        <!-- Step 1: Phone Number -->
        <div v-if="currentStep === 1">
          <form @submit.prevent="sendOtp">
            <div class="form-group">
              <input
                id="phone"
                v-model="phoneNumber"
                type="tel"
                class="form-input input-single"
                placeholder="Phone number"
                :disabled="isLoading"
              />
            </div>

            <button
              type="submit"
              class="btn btn-primary btn-lg btn-block"
              :disabled="isLoading"
            >
              <span v-if="isLoading" class="spinner"></span>
              <span v-else>Send Code</span>
            </button>
          </form>
        </div>

        <!-- Step 2: OTP Verification -->
        <div v-if="currentStep === 2">
          <form @submit.prevent="verifyOtp">
            <div class="form-group">
              <div class="otp-container">
                <input
                  v-for="i in 6"
                  :id="`otp-${i - 1}`"
                  :key="i"
                  type="text"
                  maxlength="1"
                  class="otp-input"
                  :value="otpCode[i - 1] || ''"
                  :disabled="isLoading"
                  @input="handleOtpInput(i - 1, $event)"
                  @keydown="handleOtpKeydown(i - 1, $event)"
                />
              </div>
              <p class="form-hint text-center mt-md">
                Didn't receive the code?
                <a href="#" class="text-brand" @click.prevent="resendOtp">Resend</a>
              </p>
            </div>

            <button
              type="submit"
              class="btn btn-primary btn-lg btn-block"
              :disabled="isLoading || otpCode.length !== 6"
            >
              <span v-if="isLoading" class="spinner"></span>
              <span v-else>Verify</span>
            </button>
          </form>
        </div>

        <!-- Step 3: Create New Password -->
        <div v-if="currentStep === 3">
          <form @submit.prevent="resetPassword">
            <div class="form-group">
              <input
                id="password"
                v-model="password"
                type="password"
                class="form-input input-phone"
                placeholder="New Password"
                :disabled="isLoading"
              />
            </div>

            <div class="form-group">
              <input
                id="confirmPassword"
                v-model="confirmPassword"
                type="password"
                class="form-input input-password"
                placeholder="Confirm New Password"
                :disabled="isLoading"
              />
            </div>

            <button
              type="submit"
              class="btn btn-primary btn-lg btn-block"
              :disabled="isLoading"
            >
              <span v-if="isLoading" class="spinner"></span>
              <span v-else>Reset Password</span>
            </button>
          </form>
        </div>

        <button
          class="btn btn-ghost btn-block mt-lg back-btn"
          @click="goBack"
          :disabled="isLoading"
        >
          ← Back
        </button>

        <div class="auth-footer">
          Remember your password?
          <router-link to="/login">Sign In</router-link>
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

/* Step header */
.step-header {
  text-align: center;
  margin-bottom: 32px;
}

.step-title {
  font-size: 1.5rem;
  font-weight: 600;
  color: #1B1B1B;
  margin-bottom: 8px;
}

.step-description {
  font-size: 0.875rem;
  color: rgba(27, 27, 27, 0.5);
}

/* Form inputs - Figma spacing */
.login-card .form-group {
  margin-bottom: 0;
}

/* Single input with full rounded corners */
.login-card .input-single {
  padding: 16px 24px 16px 20px;
  border-radius: 26px;
  background: rgba(27, 27, 27, 0.04);
  border: none;
  width: 100%;
  font-size: 1rem;
  color: #1B1B1B;
}

/* Stacked inputs - top */
.login-card .input-phone {
  padding: 16px 24px 16px 20px;
  border-radius: 26px 26px 0 0;
  border-bottom: 0.5px solid rgba(0, 0, 0, 0.12);
  background: rgba(27, 27, 27, 0.04);
  border-top: none;
  border-left: none;
  border-right: none;
  width: 100%;
  font-size: 1rem;
  color: #1B1B1B;
}

/* Stacked inputs - bottom */
.login-card .input-password {
  padding: 16px 22px 16px 20px;
  border-radius: 0 0 26px 26px;
  background: rgba(27, 27, 27, 0.04);
  border: none;
  width: 100%;
  font-size: 1rem;
  color: #1B1B1B;
}

.login-card .form-input::placeholder {
  color: rgba(27, 27, 27, 0.5);
}

/* Button styling */
.login-card .btn-primary {
  margin-top: 16px;
  padding: 16px 32px;
  border-radius: 100px;
  background: #1B1B1B;
  color: #FFFFFF;
  width: 100%;
  /* Mobile touch improvements */
  -webkit-tap-highlight-color: transparent;
  touch-action: manipulation;
}

.login-card .btn-primary:hover:not(:disabled) {
  background: #333333;
}

.login-card .btn-primary:active:not(:disabled) {
  background: #000000;
  transform: scale(0.98);
}

.back-btn {
  margin-top: 16px;
}

.login-card .auth-footer {
  text-align: center;
  margin-top: 24px;
  padding-top: 16px;
  color: rgba(27, 27, 27, 0.5);
  font-size: 0.875rem;
}

.form-hint {
  margin-top: 8px;
  font-size: 0.8125rem;
  color: rgba(27, 27, 27, 0.5);
}

.alert-success {
  background: #d4edda;
  color: #155724;
  padding: 12px 16px;
  border-radius: 8px;
}
</style>
