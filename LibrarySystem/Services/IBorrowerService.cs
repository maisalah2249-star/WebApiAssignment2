using LibrarySystem.Dtos.BorrowerDtos;
using LibrarySystem.Results;

namespace LibrarySystem.Services;

public interface IBorrowerService
{
    Task<GenericResult<IEnumerable<GetBorrowerResponse>>> GetAllAsync();
    Task<GenericResult<GetBorrowerResponse>> GetByIdAsync(int id);
    Task<GenericResult<GetBorrowerResponse>> CreateAsync(CreateBorrowerRequest request);
    Task<Result> UpdateAsync(UpdateBorrowerRequest request);
    Task<Result> DeleteAsync(int id);
}