import { useState, useCallback, useEffect } from 'react'
import { X, ChevronLeft, ChevronRight, ZoomIn, Image as ImageIcon } from 'lucide-react'
import { useApp } from '../../context/AppContext'

interface ImageGalleryProps {
    images: string[]
    title?: string
}

export default function ImageGallery({ images, title }: ImageGalleryProps) {
    const { t } = useApp()
    const [lightboxIndex, setLightboxIndex] = useState<number | null>(null)

    const openLightbox = useCallback((index: number) => setLightboxIndex(index), [])
    const closeLightbox = useCallback(() => setLightboxIndex(null), [])

    const goNext = useCallback(() => {
        if (lightboxIndex !== null) setLightboxIndex((lightboxIndex + 1) % images.length)
    }, [lightboxIndex, images.length])

    const goPrev = useCallback(() => {
        if (lightboxIndex !== null) setLightboxIndex((lightboxIndex - 1 + images.length) % images.length)
    }, [lightboxIndex, images.length])

    // Keyboard navigation
    useEffect(() => {
        if (lightboxIndex === null) return
        const handler = (e: KeyboardEvent) => {
            if (e.key === 'Escape') closeLightbox()
            if (e.key === 'ArrowRight') goNext()
            if (e.key === 'ArrowLeft') goPrev()
        }
        window.addEventListener('keydown', handler)
        return () => window.removeEventListener('keydown', handler)
    }, [lightboxIndex, closeLightbox, goNext, goPrev])

    if (!images.length) return null

    return (
        <>
            <div className="card p-5">
                <h3 className="text-sm font-semibold text-slate-500 uppercase tracking-wider mb-4 flex items-center gap-2">
                    <ImageIcon size={16} className="text-slate-400" />
                    {title || t('images')}
                    <span className="ml-1 text-xs font-normal text-slate-400">({images.length})</span>
                </h3>
                <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-3">
                    {images.map((src, i) => (
                        <button
                            key={i}
                            onClick={() => openLightbox(i)}
                            className="group relative aspect-[4/3] rounded-xl overflow-hidden border border-slate-200 dark:border-slate-700 bg-slate-100 dark:bg-slate-800 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2 transition-all hover:shadow-lg hover:scale-[1.02]"
                        >
                            <img
                                src={src}
                                alt={`Image ${i + 1}`}
                                className="w-full h-full object-cover transition-transform duration-300 group-hover:scale-105"
                                loading="lazy"
                            />
                            <div className="absolute inset-0 bg-black/0 group-hover:bg-black/20 transition-colors flex items-center justify-center">
                                <ZoomIn size={24} className="text-white opacity-0 group-hover:opacity-100 transition-opacity drop-shadow-lg" />
                            </div>
                        </button>
                    ))}
                </div>
            </div>

            {/* Lightbox */}
            {lightboxIndex !== null && (
                <div
                    className="fixed inset-0 z-[100] bg-black/90 backdrop-blur-md flex items-center justify-center animate-in"
                    onClick={closeLightbox}
                >
                    {/* Close button */}
                    <button
                        onClick={closeLightbox}
                        className="absolute top-4 right-4 p-2 rounded-full bg-white/10 hover:bg-white/20 text-white transition-colors z-10"
                    >
                        <X size={24} />
                    </button>

                    {/* Counter */}
                    <div className="absolute top-4 left-4 px-3 py-1.5 rounded-full bg-white/10 text-white text-sm font-medium z-10">
                        {lightboxIndex + 1} / {images.length}
                    </div>

                    {/* Navigation */}
                    {images.length > 1 && (
                        <>
                            <button
                                onClick={e => { e.stopPropagation(); goPrev() }}
                                className="absolute left-4 top-1/2 -translate-y-1/2 p-3 rounded-full bg-white/10 hover:bg-white/20 text-white transition-colors z-10"
                            >
                                <ChevronLeft size={28} />
                            </button>
                            <button
                                onClick={e => { e.stopPropagation(); goNext() }}
                                className="absolute right-4 top-1/2 -translate-y-1/2 p-3 rounded-full bg-white/10 hover:bg-white/20 text-white transition-colors z-10"
                            >
                                <ChevronRight size={28} />
                            </button>
                        </>
                    )}

                    {/* Image */}
                    <img
                        src={images[lightboxIndex]}
                        alt={`Image ${lightboxIndex + 1}`}
                        className="max-w-[90vw] max-h-[85vh] object-contain rounded-lg shadow-2xl"
                        onClick={e => e.stopPropagation()}
                    />

                    {/* Thumbnail strip */}
                    {images.length > 1 && (
                        <div className="absolute bottom-4 left-1/2 -translate-x-1/2 flex gap-2 max-w-[90vw] overflow-x-auto p-2 rounded-xl bg-black/40 backdrop-blur-sm" onClick={e => e.stopPropagation()}>
                            {images.map((src, i) => (
                                <button
                                    key={i}
                                    onClick={() => setLightboxIndex(i)}
                                    className={`flex-shrink-0 w-16 h-12 rounded-lg overflow-hidden border-2 transition-all ${i === lightboxIndex ? 'border-white scale-105' : 'border-transparent opacity-60 hover:opacity-100'}`}
                                >
                                    <img src={src} alt="" className="w-full h-full object-cover" />
                                </button>
                            ))}
                        </div>
                    )}
                </div>
            )}
        </>
    )
}

// Compact thumbnail for list rows
export function ImageThumbnail({ images }: { images?: string[] }) {
    if (!images?.length) {
        return (
            <div className="w-10 h-10 rounded-lg bg-slate-100 dark:bg-slate-800 flex items-center justify-center">
                <ImageIcon size={16} className="text-slate-400" />
            </div>
        )
    }

    return (
        <div className="flex items-center -space-x-2">
            {images.slice(0, 3).map((src, i) => (
                <img
                    key={i}
                    src={src}
                    alt=""
                    className="w-10 h-10 rounded-lg object-cover border-2 border-white dark:border-slate-900 shadow-sm"
                    loading="lazy"
                />
            ))}
            {images.length > 3 && (
                <div className="w-10 h-10 rounded-lg bg-slate-200 dark:bg-slate-700 border-2 border-white dark:border-slate-900 flex items-center justify-center shadow-sm">
                    <span className="text-xs font-semibold text-slate-600 dark:text-slate-300">+{images.length - 3}</span>
                </div>
            )}
        </div>
    )
}
