import { useState, useMemo } from 'react'
import { useSearchParams } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useApp } from '../context/AppContext'
import { useCrudQuery } from '../hooks/useApi'
import CrudPage from '../components/CrudPage'
import { showToast } from '../components/common'
import {
    realEstateTypesService, listingCategoriesService,
    roomTypesService, renovationsService, landCategoriesService,
    productionTypesService, amenityCategoriesService, amenitiesService,
    meterTypesService, meterTariffsService, languagesService,
} from '../services/api'
import type {
    RealEstateType, ListingCategory, RoomType, Renovation,
    LandCategory, ProductionType, AmenityCategory, Amenity,
    MeterType, MeterTariff, Language, BuildingType,
} from '../types'

// --- Shared types ---
interface LanguageValueInput {
    languageId: string
    languageShortCode: string
    value: string
}

// --- Tab definitions ---
const TAB_KEYS = [
    'realEstateTypes', 'listingCategories', 'roomTypes',
    'renovations', 'landCategories', 'productionTypes',
    'amenities', 'meterTypes',
] as const
type TabKey = typeof TAB_KEYS[number]




// --- Simple name-only tab (Categories, Renovations, LandCategories, ProductionTypes, RoomTypes) ---
function SimpleNameTab<T extends { id: string; name: string; nameEn?: string; nameUz?: string; nameRu?: string }>({
    queryKey, service,
}: { queryKey: string; service: any }) {
    const { t, language } = useApp()
    const [pagination, setPagination] = useState({ pageNumber: 1, pageSize: 10 })
    const { data, isLoading, create, update, remove, isCreating, isUpdating, isDeleting } =
        useCrudQuery<T>(queryKey, service, pagination)

    const columns = [{
        key: 'name', header: t('name'),
        render: (item: T) => (language === 'uz' ? (item as any).nameUz : language === 'ru' ? (item as any).nameRu : (item as any).nameEn) || item.name,
    }]

    const renderForm = (formData: Partial<T> | null, onChange: (d: Partial<T>) => void) => (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
                <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">🇺🇸 English</label>
                <input type="text" value={(formData as any)?.nameEn || ''} onChange={e => onChange({ ...formData, nameEn: e.target.value } as any)} className="input" />
            </div>
            <div>
                <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">🇺🇿 O'zbekcha</label>
                <input type="text" value={(formData as any)?.nameUz || ''} onChange={e => onChange({ ...formData, nameUz: e.target.value } as any)} className="input" />
            </div>
            <div>
                <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">🇷🇺 Русский</label>
                <input type="text" value={(formData as any)?.nameRu || ''} onChange={e => onChange({ ...formData, nameRu: e.target.value } as any)} className="input" />
            </div>
        </div>
    )

    return (
        <CrudPage data={data} columns={columns} isLoading={isLoading} pagination={pagination}
            onPageChange={page => setPagination(p => ({ ...p, pageNumber: page }))} onCreate={create} onUpdate={update}
            onDelete={remove} renderForm={renderForm} getItemId={(item: T) => item.id} getItemName={(item: T) => (item as any).nameEn || item.name}
            isCreating={isCreating} isUpdating={isUpdating} isDeleting={isDeleting} />
    )
}

