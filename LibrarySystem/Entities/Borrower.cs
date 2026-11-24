namespace LibrarySystem.Entities;

public class Borrower : BaseEntity
{
    public string Name { get; set; } = null!;
    public DateTime MembershipDate { get; set; }

    // Many-to-Many with Book through Loan
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}