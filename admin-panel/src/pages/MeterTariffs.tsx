import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { useApp } from '../context/AppContext'
import { useCrudQuery } from '../hooks/useApi'
import { meterTariffsService, meterTypesService } from '../services/api'
import CrudPage from '../components/CrudPage'
import type { MeterTariff, MeterType } from '../types'

export default function MeterTariffs() {
  const { t } = useApp()
  const [pagination, setPagination] = useState({ pageNumber: 1, pageSize: 10 })

  const { data, isLoading, create, update, remove, isCreating, isUpdating, isDeleting } =
    useCrudQuery<MeterTariff>('meter-tariffs', meterTariffsService, pagination)

  const { data: meterTypesData } = useQuery({
    queryKey: ['meter-types', { pageNumber: 1, pageSize: 100 }],
    queryFn: () => meterTypesService.getAll({ pageNumber: 1, pageSize: 100 }),
  })

  const getMeterTypeName = (meterTypeId: string) => {
    const meterType = meterTypesData?.items.find((m: MeterType) => m.id === meterTypeId)
    return meterType?.name || meterTypeId
  }

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('en-US').format(price)
  }

  const columns = [
    {
      key: 'meterTypeId',
      header: t('meterTypes') || 'Meter Type',
      render: (item: MeterTariff) => getMeterTypeName(item.meterTypeId),
    },
    {
      key: 'type',
      header: t('type') || 'Type',
    },
    {
      key: 'price',
      header: t('price') || 'Price',
      render: (item: MeterTariff) => formatPrice(item.price),
    },
    {
      key: 'fixedPrice',
      header: t('fixedPrice') || 'Fixed Price',
      render: (item: MeterTariff) => item.fixedPrice ? formatPrice(item.fixedPrice) : '-',
    },
  ]

  const renderForm = (formData: Partial<MeterTariff> | null, onChange: (data: Partial<MeterTariff>) => void) => (
    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">{t('meterTypes') || 'Meter Type'}</label>
        <select value={formData?.meterTypeId || ''} onChange={e => onChange({ ...formData, meterTypeId: e.target.value })} className="input">
          <option value="">Select Meter Type</option>
          {meterTypesData?.items.map((mt: MeterType) => (
            <option key={mt.id} value={mt.id}>
              {mt.name}
            </option>
          ))}
        </select>
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">{t('type') || 'Type'}</label>
        <input type="number" value={formData?.type ?? ''} onChange={e => onChange({ ...formData, type: parseInt(e.target.value) || 0 })} className="input" placeholder="0=Individual, 1=Legal, 2=Budget" />
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">{t('price') || 'Price'}</label>
        <input type="number" value={formData?.price || ''} onChange={e => onChange({ ...formData, price: parseInt(e.target.value) || 0 })} className="input" />
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">{t('fixedPrice') || 'Fixed Price'} ({t('optional') || 'Optional'})</label>
        <input type="number" value={formData?.fixedPrice || ''} onChange={e => onChange({ ...formData, fixedPrice: e.target.value ? parseInt(e.target.value) : undefined })} className="input" />
      </div>
    </div>
  )

  return (
    <CrudPage data={data} columns={columns} isLoading={isLoading} pagination={pagination}
      onPageChange={page => setPagination(p => ({ ...p, pageNumber: page }))} onCreate={create} onUpdate={update}
      onDelete={remove} renderForm={renderForm} getItemId={item => item.id} getItemName={item => `${getMeterTypeName(item.meterTypeId)} - ${item.type}`}
      isCreating={isCreating} isUpdating={isUpdating} isDeleting={isDeleting} />
  )
}
