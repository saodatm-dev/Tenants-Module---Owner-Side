import { useApp } from '../context/AppContext'
import { useQuery } from '@tanstack/react-query'
import { regionsService, districtsService, complexesService, buildingsService, floorsService, languagesService } from '../services/api'
import type { TreeNode, NodeLevel } from './HierarchyTreeSidebar'
import DeleteModal from './DeleteModal'
import { useState } from 'react'

interface DetailPanelProps {
    node: TreeNode | null
    mode: 'view' | 'edit' | 'create'
    createLevel: NodeLevel | null
    parentNode: TreeNode | null
    formData: Record<string, any>
    onFormChange: (data: Record<string, any>) => void
    onEdit: () => void
    onDelete: () => void
    onSave: () => void
    onCancel: () => void
    onSelectNode: (node: TreeNode) => void
    onAddChild: (parent: TreeNode, level: NodeLevel) => void
    isSaving: boolean
    isDeleting: boolean
}

const levelLabels: Record<NodeLevel, { en: string; uz: string; ru: string }> = {
    region: { en: 'Region', uz: 'Viloyat', ru: 'Регион' },
    district: { en: 'District', uz: 'Tuman', ru: 'Район' },
    complex: { en: 'Complex', uz: 'Majmua', ru: 'Комплекс' },
    building: { en: 'Building', uz: 'Bino', ru: 'Здание' },
    floor: { en: 'Floor', uz: 'Qavat', ru: 'Этаж' },
}

const childLevelMap: Record<NodeLevel, NodeLevel | null> = {
    region: 'district',
    district: 'complex',
    complex: 'building',
    building: 'floor',
    floor: null,
}

function Breadcrumb({ node, parentNode, createLevel }: { node: TreeNode | null; parentNode: TreeNode | null; createLevel: NodeLevel | null }) {
    const parts: string[] = []
    if (parentNode) parts.push(`${levelLabels[parentNode.level].en}: ${parentNode.name}`)
    if (node) parts.push(`${levelLabels[node.level].en}: ${node.name}`)
    if (createLevel && !node) parts.push(`New ${levelLabels[createLevel].en}`)
    if (parts.length === 0) return null
    return (
        <div className="flex items-center gap-1.5 text-xs text-slate-500 dark:text-slate-400 mb-3 flex-wrap">
            {parts.map((p, i) => (
                <span key={i} className="flex items-center gap-1.5">
                    {i > 0 && <span className="text-slate-400">›</span>}
                    <span>{p}</span>
                </span>
            ))}
        </div>
    )
}

function FieldRow({ label, children }: { label: string; children: React.ReactNode }) {
    return (
        <div>
            <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">{label}</label>
            {children}
        </div>
    )
}

// --- Forms for each level ---
// Helper: per-language name inputs for translates-based entities
function TranslateNameFields({ data, onChange, readOnly, languages }: { data: Record<string, any>; onChange: (d: Record<string, any>) => void; readOnly: boolean; languages: any[] }) {
    const { t } = useApp()
    if (readOnly) {
        return (
            <FieldRow label={t('name')}>
                <input type="text" value={data.name || ''} className="input" readOnly />
            </FieldRow>
        )
    }
    return (
        <>
            {languages.map((lang: any) => (
                <FieldRow key={lang.id} label={`${t('name')} (${lang.shortCode.toUpperCase()})`}>
                    <input
                        type="text"
                        value={data[`name_${lang.shortCode}`] || ''}
                        onChange={e => onChange({ ...data, [`name_${lang.shortCode}`]: e.target.value })}
                        className="input"
                        placeholder={lang.name}
                    />
                </FieldRow>
            ))}
        </>
    )
}

function RegionForm({ data, onChange, readOnly }: { data: Record<string, any>; onChange: (d: Record<string, any>) => void; readOnly: boolean }) {
    const { t } = useApp()
    const { data: langsData } = useQuery({
        queryKey: ['languages'],
        queryFn: () => languagesService.getAll({ pageNumber: 1, pageSize: 10 }),
    })
    const languages = langsData?.items || []
    return (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <TranslateNameFields data={data} onChange={onChange} readOnly={readOnly} languages={languages} />
            <FieldRow label={t('order')}>
                <input type="number" value={data.order || 0} onChange={e => onChange({ ...data, order: parseInt(e.target.value) || 0 })} className="input" readOnly={readOnly} />
            </FieldRow>
        </div>
    )
}

