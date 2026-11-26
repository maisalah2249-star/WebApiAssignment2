using LibrarySystem.Dtos.AuthorDtos;
using LibrarySystem.Filters;
using LibrarySystem.Results;
using LibrarySystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace LibrarySystem.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[ServiceFilter(typeof(ExceptionHandlingFilter))]
[ServiceFilter(typeof(ActionExecutionLoggingFilter))]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAuthors(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null)
    {
        var result = await _authorService.GetAuthorsAsync(pageNumber, pageSize, searchTerm);
        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _authorService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAuthorRequest request)
    {
        var result = await _authorService.CreateAsync(request);

        if (result.IsFailure)
            return HandleFailure(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateAuthorRequest request)
    {
        var result = await _authorService.UpdateAsync(request);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _authorService.DeleteAsync(id);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    private IActionResult HandleFailure(Result result)
        => result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase)
            ? NotFound(result.Error)
            : BadRequest(result.Error);
}