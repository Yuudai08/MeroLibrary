using System.Data;
using BookLibrary.Application.DTOs;
using BookLibrary.Application.Interfaces;
using Dapper;

namespace BookLibrary.Infrastructure.Repositories;

public class AuthorReader : IAuthorReader
{
    private readonly IDbConnection _connection;

    public AuthorReader(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IReadOnlyList<AuthorDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await _connection.QueryAsync<AuthorDto>("dbo.sp_Author_GetAll", commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<AuthorDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _connection.QueryFirstOrDefaultAsync<AuthorDto>("dbo.sp_Author_GetById", new { Id = id }, commandType: CommandType.StoredProcedure);
    }
}
