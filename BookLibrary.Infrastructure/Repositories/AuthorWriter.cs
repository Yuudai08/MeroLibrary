using BookLibrary.Application.DTOs;
using BookLibrary.Application.Interfaces;
using BookLibrary.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Infrastructure.Repositories;

public class AuthorWriter : IAuthorWriter
{
    private readonly BookLibraryDbContext _context;

    public AuthorWriter(BookLibraryDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default)
    {
        var nameParam = new SqlParameter("@Name", request.Name);
        var bioParam = new SqlParameter("@Biography", (object?)request.Biography ?? DBNull.Value);

        var ids = await _context.Database
            .SqlQueryRaw<int>("EXEC dbo.sp_Author_Create @Name, @Biography", nameParam, bioParam)
            .ToListAsync(cancellationToken);

        return ids.FirstOrDefault();
    }

    public async Task<bool> UpdateAsync(int id, UpdateAuthorRequest request, CancellationToken cancellationToken = default)
    {
        var idParam = new SqlParameter("@Id", id);
        var nameParam = new SqlParameter("@Name", request.Name);
        var bioParam = new SqlParameter("@Biography", (object?)request.Biography ?? DBNull.Value);

        var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
            "EXEC dbo.sp_Author_Update @Id, @Name, @Biography",
            new object[] { idParam, nameParam, bioParam },
            cancellationToken);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var idParam = new SqlParameter("@Id", id);
        var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
            "EXEC dbo.sp_Author_Delete @Id",
            new object[] { idParam },
            cancellationToken);

        return rowsAffected > 0;
    }

    public async Task DeleteWithBooksAsync(int authorId, CancellationToken cancellationToken = default)
    {
        var idParam = new SqlParameter("@AuthorId", authorId);
        await _context.Database.ExecuteSqlRawAsync(
            "EXEC dbo.sp_Author_DeleteWithBooks @AuthorId",
            new object[] { idParam },
            cancellationToken);
    }
}
