import { useState, ReactNode } from 'react'
import { useApp } from '../context/AppContext'
import { showToast } from './common'
import DataTable from './DataTable'
import CardGrid from './CardGrid'
import PageHeader from './PageHeader'
import Pagination from './Pagination'
import ViewToggle, { ViewMode } from './ViewToggle'
import Modal from './Modal'
import DeleteModal from './DeleteModal'
import type { PagedList, PaginationParams } from '../types'

interface Column<T> {
  key: keyof T | string
  header: string
  render?: (item: T) => ReactNode
}

interface CrudPageProps<T> {
  data: PagedList<T> | undefined
  columns: Column<T>[]
  isLoading: boolean
  pagination: PaginationParams
  onPageChange: (page: number) => void
  onPageSizeChange?: (size: number) => void
  onCreate?: (data: Partial<T>) => Promise<unknown>
  onUpdate?: (data: Partial<T>) => Promise<unknown>
  onDelete?: (id: string) => Promise<unknown>
  onFetchItem?: (id: string) => Promise<T>  // Fetch full item details before editing
  renderForm?: (
    item: Partial<T> | null,
    onChange: (data: Partial<T>) => void
  ) => ReactNode
  renderCard?: (item: T) => ReactNode
  getItemId: (item: T) => string
  getItemName?: (item: T) => string | undefined
  getItemSubtitle?: (item: T) => string | undefined
  getItemImage?: (item: T) => string | undefined
  isCreating?: boolean
  isUpdating?: boolean
  isDeleting?: boolean
  defaultView?: ViewMode
  showViewToggle?: boolean
  showPageHeader?: boolean
  cardColumns?: 2 | 3 | 4
  onRowClick?: (item: T) => void
}

