using LibrarySystem.Data;
using LibrarySystem.Entities;
using LibrarySystem.Results;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Repositories;

public class BorrowerRepository : GenericRepository<Borrower>, IBorrowerRepository
{
    public BorrowerRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<PaginatedList<Borrower>> GetBorrowersAsync(int pageNumber, int pageSize, string? searchTerm)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 20);

        var query = _dbContext.Borrowers.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(b => b.Name.Contains(term));
        }

        var totalCount = await query.CountAsync();

        var borrowers = await query
            .OrderBy(b => b.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return PaginatedList<Borrower>.Create(borrowers, pageNumber, pageSize, totalCount);
    }
}