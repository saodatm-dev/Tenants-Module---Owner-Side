import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { useApp } from '../context/AppContext'
import { useModerationListings, useModerationRealEstates, useModerationUnits, useModerationActions } from '../hooks/useModeration'
import { ConfirmAcceptDialog, ReasonDialog, ActionToolbar } from '../components/moderation/ModerationActionDialogs'
import ModerationStatusBadge from '../components/moderation/ModerationStatusBadge'
import { ImageThumbnail } from '../components/moderation/ImageGallery'
import { Search, ChevronLeft, ChevronRight, Eye, Calendar, Building2, Home, Box } from 'lucide-react'
import { toast } from 'sonner'
import clsx from 'clsx'
import type { ModerationListing, ModerationRealEstate, ModerationRealEstateUnit, ModerationFilterParams } from '../types'

type TabType = 'listings' | 'realEstates' | 'units'

const STATUS_OPTIONS = [
  { value: -1, labelKey: 'allStatuses' },
  { value: 1, labelKey: 'pending' },
  { value: 2, labelKey: 'approved' },
  { value: 3, labelKey: 'rejected' },
  { value: 4, labelKey: 'blocked' },
  { value: 0, labelKey: 'cancelled' },
]

const TAB_ICONS = {
  listings: Building2,
  realEstates: Home,
  units: Box,
}

