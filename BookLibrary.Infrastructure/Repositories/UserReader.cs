using System.Data;
using BookLibrary.Application.Interfaces;
using BookLibrary.Domain;
using Dapper;

namespace BookLibrary.Infrastructure.Repositories;

// Dapper implementation of user read side executing sp_User_GetByUsername / sp_User_GetById
public class UserReader : IUserReader
{
    private readonly IDbConnection _connection;

    public UserReader(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _connection.QueryFirstOrDefaultAsync<User>(
            "dbo.sp_User_GetByUsername",
            new { Username = username.Trim() },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _connection.QueryFirstOrDefaultAsync<User>(
            "dbo.sp_User_GetById",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }
}