// --- Amenities (unified: category list + inline amenity panel) ---
function AmenitiesTab() {
    const { t, language } = useApp()
    const queryClient = useQueryClient()
    const [selectedCategoryId, setSelectedCategoryId] = useState<string | null>(null)
    const [catPagination, setCatPagination] = useState({ pageNumber: 1, pageSize: 20 })
    const [amenityPagination, setAmenityPagination] = useState({ pageNumber: 1, pageSize: 20 })

    // Languages for translates
    const { data: languagesData } = useQuery({
        queryKey: ['languages', { pageNumber: 1, pageSize: 100 }],
        queryFn: () => languagesService.getAll({ pageNumber: 1, pageSize: 100 }),
    })
    const getLang = (code: string) => languagesData?.items.find((l: Language) => l.shortCode === code)
    const buildTranslates = (fd: Record<string, any>) =>
        ['en', 'uz', 'ru'].map(code => ({ languageId: getLang(code)?.id ?? '', languageShortCode: code, value: fd[`name_${code}`] || '' }))

    // Categories CRUD — custom mutations to send translates payload
    const { data: catData, isLoading: catLoading, remove: removeCat, isDeleting: isDeletingCat } =
        useCrudQuery<AmenityCategory>('amenity-categories', amenityCategoriesService, catPagination)
    const createCatMut = useMutation({
        mutationFn: (d: any) => amenityCategoriesService.create({ translates: buildTranslates(d) } as any),
        onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['amenity-categories'] }); showToast.success(t('success'), t('item_created')) },
        onError: (e: Error) => showToast.error(t('error'), e.message),
    })
    const updateCatMut = useMutation({
        mutationFn: (d: any) => amenityCategoriesService.update({ id: d.id, translates: buildTranslates(d) } as any),
        onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['amenity-categories'] }); showToast.success(t('success'), t('item_updated')) },
        onError: (e: Error) => showToast.error(t('error'), e.message),
    })

    const selectedCategory = catData?.items?.find((c: AmenityCategory) => c.id === selectedCategoryId) ?? null

    // Amenities for selected category
    const createAmenityMut = useMutation({
        mutationFn: (d: any) => amenitiesService.create(d),
        onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['amenities-by-cat', selectedCategoryId] }); showToast.success(t('success'), t('item_created')) },
        onError: (e: Error) => showToast.error(t('error'), e.message),
    })
    const updateAmenityMut = useMutation({
        mutationFn: (d: any) => amenitiesService.update(d),
        onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['amenities-by-cat', selectedCategoryId] }); showToast.success(t('success'), t('item_updated')) },
        onError: (e: Error) => showToast.error(t('error'), e.message),
    })
    const deleteAmenityMut = useMutation({
        mutationFn: (id: string) => amenitiesService.remove(id),
        onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['amenities-by-cat', selectedCategoryId] }); showToast.success(t('success'), t('item_deleted')) },
        onError: (e: Error) => showToast.error(t('error'), e.message),
    })

    const { data: amenitiesData, isLoading: amenitiesLoading } = useQuery({
        queryKey: ['amenities-by-cat', selectedCategoryId, amenityPagination],
        queryFn: () => amenitiesService.getAll({ ...amenityPagination, categoryId: selectedCategoryId } as any),
        enabled: !!selectedCategoryId,
    })

    const localeName = (item: any) =>
        (language === 'uz' ? item.nameUz : language === 'ru' ? item.nameRu : item.nameEn) || item.name

    // Category table columns
    const catColumns = [
        {
            key: 'name', header: t('name'), render: (item: AmenityCategory) => (
                <span className={`font-medium ${selectedCategoryId === item.id ? 'text-blue-600' : ''}`}>{localeName(item)}</span>
            )
        },
        {
            key: 'arrow', header: '', render: (item: AmenityCategory) => (
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"
                    className={`transition-transform ${selectedCategoryId === item.id ? 'rotate-90 text-blue-600' : 'text-slate-400'}`}>
                    <path d="M9 18l6-6-6-6" strokeLinecap="round" strokeLinejoin="round" />
                </svg>
            )
        },
    ]
    const catRenderForm = (formData: any, onChange: (d: any) => void) => (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div><label className="block text-xs text-slate-500 mb-1">🇺🇸 English</label>
                <input type="text" value={formData?.name_en || ''} onChange={e => onChange({ ...formData, name_en: e.target.value })} className="input" /></div>
            <div><label className="block text-xs text-slate-500 mb-1">🇺🇿 O'zbekcha</label>
                <input type="text" value={formData?.name_uz || ''} onChange={e => onChange({ ...formData, name_uz: e.target.value })} className="input" /></div>
            <div><label className="block text-xs text-slate-500 mb-1">🇷🇺 Русский</label>
                <input type="text" value={formData?.name_ru || ''} onChange={e => onChange({ ...formData, name_ru: e.target.value })} className="input" /></div>
        </div>
    )

    // Amenity form
    const amenityRenderForm = (formData: Partial<Amenity> | null, onChange: (d: Partial<Amenity>) => void) => (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div><label className="block text-xs text-slate-500 mb-1">🇺🇸 English</label>
                <input type="text" value={formData?.nameEn || ''} onChange={e => onChange({ ...formData, nameEn: e.target.value })} className="input" /></div>
            <div><label className="block text-xs text-slate-500 mb-1">🇺🇿 O'zbekcha</label>
                <input type="text" value={formData?.nameUz || ''} onChange={e => onChange({ ...formData, nameUz: e.target.value })} className="input" /></div>
            <div><label className="block text-xs text-slate-500 mb-1">🇷🇺 Русский</label>
                <input type="text" value={formData?.nameRu || ''} onChange={e => onChange({ ...formData, nameRu: e.target.value })} className="input" /></div>
            <div className="md:col-span-2"><label className="block text-xs text-slate-500 mb-1">Icon URL</label>
                <input type="text" value={formData?.icon || ''} onChange={e => onChange({ ...formData, icon: e.target.value })} className="input" placeholder="Optional" /></div>
        </div>
    )

    const amenityColumns = [
        { key: 'name', header: t('name'), render: (item: Amenity) => localeName(item) },
        {
            key: 'icon', header: 'Icon', render: (item: Amenity) => item.iconUrl
                ? <img src={item.iconUrl} alt="" className="w-6 h-6 rounded" />
                : <span className="text-slate-400 text-xs">—</span>
        },
    ]

    const handleRowClick = (item: AmenityCategory) => {
        setSelectedCategoryId(prev => prev === item.id ? null : item.id)
        setAmenityPagination({ pageNumber: 1, pageSize: 20 })
    }

    return (
        <div className="space-y-4">
            {/* Categories table */}
            <CrudPage
                data={catData} columns={catColumns} isLoading={catLoading} pagination={catPagination}
                onPageChange={page => setCatPagination(p => ({ ...p, pageNumber: page }))}
                onCreate={createCatMut.mutateAsync} onUpdate={updateCatMut.mutateAsync} onDelete={removeCat}
                renderForm={catRenderForm}
                getItemId={item => item.id} getItemName={item => item.name}
                isCreating={createCatMut.isPending} isUpdating={updateCatMut.isPending} isDeleting={isDeletingCat}
                onRowClick={handleRowClick}
            />

            {/* Amenities panel for selected category */}
            {selectedCategoryId && selectedCategory && (
                <div className="border border-blue-200 dark:border-blue-800 rounded-xl overflow-hidden">
                    {/* Panel header */}
                    <div className="flex items-center gap-2 px-4 py-3 bg-blue-50 dark:bg-blue-900/20 border-b border-blue-200 dark:border-blue-800">
                        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round" className="text-blue-600">
                            <path d="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z" />
                        </svg>
                        <span className="text-sm font-semibold text-blue-700 dark:text-blue-300">
                            {localeName(selectedCategory)} — Amenities
                        </span>
                        <button
                            onClick={() => setSelectedCategoryId(null)}
                            className="ml-auto text-blue-400 hover:text-blue-600 transition-colors"
                        >
                            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                <path d="M18 6L6 18M6 6l12 12" />
                            </svg>
                        </button>
                    </div>
                    {/* Amenities crud */}
                    <div className="p-4">
                        <CrudPage
                            data={amenitiesData} columns={amenityColumns} isLoading={amenitiesLoading}
                            pagination={amenityPagination}
                            onPageChange={page => setAmenityPagination(p => ({ ...p, pageNumber: page }))}
                            onCreate={(d: any) => createAmenityMut.mutateAsync({ ...d, categoryId: selectedCategoryId })}
                            onUpdate={(d: any) => updateAmenityMut.mutateAsync({ ...d, categoryId: selectedCategoryId })}
                            onDelete={(id: string) => deleteAmenityMut.mutateAsync(id)}
                            renderForm={amenityRenderForm}
                            getItemId={item => item.id} getItemName={item => localeName(item)}
                            isCreating={createAmenityMut.isPending} isUpdating={updateAmenityMut.isPending} isDeleting={deleteAmenityMut.isPending}
                        />
                    </div>
                </div>
            )}
        </div>
    )
}

