import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useApp } from '../context/AppContext'
import { meterTypesService, languagesService } from '../services/api'
import { showToast } from '../components/common'
import CrudPage from '../components/CrudPage'
import type { MeterType, Language } from '../types'

interface LanguageValueInput { languageId: string; languageShortCode: string; value: string }

export default function MeterTypes() {
  const { t } = useApp()
  const queryClient = useQueryClient()
  const [pagination, setPagination] = useState({ pageNumber: 1, pageSize: 10 })

  const { data, isLoading } = useQuery({
    queryKey: ['meter-types', pagination],
    queryFn: () => meterTypesService.getAll(pagination),
  })
  const { data: languagesData } = useQuery({
    queryKey: ['languages', { pageNumber: 1, pageSize: 100 }],
    queryFn: () => languagesService.getAll({ pageNumber: 1, pageSize: 100 }),
  })

  const getLanguageByCode = (code: string): Language | undefined =>
    languagesData?.items.find((l: Language) => l.shortCode === code)

  const buildLangValues = (formData: Record<string, any>, prefix: string) => {
    const values: LanguageValueInput[] = []
    for (const code of ['en', 'uz', 'ru']) {
      const lang = getLanguageByCode(code)
      if (lang) {
        values.push({ languageId: lang.id, languageShortCode: code, value: formData[`${prefix}_${code}`] || '' })
      }
    }
    return values
  }

  const createMut = useMutation({
    mutationFn: (d: any) => meterTypesService.create(d),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['meter-types'] }); showToast.success(t('success'), t('item_created')) },
    onError: (e: Error) => showToast.error(t('error'), e.message),
  })
  const updateMut = useMutation({
    mutationFn: (d: any) => meterTypesService.update(d),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['meter-types'] }); showToast.success(t('success'), t('item_updated')) },
    onError: (e: Error) => showToast.error(t('error'), e.message),
  })
  const deleteMut = useMutation({
    mutationFn: (id: string) => meterTypesService.remove(id),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['meter-types'] }); showToast.success(t('success'), t('item_deleted')) },
    onError: (e: Error) => showToast.error(t('error'), e.message),
  })

  const handleCreate = async (fd: any) => {
    return createMut.mutateAsync({
      names: buildLangValues(fd, 'name'),
      description: buildLangValues(fd, 'desc'),
      icon: fd.icon || undefined,
    })
  }
  const handleUpdate = async (fd: any) => {
    return updateMut.mutateAsync({
      id: fd.id,
      names: buildLangValues(fd, 'name'),
      description: buildLangValues(fd, 'desc'),
      icon: fd.icon || undefined,
    })
  }
  const handleFetchItem = async (id: string) => {
    const detail = await meterTypesService.getById(id)
    // Map getById response to form fields
    const fd: any = { ...detail }
    if ((detail as any).names) {
      for (const t of (detail as any).names) {
        fd[`name_${t.languageShortCode}`] = t.value
      }
    }
    if ((detail as any).description && Array.isArray((detail as any).description)) {
      for (const t of (detail as any).description) {
        fd[`desc_${t.languageShortCode}`] = t.value
      }
    }
    return fd
  }

  const columns = [
    { key: 'name', header: t('name'), render: (item: MeterType) => item.name },
    { key: 'description', header: t('description') || 'Description', render: (item: MeterType) => item.description || '-' },
    { key: 'icon', header: 'Icon', render: (item: MeterType) => item.icon ? '✓' : '-' },
  ]

  const renderForm = (formData: any, onChange: (d: any) => void) => (
    <div className="space-y-4">
      {/* Names translations */}
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">{t('name')}</label>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div><label className="block text-xs text-slate-500 mb-1">🇺🇸 English</label>
            <input type="text" value={formData?.name_en || ''} onChange={e => onChange({ ...formData, name_en: e.target.value })} className="input" /></div>
          <div><label className="block text-xs text-slate-500 mb-1">🇺🇿 O'zbekcha</label>
            <input type="text" value={formData?.name_uz || ''} onChange={e => onChange({ ...formData, name_uz: e.target.value })} className="input" /></div>
          <div><label className="block text-xs text-slate-500 mb-1">🇷🇺 Русский</label>
            <input type="text" value={formData?.name_ru || ''} onChange={e => onChange({ ...formData, name_ru: e.target.value })} className="input" /></div>
        </div>
      </div>
      {/* Description translations */}
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">{t('description') || 'Description'}</label>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div><label className="block text-xs text-slate-500 mb-1">🇺🇸 English</label>
            <textarea value={formData?.desc_en || ''} onChange={e => onChange({ ...formData, desc_en: e.target.value })} className="input min-h-[60px]" /></div>
          <div><label className="block text-xs text-slate-500 mb-1">🇺🇿 O'zbekcha</label>
            <textarea value={formData?.desc_uz || ''} onChange={e => onChange({ ...formData, desc_uz: e.target.value })} className="input min-h-[60px]" /></div>
          <div><label className="block text-xs text-slate-500 mb-1">🇷🇺 Русский</label>
            <textarea value={formData?.desc_ru || ''} onChange={e => onChange({ ...formData, desc_ru: e.target.value })} className="input min-h-[60px]" /></div>
        </div>
      </div>
      {/* Icon */}
      <div>
        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Icon URL</label>
        <input type="text" value={formData?.icon || ''} onChange={e => onChange({ ...formData, icon: e.target.value })} className="input" placeholder="Optional icon URL" />
      </div>
    </div>
  )

  return (
    <CrudPage data={data} columns={columns} isLoading={isLoading} pagination={pagination}
      onPageChange={page => setPagination(p => ({ ...p, pageNumber: page }))}
      onCreate={handleCreate} onUpdate={handleUpdate}
      onDelete={(id: string) => deleteMut.mutateAsync(id)}
      onFetchItem={handleFetchItem}
      renderForm={renderForm} getItemId={item => item.id} getItemName={item => item.name}
      isCreating={createMut.isPending} isUpdating={updateMut.isPending} isDeleting={deleteMut.isPending} />
  )
}
