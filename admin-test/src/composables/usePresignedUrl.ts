import { ref } from 'vue'
import { api } from '@/services/api'

interface UrlCache {
    url: string
    expiresAt: number
}

// Global cache for presigned URLs - shared across all component instances
const urlCache = new Map<string, UrlCache>()
const CACHE_DURATION_MS = 55 * 60 * 1000 // 55 minutes (URL valid for 60)

export function usePresignedUrl() {
    const isLoading = ref(false)
    const error = ref<string | null>(null)

    /**
     * Get a presigned URL for direct MinIO download
     * Uses cached URL if valid, otherwise fetches new one
     */
    const getPresignedUrl = async (objectKey: string): Promise<string | null> => {
        if (!objectKey) return null

        // Check cache first
        const cached = urlCache.get(objectKey)
        if (cached && Date.now() < cached.expiresAt) {
            return cached.url
        }

        isLoading.value = true
        error.value = null

        try {
            const url = await api.getPresignedDownloadUrl(objectKey)

            // Cache the URL
            urlCache.set(objectKey, {
                url,
                expiresAt: Date.now() + CACHE_DURATION_MS
            })

            return url
        } catch (e) {
            error.value = 'Failed to load image'
            console.error('Presigned URL error:', e)
            return null
        } finally {
            isLoading.value = false
        }
    }

    /**
     * Get presigned URLs for multiple object keys in parallel
     */
    const getMultipleUrls = async (objectKeys: string[]): Promise<Map<string, string>> => {
        const results = new Map<string, string>()

        // Process in parallel for speed
        const promises = objectKeys.map(async (key) => {
            const url = await getPresignedUrl(key)
            if (url) results.set(key, url)
        })

        await Promise.all(promises)
        return results
    }

    /**
     * Clear cached URL for a specific key (useful after upload/delete)
     */
    const invalidateCache = (objectKey: string) => {
        urlCache.delete(objectKey)
    }

    /**
     * Clear all cached URLs
     */
    const clearCache = () => {
        urlCache.clear()
    }

    return {
        getPresignedUrl,
        getMultipleUrls,
        invalidateCache,
        clearCache,
        isLoading,
        error
    }
}