function DistrictForm({ data, onChange, readOnly, parentNode }: { data: Record<string, any>; onChange: (d: Record<string, any>) => void; readOnly: boolean; parentNode: TreeNode | null }) {
    const { t } = useApp()
    const { data: langsData } = useQuery({
        queryKey: ['languages'],
        queryFn: () => languagesService.getAll({ pageNumber: 1, pageSize: 10 }),
    })
    const { data: regionsData } = useQuery({
        queryKey: ['regions', { pageNumber: 1, pageSize: 500 }],
        queryFn: () => regionsService.getAll({ pageNumber: 1, pageSize: 500 }),
        enabled: !readOnly,
    })
    const languages = langsData?.items || []
    return (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <TranslateNameFields data={data} onChange={onChange} readOnly={readOnly} languages={languages} />
            <FieldRow label={t('region')}>
                {readOnly ? (
                    <input type="text" value={data.regionName || parentNode?.name || ''} className="input" readOnly />
                ) : (
                    <select value={data.regionId || ''} onChange={e => onChange({ ...data, regionId: e.target.value })} className="input">
                        <option value="">{t('selectRegion')}</option>
                        {regionsData?.items.map((r: any) => <option key={r.id} value={r.id}>{r.name}</option>)}
                    </select>
                )}
            </FieldRow>
            <FieldRow label={t('order')}>
                <input type="number" value={data.order || 0} onChange={e => onChange({ ...data, order: parseInt(e.target.value) || 0 })} className="input" readOnly={readOnly} />
            </FieldRow>
        </div>
    )
}

function ComplexForm({ data, onChange, readOnly }: { data: Record<string, any>; onChange: (d: Record<string, any>) => void; readOnly: boolean }) {
    const { t } = useApp()
    const { data: regionsData } = useQuery({ queryKey: ['regions', { pageNumber: 1, pageSize: 500 }], queryFn: () => regionsService.getAll({ pageNumber: 1, pageSize: 500 }), enabled: !readOnly })
    const { data: districtsData } = useQuery({ queryKey: ['districts', { pageNumber: 1, pageSize: 500 }], queryFn: () => districtsService.getAll({ pageNumber: 1, pageSize: 500 }), enabled: !readOnly })
    return (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <FieldRow label={t('name') + ' *'}>
                <input type="text" value={data.name || ''} onChange={e => onChange({ ...data, name: e.target.value })} className="input" readOnly={readOnly} />
            </FieldRow>
            <FieldRow label={t('region')}>
                {readOnly ? (
                    <input type="text" value={data.region || ''} className="input" readOnly />
                ) : (
                    <select value={data.regionId || ''} onChange={e => onChange({ ...data, regionId: e.target.value })} className="input">
                        <option value="">{t('selectRegion')}</option>
                        {regionsData?.items.map((r: any) => <option key={r.id} value={r.id}>{r.name}</option>)}
                    </select>
                )}
            </FieldRow>
            <FieldRow label={t('district')}>
                {readOnly ? (
                    <input type="text" value={data.district || ''} className="input" readOnly />
                ) : (
                    <select value={data.districtId || ''} onChange={e => onChange({ ...data, districtId: e.target.value })} className="input">
                        <option value="">{t('selectDistrict')}</option>
                        {districtsData?.items.map((d: any) => <option key={d.id} value={d.id}>{d.name}</option>)}
                    </select>
                )}
            </FieldRow>
            <FieldRow label={t('address')}>
                <input type="text" value={data.address || ''} onChange={e => onChange({ ...data, address: e.target.value })} className="input" readOnly={readOnly} />
            </FieldRow>
            <div className="flex items-center gap-4 col-span-1 md:col-span-2">
                <label className="flex items-center gap-2 cursor-pointer">
                    <input type="checkbox" checked={data.isCommercial || false} onChange={e => onChange({ ...data, isCommercial: e.target.checked })} disabled={readOnly} className="w-4 h-4 rounded border-slate-300 text-blue-600" />
                    <span className="text-sm text-slate-700 dark:text-slate-300">{t('commercial')}</span>
                </label>
                <label className="flex items-center gap-2 cursor-pointer">
                    <input type="checkbox" checked={data.isLiving || false} onChange={e => onChange({ ...data, isLiving: e.target.checked })} disabled={readOnly} className="w-4 h-4 rounded border-slate-300 text-blue-600" />
                    <span className="text-sm text-slate-700 dark:text-slate-300">{t('living')}</span>
                </label>
            </div>
            <div className="md:col-span-2">
                <FieldRow label={t('description')}>
                    <textarea value={data.description || ''} onChange={e => onChange({ ...data, description: e.target.value })} className="input" rows={2} readOnly={readOnly} />
                </FieldRow>
            </div>
            <FieldRow label={t('latitude')}>
                <input type="number" step="any" value={data.latitude || ''} onChange={e => onChange({ ...data, latitude: parseFloat(e.target.value) })} className="input" readOnly={readOnly} />
            </FieldRow>
            <FieldRow label={t('longitude')}>
                <input type="number" step="any" value={data.longitude || ''} onChange={e => onChange({ ...data, longitude: parseFloat(e.target.value) })} className="input" readOnly={readOnly} />
            </FieldRow>
        </div>
    )
}

