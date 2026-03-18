import { useState } from 'react'
import { NavLink, useLocation } from 'react-router-dom'
import { useApp } from '../context/AppContext'
import clsx from 'clsx'

// Custom SVG Icons matching admin-test design
const DashboardIcon = () => (
  <svg width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M0.750861 4.14962C0.750938 2.84032 0.750976 2.18566 1.0457 1.70476C1.21062 1.43566 1.43688 1.20942 1.70598 1.04452C2.18691 0.749818 2.84156 0.749818 4.15086 0.749818H4.35C5.65937 0.749818 6.31406 0.749818 6.795 1.04454C7.06411 1.20945 7.29037 1.43571 7.45528 1.70482C7.75 2.18576 7.75 2.84045 7.75 4.14982V15.3503C7.75 16.6597 7.75 17.3143 7.45528 17.7953C7.29037 18.0644 7.06411 18.2906 6.795 18.4556C6.31406 18.7503 5.65937 18.7503 4.35 18.7503H4.1502C2.84076 18.7503 2.18604 18.7503 1.70509 18.4555C1.43597 18.2906 1.2097 18.0643 1.0448 17.7952C0.750085 17.3142 0.750123 16.6595 0.7502 15.3501L0.750861 4.14962Z" stroke="currentColor" strokeWidth="1.5" />
    <path d="M11.75 4.15001C11.75 2.84047 11.75 2.18569 12.0448 1.70472C12.2097 1.43559 12.436 1.20933 12.7052 1.04443C13.1862 0.749723 13.841 0.749819 15.1505 0.750012L15.3505 0.750041C16.6597 0.750234 17.3143 0.75033 17.7952 1.04507C18.0642 1.20999 18.2905 1.43624 18.4553 1.70534C18.75 2.18624 18.75 2.84084 18.75 4.15004V9.35027C18.75 10.6596 18.75 11.3143 18.4553 11.7953C18.2904 12.0644 18.0641 12.2906 17.795 12.4556C17.3141 12.7503 16.6594 12.7503 15.35 12.7503H15.15C13.8406 12.7503 13.1859 12.7503 12.705 12.4556C12.4359 12.2906 12.2096 12.0644 12.0447 11.7953C11.75 11.3143 11.75 10.6596 11.75 9.35027V4.15001Z" stroke="currentColor" strokeWidth="1.5" />
  </svg>
)

const UsersIcon = () => (
  <svg width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path fillRule="evenodd" clipRule="evenodd" d="M12 3C14.2782 3 16.125 4.84683 16.125 7.125C16.125 9.40317 14.2782 11.25 12 11.25C9.72183 11.25 7.875 9.40317 7.875 7.125C7.875 4.84683 9.72183 3 12 3Z" stroke="currentColor" strokeWidth="1.5" />
    <path fillRule="evenodd" clipRule="evenodd" d="M12 12C15.7279 12 18.75 14.0147 18.75 16.5C18.75 18.9853 15.7279 21 12 21C8.27208 21 5.25 18.9853 5.25 16.5C5.25 14.0147 8.27208 12 12 12Z" stroke="currentColor" strokeWidth="1.5" />
  </svg>
)


const MapPinIcon = () => (
  <svg width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M12 13C13.6569 13 15 11.6569 15 10C15 8.34315 13.6569 7 12 7C10.3431 7 9 8.34315 9 10C9 11.6569 10.3431 13 12 13Z" stroke="currentColor" strokeWidth="1.5" />
    <path d="M12 22C16 18 20 14.4183 20 10C20 5.58172 16.4183 2 12 2C7.58172 2 4 5.58172 4 10C4 14.4183 8 18 12 22Z" stroke="currentColor" strokeWidth="1.5" />
  </svg>
)




const BankIcon = () => (
  <svg width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M3 21H21M4 18H20M6 18V10M10 18V10M14 18V10M18 18V10M12 2L22 8H2L12 2Z" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round" />
  </svg>
)

const CoinsIcon = () => (
  <svg width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
    <ellipse cx="9" cy="9" rx="7" ry="4" stroke="currentColor" strokeWidth="1.5" />
    <path d="M16 11C16 13.2091 12.866 15 9 15C5.13401 15 2 13.2091 2 11" stroke="currentColor" strokeWidth="1.5" />
    <path d="M16 15C16 17.2091 12.866 19 9 19C5.13401 19 2 17.2091 2 15" stroke="currentColor" strokeWidth="1.5" />
    <path d="M22 9C22 11.2091 18.866 13 15 13" stroke="currentColor" strokeWidth="1.5" />
    <path d="M22 13C22 15.2091 18.866 17 15 17" stroke="currentColor" strokeWidth="1.5" />
  </svg>
)

