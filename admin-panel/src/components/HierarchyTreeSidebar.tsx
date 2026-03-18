import { useState, useEffect } from 'react'
import { useQuery } from '@tanstack/react-query'
import { useApp } from '../context/AppContext'
import { regionsService, districtsService, complexesService, buildingsService, floorsService } from '../services/api'
import clsx from 'clsx'

export type NodeLevel = 'region' | 'district' | 'complex' | 'building' | 'floor'

export interface TreeNode {
    id: string
    name: string
    level: NodeLevel
    parentId?: string
    data: any
}

interface HierarchyTreeSidebarProps {
    selectedNode: TreeNode | null
    onSelectNode: (node: TreeNode) => void
    onAddChild: (parentNode: TreeNode | null, childLevel: NodeLevel) => void
}

const ChevronIcon = ({ expanded }: { expanded: boolean }) => (
    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" className={clsx('transition-transform duration-200', expanded ? 'rotate-90' : 'rotate-0')}>
        <path d="M9 18L15 12L9 6" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
    </svg>
)

const PlusIcon = () => (
    <svg width="14" height="14" viewBox="0 0 24 24" fill="none">
        <path d="M12 5V19M5 12H19" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
    </svg>
)

const RegionSvg = () => (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
        <circle cx="12" cy="12" r="10" /><path d="M2 12h20" /><path d="M12 2a15.3 15.3 0 0 1 4 10 15.3 15.3 0 0 1-4 10 15.3 15.3 0 0 1-4-10 15.3 15.3 0 0 1 4-10z" />
    </svg>
)
const DistrictSvg = () => (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
        <path d="M20 10c0 6-8 12-8 12s-8-6-8-12a8 8 0 0 1 16 0Z" /><circle cx="12" cy="10" r="3" />
    </svg>
)
const ComplexSvg = () => (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
        <rect x="1" y="6" width="10" height="16" rx="1" /><rect x="13" y="2" width="10" height="20" rx="1" /><path d="M5 10h2M5 14h2M5 18h2M17 6h2M17 10h2M17 14h2M17 18h2" />
    </svg>
)
const BuildingSvg = () => (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
        <rect x="4" y="2" width="16" height="20" rx="2" /><path d="M9 22V12h6v10" /><path d="M8 6h.01M16 6h.01M12 6h.01M8 10h.01M16 10h.01M12 10h.01" />
    </svg>
)
const FloorSvg = () => (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
        <path d="M12 2 2 7l10 5 10-5-10-5z" /><path d="M2 17l10 5 10-5" /><path d="M2 12l10 5 10-5" />
    </svg>
)

const levelIcons: Record<NodeLevel, React.ReactNode> = {
    region: <RegionSvg />,
    district: <DistrictSvg />,
    complex: <ComplexSvg />,
    building: <BuildingSvg />,
    floor: <FloorSvg />,
}

const childLevel: Record<NodeLevel, NodeLevel | null> = {
    region: 'district',
    district: 'complex',
    complex: 'building',
    building: 'floor',
    floor: null,
}

