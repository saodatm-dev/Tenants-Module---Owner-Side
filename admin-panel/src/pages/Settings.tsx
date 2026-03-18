import { useApp } from '../context/AppContext'
import { useAuth } from '../context/AuthContext'
import { Sun, Moon, Globe, User, Bell, Shield } from 'lucide-react'
import type { LanguageCode } from '../types'

const languageOptions: { code: LanguageCode; label: string; native: string }[] = [
  { code: 'en', label: 'English', native: 'English' },
  { code: 'uz', label: 'Uzbek', native: "O'zbekcha" },
  { code: 'ru', label: 'Russian', native: 'Русский' },
]

export default function Settings() {
  const { theme, setTheme, language, setLanguage, notifications, clearNotifications, t } = useApp()
  const { user } = useAuth()

  const unreadCount = notifications.filter(n => !n.read).length

  return (
    <div className="max-w-3xl mx-auto space-y-6">
      <h2 className="text-2xl font-bold text-slate-900 dark:text-white">{t('settings')}</h2>

      <div className="card p-6">
        <div className="flex items-center gap-4 mb-6">
          <div className="w-16 h-16 bg-primary-100 dark:bg-primary-900/30 rounded-full flex items-center justify-center">
            <User size={32} className="text-primary-600 dark:text-primary-400" />
          </div>
          <div>
            <h3 className="text-lg font-semibold text-slate-900 dark:text-white">
              {user?.firstName || 'Admin'} {user?.lastName || 'User'}
            </h3>
            <p className="text-sm text-slate-500">{user?.phoneNumber}</p>
          </div>
        </div>

        <div className="space-y-4">
          <div className="flex items-center justify-between py-3 border-b border-slate-100 dark:border-slate-800">
            <div className="flex items-center gap-3">
              <Shield size={20} className="text-slate-500" />
              <div>
                <p className="font-medium text-slate-900 dark:text-white">Role</p>
                <p className="text-sm text-slate-500">Administrator</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div className="card p-6">
        <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-4">Appearance</h3>

        <div className="space-y-4">
          <div className="flex items-center justify-between py-3">
            <div className="flex items-center gap-3">
              {theme === 'light' ? <Sun size={20} className="text-yellow-500" /> : <Moon size={20} className="text-blue-500" />}
              <div>
                <p className="font-medium text-slate-900 dark:text-white">{t('theme')}</p>
                <p className="text-sm text-slate-500">
                  {theme === 'light' ? t('lightMode') : t('darkMode')}
                </p>
              </div>
            </div>
            <div className="flex rounded-lg border border-slate-200 dark:border-slate-700 overflow-hidden">
              <button
                onClick={() => setTheme('light')}
                className={`px-4 py-2 text-sm font-medium transition-colors ${
                  theme === 'light'
                    ? 'bg-primary-500 text-white'
                    : 'bg-white dark:bg-slate-800 text-slate-600 dark:text-slate-400 hover:bg-slate-50 dark:hover:bg-slate-700'
                }`}
              >
                <Sun size={16} />
              </button>
              <button
                onClick={() => setTheme('dark')}
                className={`px-4 py-2 text-sm font-medium transition-colors ${
                  theme === 'dark'
                    ? 'bg-primary-500 text-white'
                    : 'bg-white dark:bg-slate-800 text-slate-600 dark:text-slate-400 hover:bg-slate-50 dark:hover:bg-slate-700'
                }`}
              >
                <Moon size={16} />
              </button>
            </div>
          </div>

          <div className="flex items-center justify-between py-3 border-t border-slate-100 dark:border-slate-800">
            <div className="flex items-center gap-3">
              <Globe size={20} className="text-slate-500" />
              <div>
                <p className="font-medium text-slate-900 dark:text-white">{t('language')}</p>
                <p className="text-sm text-slate-500">
                  {languageOptions.find(l => l.code === language)?.native}
                </p>
              </div>
            </div>
            <select
              value={language}
              onChange={e => setLanguage(e.target.value as LanguageCode)}
              className="input w-40"
            >
              {languageOptions.map(opt => (
                <option key={opt.code} value={opt.code}>
                  {opt.native}
                </option>
              ))}
            </select>
          </div>
        </div>
      </div>

      <div className="card p-6">
        <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-4">{t('notifications')}</h3>

        <div className="flex items-center justify-between py-3">
          <div className="flex items-center gap-3">
            <Bell size={20} className="text-slate-500" />
            <div>
              <p className="font-medium text-slate-900 dark:text-white">Unread Notifications</p>
              <p className="text-sm text-slate-500">
                {unreadCount} unread notification{unreadCount !== 1 ? 's' : ''}
              </p>
            </div>
          </div>
          <button
            onClick={clearNotifications}
            className="btn btn-secondary text-sm"
            disabled={notifications.length === 0}
          >
            Clear All
          </button>
        </div>

        {notifications.length > 0 && (
          <div className="mt-4 space-y-2">
            {notifications.slice(0, 5).map(notification => (
              <div
                key={notification.id}
                className={`p-3 rounded-lg ${
                  notification.read
                    ? 'bg-slate-50 dark:bg-slate-800/30'
                    : 'bg-primary-50 dark:bg-primary-900/20'
                }`}
              >
                <p className="font-medium text-sm text-slate-900 dark:text-white">
                  {notification.title}
                </p>
                <p className="text-xs text-slate-500 mt-1">{notification.message}</p>
              </div>
            ))}
          </div>
        )}
      </div>

      <div className="card p-6">
        <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-4">About</h3>
        <div className="space-y-2 text-sm text-slate-600 dark:text-slate-400">
          <p><span className="font-medium">Application:</span> Maydon Admin</p>
          <p><span className="font-medium">Version:</span> 1.0.0</p>
          <p><span className="font-medium">Build:</span> React 19 + Vite</p>
        </div>
      </div>
    </div>
  )
}
