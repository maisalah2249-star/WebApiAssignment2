using LibrarySystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LibrarySystem.Data.Interceptors;

public class ModifiedAtInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ApplyModifiedAt(eventData);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        ApplyModifiedAt(eventData);
        return ValueTask.FromResult(result);
    }

    private static void ApplyModifiedAt(DbContextEventData eventData)
    {
        if (eventData.Context is null)
            return;

        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            if (entry.State != EntityState.Modified || entry.Entity is not BaseEntity entity)
                continue;

            entity.ModifiedAt = DateTime.UtcNow;

            // Update() with a DTO-mapped entity would overwrite the creation audit with null,
            // so keep the original values stored in the database.
            entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
            entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
        }
    }
}