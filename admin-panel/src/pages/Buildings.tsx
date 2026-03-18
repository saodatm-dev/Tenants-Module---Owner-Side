import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useApp } from '../context/AppContext'
import { buildingsService, complexesService, regionsService, districtsService } from '../services/api'
import CrudPage from '../components/CrudPage'
import { showToast } from '../components/common'
import type { Building, BuildingCreate, BuildingUpdate, Complex, Region, District } from '../types'

export default function Buildings() {
  const { t } = useApp()
  const queryClient = useQueryClient()
  const [pagination, setPagination] = useState({ pageNumber: 1, pageSize: 10 })

  // Fetch buildings
  const { data, isLoading } = useQuery({
    queryKey: ['buildings', pagination],
    queryFn: () => buildingsService.getAll(pagination),
  })

  // Fetch complexes for selector
  const { data: complexesData } = useQuery({
    queryKey: ['complexes', { pageNumber: 1, pageSize: 500 }],
    queryFn: () => complexesService.getAll({ pageNumber: 1, pageSize: 500 }),
  })

  // Fetch regions for selector
  const { data: regionsData } = useQuery({
    queryKey: ['regions', { pageNumber: 1, pageSize: 500 }],
    queryFn: () => regionsService.getAll({ pageNumber: 1, pageSize: 500 }),
  })

  // Fetch districts for selector
  const { data: districtsData } = useQuery({
    queryKey: ['districts', { pageNumber: 1, pageSize: 500 }],
    queryFn: () => districtsService.getAll({ pageNumber: 1, pageSize: 500 }),
  })

  // Mutations
  const createMutation = useMutation({
    mutationFn: (data: Partial<Building>) => buildingsService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['buildings'] })
      showToast.success(t('success'), t('item_created'))
    },
    onError: (error: Error) => {
      showToast.error(t('error'), error.message)
    },
  })

  const updateMutation = useMutation({
    mutationFn: (data: Partial<Building>) => buildingsService.update(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['buildings'] })
      showToast.success(t('success'), t('item_updated'))
    },
    onError: (error: Error) => {
      showToast.error(t('error'), error.message)
    },
  })

  const deleteMutation = useMutation({
    mutationFn: (id: string) => buildingsService.remove(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['buildings'] })
      showToast.success(t('success'), t('item_deleted'))
    },
    onError: (error: Error) => {
      showToast.error(t('error'), error.message)
    },
  })

  const columns = [
    { key: 'number', header: t('buildingNumber') || 'Building Number' },
    { key: 'region', header: t('region') || 'Region' },
    { key: 'district', header: t('district') || 'District' },
    {
      key: 'complex',
      header: t('complex') || 'Complex',
      render: (item: Building) => item.complex || '-',
    },
    {
      key: 'buildingType',
      header: t('buildingType') || 'Type',
      render: (item: Building) => {
        const types = []
        if (item.isCommercial) types.push(t('commercial') || 'Commercial')
        if (item.isLiving) types.push(t('living') || 'Living')
        return types.join(', ') || '-'
      },
    },
    {
      key: 'floorsCount',
      header: t('floorsCount') || 'Floors',
      render: (item: Building) => item.floorsCount ?? item.floors?.length ?? '-',
    },
  ]

  const handleCreate = async (formData: Partial<Building>) => {
    const createData: BuildingCreate = {
      number: formData.number || '',
      isCommercial: formData.isCommercial || false,
      isLiving: formData.isLiving || false,
      regionId: formData.regionId || undefined,
      districtId: formData.districtId || undefined,
      complexId: formData.complexId || undefined,
      totalArea: formData.totalArea,
      floorsCount: formData.floorsCount,
      latitude: formData.latitude,
      longitude: formData.longitude,
      address: formData.address,
    }
    return createMutation.mutateAsync(createData as unknown as Partial<Building>)
  }

  const handleUpdate = async (formData: Partial<Building>) => {
    const updateData: BuildingUpdate = {
      id: formData.id || '',
      number: formData.number || '',
      isCommercial: formData.isCommercial || false,
      isLiving: formData.isLiving || false,
      regionId: formData.regionId || undefined,
      districtId: formData.districtId || undefined,
      complexId: formData.complexId || undefined,
      totalSquare: formData.totalArea,
      floorsCount: formData.floorsCount,
      latitude: formData.latitude,
      longitude: formData.longitude,
      address: formData.address,
    }
    return updateMutation.mutateAsync(updateData as unknown as Partial<Building>)
  }

  const handleDelete = async (id: string) => {
    return deleteMutation.mutateAsync(id)
  }

  const renderForm = (
    formData: Partial<Building> | null,
    onChange: (data: Partial<Building>) => void
  ) => (
    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
      {/* Building Number */}
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          {t('buildingNumber') || 'Building Number'} *
        </label>
        <input
          type="text"
          value={formData?.number || ''}
          onChange={e => onChange({ ...formData, number: e.target.value })}
          className="input"
          required
        />
      </div>

      {/* Complex */}
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          {t('complex') || 'Complex'}
        </label>
        <select
          value={formData?.complexId || ''}
          onChange={e => onChange({ ...formData, complexId: e.target.value || undefined })}
          className="input"
        >
          <option value="">{t('selectComplex') || 'Select Complex...'}</option>
          {complexesData?.items.map((complex: Complex) => (
            <option key={complex.id} value={complex.id}>
              {complex.name}
            </option>
          ))}
        </select>
      </div>

      {/* Region */}
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          {t('region') || 'Region'}
        </label>
        <select
          value={formData?.regionId || ''}
          onChange={e => onChange({ ...formData, regionId: e.target.value || undefined })}
          className="input"
        >
          <option value="">{t('selectRegion') || 'Select Region...'}</option>
          {regionsData?.items.map((region: Region) => (
            <option key={region.id} value={region.id}>
              {region.name}
            </option>
          ))}
        </select>
      </div>

      {/* District */}
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          {t('district') || 'District'}
        </label>
        <select
          value={formData?.districtId || ''}
          onChange={e => onChange({ ...formData, districtId: e.target.value || undefined })}
          className="input"
        >
          <option value="">{t('selectDistrict') || 'Select District...'}</option>
          {districtsData?.items.map((district: District) => (
            <option key={district.id} value={district.id}>
              {district.name}
            </option>
          ))}
        </select>
      </div>

      {/* Building Type - Commercial */}
      <div className="flex items-center gap-4">
        <label className="flex items-center gap-2 cursor-pointer">
          <input
            type="checkbox"
            checked={formData?.isCommercial || false}
            onChange={e => onChange({ ...formData, isCommercial: e.target.checked })}
            className="w-4 h-4 rounded border-slate-300 text-blue-600 focus:ring-blue-500"
          />
          <span className="text-sm font-medium text-slate-700 dark:text-slate-300">
            {t('commercial') || 'Commercial'}
          </span>
        </label>
        <label className="flex items-center gap-2 cursor-pointer">
          <input
            type="checkbox"
            checked={formData?.isLiving || false}
            onChange={e => onChange({ ...formData, isLiving: e.target.checked })}
            className="w-4 h-4 rounded border-slate-300 text-blue-600 focus:ring-blue-500"
          />
          <span className="text-sm font-medium text-slate-700 dark:text-slate-300">
            {t('living') || 'Living'}
          </span>
        </label>
      </div>

      {/* Floors Count */}
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          {t('floorsCount') || 'Floors Count'}
        </label>
        <input
          type="number"
          value={formData?.floorsCount || ''}
          onChange={e => onChange({ ...formData, floorsCount: parseInt(e.target.value) || undefined })}
          className="input"
          min="1"
        />
      </div>

      {/* Total Area */}
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          {t('totalArea') || 'Total Area'} (m²)
        </label>
        <input
          type="number"
          value={formData?.totalArea || ''}
          onChange={e => onChange({ ...formData, totalArea: parseFloat(e.target.value) || undefined })}
          className="input"
        />
      </div>

      {/* Address */}
      <div className="md:col-span-2">
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          {t('address') || 'Address'}
        </label>
        <input
          type="text"
          value={formData?.address || ''}
          onChange={e => onChange({ ...formData, address: e.target.value || undefined })}
          className="input"
          placeholder={t('addressPlaceholder') || 'Enter address...'}
        />
      </div>

      {/* Latitude & Longitude */}
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          {t('latitude') || 'Latitude'}
        </label>
        <input
          type="number"
          step="any"
          value={formData?.latitude || ''}
          onChange={e => onChange({ ...formData, latitude: parseFloat(e.target.value) || undefined })}
          className="input"
          placeholder="41.2995"
        />
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          {t('longitude') || 'Longitude'}
        </label>
        <input
          type="number"
          step="any"
          value={formData?.longitude || ''}
          onChange={e => onChange({ ...formData, longitude: parseFloat(e.target.value) || undefined })}
          className="input"
          placeholder="69.2401"
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
      onCreate={handleCreate}
      onUpdate={handleUpdate}
      onDelete={handleDelete}
      onFetchItem={(id) => buildingsService.getById(id)}
      renderForm={renderForm}
      getItemId={item => item.id}
      getItemName={item => item.number}
      isCreating={createMutation.isPending}
      isUpdating={updateMutation.isPending}
      isDeleting={deleteMutation.isPending}
    />
  )
}
