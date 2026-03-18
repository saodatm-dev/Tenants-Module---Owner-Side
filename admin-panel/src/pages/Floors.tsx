import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useApp } from '../context/AppContext'
import { floorsService, buildingsService } from '../services/api'
import CrudPage from '../components/CrudPage'
import { showToast } from '../components/common'
import type { Floor, Building, FloorCreate, FloorUpdate } from '../types'

export default function Floors() {
  const { t } = useApp()
  const queryClient = useQueryClient()
  const [pagination, setPagination] = useState({ pageNumber: 1, pageSize: 10 })
  const [selectedBuildingId, setSelectedBuildingId] = useState<string>('')

  // Fetch buildings for selector
  const { data: buildingsData, isLoading: buildingsLoading } = useQuery({
    queryKey: ['buildings', { pageNumber: 1, pageSize: 500 }],
    queryFn: () => buildingsService.getAll({ pageNumber: 1, pageSize: 500 }),
  })

  // Fetch floors for selected building
  const { data: floorsData, isLoading: floorsLoading } = useQuery({
    queryKey: ['floors', selectedBuildingId, pagination],
    queryFn: () => floorsService.getAll({ ...pagination, buildingId: selectedBuildingId }),
    enabled: !!selectedBuildingId,
  })

  // Mutations
  const createMutation = useMutation({
    mutationFn: (data: Partial<Floor>) => floorsService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['floors'] })
      showToast.success(t('success'), t('item_created'))
    },
    onError: (error: Error) => {
      showToast.error(t('error'), error.message)
    },
  })

  const updateMutation = useMutation({
    mutationFn: (data: Partial<Floor>) => floorsService.update(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['floors'] })
      showToast.success(t('success'), t('item_updated'))
    },
    onError: (error: Error) => {
      showToast.error(t('error'), error.message)
    },
  })

  const deleteMutation = useMutation({
    mutationFn: (id: string) => floorsService.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['floors'] })
      showToast.success(t('success'), t('item_deleted'))
    },
    onError: (error: Error) => {
      showToast.error(t('error'), error.message)
    },
  })

  const columns = [
    { key: 'number', header: t('floorNumber') },
    { key: 'buildingNumber', header: t('building') },
    {
      key: 'buildingTypes',
      header: t('buildingTypes'),
      render: (item: Floor) =>
        item.buildingTypes?.join(', ') || '-',
    },
    {
      key: 'totalArea',
      header: t('totalArea'),
      render: (item: Floor) =>
        item.totalArea ? `${item.totalArea} m²` : '-',
    },
    {
      key: 'ceilingHeight',
      header: t('ceilingHeight'),
      render: (item: Floor) =>
        item.ceilingHeight ? `${item.ceilingHeight} m` : '-',
    },
  ]

  const handleCreate = async (formData: Partial<Floor & { plan?: string }>) => {
    const createData: FloorCreate = {
      buildingId: selectedBuildingId,
      number: formData.number || 0,
      totalArea: formData.totalArea,
      ceilingHeight: formData.ceilingHeight,
      plan: formData.plan,
    }
    return createMutation.mutateAsync(createData as unknown as Partial<Floor>)
  }

  const handleUpdate = async (formData: Partial<Floor & { floorPlan?: string }>) => {
    const updateData: FloorUpdate = {
      id: formData.id || '',
      buildingId: selectedBuildingId,
      number: formData.number || 0,
      totalArea: formData.totalArea,
      ceilingHeight: formData.ceilingHeight,
      floorPlan: formData.floorPlan,
    }
    return updateMutation.mutateAsync(updateData as unknown as Partial<Floor>)
  }

  const handleDelete = async (id: string) => {
    return deleteMutation.mutateAsync(id)
  }

  const renderForm = (
    formData: Partial<Floor & { plan?: string; note?: string; floorPlan?: string }> | null,
    onChange: (data: Partial<Floor & { plan?: string; note?: string; floorPlan?: string }>) => void
  ) => (
    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          {t('floorNumber')} *
        </label>
        <input
          type="number"
          value={formData?.number || ''}
          onChange={e => onChange({ ...formData, number: parseInt(e.target.value) })}
          className="input"
          required
        />
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          {t('totalArea')} (m²)
        </label>
        <input
          type="number"
          step="0.01"
          value={formData?.totalArea || ''}
          onChange={e => onChange({ ...formData, totalArea: parseFloat(e.target.value) || undefined })}
          className="input"
        />
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          {t('ceilingHeight')} (m)
        </label>
        <input
          type="number"
          step="0.01"
          value={formData?.ceilingHeight || ''}
          onChange={e => onChange({ ...formData, ceilingHeight: parseFloat(e.target.value) || undefined })}
          className="input"
        />
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          {t('floorPlan')}
        </label>
        <input
          type="text"
          value={formData?.id ? (formData?.floorPlan || '') : (formData?.plan || '')}
          onChange={e => onChange({
            ...formData,
            ...(formData?.id ? { floorPlan: e.target.value || undefined } : { plan: e.target.value || undefined })
          })}
          className="input"
          placeholder={t('floorPlanPlaceholder')}
        />
      </div>
      <div className="md:col-span-2">
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          {t('note')}
        </label>
        <textarea
          value={formData?.note || ''}
          onChange={e => onChange({ ...formData, note: e.target.value || undefined })}
          className="input min-h-[80px] resize-y"
          placeholder={t('notePlaceholder')}
          rows={3}
        />
      </div>
    </div>
  )

  return (
    <div className="space-y-4">
      {/* Building Selector */}
      <div className="card p-4">
        <div className="flex items-center gap-4">
          <label className="text-sm font-medium text-slate-700 dark:text-slate-300">
            {t('selectBuilding')}:
          </label>
          <select
            value={selectedBuildingId}
            onChange={e => {
              setSelectedBuildingId(e.target.value)
              setPagination(p => ({ ...p, pageNumber: 1 }))
            }}
            className="input max-w-md"
            disabled={buildingsLoading}
          >
            <option value="">{t('selectBuilding')}...</option>
            {buildingsData?.items.map((building: Building) => (
              <option key={building.id} value={building.id}>
                {building.number} - {building.region}, {building.district}
              </option>
            ))}
          </select>
        </div>
      </div>

      {/* Floors List */}
      {selectedBuildingId ? (
        <CrudPage
          data={floorsData}
          columns={columns}
          isLoading={floorsLoading}
          pagination={pagination}
          onPageChange={page => setPagination(p => ({ ...p, pageNumber: page }))}
          onPageSizeChange={size => setPagination(p => ({ ...p, pageSize: size, pageNumber: 1 }))}
          onCreate={handleCreate}
          onUpdate={handleUpdate}
          onDelete={handleDelete}
          renderForm={renderForm}
          getItemId={item => item.id}
          getItemName={item => `Floor ${item.number}`}
          isCreating={createMutation.isPending}
          isUpdating={updateMutation.isPending}
          isDeleting={deleteMutation.isPending}
          showViewToggle={true}
          defaultView="list"
        />
      ) : (
        <div className="card p-8 text-center">
          <p className="text-slate-500 dark:text-slate-400">
            {t('selectBuildingFirst')}
          </p>
        </div>
      )}
    </div>
  )
}
