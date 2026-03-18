import { ChevronLeft, ChevronRight, ChevronsLeft, ChevronsRight } from 'lucide-react'
import clsx from 'clsx'
import { useApp } from '../context/AppContext'

interface PaginationProps {
    pageNumber: number
    totalPages: number
    totalCount?: number
    pageSize?: number
    pageSizeOptions?: number[]
    onPageChange: (page: number) => void
    onPageSizeChange?: (size: number) => void
    showPageSize?: boolean
    showTotalCount?: boolean
    compact?: boolean
}

export default function Pagination({
    pageNumber,
    totalPages,
    totalCount,
    pageSize = 10,
    pageSizeOptions = [10, 25, 50, 100],
    onPageChange,
    onPageSizeChange,
    showPageSize = true,
    showTotalCount = true,
    compact = false,
}: PaginationProps) {
    const { t } = useApp()

    const canGoPrevious = pageNumber > 1
    const canGoNext = pageNumber < totalPages
    const showNavigation = totalPages > 1

    // Generate page numbers to display
    const getPageNumbers = () => {
        const pages: (number | 'ellipsis')[] = []
        const maxVisible = compact ? 3 : 5

        if (totalPages <= maxVisible + 2) {
            // Show all pages
            for (let i = 1; i <= totalPages; i++) {
                pages.push(i)
            }
        } else {
            // Always show first page
            pages.push(1)

            if (pageNumber > 3) {
                pages.push('ellipsis')
            }

            // Show pages around current
            const start = Math.max(2, pageNumber - 1)
            const end = Math.min(totalPages - 1, pageNumber + 1)

            for (let i = start; i <= end; i++) {
                pages.push(i)
            }

            if (pageNumber < totalPages - 2) {
                pages.push('ellipsis')
            }

            // Always show last page
            pages.push(totalPages)
        }

        return pages
    }

    const buttonClasses = 'p-2 rounded-lg transition-colors disabled:opacity-40 disabled:cursor-not-allowed'
    const activeButtonClasses = 'text-slate-600 dark:text-slate-400 hover:bg-slate-100 dark:hover:bg-slate-800'
    const pageButtonClasses = 'min-w-[36px] h-9 flex items-center justify-center rounded-lg text-sm font-medium transition-colors'

    return (
        <div className={clsx(
            'flex items-center justify-between gap-4 px-4 py-3',
            'border-t border-slate-200 dark:border-slate-800',
            'bg-white dark:bg-slate-900'
        )}>
            {/* Left side - Info */}
            <div className="flex items-center gap-4">
                {showTotalCount && totalCount !== undefined && (
                    <p className="text-sm text-slate-500 dark:text-slate-400">
                        {t('total')}: <span className="font-medium text-slate-700 dark:text-slate-300">{totalCount}</span>
                    </p>
                )}

                {showPageSize && onPageSizeChange && (
                    <div className="flex items-center gap-2">
                        <label className="text-sm text-slate-500 dark:text-slate-400">
                            {t('show')}:
                        </label>
                        <select
                            value={pageSize}
                            onChange={(e) => onPageSizeChange(Number(e.target.value))}
                            className="px-2 py-1 text-sm rounded-lg border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-800 text-slate-700 dark:text-slate-300 focus:outline-none focus:ring-2 focus:ring-primary-500"
                        >
                            {pageSizeOptions.map(size => (
                                <option key={size} value={size}>{size}</option>
                            ))}
                        </select>
                    </div>
                )}
            </div>

            {/* Right side - Navigation (only show if multiple pages) */}
            {showNavigation && (
                <div className="flex items-center gap-1">
                    {/* First page button */}
                    {!compact && (
                        <button
                            onClick={() => onPageChange(1)}
                            disabled={!canGoPrevious}
                            className={clsx(buttonClasses, activeButtonClasses)}
                            title="First page"
                        >
                            <ChevronsLeft size={18} />
                        </button>
                    )}

                    {/* Previous button */}
                    <button
                        onClick={() => onPageChange(pageNumber - 1)}
                        disabled={!canGoPrevious}
                        className={clsx(buttonClasses, activeButtonClasses)}
                        title="Previous page"
                    >
                        <ChevronLeft size={18} />
                    </button>

                    {/* Page numbers */}
                    <div className="flex items-center gap-1 mx-2">
                        {getPageNumbers().map((page, index) => (
                            page === 'ellipsis' ? (
                                <span key={`ellipsis-${index}`} className="px-2 text-slate-400">
                                    ...
                                </span>
                            ) : (
                                <button
                                    key={page}
                                    onClick={() => onPageChange(page)}
                                    className={clsx(
                                        pageButtonClasses,
                                        page === pageNumber
                                            ? 'bg-primary-500 text-white shadow-sm'
                                            : 'text-slate-600 dark:text-slate-400 hover:bg-slate-100 dark:hover:bg-slate-800'
                                    )}
                                >
                                    {page}
                                </button>
                            )
                        ))}
                    </div>

                    {/* Next button */}
                    <button
                        onClick={() => onPageChange(pageNumber + 1)}
                        disabled={!canGoNext}
                        className={clsx(buttonClasses, activeButtonClasses)}
                        title="Next page"
                    >
                        <ChevronRight size={18} />
                    </button>

                    {/* Last page button */}
                    {!compact && (
                        <button
                            onClick={() => onPageChange(totalPages)}
                            disabled={!canGoNext}
                            className={clsx(buttonClasses, activeButtonClasses)}
                            title="Last page"
                        >
                            <ChevronsRight size={18} />
                        </button>
                    )}
                </div>
            )}
        </div>
    )
}