export default function Moderation() {
  const { t } = useApp()
  const navigate = useNavigate()
  const [activeTab, setActiveTab] = useState<TabType>('listings')
  const [pagination, setPagination] = useState({ pageNumber: 1, pageSize: 10 })
  const [searchInput, setSearchInput] = useState('')
  const [debouncedSearch, setDebouncedSearch] = useState('')
  const [statusFilter, setStatusFilter] = useState<number>(-1)

  useEffect(() => {
    const timer = setTimeout(() => setDebouncedSearch(searchInput), 400)
    return () => clearTimeout(timer)
  }, [searchInput])

  useEffect(() => {
    setPagination(p => ({ ...p, pageNumber: 1 }))
  }, [debouncedSearch, statusFilter, activeTab])

  const params: ModerationFilterParams = {
    ...pagination,
    filter: debouncedSearch || undefined,
  }

  const listingsQuery = useModerationListings(params, activeTab === 'listings')
  const realEstatesQuery = useModerationRealEstates(params, activeTab === 'realEstates')
  const unitsQuery = useModerationUnits(params, activeTab === 'units')

  // ── Actions ──
  const [acceptDialogId, setAcceptDialogId] = useState<string | null>(null)
  const [rejectDialogId, setRejectDialogId] = useState<string | null>(null)
  const [blockDialogId, setBlockDialogId] = useState<string | null>(null)

  const actions = useModerationActions(activeTab, {
    onSuccess: () => {
      setAcceptDialogId(null)
      setRejectDialogId(null)
      setBlockDialogId(null)
    },
  })

  const handleAcceptConfirm = () => {
    if (acceptDialogId) {
      actions.acceptMutation.mutate(acceptDialogId, {
        onSuccess: () => toast.success(t('accepted')),
        onError: () => toast.error(t('error')),
      })
    }
  }

  const handleRejectConfirm = (reason: string) => {
    if (rejectDialogId) {
      actions.rejectMutation.mutate({ id: rejectDialogId, reason }, {
        onSuccess: () => toast.success(t('rejectedMsg')),
        onError: () => toast.error(t('error')),
      })
    }
  }

  const handleBlockConfirm = (reason: string) => {
    if (blockDialogId) {
      actions.blockMutation.mutate({ id: blockDialogId, reason }, {
        onSuccess: () => toast.success(t('blockedMsg')),
        onError: () => toast.error(t('error')),
      })
    }
  }

  const handleViewDetail = (id: string) => {
    const prefix = activeTab === 'listings' ? 'listings' : activeTab === 'realEstates' ? 'realestates' : 'units'
    navigate(`/moderation/${prefix}/${id}`)
  }

  const tabs = [
    { id: 'listings' as TabType, label: t('listings') },
    { id: 'realEstates' as TabType, label: t('realEstates') },
    { id: 'units' as TabType, label: t('units') },
  ]

  const handleTabChange = (tab: TabType) => {
    setActiveTab(tab)
    setSearchInput('')
    setDebouncedSearch('')
    setStatusFilter(-1)
  }

  const getCurrentData = () => {
    const raw = activeTab === 'listings' ? listingsQuery.data :
      activeTab === 'realEstates' ? realEstatesQuery.data : unitsQuery.data
    if (!raw) return raw
    if (statusFilter !== -1) {
      const filteredItems = raw.items.filter(
        (item: ModerationListing | ModerationRealEstate | ModerationRealEstateUnit) =>
          item.moderationStatus === statusFilter
      )
      return { ...raw, items: filteredItems }
    }
    return raw
  }

  const isLoading = activeTab === 'listings' ? listingsQuery.isLoading :
    activeTab === 'realEstates' ? realEstatesQuery.isLoading : unitsQuery.isLoading

  const data = getCurrentData()

  return (
    <div className="max-w-7xl mx-auto">


      {/* Tabs */}
      <div className="flex gap-2 mb-6">
        {tabs.map(tab => {
          const Icon = TAB_ICONS[tab.id]
          return (
            <button
              key={tab.id}
              onClick={() => handleTabChange(tab.id)}
              className={clsx(
                'inline-flex items-center gap-2 px-4 py-2.5 rounded-xl text-sm font-medium transition-all',
                activeTab === tab.id
                  ? 'bg-slate-900 dark:bg-white text-white dark:text-slate-900 shadow-md'
                  : 'bg-white dark:bg-slate-800 text-slate-600 dark:text-slate-400 hover:bg-slate-100 dark:hover:bg-slate-700 border border-slate-200 dark:border-slate-700'
              )}
            >
              <Icon size={16} />
              {tab.label}
            </button>
          )
        })}
      </div>

      {/* Search + Filter */}
      <div className="flex flex-col sm:flex-row gap-3 mb-5">
        <div className="relative flex-1">
          <Search size={18} className="absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400" />
          <input
            type="text"
            value={searchInput}
            onChange={e => setSearchInput(e.target.value)}
            placeholder={t('searchModeration')}
            className="input pl-10"
          />
        </div>
        <select
          value={statusFilter}
          onChange={e => setStatusFilter(Number(e.target.value))}
          className="input w-full sm:w-52"
        >
          {STATUS_OPTIONS.map(opt => (
            <option key={opt.value} value={opt.value}>{t(opt.labelKey)}</option>
          ))}
        </select>
      </div>

      {/* Content */}
      <div className="card overflow-hidden">
        {isLoading ? (
          <div className="p-16 text-center">
            <div className="animate-spin rounded-full h-10 w-10 border-2 border-slate-200 border-t-primary-500 mx-auto"></div>
            <p className="mt-4 text-slate-500 text-sm">{t('loading')}</p>
          </div>
        ) : !data?.items.length ? (
          <div className="p-16 text-center">
            <div className="w-20 h-20 rounded-2xl bg-slate-100 dark:bg-slate-800 flex items-center justify-center mx-auto mb-4">
              <Search size={32} className="text-slate-300 dark:text-slate-600" />
            </div>
            <p className="text-slate-500 font-medium text-lg">{t('noData')}</p>
            <p className="text-sm text-slate-400 mt-1">Try adjusting your filters</p>
          </div>
        ) : (
          <>
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="border-b border-slate-200 dark:border-slate-800">
                    <th className="px-4 py-3.5 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider"></th>
                    {activeTab === 'listings' && (
                      <>
                        <th className="px-4 py-3.5 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">{t('name')}</th>
                        <th className="px-4 py-3.5 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">{t('pricePerMonth')}</th>
                        <th className="px-4 py-3.5 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">{t('totalArea')}</th>
                      </>
                    )}
                    {activeTab === 'realEstates' && (
                      <>
                        <th className="px-4 py-3.5 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">{t('buildingNumber')}</th>
                        <th className="px-4 py-3.5 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">{t('type')}</th>
                        <th className="px-4 py-3.5 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">{t('address')}</th>
                      </>
                    )}
                    {activeTab === 'units' && (
                      <>
                        <th className="px-4 py-3.5 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">{t('room')}</th>
                        <th className="px-4 py-3.5 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">{t('floor')}</th>
                        <th className="px-4 py-3.5 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">{t('totalArea')}</th>
                      </>
                    )}
                    <th className="px-4 py-3.5 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">{t('status')}</th>
                    <th className="px-4 py-3.5 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">{t('createdAt')}</th>
                    <th className="px-4 py-3.5 text-right text-xs font-semibold text-slate-500 uppercase tracking-wider">{t('actions')}</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-100 dark:divide-slate-800/50">
                  {activeTab === 'listings' && (data.items as ModerationListing[]).map((item) => (
                    <tr key={item.id} className="group hover:bg-slate-50/50 dark:hover:bg-slate-800/20 transition-colors cursor-pointer" onClick={() => handleViewDetail(item.id)}>
                      <td className="px-4 py-3">
                        <ImageThumbnail images={item.objectNames} />
                      </td>
                      <td className="px-4 py-3">
                        <div>
                          <p className="text-sm font-semibold text-slate-800 dark:text-slate-200">{item.complexName || item.address || '—'}</p>
                          {item.categories?.length > 0 && (
                            <p className="text-xs text-slate-400 mt-0.5 truncate max-w-[200px]">{item.categories.join(', ')}</p>
                          )}
                        </div>
                      </td>
                      <td className="px-4 py-3">
                        <span className="text-sm font-medium text-slate-700 dark:text-slate-300">{item.priceForMonth ? `${item.priceForMonth.toLocaleString()}` : '—'}</span>
                        {item.priceForMonth && <span className="text-xs text-slate-400 ml-1">UZS</span>}
                      </td>
                      <td className="px-4 py-3 text-sm text-slate-600 dark:text-slate-400">{item.totalArea} m²</td>
                      <td className="px-4 py-3"><ModerationStatusBadge status={item.moderationStatus} /></td>
                      <td className="px-4 py-3">
                        <div className="flex items-center gap-1.5 text-sm text-slate-500">
                          <Calendar size={14} />
                          {new Date(item.createdAt).toLocaleDateString()}
                        </div>
                      </td>
                      <td className="px-4 py-3 text-right" onClick={e => e.stopPropagation()}>
                        <div className="flex items-center justify-end gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                          <button onClick={() => handleViewDetail(item.id)} className="p-2 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 text-primary-500 transition-colors" title="View">
                            <Eye size={16} />
                          </button>
                          {item.moderationStatus === 1 && (
                            <ActionToolbar
                              onAccept={() => setAcceptDialogId(item.id)}
                              onReject={() => setRejectDialogId(item.id)}
                              onBlock={() => setBlockDialogId(item.id)}
                              disabled={actions.isActing}
                            />
                          )}
                        </div>
                      </td>
                    </tr>
                  ))}

                  {activeTab === 'realEstates' && (data.items as ModerationRealEstate[]).map((item) => (
                    <tr key={item.id} className="group hover:bg-slate-50/50 dark:hover:bg-slate-800/20 transition-colors cursor-pointer" onClick={() => handleViewDetail(item.id)}>
                      <td className="px-4 py-3">
                        <ImageThumbnail images={item.objectNames} />
                      </td>
                      <td className="px-4 py-3">
                        <p className="text-sm font-semibold text-slate-800 dark:text-slate-200">{item.number || '—'}</p>
                        {item.buildingNumber && <p className="text-xs text-slate-400 mt-0.5">#{item.buildingNumber}</p>}
                      </td>
                      <td className="px-4 py-3 text-sm text-slate-600 dark:text-slate-400">{item.realEstateType || '—'}</td>
                      <td className="px-4 py-3 text-sm text-slate-600 dark:text-slate-400 max-w-[180px] truncate">{item.address || '—'}</td>
                      <td className="px-4 py-3"><ModerationStatusBadge status={item.moderationStatus} /></td>
                      <td className="px-4 py-3">
                        <div className="flex items-center gap-1.5 text-sm text-slate-500">
                          <Calendar size={14} />
                          {new Date(item.createdAt).toLocaleDateString()}
                        </div>
                      </td>
                      <td className="px-4 py-3 text-right" onClick={e => e.stopPropagation()}>
                        <div className="flex items-center justify-end gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                          <button onClick={() => handleViewDetail(item.id)} className="p-2 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 text-primary-500 transition-colors" title="View">
                            <Eye size={16} />
                          </button>
                          {item.moderationStatus === 1 && (
                            <ActionToolbar
                              onAccept={() => setAcceptDialogId(item.id)}
                              onReject={() => setRejectDialogId(item.id)}
                              onBlock={() => setBlockDialogId(item.id)}
                              disabled={actions.isActing}
                            />
                          )}
                        </div>
                      </td>
                    </tr>
                  ))}

                  {activeTab === 'units' && (data.items as ModerationRealEstateUnit[]).map((item) => (
                    <tr key={item.id} className="group hover:bg-slate-50/50 dark:hover:bg-slate-800/20 transition-colors cursor-pointer" onClick={() => handleViewDetail(item.id)}>
                      <td className="px-4 py-3">
                        <ImageThumbnail images={item.objectNames} />
                      </td>
                      <td className="px-4 py-3 text-sm font-semibold text-slate-800 dark:text-slate-200">{item.roomNumber || '—'}</td>
                      <td className="px-4 py-3 text-sm text-slate-600 dark:text-slate-400">{item.floorNumber ?? '—'}</td>
                      <td className="px-4 py-3 text-sm text-slate-600 dark:text-slate-400">{item.totalArea ? `${item.totalArea} m²` : '—'}</td>
                      <td className="px-4 py-3"><ModerationStatusBadge status={item.moderationStatus} /></td>
                      <td className="px-4 py-3">
                        <div className="flex items-center gap-1.5 text-sm text-slate-500">
                          <Calendar size={14} />
                          {new Date(item.createdAt).toLocaleDateString()}
                        </div>
                      </td>
                      <td className="px-4 py-3 text-right" onClick={e => e.stopPropagation()}>
                        <div className="flex items-center justify-end gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                          <button onClick={() => handleViewDetail(item.id)} className="p-2 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 text-primary-500 transition-colors" title="View">
                            <Eye size={16} />
                          </button>
                          {item.moderationStatus === 1 && (
                            <ActionToolbar
                              onAccept={() => setAcceptDialogId(item.id)}
                              onReject={() => setRejectDialogId(item.id)}
                              onBlock={() => setBlockDialogId(item.id)}
                              disabled={actions.isActing}
                            />
                          )}
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            {/* Pagination */}
            {data.totalPages > 1 && (
              <div className="px-5 py-4 border-t border-slate-200 dark:border-slate-800 flex items-center justify-between">
                <p className="text-sm text-slate-500">
                  {t('Page')} <span className="font-semibold text-slate-700 dark:text-slate-300">{pagination.pageNumber}</span> / {data.totalPages}
                  <span className="mx-2 text-slate-300">·</span>
                  <span className="font-semibold text-slate-700 dark:text-slate-300">{data.totalCount}</span> {t('items')}
                </p>
                <div className="flex items-center gap-1">
                  <button
                    onClick={() => setPagination(p => ({ ...p, pageNumber: p.pageNumber - 1 }))}
                    disabled={pagination.pageNumber <= 1}
                    className={clsx('p-2 rounded-lg transition-all', pagination.pageNumber <= 1 ? 'text-slate-300 cursor-not-allowed' : 'text-slate-600 hover:bg-slate-100 dark:hover:bg-slate-800 hover:text-slate-900')}
                  >
                    <ChevronLeft size={18} />
                  </button>
                  <button
                    onClick={() => setPagination(p => ({ ...p, pageNumber: p.pageNumber + 1 }))}
                    disabled={pagination.pageNumber >= data.totalPages}
                    className={clsx('p-2 rounded-lg transition-all', pagination.pageNumber >= data.totalPages ? 'text-slate-300 cursor-not-allowed' : 'text-slate-600 hover:bg-slate-100 dark:hover:bg-slate-800 hover:text-slate-900')}
                  >
                    <ChevronRight size={18} />
                  </button>
                </div>
              </div>
            )}
          </>
        )}
      </div>

      {/* Dialogs */}
      <ConfirmAcceptDialog open={!!acceptDialogId} onConfirm={handleAcceptConfirm} onCancel={() => setAcceptDialogId(null)} loading={actions.acceptMutation.isPending} />
      <ReasonDialog open={!!rejectDialogId} type="reject" onConfirm={handleRejectConfirm} onCancel={() => setRejectDialogId(null)} loading={actions.rejectMutation.isPending} />
      <ReasonDialog open={!!blockDialogId} type="block" onConfirm={handleBlockConfirm} onCancel={() => setBlockDialogId(null)} loading={actions.blockMutation.isPending} />
    </div>
  )
}
