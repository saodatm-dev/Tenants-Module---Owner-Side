import { ReactNode } from 'react'
import { useApp } from '../context/AppContext'
import { Plus, Search } from 'lucide-react'

interface PageHeaderProps {
  onAdd?: () => void
  searchValue?: string
  onSearch?: (value: string) => void
  children?: ReactNode
  isShow?: boolean
}

export default function PageHeader({
  onAdd,
  searchValue,
  onSearch,
  children,
  isShow = true,
}: PageHeaderProps) {
  const { t } = useApp()

  if (!isShow) return null

  return (
    <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
      <div className="flex items-center gap-3">
        {onSearch && (
          <div className="relative">
            <Search
              size={18}
              className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400"
            />
            <input
              type="text"
              value={searchValue}
              onChange={e => onSearch(e.target.value)}
              placeholder={t('search')}
              className="input pl-10 w-64"
            />
          </div>
        )}
        {children}
        {onAdd && (
          <button onClick={onAdd} className="btn btn-primary flex items-center gap-2">
            <Plus size={18} />
            <span>{t('add')}</span>
          </button>
        )}
      </div>
    </div>
  )
}
