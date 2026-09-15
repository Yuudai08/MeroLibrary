using BookLibrary.Domain;

namespace BookLibrary.Application.DTOs;

// Response DTO: flattened read model matching sp_Book_* result shape.
// Entity != Request DTO != Response DTO:
// - Domain Book carries Author/Category navigation OBJECTS.
// - This DTO carries AuthorName/CategoryName STRINGS (no lazy loading, API-safe).
public sealed record BookDto(
    int Id,
    string Title,
    string ISBN,
    int AuthorId,
    string AuthorName,
    int CategoryId,
    string CategoryName,
    int PublishedYear,
    BookStatus Status,
    int UserId,
    int? BookNumber,
    DateTime CreatedAt,
    DateTime? UpdatedAt)
{
    public BookDto(
        int id,
        string title,
        string isbn,
        int authorId,
        string authorName,
        int categoryId,
        string categoryName,
        int publishedYear,
        byte status,
        int userId,
        int? bookNumber,
        DateTime createdAt,
        DateTime? updatedAt)
        : this(id, title, isbn, authorId, authorName, categoryId, categoryName, publishedYear, (BookStatus)status, userId, bookNumber, createdAt, updatedAt)
    {
    }

    public BookDto(
        int id,
        string title,
        string isbn,
        int authorId,
        string authorName,
        int categoryId,
        string categoryName,
        int publishedYear,
        BookStatus status,
        DateTime createdAt,
        DateTime? updatedAt)
        : this(id, title, isbn, authorId, authorName, categoryId, categoryName, publishedYear, status, 1, null, createdAt, updatedAt)
    {
    }

    public BookDto(
        int id,
        string title,
        string isbn,
        int authorId,
        string authorName,
        int categoryId,
        string categoryName,
        int publishedYear,
        byte status,
        DateTime createdAt,
        DateTime? updatedAt)
        : this(id, title, isbn, authorId, authorName, categoryId, categoryName, publishedYear, (BookStatus)status, 1, null, createdAt, updatedAt)
    {
    }

    public BookDto(
        int id,
        string title,
        string isbn,
        int authorId,
        string authorName,
        int categoryId,
        string categoryName,
        int publishedYear,
        int status,
        DateTime createdAt,
        DateTime? updatedAt)
        : this(id, title, isbn, authorId, authorName, categoryId, categoryName, publishedYear, (BookStatus)status, 1, null, createdAt, updatedAt)
    {
    }
}
