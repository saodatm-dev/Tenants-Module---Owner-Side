import { useApp } from '../context/AppContext'
import Modal from './Modal'
import { AlertTriangle } from 'lucide-react'

interface DeleteModalProps {
  isOpen: boolean
  onClose: () => void
  onConfirm: () => void
  isLoading?: boolean
  itemName?: string
}

export default function DeleteModal({
  isOpen,
  onClose,
  onConfirm,
  isLoading,
  itemName,
}: DeleteModalProps) {
  const { t } = useApp()

  return (
    <Modal isOpen={isOpen} onClose={onClose} title={t('delete')} size="sm">
      <div className="text-center">
        <div className="mx-auto w-12 h-12 bg-red-100 dark:bg-red-900/30 rounded-full flex items-center justify-center mb-4">
          <AlertTriangle className="text-red-600 dark:text-red-400" size={24} />
        </div>
        <p className="text-slate-700 dark:text-slate-300 mb-2">
          {t('deleteConfirm')}
        </p>
        {itemName && (
          <p className="font-medium text-slate-900 dark:text-white mb-2">
            "{itemName}"
          </p>
        )}
        <p className="text-sm text-slate-500 dark:text-slate-400 mb-6">
          {t('deleteWarning')}
        </p>
        <div className="flex gap-3 justify-center">
          <button
            onClick={onClose}
            className="btn btn-secondary"
            disabled={isLoading}
          >
            {t('cancel')}
          </button>
          <button
            onClick={onConfirm}
            className="btn btn-danger"
            disabled={isLoading}
          >
            {isLoading ? t('loading') : t('delete')}
          </button>
        </div>
      </div>
    </Modal>
  )
}
