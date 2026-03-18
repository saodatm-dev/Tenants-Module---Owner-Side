import { useState } from 'react'
import { useApp } from '../context/AppContext'
import { useCrudQuery } from '../hooks/useApi'
import { regionsService } from '../services/api'
import CrudPage from '../components/CrudPage'
import type { Region } from '../types'

export default function Regions() {
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
  } = useCrudQuery<Region>('regions', regionsService, pagination)

  const columns = [
    { key: 'name', header: t('name') },
    { key: 'order', header: t('order') },
  ]

  const renderForm = (
    formData: Partial<Region> | null,
    onChange: (data: Partial<Region>) => void
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
          {t('order')}
        </label>
        <input
          type="number"
          value={formData?.order || 0}
          onChange={e => onChange({ ...formData, order: parseInt(e.target.value) || 0 })}
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
      onPageSizeChange={size => setPagination(p => ({ ...p, pageSize: size, pageNumber: 1 }))}
      onCreate={create}
      onUpdate={update}
      onDelete={remove}
      renderForm={renderForm}
      getItemId={item => item.id}
      getItemName={item => item.name}
      isCreating={isCreating}
      isUpdating={isUpdating}
      isDeleting={isDeleting}
      showViewToggle={true}
      defaultView="list"
    />
  )
}
