import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useApp } from '../context/AppContext'
import { listingCategoriesService, languagesService } from '../services/api'
import CrudPage from '../components/CrudPage'
import { showToast } from '../components/common'
import type { ListingCategory, Language, BuildingType } from '../types'

interface LanguageValueInput {
  languageId: string
  languageShortCode: string
  value: string
}

interface ListingCategoryCreate {
  buildingType: number
  iconUrl: string
  translates: LanguageValueInput[]
  parentId?: string
}

interface ListingCategoryUpdate {
  id: string
  parentId?: string
  buildingType: number
  iconUrl: string
  translates: LanguageValueInput[]
}

export default function ListingCategories() {
  const { t } = useApp()
  const queryClient = useQueryClient()
  const [pagination, setPagination] = useState({ pageNumber: 1, pageSize: 10 })

  // Fetch listing categories
  const { data, isLoading } = useQuery({
    queryKey: ['listing-categories', pagination],
    queryFn: () => listingCategoriesService.getAll(pagination),
  })

  // Fetch languages for LanguageValue creation
  const { data: languagesData } = useQuery({
    queryKey: ['languages', { pageNumber: 1, pageSize: 100 }],
    queryFn: () => languagesService.getAll({ pageNumber: 1, pageSize: 100 }),
  })

  // Fetch parent categories for selector
  const { data: parentCategoriesData } = useQuery({
    queryKey: ['listing-categories-all', { pageNumber: 1, pageSize: 500 }],
    queryFn: () => listingCategoriesService.getAll({ pageNumber: 1, pageSize: 500 }),
  })

  // Helper to get language by shortCode
  const getLanguageByCode = (code: string): Language | undefined => {
    return languagesData?.items.find((lang: Language) => lang.shortCode === code)
  }

  // Helper to create LanguageValue array from form data
  const createLanguageValues = (
    nameEn?: string,
    nameUz?: string,
    nameRu?: string
  ): LanguageValueInput[] => {
    const values: LanguageValueInput[] = []

    const enLang = getLanguageByCode('en')
    const uzLang = getLanguageByCode('uz')
    const ruLang = getLanguageByCode('ru')

    if (nameEn && enLang) {
      values.push({ languageId: enLang.id, languageShortCode: 'en', value: nameEn })
    }
    if (nameUz && uzLang) {
      values.push({ languageId: uzLang.id, languageShortCode: 'uz', value: nameUz })
    }
    if (nameRu && ruLang) {
      values.push({ languageId: ruLang.id, languageShortCode: 'ru', value: nameRu })
    }

    return values
  }

  // Mutations
  const createMutation = useMutation({
    mutationFn: (data: Partial<ListingCategory>) => listingCategoriesService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['listing-categories'] })
      showToast.success(t('success'), t('item_created'))
    },
    onError: (error: Error) => {
      showToast.error(t('error'), error.message)
    },
  })

  const updateMutation = useMutation({
    mutationFn: (data: Partial<ListingCategory>) => listingCategoriesService.update(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['listing-categories'] })
      showToast.success(t('success'), t('item_updated'))
    },
    onError: (error: Error) => {
      showToast.error(t('error'), error.message)
    },
  })

  const deleteMutation = useMutation({
    mutationFn: (id: string) => listingCategoriesService.remove(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['listing-categories'] })
      showToast.success(t('success'), t('item_deleted'))
    },
    onError: (error: Error) => {
      showToast.error(t('error'), error.message)
    },
  })

  // Get building type label
  const getBuildingTypeLabel = (type: BuildingType | number): string => {
    const typeValue = typeof type === 'number' ? type : type
    switch (typeValue) {
      case 0:
      case 'Commercial':
        return t('commercial') || 'Commercial'
      case 1:
      case 'Living':
        return t('living') || 'Living'
      default:
        return String(type)
    }
  }

  // Get parent category name
  const getParentCategoryName = (parentId?: string): string => {
    if (!parentId) return '-'
    const parent = parentCategoriesData?.items.find((c: ListingCategory) => c.id === parentId)
    return parent?.name || parentId
  }

  const columns = [
    { key: 'name', header: t('name') || 'Name' },
    {
      key: 'buildingType',
      header: t('buildingType') || 'Building Type',
      render: (item: ListingCategory) => getBuildingTypeLabel(item.buildingType),
    },
    {
      key: 'parentId',
      header: t('parentCategory') || 'Parent Category',
      render: (item: ListingCategory) => getParentCategoryName(item.parentId),
    },
    {
      key: 'iconUrl',
      header: t('icon') || 'Icon',
      render: (item: ListingCategory) => item.iconUrl ? (
        <img src={item.iconUrl} alt="" className="w-6 h-6 object-contain" />
      ) : '-',
    },
  ]

  type FormData = Partial<ListingCategory & {
    nameEn?: string
    nameUz?: string
    nameRu?: string
  }>

  const handleCreate = async (formData: FormData) => {
    const createData: ListingCategoryCreate = {
      buildingType: typeof formData.buildingType === 'number' ? formData.buildingType : 0,
      iconUrl: formData.iconUrl || '',
      translates: createLanguageValues(formData.nameEn, formData.nameUz, formData.nameRu),
      parentId: formData.parentId || undefined,
    }
    return createMutation.mutateAsync(createData as unknown as Partial<ListingCategory>)
  }

  const handleUpdate = async (formData: FormData) => {
    const updateData: ListingCategoryUpdate = {
      id: formData.id || '',
      parentId: formData.parentId || undefined,
      buildingType: typeof formData.buildingType === 'number' ? formData.buildingType : 0,
      iconUrl: formData.iconUrl || '',
      translates: createLanguageValues(formData.nameEn, formData.nameUz, formData.nameRu),
    }
    return updateMutation.mutateAsync(updateData as unknown as Partial<ListingCategory>)
  }

  const handleDelete = async (id: string) => {
    return deleteMutation.mutateAsync(id)
  }

  const renderForm = (
    formData: FormData | null,
    onChange: (data: FormData) => void
  ) => (
    <div className="space-y-6">
      {/* Names */}
      <div>
        <h4 className="text-sm font-semibold text-slate-700 dark:text-slate-300 mb-3">
          {t('name') || 'Name'}
        </h4>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div>
            <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
              🇺🇸 English
            </label>
            <input
              type="text"
              value={formData?.nameEn || ''}
              onChange={e => onChange({ ...formData, nameEn: e.target.value })}
              className="input"
              placeholder="Name in English"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
              🇺🇿 O'zbekcha
            </label>
            <input
              type="text"
              value={formData?.nameUz || ''}
              onChange={e => onChange({ ...formData, nameUz: e.target.value })}
              className="input"
              placeholder="Nomi o'zbekcha"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
              🇷🇺 Русский
            </label>
            <input
              type="text"
              value={formData?.nameRu || ''}
              onChange={e => onChange({ ...formData, nameRu: e.target.value })}
              className="input"
              placeholder="Название на русском"
            />
          </div>
        </div>
      </div>

      {/* Building Type & Parent Category */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
            {t('buildingType') || 'Building Type'} *
          </label>
          <select
            value={formData?.buildingType ?? 0}
            onChange={e => onChange({ ...formData, buildingType: parseInt(e.target.value) as unknown as BuildingType })}
            className="input"
          >
            <option value={0}>{t('commercial') || 'Commercial'}</option>
            <option value={1}>{t('living') || 'Living'}</option>
          </select>
        </div>
        <div>
          <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
            {t('parentCategory') || 'Parent Category'}
          </label>
          <select
            value={formData?.parentId || ''}
            onChange={e => onChange({ ...formData, parentId: e.target.value || undefined })}
            className="input"
          >
            <option value="">{t('noParent') || 'No Parent (Root Category)'}</option>
            {parentCategoriesData?.items
              .filter((c: ListingCategory) => c.id !== formData?.id) // Exclude self
              .map((category: ListingCategory) => (
                <option key={category.id} value={category.id}>
                  {category.name}
                </option>
              ))}
          </select>
        </div>
      </div>

      {/* Icon URL */}
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          {t('iconUrl') || 'Icon URL'} *
        </label>
        <input
          type="text"
          value={formData?.iconUrl || ''}
          onChange={e => onChange({ ...formData, iconUrl: e.target.value })}
          className="input"
          placeholder="https://example.com/icon.png"
        />
        {formData?.iconUrl && (
          <div className="mt-2 flex items-center gap-2">
            <span className="text-sm text-slate-500">{t('preview') || 'Preview'}:</span>
            <img src={formData.iconUrl} alt="Icon preview" className="w-8 h-8 object-contain" />
          </div>
        )}
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
      renderForm={renderForm}
      getItemId={item => item.id}
      getItemName={item => item.name}
      isCreating={createMutation.isPending}
      isUpdating={updateMutation.isPending}
      isDeleting={deleteMutation.isPending}
    />
  )
}