// --- Meter Types + Tariffs (unified) ---
function MeterTypesTab() {
    const { t } = useApp()
    const queryClient = useQueryClient()
    const [selectedTypeId, setSelectedTypeId] = useState<string | null>(null)
    const [pagination, setPagination] = useState({ pageNumber: 1, pageSize: 10 })
    const [tariffPagination, setTariffPagination] = useState({ pageNumber: 1, pageSize: 20 })

    const { data: languagesData } = useQuery({
        queryKey: ['languages', { pageNumber: 1, pageSize: 100 }],
        queryFn: () => languagesService.getAll({ pageNumber: 1, pageSize: 100 }),
    })
    const getLanguageByCode = (code: string): Language | undefined =>
        languagesData?.items.find((l: Language) => l.shortCode === code)
    const buildLangValues = (formData: Record<string, any>, prefix: string) =>
        ['en', 'uz', 'ru'].map(code => ({ languageId: getLanguageByCode(code)?.id ?? '', languageShortCode: code, value: formData[`${prefix}_${code}`] || '' }))

    // Meter Types CRUD
    const { data, isLoading } = useQuery({
        queryKey: ['meter-types', pagination],
        queryFn: () => meterTypesService.getAll(pagination),
    })
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
    const handleCreate = async (fd: any) => createMut.mutateAsync({ names: buildLangValues(fd, 'name'), description: buildLangValues(fd, 'desc'), icon: fd.icon || undefined })
    const handleUpdate = async (fd: any) => updateMut.mutateAsync({ id: fd.id, names: buildLangValues(fd, 'name'), description: buildLangValues(fd, 'desc'), icon: fd.icon || undefined })
    const handleFetchItem = async (id: string) => {
        const detail = await meterTypesService.getById(id)
        const fd: any = { ...detail }
        if ((detail as any).names) for (const tr of (detail as any).names) fd[`name_${tr.languageShortCode}`] = tr.value
        if (Array.isArray((detail as any).description)) for (const tr of (detail as any).description) fd[`desc_${tr.languageShortCode}`] = tr.value
        return fd
    }
    const selectedType = data?.items?.find((m: MeterType) => m.id === selectedTypeId) ?? null

    // Tariffs for selected type
    const { data: tariffsData, isLoading: tariffsLoading } = useQuery({
        queryKey: ['meter-tariffs-by-type', selectedTypeId, tariffPagination],
        queryFn: () => meterTariffsService.getAll({ ...tariffPagination, meterTypeId: selectedTypeId } as any),
        enabled: !!selectedTypeId,
    })
    const createTariffMut = useMutation({
        mutationFn: (d: any) => meterTariffsService.create(d),
        onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['meter-tariffs-by-type', selectedTypeId] }); showToast.success(t('success'), t('item_created')) },
        onError: (e: Error) => showToast.error(t('error'), e.message),
    })
    const updateTariffMut = useMutation({
        mutationFn: (d: any) => meterTariffsService.update(d),
        onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['meter-tariffs-by-type', selectedTypeId] }); showToast.success(t('success'), t('item_updated')) },
        onError: (e: Error) => showToast.error(t('error'), e.message),
    })
    const deleteTariffMut = useMutation({
        mutationFn: (id: string) => meterTariffsService.remove(id),
        onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['meter-tariffs-by-type', selectedTypeId] }); showToast.success(t('success'), t('item_deleted')) },
        onError: (e: Error) => showToast.error(t('error'), e.message),
    })

    const formatPrice = (p: number) => new Intl.NumberFormat('en-US').format(p)
    const TARIFF_TYPES = [
        { value: 0, label: 'Individual' },
        { value: 1, label: 'Legal' },
        { value: 2, label: 'Budget' },
    ]
    const SEASONS = [
        { value: 0, label: 'All Year' },
        { value: 1, label: 'Summer' },
        { value: 2, label: 'Winter' },
    ]

    const typeColumns = [
        {
            key: 'name', header: t('name'), render: (item: MeterType) => (
                <span className={`font-medium ${selectedTypeId === item.id ? 'text-emerald-600' : ''}`}>{item.name}</span>
            )
        },
        {
            key: 'arrow', header: '', render: (item: MeterType) => (
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"
                    className={`transition-transform ${selectedTypeId === item.id ? 'rotate-90 text-emerald-600' : 'text-slate-400'}`}>
                    <path d="M9 18l6-6-6-6" strokeLinecap="round" strokeLinejoin="round" />
                </svg>
            )
        },
    ]
    const typeRenderForm = (formData: any, onChange: (d: any) => void) => (
        <div className="space-y-4">
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
            <div><label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Icon URL</label>
                <input type="text" value={formData?.icon || ''} onChange={e => onChange({ ...formData, icon: e.target.value })} className="input" placeholder="Optional" /></div>
        </div>
    )

    const tariffColumns = [
        { key: 'type', header: 'Type', render: (item: MeterTariff) => TARIFF_TYPES.find(t => t.value === item.type)?.label ?? item.type },
        { key: 'price', header: 'Price', render: (item: MeterTariff) => formatPrice(item.price) },
        { key: 'validFrom', header: 'Valid From', render: (item: MeterTariff) => item.validFrom ?? '—' },
        { key: 'validTo', header: 'Valid To', render: (item: MeterTariff) => item.validTo ?? '∞' },
        { key: 'isActual', header: 'Actual', render: (item: MeterTariff) => item.isActual ? '✓' : '—' },
    ]
    const tariffRenderForm = (formData: Partial<MeterTariff> | null, onChange: (d: Partial<MeterTariff>) => void) => (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div><label className="block text-xs text-slate-500 mb-1">Type</label>
                <select value={formData?.type ?? 0} onChange={e => onChange({ ...formData, type: parseInt(e.target.value) })} className="input">
                    {TARIFF_TYPES.map(tt => <option key={tt.value} value={tt.value}>{tt.label}</option>)}
                </select></div>
            <div><label className="block text-xs text-slate-500 mb-1">Price</label>
                <input type="number" value={formData?.price ?? ''} onChange={e => onChange({ ...formData, price: parseFloat(e.target.value) || 0 })} className="input" /></div>
            <div><label className="block text-xs text-slate-500 mb-1">Fixed Price</label>
                <input type="number" value={formData?.fixedPrice ?? ''} onChange={e => onChange({ ...formData, fixedPrice: e.target.value ? parseFloat(e.target.value) : undefined })} className="input" /></div>
            <div><label className="block text-xs text-slate-500 mb-1">Valid From</label>
                <input type="date" value={formData?.validFrom ?? ''} onChange={e => onChange({ ...formData, validFrom: e.target.value || undefined })} className="input" /></div>
            <div><label className="block text-xs text-slate-500 mb-1">Valid To</label>
                <input type="date" value={formData?.validTo ?? ''} onChange={e => onChange({ ...formData, validTo: e.target.value || undefined })} className="input" /></div>
            <div><label className="block text-xs text-slate-500 mb-1">Season</label>
                <select value={formData?.season ?? 0} onChange={e => onChange({ ...formData, season: parseInt(e.target.value) })} className="input">
                    {SEASONS.map(s => <option key={s.value} value={s.value}>{s.label}</option>)}
                </select></div>
            <div><label className="block text-xs text-slate-500 mb-1">Min Limit</label>
                <input type="number" value={formData?.minLimit ?? ''} onChange={e => onChange({ ...formData, minLimit: e.target.value ? parseFloat(e.target.value) : undefined })} className="input" /></div>
            <div><label className="block text-xs text-slate-500 mb-1">Max Limit</label>
                <input type="number" value={formData?.maxLimit ?? ''} onChange={e => onChange({ ...formData, maxLimit: e.target.value ? parseFloat(e.target.value) : undefined })} className="input" /></div>
            <div><label className="block text-xs text-slate-500 mb-1">Social Norm Limit</label>
                <input type="number" value={formData?.socialNormLimit ?? ''} onChange={e => onChange({ ...formData, socialNormLimit: e.target.value ? parseFloat(e.target.value) : undefined })} className="input" /></div>
            <div className="flex items-center gap-2 mt-2">
                <input type="checkbox" id="isActual" checked={formData?.isActual ?? false} onChange={e => onChange({ ...formData, isActual: e.target.checked })} className="w-4 h-4" />
                <label htmlFor="isActual" className="text-sm text-slate-700 dark:text-slate-300">Is Actual</label>
            </div>
            <div className="md:col-span-2"><label className="block text-xs text-slate-500 mb-1">Comment</label>
                <textarea value={formData?.comment ?? ''} onChange={e => onChange({ ...formData, comment: e.target.value || undefined })} className="input min-h-[60px]" /></div>
        </div>
    )

    const handleRowClick = (item: MeterType) => {
        setSelectedTypeId(prev => prev === item.id ? null : item.id)
        setTariffPagination({ pageNumber: 1, pageSize: 20 })
    }

    return (
        <div className="space-y-4">
            <CrudPage data={data} columns={typeColumns} isLoading={isLoading} pagination={pagination}
                onPageChange={page => setPagination(p => ({ ...p, pageNumber: page }))}
                onCreate={handleCreate} onUpdate={handleUpdate}
                onDelete={(id: string) => deleteMut.mutateAsync(id)}
                onFetchItem={handleFetchItem}
                renderForm={typeRenderForm} getItemId={item => item.id} getItemName={item => item.name}
                isCreating={createMut.isPending} isUpdating={updateMut.isPending} isDeleting={deleteMut.isPending}
                onRowClick={handleRowClick}
            />

            {selectedTypeId && selectedType && (
                <div className="border border-emerald-200 dark:border-emerald-800 rounded-xl overflow-hidden">
                    <div className="flex items-center gap-2 px-4 py-3 bg-emerald-50 dark:bg-emerald-900/20 border-b border-emerald-200 dark:border-emerald-800">
                        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round" className="text-emerald-600">
                            <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-1 14H9V8h2v8zm4 0h-2V8h2v8z" />
                        </svg>
                        <span className="text-sm font-semibold text-emerald-700 dark:text-emerald-300">{selectedType.name} — Tariffs</span>
                        <button onClick={() => setSelectedTypeId(null)} className="ml-auto text-emerald-400 hover:text-emerald-600 transition-colors">
                            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                <path d="M18 6L6 18M6 6l12 12" />
                            </svg>
                        </button>
                    </div>
                    <div className="p-4">
                        <CrudPage
                            data={tariffsData} columns={tariffColumns} isLoading={tariffsLoading}
                            pagination={tariffPagination}
                            onPageChange={page => setTariffPagination(p => ({ ...p, pageNumber: page }))}
                            onCreate={(d: any) => createTariffMut.mutateAsync({ ...d, meterTypeId: selectedTypeId })}
                            onUpdate={(d: any) => updateTariffMut.mutateAsync({ ...d, meterTypeId: selectedTypeId })}
                            onDelete={(id: string) => deleteTariffMut.mutateAsync(id)}
                            renderForm={tariffRenderForm}
                            getItemId={item => item.id} getItemName={item => `${TARIFF_TYPES.find(tt => tt.value === item.type)?.label ?? item.type} — ${formatPrice(item.price)}`}
                            isCreating={createTariffMut.isPending} isUpdating={updateTariffMut.isPending} isDeleting={deleteTariffMut.isPending}
                        />
                    </div>
                </div>
            )}
        </div>
    )
}

