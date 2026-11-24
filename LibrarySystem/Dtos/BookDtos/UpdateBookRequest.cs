using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Dtos.BookDtos;

public class UpdateBookRequest
{
    [Range(1, int.MaxValue)]
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = null!;

    [Required, StringLength(20)]
    public string ISBN { get; set; } = null!;

    [Range(1, int.MaxValue)]
    public int AuthorId { get; set; }
}