using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Dtos.BorrowerDtos;

public class UpdateBorrowerRequest
{
    [Range(1, int.MaxValue)]
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = null!;

    public DateTime MembershipDate { get; set; }
}