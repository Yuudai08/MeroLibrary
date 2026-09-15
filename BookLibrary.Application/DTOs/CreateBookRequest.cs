using BookLibrary.Domain;

namespace BookLibrary.Application.DTOs;

// Command model for CreateBook. IDs only, no navigation objects.
// Full validation (required title, ISBN uniqueness, FK existence) lands in Phase 14;
// the database already enforces UQ/FK/CHECK as the final guard.
public sealed record CreateBookRequest(
    string Title,
    string ISBN,
    int AuthorId,
    int CategoryId,
    int PublishedYear,
    BookStatus Status,
    int? BookNumber = null);
