using AutoMapper;
using LibrarySystem.Dtos.AuthorDtos;
using LibrarySystem.Entities;
using LibrarySystem.Repositories;
using LibrarySystem.Results;

namespace LibrarySystem.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthorService> _logger;

    public AuthorService(IAuthorRepository repository, IMapper mapper, ILogger<AuthorService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GenericResult<PaginatedList<GetAuthorResponse>>> GetAuthorsAsync(int pageNumber, int pageSize, string? searchTerm)
    {
        var page = await _repository.GetAuthorsAsync(pageNumber, pageSize, searchTerm);

        var items = _mapper.Map<List<GetAuthorResponse>>(page.Data ?? Enumerable.Empty<Author>());
        var response = PaginatedList<GetAuthorResponse>.Create(items, page.PageNumber, page.PageSize, page.TotalRecords);

        return GenericResult<PaginatedList<GetAuthorResponse>>.Success(response);
    }

    public async Task<GenericResult<GetAuthorResponse>> GetByIdAsync(int id)
    {
        var author = await _repository.GetByIdAsync(id);

        if (author is null)
        {
            _logger.LogWarning("Author with ID = {Id} is not found.", id);
            return GenericResult<GetAuthorResponse>.Failure($"Author with ID = {id} is not found.");
        }

        return GenericResult<GetAuthorResponse>.Success(_mapper.Map<GetAuthorResponse>(author));
    }

    public async Task<GenericResult<GetAuthorResponse>> CreateAsync(CreateAuthorRequest request)
    {
        var author = _mapper.Map<Author>(request);
        await _repository.CreateAsync(author);

        _logger.LogInformation("Author {Id} created.", author.Id);
        return GenericResult<GetAuthorResponse>.Success(_mapper.Map<GetAuthorResponse>(author));
    }

    public async Task<Result> UpdateAsync(UpdateAuthorRequest request)
    {
        var author = _mapper.Map<Author>(request);
        var updated = await _repository.UpdateAsync(author);

        if (!updated)
        {
            _logger.LogWarning("Update failed: author {Id} is not found.", request.Id);
            return Result.Failure($"Author with ID = {request.Id} is not found.");
        }

        _logger.LogInformation("Author {Id} updated.", request.Id);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var deleted = await _repository.DeleteAsync(id);

        if (!deleted)
        {
            _logger.LogWarning("Delete failed: author {Id} is not found.", id);
            return Result.Failure($"Author with ID = {id} is not found.");
        }

        _logger.LogInformation("Author {Id} deleted.", id);
        return Result.Success();
    }
}