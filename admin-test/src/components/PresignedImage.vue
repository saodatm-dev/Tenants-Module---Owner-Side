<template>
  <div class="presigned-image-container" :class="{ loading: isLoading }">
    <img
      v-if="imageUrl && !hasError"
      :src="imageUrl"
      :alt="alt"
      loading="lazy"
      @load="onLoad"
      @error="onError"
      :class="imageClass"
    />
    <div v-else-if="isLoading" class="image-skeleton">
      <slot name="loading">
        <div class="spinner"></div>
      </slot>
    </div>
    <div v-else class="image-placeholder">
      <slot name="error">
        <span>{{ fallbackEmoji }}</span>
      </slot>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import { usePresignedUrl } from '@/composables/usePresignedUrl'

const props = withDefaults(defineProps<{
  objectKey: string | undefined | null
  alt?: string
  imageClass?: string
  fallbackEmoji?: string
}>(), {
  alt: '',
  imageClass: '',
  fallbackEmoji: '🖼️'
})

const emit = defineEmits<{
  (e: 'load'): void
  (e: 'error'): void
  (e: 'urlLoaded', url: string): void
}>()

const { getPresignedUrl, isLoading } = usePresignedUrl()
const imageUrl = ref<string | null>(null)
const hasError = ref(false)

const loadImage = async () => {
  if (!props.objectKey) {
    imageUrl.value = null
    return
  }

  hasError.value = false
  const url = await getPresignedUrl(props.objectKey)
  imageUrl.value = url

  if (url) {
    emit('urlLoaded', url)
  }
}

const onLoad = () => {
  emit('load')
}

const onError = () => {
  hasError.value = true
  emit('error')
}

watch(() => props.objectKey, loadImage, { immediate: false })
onMounted(loadImage)
</script>

<style scoped>
.presigned-image-container {
  position: relative;
  display: inline-block;
  width: 100%;
  height: 100%;
}

.presigned-image-container img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.image-skeleton {
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
  border-radius: inherit;
  min-height: 100px;
  display: flex;
  align-items: center;
  justify-content: center;
}

@keyframes shimmer {
  0% { background-position: -200% 0; }
  100% { background-position: 200% 0; }
}

.spinner {
  width: 24px;
  height: 24px;
  border: 2px solid rgba(27, 27, 27, 0.1);
  border-top-color: #FF5B3C;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.image-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f5f5f5;
  border-radius: inherit;
  min-height: 100px;
  font-size: 32px;
  opacity: 0.4;
}
</style>
