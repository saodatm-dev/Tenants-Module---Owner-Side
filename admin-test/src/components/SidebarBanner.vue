<script setup lang="ts">
interface PreviewImage {
  src: string
  label?: string
}

defineProps<{
  images?: PreviewImage[]
  title?: string
  subtitle?: string
  buttonText?: string
}>()

const emit = defineEmits<{
  action: []
}>()
</script>

<template>
  <div class="sidebar-banner">
    <div v-if="images?.length" class="banner-previews">
      <div
        v-for="(img, i) in images.slice(0, 3)"
        :key="i"
        class="preview-item"
      >
        <img :src="img.src" :alt="img.label || 'Preview'" />
        <span v-if="img.label" class="preview-label">{{ img.label }}</span>
      </div>
    </div>

    <div class="banner-content">
      <h4 class="banner-title">{{ title || 'Пора выйти на рынок' }}</h4>
      <p class="banner-subtitle">{{ subtitle || 'Опубликуйте объекты и получайте заявки' }}</p>

      <button class="banner-button" @click="emit('action')">
        {{ buttonText || 'Выбрать объект' }}
      </button>
    </div>
  </div>
</template>

<style scoped>
/* SidebarBanner - Figma Exact Values */
.sidebar-banner {
  background: #FFFFFF;
  border-radius: 24px;
  padding: 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
  border: 0.5px solid rgba(27, 27, 27, 0.04);
}

.banner-previews {
  display: flex;
  gap: 8px;
  margin-bottom: 16px;
}

.preview-item {
  flex: 1;
  position: relative;
  aspect-ratio: 1;
  border-radius: 14px;
  overflow: hidden;
}

.preview-item img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.preview-label {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  padding: 8px;
  font-size: 10px;
  font-weight: 500;
  color: white;
  background: linear-gradient(transparent, rgba(0, 0, 0, 0.7));
  text-overflow: ellipsis;
  overflow: hidden;
  white-space: nowrap;
}

.banner-content {
  text-align: left;
}

.banner-title {
  font-size: 18px;
  font-weight: 700;
  color: #1B1B1B;
  margin: 0 0 4px;
  line-height: 1.3;
}

.banner-subtitle {
  font-size: 13px;
  color: rgba(27, 27, 27, 0.6);
  margin: 0 0 16px;
  line-height: 1.4;
}

.banner-button {
  width: 100%;
  padding: 12px 20px;
  font-size: 14px;
  font-weight: 600;
  color: white;
  background: #1B1B1B;
  border: none;
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.banner-button:hover {
  background: #333333;
  transform: translateY(-1px);
}
</style>
