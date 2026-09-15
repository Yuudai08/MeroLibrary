namespace BookLibrary.Domain;

// One Category has many Books (via Books navigation).
// Name uniqueness is enforced by UQ_Categories_Name in SQL Server.
public sealed class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Book> Books { get; } = new List<Book>();
}
