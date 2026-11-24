using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Dtos.AuthorDtos;

public class CreateAuthorRequest
{
    [Required, StringLength(100)]
    public string Name { get; set; } = null!;

    public DateTime BirthDate { get; set; }
}