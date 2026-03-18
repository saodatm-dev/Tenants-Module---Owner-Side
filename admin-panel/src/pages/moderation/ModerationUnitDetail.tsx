import { useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useApp } from '../../context/AppContext'
import { useModerationUnitDetail, useModerationActions } from '../../hooks/useModeration'
import ModerationStatusBadge from '../../components/moderation/ModerationStatusBadge'
import ImageGallery from '../../components/moderation/ImageGallery'
import { ConfirmAcceptDialog, ReasonDialog } from '../../components/moderation/ModerationActionDialogs'
import { ArrowLeft, Check, X, Ban, Ruler, FileText, Loader2, AlertTriangle, Layers, Box } from 'lucide-react'
import OwnerCard from '../../components/moderation/OwnerCard'
import { toast } from 'sonner'

export default function ModerationUnitDetail() {
    const { id } = useParams<{ id: string }>()
    const navigate = useNavigate()
    const { t } = useApp()
    const { data, isLoading, isError } = useModerationUnitDetail(id!)

    const [showAccept, setShowAccept] = useState(false)
    const [showReject, setShowReject] = useState(false)
    const [showBlock, setShowBlock] = useState(false)

    const actions = useModerationActions('units', {
        onSuccess: () => {
            setShowAccept(false)
            setShowReject(false)
            setShowBlock(false)
        },
    })

    if (isLoading) {
        return (
            <div className="flex items-center justify-center py-32">
                <div className="text-center">
                    <Loader2 size={36} className="animate-spin text-primary-500 mx-auto" />
                    <p className="mt-4 text-sm text-slate-400">{t('loading')}</p>
                </div>
            </div>
        )
    }

    if (isError || !data) {
        return (
            <div className="text-center py-32">
                <AlertTriangle size={48} className="mx-auto text-red-400 mb-4" />
                <p className="text-lg font-medium text-slate-600 dark:text-slate-400">{t('error')}</p>
                <button onClick={() => navigate('/moderation')} className="btn btn-secondary mt-4">{t('back')}</button>
            </div>
        )
    }

    const isPending = data.moderationStatus === 1

    return (
        <div className="max-w-5xl mx-auto pb-8">
            {/* Header */}
            <div className="flex items-start justify-between mb-6">
                <div className="flex items-start gap-4">
                    <button onClick={() => navigate('/moderation')} className="mt-1 p-2 rounded-xl hover:bg-slate-100 dark:hover:bg-slate-800 transition-colors">
                        <ArrowLeft size={20} className="text-slate-600 dark:text-slate-400" />
                    </button>
                    <div>
                        <div className="flex items-center gap-3 mb-1">
                            <h1 className="text-2xl font-bold text-slate-900 dark:text-white">{data.type || t('units')}</h1>
                            <ModerationStatusBadge status={data.moderationStatus} />
                        </div>
                        <p className="text-sm text-slate-400 font-mono">ID: {data.id}</p>
                    </div>
                </div>
                {isPending && (
                    <div className="flex items-center gap-2">
                        <button onClick={() => setShowAccept(true)} className="btn btn-success flex items-center gap-2 text-sm shadow-sm"><Check size={16} />{t('accept')}</button>
                        <button onClick={() => setShowReject(true)} className="btn btn-danger flex items-center gap-2 text-sm shadow-sm"><X size={16} />{t('reject')}</button>
                        <button onClick={() => setShowBlock(true)} className="btn btn-secondary flex items-center gap-2 text-sm shadow-sm"><Ban size={16} />{t('block')}</button>
                    </div>
                )}
            </div>

            {/* Image Gallery */}
            {data.images && data.images.length > 0 && (
                <div className="mb-6">
                    <ImageGallery images={data.images} />
                </div>
            )}

            {/* Info Cards */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-5 mb-6">
                {/* Unit Info */}
                <div className="card p-6">
                    <h3 className="text-xs font-bold text-slate-400 uppercase tracking-widest mb-5 flex items-center gap-2">
                        <Box size={14} />
                        {t('propertyInfo')}
                    </h3>
                    <div className="space-y-4">
                        <InfoRow icon={<FileText size={16} />} label={t('type')} value={data.type} />
                        {data.room && <InfoRow icon={<FileText size={16} />} label={t('room')} value={data.room} />}
                        {data.floor != null && <InfoRow icon={<Layers size={16} />} label={t('floor')} value={`${data.floor}`} />}
                        {data.totalArea != null && <InfoRow icon={<Ruler size={16} />} label={t('totalArea')} value={`${data.totalArea} m²`} />}
                        {data.ceilingHeight != null && <InfoRow icon={<Ruler size={16} />} label={t('ceilingHeight')} value={`${data.ceilingHeight} m`} />}
                    </div>
                </div>

                {/* Owner Info */}
                <OwnerCard ownerId={data.ownerId} />
            </div>

            {/* Floor Plan */}
            {data.plan && (
                <div className="card p-6 mb-6">
                    <h3 className="text-xs font-bold text-slate-400 uppercase tracking-widest mb-4 flex items-center gap-2">
                        <Layers size={14} />
                        {t('floorPlanView')}
                    </h3>
                    <div className="rounded-xl overflow-hidden border border-slate-200 dark:border-slate-700 bg-slate-50 dark:bg-slate-800">
                        <img src={data.plan} alt="Floor Plan" className="w-full max-h-[500px] object-contain" />
                    </div>
                </div>
            )}

            {/* Dialogs */}
            <ConfirmAcceptDialog open={showAccept} onConfirm={() => actions.acceptMutation.mutate(id!, { onSuccess: () => toast.success(t('accepted')), onError: () => toast.error(t('error')) })} onCancel={() => setShowAccept(false)} loading={actions.acceptMutation.isPending} />
            <ReasonDialog open={showReject} type="reject" onConfirm={(reason) => actions.rejectMutation.mutate({ id: id!, reason }, { onSuccess: () => toast.success(t('rejectedMsg')), onError: () => toast.error(t('error')) })} onCancel={() => setShowReject(false)} loading={actions.rejectMutation.isPending} />
            <ReasonDialog open={showBlock} type="block" onConfirm={(reason) => actions.blockMutation.mutate({ id: id!, reason }, { onSuccess: () => toast.success(t('blockedMsg')), onError: () => toast.error(t('error')) })} onCancel={() => setShowBlock(false)} loading={actions.blockMutation.isPending} />
        </div>
    )
}

function InfoRow({ icon, label, value }: { icon: React.ReactNode; label: string; value: string }) {
    return (
        <div className="flex items-center gap-3">
            <span className="text-slate-400">{icon}</span>
            <span className="text-sm text-slate-500 min-w-[120px]">{label}</span>
            <span className="text-sm text-slate-800 dark:text-slate-200 font-medium">{value}</span>
        </div>
    )
}
