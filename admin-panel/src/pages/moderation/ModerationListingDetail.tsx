import { useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useApp } from '../../context/AppContext'
import { useModerationListingDetail, useModerationActions } from '../../hooks/useModeration'
import ModerationStatusBadge from '../../components/moderation/ModerationStatusBadge'
import ImageGallery from '../../components/moderation/ImageGallery'
import { ConfirmAcceptDialog, ReasonDialog } from '../../components/moderation/ModerationActionDialogs'
import { ArrowLeft, Check, X, Ban, MapPin, Ruler, DoorOpen, Banknote, Loader2, AlertTriangle, Clock, Tag, Building2, Layers } from 'lucide-react'
import OwnerCard from '../../components/moderation/OwnerCard'
import { toast } from 'sonner'

export default function ModerationListingDetail() {
    const { id } = useParams<{ id: string }>()
    const navigate = useNavigate()
    const { t } = useApp()
    const { data, isLoading, isError } = useModerationListingDetail(id!)

    const [showAccept, setShowAccept] = useState(false)
    const [showReject, setShowReject] = useState(false)
    const [showBlock, setShowBlock] = useState(false)

    const actions = useModerationActions('listings', {
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
                            <h1 className="text-2xl font-bold text-slate-900 dark:text-white">{data.complex || data.address || t('listings')}</h1>
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

            {/* Reject Reason Banner */}
            {data.reason && (
                <div className="rounded-xl p-4 mb-6 bg-red-50 dark:bg-red-950/30 border border-red-200 dark:border-red-900/50">
                    <div className="flex items-start gap-3">
                        <div className="w-8 h-8 rounded-lg bg-red-100 dark:bg-red-900/40 flex items-center justify-center flex-shrink-0 mt-0.5">
                            <AlertTriangle size={16} className="text-red-600 dark:text-red-400" />
                        </div>
                        <div>
                            <p className="text-sm font-semibold text-red-700 dark:text-red-400">{t('reason')}</p>
                            <p className="text-sm text-red-600 dark:text-red-300 mt-1">{data.reason}</p>
                        </div>
                    </div>
                </div>
            )}

            {/* Image Gallery */}
            {data.objectNames && data.objectNames.length > 0 && (
                <div className="mb-6">
                    <ImageGallery images={data.objectNames} />
                </div>
            )}

            {/* Info Cards */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-5 mb-6">
                {/* Property Info */}
                <div className="card p-6">
                    <h3 className="text-xs font-bold text-slate-400 uppercase tracking-widest mb-5 flex items-center gap-2">
                        <Building2 size={14} />
                        {t('propertyInfo')}
                    </h3>
                    <div className="space-y-4">
                        <InfoRow icon={<Ruler size={16} />} label={t('totalArea')} value={`${data.totalArea} m²`} />
                        {data.livingArea != null && <InfoRow icon={<Ruler size={16} />} label={t('livingArea')} value={`${data.livingArea} m²`} />}
                        {data.ceilingHeight != null && <InfoRow icon={<Ruler size={16} />} label={t('ceilingHeight')} value={`${data.ceilingHeight} m`} />}
                        {data.roomsCount != null && <InfoRow icon={<DoorOpen size={16} />} label={t('room')} value={`${data.roomsCount}`} />}
                        {data.floorNumbers && data.floorNumbers.length > 0 && (
                            <InfoRow icon={<Layers size={16} />} label={t('floor')} value={data.floorNumbers.join(', ')} />
                        )}
                        {data.building && <InfoRow icon={<Building2 size={16} />} label={t('building')} value={data.building} />}
                    </div>
                </div>

                {/* Pricing */}
                <div className="card p-6">
                    <h3 className="text-xs font-bold text-slate-400 uppercase tracking-widest mb-5 flex items-center gap-2">
                        <Banknote size={14} />
                        {t('pricePerMonth')}
                    </h3>
                    <div className="space-y-4">
                        {data.priceForMonth != null && (
                            <div className="flex items-baseline gap-2">
                                <span className="text-3xl font-bold text-slate-900 dark:text-white">{data.priceForMonth.toLocaleString()}</span>
                                <span className="text-sm text-slate-400">UZS / {t('pricePerMonth').toLowerCase()}</span>
                            </div>
                        )}
                        {data.pricePerSquareMeter != null && (
                            <InfoRow icon={<Banknote size={16} />} label={t('pricePerSqm')} value={`${data.pricePerSquareMeter.toLocaleString()} UZS`} />
                        )}
                    </div>
                </div>

                {/* Location */}
                <div className="card p-6">
                    <h3 className="text-xs font-bold text-slate-400 uppercase tracking-widest mb-5 flex items-center gap-2">
                        <MapPin size={14} />
                        {t('location')}
                    </h3>
                    <div className="space-y-4">
                        {data.region && <InfoRow icon={<MapPin size={16} />} label={t('region')} value={data.region} />}
                        {data.district && <InfoRow icon={<MapPin size={16} />} label={t('district')} value={data.district} />}
                        {data.address && <InfoRow icon={<MapPin size={16} />} label={t('address')} value={data.address} />}
                        {data.complex && <InfoRow icon={<Building2 size={16} />} label={t('complex')} value={data.complex} />}
                    </div>
                </div>

                {/* Categories */}
                <div className="card p-6">
                    <h3 className="text-xs font-bold text-slate-400 uppercase tracking-widest mb-5 flex items-center gap-2">
                        <Tag size={14} />
                        {t('categories')}
                    </h3>
                    <div className="flex flex-wrap gap-2">
                        {data.categories?.length ? data.categories.map((cat, i) => (
                            <span key={i} className="inline-flex items-center px-3 py-1.5 rounded-lg bg-blue-50 dark:bg-blue-900/20 text-blue-700 dark:text-blue-300 text-sm font-medium">{cat}</span>
                        )) : (
                            <p className="text-sm text-slate-400">—</p>
                        )}
                    </div>
                </div>
            </div>

            {/* Description */}
            {data.description && (
                <div className="card p-6 mb-6">
                    <h3 className="text-xs font-bold text-slate-400 uppercase tracking-widest mb-4">{t('description')}</h3>
                    <p className="text-sm text-slate-700 dark:text-slate-300 leading-relaxed whitespace-pre-wrap">{data.description}</p>
                </div>
            )}

            {/* Owner & Meta */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-5">
                <OwnerCard ownerId={data.ownerId} />
                <div className="card p-5 flex items-center gap-3">
                    <Clock size={16} className="text-slate-400" />
                    <span className="text-sm text-slate-500">{t('createdAt')}:</span>
                    <span className="text-sm font-medium text-slate-700 dark:text-slate-300">{new Date(data.createdAt).toLocaleString()}</span>
                </div>
            </div>

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
