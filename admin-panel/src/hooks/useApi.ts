import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import type { PagedList, PaginationParams } from '../types'

interface CrudService<T> {
  getAll: (params?: PaginationParams) => Promise<PagedList<T>>
  getById: (id: string) => Promise<T>
  create: (data: Partial<T>) => Promise<string>
  update: (data: Partial<T>) => Promise<string>
  remove: (id: string) => Promise<void>
}

export function useCrudQuery<T>(
  key: string,
  service: CrudService<T>,
  params?: PaginationParams
) {
  const queryClient = useQueryClient()

  const listQuery = useQuery({
    queryKey: [key, params],
    queryFn: () => service.getAll(params),
  })

  const createMutation = useMutation({
    mutationFn: (data: Partial<T>) => service.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [key] })
    },
  })

  const updateMutation = useMutation({
    mutationFn: (data: Partial<T>) => service.update(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [key] })
    },
  })

  const deleteMutation = useMutation({
    mutationFn: (id: string) => service.remove(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [key] })
    },
  })

  return {
    data: listQuery.data,
    isLoading: listQuery.isLoading,
    error: listQuery.error,
    refetch: listQuery.refetch,
    create: createMutation.mutateAsync,
    update: updateMutation.mutateAsync,
    remove: deleteMutation.mutateAsync,
    isCreating: createMutation.isPending,
    isUpdating: updateMutation.isPending,
    isDeleting: deleteMutation.isPending,
  }
}

export function useItemQuery<T>(
  key: string,
  id: string | undefined,
  getById: (id: string) => Promise<T>
) {
  return useQuery({
    queryKey: [key, id],
    queryFn: () => getById(id!),
    enabled: !!id,
  })
}
