<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const status = ref<'loading' | 'success' | 'error'>('loading')
const statusMessage = ref('Verifying via OneID...')
const errorMessage = ref('')

onMounted(async () => {
  await handleCallback()
})

async function handleCallback() {
  try {
    // Get code and state from URL query parameters
    const code = route.query.code as string
    const returnedState = route.query.state as string
    const savedState = localStorage.getItem('oauth_state')
    const authMode = localStorage.getItem('auth_mode') || 'login'

    // Validate state to prevent CSRF
    if (savedState && returnedState !== savedState) {
      throw new Error('Security check failed. Please try again.')
    }

    // Validate code
    if (!code) {
      throw new Error('Authorization code not found.')
    }

    // Clear the state after validation
    localStorage.removeItem('oauth_state')
    localStorage.removeItem('auth_mode')

    let response

    // Try login first, if user not found, try registration
    if (authMode === 'login') {
      try {
        response = await authStore.loginWithOneId(code)
        statusMessage.value = 'Successfully signed in!'
      } catch (loginError) {
        // If login fails, user might not exist - try registration
        console.log('Login failed, trying registration...', loginError)
        try {
          response = await authStore.registerWithOneId(code)
          statusMessage.value = 'Successfully registered!'
        } catch {
          throw new Error('Could not sign in. User not found.')
        }
      }
    } else {
      // Registration mode
      try {
        response = await authStore.registerWithOneId(code)
        statusMessage.value = 'Successfully registered!'
      } catch {
        // If registration fails (user exists), try login
        console.log('Registration failed, trying login...')
        try {
          response = await authStore.loginWithOneId(code)
          statusMessage.value = 'You are already registered. Signed in!'
        } catch {
          throw new Error('Registration failed.')
        }
      }
    }

    status.value = 'success'

    // Redirect to dashboard after short delay
    setTimeout(() => {
      router.push('/realestates')
    }, 2000)

  } catch (error) {
    console.error('Authentication error:', error)
    status.value = 'error'
    errorMessage.value = error instanceof Error ? error.message : 'Unknown error occurred.'
  }
}

function goToLogin() {
  router.push('/login')
}
</script>

<template>
  <div class="auth-layout">
    <div class="auth-container fade-in">
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

      <div class="card text-center" style="padding: 3rem;">
        <!-- Loading State -->
        <div v-if="status === 'loading'" class="flex flex-col items-center gap-md">
          <div class="spinner spinner-lg"></div>
          <h3>{{ statusMessage }}</h3>
          <p class="text-muted">Please wait. Verifying your information.</p>
        </div>

        <!-- Success State -->
        <div v-else-if="status === 'success'" class="flex flex-col items-center gap-md">
          <div style="font-size: 4rem; color: var(--color-success);">✓</div>
          <h3>Success!</h3>
          <p class="text-muted">{{ statusMessage }}</p>
          <p style="color: var(--color-success); font-weight: 500;">Redirecting to Dashboard...</p>
        </div>

        <!-- Error State -->
        <div v-else-if="status === 'error'" class="flex flex-col items-center gap-md">
          <div style="font-size: 4rem; color: var(--color-error);">✗</div>
          <h3>An Error Occurred</h3>
          <div class="alert alert-error">
            {{ errorMessage }}
          </div>
          <button class="btn btn-primary mt-lg" @click="goToLogin">
            Try Again
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
