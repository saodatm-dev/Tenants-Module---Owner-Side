import { Toaster, toast } from 'sonner'

// Toast provider component
export function ToastProvider() {
    return (
        <Toaster
            position="top-right"
            expand={false}
            richColors
            closeButton
            theme="system"
            toastOptions={{
                duration: 4000,
                classNames: {
                    toast: 'font-sans',
                    title: 'font-medium',
                    description: 'text-sm opacity-90',
                },
            }}
        />
    )
}

// Toast utility functions
export const showToast = {
    success: (message: string, description?: string) => {
        toast.success(message, { description })
    },

    error: (message: string, description?: string) => {
        toast.error(message, { description })
    },

    warning: (message: string, description?: string) => {
        toast.warning(message, { description })
    },

    info: (message: string, description?: string) => {
        toast.info(message, { description })
    },

    loading: (message: string) => {
        return toast.loading(message)
    },

    promise: <T,>(
        promise: Promise<T>,
        messages: {
            loading: string
            success: string | ((data: T) => string)
            error: string | ((error: Error) => string)
        }
    ) => {
        return toast.promise(promise, messages)
    },

    dismiss: (id?: string | number) => {
        toast.dismiss(id)
    },
}

export { toast }
