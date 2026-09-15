using System.Data;
using BookLibrary.Application.DTOs;
using BookLibrary.Application.Interfaces;
using BookLibrary.Domain;
using Dapper;

namespace BookLibrary.Infrastructure.Repositories;

// Dapper implementation of the read side (query side).
// Executes stored procedures (sp_Book_*) via IDbConnection and maps results to BookDto.
public class BookReader : IBookReader
{
    private readonly IDbConnection _connection;

    public BookReader(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IReadOnlyList<BookDto>> GetAllAsync(int userId, BookStatus? status = null, int? authorId = null, int? categoryId = null, CancellationToken cancellationToken = default)
    {
        var parameters = new
        {
            UserId = userId,
            Status = status.HasValue ? (byte?)status.Value : null,
            AuthorId = authorId,
            CategoryId = categoryId
        };

        var result = await _connection.QueryAsync<BookDto>(
            "dbo.sp_Book_GetAll",
            parameters,
            commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<PagedResult<BookDto>> GetPagedAsync(
        int userId,
        BookStatus? status = null,
        int? authorId = null,
        int? categoryId = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var parameters = new
        {
            UserId = userId,
            Status = status.HasValue ? (byte?)status.Value : null,
            AuthorId = authorId,
            CategoryId = categoryId,
            Page = page,
            PageSize = pageSize
        };

        using var multi = await _connection.QueryMultipleAsync(
            "dbo.sp_Book_GetPaged",
            parameters,
            commandType: CommandType.StoredProcedure);

        var totalCount = await multi.ReadFirstAsync<int>();
        var items = (await multi.ReadAsync<BookDto>()).ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        if (totalPages < 1) totalPages = 1;

        return new PagedResult<BookDto>(items, page, pageSize, totalCount, totalPages);
    }

    public async Task<BookDto?> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var result = await _connection.QueryFirstOrDefaultAsync<BookDto>(
            "dbo.sp_Book_GetById",
            new { Id = id, UserId = userId },
            commandType: CommandType.StoredProcedure);
        return result;
    }

    public async Task<IReadOnlyList<BookDto>> SearchAsync(int userId, string term, CancellationToken cancellationToken = default)
    {
        var result = await _connection.QueryAsync<BookDto>(
            "dbo.sp_Book_Search",
            new { UserId = userId, Term = term },
            commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<IReadOnlyList<BookDto>> GetByAuthorAsync(int userId, int authorId, CancellationToken cancellationToken = default)
    {
        var result = await _connection.QueryAsync<BookDto>(
            "dbo.sp_Book_GetByAuthor",
            new { UserId = userId, AuthorId = authorId },
            commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<IReadOnlyList<BookDto>> GetByCategoryAsync(int userId, int categoryId, CancellationToken cancellationToken = default)
    {
        var result = await _connection.QueryAsync<BookDto>(
            "dbo.sp_Book_GetByCategory",
            new { UserId = userId, CategoryId = categoryId },
            commandType: CommandType.StoredProcedure);
        return result.ToList();
    }
}