function BuildingForm({ data, onChange, readOnly }: { data: Record<string, any>; onChange: (d: Record<string, any>) => void; readOnly: boolean }) {
    const { t } = useApp()
    return (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <FieldRow label={t('buildingNumber') + ' *'}>
                <input type="text" value={data.number || ''} onChange={e => onChange({ ...data, number: e.target.value })} className="input" readOnly={readOnly} />
            </FieldRow>
            <FieldRow label={t('floorsCount')}>
                <input type="number" value={data.floorsCount || ''} onChange={e => onChange({ ...data, floorsCount: parseInt(e.target.value) || undefined })} className="input" readOnly={readOnly} />
            </FieldRow>
            <div className="flex items-center gap-4">
                <label className="flex items-center gap-2 cursor-pointer">
                    <input type="checkbox" checked={data.isCommercial || false} onChange={e => onChange({ ...data, isCommercial: e.target.checked })} disabled={readOnly} className="w-4 h-4 rounded border-slate-300 text-blue-600" />
                    <span className="text-sm text-slate-700 dark:text-slate-300">{t('commercial')}</span>
                </label>
                <label className="flex items-center gap-2 cursor-pointer">
                    <input type="checkbox" checked={data.isLiving || false} onChange={e => onChange({ ...data, isLiving: e.target.checked })} disabled={readOnly} className="w-4 h-4 rounded border-slate-300 text-blue-600" />
                    <span className="text-sm text-slate-700 dark:text-slate-300">{t('living')}</span>
                </label>
            </div>
            <FieldRow label={t('totalArea') + ' (m²)'}>
                <input type="number" value={data.totalArea || ''} onChange={e => onChange({ ...data, totalArea: parseFloat(e.target.value) || undefined })} className="input" readOnly={readOnly} />
            </FieldRow>
            <div className="md:col-span-2">
                <FieldRow label={t('address')}>
                    <input type="text" value={data.address || ''} onChange={e => onChange({ ...data, address: e.target.value })} className="input" readOnly={readOnly} />
                </FieldRow>
            </div>
            <FieldRow label={t('latitude')}>
                <input type="number" step="any" value={data.latitude || ''} onChange={e => onChange({ ...data, latitude: parseFloat(e.target.value) || undefined })} className="input" readOnly={readOnly} />
            </FieldRow>
            <FieldRow label={t('longitude')}>
                <input type="number" step="any" value={data.longitude || ''} onChange={e => onChange({ ...data, longitude: parseFloat(e.target.value) || undefined })} className="input" readOnly={readOnly} />
            </FieldRow>
        </div>
    )
}

