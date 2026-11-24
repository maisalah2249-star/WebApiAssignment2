namespace LibrarySystem.Entities;

public class Book : BaseEntity
{
    public string Title { get; set; } = null!;
    public string ISBN { get; set; } = null!;

    // Many-to-One: Book -> Author
    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;

    // Many-to-Many with Borrower through Loan
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}