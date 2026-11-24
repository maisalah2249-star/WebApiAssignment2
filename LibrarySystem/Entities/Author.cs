namespace LibrarySystem.Entities;

public class Author : BaseEntity
{
    public string Name { get; set; } = null!;
    public DateTime BirthDate { get; set; }

    // One-to-Many: Author -> Books
    public ICollection<Book> Books { get; set; } = new List<Book>();
}