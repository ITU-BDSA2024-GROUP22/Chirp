using System.Runtime.CompilerServices;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Tests;

public class CheepRepositoryTest
{

    public async Task<ICheepRepository> SetUpRepositoryAsync()
    {
        // Arrange
        var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<DBContext>().UseSqlite(connection);

        var context = new DBContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); // Applies the schema to the database

        return new CheepRepository(context);
    }

    [Fact]
    public async void CheckThatRepositoryIsEmpty()
    {
        var repository = await SetUpRepositoryAsync();

        // Act
        var result = await repository.GetCheeps(1);
        Assert.Empty(result);
    }

    [Fact]
    public async void CreateAuthor()
    {
        var repository = await SetUpRepositoryAsync();
        await repository.CreateAuthor("Anders And", "anders@and.dk");
        var author = repository.GetAuthorByName("Anders And");
        Assert.NotNull(author);
    }

    [Fact]
    public async void GetAuthorByNameNotFound()
    {
        var repository = await SetUpRepositoryAsync();
        Assert.Throws<KeyNotFoundException>(() => repository.GetAuthorByName("Anna"));
    }

}