function FloorForm({ data, onChange, readOnly }: { data: Record<string, any>; onChange: (d: Record<string, any>) => void; readOnly: boolean }) {
    const { t } = useApp()
    return (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <FieldRow label={t('floorNumber') + ' *'}>
                <input type="number" value={data.number || ''} onChange={e => onChange({ ...data, number: parseInt(e.target.value) })} className="input" readOnly={readOnly} />
            </FieldRow>
            <FieldRow label={t('totalArea') + ' (m²)'}>
                <input type="number" step="0.01" value={data.totalArea || ''} onChange={e => onChange({ ...data, totalArea: parseFloat(e.target.value) || undefined })} className="input" readOnly={readOnly} />
            </FieldRow>
            <FieldRow label={t('ceilingHeight') + ' (m)'}>
                <input type="number" step="0.01" value={data.ceilingHeight || ''} onChange={e => onChange({ ...data, ceilingHeight: parseFloat(e.target.value) || undefined })} className="input" readOnly={readOnly} />
            </FieldRow>
            <FieldRow label={t('floorPlan')}>
                <input type="text" value={data.id ? (data.floorPlan || '') : (data.plan || '')} onChange={e => onChange({ ...data, ...(data.id ? { floorPlan: e.target.value } : { plan: e.target.value }) })} className="input" readOnly={readOnly} placeholder={t('floorPlanPlaceholder')} />
            </FieldRow>
            <div className="md:col-span-2">
                <FieldRow label={t('note')}>
                    <textarea value={data.note || ''} onChange={e => onChange({ ...data, note: e.target.value })} className="input min-h-[60px] resize-y" readOnly={readOnly} rows={2} />
                </FieldRow>
            </div>
        </div>
    )
}

