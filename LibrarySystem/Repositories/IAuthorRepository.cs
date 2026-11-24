using LibrarySystem.Entities;
using LibrarySystem.Results;

namespace LibrarySystem.Repositories;

public interface IAuthorRepository : IGenericRepository<Author>
{
    Task<PaginatedList<Author>> GetAuthorsAsync(int pageNumber, int pageSize, string? searchTerm);
}