// --- Real Estate Types (complex: translates + booleans) ---
function RealEstateTypesTab() {
    const { t } = useApp()
    const queryClient = useQueryClient()
    const [pagination, setPagination] = useState({ pageNumber: 1, pageSize: 10 })

    const { data, isLoading } = useQuery({
        queryKey: ['real-estate-types', pagination],
        queryFn: () => realEstateTypesService.getAll(pagination),
    })
    const { data: languagesData } = useQuery({
        queryKey: ['languages', { pageNumber: 1, pageSize: 100 }],
        queryFn: () => languagesService.getAll({ pageNumber: 1, pageSize: 100 }),
    })

    const getLanguageByCode = (code: string): Language | undefined =>
        languagesData?.items.find((lang: Language) => lang.shortCode === code)

    const createLanguageValues = (en?: string, uz?: string, ru?: string): LanguageValueInput[] => {
        const vals: LanguageValueInput[] = []
        const enL = getLanguageByCode('en'), uzL = getLanguageByCode('uz'), ruL = getLanguageByCode('ru')
        if (en && enL) vals.push({ languageId: enL.id, languageShortCode: 'en', value: en })
        if (uz && uzL) vals.push({ languageId: uzL.id, languageShortCode: 'uz', value: uz })
        if (ru && ruL) vals.push({ languageId: ruL.id, languageShortCode: 'ru', value: ru })
        return vals
    }

    const createMut = useMutation({
        mutationFn: (d: any) => realEstateTypesService.create(d),
        onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['real-estate-types'] }); showToast.success(t('success'), t('item_created')) },
        onError: (e: Error) => showToast.error(t('error'), e.message),
    })
    const updateMut = useMutation({
        mutationFn: (d: any) => realEstateTypesService.update(d),
        onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['real-estate-types'] }); showToast.success(t('success'), t('item_updated')) },
        onError: (e: Error) => showToast.error(t('error'), e.message),
    })
    const deleteMut = useMutation({
        mutationFn: (id: string) => realEstateTypesService.delete(id),
        onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['real-estate-types'] }); showToast.success(t('success'), t('item_deleted')) },
        onError: (e: Error) => showToast.error(t('error'), e.message),
    })

    const columns = [
        { key: 'name', header: t('name') || 'Name' },
        { key: 'description', header: t('description') || 'Description' },
        { key: 'showBuildingSuggestion', header: 'Building', render: (i: RealEstateType) => i.showBuildingSuggestion ? '✓' : '-' },
        { key: 'showFloorSuggestion', header: 'Floor', render: (i: RealEstateType) => i.showFloorSuggestion ? '✓' : '-' },
        { key: 'canHaveUnits', header: 'Units', render: (i: RealEstateType) => i.canHaveUnits ? '✓' : '-' },
        { key: 'canHaveMeters', header: 'Meters', render: (i: RealEstateType) => i.canHaveMeters ? '✓' : '-' },
    ]

    type FormData = Partial<RealEstateType & { nameEn?: string; nameUz?: string; nameRu?: string; descriptionEn?: string; descriptionUz?: string; descriptionRu?: string }>

    const handleCreate = (fd: FormData) => createMut.mutateAsync({
        names: createLanguageValues(fd.nameEn, fd.nameUz, fd.nameRu),
        descriptions: createLanguageValues(fd.descriptionEn, fd.descriptionUz, fd.descriptionRu),
        iconUrl: fd.icon, showBuildingSuggestion: fd.showBuildingSuggestion || false,
        showFloorSuggestion: fd.showFloorSuggestion || false, canHaveUnits: fd.canHaveUnits || false, canHaveMeters: fd.canHaveMeters || false,
    })
    const handleUpdate = (fd: FormData) => updateMut.mutateAsync({
        id: fd.id, names: createLanguageValues(fd.nameEn, fd.nameUz, fd.nameRu),
        descriptions: createLanguageValues(fd.descriptionEn, fd.descriptionUz, fd.descriptionRu),
        iconUrl: fd.icon, showBuildingSuggestion: fd.showBuildingSuggestion || false,
        showFloorSuggestion: fd.showFloorSuggestion || false, canHaveUnits: fd.canHaveUnits || false, canHaveMeters: fd.canHaveMeters || false,
    })
    const handleDelete = (id: string) => deleteMut.mutateAsync(id)

    const renderForm = (fd: FormData | null, onChange: (d: FormData) => void) => (
        <div className="space-y-5">
            <div>
                <h4 className="text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">{t('name')}</h4>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
                    <div><label className="block text-xs text-slate-500 mb-1">🇺🇸 English</label>
                        <input type="text" value={fd?.nameEn || ''} onChange={e => onChange({ ...fd, nameEn: e.target.value })} className="input" /></div>
                    <div><label className="block text-xs text-slate-500 mb-1">🇺🇿 O'zbekcha</label>
                        <input type="text" value={fd?.nameUz || ''} onChange={e => onChange({ ...fd, nameUz: e.target.value })} className="input" /></div>
                    <div><label className="block text-xs text-slate-500 mb-1">🇷🇺 Русский</label>
                        <input type="text" value={fd?.nameRu || ''} onChange={e => onChange({ ...fd, nameRu: e.target.value })} className="input" /></div>
                </div>
            </div>
            <div>
                <h4 className="text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">{t('description')}</h4>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
                    <div><label className="block text-xs text-slate-500 mb-1">🇺🇸 English</label>
                        <textarea value={fd?.descriptionEn || ''} onChange={e => onChange({ ...fd, descriptionEn: e.target.value })} className="input min-h-[60px]" rows={2} /></div>
                    <div><label className="block text-xs text-slate-500 mb-1">🇺🇿 O'zbekcha</label>
                        <textarea value={fd?.descriptionUz || ''} onChange={e => onChange({ ...fd, descriptionUz: e.target.value })} className="input min-h-[60px]" rows={2} /></div>
                    <div><label className="block text-xs text-slate-500 mb-1">🇷🇺 Русский</label>
                        <textarea value={fd?.descriptionRu || ''} onChange={e => onChange({ ...fd, descriptionRu: e.target.value })} className="input min-h-[60px]" rows={2} /></div>
                </div>
            </div>
            <div><label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Icon URL</label>
                <input type="text" value={fd?.icon || ''} onChange={e => onChange({ ...fd, icon: e.target.value || undefined })} className="input" placeholder="https://..." /></div>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
                {[
                    { key: 'showBuildingSuggestion', label: t('showBuildingSuggestion') || 'Building Suggestion' },
                    { key: 'showFloorSuggestion', label: t('showFloorSuggestion') || 'Floor Suggestion' },
                    { key: 'canHaveUnits', label: t('canHaveUnits') || 'Can Have Units' },
                    { key: 'canHaveMeters', label: t('canHaveMeters') || 'Can Have Meters' },
                ].map(opt => (
                    <label key={opt.key} className="flex items-center gap-2 cursor-pointer">
                        <input type="checkbox" checked={(fd as any)?.[opt.key] || false} onChange={e => onChange({ ...fd, [opt.key]: e.target.checked })}
                            className="w-4 h-4 rounded border-slate-300 text-blue-600" />
                        <span className="text-sm text-slate-700 dark:text-slate-300">{opt.label}</span>
                    </label>
                ))}
            </div>
        </div>
    )

    return (
        <CrudPage data={data} columns={columns} isLoading={isLoading} pagination={pagination}
            onPageChange={page => setPagination(p => ({ ...p, pageNumber: page }))}
            onPageSizeChange={size => setPagination(p => ({ ...p, pageSize: size, pageNumber: 1 }))}
            onCreate={handleCreate} onUpdate={handleUpdate} onDelete={handleDelete} renderForm={renderForm}
            getItemId={item => item.id} getItemName={item => item.name}
            isCreating={createMut.isPending} isUpdating={updateMut.isPending} isDeleting={deleteMut.isPending} />
    )
}

