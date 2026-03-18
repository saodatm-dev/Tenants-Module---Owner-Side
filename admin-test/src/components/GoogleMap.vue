<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue'

// Type declarations for Google Maps (loaded dynamically)
declare global {
  interface Window {
    google: typeof google
  }
}

const props = defineProps<{
  latitude?: number | null
  longitude?: number | null
  editable?: boolean
  height?: string
  enableReverseGeocoding?: boolean
}>()

const emit = defineEmits<{
  (e: 'update:location', location: { lat: number; lng: number; address?: string }): void
}>()

const mapContainer = ref<HTMLDivElement | null>(null)
const searchInput = ref<HTMLInputElement | null>(null)
let map: google.maps.Map | null = null
let marker: google.maps.Marker | null = null
let autocomplete: google.maps.places.Autocomplete | null = null
let geocoder: google.maps.Geocoder | null = null

// Default to Tashkent, Uzbekistan
const defaultCenter = { lat: 41.311081, lng: 69.240562 }

// Load Google Maps Script
function loadGoogleMapsScript(): Promise<void> {
  return new Promise((resolve, reject) => {
    if (window.google?.maps) {
      resolve()
      return
    }

    const apiKey = import.meta.env.VITE_GOOGLE_MAP_API_KEY
    if (!apiKey) {
      reject(new Error('Google Maps API key not found'))
      return
    }

    const script = document.createElement('script')
    script.src = `https://maps.googleapis.com/maps/api/js?key=${apiKey}&libraries=places`
    script.async = true
    script.defer = true
    script.onload = () => resolve()
    script.onerror = () => reject(new Error('Failed to load Google Maps'))
    document.head.appendChild(script)
  })
}

function initMap() {
  if (!mapContainer.value) return

  const center = {
    lat: props.latitude || defaultCenter.lat,
    lng: props.longitude || defaultCenter.lng
  }

  map = new google.maps.Map(mapContainer.value, {
    center,
    zoom: props.latitude && props.longitude ? 16 : 12,
    styles: [
      {
        featureType: 'poi',
        elementType: 'labels',
        stylers: [{ visibility: 'off' }]
      }
    ],
    mapTypeControl: true,
    streetViewControl: false,
    fullscreenControl: true
  })

  // Create marker
  marker = new google.maps.Marker({
    position: center,
    map,
    draggable: props.editable,
    animation: google.maps.Animation.DROP,
    title: 'Location'
  })

  // Only show marker if we have coordinates
  if (!props.latitude || !props.longitude) {
    marker.setVisible(false)
  }

  // Initialize geocoder for reverse geocoding
  if (props.enableReverseGeocoding) {
    geocoder = new google.maps.Geocoder()
  }

  // Reverse geocode and emit location with address
  async function emitLocationWithAddress(lat: number, lng: number) {
    if (props.enableReverseGeocoding && geocoder) {
      try {
        const response = await geocoder.geocode({ location: { lat, lng } })
        const address = response.results?.[0]?.formatted_address
        if (address) {
          emit('update:location', { lat, lng, address })
          return
        }
      } catch (error) {
        console.error('Reverse geocoding error:', error)
      }
    }
    // Fallback without address
    emit('update:location', { lat, lng })
  }

  // Handle marker drag
  if (props.editable) {
    marker.addListener('dragend', () => {
      const position = marker?.getPosition()
      if (position) {
        emitLocationWithAddress(position.lat(), position.lng())
      }
    })

    // Handle map click
    map.addListener('click', (event: google.maps.MapMouseEvent) => {
      if (event.latLng && marker) {
        marker.setPosition(event.latLng)
        marker.setVisible(true)
        emitLocationWithAddress(event.latLng.lat(), event.latLng.lng())
      }
    })

    // Initialize Places Autocomplete
    if (searchInput.value) {
      autocomplete = new google.maps.places.Autocomplete(searchInput.value, {
        types: ['geocode', 'establishment'],
        componentRestrictions: { country: 'uz' }
      })

      autocomplete.addListener('place_changed', () => {
        const place = autocomplete?.getPlace()
        if (place?.geometry?.location && map && marker) {
          map.setCenter(place.geometry.location)
          map.setZoom(16)
          marker.setPosition(place.geometry.location)
          marker.setVisible(true)
          // Use place name/address from autocomplete
          const address = place.formatted_address || place.name
          emit('update:location', {
            lat: place.geometry.location.lat(),
            lng: place.geometry.location.lng(),
            address
          })
        }
      })
    }
  }
}

// Watch for external coordinate changes
watch([() => props.latitude, () => props.longitude], ([lat, lng]) => {
  if (map && marker && lat && lng) {
    const position = new google.maps.LatLng(lat, lng)
    map.setCenter(position)
    marker.setPosition(position)
    marker.setVisible(true)
  }
})

onMounted(async () => {
  try {
    await loadGoogleMapsScript()
    initMap()
  } catch (error) {
    console.error('Error loading Google Maps:', error)
  }
})

onUnmounted(() => {
  if (marker) {
    google.maps.event.clearInstanceListeners(marker)
  }
  if (map) {
    google.maps.event.clearInstanceListeners(map)
  }
  if (autocomplete) {
    google.maps.event.clearInstanceListeners(autocomplete)
  }
})
</script>

<template>
  <div class="google-map-wrapper">
    <div v-if="editable" class="search-container">
      <input
        ref="searchInput"
        type="text"
        class="search-input"
        placeholder="Search address..."
      />
    </div>
    <div 
      ref="mapContainer" 
      class="map-container"
      :style="{ height: height || '400px' }"
    ></div>
    <p v-if="editable" class="map-hint">
      💡 Click on the map or drag the marker to select location
    </p>
  </div>
</template>

<style scoped>
.google-map-wrapper {
  width: 100%;
}

.search-container {
  margin-bottom: 12px;
}

.search-input {
  width: 100%;
  padding: 12px 16px;
  border: 1px solid var(--color-border, #e5e7eb);
  border-radius: 8px;
  font-size: 14px;
  background: var(--color-surface, white);
  color: var(--color-text, #1a1a2e);
  transition: border-color 0.2s, box-shadow 0.2s;
}

.search-input:focus {
  outline: none;
  border-color: var(--color-brand, #4f46e5);
  box-shadow: 0 0 0 3px rgba(79, 70, 229, 0.1);
}

.search-input::placeholder {
  color: var(--color-text-muted, #9ca3af);
}

.map-container {
  width: 100%;
  border-radius: 12px;
  overflow: hidden;
  border: 1px solid var(--color-border, #e5e7eb);
}

.map-hint {
  margin-top: 8px;
  font-size: 13px;
  color: var(--color-text-muted, #6b7280);
  text-align: center;
}
</style>
