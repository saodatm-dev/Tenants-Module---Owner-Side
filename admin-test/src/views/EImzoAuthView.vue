<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { api } from '@/services/api'
import { EImzoService } from '@/services/eimzo/eimzo-client'
import type { EImzoKeyItem } from '@/types/auth'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

// State
const status = ref<'loading' | 'connected' | 'error' | 'signing'>('loading')
const statusMessage = ref('Connecting to E-IMZO...')
const certificates = ref<EImzoKeyItem[]>([])
const selectedCert = ref<EImzoKeyItem | null>(null)
const challenge = ref('')
const errorMessage = ref('')
const isLoading = ref(false)

const isRegistration = computed(() => route.path.includes('register'))
const pageTitle = computed(() => isRegistration.value ? 'Register via E-IMZO' : 'Sign In via E-IMZO')

onMounted(async () => {
  await initEImzo()
})

async function initEImzo() {
  status.value = 'loading'
  statusMessage.value = 'Connecting to E-IMZO...'
  errorMessage.value = ''

  try {
    const version = await EImzoService.checkConnection()
    statusMessage.value = `E-IMZO connected (v${version.major}.${version.minor})`

    statusMessage.value = 'Installing API keys...'
    await EImzoService.installApiKeys()

    statusMessage.value = 'Loading certificates...'
    const certs = await EImzoService.listCertificates()
    certificates.value = certs

    if (certs.length === 0) {
      status.value = 'error'
      statusMessage.value = 'No certificates found'
      errorMessage.value = 'E-IMZO certificates not found. Make sure the key is connected.'
    } else {
      status.value = 'connected'
      statusMessage.value = `Found ${certs.length} certificate(s)`
      selectedCert.value = certs[0] ?? null
    }
  } catch (error) {
    status.value = 'error'
    statusMessage.value = 'Connection error'
    errorMessage.value = error instanceof Error
      ? error.message
      : 'Could not connect to E-IMZO. Make sure the E-IMZO application is running.'
  }
}

function selectCertificate(cert: EImzoKeyItem) {
  selectedCert.value = cert
}

function formatDate(date: Date): string {
  if (!date || isNaN(date.getTime())) return 'N/A'
  return date.toLocaleDateString('ru-RU', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  })
}

function isExpired(cert: EImzoKeyItem): boolean {
  if (!cert.validTo) return false
  return new Date() > cert.validTo
}

async function handleSign() {
  if (!selectedCert.value) {
    errorMessage.value = 'Select a certificate'
    return
  }

  isLoading.value = true
  status.value = 'signing'
  errorMessage.value = ''

  try {
    statusMessage.value = 'Getting challenge...'
    challenge.value = await api.getEImzoChallenge()

    statusMessage.value = 'Signing... (enter PIN code)'
    const pkcs7 = await EImzoService.signChallenge(selectedCert.value, challenge.value)

    statusMessage.value = 'Verifying signature...'

    if (isRegistration.value) {
      await authStore.registerWithEImzo(pkcs7)
    } else {
      await authStore.loginWithEImzo(pkcs7)
    }

    router.push('/realestates')
  } catch (error) {
    status.value = 'connected'
    statusMessage.value = `Found ${certificates.value.length} certificate(s)`
    errorMessage.value = error instanceof Error ? error.message : 'Authentication error'
  } finally {
    isLoading.value = false
  }
}

function goBack() {
  if (isRegistration.value) {
    router.push('/register')
  } else {
    router.push('/login')
  }
}
</script>