// --- Listing Categories (translates + buildingType + parent + icon) ---
function ListingCategoriesTab() {
    const { t } = useApp()
    const queryClient = useQueryClient()
    const [pagination, setPagination] = useState({ pageNumber: 1, pageSize: 10 })

    const { data, isLoading } = useQuery({
        queryKey: ['listing-categories', pagination],
        queryFn: () => listingCategoriesService.getAll(pagination),
    })
    const { data: languagesData } = useQuery({
        queryKey: ['languages', { pageNumber: 1, pageSize: 100 }],
        queryFn: () => languagesService.getAll({ pageNumber: 1, pageSize: 100 }),
    })
    const { data: parentCategoriesData } = useQuery({
        queryKey: ['listing-categories-all', { pageNumber: 1, pageSize: 500 }],
        queryFn: () => listingCategoriesService.getAll({ pageNumber: 1, pageSize: 500 }),
    })

    const getLanguageByCode = (code: string): Language | undefined =>
        languagesData?.items.find((lang: Language) => lang.shortCode === code)
    const createLanguageValues = (en?: string, uz?: string, ru?: string): LanguageValueInput[] => {
        const vals: LanguageValueInput[] = []
        const enL = getLanguageByCode('en'), uzL = getLanguageByCode('uz'), ruL = getLanguageByCode('ru')
        if (en && enL) vals.push({ languageId: enL.id, languageShortCode: 'en', value: en })
        if (uz && uzL) vals.push({ languageId: uzL.id, languageShortCode: 'uz', value: uz })
        if (ru && ruL) vals.push({ languageId: ruL.id, languageShortCode: 'ru', value: ru })
        return vals
    }

    const getBuildingTypeLabel = (type: BuildingType | number) => {
        switch (typeof type === 'number' ? type : type) {
            case 0: case 'Commercial': return t('commercial') || 'Commercial'
            case 1: case 'Living': return t('living') || 'Living'
            default: return String(type)
        }
    }
    const getParentName = (parentId?: string) => {
        if (!parentId) return '-'
        return parentCategoriesData?.items.find((c: ListingCategory) => c.id === parentId)?.name || parentId
    }

    const createMut = useMutation({
        mutationFn: (d: any) => listingCategoriesService.create(d),
        onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['listing-categories'] }); showToast.success(t('success'), t('item_created')) },
        onError: (e: Error) => showToast.error(t('error'), e.message),
    })
    const updateMut = useMutation({
        mutationFn: (d: any) => listingCategoriesService.update(d),
        onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['listing-categories'] }); showToast.success(t('success'), t('item_updated')) },
        onError: (e: Error) => showToast.error(t('error'), e.message),
    })
    const deleteMut = useMutation({
        mutationFn: (id: string) => listingCategoriesService.remove(id),
        onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['listing-categories'] }); showToast.success(t('success'), t('item_deleted')) },
        onError: (e: Error) => showToast.error(t('error'), e.message),
    })

    const columns = [
        { key: 'name', header: t('name') },
        { key: 'buildingType', header: t('buildingType') || 'Building Type', render: (i: ListingCategory) => getBuildingTypeLabel(i.buildingType) },
        { key: 'parentId', header: t('parentCategory') || 'Parent', render: (i: ListingCategory) => getParentName(i.parentId) },
        { key: 'iconUrl', header: 'Icon', render: (i: ListingCategory) => i.iconUrl ? <img src={i.iconUrl} alt="" className="w-6 h-6 object-contain" /> : '-' },
    ]

    type FormData = Partial<ListingCategory & { nameEn?: string; nameUz?: string; nameRu?: string }>
    const handleCreate = (fd: FormData) => createMut.mutateAsync({
        buildingType: typeof fd.buildingType === 'number' ? fd.buildingType : 0,
        iconUrl: fd.iconUrl || '', translates: createLanguageValues(fd.nameEn, fd.nameUz, fd.nameRu),
        parentId: fd.parentId || undefined,
    })
    const handleUpdate = (fd: FormData) => updateMut.mutateAsync({
        id: fd.id, parentId: fd.parentId || undefined,
        buildingType: typeof fd.buildingType === 'number' ? fd.buildingType : 0,
        iconUrl: fd.iconUrl || '', translates: createLanguageValues(fd.nameEn, fd.nameUz, fd.nameRu),
    })
    const handleDelete = (id: string) => deleteMut.mutateAsync(id)

    const renderForm = (fd: FormData | null, onChange: (d: FormData) => void) => (
        <div className="space-y-5">
            <div>
                <h4 className="text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">{t('name')}</h4>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
                    <div><label className="block text-xs text-slate-500 mb-1">🇺🇸 English</label>
                        <input type="text" value={fd?.nameEn || ''} onChange={e => onChange({ ...fd, nameEn: e.target.value })} className="input" /></div>
                    <div><label className="block text-xs text-slate-500 mb-1">🇺🇿 O'zbekcha</label>
                        <input type="text" value={fd?.nameUz || ''} onChange={e => onChange({ ...fd, nameUz: e.target.value })} className="input" /></div>
                    <div><label className="block text-xs text-slate-500 mb-1">🇷🇺 Русский</label>
                        <input type="text" value={fd?.nameRu || ''} onChange={e => onChange({ ...fd, nameRu: e.target.value })} className="input" /></div>
                </div>
            </div>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div><label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">{t('buildingType') || 'Building Type'} *</label>
                    <select value={fd?.buildingType ?? 0} onChange={e => onChange({ ...fd, buildingType: parseInt(e.target.value) as unknown as BuildingType })} className="input">
                        <option value={0}>{t('commercial') || 'Commercial'}</option>
                        <option value={1}>{t('living') || 'Living'}</option>
                    </select></div>
                <div><label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">{t('parentCategory') || 'Parent'}</label>
                    <select value={fd?.parentId || ''} onChange={e => onChange({ ...fd, parentId: e.target.value || undefined })} className="input">
                        <option value="">No Parent</option>
                        {parentCategoriesData?.items.filter((c: ListingCategory) => c.id !== fd?.id).map((c: ListingCategory) => (
                            <option key={c.id} value={c.id}>{c.name}</option>
                        ))}
                    </select></div>
            </div>
            <div><label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Icon URL *</label>
                <input type="text" value={fd?.iconUrl || ''} onChange={e => onChange({ ...fd, iconUrl: e.target.value })} className="input" placeholder="https://..." />
                {fd?.iconUrl && <div className="mt-2 flex items-center gap-2"><span className="text-sm text-slate-500">Preview:</span><img src={fd.iconUrl} alt="" className="w-8 h-8 object-contain" /></div>}
            </div>
        </div>
    )

    return (
        <CrudPage data={data} columns={columns} isLoading={isLoading} pagination={pagination}
            onPageChange={page => setPagination(p => ({ ...p, pageNumber: page }))}
            onPageSizeChange={size => setPagination(p => ({ ...p, pageSize: size, pageNumber: 1 }))}
            onCreate={handleCreate} onUpdate={handleUpdate} onDelete={handleDelete} renderForm={renderForm}
            getItemId={item => item.id} getItemName={item => item.name}
            isCreating={createMut.isPending} isUpdating={updateMut.isPending} isDeleting={deleteMut.isPending} />
    )
}

