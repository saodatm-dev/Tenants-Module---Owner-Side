// eimzo-client.ts
// High-level E-IMZO client for certificate listing, key loading, and PKCS7 creation
/* eslint-disable @typescript-eslint/no-explicit-any */

import { CAPIWS, Base64 } from './eimzo-global'
import type { EImzoKeyItem } from '@/types/auth'

// Extend String prototype for parsing
declare global {
    interface String {
        splitKeep(splitter: RegExp | string, ahead?: boolean): string[]
    }
}

String.prototype.splitKeep = function (splitter: RegExp | string, ahead?: boolean): string[] {
    const self = this as string
    const result: string[] = []

    if (splitter !== '') {
        const getSubst = (value: string): string => {
            const substChar = value[0] === '0' ? '1' : '0'
            return substChar.repeat(value.length)
        }

        const matches: { value: string; index: number }[] = []
        const replaceFn = (m: string, i: number): string => {
            matches.push({ value: m, index: i })
            return getSubst(m)
        }

        if (splitter instanceof RegExp) {
            self.replace(splitter, (m, ...args) => {
                const index = args[args.length - 2] as number
                return replaceFn(m, index)
            })
        }

        let lastIndex = 0
        for (const m of matches) {
            const nextIndex = ahead ? m.index : m.index + m.value.length
            if (nextIndex !== lastIndex) {
                result.push(self.substring(lastIndex, nextIndex))
                lastIndex = nextIndex
            }
        }

        if (lastIndex < self.length) {
            result.push(self.substring(lastIndex, self.length))
        }
    } else {
        result.push(self)
    }

    return result
}

type SuccessCallback<T> = (result: T) => void
type ErrorCallback = (error: any, reason: string | null) => void

export const EIMZOClient = {
    NEW_API: false,
    NEW_API2: false,
    NEW_API3: false,

    API_KEYS: [
        'localhost', '96D0C1491615C82B9A54D9989779DF825B690748224C2B04F500F370D51827CE2644D8D4A82C18184D73AB8530BB8ED537269603F61DB0D03D2104ABF789970B',
        '127.0.0.1', 'A7BCFA5D490B351BE0754130DF03A068F855DB4333D43921125B9CF2670EF6A40370C646B90401955E1F7BC9CDBF59CE0B2C5467D820BE189C845D0B79CFC96F'
    ],

    checkVersion(success: (major: number, minor: number) => void, fail: ErrorCallback): void {
        CAPIWS.version((_event, data) => {
            if (data.success === true) {
                if (data.major && data.minor) {
                    const installedVersion = parseInt(data.major) * 100 + parseInt(data.minor)
                    EIMZOClient.NEW_API = installedVersion >= 336
                    EIMZOClient.NEW_API2 = installedVersion >= 412
                    EIMZOClient.NEW_API3 = installedVersion >= 486
                    success(data.major, data.minor)
                } else {
                    fail(null, 'E-IMZO Version is undefined')
                }
            } else {
                fail(null, data.reason)
            }
        }, (e) => {
            fail(e, null)
        })
    },

    installApiKeys(success: () => void, fail: ErrorCallback): void {
        CAPIWS.apikey(EIMZOClient.API_KEYS, (_event, data) => {
            if (data.success) {
                success()
            } else {
                fail(null, data.reason)
            }
        }, (e) => {
            fail(e, null)
        })
    },

    listAllUserKeys(
        itemIdGen: (item: any, index: number) => string,
        itemUiGen: (itemId: string, item: any) => EImzoKeyItem,
        success: (items: EImzoKeyItem[], firstId: string | null) => void,
        fail: ErrorCallback
    ): void {
        const items: EImzoKeyItem[] = []
        const errors: { e?: any; r?: string }[] = []

        if (!EIMZOClient.NEW_API) {
            fail(null, 'Please install new version of E-IMZO')
        } else {
            EIMZOClient._findPfxs2(itemIdGen, itemUiGen, items, errors, (firstItmId2) => {
                if (items.length === 0 && errors.length > 0) {
                    const firstError = errors[0]
                    fail(firstError?.e, firstError?.r || null)
                } else {
                    let firstId: string | null = null
                    if (items.length === 1 && firstItmId2) {
                        firstId = firstItmId2
                    }
                    success(items, firstId)
                }
            })
        }
    },

    loadKey(
        itemObject: EImzoKeyItem,
        success: SuccessCallback<string>,
        fail: ErrorCallback,
        verifyPassword?: boolean
    ): void {
        if (itemObject) {
            const vo = itemObject
            if (vo.type === 'pfx') {
                CAPIWS.callFunction(
                    { plugin: 'pfx', name: 'load_key', arguments: [vo.disk, vo.path, vo.name, vo.alias] },
                    (_event, data) => {
                        if (data.success) {
                            const id = data.keyId
                            if (verifyPassword) {
                                CAPIWS.callFunction(
                                    { name: 'verify_password', plugin: 'pfx', arguments: [id] },
                                    (_event2, data2) => {
                                        if (data2.success) {
                                            success(id)
                                        } else {
                                            fail(null, data2.reason)
                                        }
                                    },
                                    (e) => fail(e, null)
                                )
                            } else {
                                success(id)
                            }
                        } else {
                            fail(null, data.reason)
                        }
                    },
                    (e) => fail(e, null)
                )
            }
        }
    },

    createPkcs7(
        id: string,
        data: string,
        _timestamper: unknown,
        success: SuccessCallback<string>,
        fail: ErrorCallback,
        detached?: boolean,
        isDataBase64Encoded?: boolean
    ): void {
        let data64: string
        if (isDataBase64Encoded === true) {
            data64 = data
        } else {
            data64 = Base64.encode(data)
        }

        const detachedStr = detached === true ? 'yes' : 'no'

        CAPIWS.callFunction(
            { plugin: 'pkcs7', name: 'create_pkcs7', arguments: [data64, id, detachedStr] },
            (_event, result) => {
                if (result.success) {
                    success(result.pkcs7_64)
                } else {
                    fail(null, result.reason)
                }
            },
            (e) => fail(e, null)
        )
    },

    // Internal helper to extract X500 values from certificate alias
    _getX500Val(s: string, f: string): string {
        const res = s.splitKeep(/,[A-Z]+=/g, true)
        for (let i = 0; i < res.length; i++) {
            const item = res[i]
            if (!item) continue
            const prefix = i > 0 ? ',' : ''
            const n = item.search(prefix + f + '=')
            if (n !== -1) {
                return item.slice(n + f.length + 1 + (i > 0 ? 1 : 0))
            }
        }
        return ''
    },

    _findPfxs2(
        itemIdGen: (item: any, index: number) => string,
        itemUiGen: (itemId: string, item: any) => EImzoKeyItem,
        items: EImzoKeyItem[],
        errors: { e?: any; r?: string }[],
        callback: (firstItemId: string | null) => void
    ): void {
        let itmkey0: string | null = null

        CAPIWS.callFunction(
            { plugin: 'pfx', name: 'list_all_certificates' },
            (_event, data) => {
                if (data.success) {
                    for (const rec in data.certificates) {
                        const el = data.certificates[rec]
                        let x500name_ex = el.alias.toUpperCase()
                        x500name_ex = x500name_ex.replace('1.2.860.3.16.1.1=', 'INN=')
                        x500name_ex = x500name_ex.replace('1.2.860.3.16.1.2=', 'PINFL=')

                        const vo: EImzoKeyItem = {
                            disk: el.disk,
                            path: el.path,
                            name: el.name,
                            alias: el.alias,
                            serialNumber: EIMZOClient._getX500Val(x500name_ex, 'SERIALNUMBER'),
                            validFrom: new Date(EIMZOClient._getX500Val(x500name_ex, 'VALIDFROM').replace(/\./g, '-').replace(' ', 'T')),
                            validTo: new Date(EIMZOClient._getX500Val(x500name_ex, 'VALIDTO').replace(/\./g, '-').replace(' ', 'T')),
                            CN: EIMZOClient._getX500Val(x500name_ex, 'CN'),
                            TIN: EIMZOClient._getX500Val(x500name_ex, 'INN') || EIMZOClient._getX500Val(x500name_ex, 'UID'),
                            UID: EIMZOClient._getX500Val(x500name_ex, 'UID'),
                            PINFL: EIMZOClient._getX500Val(x500name_ex, 'PINFL'),
                            O: EIMZOClient._getX500Val(x500name_ex, 'O'),
                            T: EIMZOClient._getX500Val(x500name_ex, 'T'),
                            type: 'pfx'
                        }

                        if (!vo.TIN && !vo.PINFL) continue

                        const itmkey = itemIdGen(vo, parseInt(rec))
                        if (!itmkey0) {
                            itmkey0 = itmkey
                        }
                        const itm = itemUiGen(itmkey, vo)
                        items.push(itm)
                    }
                } else {
                    errors.push({ r: data.reason })
                }
                callback(itmkey0)
            },
            (e) => {
                errors.push({ e })
                callback(itmkey0)
            }
        )
    },
}

