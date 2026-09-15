using BookLibrary.Application.DTOs;
using BookLibrary.Application.Interfaces;
using BookLibrary.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Infrastructure.Repositories;

// EF Core implementation of user write side executing sp_User_Create
public class UserWriter : IUserWriter
{
    private readonly BookLibraryDbContext _context;

    public UserWriter(BookLibraryDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAsync(RegisterRequest request, string passwordHash, CancellationToken cancellationToken = default)
    {
        var usernameParam = new SqlParameter("@Username", request.Username.Trim());
        var emailParam = new SqlParameter("@Email", request.Email.Trim().ToLowerInvariant());
        var passwordHashParam = new SqlParameter("@PasswordHash", passwordHash);

        var ids = await _context.Database
            .SqlQueryRaw<int>(
                "EXEC dbo.sp_User_Create @Username, @Email, @PasswordHash",
                usernameParam, emailParam, passwordHashParam)
            .ToListAsync(cancellationToken);

        return ids.FirstOrDefault();
    }
}
