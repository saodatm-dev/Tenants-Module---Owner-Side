import {
    useForm,
    UseFormReturn,
    FieldValues,
    DefaultValues,
    Path,
    SubmitHandler,
} from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { clsx } from 'clsx'

// Generic hook for creating validated forms
export function useValidatedForm<T extends FieldValues>(
    schema: z.ZodType<T>,
    defaultValues?: DefaultValues<T>
) {
    return useForm<T>({
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        resolver: zodResolver(schema as any),
        defaultValues,
        mode: 'onBlur',
    })
}

// Form field wrapper component
interface FormFieldProps<T extends FieldValues> {
    form: UseFormReturn<T>
    name: Path<T>
    label: string
    type?: 'text' | 'email' | 'password' | 'number' | 'textarea' | 'select'
    placeholder?: string
    options?: { value: string; label: string }[]
    disabled?: boolean
    required?: boolean
    className?: string
}

export function FormField<T extends FieldValues>({
    form,
    name,
    label,
    type = 'text',
    placeholder,
    options,
    disabled,
    required,
    className,
}: FormFieldProps<T>) {
    const {
        register,
        formState: { errors },
    } = form

    const error = errors[name]
    const errorMessage = error?.message as string | undefined

    const inputClasses = clsx(
        'input',
        error && 'border-red-500 focus:ring-red-500',
        disabled && 'opacity-50 cursor-not-allowed',
        className
    )

    const renderInput = () => {
        if (type === 'textarea') {
            return (
                <textarea
                    {...register(name)}
                    placeholder={placeholder}
                    disabled={disabled}
                    className={clsx(inputClasses, 'min-h-[100px] resize-y')}
                    rows={4}
                />
            )
        }

        if (type === 'select' && options) {
            return (
                <select {...register(name)} disabled={disabled} className={inputClasses}>
                    <option value="">{placeholder || 'Select...'}</option>
                    {options.map(option => (
                        <option key={option.value} value={option.value}>
                            {option.label}
                        </option>
                    ))}
                </select>
            )
        }

        return (
            <input
                {...register(name, { valueAsNumber: type === 'number' })}
                type={type}
                placeholder={placeholder}
                disabled={disabled}
                className={inputClasses}
            />
        )
    }

    return (
        <div className="space-y-1.5">
            <label className="block text-sm font-medium text-slate-700 dark:text-slate-300">
                {label}
                {required && <span className="text-red-500 ml-1">*</span>}
            </label>
            {renderInput()}
            {errorMessage && (
                <p className="text-sm text-red-500 flex items-center gap-1">
                    <svg className="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                        <path
                            fillRule="evenodd"
                            d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z"
                            clipRule="evenodd"
                        />
                    </svg>
                    {errorMessage}
                </p>
            )}
        </div>
    )
}

// Localized input for multi-language fields
interface LocalizedFieldProps<T extends FieldValues> {
    form: UseFormReturn<T>
    baseName: string
    label: string
    required?: boolean
}

export function LocalizedField<T extends FieldValues>({
    form,
    baseName,
    label,
    required,
}: LocalizedFieldProps<T>) {
    const languages = [
        { code: 'uz', label: '🇺🇿 Uzbek' },
        { code: 'ru', label: '🇷🇺 Russian' },
        { code: 'en', label: '🇺🇸 English' },
    ]

    return (
        <div className="space-y-3">
            <label className="block text-sm font-medium text-slate-700 dark:text-slate-300">
                {label}
                {required && <span className="text-red-500 ml-1">*</span>}
            </label>
            <div className="grid gap-3">
                {languages.map(lang => (
                    <FormField
                        key={lang.code}
                        form={form}
                        name={`${baseName}.${lang.code}` as Path<T>}
                        label={lang.label}
                        placeholder={`Enter ${label.toLowerCase()} in ${lang.label.split(' ')[1]}`}
                    />
                ))}
            </div>
        </div>
    )
}

// Form submit button
interface SubmitButtonProps {
    isLoading?: boolean
    loadingText?: string
    children: React.ReactNode
    className?: string
}

export function SubmitButton({
    isLoading,
    loadingText = 'Saving...',
    children,
    className,
}: SubmitButtonProps) {
    return (
        <button
            type="submit"
            disabled={isLoading}
            className={clsx(
                'btn btn-primary flex items-center justify-center gap-2',
                isLoading && 'opacity-70 cursor-not-allowed',
                className
            )}
        >
            {isLoading && (
                <svg className="animate-spin h-4 w-4" viewBox="0 0 24 24">
                    <circle
                        className="opacity-25"
                        cx="12"
                        cy="12"
                        r="10"
                        stroke="currentColor"
                        strokeWidth="4"
                        fill="none"
                    />
                    <path
                        className="opacity-75"
                        fill="currentColor"
                        d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                    />
                </svg>
            )}
            {isLoading ? loadingText : children}
        </button>
    )
}

export { useForm, zodResolver }
export type { UseFormReturn, SubmitHandler }
