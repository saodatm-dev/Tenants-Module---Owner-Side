import { useApp } from '../../context/AppContext'
import clsx from 'clsx'

// ModerationStatus: 0=Cancel, 1=InModeration, 2=Accept, 3=Reject, 4=Block
const statusConfig: Record<number, { color: string; labelKey: string }> = {
    0: { color: 'badge-secondary', labelKey: 'cancelled' },
    1: { color: 'badge-warning', labelKey: 'pending' },
    2: { color: 'badge-success', labelKey: 'approved' },
    3: { color: 'badge-danger', labelKey: 'rejected' },
    4: { color: 'badge-info', labelKey: 'blocked' },
}

export default function ModerationStatusBadge({ status }: { status: number }) {
    const { t } = useApp()
    const config = statusConfig[status] || statusConfig[0]

    return (
        <span className={clsx('badge', config.color)}>
            {t(config.labelKey)}
        </span>
    )
}
