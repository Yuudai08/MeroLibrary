namespace BookLibrary.Web.Models;

public sealed record CreateBookRequest(
    string Title,
    string ISBN,
    int AuthorId,
    int CategoryId,
    int PublishedYear,
    BookStatus Status,
    int? BookNumber = null);
