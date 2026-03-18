// Meter Types
export interface MeterType {
  id: string
  name: string
  description?: string
  icon?: string
  unit?: string
}

// Meter - matches GetMetersResponse from backend
export interface Meter {
  id: string
  realEstateId?: string
  meterTypeId: string
  meterName: string
  serialNumber: string
  installationDate?: string
  verificationDate?: string
  nextVerificationDate?: string
  initialReading: number
}

// Meter Reading - matches GetMeterReadingsResponse from backend
export interface MeterReading {
  meterId: string
  meterName: string
  serialNumber: string
  readingDate: string
  value: number
  previousValue: number
  consumption: number
  isManual: boolean
}

// Meter Tariff - matches GetMeterTariffsResponse from backend
export interface MeterTariff {
  id: string
  meterTypeId: string
  meterType: string
  price: number
  type: number
  isActual: boolean
  minLimit?: number
  maxLimit?: number
  fixedPrice?: number
  season: Season
  socialNormLimit?: number
}

// Enums
export enum Season {
  All = 0,
  Summer = 1,
  Winter = 2
}

export enum BillStatus {
  Pending = 0,
  PartiallyPaid = 1,
  Paid = 2,
  Overdue = 3,
  Cancelled = 4
}

// Commands
export interface CreateMeterCommand {
  realEstateId: string
  meterTypeId: string
  initialReading?: number
  serialNumber?: string
  installationDate?: string
  verificationDate?: string
  nextVerificationDate?: string
}

export interface UpdateMeterCommand {
  id: string
  meterTypeId: string
  serialNumber?: string
  installationDate?: string
  verificationDate?: string
  nextVerificationDate?: string
  initialReading?: number
}

export interface UpsertMeterReadingCommand {
  meterId: string
  value: number
  previousValue?: number
  readingDate?: string
  isManual: boolean
  note?: string
}

// Paged response
export interface PagedMeterReadings {
  items: MeterReading[]
  page: number
  pageSize: number
  totalCount: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

export interface PagedMeterTariffs {
  items: MeterTariff[]
  page: number
  pageSize: number
  totalCount: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}
