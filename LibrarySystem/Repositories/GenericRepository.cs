using LibrarySystem.Data;
using LibrarySystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _dbContext;

    public GenericRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T> CreateAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);

        if (entity is null)
            return false;

        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
        => await _dbContext.Set<T>().AsNoTracking().ToListAsync();

    public async Task<T?> GetByIdAsync(int id)
     => await _dbContext.Set<T>().FirstOrDefaultAsync(e => e.Id == id);

    public async Task<bool> UpdateAsync(T entity)
    {
        // AnyAsync (no tracking) avoids the "another instance is already tracked" error
        var exists = await _dbContext.Set<T>().AnyAsync(e => e.Id == entity.Id);

        if (!exists)
            return false;

        _dbContext.Set<T>().Update(entity);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}