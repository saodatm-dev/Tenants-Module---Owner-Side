// Authentication Store
import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import { api } from '@/services/api'
import type { AuthenticationResponse, SessionClaims, UserInfo, LoginCredentials } from '@/types/auth'

// JWT token decoder
function decodeToken(token: string): SessionClaims | null {
    try {
        const base64Url = token.split('.')[1]
        if (!base64Url) return null
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
        const jsonPayload = decodeURIComponent(
            atob(base64)
                .split('')
                .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
                .join('')
        )
        const payload = JSON.parse(jsonPayload)

        // Parse boolean claims (they come as strings "True"/"False")
        const parseBoolean = (value: unknown): boolean => {
            if (typeof value === 'boolean') return value
            if (typeof value === 'string') return value.toLowerCase() === 'true'
            return false
        }

        return {
            // JWT standard claims use underscore naming (matches backend IExecutionContextProvider)
            userId: payload.user_id || payload.sub,
            tenantId: payload.tenant_id,
            accountId: payload.account_id,
            sessionId: payload.session_id,
            roleId: payload.role_id,
            accountType: payload.account_type || '0',
            isIndividual: parseBoolean(payload.is_individual),
            isOwner: parseBoolean(payload.is_owner),
            exp: payload.exp,
        }
    } catch {
        return null
    }
}

export const useAuthStore = defineStore('auth', () => {
    // State
    const token = ref<string | null>(localStorage.getItem('auth_token'))
    const refreshToken = ref<string | null>(localStorage.getItem('refresh_token'))
    const tokenExpiry = ref<Date | null>(null)
    const isLoading = ref(false)
    const error = ref<string | null>(null)

    // Computed
    const isAuthenticated = computed(() => !!token.value && !isTokenExpired())

    const claims = computed<SessionClaims | null>(() => {
        if (!token.value) return null
        return decodeToken(token.value)
    })

    const userInfo = computed<UserInfo | null>(() => {
        if (!claims.value) return null
        return {
            userId: claims.value.userId,
            tenantId: claims.value.tenantId,
            isIndividual: claims.value.isIndividual,
            isOwner: claims.value.isOwner,
            accountType: parseInt(claims.value.accountType) || 0, // 0 = Client, 1 = Owner
        }
    })

    // Helpers
    function isTokenExpired(): boolean {
        if (!claims.value?.exp) return true
        return Date.now() >= claims.value.exp * 1000
    }

    function setAuthData(response: AuthenticationResponse) {
        token.value = response.token
        refreshToken.value = response.refreshToken
        tokenExpiry.value = new Date(response.tokenExpiredTime)

        localStorage.setItem('auth_token', response.token)
        localStorage.setItem('refresh_token', response.refreshToken)
        localStorage.setItem('token_expiry', response.tokenExpiredTime)
        localStorage.setItem('refresh_token_expiry', response.refreshTokenExpiredTime)
    }

    function clearAuthData() {
        token.value = null
        refreshToken.value = null
        tokenExpiry.value = null

        localStorage.removeItem('auth_token')
        localStorage.removeItem('refresh_token')
        localStorage.removeItem('token_expiry')
        localStorage.removeItem('refresh_token_expiry')
    }

    // Actions
    async function login(credentials: LoginCredentials) {
        isLoading.value = true
        error.value = null

        try {
            const response = await api.login(credentials)
            setAuthData(response)
            return response
        } catch (e) {
            error.value = e instanceof Error ? e.message : 'Login failed'
            throw e
        } finally {
            isLoading.value = false
        }
    }

    async function loginWithEImzo(pkcs7: string) {
        isLoading.value = true
        error.value = null

        try {
            const response = await api.eimzoLogin({ pkcs7 })
            setAuthData(response)
            return response
        } catch (e) {
            error.value = e instanceof Error ? e.message : 'E-IMZO login failed'
            throw e
        } finally {
            isLoading.value = false
        }
    }

    async function registerWithEImzo(pkcs7: string) {
        isLoading.value = true
        error.value = null

        try {
            const response = await api.eimzoRegister({ pkcs7 })
            setAuthData(response)
            return response
        } catch (e) {
            error.value = e instanceof Error ? e.message : 'E-IMZO registration failed'
            throw e
        } finally {
            isLoading.value = false
        }
    }

    async function loginWithOneId(code: string) {
        isLoading.value = true
        error.value = null

        try {
            const response = await api.oneidLogin({ code })
            setAuthData(response)
            return response
        } catch (e) {
            error.value = e instanceof Error ? e.message : 'OneID login failed'
            throw e
        } finally {
            isLoading.value = false
        }
    }

    async function registerWithOneId(code: string) {
        isLoading.value = true
        error.value = null

        try {
            const response = await api.oneidRegister({ code })
            setAuthData(response)
            return response
        } catch (e) {
            error.value = e instanceof Error ? e.message : 'OneID registration failed'
            throw e
        } finally {
            isLoading.value = false
        }
    }

    async function logout() {
        try {
            await api.logout()
        } catch {
            // Ignore logout API errors
        } finally {
            clearAuthData()
        }
    }

    async function tryRefreshToken() {
        if (!refreshToken.value) return false

        try {
            const response = await api.refreshToken(refreshToken.value)
            setAuthData(response)
            return true
        } catch {
            clearAuthData()
            return false
        }
    }

    // Initialize - check token validity on store creation
    if (token.value && isTokenExpired()) {
        tryRefreshToken()
    }

    return {
        // State
        token,
        refreshToken,
        tokenExpiry,
        isLoading,
        error,
        // Computed
        isAuthenticated,
        claims,
        userInfo,
        // Actions
        login,
        loginWithEImzo,
        registerWithEImzo,
        loginWithOneId,
        registerWithOneId,
        logout,
        tryRefreshToken,
        setAuthData,
        clearAuthData,
    }
})
