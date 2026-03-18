import { useState, useMemo } from 'react'
import { useQuery } from '@tanstack/react-query'
import { Check, X } from 'lucide-react'
import { useApp } from '../context/AppContext'
import { usersService } from '../services/api'
import CrudPage from '../components/CrudPage'
import type { SystemUser, RegisterType } from '../types'

const FILTER_KEYS = ['all', 'active', 'inactive', 'verified'] as const
type FilterKey = typeof FILTER_KEYS[number]

export default function Users() {
    const { t } = useApp()
    const [pagination, setPagination] = useState({ pageNumber: 1, pageSize: 10 })
    const [activeFilter, setActiveFilter] = useState<FilterKey>('all')

    const { data, isLoading } = useQuery({
        queryKey: ['users', pagination],
        queryFn: () => usersService.getAll(pagination),
    })

    const filterLabels: Record<FilterKey, string> = useMemo(() => ({
        all: t('all') || 'All',
        active: t('active') || 'Active',
        inactive: t('inactive') || 'Inactive',
        verified: t('verified') || 'Verified',
    }), [t])

    // Filter users client-side based on pill selection
    const filteredData = useMemo(() => {
        if (!data) return data
        if (activeFilter === 'all') return data

        const filtered = data.items.filter((user: SystemUser) => {
            switch (activeFilter) {
                case 'active': return user.isActive === true
                case 'inactive': return user.isActive === false
                case 'verified': return user.isVerified === true
                default: return true
            }
        })

        return { ...data, items: filtered, totalCount: filtered.length }
    }, [data, activeFilter])

    const getFullName = (user: SystemUser) => {
        const parts = [user.firstName, user.middleName, user.lastName].filter(Boolean)
        return parts.join(' ')
    }

    const getRegisterTypeLabel = (type: RegisterType | number): string => {
        const typeValue = typeof type === 'number' ? type : type
        switch (typeValue) {
            case 0:
            case 'PhoneNumber':
                return t('phone') || 'Phone'
            case 1:
            case 'EImzo':
                return 'E-IMZO'
            case 2:
            case 'OneID':
                return 'One ID'
            default:
                return String(type)
        }
    }

    const columns = [
        {
            key: 'photo',
            header: '',
            render: (user: SystemUser) => (
                user.photo ? (
                    <img
                        src={user.photo}
                        alt={getFullName(user)}
                        className="w-10 h-10 rounded-full object-cover ring-2 ring-slate-100 dark:ring-slate-700"
                    />
                ) : (
                    <div className="w-10 h-10 rounded-full bg-gradient-to-br from-primary-400 to-primary-600 flex items-center justify-center text-white font-semibold">
                        {`${user.firstName?.[0] || ''}${user.lastName?.[0] || ''}`.toUpperCase()}
                    </div>
                )
            ),
        },
        {
            key: 'firstName',
            header: t('firstName') || 'First Name',
        },
        {
            key: 'lastName',
            header: t('lastName') || 'Last Name',
        },
        {
            key: 'phoneNumber',
            header: t('phoneNumber') || 'Phone',
            render: (user: SystemUser) => (
                <span className="text-slate-600 dark:text-slate-400">
                    {user.phoneNumber || '-'}
                </span>
            ),
        },
        {
            key: 'registerType',
            header: t('registerType') || 'Register Type',
            render: (user: SystemUser) => getRegisterTypeLabel(user.registerType),
        },
        {
            key: 'accountsCount',
            header: t('accountsCount') || 'Accounts',
            render: (user: SystemUser) => (
                <span className="inline-flex items-center justify-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-slate-100 dark:bg-slate-800 text-slate-700 dark:text-slate-300">
                    {user.accountsCount}
                </span>
            ),
        },
        {
            key: 'isVerified',
            header: t('verified') || 'Verified',
            render: (user: SystemUser) => (
                user.isVerified ? (
                    <span className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs font-medium bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-400">
                        <Check size={14} />
                        {t('yes') || 'Yes'}
                    </span>
                ) : (
                    <span className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs font-medium bg-slate-100 dark:bg-slate-800 text-slate-500 dark:text-slate-400">
                        <X size={14} />
                        {t('no') || 'No'}
                    </span>
                )
            ),
        },
        {
            key: 'isActive',
            header: t('status') || 'Status',
            render: (user: SystemUser) => (
                user.isActive ? (
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-400">
                        {t('active') || 'Active'}
                    </span>
                ) : (
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-100 dark:bg-red-900/30 text-red-700 dark:text-red-400">
                        {t('inactive') || 'Inactive'}
                    </span>
                )
            ),
        },
    ]

    return (
        <div>
            {/* Pill tab filter bar */}
            <div className="mb-6 -mt-1">
                <div
                    className="inline-flex flex-wrap gap-1 items-center"
                    style={{ padding: '4px', borderRadius: '100px', background: '#F6F6F6' }}
                >
                    {FILTER_KEYS.map(key => (
                        <button
                            key={key}
                            onClick={() => { setActiveFilter(key); setPagination(p => ({ ...p, pageNumber: 1 })) }}
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
                                ...(activeFilter === key
                                    ? { background: '#000', color: '#fff' }
                                    : { background: 'transparent', color: '#19191C' }
                                ),
                            }}
                            onMouseEnter={e => {
                                if (activeFilter !== key) {
                                    (e.target as HTMLElement).style.background = '#E8E8E8'
                                }
                            }}
                            onMouseLeave={e => {
                                if (activeFilter !== key) {
                                    (e.target as HTMLElement).style.background = 'transparent'
                                }
                            }}
                        >
                            {filterLabels[key]}
                        </button>
                    ))}
                </div>
            </div>

            {/* Users table */}
            <CrudPage<SystemUser>
                data={filteredData}
                columns={columns}
                isLoading={isLoading}
                pagination={pagination}
                onPageChange={page => setPagination(p => ({ ...p, pageNumber: page }))}
                onPageSizeChange={size => setPagination(p => ({ ...p, pageSize: size, pageNumber: 1 }))}
                getItemId={item => item.id}
                getItemName={item => getFullName(item)}
                getItemImage={item => item.photo}
                showViewToggle={true}
                defaultView="list"
            />
        </div>
    )
}