function TreeNodeItem({
    node,
    depth,
    selectedNode,
    onSelectNode,
    onAddChild,
}: {
    node: TreeNode
    depth: number
    selectedNode: TreeNode | null
    onSelectNode: (node: TreeNode) => void
    onAddChild: (parentNode: TreeNode, childLevel: NodeLevel) => void
}) {
    const [expanded, setExpanded] = useState(false)
    const [children, setChildren] = useState<TreeNode[]>([])
    const [loaded, setLoaded] = useState(false)
    const isSelected = selectedNode?.id === node.id && selectedNode?.level === node.level
    const nextLevel = childLevel[node.level]
    const hasChildren = nextLevel !== null

    // Auto-expand if a descendant is selected
    useEffect(() => {
        if (selectedNode && !expanded && loaded) {
            // Check if selected node is a child
            const isChild = children.some(c => c.id === selectedNode.id && c.level === selectedNode.level)
            if (isChild) setExpanded(true)
        }
    }, [selectedNode, children])

    const fetchChildren = async () => {
        if (!nextLevel || loaded) return
        try {
            let items: TreeNode[] = []
            const params = { pageNumber: 1, pageSize: 500 }
            if (nextLevel === 'district') {
                // Use RegionId server-side filter
                const districts = await districtsService.getAll({ ...params, regionId: node.id })
                items = districts.items
                    .map((d: any) => ({ id: d.id, name: d.name || d.nameEn || d.nameUz || d.nameRu || d.id, level: 'district' as NodeLevel, parentId: node.id, data: d }))
            } else if (nextLevel === 'complex') {
                const allComplexes = await complexesService.getAll(params)
                // API returns `district` as a string name, not districtId
                // Match by district name OR districtId if available
                const districtName = node.name
                items = allComplexes.items
                    .filter((c: any) => c.district === districtName || c.districtId === node.id)
                    .map((c: any) => ({ id: c.id, name: c.name || c.id, level: 'complex' as NodeLevel, parentId: node.id, data: c }))
            } else if (nextLevel === 'building') {
                const allBuildings = await buildingsService.getAll(params)
                // API returns `complex` as a string name
                // Match by complex name OR complexId if available
                const complexName = node.name
                items = allBuildings.items
                    .filter((b: any) => b.complex === complexName || b.complexId === node.id)
                    .map((b: any) => ({ id: b.id, name: b.number || b.id, level: 'building' as NodeLevel, parentId: node.id, data: b }))
            } else if (nextLevel === 'floor') {
                const allFloors = await floorsService.getAll({ ...params, buildingId: node.id })
                items = allFloors.items.map((f: any) => ({ id: f.id, name: `Floor ${f.number}`, level: 'floor' as NodeLevel, parentId: node.id, data: f }))
            }
            setChildren(items)
            setLoaded(true)
        } catch (err) {
            console.error('Failed to load children:', err)
            setLoaded(true)
        }
    }

    const handleToggle = (e: React.MouseEvent) => {
        e.stopPropagation()
        if (!hasChildren) return
        if (!loaded) fetchChildren()
        setExpanded(!expanded)
    }

    const handleSelect = () => {
        if (!loaded && hasChildren) fetchChildren()
        if (!expanded && hasChildren) setExpanded(true)
        onSelectNode(node)
    }

    // Expose a way to refresh children from parent
    useEffect(() => {
        if (expanded && loaded) {
            // Auto-refresh when node is expanded (re-fetch periodically is not needed, 
            // but we could add invalidation later)
        }
    }, [expanded])

    return (
        <div>
            <div
                className={clsx(
                    'group flex items-center gap-1.5 py-1.5 px-2 rounded-lg cursor-pointer transition-all duration-150 text-sm',
                    isSelected
                        ? 'bg-blue-50 text-blue-700 dark:bg-blue-900/30 dark:text-blue-300 font-medium'
                        : 'text-slate-700 dark:text-slate-300 hover:bg-slate-100 dark:hover:bg-slate-800'
                )}
                style={{ paddingLeft: `${depth * 16 + 8}px` }}
                onClick={handleSelect}
            >
                {/* Expand/collapse */}
                {hasChildren ? (
                    <button onClick={handleToggle} className="p-0.5 rounded hover:bg-slate-200 dark:hover:bg-slate-700 shrink-0">
                        <ChevronIcon expanded={expanded} />
                    </button>
                ) : (
                    <span className="w-[18px] shrink-0" />
                )}

                {/* Icon */}
                <span className="text-sm shrink-0">{levelIcons[node.level]}</span>

                {/* Name */}
                <span className="truncate flex-1">{node.name}</span>

                {/* Add child button */}
                {nextLevel && (
                    <button
                        onClick={(e) => { e.stopPropagation(); onAddChild(node, nextLevel) }}
                        className="opacity-0 group-hover:opacity-100 p-0.5 rounded hover:bg-slate-200 dark:hover:bg-slate-700 text-slate-500 dark:text-slate-400 transition-opacity shrink-0"
                        title={`Add ${nextLevel}`}
                    >
                        <PlusIcon />
                    </button>
                )}
            </div>

            {/* Children */}
            {expanded && (
                <div className="relative">
                    {/* Indent line */}
                    <div
                        className="absolute top-0 bottom-0 border-l border-slate-200 dark:border-slate-700"
                        style={{ left: `${depth * 16 + 20}px` }}
                    />
                    {children.length > 0 ? (
                        children.map(child => (
                            <TreeNodeItem
                                key={`${child.level}-${child.id}`}
                                node={child}
                                depth={depth + 1}
                                selectedNode={selectedNode}
                                onSelectNode={onSelectNode}
                                onAddChild={onAddChild}
                            />
                        ))
                    ) : loaded ? (
                        <div className="text-xs text-slate-400 dark:text-slate-500 py-1" style={{ paddingLeft: `${(depth + 1) * 16 + 28}px` }}>
                            No {nextLevel}s found
                        </div>
                    ) : (
                        <div className="text-xs text-slate-400 py-1" style={{ paddingLeft: `${(depth + 1) * 16 + 28}px` }}>
                            Loading...
                        </div>
                    )}
                </div>
            )}
        </div>
    )
}

