using LibrarySystem.Data;
using LibrarySystem.Entities;
using LibrarySystem.Results;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Repositories;

public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
{
    public AuthorRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<PaginatedList<Author>> GetAuthorsAsync(int pageNumber, int pageSize, string? searchTerm)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 20);

        var query = _dbContext.Authors.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(a => a.Name.Contains(term));
        }

        var totalCount = await query.CountAsync();

        var authors = await query
            .OrderBy(a => a.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return PaginatedList<Author>.Create(authors, pageNumber, pageSize, totalCount);
    }
}