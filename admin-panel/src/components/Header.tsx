import { useState, useRef, useEffect } from 'react'
import { useLocation } from 'react-router-dom'
import { useApp } from '../context/AppContext'
import { useAuth } from '../context/AuthContext'
import { Bell, Sun, Moon, LogOut, User, ChevronDown } from 'lucide-react'
import type { LanguageCode } from '../types'

const languageOptions: { code: LanguageCode; label: string; flag: string }[] = [
  { code: 'uz', label: "O'zbekcha", flag: '🇺🇿' },
  { code: 'ru', label: 'Русский', flag: '🇷🇺' },
  { code: 'en', label: 'English', flag: '🇺🇸' },
]

export default function Header() {
  const { theme, setTheme, language, setLanguage, notifications, t } = useApp()
  const { user, logout } = useAuth()
  const location = useLocation()
  const [isLangOpen, setIsLangOpen] = useState(false)
  const langDropdownRef = useRef<HTMLDivElement>(null)

  const unreadCount = notifications.filter(n => !n.read).length
  const currentLang = languageOptions.find(opt => opt.code === language) || languageOptions[0]

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (langDropdownRef.current && !langDropdownRef.current.contains(event.target as Node)) {
        setIsLangOpen(false)
      }
    }
    document.addEventListener('mousedown', handleClickOutside)
    return () => document.removeEventListener('mousedown', handleClickOutside)
  }, [])

  const handleLanguageSelect = (code: LanguageCode) => {
    setLanguage(code)
    setIsLangOpen(false)
  }

  const getPageTitle = () => {
    const path = location.pathname.slice(1) || 'dashboard'
    const key = path.replace(/-/g, '')
    return t(key) || path.charAt(0).toUpperCase() + path.slice(1)
  }

  return (
    <header className="h-16 bg-white dark:bg-slate-900 border-b border-slate-200 dark:border-slate-800 flex items-center justify-between px-6">
      <h1 className="text-xl font-semibold text-slate-900 dark:text-white">
        {getPageTitle()}
      </h1>

      <div className="flex items-center gap-4">
        {/* Language Selector Dropdown */}
        <div className="relative" ref={langDropdownRef}>
          <button
            onClick={() => setIsLangOpen(!isLangOpen)}
            className="flex items-center gap-2 px-3 py-2 rounded-xl border border-slate-200 dark:border-slate-700 bg-gradient-to-b from-white to-slate-50 dark:from-slate-800 dark:to-slate-850 hover:from-slate-50 hover:to-slate-100 dark:hover:from-slate-750 dark:hover:to-slate-800 transition-all duration-200 shadow-sm hover:shadow-md group"
          >
            <span className="text-xl leading-none">{currentLang.flag}</span>
            <span className="text-sm font-medium text-slate-700 dark:text-slate-300 hidden sm:block">
              {currentLang.code}
            </span>
            <ChevronDown
              size={16}
              className={`text-slate-400 transition-transform duration-200 ${isLangOpen ? 'rotate-180' : ''}`}
            />
          </button>

          {/* Dropdown Menu */}
          {isLangOpen && (
            <div className="absolute right-0 mt-2 py-2 bg-white dark:bg-slate-800 rounded-xl border border-slate-200 dark:border-slate-700 shadow-xl shadow-slate-200/50 dark:shadow-slate-900/50 z-50 animate-in fade-in slide-in-from-top-2 duration-200">
              {/* <div className="px-3 pb-2 mb-2 border-b border-slate-100 dark:border-slate-700">
                <div className="flex items-center gap-2 text-xs font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-wider">
                  <Globe size={12} />
                  {t('language') || 'Language'}
                </div>
              </div> */}
              {languageOptions.map(opt => (
                <button
                  key={opt.code}
                  onClick={() => handleLanguageSelect(opt.code)}
                  className={`w-full flex items-center gap-3 px-3 py-2.5 text-left hover:bg-gradient-to-r hover:from-primary-50 hover:to-transparent dark:hover:from-primary-900/20 transition-all duration-150 ${opt.code === language
                    ? 'bg-gradient-to-r from-primary-50 to-transparent dark:from-primary-900/30'
                    : ''
                    }`}
                >
                  <span className="text-2xl leading-none drop-shadow-sm">{opt.flag}</span>
                  <div className="flex flex-col">
                    {/* <span className={`text-sm font-medium ${opt.code === language
                      ? 'text-primary-600 dark:text-primary-400'
                      : 'text-slate-700 dark:text-slate-300'
                      }`}>
                      {opt.label}
                    </span> */}
                    <span className="text-xs text-slate-400 dark:text-slate-500 uppercase">
                      {opt.code}
                    </span>
                  </div>
                  {opt.code === language && (
                    <span className="ml-auto w-2 h-2 rounded-full bg-primary-500 animate-pulse" />
                  )}
                </button>
              ))}
            </div>
          )}
        </div>

        <button className="relative p-2 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-600 dark:text-slate-400">
          <Bell size={20} />
          {unreadCount > 0 && (
            <span className="absolute -top-1 -right-1 w-5 h-5 bg-primary-500 text-white text-xs rounded-full flex items-center justify-center">
              {unreadCount}
            </span>
          )}
        </button>

        <button
          onClick={() => setTheme(theme === 'light' ? 'dark' : 'light')}
          className="p-2 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-600 dark:text-slate-400"
        >
          {theme === 'light' ? <Moon size={20} /> : <Sun size={20} />}
        </button>

        <div className="flex items-center gap-3 pl-4 border-l border-slate-200 dark:border-slate-700">
          <div className="w-8 h-8 bg-primary-100 dark:bg-primary-900/30 rounded-full flex items-center justify-center">
            <User size={16} className="text-primary-600 dark:text-primary-400" />
          </div>
          {user && (
            <span className="text-sm font-medium text-slate-700 dark:text-slate-300">
              {user.firstName || user.phoneNumber}
            </span>
          )}
          <button
            onClick={logout}
            className="p-2 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-600 dark:text-slate-400"
            title={t('logout')}
          >
            <LogOut size={18} />
          </button>
        </div>
      </div>
    </header>
  )
}