// --- Children Table ---
function ChildrenTable({ node, onSelectNode, onAddChild }: { node: TreeNode; onSelectNode: (n: TreeNode) => void; onAddChild: (parent: TreeNode, level: NodeLevel) => void }) {
    const { t } = useApp()
    const nextLevel = childLevelMap[node.level]
    if (!nextLevel) return null

    const params = { pageNumber: 1, pageSize: 500 }

    const { data: districtsData } = useQuery({
        queryKey: ['districts-children', node.id],
        queryFn: () => districtsService.getAll({ ...params, regionId: node.id }),
        enabled: node.level === 'region',
    })

    const { data: complexesData } = useQuery({
        queryKey: ['complexes-children', node.id],
        queryFn: () => complexesService.getAll(params),
        enabled: node.level === 'district',
    })

    const { data: buildingsData } = useQuery({
        queryKey: ['buildings-children', node.id],
        queryFn: () => buildingsService.getAll(params),
        enabled: node.level === 'complex',
    })

    const { data: floorsData } = useQuery({
        queryKey: ['floors-children', node.id],
        queryFn: () => floorsService.getAll({ ...params, buildingId: node.id }),
        enabled: node.level === 'building',
    })

    let children: any[] = []
    let columns: { key: string; label: string }[] = []

    if (node.level === 'region' && districtsData) {
        children = districtsData.items
        columns = [{ key: 'name', label: t('name') }, { key: 'order', label: t('order') }]
    } else if (node.level === 'district' && complexesData) {
        children = complexesData.items.filter((c: any) => c.district === node.name || c.districtId === node.id)
        columns = [{ key: 'name', label: t('name') }, { key: 'address', label: t('address') }, { key: 'isCommercial', label: t('commercial') }, { key: 'isLiving', label: t('living') }]
    } else if (node.level === 'complex' && buildingsData) {
        children = buildingsData.items.filter((b: any) => b.complex === node.name || b.complexId === node.id)
        columns = [{ key: 'number', label: t('buildingNumber') }, { key: 'address', label: t('address') }, { key: 'totalArea', label: t('totalArea') }]
    } else if (node.level === 'building' && floorsData) {
        children = floorsData.items
        columns = [{ key: 'number', label: t('floorNumber') }, { key: 'totalArea', label: t('totalArea') }, { key: 'ceilingHeight', label: t('ceilingHeight') }]
    }

    const childLabel = levelLabels[nextLevel].en

    const makeTreeNode = (item: any): TreeNode => ({
        id: item.id,
        name: item.name || item.number || `${childLabel} ${item.number || item.id?.slice(0, 6)}`,
        level: nextLevel,
        parentId: node.id,
        data: item,
    })

    return (
        <div className="mt-6">
            <div className="flex items-center justify-between mb-3">
                <h3 className="text-sm font-semibold text-slate-800 dark:text-slate-200">
                    {childLabel}s ({children.length})
                </h3>
                <button
                    onClick={() => onAddChild(node, nextLevel)}
                    className="flex items-center gap-1 text-xs font-medium text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300 px-2 py-1 rounded-md hover:bg-blue-50 dark:hover:bg-blue-900/20 transition-colors"
                >
                    <svg width="12" height="12" viewBox="0 0 24 24" fill="none"><path d="M12 5V19M5 12H19" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" /></svg>
                    Add {childLabel}
                </button>
            </div>

            {children.length > 0 ? (
                <div className="border border-slate-200 dark:border-slate-700 rounded-lg overflow-hidden">
                    <table className="w-full text-sm">
                        <thead>
                            <tr className="bg-slate-50 dark:bg-slate-800/50 border-b border-slate-200 dark:border-slate-700">
                                {columns.map(col => (
                                    <th key={col.key} className="text-left px-3 py-2 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">{col.label}</th>
                                ))}
                            </tr>
                        </thead>
                        <tbody>
                            {children.map((item: any, idx: number) => (
                                <tr
                                    key={item.id || idx}
                                    onClick={() => onSelectNode(makeTreeNode(item))}
                                    className="border-b border-slate-100 dark:border-slate-800 last:border-0 hover:bg-blue-50/50 dark:hover:bg-blue-900/10 cursor-pointer transition-colors"
                                >
                                    {columns.map(col => (
                                        <td key={col.key} className="px-3 py-2.5 text-slate-700 dark:text-slate-300">
                                            {typeof item[col.key] === 'boolean' ? (
                                                <span className={`inline-flex px-1.5 py-0.5 rounded text-xs font-medium ${item[col.key] ? 'bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400' : 'bg-slate-100 text-slate-500 dark:bg-slate-800 dark:text-slate-500'}`}>
                                                    {item[col.key] ? '✓' : '—'}
                                                </span>
                                            ) : (
                                                item[col.key] ?? '—'
                                            )}
                                        </td>
                                    ))}
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            ) : (
                <div className="text-center py-6 bg-slate-50 dark:bg-slate-800/30 rounded-lg border border-dashed border-slate-200 dark:border-slate-700">
                    <p className="text-sm text-slate-400 dark:text-slate-500 mb-2">No {childLabel.toLowerCase()}s yet</p>
                    <button
                        onClick={() => onAddChild(node, nextLevel)}
                        className="text-xs font-medium text-blue-600 dark:text-blue-400 hover:underline"
                    >
                        + Create first {childLabel.toLowerCase()}
                    </button>
                </div>
            )}
        </div>
    )
}

// --- Icons ---
const EditIcon = () => (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none"><path d="M11 4H4C3.47 4 2.96 4.21 2.59 4.59C2.21 4.96 2 5.47 2 6V20C2 20.53 2.21 21.04 2.59 21.41C2.96 21.79 3.47 22 4 22H18C18.53 22 19.04 21.79 19.41 21.41C19.79 21.04 20 20.53 20 20V13" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" /><path d="M18.5 2.5C18.9 2.1 19.44 1.88 20 1.88C20.56 1.88 21.1 2.1 21.5 2.5C21.9 2.9 22.12 3.44 22.12 4C22.12 4.56 21.9 5.1 21.5 5.5L12 15L8 16L9 12L18.5 2.5Z" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" /></svg>
)
const TrashIcon = () => (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none"><path d="M3 6H5H21" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" /><path d="M8 6V4C8 3.47 8.21 2.96 8.59 2.59C8.96 2.21 9.47 2 10 2H14C14.53 2 15.04 2.21 15.41 2.59C15.79 2.96 16 3.47 16 4V6M19 6V20C19 20.53 18.79 21.04 18.41 21.41C18.04 21.79 17.53 22 17 22H7C6.47 22 5.96 21.79 5.59 21.41C5.21 21.04 5 20.53 5 20V6H19Z" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" /></svg>
)

// --- Main Component ---
export default function HierarchyDetailPanel({
    node, mode, createLevel, parentNode, formData, onFormChange,
    onEdit, onDelete, onSave, onCancel, onSelectNode, onAddChild, isSaving, isDeleting,
}: DetailPanelProps) {
    const { t, language } = useApp()
    const [showDeleteModal, setShowDeleteModal] = useState(false)

    const activeLevel = mode === 'create' ? createLevel : node?.level
    if (!activeLevel) {
        return (
            <div className="h-full flex items-center justify-center">
                <div className="text-center">
                    <div className="mb-3"><svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.2" strokeLinecap="round" strokeLinejoin="round" className="mx-auto text-slate-400"><path d="M20 10c0 6-8 12-8 12s-8-6-8-12a8 8 0 0 1 16 0Z" /><circle cx="12" cy="10" r="3" /></svg></div>
                    <p className="text-slate-500 dark:text-slate-400 text-sm">Select a location from the tree to view details</p>
                    <p className="text-slate-400 dark:text-slate-500 text-xs mt-1">Or click + to add a new item</p>
                </div>
            </div>
        )
    }

    const readOnly = mode === 'view'
    const label = levelLabels[activeLevel][language] || levelLabels[activeLevel].en
    const title = mode === 'create' ? `${t('add')} ${label}` : mode === 'edit' ? `${t('edit')} ${label}` : node?.name || ''

    const renderForm = () => {
        const props = { data: formData, onChange: onFormChange, readOnly }
        switch (activeLevel) {
            case 'region': return <RegionForm {...props} />
            case 'district': return <DistrictForm {...props} parentNode={parentNode} />
            case 'complex': return <ComplexForm {...props} />
            case 'building': return <BuildingForm {...props} />
            case 'floor': return <FloorForm {...props} />
        }
    }

    return (
        <div className="h-full overflow-y-auto">
            <Breadcrumb node={mode === 'view' || mode === 'edit' ? node : null} parentNode={parentNode} createLevel={mode === 'create' ? createLevel : null} />

            {/* Header */}
            <div className="flex items-center justify-between mb-4">
                <h2 className="text-lg font-semibold text-slate-900 dark:text-white">{title}</h2>
                {mode === 'view' && node && (
                    <div className="flex items-center gap-2">
                        <button onClick={onEdit} className="btn btn-secondary flex items-center gap-1.5 text-sm py-1.5 px-3">
                            <EditIcon /> {t('edit')}
                        </button>
                        <button onClick={() => setShowDeleteModal(true)} className="btn btn-danger flex items-center gap-1.5 text-sm py-1.5 px-3">
                            <TrashIcon /> {t('delete')}
                        </button>
                    </div>
                )}
            </div>

            {/* Form */}
            <div className="card p-4 mb-4">
                {renderForm()}
            </div>

            {/* Action buttons for edit/create */}
            {(mode === 'edit' || mode === 'create') && (
                <div className="flex justify-end gap-3 mb-4">
                    <button onClick={onCancel} className="btn btn-secondary">{t('cancel')}</button>
                    <button onClick={onSave} className="btn btn-primary" disabled={isSaving}>
                        {isSaving ? t('loading') : t('save')}
                    </button>
                </div>
            )}

            {/* Children table - only in view mode */}
            {mode === 'view' && node && childLevelMap[node.level] && (
                <ChildrenTable node={node} onSelectNode={onSelectNode} onAddChild={onAddChild} />
            )}

            <DeleteModal
                isOpen={showDeleteModal}
                onClose={() => setShowDeleteModal(false)}
                onConfirm={() => { setShowDeleteModal(false); onDelete() }}
                isLoading={isDeleting}
                itemName={node?.name}
            />
        </div>
    )
}
