import { useState } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { useApp } from '../context/AppContext'
import { showToast } from '../components/common'
import {
    regionsService, districtsService, complexesService,
    buildingsService, floorsService,
} from '../services/api'
import HierarchyTreeSidebar from '../components/HierarchyTreeSidebar'
import HierarchyDetailPanel from '../components/HierarchyDetailPanel'
import type { TreeNode, NodeLevel } from '../components/HierarchyTreeSidebar'

export default function HierarchyPage() {
    const { t } = useApp()
    const queryClient = useQueryClient()

    // --- State ---
    const [selectedNode, setSelectedNode] = useState<TreeNode | null>(null)
    const [mode, setMode] = useState<'view' | 'edit' | 'create'>('view')
    const [createLevel, setCreateLevel] = useState<NodeLevel | null>(null)
    const [parentNode, setParentNode] = useState<TreeNode | null>(null)
    const [formData, setFormData] = useState<Record<string, any>>({})

    // --- Service map ---
    const serviceMap: Record<NodeLevel, any> = {
        region: regionsService,
        district: districtsService,
        complex: complexesService,
        building: buildingsService,
        floor: floorsService,
    }

    // --- Mutations ---
    const createMut = useMutation({
        mutationFn: async ({ level, data }: { level: NodeLevel; data: any }) => {
            const svc = serviceMap[level]
            return svc.create(data)
        },
        onSuccess: (_: any, vars: { level: NodeLevel; data: any }) => {
            invalidateLevel(vars.level)
            showToast.success(t('success'), t('item_created'))
            setMode('view')
            setCreateLevel(null)
        },
        onError: (e: Error) => showToast.error(t('error'), e.message),
    })

    const updateMut = useMutation({
        mutationFn: async ({ level, data }: { level: NodeLevel; data: any }) => {
            const svc = serviceMap[level]
            return svc.update(data)
        },
        onSuccess: (_: any, vars: { level: NodeLevel; data: any }) => {
            invalidateLevel(vars.level)
            showToast.success(t('success'), t('item_updated'))
            setMode('view')
        },
        onError: (e: Error) => showToast.error(t('error'), e.message),
    })

    const deleteMut = useMutation({
        mutationFn: async ({ level, id }: { level: NodeLevel; id: string }) => {
            const svc = serviceMap[level]
            return svc.remove ? svc.remove(id) : svc.delete(id)
        },
        onSuccess: (_: any, vars: { level: NodeLevel; id: string }) => {
            invalidateLevel(vars.level)
            showToast.success(t('success'), t('item_deleted'))
            setSelectedNode(null)
            setMode('view')
        },
        onError: (e: Error) => showToast.error(t('error'), e.message),
    })

    const invalidateLevel = (level: NodeLevel) => {
        const keys: Record<NodeLevel, string> = {
            region: 'regions',
            district: 'districts',
            complex: 'complexes',
            building: 'buildings',
            floor: 'floors',
        }
        queryClient.invalidateQueries({ queryKey: [keys[level]] })
        // Also invalidate tree
        queryClient.invalidateQueries({ queryKey: ['hierarchy-tree'] })
    }

    // --- Handlers ---
    const handleSelectNode = async (node: TreeNode) => {
        setSelectedNode(node)
        setMode('view')
        setCreateLevel(null)
        setParentNode(null)

        // Fetch full item details to populate form
        try {
            const svc = serviceMap[node.level]
            if (svc.getById) {
                const detail = await svc.getById(node.id)
                const fd: Record<string, any> = { ...detail }

                // Map translation fields for regions/districts
                if (detail.translates && Array.isArray(detail.translates)) {
                    for (const tr of detail.translates) {
                        fd[`name_${tr.languageShortCode}`] = tr.value
                    }
                }

                setFormData(fd)
            } else {
                setFormData(node.data || {})
            }
        } catch {
            setFormData(node.data || {})
        }
    }

    const handleAddChild = (parent: TreeNode | null, childLevel: NodeLevel) => {
        setParentNode(parent)
        setCreateLevel(childLevel)
        setSelectedNode(null)
        setMode('create')
        setFormData({})
    }

    const handleEdit = () => {
        setMode('edit')
    }

    const handleCancel = () => {
        if (selectedNode) {
            setMode('view')
            handleSelectNode(selectedNode)
        } else {
            setMode('view')
            setCreateLevel(null)
            setFormData({})
        }
    }

    const handleDelete = () => {
        if (!selectedNode) return
        deleteMut.mutate({ level: selectedNode.level, id: selectedNode.id })
    }

    const handleSave = async () => {
        const level = mode === 'create' ? createLevel : selectedNode?.level
        if (!level) return

        try {
            const payload = buildPayload(level, formData)
            if (mode === 'create') {
                await createMut.mutateAsync({ level, data: payload })
            } else if (mode === 'edit' && selectedNode) {
                await updateMut.mutateAsync({ level, data: { ...payload, id: selectedNode.id } })
            }
        } catch {
            // Error handled by mutation
        }
    }

    // --- Payload builders ---
    const buildPayload = (level: NodeLevel, fd: Record<string, any>) => {
        switch (level) {
            case 'region':
                return { translates: buildTranslates(fd) }
            case 'district':
                return {
                    regionId: fd.regionId || parentNode?.id,
                    translates: buildTranslates(fd),
                }
            case 'complex':
                return {
                    name: fd.name,
                    regionId: fd.regionId,
                    districtId: fd.districtId || parentNode?.id,
                    descriptions: buildDescriptions(fd),
                    isCommercial: fd.isCommercial || false,
                    isLiving: fd.isLiving || false,
                    address: fd.address || undefined,
                    images: fd.images || [],
                }
            case 'building':
                return {
                    number: fd.number,
                    complexId: fd.complexId || parentNode?.id,
                    totalArea: fd.totalArea ? parseFloat(fd.totalArea) : undefined,
                    floorsCount: fd.floorsCount ? parseInt(fd.floorsCount) : undefined,
                    address: fd.address,
                    isCommercial: fd.isCommercial || false,
                    isLiving: fd.isLiving || false,
                    images: fd.images || [],
                }
            case 'floor':
                return {
                    buildingId: fd.buildingId || parentNode?.id,
                    number: parseInt(fd.number) || 0,
                    type: parseInt(fd.type) || 0,
                    label: fd.label || undefined,
                    totalArea: fd.totalArea ? parseFloat(fd.totalArea) : undefined,
                    ceilingHeight: fd.ceilingHeight ? parseFloat(fd.ceilingHeight) : undefined,
                    plan: fd.plan || undefined,
                }
            default:
                return fd
        }
    }

    const buildTranslates = (fd: Record<string, any>) => {
        const translates: any[] = []
        // We'll get language data from the form or use defaults
        for (const code of ['en', 'uz', 'ru']) {
            if (fd[`name_${code}`] !== undefined) {
                // Try to find language ID from cached data
                const langId = fd[`langId_${code}`] || ''
                translates.push({
                    languageId: langId,
                    languageShortCode: code,
                    value: fd[`name_${code}`] || '',
                })
            }
        }
        return translates
    }

    const buildDescriptions = (fd: Record<string, any>) => {
        const descriptions: any[] = []
        for (const code of ['en', 'uz', 'ru']) {
            if (fd[`desc_${code}`] !== undefined) {
                const langId = fd[`langId_${code}`] || ''
                descriptions.push({
                    languageId: langId,
                    languageShortCode: code,
                    value: fd[`desc_${code}`] || '',
                })
            }
        }
        return descriptions
    }

    return (
        <div className="h-[calc(100vh-4rem)]">
            <div className="flex h-full gap-4">
                {/* Tree Sidebar */}
                <div className="w-80 flex-shrink-0">
                    <HierarchyTreeSidebar
                        selectedNode={selectedNode}
                        onSelectNode={handleSelectNode}
                        onAddChild={handleAddChild}
                    />
                </div>

                {/* Detail Panel */}
                <div className="flex-1 overflow-hidden">
                    <div className="card h-full p-4 overflow-y-auto">
                        <HierarchyDetailPanel
                            node={selectedNode}
                            mode={mode}
                            createLevel={createLevel}
                            parentNode={parentNode}
                            formData={formData}
                            onFormChange={setFormData}
                            onEdit={handleEdit}
                            onDelete={handleDelete}
                            onSave={handleSave}
                            onCancel={handleCancel}
                            onSelectNode={handleSelectNode}
                            onAddChild={handleAddChild}
                            isSaving={createMut.isPending || updateMut.isPending}
                            isDeleting={deleteMut.isPending}
                        />
                    </div>
                </div>
            </div>
        </div>
    )
}
