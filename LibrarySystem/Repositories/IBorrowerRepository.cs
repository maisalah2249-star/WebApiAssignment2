using LibrarySystem.Entities;
using LibrarySystem.Results;

namespace LibrarySystem.Repositories;

public interface IBorrowerRepository : IGenericRepository<Borrower>
{
    Task<PaginatedList<Borrower>> GetBorrowersAsync(int pageNumber, int pageSize, string? searchTerm);
}