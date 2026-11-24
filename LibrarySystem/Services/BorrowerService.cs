using AutoMapper;
using LibrarySystem.Dtos.BorrowerDtos;
using LibrarySystem.Entities;
using LibrarySystem.Repositories;
using LibrarySystem.Results;

namespace LibrarySystem.Services;

public class BorrowerService : IBorrowerService
{
    private readonly IBorrowerRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<BorrowerService> _logger;

    public BorrowerService(IBorrowerRepository repository, IMapper mapper, ILogger<BorrowerService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GenericResult<PaginatedList<GetBorrowerResponse>>> GetBorrowersAsync(int pageNumber, int pageSize, string? searchTerm)
    {
        var page = await _repository.GetBorrowersAsync(pageNumber, pageSize, searchTerm);

        var items = _mapper.Map<List<GetBorrowerResponse>>(page.Data ?? Enumerable.Empty<Borrower>());
        var response = PaginatedList<GetBorrowerResponse>.Create(items, page.PageNumber, page.PageSize, page.TotalRecords);

        return GenericResult<PaginatedList<GetBorrowerResponse>>.Success(response);
    }

    public async Task<GenericResult<GetBorrowerResponse>> GetByIdAsync(int id)
    {
        var borrower = await _repository.GetByIdAsync(id);

        if (borrower is null)
        {
            _logger.LogWarning("Borrower with ID = {Id} is not found.", id);
            return GenericResult<GetBorrowerResponse>.Failure($"Borrower with ID = {id} is not found.");
        }

        return GenericResult<GetBorrowerResponse>.Success(_mapper.Map<GetBorrowerResponse>(borrower));
    }

    public async Task<GenericResult<GetBorrowerResponse>> CreateAsync(CreateBorrowerRequest request)
    {
        var borrower = _mapper.Map<Borrower>(request);
        await _repository.CreateAsync(borrower);

        _logger.LogInformation("Borrower {Id} created.", borrower.Id);
        return GenericResult<GetBorrowerResponse>.Success(_mapper.Map<GetBorrowerResponse>(borrower));
    }

    public async Task<Result> UpdateAsync(UpdateBorrowerRequest request)
    {
        var borrower = _mapper.Map<Borrower>(request);
        var updated = await _repository.UpdateAsync(borrower);

        if (!updated)
        {
            _logger.LogWarning("Update failed: borrower {Id} is not found.", request.Id);
            return Result.Failure($"Borrower with ID = {request.Id} is not found.");
        }

        _logger.LogInformation("Borrower {Id} updated.", request.Id);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var deleted = await _repository.DeleteAsync(id);

        if (!deleted)
        {
            _logger.LogWarning("Delete failed: borrower {Id} is not found.", id);
            return Result.Failure($"Borrower with ID = {id} is not found.");
        }

        _logger.LogInformation("Borrower {Id} deleted.", id);
        return Result.Success();
    }
}