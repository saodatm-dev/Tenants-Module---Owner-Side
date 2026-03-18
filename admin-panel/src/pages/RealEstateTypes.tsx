import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useApp } from '../context/AppContext'
import { realEstateTypesService, languagesService } from '../services/api'
import CrudPage from '../components/CrudPage'
import { showToast } from '../components/common'
import type { RealEstateType, Language } from '../types'

interface LanguageValueInput {
  languageId: string
  languageShortCode: string
  value: string
}

interface RealEstateTypeCreate {
  names: LanguageValueInput[]
  descriptions: LanguageValueInput[]
  iconUrl?: string
  showBuildingSuggestion?: boolean
  showFloorSuggestion?: boolean
  canHaveUnits?: boolean
  canHaveMeters?: boolean
}

interface RealEstateTypeUpdate {
  id: string
  names: LanguageValueInput[]
  descriptions: LanguageValueInput[]
  iconUrl?: string
  showBuildingSuggestion?: boolean
  showFloorSuggestion?: boolean
  canHaveUnits?: boolean
  canHaveMeters?: boolean
}

export default function RealEstateTypes() {
  const { t } = useApp()
  const queryClient = useQueryClient()
  const [pagination, setPagination] = useState({ pageNumber: 1, pageSize: 10 })

  // Fetch real estate types
  const { data, isLoading } = useQuery({
    queryKey: ['real-estate-types', pagination],
    queryFn: () => realEstateTypesService.getAll(pagination),
  })

  // Fetch languages for LanguageValue creation
  const { data: languagesData } = useQuery({
    queryKey: ['languages', { pageNumber: 1, pageSize: 100 }],
    queryFn: () => languagesService.getAll({ pageNumber: 1, pageSize: 100 }),
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
    mutationFn: (data: Partial<RealEstateType>) => realEstateTypesService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['real-estate-types'] })
      showToast.success(t('success'), t('item_created'))
    },
    onError: (error: Error) => {
      showToast.error(t('error'), error.message)
    },
  })

  const updateMutation = useMutation({
    mutationFn: (data: Partial<RealEstateType>) => realEstateTypesService.update(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['real-estate-types'] })
      showToast.success(t('success'), t('item_updated'))
    },
    onError: (error: Error) => {
      showToast.error(t('error'), error.message)
    },
  })

  const deleteMutation = useMutation({
    mutationFn: (id: string) => realEstateTypesService.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['real-estate-types'] })
      showToast.success(t('success'), t('item_deleted'))
    },
    onError: (error: Error) => {
      showToast.error(t('error'), error.message)
    },
  })

  const columns = [
    { key: 'name', header: t('name') || 'Name' },
    { key: 'description', header: t('description') || 'Description' },
    {
      key: 'showBuildingSuggestion',
      header: t('showBuildingSuggestion') || 'Building Suggestion',
      render: (item: RealEstateType) => item.showBuildingSuggestion ? '✓' : '-',
    },
    {
      key: 'showFloorSuggestion',
      header: t('showFloorSuggestion') || 'Floor Suggestion',
      render: (item: RealEstateType) => item.showFloorSuggestion ? '✓' : '-',
    },
    {
      key: 'canHaveUnits',
      header: t('canHaveUnits') || 'Has Units',
      render: (item: RealEstateType) => item.canHaveUnits ? '✓' : '-',
    },
    {
      key: 'canHaveMeters',
      header: t('canHaveMeters') || 'Has Meters',
      render: (item: RealEstateType) => item.canHaveMeters ? '✓' : '-',
    },
  ]

  type FormData = Partial<RealEstateType & {
    nameEn?: string; nameUz?: string; nameRu?: string;
    descriptionEn?: string; descriptionUz?: string; descriptionRu?: string
  }>

  const handleCreate = async (formData: FormData) => {
    const createData: RealEstateTypeCreate = {
      names: createLanguageValues(formData.nameEn, formData.nameUz, formData.nameRu),
      descriptions: createLanguageValues(formData.descriptionEn, formData.descriptionUz, formData.descriptionRu),
      iconUrl: formData.icon,
      showBuildingSuggestion: formData.showBuildingSuggestion || false,
      showFloorSuggestion: formData.showFloorSuggestion || false,
      canHaveUnits: formData.canHaveUnits || false,
      canHaveMeters: formData.canHaveMeters || false,
    }
    return createMutation.mutateAsync(createData as unknown as Partial<RealEstateType>)
  }

  const handleUpdate = async (formData: FormData) => {
    const updateData: RealEstateTypeUpdate = {
      id: formData.id || '',
      names: createLanguageValues(formData.nameEn, formData.nameUz, formData.nameRu),
      descriptions: createLanguageValues(formData.descriptionEn, formData.descriptionUz, formData.descriptionRu),
      iconUrl: formData.icon,
      showBuildingSuggestion: formData.showBuildingSuggestion || false,
      showFloorSuggestion: formData.showFloorSuggestion || false,
      canHaveUnits: formData.canHaveUnits || false,
      canHaveMeters: formData.canHaveMeters || false,
    }
    return updateMutation.mutateAsync(updateData as unknown as Partial<RealEstateType>)
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

      {/* Descriptions */}
      <div>
        <h4 className="text-sm font-semibold text-slate-700 dark:text-slate-300 mb-3">
          {t('description') || 'Description'}
        </h4>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div>
            <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
              🇺🇸 English
            </label>
            <textarea
              value={formData?.descriptionEn || ''}
              onChange={e => onChange({ ...formData, descriptionEn: e.target.value })}
              className="input min-h-[60px]"
              placeholder="Description in English"
              rows={2}
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
              🇺🇿 O'zbekcha
            </label>
            <textarea
              value={formData?.descriptionUz || ''}
              onChange={e => onChange({ ...formData, descriptionUz: e.target.value })}
              className="input min-h-[60px]"
              placeholder="Ta'rif o'zbekcha"
              rows={2}
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
              🇷🇺 Русский
            </label>
            <textarea
              value={formData?.descriptionRu || ''}
              onChange={e => onChange({ ...formData, descriptionRu: e.target.value })}
              className="input min-h-[60px]"
              placeholder="Описание на русском"
              rows={2}
            />
          </div>
        </div>
      </div>

      {/* Icon URL */}
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          {t('iconUrl') || 'Icon URL'}
        </label>
        <input
          type="text"
          value={formData?.icon || ''}
          onChange={e => onChange({ ...formData, icon: e.target.value || undefined })}
          className="input"
          placeholder="https://example.com/icon.png"
        />
      </div>

      {/* Boolean Options */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        <label className="flex items-center gap-2 cursor-pointer">
          <input
            type="checkbox"
            checked={formData?.showBuildingSuggestion || false}
            onChange={e => onChange({ ...formData, showBuildingSuggestion: e.target.checked })}
            className="w-4 h-4 rounded border-slate-300 text-blue-600 focus:ring-blue-500"
          />
          <span className="text-sm text-slate-700 dark:text-slate-300">
            {t('showBuildingSuggestion') || 'Building Suggestion'}
          </span>
        </label>
        <label className="flex items-center gap-2 cursor-pointer">
          <input
            type="checkbox"
            checked={formData?.showFloorSuggestion || false}
            onChange={e => onChange({ ...formData, showFloorSuggestion: e.target.checked })}
            className="w-4 h-4 rounded border-slate-300 text-blue-600 focus:ring-blue-500"
          />
          <span className="text-sm text-slate-700 dark:text-slate-300">
            {t('showFloorSuggestion') || 'Floor Suggestion'}
          </span>
        </label>
        <label className="flex items-center gap-2 cursor-pointer">
          <input
            type="checkbox"
            checked={formData?.canHaveUnits || false}
            onChange={e => onChange({ ...formData, canHaveUnits: e.target.checked })}
            className="w-4 h-4 rounded border-slate-300 text-blue-600 focus:ring-blue-500"
          />
          <span className="text-sm text-slate-700 dark:text-slate-300">
            {t('canHaveUnits') || 'Can Have Units'}
          </span>
        </label>
        <label className="flex items-center gap-2 cursor-pointer">
          <input
            type="checkbox"
            checked={formData?.canHaveMeters || false}
            onChange={e => onChange({ ...formData, canHaveMeters: e.target.checked })}
            className="w-4 h-4 rounded border-slate-300 text-blue-600 focus:ring-blue-500"
          />
          <span className="text-sm text-slate-700 dark:text-slate-300">
            {t('canHaveMeters') || 'Can Have Meters'}
          </span>
        </label>
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
