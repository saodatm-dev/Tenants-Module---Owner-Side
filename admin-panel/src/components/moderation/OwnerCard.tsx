import { useQuery } from '@tanstack/react-query'
import { usersService } from '../../services/api'
import { User, Loader2, Phone } from 'lucide-react'

interface OwnerCardProps {
    ownerId: string
    compact?: boolean
}

export default function OwnerCard({ ownerId, compact = false }: OwnerCardProps) {
    const { data: owner, isLoading } = useQuery({
        queryKey: ['user', ownerId],
        queryFn: () => usersService.getById(ownerId),
        enabled: !!ownerId,
        staleTime: 5 * 60 * 1000,
        retry: 1,
    })

    if (isLoading) {
        return (
            <div className={compact ? 'flex items-center gap-2' : 'card p-5'}>
                <Loader2 size={compact ? 14 : 18} className="animate-spin text-slate-400" />
                {!compact && <span className="text-sm text-slate-400 ml-2">Loading…</span>}
            </div>
        )
    }

    if (!owner) {
        return compact ? (
            <code className="text-xs bg-slate-100 dark:bg-slate-800 px-2 py-0.5 rounded font-mono text-slate-500">{ownerId}</code>
        ) : (
            <div className="card p-5">
                <div className="flex items-center gap-3">
                    <div className="w-10 h-10 rounded-full bg-slate-100 dark:bg-slate-800 flex items-center justify-center">
                        <User size={18} className="text-slate-400" />
                    </div>
                    <code className="text-sm font-mono text-slate-500 break-all">{ownerId}</code>
                </div>
            </div>
        )
    }

    const fullName = [owner.firstName, owner.lastName].filter(Boolean).join(' ')
    const hasName = fullName.trim().length > 0
    const displayName = hasName ? fullName : (owner.phoneNumber || ownerId)
    const initials = hasName
        ? [owner.firstName?.[0], owner.lastName?.[0]].filter(Boolean).join('').toUpperCase()
        : null

    if (compact) {
        return (
            <div className="flex items-center gap-2.5">
                {owner.photo ? (
                    <img src={owner.photo} alt={displayName} className="w-7 h-7 rounded-full object-cover ring-1 ring-slate-200 dark:ring-slate-700" />
                ) : initials ? (
                    <div className="w-7 h-7 rounded-full bg-slate-900 dark:bg-slate-600 flex items-center justify-center">
                        <span className="text-[10px] font-bold text-white">{initials}</span>
                    </div>
                ) : (
                    <div className="w-7 h-7 rounded-full bg-slate-200 dark:bg-slate-700 flex items-center justify-center">
                        <Phone size={12} className="text-slate-500" />
                    </div>
                )}
                <span className="text-sm font-medium text-slate-700 dark:text-slate-300 truncate max-w-[160px]">{displayName}</span>
            </div>
        )
    }

    return (
        <div className="card p-5">
            <div className="flex items-center gap-4">
                {owner.photo ? (
                    <img src={owner.photo} alt={displayName} className="w-12 h-12 rounded-full object-cover ring-2 ring-slate-200 dark:ring-slate-700" />
                ) : initials ? (
                    <div className="w-12 h-12 rounded-full bg-slate-900 dark:bg-slate-600 flex items-center justify-center">
                        <span className="text-sm font-bold text-white">{initials}</span>
                    </div>
                ) : (
                    <div className="w-12 h-12 rounded-full bg-slate-200 dark:bg-slate-700 flex items-center justify-center">
                        <Phone size={18} className="text-slate-500" />
                    </div>
                )}
                <div className="min-w-0 flex-1">
                    {hasName ? (
                        <>
                            <p className="text-base font-semibold text-slate-900 dark:text-white truncate">{fullName}</p>
                            {owner.middleName && (
                                <p className="text-sm text-slate-500 dark:text-slate-400 truncate">{owner.middleName}</p>
                            )}
                            {owner.phoneNumber && (
                                <p className="text-sm text-slate-400 mt-0.5">{owner.phoneNumber}</p>
                            )}
                        </>
                    ) : (
                        <>
                            <p className="text-base font-semibold text-slate-900 dark:text-white truncate">
                                {owner.phoneNumber || <span className="text-slate-400 font-mono text-sm">{ownerId}</span>}
                            </p>
                            <p className="text-xs text-slate-400 mt-0.5">No name provided</p>
                        </>
                    )}
                </div>
            </div>
        </div>
    )
}

