using LibrarySystem.Dtos.BookDtos;
using LibrarySystem.Filters;
using LibrarySystem.Results;
using LibrarySystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace LibrarySystem.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[ServiceFilter(typeof(ExceptionHandlingFilter))]
[ServiceFilter(typeof(ActionExecutionLoggingFilter))]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    /// <summary>
    /// Gets books with searching, filtration, sorting and pagination. Public endpoint.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ServiceFilter(typeof(ActionTimingFilter))]
    public async Task<ActionResult<PaginatedList<GetBookResponse>>> GetBooks([FromQuery] BookQueryParams queryParams)
    {
        var result = await _bookService.GetBooksAsync(queryParams);
        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ServiceFilter(typeof(ActionTimingFilter))]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _bookService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    // Demo endpoint to prove ExceptionHandlingFilter works
    [HttpGet("test-error")]
    [AllowAnonymous]
    public IActionResult TestError()
        => throw new InvalidOperationException("Test exception to demonstrate the exception filter.");

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookRequest request)
    {
        var result = await _bookService.CreateAsync(request);

        if (result.IsFailure)
            return HandleFailure(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateBookRequest request)
    {
        var result = await _bookService.UpdateAsync(request);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _bookService.DeleteAsync(id);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    // Simple mapping: "not found" errors -> 404, everything else -> 400
    private IActionResult HandleFailure(Result result)
        => result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase)
            ? NotFound(result.Error)
            : BadRequest(result.Error);
}