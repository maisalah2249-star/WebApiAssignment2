using LibrarySystem.Dtos.AuthorDtos;
using LibrarySystem.Results;

namespace LibrarySystem.Services;

public interface IAuthorService
{
    Task<GenericResult<PaginatedList<GetAuthorResponse>>> GetAuthorsAsync(int pageNumber, int pageSize, string? searchTerm);
    Task<GenericResult<GetAuthorResponse>> GetByIdAsync(int id);
    Task<GenericResult<GetAuthorResponse>> CreateAsync(CreateAuthorRequest request);
    Task<Result> UpdateAsync(UpdateAuthorRequest request);
    Task<Result> DeleteAsync(int id);
}