<template>
  <div class="auth-layout">
    <div class="auth-container fade-in" style="max-width: 480px;">
      <div class="auth-logo">
        <svg xmlns="http://www.w3.org/2000/svg" width="170" height="32" viewBox="0 0 170 32" fill="none">
          <path fill-rule="evenodd" clip-rule="evenodd" d="M44.4189 15.4938L53.3025 10.3291V0L44.4189 5.16475V15.4938ZM35.535 10.3291V0L17.7675 10.3291V0L0 10.3291V32H8.88392V15.4938L17.7675 10.3291V32H26.6514V15.4938L35.535 10.3291Z" fill="#FC4D3D"/>
          <path d="M44.4189 15.4938L35.535 10.3291V32H44.4189V15.4938Z" fill="#FC4D3D"/>
          <path d="M158.261 23.6711H155.398V8.32864H158.04L166.228 18.63V8.32864H169.091V23.6711H166.45L158.261 13.3917V23.6711Z" fill="#FC4D3D"/>
          <path d="M144.689 8C147.027 8 148.957 8.76713 150.481 10.3014C152.034 11.8064 152.811 13.706 152.811 16C152.811 18.2795 152.034 20.1863 150.481 21.7206C148.943 23.2402 147.012 24 144.689 24C142.396 24 140.465 23.2402 138.897 21.7206C137.358 20.1717 136.589 18.2649 136.589 16C136.589 13.7206 137.358 11.821 138.897 10.3014C140.45 8.76713 142.381 8 144.689 8ZM141.116 12.3179C140.169 13.2968 139.695 14.5242 139.695 16C139.695 17.4758 140.169 18.7032 141.116 19.6822C142.062 20.6612 143.254 21.1507 144.689 21.1507C146.123 21.1507 147.315 20.6612 148.262 19.6822C149.223 18.6886 149.704 17.4612 149.704 16C149.704 14.5388 149.223 13.3115 148.262 12.3179C147.3 11.3242 146.109 10.8274 144.689 10.8274C143.269 10.8274 142.077 11.3242 141.116 12.3179Z" fill="#FC4D3D"/>
          <path d="M126.311 23.6711H120.009V8.32864H126.311C128.62 8.32864 130.543 9.06649 132.081 10.5424C133.635 12.0181 134.411 13.8373 134.411 15.9999C134.411 18.1624 133.635 19.9816 132.081 21.4574C130.543 22.9332 128.62 23.6711 126.311 23.6711ZM126.311 11.0465H122.983V20.9532H126.311C127.732 20.9532 128.923 20.4784 129.885 19.5286C130.846 18.5496 131.327 17.3734 131.327 15.9999C131.327 14.5971 130.846 13.4209 129.885 12.4711C128.923 11.5213 127.732 11.0465 126.311 11.0465Z" fill="#FC4D3D"/>
          <path d="M109.145 17.4026L103.242 8.32864H106.682L110.61 14.641L114.671 8.32864H118.089L112.119 17.3149V23.6711H109.145V17.4026Z" fill="#FC4D3D"/>
          <path d="M91.6672 23.6711H88.5381L95.196 8.32864H97.6145L104.272 23.6711H101.143L99.9228 20.778H92.8876L91.6672 23.6711ZM98.7686 18.082L96.4164 12.5149L94.0418 18.082H98.7686Z" fill="#FC4D3D"/>
          <path d="M72.5206 23.6711H69.5469V8.32864H72.5206L78.0909 18.0163L83.6389 8.32864H86.6126V23.6711H83.6389V13.8957L79.0673 21.9396H77.0922L72.5206 13.9176V23.6711Z" fill="#FC4D3D"/>
        </svg>
      </div>

      <div class="card">
        <div class="card-header">
          <h2 class="card-title">{{ pageTitle }}</h2>
          <p class="card-subtitle">Use your digital signature for secure access</p>
        </div>

        <!-- Status Badge -->
        <div class="flex items-center justify-center gap-sm mb-lg">
          <span
            v-if="status === 'loading' || status === 'signing'"
            class="spinner"
          ></span>
          <span
            v-else-if="status === 'connected'"
            style="color: var(--color-success);">✓</span>
          <span
            v-else-if="status === 'error'"
            style="color: var(--color-error);">✗</span>
          <span class="text-muted">{{ statusMessage }}</span>
        </div>

        <!-- Error Message -->
        <div v-if="errorMessage" class="alert alert-error mb-lg">
          {{ errorMessage }}
          <button
            v-if="status === 'error'"
            class="btn btn-ghost"
            style="margin-left: auto; padding: 0.25rem 0.5rem;"
            @click="initEImzo"
          >
            Retry
          </button>
        </div>

        <!-- Certificate List -->
        <div v-if="certificates.length > 0" class="mb-lg">
          <p class="form-label">Select Certificate</p>
          <div class="cert-list">
            <div
              v-for="cert in certificates"
              :key="cert.serialNumber"
              class="cert-item"
              :class="{
                selected: selectedCert?.serialNumber === cert.serialNumber,
                'text-muted': isExpired(cert)
              }"
              @click="selectCertificate(cert)"
            >
              <div class="cert-info">
                <div class="cert-name">{{ cert.CN }}</div>
                <div v-if="cert.O && cert.O !== 'NOT SPECIFIED'" class="cert-org">
                  🏢 {{ cert.O }}
                </div>
                <div class="cert-details">
                  <span v-if="cert.TIN">TIN: {{ cert.TIN }}</span>
                  <span v-if="cert.PINFL">PINFL: {{ cert.PINFL }}</span>
                  <span :style="{ color: isExpired(cert) ? 'var(--color-error)' : 'inherit' }">
                    {{ isExpired(cert) ? 'Expired' : `Until ${formatDate(cert.validTo)}` }}
                  </span>
                </div>
              </div>
              <div v-if="selectedCert?.serialNumber === cert.serialNumber" style="color: var(--color-brand);">
                ✓
              </div>
            </div>
          </div>
        </div>

        <!-- Sign Button -->
        <button
          v-if="status === 'connected' || status === 'signing'"
          class="btn btn-primary btn-lg btn-block"
          :disabled="!selectedCert || isLoading || isExpired(selectedCert)"
          @click="handleSign"
        >
          <span v-if="isLoading" class="spinner"></span>
          <span v-else>
            {{ isRegistration ? 'Register' : 'Sign In' }}
          </span>
        </button>

        <!-- Retry Button for Error -->
        <button
          v-if="status === 'error'"
          class="btn btn-primary btn-lg btn-block"
          @click="initEImzo"
        >
          Try Again
        </button>

        <button
          class="btn btn-ghost btn-block mt-lg"
          @click="goBack"
          :disabled="isLoading"
        >
          ← Back
        </button>
      </div>

      <div class="auth-footer">
        <p class="text-muted" style="font-size: 0.8125rem;">
          Make sure the E-IMZO application is running on your computer
        </p>
      </div>
    </div>
  </div>
</template>
