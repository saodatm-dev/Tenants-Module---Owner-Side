import { useState } from 'react'
import { useApp } from '../context/AppContext'
import { useCrudQuery } from '../hooks/useApi'
import { categoriesService } from '../services/api'
import CrudPage from '../components/CrudPage'
import type { Category } from '../types'

export default function Categories() {
  const { t, language } = useApp()
  const [pagination, setPagination] = useState({ pageNumber: 1, pageSize: 10 })

  const { data, isLoading, create, update, remove, isCreating, isUpdating, isDeleting } =
    useCrudQuery<Category>('categories', categoriesService, pagination)

  const columns = [
    {
      key: 'name',
      header: t('name'),
      render: (item: Category) =>
        (language === 'uz' ? item.nameUz : language === 'ru' ? item.nameRu : item.nameEn) || item.name,
    },
  ]

  const renderForm = (formData: Partial<Category> | null, onChange: (data: Partial<Category>) => void) => (
    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Name (English)</label>
        <input type="text" value={formData?.nameEn || ''} onChange={e => onChange({ ...formData, nameEn: e.target.value })} className="input" />
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Name (Uzbek)</label>
        <input type="text" value={formData?.nameUz || ''} onChange={e => onChange({ ...formData, nameUz: e.target.value })} className="input" />
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Name (Russian)</label>
        <input type="text" value={formData?.nameRu || ''} onChange={e => onChange({ ...formData, nameRu: e.target.value })} className="input" />
      </div>

    </div>
  )

  return (
    <CrudPage data={data} columns={columns} isLoading={isLoading} pagination={pagination}
      onPageChange={page => setPagination(p => ({ ...p, pageNumber: page }))} onCreate={create} onUpdate={update}
      onDelete={remove} renderForm={renderForm} getItemId={item => item.id} getItemName={item => item.nameEn || item.name}
      isCreating={isCreating} isUpdating={isUpdating} isDeleting={isDeleting} />
  )
}
