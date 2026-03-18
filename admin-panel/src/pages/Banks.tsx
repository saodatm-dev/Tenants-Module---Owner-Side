import { useState } from 'react'
import { useApp } from '../context/AppContext'
import { useCrudQuery } from '../hooks/useApi'
import { banksService } from '../services/api'
import CrudPage from '../components/CrudPage'
import type { Bank } from '../types'

export default function Banks() {
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
  } = useCrudQuery<Bank>('banks', banksService, pagination)

  const columns = [
    { key: 'name', header: t('name') },
  ]

  const renderForm = (
    formData: Partial<Bank> | null,
    onChange: (data: Partial<Bank>) => void
  ) => (
    <div className="grid grid-cols-1 gap-4">
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