export default function HierarchyTreeSidebar({ selectedNode, onSelectNode, onAddChild }: HierarchyTreeSidebarProps) {
    const { t } = useApp()
    const [searchQuery, setSearchQuery] = useState('')

    const { data: regionsData, isLoading } = useQuery({
        queryKey: ['regions', { pageNumber: 1, pageSize: 500 }],
        queryFn: () => regionsService.getAll({ pageNumber: 1, pageSize: 500 }),
    })

    const regionNodes: TreeNode[] = (regionsData?.items || []).map((r: any) => ({
        id: r.id,
        name: r.name || r.id,
        level: 'region' as NodeLevel,
        data: r,
    }))

    const filteredNodes = searchQuery
        ? regionNodes.filter(n => n.name.toLowerCase().includes(searchQuery.toLowerCase()))
        : regionNodes

    return (
        <div className="h-full flex flex-col bg-white dark:bg-slate-900 border-r border-slate-200 dark:border-slate-800">
            {/* Header */}
            <div className="p-3 border-b border-slate-200 dark:border-slate-800">
                <h2 className="text-sm font-semibold text-slate-800 dark:text-slate-200 mb-2 flex items-center gap-2">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round"><path d="M20 10c0 6-8 12-8 12s-8-6-8-12a8 8 0 0 1 16 0Z" /><circle cx="12" cy="10" r="3" /></svg> {t('hierarchy')}
                </h2>
                <input
                    type="text"
                    placeholder={t('search') + '...'}
                    value={searchQuery}
                    onChange={e => setSearchQuery(e.target.value)}
                    className="w-full px-2.5 py-1.5 text-sm rounded-lg border border-slate-200 dark:border-slate-700 bg-slate-50 dark:bg-slate-800 text-slate-700 dark:text-slate-300 focus:outline-none focus:ring-1 focus:ring-blue-500"
                />
            </div>

            {/* Tree */}
            <div className="flex-1 overflow-y-auto py-2 px-1">
                {isLoading ? (
                    <div className="flex items-center justify-center py-8">
                        <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-blue-600" />
                    </div>
                ) : filteredNodes.length > 0 ? (
                    filteredNodes.map(node => (
                        <TreeNodeItem
                            key={node.id}
                            node={node}
                            depth={0}
                            selectedNode={selectedNode}
                            onSelectNode={onSelectNode}
                            onAddChild={onAddChild}
                        />
                    ))
                ) : (
                    <p className="text-sm text-slate-400 dark:text-slate-500 text-center py-4">{t('noData')}</p>
                )}
            </div>

            {/* Add root region button */}
            <div className="p-2 border-t border-slate-200 dark:border-slate-800">
                <button
                    onClick={() => onAddChild(null, 'region')}
                    className="w-full flex items-center justify-center gap-1.5 py-2 text-sm text-blue-600 dark:text-blue-400 hover:bg-blue-50 dark:hover:bg-blue-900/20 rounded-lg transition-colors"
                >
                    <PlusIcon /> {t('add')} {t('regions')}
                </button>
            </div>
        </div>
    )
}
