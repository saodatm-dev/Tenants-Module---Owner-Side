import { ReactNode } from 'react'
import { useApp } from '../context/AppContext'
import { Edit, Trash2 } from 'lucide-react'
import clsx from 'clsx'

interface CardGridProps<T> {
    data: T[]
    isLoading?: boolean
    renderCard?: (item: T) => ReactNode
    onEdit?: (item: T) => void
    onDelete?: (item: T) => void
    getId?: (item: T) => string
    getTitle?: (item: T) => string | undefined
    getSubtitle?: (item: T) => string | undefined
    getImage?: (item: T) => string | undefined
    columns?: 2 | 3 | 4
}

export default function CardGrid<T extends object>({
    data,
    isLoading,
    renderCard,
    onEdit,
    onDelete,
    getId = (item) => (item as { id?: string }).id ?? '',
    getTitle = (item) => (item as { name?: string }).name,
    getSubtitle,
    getImage,
    columns = 3,
}: CardGridProps<T>) {
    const { t } = useApp()

    if (isLoading) {
        return (
            <div className={clsx(
                'grid gap-4',
                columns === 2 && 'grid-cols-1 sm:grid-cols-2',
                columns === 3 && 'grid-cols-1 sm:grid-cols-2 lg:grid-cols-3',
                columns === 4 && 'grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4'
            )}>
                {Array.from({ length: 6 }).map((_, i) => (
                    <div key={i} className="card p-4 animate-pulse">
                        <div className="h-32 bg-slate-200 dark:bg-slate-700 rounded-lg mb-4" />
                        <div className="h-4 bg-slate-200 dark:bg-slate-700 rounded w-3/4 mb-2" />
                        <div className="h-3 bg-slate-200 dark:bg-slate-700 rounded w-1/2" />
                    </div>
                ))}
            </div>
        )
    }

    if (data.length === 0) {
        return (
            <div className="card p-12 text-center">
                <div className="w-16 h-16 bg-slate-100 dark:bg-slate-800 rounded-full flex items-center justify-center mx-auto mb-4">
                    <svg className="w-8 h-8 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
                    </svg>
                </div>
                <p className="text-slate-500 dark:text-slate-400">{t('noData')}</p>
            </div>
        )
    }

    const showActions = onEdit || onDelete

    return (
        <div className={clsx(
            'grid gap-4',
            columns === 2 && 'grid-cols-1 sm:grid-cols-2',
            columns === 3 && 'grid-cols-1 sm:grid-cols-2 lg:grid-cols-3',
            columns === 4 && 'grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4'
        )}>
            {data.map(item => (
                <div
                    key={getId(item)}
                    className="card group hover:shadow-lg hover:border-primary-200 dark:hover:border-primary-800 transition-all duration-200"
                >
                    {renderCard ? (
                        renderCard(item)
                    ) : (
                        <>
                            {/* Card Image */}
                            {getImage && (
                                <div className="aspect-video bg-gradient-to-br from-slate-100 to-slate-200 dark:from-slate-800 dark:to-slate-700 rounded-t-xl overflow-hidden">
                                    {getImage(item) ? (
                                        <img
                                            src={getImage(item)}
                                            alt={getTitle(item)}
                                            className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
                                        />
                                    ) : (
                                        <div className="w-full h-full flex items-center justify-center">
                                            <svg className="w-12 h-12 text-slate-300 dark:text-slate-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                                            </svg>
                                        </div>
                                    )}
                                </div>
                            )}

                            {/* Card Content */}
                            <div className="p-4">
                                <h3 className="font-semibold text-slate-900 dark:text-slate-100 truncate">
                                    {getTitle(item) || 'Untitled'}
                                </h3>
                                {getSubtitle && (
                                    <p className="text-sm text-slate-500 dark:text-slate-400 mt-1 truncate">
                                        {getSubtitle(item)}
                                    </p>
                                )}

                                {/* Actions */}
                                {showActions && (
                                    <div className="flex items-center gap-2 mt-4 pt-4 border-t border-slate-100 dark:border-slate-800">
                                        {onEdit && (
                                            <button
                                                onClick={() => onEdit(item)}
                                                className="flex-1 flex items-center justify-center gap-2 px-3 py-2 rounded-lg text-sm font-medium text-primary-600 dark:text-primary-400 hover:bg-primary-50 dark:hover:bg-primary-900/20 transition-colors"
                                            >
                                                <Edit size={14} />
                                                {t('edit')}
                                            </button>
                                        )}
                                        {onDelete && (
                                            <button
                                                onClick={() => onDelete(item)}
                                                className="flex-1 flex items-center justify-center gap-2 px-3 py-2 rounded-lg text-sm font-medium text-red-600 dark:text-red-400 hover:bg-red-50 dark:hover:bg-red-900/20 transition-colors"
                                            >
                                                <Trash2 size={14} />
                                                {t('delete')}
                                            </button>
                                        )}
                                    </div>
                                )}
                            </div>
                        </>
                    )}
                </div>
            ))}
        </div>
    )
}
