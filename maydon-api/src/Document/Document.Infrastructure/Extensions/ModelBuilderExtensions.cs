using System.Linq.Expressions;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Document.Infrastructure.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplySoftDeleteQueryFilter(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDeleteEntity).IsAssignableFrom(entityType.ClrType))
                continue;

            var parameter = Expression.Parameter(entityType.ClrType, "e");

            var isDeleted = Expression.Call(
                typeof(EF),
                nameof(EF.Property),
                new[] { typeof(bool) },
                parameter,
                Expression.Constant(nameof(ISoftDeleteEntity.IsDeleted))
            );

            var filter = Expression.Lambda(
                Expression.Equal(isDeleted, Expression.Constant(false)),
                parameter
            );

            entityType.SetQueryFilter(filter);
        }
    }
}
