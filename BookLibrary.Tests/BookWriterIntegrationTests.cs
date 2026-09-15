using BookLibrary.Application.DTOs;
using BookLibrary.Domain;
using BookLibrary.Infrastructure.Data;
using BookLibrary.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookLibrary.Tests;

public class BookWriterIntegrationTests
{
    private const string ConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Database=BookLibraryDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

    private BookLibraryDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<BookLibraryDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        return new BookLibraryDbContext(options);
    }

    [Fact]
    public async Task BookWriter_CreateUpdateDelete_ReturnsExpectedRowcounts()
    {
        await using var context = CreateDbContext();
        var writer = new BookWriter(context);

        // 1. Create a test book
        var uniqueIsbn = "TEST-" + Guid.NewGuid().ToString("N")[..10];
        var createRequest = new CreateBookRequest(
            "Integration Test Book",
            uniqueIsbn,
            1,
            1,
            2025,
            BookStatus.Reading,
            10);

        var createdId = await writer.CreateAsync(1, createRequest);
        Assert.True(createdId > 0, "CreateAsync should return positive identity ID.");

        try
        {
            // 2. Update the test book (valid user and ID)
            var updateRequest = new UpdateBookRequest(
                "Integration Test Book Updated",
                uniqueIsbn,
                1,
                1,
                2026,
                BookStatus.Reading,
                11);

            var updateSuccess = await writer.UpdateAsync(createdId, 1, updateRequest);
            Assert.True(updateSuccess, "UpdateAsync should return true when the row is successfully updated.");

            // 3. Update with wrong UserId should return false
            var wrongUserUpdate = await writer.UpdateAsync(createdId, 99999, updateRequest);
            Assert.False(wrongUserUpdate, "UpdateAsync should return false when UserId does not match.");

            // 4. Update with non-existent ID should return false
            var nonExistentUpdate = await writer.UpdateAsync(-1, 1, updateRequest);
            Assert.False(nonExistentUpdate, "UpdateAsync should return false when book ID does not exist.");

            // 5. Delete with wrong UserId should return false
            var wrongUserDelete = await writer.DeleteAsync(createdId, 99999);
            Assert.False(wrongUserDelete, "DeleteAsync should return false when UserId does not match.");

            // 6. Delete with correct ID and UserId should return true
            var deleteSuccess = await writer.DeleteAsync(createdId, 1);
            Assert.True(deleteSuccess, "DeleteAsync should return true when the row is successfully deleted.");

            // 7. Second delete on same ID should return false (already deleted)
            var secondDelete = await writer.DeleteAsync(createdId, 1);
            Assert.False(secondDelete, "DeleteAsync should return false when row is already deleted.");
        }
        finally
        {
            // Clean up just in case assertions failed before delete
            try
            {
                await writer.DeleteAsync(createdId, 1);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
