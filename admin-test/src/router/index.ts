import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    // ==================== Guest Routes ====================
    {
      path: '/',
      redirect: '/login',
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('@/views/LoginView.vue'),
      meta: { guest: true },
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('@/views/RegisterView.vue'),
      meta: { guest: true },
    },
    {
      path: '/register/phone',
      name: 'register-phone',
      component: () => import('@/views/PhoneRegisterView.vue'),
      meta: { guest: true },
    },
    {
      path: '/register/eimzo',
      name: 'register-eimzo',
      component: () => import('@/views/EImzoAuthView.vue'),
      meta: { guest: true },
    },
    {
      path: '/forgot-password',
      name: 'forgot-password',
      component: () => import('@/views/ForgotPasswordView.vue'),
      meta: { guest: true },
    },
    {
      path: '/auth/eimzo',
      name: 'auth-eimzo',
      component: () => import('@/views/EImzoAuthView.vue'),
      meta: { guest: true },
    },
    {
      path: '/auth/oneid/callback',
      name: 'oneid-callback',
      component: () => import('@/views/OneIdCallbackView.vue'),
    },
    {
      path: '/callback',
      name: 'callback',
      component: () => import('@/views/OneIdCallbackView.vue'),
    },

    // ==================== Full-Screen Views (No Layout) ====================
    {
      path: '/realestates/:id/floorplan',
      name: 'floorplan-detail',
      component: () => import('@/views/realestates/FloorplanDetailView.vue'),
      meta: { requiresAuth: true },
    },

    // ==================== Authenticated Routes (with Layout) ====================
    {
      path: '/',
      component: () => import('@/layouts/MainLayout.vue'),
      meta: { requiresAuth: true },
      children: [
        {
          path: 'dashboard',
          name: 'dashboard',
          component: () => import('@/views/DashboardView.vue'),
        },
        // Buildings routes - list redirects to realestates (unified view)
        {
          path: 'buildings',
          redirect: '/realestates',
        },
        {
          path: 'buildings/new',
          redirect: '/realestates/new',
        },
        {
          path: 'buildings/:id',
          name: 'building-detail',
          component: () => import('@/views/buildings/BuildingDetailView.vue'),
        },
        {
          path: 'buildings/:id/edit',
          name: 'building-edit',
          component: () => import('@/views/buildings/BuildingCreateView.vue'),
          props: { editMode: true },
        },
        // Real Estates routes
        {
          path: 'realestates',
          name: 'realestates',
          component: () => import('@/views/realestates/RealEstatesListView.vue'),
        },
        {
          path: 'realestates/new',
          name: 'realestate-create',
          component: () => import('@/views/realestates/RealEstateCreateWizard.vue'),
          meta: { hideSidebar: true },
        },
        {
          path: 'realestates/:id',
          name: 'realestate-detail',
          component: () => import('@/views/realestates/RealEstateDetailView.vue'),
        },
        {
          path: 'realestates/:id/edit',
          name: 'realestate-edit',
          component: () => import('@/views/realestates/RealEstateCreateWizard.vue'),
          meta: { hideSidebar: true },
        },

        // Listings routes
        {
          path: 'listings',
          name: 'listings',
          component: () => import('@/views/listings/ListingsListView.vue'),
        },
        {
          path: 'listings/new',
          name: 'listing-create',
          component: () => import('@/views/listings/ListingCreateView.vue'),
          meta: { hideSidebar: true },
        },
        {
          path: 'listings/:id',
          name: 'listing-detail',
          component: () => import('@/views/listings/ListingDetailView.vue'),
        },
        {
          path: 'listings/:id/edit',
          name: 'listing-edit',
          component: () => import('@/views/listings/ListingCreateView.vue'),
          meta: { hideSidebar: true },
        },

        // New Listing Wizard (with mock data)
        {
          path: 'listing-wizard',
          name: 'listing-wizard',
          component: () => import('@/views/listings/CreateListingWizard.vue'),
          meta: { hideSidebar: true },
        },

        // Invitations routes
        {
          path: 'invitations',
          name: 'invitations',
          component: () => import('@/views/invitations/InvitationsListView.vue'),
        },
        {
          path: 'invitations/new',
          name: 'invitation-create',
          component: () => import('@/views/invitations/InvitationCreateView.vue'),
        },
        {
          path: 'invitations/:id',
          name: 'invitation-detail',
          component: () => import('@/views/invitations/InvitationDetailView.vue'),
        },
        {
          path: 'invitations/:id/edit',
          name: 'invitation-edit',
          component: () => import('@/views/invitations/InvitationCreateView.vue'),
        },

        // Contract Templates routes
        {
          path: 'templates',
          name: 'templates',
          component: () => import('@/views/templates/TemplatesListView.vue'),
        },
        {
          path: 'templates/new',
          name: 'template-create',
          component: () => import('@/views/templates/TemplateFormView.vue'),
        },
        {
          path: 'templates/:id',
          name: 'template-detail',
          component: () => import('@/views/templates/TemplateDetailView.vue'),
        },
        {
          path: 'templates/:id/edit',
          name: 'template-edit',
          component: () => import('@/views/templates/TemplateFormView.vue'),
          props: { editMode: true },
        },

        // Contracts routes
        {
          path: 'contracts',
          name: 'contracts',
          component: () => import('@/views/contracts/ContractsListView.vue'),
        },
        {
          path: 'contracts/new',
          name: 'contract-create',
          component: () => import('@/views/contracts/ContractCreateView.vue'),
        },
        {
          path: 'contracts/:id',
          name: 'contract-detail',
          component: () => import('@/views/contracts/ContractDetailView.vue'),
        },

        // Settings route
        {
          path: 'settings',
          name: 'settings',
          component: () => import('@/views/settings/SettingsView.vue'),
        },

        {
          path: 'tenants',
          name: 'tenants',
          component: () => import('@/views/DashboardView.vue'), // Placeholder
        },
        {
          path: 'requests',
          name: 'requests',
          component: () => import('@/views/listingRequests/ListingRequestsListView.vue'),
        },

        // Communal (Utilities) routes
        {
          path: 'communal',
          name: 'communal',
          component: () => import('@/views/meters/UtilitiesDashboardView.vue'),
        },
        {
          path: 'communal/meters',
          name: 'communal-meters',
          component: () => import('@/views/meters/MetersListView.vue'),
        },

        // ==================== Client Marketplace Routes ====================
        {
          path: 'marketplace',
          name: 'marketplace',
          component: () => import('@/views/marketplace/MarketplaceView.vue'),
        },
        {
          path: 'marketplace/:id',
          name: 'marketplace-listing-detail',
          component: () => import('@/views/marketplace/ClientListingDetailView.vue'),
        },
        {
          path: 'my-requests',
          name: 'my-requests',
          component: () => import('@/views/marketplace/ClientRequestsView.vue'),
        },
      ],
    },
  ],
})

