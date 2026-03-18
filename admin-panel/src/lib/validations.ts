import { z } from 'zod'

// Common validation schemas
export const requiredString = (fieldName: string) =>
    z.string().min(1, `${fieldName} is required`)

export const optionalString = () => z.string().optional()

export const requiredEmail = () =>
    z.string().min(1, 'Email is required').email('Invalid email address')

export const requiredPassword = (minLength = 8) =>
    z
        .string()
        .min(1, 'Password is required')
        .min(minLength, `Password must be at least ${minLength} characters`)

export const optionalNumber = () => z.number().optional()

export const requiredNumber = (fieldName: string) =>
    z.number().refine((val) => val !== undefined && val !== null, {
        message: `${fieldName} is required`,
    })

export const requiredPositiveNumber = (fieldName: string) =>
    z
        .number()
        .positive(`${fieldName} must be positive`)
        .refine((val) => val !== undefined && val !== null, {
            message: `${fieldName} is required`,
        })

export const requiredUUID = (fieldName: string) =>
    z.string().min(1, `${fieldName} is required`).uuid(`Invalid ${fieldName}`)

export const optionalUUID = () => z.string().uuid().optional().or(z.literal(''))

// Common entity schemas
export const baseEntitySchema = z.object({
    id: z.string().uuid().optional(),
    createdAt: z.string().datetime().optional(),
    updatedAt: z.string().datetime().optional(),
})

// Localized name schema (for multi-language fields)
export const localizedNameSchema = z.object({
    uz: z.string().min(1, 'Uzbek name is required'),
    ru: z.string().min(1, 'Russian name is required'),
    en: z.string().min(1, 'English name is required'),
})

// Address schema
export const addressSchema = z.object({
    street: z.string().optional(),
    city: z.string().optional(),
    state: z.string().optional(),
    zipCode: z.string().optional(),
    country: z.string().optional(),
})

// Pagination schema
export const paginationSchema = z.object({
    pageNumber: z.number().min(1).default(1),
    pageSize: z.number().min(1).max(100).default(10),
})

// Example entity schemas
export const regionSchema = z.object({
    id: z.string().uuid().optional(),
    name: localizedNameSchema,
    code: z.string().min(1, 'Code is required'),
    isActive: z.boolean().default(true),
})

export const districtSchema = z.object({
    id: z.string().uuid().optional(),
    name: localizedNameSchema,
    regionId: requiredUUID('Region'),
    code: z.string().min(1, 'Code is required'),
    isActive: z.boolean().default(true),
})

export const bankSchema = z.object({
    id: z.string().uuid().optional(),
    name: requiredString('Name'),
    code: z.string().min(1, 'Code is required'),
    swift: z.string().optional(),
    isActive: z.boolean().default(true),
})

export const currencySchema = z.object({
    id: z.string().uuid().optional(),
    name: requiredString('Name'),
    code: z.string().min(1, 'Code is required').max(3, 'Code must be 3 characters'),
    symbol: z.string().min(1, 'Symbol is required'),
    isActive: z.boolean().default(true),
})

export const loginSchema = z.object({
    email: requiredEmail(),
    password: requiredPassword(),
})

// Type exports from schemas
export type RegionFormData = z.infer<typeof regionSchema>
export type DistrictFormData = z.infer<typeof districtSchema>
export type BankFormData = z.infer<typeof bankSchema>
export type CurrencyFormData = z.infer<typeof currencySchema>
export type LoginFormData = z.infer<typeof loginSchema>
export type LocalizedName = z.infer<typeof localizedNameSchema>