// Promise-based wrapper for modern async/await usage
export const EImzoService = {
    async checkConnection(): Promise<{ major: number; minor: number }> {
        return new Promise((resolve, reject) => {
            EIMZOClient.checkVersion(
                (major, minor) => resolve({ major, minor }),
                (e, r) => reject(new Error(r || String(e) || 'Connection failed'))
            )
        })
    },

    async installApiKeys(): Promise<void> {
        return new Promise((resolve, reject) => {
            EIMZOClient.installApiKeys(
                () => resolve(),
                (e, r) => reject(new Error(r || String(e) || 'API key installation failed'))
            )
        })
    },

    async listCertificates(): Promise<EImzoKeyItem[]> {
        return new Promise((resolve, reject) => {
            EIMZOClient.listAllUserKeys(
                (_item, index) => `key-${index}`,
                (_itemId, item) => item,
                (items) => resolve(items),
                (e, r) => reject(new Error(r || String(e) || 'Failed to list certificates'))
            )
        })
    },

    async loadKey(item: EImzoKeyItem): Promise<string> {
        return new Promise((resolve, reject) => {
            EIMZOClient.loadKey(
                item,
                (id) => resolve(id),
                (e, r) => reject(new Error(r || String(e) || 'Failed to load key')),
                false
            )
        })
    },

    async createPkcs7(keyId: string, data: string, detached = false): Promise<string> {
        return new Promise((resolve, reject) => {
            EIMZOClient.createPkcs7(
                keyId,
                data,
                null,
                (pkcs7) => resolve(pkcs7),
                (e, r) => reject(new Error(r || String(e) || 'Failed to create signature')),
                detached,
                false
            )
        })
    },

    async signChallenge(item: EImzoKeyItem, challenge: string): Promise<string> {
        const keyId = await this.loadKey(item)
        return this.createPkcs7(keyId, challenge, false)
    },
}
