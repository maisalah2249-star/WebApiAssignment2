namespace LibrarySystem.Dtos.AuthorDtos;

public class GetAuthorResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime BirthDate { get; set; }
}