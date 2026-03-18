import { Navigate, useLocation } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'

interface PrivateRouteProps {
    children: React.ReactNode
    requiredPermissions?: string[]
    fallbackPath?: string
}

export default function PrivateRoute({
    children,
    requiredPermissions = [],
    fallbackPath = '/login',
}: PrivateRouteProps) {
    const { user, isLoading, isAuthenticated } = useAuth()
    const location = useLocation()

    // Show loading state while checking authentication
    if (isLoading) {
        return (
            <div className="min-h-screen flex items-center justify-center bg-slate-50 dark:bg-slate-950">
                <div className="text-center space-y-4">
                    <div className="animate-spin rounded-full h-12 w-12 border-4 border-primary-500 border-t-transparent mx-auto" />
                    <p className="text-slate-600 dark:text-slate-400">Loading...</p>
                </div>
            </div>
        )
    }

    // Redirect to login if not authenticated
    if (!isAuthenticated) {
        return <Navigate to={fallbackPath} state={{ from: location }} replace />
    }

    // Check permissions if required
    if (requiredPermissions.length > 0 && user) {
        const userPermissions = user.permissions || []
        const hasRequiredPermissions = requiredPermissions.every(
            permission => userPermissions.includes(permission)
        )

        if (!hasRequiredPermissions) {
            return (
                <div className="min-h-screen flex items-center justify-center bg-slate-50 dark:bg-slate-950">
                    <div className="text-center space-y-4 max-w-md mx-auto px-4">
                        <div className="w-16 h-16 bg-red-100 dark:bg-red-900/30 rounded-full flex items-center justify-center mx-auto">
                            <svg
                                className="w-8 h-8 text-red-500"
                                fill="none"
                                stroke="currentColor"
                                viewBox="0 0 24 24"
                            >
                                <path
                                    strokeLinecap="round"
                                    strokeLinejoin="round"
                                    strokeWidth={2}
                                    d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"
                                />
                            </svg>
                        </div>
                        <h2 className="text-xl font-semibold text-slate-900 dark:text-slate-100">
                            Access Denied
                        </h2>
                        <p className="text-slate-600 dark:text-slate-400">
                            You don't have permission to access this page.
                        </p>
                        <button
                            onClick={() => window.history.back()}
                            className="btn btn-primary"
                        >
                            Go Back
                        </button>
                    </div>
                </div>
            )
        }
    }

    return <>{children}</>
}

// HOC for wrapping components with auth protection
export function withAuth<P extends object>(
    WrappedComponent: React.ComponentType<P>,
    requiredPermissions?: string[]
) {
    return function AuthenticatedComponent(props: P) {
        return (
            <PrivateRoute requiredPermissions={requiredPermissions}>
                <WrappedComponent {...props} />
            </PrivateRoute>
        )
    }
}
