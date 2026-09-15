using BookLibrary.Application.DTOs;
using BookLibrary.Application.Interfaces;
using BookLibrary.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Infrastructure.Repositories;

public class CategoryWriter : ICategoryWriter
{
    private readonly BookLibraryDbContext _context;

    public CategoryWriter(BookLibraryDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var nameParam = new SqlParameter("@Name", request.Name);

        var ids = await _context.Database
            .SqlQueryRaw<int>("EXEC dbo.sp_Category_Create @Name", nameParam)
            .ToListAsync(cancellationToken);

        return ids.FirstOrDefault();
    }

    public async Task<bool> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var idParam = new SqlParameter("@Id", id);
        var nameParam = new SqlParameter("@Name", request.Name);

        var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
            "EXEC dbo.sp_Category_Update @Id, @Name",
            new object[] { idParam, nameParam },
            cancellationToken);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var idParam = new SqlParameter("@Id", id);
        var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
            "EXEC dbo.sp_Category_Delete @Id",
            new object[] { idParam },
            cancellationToken);

        return rowsAffected > 0;
    }
}
