using AutoMapper;
using LibrarySystem.Dtos.BookDtos;
using LibrarySystem.Entities;
using LibrarySystem.Repositories;
using LibrarySystem.Results;

namespace LibrarySystem.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IGenericRepository<Author> _authorRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<BookService> _logger;

    public BookService(
        IBookRepository bookRepository,
        IGenericRepository<Author> authorRepository,
        IMapper mapper,
        ILogger<BookService> logger)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GenericResult<PaginatedList<GetBookResponse>>> GetBooksAsync(BookQueryParams queryParams)
    {
        var page = await _bookRepository.GetBooksAsync(queryParams);

        var items = _mapper.Map<List<GetBookResponse>>(page.Data ?? Enumerable.Empty<Book>());
        var response = PaginatedList<GetBookResponse>.Create(items, page.PageNumber, page.PageSize, page.TotalRecords);

        _logger.LogInformation("Returned {Count} of {Total} books (page {Page})",
            items.Count, page.TotalRecords, page.PageNumber);

        return GenericResult<PaginatedList<GetBookResponse>>.Success(response);
    }

    public async Task<GenericResult<GetBookResponse>> GetByIdAsync(int id)
    {
        var book = await _bookRepository.GetBookWithAuthorAsync(id);

        if (book is null)
        {
            _logger.LogWarning("Book with ID = {Id} is not found.", id);
            return GenericResult<GetBookResponse>.Failure($"Book with ID = {id} is not found.");
        }

        return GenericResult<GetBookResponse>.Success(_mapper.Map<GetBookResponse>(book));
    }

    public async Task<GenericResult<GetBookResponse>> CreateAsync(CreateBookRequest request)
    {
        if (await _authorRepository.GetByIdAsync(request.AuthorId) is null)
        {
            _logger.LogWarning("Create book failed: author {AuthorId} is not found.", request.AuthorId);
            return GenericResult<GetBookResponse>.Failure($"Author with ID = {request.AuthorId} is not found.");
        }

        if (await _bookRepository.IsbnExistsAsync(request.ISBN))
        {
            _logger.LogWarning("Create book failed: ISBN {ISBN} already exists.", request.ISBN);
            return GenericResult<GetBookResponse>.Failure("A book with the same ISBN already exists.");
        }

        var book = _mapper.Map<Book>(request);
        await _bookRepository.CreateAsync(book);

        _logger.LogInformation("Book {Id} created.", book.Id);

        // Reload with the Author so AuthorName is filled
        var created = await _bookRepository.GetBookWithAuthorAsync(book.Id);
        return GenericResult<GetBookResponse>.Success(_mapper.Map<GetBookResponse>(created));
    }

    public async Task<Result> UpdateAsync(UpdateBookRequest request)
    {
        if (await _authorRepository.GetByIdAsync(request.AuthorId) is null)
            return Result.Failure($"Author with ID = {request.AuthorId} is not found.");

        if (await _bookRepository.IsbnExistsAsync(request.ISBN, request.Id))
            return Result.Failure("A book with the same ISBN already exists.");

        var book = _mapper.Map<Book>(request);
        var updated = await _bookRepository.UpdateAsync(book);

        if (!updated)
        {
            _logger.LogWarning("Update failed: book {Id} is not found.", request.Id);
            return Result.Failure($"Book with ID = {request.Id} is not found.");
        }

        _logger.LogInformation("Book {Id} updated.", request.Id);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var deleted = await _bookRepository.DeleteAsync(id);

        if (!deleted)
        {
            _logger.LogWarning("Delete failed: book {Id} is not found.", id);
            return Result.Failure($"Book with ID = {id} is not found.");
        }

        _logger.LogInformation("Book {Id} deleted.", id);
        return Result.Success();
    }
}