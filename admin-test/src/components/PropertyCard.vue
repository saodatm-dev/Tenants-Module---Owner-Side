<script setup lang="ts">
defineProps<{
  image?: string | null
  title: string
  subtitle?: string
  location?: string
  price?: string | number | null
  priceLabel?: string
  badges?: { text: string; type?: 'success' | 'warning' | 'info' | 'default' }[]
  meta?: { icon?: string; value: string }[]
  clickable?: boolean
}>()

const emit = defineEmits<{
  click: []
}>()

function formatPrice(price: string | number | null | undefined): string {
  if (!price) return 'Negotiable'
  const num = typeof price === 'string' ? parseFloat(price) : price
  if (isNaN(num)) return 'Negotiable'
  // Format as currency (assuming tiyins, divide by 100)
  const formatted = new Intl.NumberFormat('ru-RU').format(num / 100)
  return `${formatted} UZS`
}
</script>

<template>
  <div
    class="property-card"
    :class="{ clickable }"
    @click="clickable && emit('click')"
  >
    <div class="property-card__image">
      <img v-if="image" :src="image" :alt="title" />
      <div v-else class="property-card__placeholder">
        <span>🏢</span>
      </div>
      <div v-if="badges?.length" class="property-card__badges">
        <span
          v-for="(badge, i) in badges"
          :key="i"
          class="property-badge"
          :class="`property-badge--${badge.type || 'default'}`"
        >
          {{ badge.text }}
        </span>
      </div>
    </div>

    <div class="property-card__content">
      <h4 class="property-card__title">{{ title }}</h4>
      <p v-if="subtitle" class="property-card__subtitle">{{ subtitle }}</p>
      <p v-if="location" class="property-card__location">{{ location }}</p>

      <div v-if="meta?.length" class="property-card__meta">
        <span v-for="(item, i) in meta" :key="i" class="meta-item">
          <span v-if="item.icon" class="meta-icon">{{ item.icon }}</span>
          {{ item.value }}
        </span>
      </div>

      <div v-if="price !== undefined" class="property-card__price">
        <span class="price-value">{{ formatPrice(price) }}</span>
        <span v-if="priceLabel" class="price-label">{{ priceLabel }}</span>
      </div>
    </div>
  </div>
</template>

<style scoped>
.property-card {
  background: var(--bg-primary, #FFFFFF);
  border-radius: 16px;
  overflow: hidden;
  transition: all 0.25s ease;
  border: 1px solid transparent;
}

.property-card.clickable {
  cursor: pointer;
}

.property-card.clickable:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 32px rgba(0, 0, 0, 0.1);
  border-color: var(--color-brand, #FF5B3C);
}

.property-card__image {
  position: relative;
  height: 160px;
  background: var(--bg-secondary, #F7F7F7);
  overflow: hidden;
}

.property-card__image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.property-card__placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 48px;
  opacity: 0.3;
}

.property-card__badges {
  position: absolute;
  top: 12px;
  left: 12px;
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
}

.property-badge {
  padding: 4px 10px;
  font-size: 11px;
  font-weight: 600;
  border-radius: 6px;
  text-transform: uppercase;
}

.property-badge--default {
  background: rgba(27, 27, 27, 0.8);
  color: white;
}

.property-badge--success {
  background: var(--color-success, #22C55E);
  color: white;
}

.property-badge--warning {
  background: var(--color-warning, #F59E0B);
  color: white;
}

.property-badge--info {
  background: var(--color-brand, #FF5B3C);
  color: white;
}

.property-card__content {
  padding: 16px;
}

.property-card__title {
  font-size: 15px;
  font-weight: 600;
  color: var(--text-primary, #1B1B1B);
  margin: 0 0 4px;
  line-height: 1.3;
}

.property-card__subtitle {
  font-size: 13px;
  color: var(--text-muted, rgba(27, 27, 27, 0.4));
  margin: 0 0 4px;
}

.property-card__location {
  font-size: 13px;
  color: var(--text-secondary, rgba(27, 27, 27, 0.6));
  margin: 0 0 8px;
}

.property-card__meta {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
  margin-bottom: 8px;
}

.meta-item {
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: 12px;
  color: var(--text-muted, rgba(27, 27, 27, 0.4));
}

.meta-icon {
  font-size: 14px;
}

.property-card__price {
  display: flex;
  align-items: baseline;
  gap: 4px;
  margin-top: 8px;
  padding-top: 8px;
  border-top: 1px solid var(--border-color, rgba(27, 27, 27, 0.08));
}

.price-value {
  font-size: 14px;
  font-weight: 700;
  color: var(--color-brand, #FF5B3C);
}

.price-label {
  font-size: 12px;
  color: var(--text-muted, rgba(27, 27, 27, 0.4));
}
</style>