const LanguagesIcon = () => (
  <svg width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M5 8L10 16L15 8" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round" />
    <path d="M4 14H16" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" />
    <path d="M10 3V5" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" />
    <path d="M10 5C7 5 4 7 4 7M10 5C13 5 16 7 16 7" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" />
  </svg>
)

const SettingsIcon = () => (
  <svg width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M12 15C13.6569 15 15 13.6569 15 12C15 10.3431 13.6569 9 12 9C10.3431 9 9 10.3431 9 12C9 13.6569 10.3431 15 12 15Z" stroke="currentColor" strokeWidth="1.5" />
    <path d="M19.4 15C19.2669 15.3016 19.2272 15.6362 19.286 15.9606C19.3448 16.285 19.4995 16.5843 19.73 16.82L19.79 16.88C19.976 17.0657 20.1235 17.2863 20.2241 17.5291C20.3248 17.7719 20.3766 18.0322 20.3766 18.295C20.3766 18.5578 20.3248 18.8181 20.2241 19.0609C20.1235 19.3037 19.976 19.5243 19.79 19.71C19.6043 19.896 19.3837 20.0435 19.1409 20.1441C18.8981 20.2448 18.6378 20.2966 18.375 20.2966C18.1122 20.2966 17.8519 20.2448 17.6091 20.1441C17.3663 20.0435 17.1457 19.896 16.96 19.71L16.9 19.65C16.6643 19.4195 16.365 19.2648 16.0406 19.206C15.7162 19.1472 15.3816 19.1869 15.08 19.32C14.7842 19.4468 14.532 19.6572 14.3543 19.9255C14.1766 20.1938 14.0813 20.5082 14.08 20.83V21C14.08 21.5304 13.8693 22.0391 13.4942 22.4142C13.1191 22.7893 12.6104 23 12.08 23C11.5496 23 11.0409 22.7893 10.6658 22.4142C10.2907 22.0391 10.08 21.5304 10.08 21V20.91C10.0723 20.579 9.96512 20.258 9.77251 19.9887C9.5799 19.7194 9.31074 19.5143 9 19.4C8.69838 19.2669 8.36381 19.2272 8.03941 19.286C7.71502 19.3448 7.41568 19.4995 7.18 19.73L7.12 19.79C6.93425 19.976 6.71368 20.1235 6.47088 20.2241C6.22808 20.3248 5.96783 20.3766 5.705 20.3766C5.44217 20.3766 5.18192 20.3248 4.93912 20.2241C4.69632 20.1235 4.47575 19.976 4.29 19.79C4.10405 19.6043 3.95653 19.3837 3.85588 19.1409C3.75523 18.8981 3.70343 18.6378 3.70343 18.375C3.70343 18.1122 3.75523 17.8519 3.85588 17.6091C3.95653 17.3663 4.10405 17.1457 4.29 16.96L4.35 16.9C4.58054 16.6643 4.73519 16.365 4.794 16.0406C4.85282 15.7162 4.81312 15.3816 4.68 15.08C4.55324 14.7842 4.34276 14.532 4.07447 14.3543C3.80618 14.1766 3.49179 14.0813 3.17 14.08H3C2.46957 14.08 1.96086 13.8693 1.58579 13.4942C1.21071 13.1191 1 12.6104 1 12.08C1 11.5496 1.21071 11.0409 1.58579 10.6658C1.96086 10.2907 2.46957 10.08 3 10.08H3.09C3.42099 10.0723 3.742 9.96512 4.0113 9.77251C4.28059 9.5799 4.48572 9.31074 4.6 9C4.73312 8.69838 4.77282 8.36381 4.714 8.03941C4.65519 7.71502 4.50054 7.41568 4.27 7.18L4.21 7.12C4.02405 6.93425 3.87653 6.71368 3.77588 6.47088C3.67523 6.22808 3.62343 5.96783 3.62343 5.705C3.62343 5.44217 3.67523 5.18192 3.77588 4.93912C3.87653 4.69632 4.02405 4.47575 4.21 4.29C4.39575 4.10405 4.61632 3.95653 4.85912 3.85588C5.10192 3.75523 5.36217 3.70343 5.625 3.70343C5.88783 3.70343 6.14808 3.75523 6.39088 3.85588C6.63368 3.95653 6.85425 4.10405 7.04 4.29L7.1 4.35C7.33568 4.58054 7.63502 4.73519 7.95941 4.794C8.28381 4.85282 8.61838 4.81312 8.92 4.68H9C9.29577 4.55324 9.54802 4.34276 9.72569 4.07447C9.90337 3.80618 9.99872 3.49179 10 3.17V3C10 2.46957 10.2107 1.96086 10.5858 1.58579C10.9609 1.21071 11.4696 1 12 1C12.5304 1 13.0391 1.21071 13.4142 1.58579C13.7893 1.96086 14 2.46957 14 3V3.09C14.0013 3.41179 14.0966 3.72618 14.2743 3.99447C14.452 4.26276 14.7042 4.47324 15 4.6C15.3016 4.73312 15.6362 4.77282 15.9606 4.714C16.285 4.65519 16.5843 4.50054 16.82 4.27L16.88 4.21C17.0657 4.02405 17.2863 3.87653 17.5291 3.77588C17.7719 3.67523 18.0322 3.62343 18.295 3.62343C18.5578 3.62343 18.8181 3.67523 19.0609 3.77588C19.3037 3.87653 19.5243 4.02405 19.71 4.21C19.896 4.39575 20.0435 4.61632 20.1441 4.85912C20.2448 5.10192 20.2966 5.36217 20.2966 5.625C20.2966 5.88783 20.2448 6.14808 20.1441 6.39088C20.0435 6.63368 19.896 6.85425 19.71 7.04L19.65 7.1C19.4195 7.33568 19.2648 7.63502 19.206 7.95941C19.1472 8.28381 19.1869 8.61838 19.32 8.92V9C19.4468 9.29577 19.6572 9.54802 19.9255 9.72569C20.1938 9.90337 20.5082 9.99872 20.83 10H21C21.5304 10 22.0391 10.2107 22.4142 10.5858C22.7893 10.9609 23 11.4696 23 12C23 12.5304 22.7893 13.0391 22.4142 13.4142C22.0391 13.7893 21.5304 14 21 14H20.91C20.5882 14.0013 20.2738 14.0966 20.0055 14.2743C19.7372 14.452 19.5268 14.7042 19.4 15Z" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round" />
  </svg>
)

