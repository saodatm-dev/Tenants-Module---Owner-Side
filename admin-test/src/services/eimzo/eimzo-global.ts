// eimzo-global.ts
// Base64 utilities and CAPIWS WebSocket client for E-IMZO agent communication
/* eslint-disable @typescript-eslint/no-explicit-any */

// Extend Window interface for CAPIWS and Base64
declare global {
    interface Window {
        CAPIWS: typeof CAPIWS
        Base64: typeof Base64
    }
}

// Base64 implementation
const b64chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/'
const b64tab: Record<string, number> = {}
for (let i = 0; i < b64chars.length; i++) {
    b64tab[b64chars.charAt(i)] = i
}

const fromCharCode = String.fromCharCode

const cb_utob = (c: string): string => {
    if (c.length < 2) {
        const cc = c.charCodeAt(0)
        return cc < 0x80 ? c
            : cc < 0x800 ? (fromCharCode(0xc0 | (cc >>> 6)) + fromCharCode(0x80 | (cc & 0x3f)))
                : (fromCharCode(0xe0 | ((cc >>> 12) & 0x0f)) + fromCharCode(0x80 | ((cc >>> 6) & 0x3f)) + fromCharCode(0x80 | (cc & 0x3f)))
    } else {
        const cc = 0x10000 + (c.charCodeAt(0) - 0xD800) * 0x400 + (c.charCodeAt(1) - 0xDC00)
        return (fromCharCode(0xf0 | ((cc >>> 18) & 0x07))
            + fromCharCode(0x80 | ((cc >>> 12) & 0x3f))
            + fromCharCode(0x80 | ((cc >>> 6) & 0x3f))
            + fromCharCode(0x80 | (cc & 0x3f)))
    }
}

const re_utob = /[\uD800-\uDBFF][\uDC00-\uDFFFF]|[^\x00-\x7F]/g
const utob = (u: string): string => u.replace(re_utob, cb_utob)

const cb_encode = (ccc: string): string => {
    const padlen = [0, 2, 1][ccc.length % 3] ?? 0
    const ord = ccc.charCodeAt(0) << 16
        | ((ccc.length > 1 ? ccc.charCodeAt(1) : 0) << 8)
        | ((ccc.length > 2 ? ccc.charCodeAt(2) : 0))
    const chars = [
        b64chars.charAt(ord >>> 18),
        b64chars.charAt((ord >>> 12) & 63),
        padlen >= 2 ? '=' : b64chars.charAt((ord >>> 6) & 63),
        padlen >= 1 ? '=' : b64chars.charAt(ord & 63)
    ]
    return chars.join('')
}

const _btoa = (b: string): string => b.replace(/[\s\S]{1,3}/g, cb_encode)
const _encode = (u: string): string => _btoa(utob(u))

const re_btou = new RegExp([
    '[\xC0-\xDF][\x80-\xBF]',
    '[\xE0-\xEF][\x80-\xBF]{2}',
    '[\xF0-\xF7][\x80-\xBF]{3}'
].join('|'), 'g')

const cb_btou = (cccc: string): string => {
    switch (cccc.length) {
        case 4: {
            const cp = ((0x07 & cccc.charCodeAt(0)) << 18)
                | ((0x3f & cccc.charCodeAt(1)) << 12)
                | ((0x3f & cccc.charCodeAt(2)) << 6)
                | (0x3f & cccc.charCodeAt(3))
            const offset = cp - 0x10000
            return (fromCharCode((offset >>> 10) + 0xD800) + fromCharCode((offset & 0x3FF) + 0xDC00))
        }
        case 3:
            return fromCharCode(
                ((0x0f & cccc.charCodeAt(0)) << 12)
                | ((0x3f & cccc.charCodeAt(1)) << 6)
                | (0x3f & cccc.charCodeAt(2))
            )
        default:
            return fromCharCode(
                ((0x1f & cccc.charCodeAt(0)) << 6)
                | (0x3f & cccc.charCodeAt(1))
            )
    }
}

const btou = (b: string): string => b.replace(re_btou, cb_btou)

