namespace BookLibrary.Domain;

// One Author has many Books (via Books navigation).
// Pure POCO: no EF, Dapper, or HTTP references.
public sealed class Author
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Biography { get; set; }

    // Read-only collection: callers can add/remove items but not replace the list.
    public ICollection<Book> Books { get; } = new List<Book>();
}
