namespace BookLibrary.Domain;

// Each Book belongs to one Author (AuthorId) and one Category (CategoryId).
// Stores IDs, not names: avoids duplication, enforced by FK_Books_Authors/Categories.
// ISBN uniqueness + Status range enforced in DB (UQ_Books_ISBN, CK_Books_Status);
// this class carries the same shape so violations surface early in C# too.
public sealed class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string ISBN { get; set; } = string.Empty;

    public int AuthorId { get; set; }

    // null! = required after load; avoids nullable warnings without a constructor.
    // EF Core populates this via Fluent API config in Infrastructure (Phase 9).
    public Author Author { get; set; } = null!;

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public int? BookNumber { get; set; }

    public int PublishedYear { get; set; }

    public BookStatus Status { get; set; } = BookStatus.WantToRead;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
