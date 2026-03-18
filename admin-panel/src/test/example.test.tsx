import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent } from './utils'

// Example test for a component
describe('Example Component Tests', () => {
    it('should demonstrate basic test setup', () => {
        // This is an example test to verify the testing setup works
        expect(true).toBe(true)
    })

    it('should demonstrate DOM testing', () => {
        // Render a simple element
        render(<button>Click me</button>)

        const button = screen.getByRole('button', { name: /click me/i })
        expect(button).toBeInTheDocument()
    })

    it('should demonstrate event testing', () => {
        const handleClick = vi.fn()
        render(<button onClick={handleClick}>Click me</button>)

        const button = screen.getByRole('button')
        fireEvent.click(button)

        expect(handleClick).toHaveBeenCalledTimes(1)
    })
})

// Example test for form validation
describe('Form Validation', () => {
    it('should validate required fields', async () => {
        const { requiredString } = await import('../lib/validations')

        const schema = requiredString('Name')

        // Valid input
        expect(schema.safeParse('John').success).toBe(true)

        // Invalid input (empty string)
        const result = schema.safeParse('')
        expect(result.success).toBe(false)
    })

    it('should validate email format', async () => {
        const { requiredEmail } = await import('../lib/validations')

        const schema = requiredEmail()

        // Valid email
        expect(schema.safeParse('test@example.com').success).toBe(true)

        // Invalid email
        expect(schema.safeParse('invalid-email').success).toBe(false)
    })
})
