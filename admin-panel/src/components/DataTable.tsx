import { ReactNode } from 'react'
import { useApp } from '../context/AppContext'
import { ChevronLeft, ChevronRight, Edit, Trash2 } from 'lucide-react'
import clsx from 'clsx'

interface Column<T> {
  key: keyof T | string
  header: string
  render?: (item: T) => ReactNode
}

interface DataTableProps<T> {
  data: T[]
  columns: Column<T>[]
  isLoading?: boolean
  pageNumber?: number
  totalPages?: number
  onPageChange?: (page: number) => void
  onEdit?: (item: T) => void
  onDelete?: (item: T) => void
  getId?: (item: T) => string
  onRowClick?: (item: T) => void
}

export default function DataTable<T extends object>({
  data,
  columns,
  isLoading,
  pageNumber = 1,
  totalPages = 1,
  onPageChange,
  onEdit,
  onDelete,
  getId = (item) => (item as { id?: string }).id ?? '',
  onRowClick,
}: DataTableProps<T>) {
  const { t } = useApp()

  if (isLoading) {
    return (
      <div className="card p-8 text-center">
        <div className="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-primary-500 mx-auto"></div>
        <p className="mt-4 text-slate-500">{t('loading')}</p>
      </div>
    )
  }

  if (data.length === 0) {
    return (
      <div className="card p-8 text-center">
        <p className="text-slate-500">{t('noData')}</p>
      </div>
    )
  }

  const showActions = onEdit || onDelete

  return (
    <div className="card overflow-hidden">
      <div className="overflow-x-auto">
        <table className="w-full">
          <thead className="bg-slate-50 dark:bg-slate-800/50">
            <tr>
              {columns.map(col => (
                <th
                  key={String(col.key)}
                  className="px-4 py-3 text-left text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider"
                >
                  {col.header}
                </th>
              ))}
              {showActions && (
                <th className="px-4 py-3 text-right text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">
                  {t('actions')}
                </th>
              )}
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-200 dark:divide-slate-800">
            {data.map(item => (
              <tr
                key={getId(item)}
                className={`hover:bg-slate-50 dark:hover:bg-slate-800/30 transition-colors${onRowClick ? ' cursor-pointer' : ''}`}
                onClick={() => onRowClick?.(item)}
              >
                {columns.map(col => (
                  <td
                    key={`${getId(item)}-${String(col.key)}`}
                    className="px-4 py-3 text-sm text-slate-700 dark:text-slate-300"
                  >
                    {col.render
                      ? col.render(item)
                      : String(item[col.key as keyof T] ?? '')}
                  </td>
                ))}
                {showActions && (
                  <td className="px-4 py-3 text-right">
                    <div className="flex items-center justify-end gap-2">
                      {onEdit && (
                        <button
                          onClick={() => onEdit(item)}
                          className="p-1.5 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-500 hover:text-primary-500"
                        >
                          <Edit size={16} />
                        </button>
                      )}
                      {onDelete && (
                        <button
                          onClick={() => onDelete(item)}
                          className="p-1.5 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-500 hover:text-red-500"
                        >
                          <Trash2 size={16} />
                        </button>
                      )}
                    </div>
                  </td>
                )}
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {totalPages > 1 && onPageChange && (
        <div className="px-4 py-3 border-t border-slate-200 dark:border-slate-800 flex items-center justify-between">
          <p className="text-sm text-slate-500">
            Page {pageNumber} of {totalPages}
          </p>
          <div className="flex items-center gap-2">
            <button
              onClick={() => onPageChange(pageNumber - 1)}
              disabled={pageNumber <= 1}
              className={clsx(
                'p-2 rounded-lg',
                pageNumber <= 1
                  ? 'text-slate-300 dark:text-slate-600 cursor-not-allowed'
                  : 'text-slate-600 dark:text-slate-400 hover:bg-slate-100 dark:hover:bg-slate-800'
              )}
            >
              <ChevronLeft size={20} />
            </button>
            <button
              onClick={() => onPageChange(pageNumber + 1)}
              disabled={pageNumber >= totalPages}
              className={clsx(
                'p-2 rounded-lg',
                pageNumber >= totalPages
                  ? 'text-slate-300 dark:text-slate-600 cursor-not-allowed'
                  : 'text-slate-600 dark:text-slate-400 hover:bg-slate-100 dark:hover:bg-slate-800'
              )}
            >
              <ChevronRight size={20} />
            </button>
          </div>
        </div>
      )}
    </div>
  )
}