const ShieldIcon = () => (
  <svg width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M12 22C12 22 20 18 20 12V5L12 2L4 5V12C4 18 12 22 12 22Z" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round" />
    <path d="M9 12L11 14L15 10" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round" />
  </svg>
)




const ChevronDownIcon = () => (
  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M6 9L12 15L18 9" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
  </svg>
)

const ChevronLeftIcon = () => (
  <svg width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M15 18L9 12L15 6" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
  </svg>
)

const ChevronRightIcon = () => (
  <svg width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M9 18L15 12L9 6" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
  </svg>
)

// Maydon RENT Logo SVG
const MaydonLogo = () => (
  <svg xmlns="http://www.w3.org/2000/svg" width="123" height="22" viewBox="0 0 123 22" fill="none">
    <g clipPath="url(#clip0_sidebar)">
      <path d="M0 6.72256V21.7456H5.92605V10.0847L11.8521 6.72256V0L0 6.72256Z" fill="#FC4D3D" />
      <path d="M17.7781 3.36214L11.8521 6.72256V21.7456H17.7781V10.0847L23.7041 6.72256V0L17.7781 3.36214Z" fill="#FC4D3D" />
      <path d="M23.7041 21.7457H29.6319V10.0848L23.7041 6.72266V21.7457Z" fill="#FC4D3D" />
      <path d="M29.6318 3.36214V10.0847L35.5579 6.72256V0L29.6318 3.36214Z" fill="#FC4D3D" />
      <path d="M105.535 3.10087C105.6 3.08024 105.659 3.05789 105.72 3.03211C106.014 2.90663 106.24 2.72099 106.397 2.47863C106.557 2.23971 106.634 1.95265 106.634 1.62091C106.634 1.1104 106.459 0.713337 106.107 0.428002C105.757 0.142667 105.278 0 104.667 0H102.87V4.72178H103.376V3.22463H104.667C104.802 3.22463 104.933 3.21775 105.055 3.19884L106.151 4.72178H106.711L105.535 3.09915V3.10087ZM104.655 2.80006H103.378V0.43144H104.655C105.138 0.43144 105.502 0.534573 105.75 0.742558C106 0.947105 106.124 1.24275 106.124 1.62091C106.124 1.99906 106 2.28268 105.75 2.48894C105.5 2.69693 105.136 2.80006 104.655 2.80006Z" fill="#2B2B2B" />
      <path d="M111.853 4.29034V4.72349H108.455V0H111.751V0.43144H108.964V2.11079H111.449V2.53707H108.964V4.29034H111.853Z" fill="#2B2B2B" />
      <path d="M117.684 0V4.72349H117.266L114.175 0.892101V4.72349H113.666V0H114.084L117.182 3.82967V0H117.684Z" fill="#2B2B2B" />
      <path d="M120.802 4.72349V0.43144H119.112V0H123V0.43144H121.31V4.72178H120.801L120.802 4.72349Z" fill="#2B2B2B" />
      <path d="M114.433 21.7454H112.169V9.82666H114.257L120.736 17.8281V9.82666H123V21.7454H120.911L114.433 13.7595V21.7454Z" fill="#2B2B2B" />
      <path d="M103.696 9.57227C105.546 9.57227 107.073 10.1687 108.279 11.3599C109.507 12.5287 110.123 14.0053 110.123 15.7878C110.123 17.5702 109.509 19.0399 108.279 20.2311C107.063 21.4119 105.535 22.0015 103.696 22.0015C101.857 22.0015 100.354 21.4119 99.1157 20.2311C97.8979 19.0278 97.2891 17.5462 97.2891 15.7878C97.2891 14.0293 97.8979 12.5408 99.1157 11.3599C100.344 10.1687 101.871 9.57227 103.696 9.57227ZM100.871 12.9258C100.122 13.6856 99.7473 14.6395 99.7473 15.786C99.7473 16.9325 100.122 17.8848 100.871 18.6463C101.619 19.4077 102.562 19.7876 103.696 19.7876C104.83 19.7876 105.773 19.4077 106.522 18.6463C107.283 17.8745 107.663 16.9205 107.663 15.786C107.663 14.6516 107.281 13.6976 106.522 12.9258C105.761 12.154 104.82 11.7673 103.696 11.7673C102.573 11.7673 101.63 12.154 100.871 12.9258Z" fill="#2B2B2B" />
      <path d="M89.1581 21.7454H84.1733V9.82666H89.1581C90.983 9.82666 92.5051 10.4008 93.7229 11.5473C94.9511 12.6938 95.567 14.1067 95.567 15.786C95.567 17.4654 94.9529 18.8783 93.7229 20.0248C92.5051 21.1713 90.9847 21.7454 89.1581 21.7454ZM89.1581 11.9374H86.5249V19.6329H89.1581C90.2813 19.6329 91.2244 19.2633 91.9837 18.5259C92.7448 17.7662 93.1245 16.8517 93.1245 15.786C93.1245 14.7203 92.7431 13.7818 91.9837 13.0444C91.2227 12.307 90.2813 11.9374 89.1581 11.9374Z" fill="#2B2B2B" />
      <path d="M75.5792 16.8758L70.9111 9.82666H73.6301L76.7374 14.7306L79.9498 9.82666H82.6547L77.9325 16.807V21.7454H75.5792V16.8758Z" fill="#2B2B2B" />
      <path d="M61.7536 21.7454H59.2778L64.5443 9.82666H66.4584L71.7248 21.7454H69.2508L68.285 19.4988H62.7194L61.7536 21.7454ZM67.3717 17.4035L65.5101 13.0788L63.6309 17.4035H67.3699H67.3717Z" fill="#2B2B2B" />
      <path d="M46.6086 21.7454H44.2554V9.82666H46.6086L51.0142 17.3519L55.4041 9.82666H57.7556V21.7454H55.4041V14.1514L51.7876 20.3995H50.2252L46.6086 14.1686V21.7454Z" fill="#2B2B2B" />
    </g>
    <defs>
      <clipPath id="clip0_sidebar">
        <rect width="123" height="22" fill="white" />
      </clipPath>
    </defs>
  </svg>
)

