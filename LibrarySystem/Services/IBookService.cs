using LibrarySystem.Dtos.BookDtos;
using LibrarySystem.Results;

namespace LibrarySystem.Services;

public interface IBookService
{
    Task<GenericResult<PaginatedList<GetBookResponse>>> GetBooksAsync(BookQueryParams queryParams);
    Task<GenericResult<GetBookResponse>> GetByIdAsync(int id);
    Task<GenericResult<GetBookResponse>> CreateAsync(CreateBookRequest request);
    Task<Result> UpdateAsync(UpdateBookRequest request);
    Task<Result> DeleteAsync(int id);
}