const cb_decode = (cccc: string): string => {
    const len = cccc.length
    const padlen = len % 4
    const n = (len > 0 ? (b64tab[cccc.charAt(0)] ?? 0) << 18 : 0)
        | (len > 1 ? (b64tab[cccc.charAt(1)] ?? 0) << 12 : 0)
        | (len > 2 ? (b64tab[cccc.charAt(2)] ?? 0) << 6 : 0)
        | (len > 3 ? (b64tab[cccc.charAt(3)] ?? 0) : 0)
    const chars = [
        fromCharCode(n >>> 16),
        fromCharCode((n >>> 8) & 0xff),
        fromCharCode(n & 0xff)
    ]
    chars.length -= [0, 0, 2, 1][padlen] ?? 0
    return chars.join('')
}

const _atob = (a: string): string => a.replace(/[\s\S]{1,4}/g, cb_decode)
const _decode = (a: string): string => btou(_atob(a))

export const Base64 = {
    VERSION: '2.1.4',
    atob: _atob,
    btoa: _btoa,
    encode: (u: string, urisafe?: boolean): string => {
        return !urisafe
            ? _encode(u)
            : _encode(u).replace(/[+/]/g, (m0) => m0 == '+' ? '-' : '_').replace(/=/g, '')
    },
    decode: (a: string): string => {
        return _decode(
            a.replace(/[-_]/g, (m0) => m0 == '-' ? '+' : '/')
                .replace(/[^A-Za-z0-9+/]/g, '')
        )
    },
    utob,
    btou,
}

// Make Base64 available globally
if (typeof window !== 'undefined') {
    window.Base64 = Base64
}

// CAPIWS WebSocket client
type CallbackFn = (event: MessageEvent, data: any) => void
type ErrorFn = (error: any) => void

export const CAPIWS = {
    URL: (typeof window !== 'undefined' && window.location.protocol.toLowerCase() === 'https:'
        ? 'wss://127.0.0.1:64443'
        : 'ws://127.0.0.1:64646') + '/service/cryptapi',

    callFunction(funcDef: any, callback: CallbackFn, error: ErrorFn): void {
        if (typeof WebSocket === 'undefined') {
            error?.('WebSocket not supported')
            return
        }

        let socket: WebSocket
        try {
            socket = new WebSocket(this.URL)
        } catch (e) {
            error?.(e)
            return
        }

        socket.onerror = () => {
            error?.('Connection error')
        }

        socket.onclose = (e) => {
            if (error && e.code !== 1000) {
                error(e.code)
            }
        }

        socket.onmessage = (event) => {
            const data = JSON.parse(event.data)
            socket.close()
            callback(event, data)
        }

        socket.onopen = () => {
            socket.send(JSON.stringify(funcDef))
        }
    },

    version(callback: CallbackFn, error: ErrorFn): void {
        if (typeof WebSocket === 'undefined') {
            error?.('WebSocket not supported')
            return
        }

        let socket: WebSocket
        try {
            socket = new WebSocket(this.URL)
        } catch (e) {
            error?.(e)
            return
        }

        socket.onerror = () => error?.('Connection error')
        socket.onclose = (e) => {
            if (error && e.code !== 1000) {
                error(e.code)
            }
        }

        socket.onmessage = (event) => {
            const data = JSON.parse(event.data)
            socket.close()
            callback(event, data)
        }

        socket.onopen = () => {
            socket.send(JSON.stringify({ name: 'version' }))
        }
    },

    apikey(domainAndKey: string[], callback: CallbackFn, error: ErrorFn): void {
        if (typeof WebSocket === 'undefined') {
            error?.('WebSocket not supported')
            return
        }

        let socket: WebSocket
        try {
            socket = new WebSocket(this.URL)
        } catch (e) {
            error?.(e)
            return
        }

        socket.onerror = () => error?.('Connection error')
        socket.onclose = (e) => {
            if (error && e.code !== 1000) {
                error(e.code)
            }
        }

        socket.onmessage = (event) => {
            const data = JSON.parse(event.data)
            socket.close()
            callback(event, data)
        }

        socket.onopen = () => {
            socket.send(JSON.stringify({ name: 'apikey', arguments: domainAndKey }))
        }
    },
}

// Make CAPIWS available globally
if (typeof window !== 'undefined') {
    window.CAPIWS = CAPIWS
}
