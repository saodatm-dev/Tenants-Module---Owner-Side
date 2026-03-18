import { Routes, Route } from 'react-router-dom'
import { Layout, PrivateRoute } from './components'
import {
  Login,
  Dashboard,
  Users,
  Banks,
  Currencies,
  LanguagesPage,
  Moderation,
  ModerationListingDetail,
  ModerationRealEstateDetail,
  ModerationUnitDetail,
  Settings,
} from './pages'
import HierarchyPage from './pages/HierarchyPage'
import PropertyConfigPage from './pages/PropertyConfigPage'

// Protected layout wrapper
function ProtectedLayout() {
  return (
    <PrivateRoute>
      <Layout />
    </PrivateRoute>
  )
}

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route element={<ProtectedLayout />}>
        <Route path="/" element={<Dashboard />} />
        <Route path="/users" element={<Users />} />
        <Route path="/moderation" element={<Moderation />} />
        <Route path="/moderation/listings/:id" element={<ModerationListingDetail />} />
        <Route path="/moderation/realestates/:id" element={<ModerationRealEstateDetail />} />
        <Route path="/moderation/units/:id" element={<ModerationUnitDetail />} />

        {/* Hierarchy */}
        <Route path="/hierarchy" element={<HierarchyPage />} />

        {/* Property Config (unified tabbed page) */}
        <Route path="/property-config" element={<PropertyConfigPage />} />

        {/* System */}
        <Route path="/banks" element={<Banks />} />
        <Route path="/currencies" element={<Currencies />} />
        <Route path="/languages" element={<LanguagesPage />} />
        <Route path="/settings" element={<Settings />} />
      </Route>
    </Routes>
  )
}
