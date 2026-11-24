using LibrarySystem.Dtos.BorrowerDtos;
using LibrarySystem.Filters;
using LibrarySystem.Results;
using LibrarySystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ActionExecutionLoggingFilter))]
public class BorrowersController : ControllerBase
{
    private readonly IBorrowerService _borrowerService;

    public BorrowersController(IBorrowerService borrowerService)
    {
        _borrowerService = borrowerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _borrowerService.GetAllAsync();
        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _borrowerService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBorrowerRequest request)
    {
        var result = await _borrowerService.CreateAsync(request);

        if (result.IsFailure)
            return HandleFailure(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateBorrowerRequest request)
    {
        var result = await _borrowerService.UpdateAsync(request);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _borrowerService.DeleteAsync(id);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    private IActionResult HandleFailure(Result result)
        => result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase)
            ? NotFound(result.Error)
            : BadRequest(result.Error);
}