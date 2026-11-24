using LibrarySystem.Dtos.AuthorDtos;
using LibrarySystem.Results;

namespace LibrarySystem.Services;

public interface IAuthorService
{
    Task<GenericResult<IEnumerable<GetAuthorResponse>>> GetAllAsync();
    Task<GenericResult<GetAuthorResponse>> GetByIdAsync(int id);
    Task<GenericResult<GetAuthorResponse>> CreateAsync(CreateAuthorRequest request);
    Task<Result> UpdateAsync(UpdateAuthorRequest request);
    Task<Result> DeleteAsync(int id);
}