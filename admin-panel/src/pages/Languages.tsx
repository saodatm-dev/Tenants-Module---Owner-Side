import { useState } from 'react'
import { useApp } from '../context/AppContext'
import { useCrudQuery } from '../hooks/useApi'
import { languagesService } from '../services/api'
import CrudPage from '../components/CrudPage'
import type { Language } from '../types'

export default function LanguagesPage() {
  const { t } = useApp()
  const [pagination, setPagination] = useState({ pageNumber: 1, pageSize: 10 })

  const {
    data,
    isLoading,
    create,
    update,
    remove,
    isCreating,
    isUpdating,
    isDeleting,
  } = useCrudQuery<Language>('languages', languagesService, pagination)

  const columns = [
    { key: 'name', header: t('name') },
    { key: 'code', header: t('code') },
    { key: 'nativeName', header: 'Native Name' },
  ]

  const renderForm = (
    formData: Partial<Language> | null,
    onChange: (data: Partial<Language>) => void
  ) => (
    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          {t('name')}
        </label>
        <input
          type="text"
          value={formData?.name || ''}
          onChange={e => onChange({ ...formData, name: e.target.value })}
          className="input"
        />
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          {t('code')}
        </label>
        <input
          type="text"
          value={formData?.code || ''}
          onChange={e => onChange({ ...formData, code: e.target.value })}
          className="input"
          maxLength={5}
        />
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          Native Name
        </label>
        <input
          type="text"
          value={formData?.nativeName || ''}
          onChange={e => onChange({ ...formData, nativeName: e.target.value })}
          className="input"
        />
      </div>
    </div>
  )

  return (
    <CrudPage
      
      data={data}
      columns={columns}
      isLoading={isLoading}
      pagination={pagination}
      onPageChange={page => setPagination(p => ({ ...p, pageNumber: page }))}
      onCreate={create}
      onUpdate={update}
      onDelete={remove}
      renderForm={renderForm}
      getItemId={item => item.id}
      getItemName={item => item.name}
      isCreating={isCreating}
      isUpdating={isUpdating}
      isDeleting={isDeleting}
    />
  )
}
