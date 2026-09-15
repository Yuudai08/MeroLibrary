using System.Data;
using BookLibrary.Application.DTOs;
using BookLibrary.Application.Interfaces;
using Dapper;

namespace BookLibrary.Infrastructure.Repositories;

public class CategoryReader : ICategoryReader
{
    private readonly IDbConnection _connection;

    public CategoryReader(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await _connection.QueryAsync<CategoryDto>("dbo.sp_Category_GetAll", commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _connection.QueryFirstOrDefaultAsync<CategoryDto>("dbo.sp_Category_GetById", new { Id = id }, commandType: CommandType.StoredProcedure);
    }
}
