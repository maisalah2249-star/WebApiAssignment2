using LibrarySystem.Dtos.BookDtos;
using LibrarySystem.Entities;
using LibrarySystem.Results;

namespace LibrarySystem.Repositories;

public interface IBookRepository : IGenericRepository<Book>
{
    Task<PaginatedList<Book>> GetBooksAsync(BookQueryParams queryParams);
    Task<Book?> GetBookWithAuthorAsync(int id);
    Task<bool> IsbnExistsAsync(string isbn, int? excludeId = null);
}