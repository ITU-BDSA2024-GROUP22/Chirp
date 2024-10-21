using System.Runtime.CompilerServices;
using Chirp.Core;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Tests;

public class AuthorTest
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
    public async void GetEmail()
    {
        var repository = await SetUpRepositoryAsync();
        repository.CreateAuthor("Anders And", "anders@and.dk");

        var author = repository.GetAuthorByName("Anders And");

        Assert.Equal("anders@and.dk", author.Email);
    }

    [Fact]
    public async void GetName()
    {
        var repository = await SetUpRepositoryAsync();
        repository.CreateAuthor("Anders And", "anders@and.dk");

        var author = repository.GetAuthorByName("Anders And");

        Assert.Equal("Anders And", author.Name);
    }

}
