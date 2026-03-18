import { useState } from 'react'
import { useApp } from '../../context/AppContext'
import { Check, X, Ban, Loader2 } from 'lucide-react'

// ── Confirm Accept Dialog ──
interface ConfirmAcceptDialogProps {
    open: boolean
    onConfirm: () => void
    onCancel: () => void
    loading?: boolean
}

export function ConfirmAcceptDialog({ open, onConfirm, onCancel, loading }: ConfirmAcceptDialogProps) {
    const { t } = useApp()
    if (!open) return null

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm" onClick={onCancel}>
            <div className="bg-white dark:bg-slate-900 rounded-2xl shadow-2xl w-full max-w-md mx-4 p-6 animate-in" onClick={e => e.stopPropagation()}>
                <div className="flex items-center gap-3 mb-4">
                    <div className="w-10 h-10 rounded-full bg-green-100 dark:bg-green-900/30 flex items-center justify-center">
                        <Check size={20} className="text-green-600" />
                    </div>
                    <h3 className="text-lg font-semibold text-slate-900 dark:text-white">{t('confirmAccept')}</h3>
                </div>
                <p className="text-sm text-slate-600 dark:text-slate-400 mb-6">{t('confirmAcceptMessage')}</p>
                <div className="flex items-center justify-end gap-3">
                    <button onClick={onCancel} disabled={loading} className="btn btn-secondary px-5">{t('cancel')}</button>
                    <button onClick={onConfirm} disabled={loading} className="btn btn-success px-5 flex items-center gap-2">
                        {loading && <Loader2 size={16} className="animate-spin" />}
                        {t('accept')}
                    </button>
                </div>
            </div>
        </div>
    )
}

// ── Reject/Block Reason Dialog ──
interface ReasonDialogProps {
    open: boolean
    type: 'reject' | 'block'
    onConfirm: (reason: string) => void
    onCancel: () => void
    loading?: boolean
}

export function ReasonDialog({ open, type, onConfirm, onCancel, loading }: ReasonDialogProps) {
    const { t } = useApp()
    const [reason, setReason] = useState('')

    if (!open) return null

    const isReject = type === 'reject'
    const title = isReject ? t('rejectReason') : t('blockReason')
    const icon = isReject ? <X size={20} className="text-red-600" /> : <Ban size={20} className="text-slate-600" />
    const iconBg = isReject ? 'bg-red-100 dark:bg-red-900/30' : 'bg-slate-100 dark:bg-slate-800'
    const confirmLabel = isReject ? t('reject') : t('block')
    const confirmClass = isReject ? 'btn btn-danger' : 'btn bg-slate-800 text-white hover:bg-slate-700'

    const handleConfirm = () => {
        onConfirm(reason)
        setReason('')
    }

    const handleCancel = () => {
        setReason('')
        onCancel()
    }

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm" onClick={handleCancel}>
            <div className="bg-white dark:bg-slate-900 rounded-2xl shadow-2xl w-full max-w-md mx-4 p-6 animate-in" onClick={e => e.stopPropagation()}>
                <div className="flex items-center gap-3 mb-4">
                    <div className={`w-10 h-10 rounded-full ${iconBg} flex items-center justify-center`}>
                        {icon}
                    </div>
                    <h3 className="text-lg font-semibold text-slate-900 dark:text-white">{title}</h3>
                </div>
                <textarea
                    value={reason}
                    onChange={e => setReason(e.target.value)}
                    placeholder={t('reasonPlaceholder')}
                    rows={4}
                    className="input resize-none mb-4"
                    autoFocus
                />
                <div className="flex items-center justify-end gap-3">
                    <button onClick={handleCancel} disabled={loading} className="btn btn-secondary px-5">{t('cancel')}</button>
                    <button onClick={handleConfirm} disabled={loading} className={`${confirmClass} px-5 flex items-center gap-2`}>
                        {loading && <Loader2 size={16} className="animate-spin" />}
                        {confirmLabel}
                    </button>
                </div>
            </div>
        </div>
    )
}

// ── Action Toolbar (inline buttons) ──
interface ActionToolbarProps {
    onAccept: () => void
    onReject: () => void
    onBlock: () => void
    disabled?: boolean
    size?: 'sm' | 'md'
}

export function ActionToolbar({ onAccept, onReject, onBlock, disabled, size = 'sm' }: ActionToolbarProps) {
    const iconSize = size === 'sm' ? 16 : 18
    const padding = size === 'sm' ? 'p-1.5' : 'p-2'

    return (
        <div className="flex items-center gap-1">
            <button
                onClick={onAccept}
                disabled={disabled}
                className={`${padding} rounded-lg hover:bg-green-100 dark:hover:bg-green-900/30 text-green-600 transition-colors disabled:opacity-40`}
                title="Accept"
            >
                <Check size={iconSize} />
            </button>
            <button
                onClick={onReject}
                disabled={disabled}
                className={`${padding} rounded-lg hover:bg-red-100 dark:hover:bg-red-900/30 text-red-600 transition-colors disabled:opacity-40`}
                title="Reject"
            >
                <X size={iconSize} />
            </button>
            <button
                onClick={onBlock}
                disabled={disabled}
                className={`${padding} rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-600 transition-colors disabled:opacity-40`}
                title="Block"
            >
                <Ban size={iconSize} />
            </button>
        </div>
    )
}
