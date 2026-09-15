using BookLibrary.Domain;

namespace BookLibrary.Application.DTOs;

// Command model for UpdateBook. Id travels separately (route), not in body.
public sealed record UpdateBookRequest(
    string Title,
    string ISBN,
    int AuthorId,
    int CategoryId,
    int PublishedYear,
    BookStatus Status,
    int? BookNumber = null);
