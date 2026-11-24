namespace LibrarySystem.Dtos.BookDtos;

public class GetBookResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string ISBN { get; set; } = null!;
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
}