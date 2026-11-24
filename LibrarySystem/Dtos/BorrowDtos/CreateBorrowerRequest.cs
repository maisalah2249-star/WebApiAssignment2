using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Dtos.BorrowerDtos;

public class CreateBorrowerRequest
{
    [Required, StringLength(100)]
    public string Name { get; set; } = null!;

    public DateTime MembershipDate { get; set; }
}