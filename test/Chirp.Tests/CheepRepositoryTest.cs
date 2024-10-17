using System.Runtime.CompilerServices;
using Chirp.Core;
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
        repository.CreateAuthor("Anders And", "anders@and.dk");
        var author = repository.GetAuthorByName("Anders And");
        Assert.NotNull(author);
    }

    [Fact]
    public async void GetAuthorByNameNotFound()
    {
        var repository = await SetUpRepositoryAsync();
        Assert.Throws<KeyNotFoundException>(() => repository.GetAuthorByName("Anna"));
    }




    [Fact]
    public async void GetAuthorByEmailTest()
    {
        var repository = await SetUpRepositoryAsync();
        repository.CreateAuthor("Anders And", "anders@and.dk");

        var author = repository.GetAuthorByName("Anders And");

        Assert.Equal("anders@and.dk", author.Email);
    }




    [Fact]
    public async void CreateCheep()
    {
        var repository = await SetUpRepositoryAsync();
        //Create author first
        repository.CreateAuthor("Anders And", "anders@and.dk");
        var author = repository.GetAuthorByName("Anders And");

        //Now create new cheep with the author
        repository.CreateCheep(author, "Group 22 is so cool", DateTime.Now);

        //Check if the Anders Ands page has cheeps now
        Assert.NotNull(repository.GetCheepsFromAuthor(1, "Anders And"));
    }
}
