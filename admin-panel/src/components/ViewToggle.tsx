import { LayoutGrid, List } from 'lucide-react'
import clsx from 'clsx'

export type ViewMode = 'list' | 'card'

interface ViewToggleProps {
    view: ViewMode
    onChange: (view: ViewMode) => void
    className?: string
}

export default function ViewToggle({ view, onChange, className }: ViewToggleProps) {
    return (
        <div className={clsx('inline-flex rounded-lg bg-slate-100 dark:bg-slate-800 p-1', className)}>
            <button
                onClick={() => onChange('list')}
                className={clsx(
                    'flex items-center gap-2 px-3 py-1.5 rounded-md text-sm font-medium transition-all duration-200',
                    view === 'list'
                        ? 'bg-white dark:bg-slate-700 text-primary-600 dark:text-primary-400 shadow-sm'
                        : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-300'
                )}
                aria-pressed={view === 'list'}
                title="List view"
            >
                <List size={16} />
                <span className="hidden sm:inline">List</span>
            </button>
            <button
                onClick={() => onChange('card')}
                className={clsx(
                    'flex items-center gap-2 px-3 py-1.5 rounded-md text-sm font-medium transition-all duration-200',
                    view === 'card'
                        ? 'bg-white dark:bg-slate-700 text-primary-600 dark:text-primary-400 shadow-sm'
                        : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-300'
                )}
                aria-pressed={view === 'card'}
                title="Card view"
            >
                <LayoutGrid size={16} />
                <span className="hidden sm:inline">Cards</span>
            </button>
        </div>
    )
}