// Navigation guards
router.beforeEach((to, _from, next) => {
  const authStore = useAuthStore()

  // Check if route requires authentication
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next({ name: 'login', query: { redirect: to.fullPath } })
    return
  }

  // If user is authenticated and trying to access guest-only pages
  if (to.meta.guest && authStore.isAuthenticated) {
    // Redirect based on account type: 0 = Client (marketplace), 1 = Owner (dashboard)
    const accountType = authStore.userInfo?.accountType
    if (accountType === 0) {
      next({ name: 'marketplace' })
    } else {
      next({ name: 'dashboard' })
    }
    return
  }

  // Account type based route restrictions for authenticated users
  if (authStore.isAuthenticated && to.matched.some(r => r.meta.requiresAuth)) {
    const accountType = authStore.userInfo?.accountType

    // Owner routes (accountType 1 can access these)
    const ownerRoutes = ['dashboard', 'realestates', 'realestate-create', 'realestate-detail', 'realestate-edit',
      'buildings', 'building-detail', 'building-edit',
      'listings', 'listing-create', 'listing-detail', 'listing-edit', 'listing-wizard',
      'invitations', 'invitation-create', 'invitation-detail', 'invitation-edit',
      'requests', 'tenants', 'floorplan-detail', 'communal']

    const routeName = to.name as string

    // If client (accountType 0) tries to access owner routes
    if (accountType === 0 && ownerRoutes.includes(routeName)) {
      next({ name: 'marketplace' })
      return
    }

    // If owner (accountType 1) tries to access client-only routes (except settings which is shared)
    if (accountType === 1 && routeName === 'my-requests') {
      next({ name: 'dashboard' })
      return
    }
  }

  next()
})

export default router