interface NavItem {
  path: string
  label: string
  icon: React.ReactNode
}

interface NavSection {
  id: string
  title: string
  items: NavItem[]
}

export default function Sidebar() {
  const { sidebarCollapsed, toggleSidebar, t } = useApp()
  const location = useLocation()

  const navSections: NavSection[] = [
    {
      id: 'main',
      title: t('main'),
      items: [
        { path: '/', label: t('dashboard'), icon: <DashboardIcon /> },
        { path: '/moderation', label: t('moderation'), icon: <ShieldIcon /> },
        { path: '/users', label: t('users') || 'Users', icon: <UsersIcon /> },
        { path: '/hierarchy', label: t('locationHierarchy') || 'Location Hierarchy', icon: <MapPinIcon /> },
        { path: '/property-config', label: t('propertyConfig') || 'Property Config', icon: <SettingsIcon /> },
      ],
    },
    {
      id: 'system',
      title: t('system'),
      items: [
        { path: '/banks', label: t('banks'), icon: <BankIcon /> },
        { path: '/currencies', label: t('currencies'), icon: <CoinsIcon /> },
        { path: '/languages', label: t('languages'), icon: <LanguagesIcon /> },
        { path: '/settings', label: t('settings'), icon: <SettingsIcon /> },
      ],
    },
  ]

  // Initialize expanded sections - sections with active route are expanded by default
  const getInitialExpandedSections = () => {
    const expanded: Record<string, boolean> = {}
    navSections.forEach(section => {
      const hasActiveItem = section.items.some(item =>
        item.path === location.pathname ||
        (item.path !== '/' && location.pathname.startsWith(item.path))
      )
      expanded[section.id] = hasActiveItem || section.id === 'main'
    })
    return expanded
  }

  const [expandedSections, setExpandedSections] = useState<Record<string, boolean>>(getInitialExpandedSections)

  const toggleSection = (sectionId: string) => {
    setExpandedSections(prev => ({
      ...prev,
      [sectionId]: !prev[sectionId]
    }))
  }

  return (
    <aside
      className={clsx(
        'fixed left-0 top-0 h-screen bg-white dark:bg-slate-900 border-r border-slate-200 dark:border-slate-800 transition-all duration-300 z-40 flex flex-col',
        sidebarCollapsed ? 'w-16' : 'w-64'
      )}
    >
      <div className="h-16 flex items-center justify-between px-4 border-b border-slate-200 dark:border-slate-800">
        {!sidebarCollapsed && (
          <MaydonLogo />
        )}
        <button
          onClick={toggleSidebar}
          className="p-2 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-600 dark:text-slate-400"
        >
          {sidebarCollapsed ? <ChevronRightIcon /> : <ChevronLeftIcon />}
        </button>
      </div>

      <nav className="flex-1 overflow-y-auto scrollbar-thin py-4">
        {navSections.map((section) => {
          const isExpanded = expandedSections[section.id]
          const hasActiveItem = section.items.some(item =>
            item.path === location.pathname ||
            (item.path !== '/' && location.pathname.startsWith(item.path))
          )

          return (
            <div key={section.id} className="mb-2">
              {/* Section Header - Collapsible */}
              {!sidebarCollapsed ? (
                <button
                  onClick={() => toggleSection(section.id)}
                  className={clsx(
                    'w-full flex items-center justify-between px-4 py-2 text-xs font-semibold uppercase tracking-wider transition-colors rounded-lg mx-0',
                    hasActiveItem
                      ? 'text-primary-600 dark:text-primary-400'
                      : 'text-slate-400 hover:text-slate-600 dark:hover:text-slate-300'
                  )}
                >
                  <span>{section.title}</span>
                  <span className={clsx(
                    'transition-transform duration-200',
                    isExpanded ? 'rotate-0' : '-rotate-90'
                  )}>
                    <ChevronDownIcon />
                  </span>
                </button>
              ) : (
                <div className="h-2" />
              )}

              {/* Section Items - Collapsible Content */}
              <div
                className={clsx(
                  'overflow-hidden transition-all duration-200 ease-in-out',
                  sidebarCollapsed ? 'max-h-[500px]' : (isExpanded ? 'max-h-[500px] opacity-100' : 'max-h-0 opacity-0')
                )}
              >
                <ul className="space-y-1 px-2">
                  {section.items.map(item => (
                    <li key={item.path}>
                      <NavLink
                        to={item.path}
                        className={({ isActive }) =>
                          clsx(
                            'flex items-center gap-3 px-3 py-2 rounded-lg transition-colors',
                            isActive
                              ? 'bg-primary-50 text-primary-600 dark:bg-primary-900/20 dark:text-primary-400'
                              : 'text-slate-600 dark:text-slate-400 hover:bg-slate-100 dark:hover:bg-slate-800'
                          )
                        }
                        title={sidebarCollapsed ? item.label : undefined}
                      >
                        {item.icon}
                        {!sidebarCollapsed && <span className="text-sm font-medium">{item.label}</span>}
                      </NavLink>
                    </li>
                  ))}
                </ul>
              </div>
            </div>
          )
        })}
      </nav>
    </aside>
  )
}
