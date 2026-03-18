import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { moderationService } from '../services/api'
import type {
    ModerationFilterParams,
    ModerationListingDetail,
    ModerationRealEstateDetail,
    ModerationUnitDetail,
} from '../types'

type EntityType = 'listings' | 'realEstates' | 'units'

// ── List Queries ──

export function useModerationListings(params: ModerationFilterParams, enabled = true) {
    return useQuery({
        queryKey: ['moderation-listings', params],
        queryFn: () => moderationService.getListings(params),
        enabled,
    })
}

export function useModerationRealEstates(params: ModerationFilterParams, enabled = true) {
    return useQuery({
        queryKey: ['moderation-realestates', params],
        queryFn: () => moderationService.getRealEstates(params),
        enabled,
    })
}

export function useModerationUnits(params: ModerationFilterParams, enabled = true) {
    return useQuery({
        queryKey: ['moderation-units', params],
        queryFn: () => moderationService.getUnits(params),
        enabled,
    })
}

// ── Detail Queries ──

export function useModerationListingDetail(id: string) {
    return useQuery<ModerationListingDetail>({
        queryKey: ['moderation-listing', id],
        queryFn: () => moderationService.getListingById(id),
        enabled: !!id,
    })
}

export function useModerationRealEstateDetail(id: string) {
    return useQuery<ModerationRealEstateDetail>({
        queryKey: ['moderation-realestate', id],
        queryFn: () => moderationService.getRealEstateById(id),
        enabled: !!id,
    })
}

export function useModerationUnitDetail(id: string) {
    return useQuery<ModerationUnitDetail>({
        queryKey: ['moderation-unit', id],
        queryFn: () => moderationService.getUnitById(id),
        enabled: !!id,
    })
}

// ── Mutations ──

export function useModerationActions(entityType: EntityType, options?: { onSuccess?: () => void }) {
    const queryClient = useQueryClient()

    const invalidateKey =
        entityType === 'listings' ? 'moderation-listings' :
            entityType === 'realEstates' ? 'moderation-realestates' :
                'moderation-units'

    const detailKey =
        entityType === 'listings' ? 'moderation-listing' :
            entityType === 'realEstates' ? 'moderation-realestate' :
                'moderation-unit'

    const onMutationSuccess = () => {
        queryClient.invalidateQueries({ queryKey: [invalidateKey] })
        queryClient.invalidateQueries({ queryKey: [detailKey] })
        options?.onSuccess?.()
    }

    const acceptMutation = useMutation({
        mutationFn: (id: string) => {
            if (entityType === 'listings') return moderationService.acceptListing(id)
            if (entityType === 'realEstates') return moderationService.acceptRealEstate(id)
            return moderationService.acceptUnit(id)
        },
        onSuccess: onMutationSuccess,
    })

    const rejectMutation = useMutation({
        mutationFn: ({ id, reason }: { id: string; reason?: string }) => {
            if (entityType === 'listings') return moderationService.rejectListing(id, reason)
            if (entityType === 'realEstates') return moderationService.rejectRealEstate(id, reason)
            return moderationService.rejectUnit(id, reason)
        },
        onSuccess: onMutationSuccess,
    })

    const blockMutation = useMutation({
        mutationFn: ({ id, reason }: { id: string; reason?: string }) => {
            if (entityType === 'listings') return moderationService.blockListing(id, reason)
            if (entityType === 'realEstates') return moderationService.blockRealEstate(id, reason)
            return moderationService.blockUnit(id, reason)
        },
        onSuccess: onMutationSuccess,
    })

    return {
        acceptMutation,
        rejectMutation,
        blockMutation,
        isActing: acceptMutation.isPending || rejectMutation.isPending || blockMutation.isPending,
    }
}
