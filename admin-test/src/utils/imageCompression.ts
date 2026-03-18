/**
 * Image compression utility using Canvas API
 * Compresses images before upload to reduce file size
 */

export interface CompressionOptions {
    maxWidth?: number
    maxHeight?: number
    quality?: number  // 0-1, default 0.8
    mimeType?: 'image/jpeg' | 'image/webp'
}

const DEFAULT_OPTIONS: CompressionOptions = {
    maxWidth: 1920,
    maxHeight: 1920,
    quality: 0.8,
    mimeType: 'image/jpeg'
}

/**
 * Compress an image file using Canvas
 * @param file - Original image file
 * @param options - Compression options
 * @returns Compressed image as a new File object
 */
export async function compressImage(
    file: File,
    options: CompressionOptions = {}
): Promise<File> {
    const opts = { ...DEFAULT_OPTIONS, ...options }

    // Skip non-image files
    if (!file.type.startsWith('image/')) {
        return file
    }

    // Skip already small files (< 100KB)
    if (file.size < 100 * 1024) {
        return file
    }

    // Skip GIF and SVG (they don't compress well with this method)
    if (file.type === 'image/gif' || file.type === 'image/svg+xml') {
        return file
    }

    return new Promise((resolve, reject) => {
        const img = new Image()
        const url = URL.createObjectURL(file)

        img.onload = () => {
            URL.revokeObjectURL(url)

            // Calculate new dimensions maintaining aspect ratio
            let { width, height } = img
            const maxW = opts.maxWidth!
            const maxH = opts.maxHeight!

            if (width > maxW || height > maxH) {
                const ratio = Math.min(maxW / width, maxH / height)
                width = Math.round(width * ratio)
                height = Math.round(height * ratio)
            }

            // Create canvas and draw resized image
            const canvas = document.createElement('canvas')
            canvas.width = width
            canvas.height = height

            const ctx = canvas.getContext('2d')
            if (!ctx) {
                resolve(file) // Fallback to original
                return
            }

            // Use better quality scaling
            ctx.imageSmoothingEnabled = true
            ctx.imageSmoothingQuality = 'high'
            ctx.drawImage(img, 0, 0, width, height)

            // Convert to blob
            canvas.toBlob(
                (blob) => {
                    if (!blob) {
                        resolve(file) // Fallback to original
                        return
                    }

                    // Only use compressed version if it's smaller
                    if (blob.size >= file.size) {
                        resolve(file)
                        return
                    }

                    // Create new File with original name
                    const extension = opts.mimeType === 'image/webp' ? '.webp' : '.jpg'
                    const baseName = file.name.replace(/\.[^.]+$/, '')
                    const compressedFile = new File(
                        [blob],
                        `${baseName}${extension}`,
                        { type: opts.mimeType, lastModified: Date.now() }
                    )

                    console.log(
                        `Image compressed: ${file.name} (${formatBytes(file.size)} → ${formatBytes(compressedFile.size)}, ${Math.round((1 - compressedFile.size / file.size) * 100)}% reduction)`
                    )

                    resolve(compressedFile)
                },
                opts.mimeType,
                opts.quality
            )
        }

        img.onerror = () => {
            URL.revokeObjectURL(url)
            reject(new Error('Failed to load image for compression'))
        }

        img.src = url
    })
}

/**
 * Compress multiple images
 */
export async function compressImages(
    files: File[],
    options: CompressionOptions = {}
): Promise<File[]> {
    return Promise.all(files.map(file => compressImage(file, options)))
}

/**
 * Format bytes to human readable string
 */
function formatBytes(bytes: number): string {
    if (bytes < 1024) return `${bytes} B`
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`
    return `${(bytes / (1024 * 1024)).toFixed(2)} MB`
}