// ========================
// MAIN PAGE COMPONENT
// ========================
export default function PropertyConfigPage() {
    const { t } = useApp()
    const [searchParams, setSearchParams] = useSearchParams()
    const activeTab = (searchParams.get('tab') as TabKey) || 'realEstateTypes'

    const tabLabels: Record<TabKey, string> = useMemo(() => ({
        realEstateTypes: t('realEstateTypes') || 'RE Types',
        listingCategories: t('listingCategories') || 'Listing Cat.',
        roomTypes: t('roomTypes') || 'Room Types',
        renovations: t('renovations') || 'Renovations',
        landCategories: t('landCategories') || 'Land Cat.',
        productionTypes: t('productionTypes') || 'Production Types',
        amenities: t('amenities') || 'Amenities',
        meterTypes: t('meterTypes') || 'Meter Types',
    }), [t])

    const renderTab = () => {
        switch (activeTab) {
            case 'roomTypes': return <SimpleNameTab<RoomType> queryKey="room-types" service={roomTypesService} />
            case 'renovations': return <SimpleNameTab<Renovation> queryKey="renovations" service={renovationsService} />
            case 'landCategories': return <SimpleNameTab<LandCategory> queryKey="landcategories" service={landCategoriesService} />
            case 'productionTypes': return <SimpleNameTab<ProductionType> queryKey="production-types" service={productionTypesService} />
            case 'amenities': return <AmenitiesTab />
            case 'meterTypes': return <MeterTypesTab />
            case 'realEstateTypes': return <RealEstateTypesTab />
            case 'listingCategories': return <ListingCategoriesTab />
            default: return <RealEstateTypesTab />
        }
    }

    return (
        <div>
            {/* Tab bar — matching reference design */}
            <div className="mb-6 -mt-1">
                <div
                    className="inline-flex flex-wrap gap-1 items-center"
                    style={{ padding: '4px', borderRadius: '100px', background: '#F6F6F6' }}
                >
                    {TAB_KEYS.map(key => (
                        <button
                            key={key}
                            onClick={() => setSearchParams({ tab: key })}
                            style={{
                                padding: '6px 16px',
                                borderRadius: '100px',
                                fontSize: '13px',
                                fontWeight: 500,
                                lineHeight: '24px',
                                border: 'none',
                                cursor: 'pointer',
                                transition: 'all 0.2s ease',
                                whiteSpace: 'nowrap',
                                ...(activeTab === key
                                    ? { background: '#000', color: '#fff' }
                                    : { background: 'transparent', color: '#19191C' }
                                ),
                            }}
                            onMouseEnter={e => {
                                if (activeTab !== key) {
                                    (e.target as HTMLElement).style.background = '#E8E8E8'
                                }
                            }}
                            onMouseLeave={e => {
                                if (activeTab !== key) {
                                    (e.target as HTMLElement).style.background = 'transparent'
                                }
                            }}
                        >
                            {tabLabels[key]}
                        </button>
                    ))}
                </div>
            </div>

            {/* Active tab content */}
            {renderTab()}
        </div>
    )
}
