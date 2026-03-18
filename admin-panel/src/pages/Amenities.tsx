import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { useApp } from '../context/AppContext'
import { useCrudQuery } from '../hooks/useApi'
import { amenitiesService, amenityCategoriesService } from '../services/api'
import CrudPage from '../components/CrudPage'
import type { Amenity, AmenityCategory } from '../types'

export default function Amenities() {
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
  } = useCrudQuery<Amenity>('amenities', amenitiesService, pagination)

  const { data: categoriesData } = useQuery({
    queryKey: ['amenity-categories', { pageNumber: 1, pageSize: 100 }],
    queryFn: () => amenityCategoriesService.getAll({ pageNumber: 1, pageSize: 100 }),
  })

  const getCategoryName = (categoryId: string) => {
    const category = categoriesData?.items.find((c: AmenityCategory) => c.id === categoryId)
    if (!category) return categoryId
    return (language === 'uz' ? category.nameUz : language === 'ru' ? category.nameRu : category.nameEn) || category.name
  }

  const columns = [
    {
      key: 'name',
      header: t('name'),
      render: (item: Amenity) =>
        (language === 'uz' ? item.nameUz : language === 'ru' ? item.nameRu : item.nameEn) || item.name,
    },
    {
      key: 'categoryId',
      header: 'Category',
      render: (item: Amenity) => getCategoryName(item.categoryId || ''),
    },
    {
      key: 'icon',
      header: 'Icon',
      render: (item: Amenity) => item.iconUrl ? (
        <img src={item.iconUrl} alt="" className="w-6 h-6" />
      ) : null,
    },
  ]

  const renderForm = (
    formData: Partial<Amenity> | null,
    onChange: (data: Partial<Amenity>) => void
  ) => (
    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          Name (English)
        </label>
        <input
          type="text"
          value={formData?.nameEn || ''}
          onChange={e => onChange({ ...formData, nameEn: e.target.value })}
          className="input"
        />
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          Name (Uzbek)
        </label>
        <input
          type="text"
          value={formData?.nameUz || ''}
          onChange={e => onChange({ ...formData, nameUz: e.target.value })}
          className="input"
        />
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          Name (Russian)
        </label>
        <input
          type="text"
          value={formData?.nameRu || ''}
          onChange={e => onChange({ ...formData, nameRu: e.target.value })}
          className="input"
        />
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          Category
        </label>
        <select
          value={formData?.categoryId || ''}
          onChange={e => onChange({ ...formData, categoryId: e.target.value })}
          className="input"
        >
          <option value="">Select Category</option>
          {categoriesData?.items.map((category: AmenityCategory) => (
            <option key={category.id} value={category.id}>
              {(language === 'uz' ? category.nameUz : language === 'ru' ? category.nameRu : category.nameEn) || category.name}
            </option>
          ))}
        </select>
      </div>
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          Icon
        </label>
        <input
          type="text"
          value={formData?.icon || ''}
          onChange={e => onChange({ ...formData, icon: e.target.value })}
          className="input"
          placeholder="Icon name or URL"
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
      getItemName={item => item.nameEn || item.name}
      isCreating={isCreating}
      isUpdating={isUpdating}
      isDeleting={isDeleting}
    />
  )
}
