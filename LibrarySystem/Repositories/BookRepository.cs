using LibrarySystem.Data;
using LibrarySystem.Dtos.BookDtos;
using LibrarySystem.Entities;
using LibrarySystem.Results;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Repositories;

public class BookRepository : GenericRepository<Book>, IBookRepository
{
    public BookRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Book?> GetBookWithAuthorAsync(int id)
        => await _dbContext.Books
            .Include(b => b.Author)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task<bool> IsbnExistsAsync(string isbn, int? excludeId = null)
    => await _dbContext.Books.AnyAsync(b => b.ISBN == isbn && (excludeId == null || b.Id != excludeId));

    public async Task<PaginatedList<Book>> GetBooksAsync(BookQueryParams queryParams)
    {
        var query = _dbContext.Books
            .Include(b => b.Author)
            .AsNoTracking()
            .AsQueryable();

        // Searching
        if (!string.IsNullOrWhiteSpace(queryParams.Search))
        {
            var search = queryParams.Search.Trim();
            query = query.Where(b => b.Title.Contains(search) || b.ISBN.Contains(search));
        }

        // Filtration
        if (queryParams.AuthorId.HasValue)
            query = query.Where(b => b.AuthorId == queryParams.AuthorId.Value);

        if (queryParams.IsAvailable.HasValue)
        {
            query = queryParams.IsAvailable.Value
                ? query.Where(b => !b.Loans.Any(l => l.ReturnDate == null))
                : query.Where(b => b.Loans.Any(l => l.ReturnDate == null));
        }

        // Sorting
        query = queryParams.Sort?.ToLower() switch
        {
            "title_desc" => query.OrderByDescending(b => b.Title),
            "isbn" => query.OrderBy(b => b.ISBN),
            _ => query.OrderBy(b => b.Title)
        };

        // Pagination
        var totalRecords = await query.CountAsync();

        var books = await query
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();

        return PaginatedList<Book>.Create(books, queryParams.PageNumber, queryParams.PageSize, totalRecords);
    }
}