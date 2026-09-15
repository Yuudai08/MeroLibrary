namespace BookLibrary.Domain;

// Domain entity representing a registered user in MeroLibrary.
// Independent of EF Core / Dapper / SQL Server.
public sealed class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