export default function CrudPage<T extends object>({
  data,
  columns,
  isLoading,
  pagination,
  onPageChange,
  onPageSizeChange,
  onCreate,
  onUpdate,
  onDelete,
  onFetchItem,
  renderForm,
  renderCard,
  getItemId,
  getItemName,
  getItemSubtitle,
  getItemImage,
  isCreating,
  isUpdating,
  isDeleting,
  defaultView = 'list',
  showViewToggle = true,
  showPageHeader = true,
  cardColumns = 3,
  onRowClick,
}: CrudPageProps<T>) {
  const { t } = useApp()

  const [viewMode, setViewMode] = useState<ViewMode>(defaultView)
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false)
  const [editingItem, setEditingItem] = useState<T | null>(null)
  const [deletingItem, setDeletingItem] = useState<T | null>(null)
  const [formData, setFormData] = useState<Partial<T>>({})
  const [searchQuery, setSearchQuery] = useState('')
  const [isFetchingItem, setIsFetchingItem] = useState(false)

  const handleAdd = () => {
    if (!onCreate || !renderForm) return
    setEditingItem(null)
    setFormData({})
    setIsModalOpen(true)
  }

  const handleEdit = async (item: T) => {
    if (!onUpdate || !renderForm) return
    setEditingItem(item)

    // If onFetchItem is provided, fetch the full details before editing
    if (onFetchItem) {
      setIsFetchingItem(true)
      setIsModalOpen(true)
      try {
        const fullItem = await onFetchItem(getItemId(item))
        setFormData(fullItem as Partial<T>)
      } catch (error) {
        console.error('Error fetching item details:', error)
        showToast.error(t('error'), t('fetch_failed') || 'Failed to fetch item details')
        setIsModalOpen(false)
      } finally {
        setIsFetchingItem(false)
      }
    } else {
      setFormData(item as Partial<T>)
      setIsModalOpen(true)
    }
  }

  const handleDelete = (item: T) => {
    if (!onDelete) return
    setDeletingItem(item)
    setIsDeleteModalOpen(true)
  }

  const handleSubmit = async () => {
    try {
      if (editingItem) {
        if (onUpdate) {
          await onUpdate({ ...formData, id: getItemId(editingItem) } as Partial<T>)
          showToast.success(t('success'), t('item_updated'))
        }
      } else {
        if (onCreate) {
          await onCreate(formData)
          showToast.success(t('success'), t('item_created'))
        }
      }
      setIsModalOpen(false)
      setFormData({})
      setEditingItem(null)
    } catch (error) {
      console.error('Error saving:', error)
      showToast.error(
        t('error'),
        error instanceof Error ? error.message : t('operation_failed')
      )
    }
  }

  const handleConfirmDelete = async () => {
    if (deletingItem && onDelete) {
      try {
        await onDelete(getItemId(deletingItem))
        showToast.success(t('success'), t('item_deleted'))
        setIsDeleteModalOpen(false)
        setDeletingItem(null)
      } catch (error) {
        console.error('Error deleting:', error)
        showToast.error(
          t('error'),
          error instanceof Error ? error.message : t('delete_failed')
        )
      }
    }
  }

  const filteredData = data?.items.filter(item => {
    if (!searchQuery) return true
    const searchLower = searchQuery.toLowerCase()
    return Object.values(item as object).some(
      val => typeof val === 'string' && val.toLowerCase().includes(searchLower)
    )
  }) || []

  return (
    <div className="space-y-4">
      {/* Header with View Toggle */}
      <div className="card">
        <div className="w-full flex items-center justify-between p-4">
          <PageHeader
            onAdd={onCreate && renderForm ? handleAdd : undefined}
            searchValue={searchQuery}
            onSearch={setSearchQuery}
            isShow={showPageHeader}
          />
          {showViewToggle && (
            <ViewToggle view={viewMode} onChange={setViewMode} />
          )}
        </div>
      </div>

      {/* Content - List or Card View */}
      <div className="card overflow-hidden">
        {viewMode === 'list' ? (
          <DataTable
            data={filteredData}
            columns={columns}
            isLoading={isLoading}
            onEdit={onUpdate && renderForm ? handleEdit : undefined}
            onDelete={onDelete ? handleDelete : undefined}
            getId={getItemId}
            onRowClick={onRowClick}
          />
        ) : (
          <div className="p-4">
            <CardGrid
              data={filteredData}
              isLoading={isLoading}
              renderCard={renderCard}
              onEdit={handleEdit}
              onDelete={handleDelete}
              getId={getItemId}
              getTitle={getItemName}
              getSubtitle={getItemSubtitle}
              getImage={getItemImage}
              columns={cardColumns}
            />
          </div>
        )}

        {/* Pagination - Always show when data exists */}
        {data && (
          <Pagination
            pageNumber={pagination.pageNumber}
            totalPages={data.totalPages || Math.ceil((data.totalCount || 0) / pagination.pageSize) || 1}
            totalCount={data.totalCount}
            pageSize={pagination.pageSize}
            onPageChange={onPageChange}
            onPageSizeChange={onPageSizeChange}
            showPageSize={!!onPageSizeChange}
          />
        )}
      </div>

      {/* Create/Edit Modal */}
      {renderForm && (
        <Modal
          isOpen={isModalOpen}
          onClose={() => setIsModalOpen(false)}
          title={editingItem ? t('edit') : t('add')}
          size="lg"
        >
          <div className="space-y-4">
            {isFetchingItem ? (
              <div className="flex items-center justify-center py-8">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
                <span className="ml-3 text-slate-600 dark:text-slate-400">{t('loading')}</span>
              </div>
            ) : (
              renderForm(formData, setFormData)
            )}
            <div className="flex justify-end gap-3 pt-4 border-t border-slate-200 dark:border-slate-700">
              <button
                onClick={() => setIsModalOpen(false)}
                className="btn btn-secondary"
              >
                {t('cancel')}
              </button>
              <button
                onClick={handleSubmit}
                className="btn btn-primary"
                disabled={isCreating || isUpdating || isFetchingItem}
              >
                {isCreating || isUpdating ? t('loading') : t('save')}
              </button>
            </div>
          </div>
        </Modal>
      )}

      {/* Delete Confirmation Modal */}
      <DeleteModal
        isOpen={isDeleteModalOpen}
        onClose={() => setIsDeleteModalOpen(false)}
        onConfirm={handleConfirmDelete}
        isLoading={isDeleting}
        itemName={deletingItem && getItemName ? getItemName(deletingItem) : undefined}
      />
    </div>
  )
}
