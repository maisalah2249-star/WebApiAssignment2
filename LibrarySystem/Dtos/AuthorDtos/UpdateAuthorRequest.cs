using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Dtos.AuthorDtos;

public class UpdateAuthorRequest
{
    [Range(1, int.MaxValue)]
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = null!;

    public DateTime BirthDate { get; set; }
}