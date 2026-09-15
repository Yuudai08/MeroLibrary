namespace BookLibrary.Web.Models;

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
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int UserId = 0,
    int? BookNumber = null);
