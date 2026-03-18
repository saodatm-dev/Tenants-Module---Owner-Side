import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { useApp } from '../context/AppContext'
import { useCrudQuery } from '../hooks/useApi'
import { complexesService, districtsService } from '../services/api'
import CrudPage from '../components/CrudPage'
import type { Complex, District } from '../types'

export default function Complexes() {
  const { t, language } = useApp()
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
  } = useCrudQuery<Complex>('complexes', complexesService, pagination)

  const { data: districtsData } = useQuery({
    queryKey: ['districts', { pageNumber: 1, pageSize: 500 }],
    queryFn: () => districtsService.getAll({ pageNumber: 1, pageSize: 500 }),
  })

  const getDistrictName = (districtId: string) => {
    const district = districtsData?.items.find((d: District) => d.id === districtId)
    if (!district) return districtId
    return language === 'uz' ? district.nameUz : language === 'ru' ? district.nameRu : district.nameEn
  }

  const columns = [
    { key: 'name', header: t('name') },
    {
      key: 'districtId',
      header: t('districts'),
      render: (item: Complex) => getDistrictName(item.districtId || ''),
    },
    { key: 'address', header: 'Address' },
  ]

  const renderForm = (
    formData: Partial<Complex> | null,
    onChange: (data: Partial<Complex>) => void
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
          {t('districts')}
        </label>
        <select
          value={formData?.districtId || ''}
          onChange={e => onChange({ ...formData, districtId: e.target.value })}
          className="input"
        >
          <option value="">Select District</option>
          {districtsData?.items.map((district: District) => (
            <option key={district.id} value={district.id}>
              {language === 'uz' ? district.nameUz : language === 'ru' ? district.nameRu : district.nameEn}
            </option>
          ))}
        </select>
      </div>
      <div className="md:col-span-2">
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          Address
        </label>
        <input
          type="text"
          value={formData?.address || ''}
          onChange={e => onChange({ ...formData, address: e.target.value })}
          className="input"
        />
      </div>
      <div className="md:col-span-2">
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          Description
        </label>
        <textarea
          value={formData?.description || ''}
          onChange={e => onChange({ ...formData, description: e.target.value })}
          className="input"
          rows={3}
        />
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          Latitude
        </label>
        <input
          type="number"
          step="any"
          value={formData?.latitude || ''}
          onChange={e => onChange({ ...formData, latitude: parseFloat(e.target.value) })}
          className="input"
        />
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          Longitude
        </label>
        <input
          type="number"
          step="any"
          value={formData?.longitude || ''}
          onChange={e => onChange({ ...formData, longitude: parseFloat(e.target.value) })}
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
