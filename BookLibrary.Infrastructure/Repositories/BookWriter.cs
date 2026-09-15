using BookLibrary.Application.DTOs;
using BookLibrary.Application.Interfaces;
using BookLibrary.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Infrastructure.Repositories;

// EF Core implementation of the write side (command side).
// Explicitly executes Stored Procedures (sp_Book_Create, sp_Book_Update, sp_Book_Delete)
// instead of bypassing them with generic DbSet.Add/Update/Remove, fulfilling the project requirements.
public class BookWriter : IBookWriter
{
    private readonly BookLibraryDbContext _context;

    public BookWriter(BookLibraryDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAsync(int userId, CreateBookRequest request, CancellationToken cancellationToken = default)
    {
        var titleParam = new SqlParameter("@Title", request.Title);
        var isbnParam = new SqlParameter("@ISBN", request.ISBN);
        var authorIdParam = new SqlParameter("@AuthorId", request.AuthorId);
        var categoryIdParam = new SqlParameter("@CategoryId", request.CategoryId);
        var publishedYearParam = new SqlParameter("@PublishedYear", request.PublishedYear);
        var statusParam = new SqlParameter("@Status", (byte)request.Status);
        var userIdParam = new SqlParameter("@UserId", userId);
        var bookNumberParam = new SqlParameter("@BookNumber", (object?)request.BookNumber ?? DBNull.Value);

        var ids = await _context.Database
            .SqlQueryRaw<int>("EXEC dbo.sp_Book_Create @Title, @ISBN, @AuthorId, @CategoryId, @PublishedYear, @Status, @UserId, @BookNumber",
                titleParam, isbnParam, authorIdParam, categoryIdParam, publishedYearParam, statusParam, userIdParam, bookNumberParam)
            .ToListAsync(cancellationToken);

        return ids.FirstOrDefault();
    }

    public async Task<bool> UpdateAsync(int id, int userId, UpdateBookRequest request, CancellationToken cancellationToken = default)
    {
        var idParam = new SqlParameter("@Id", id);
        var titleParam = new SqlParameter("@Title", request.Title);
        var isbnParam = new SqlParameter("@ISBN", request.ISBN);
        var authorIdParam = new SqlParameter("@AuthorId", request.AuthorId);
        var categoryIdParam = new SqlParameter("@CategoryId", request.CategoryId);
        var publishedYearParam = new SqlParameter("@PublishedYear", request.PublishedYear);
        var statusParam = new SqlParameter("@Status", (byte)request.Status);
        var userIdParam = new SqlParameter("@UserId", userId);
        var bookNumberParam = new SqlParameter("@BookNumber", (object?)request.BookNumber ?? DBNull.Value);

        var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
            "EXEC dbo.sp_Book_Update @Id, @Title, @ISBN, @AuthorId, @CategoryId, @PublishedYear, @Status, @UserId, @BookNumber",
            new object[] { idParam, titleParam, isbnParam, authorIdParam, categoryIdParam, publishedYearParam, statusParam, userIdParam, bookNumberParam },
            cancellationToken);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var idParam = new SqlParameter("@Id", id);
        var userIdParam = new SqlParameter("@UserId", userId);

        var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
            "EXEC dbo.sp_Book_Delete @Id, @UserId",
            new object[] { idParam, userIdParam },
            cancellationToken);

        return rowsAffected > 0;
    }
}
