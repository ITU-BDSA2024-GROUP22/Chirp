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


    //Skal nok finpusses lidt ;)
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


    [Fact]
    public async void GetCheepsFromAuthorTest()
    {
        // Arrange
        var repository = await SetUpRepositoryAsync();

        // Opret en forfatter og nogle cheeps for denne forfatter
        repository.CreateAuthor("Anders And", "anders@and.dk");

        //Gemme vores nye author i en instans af Author
        var author = repository.GetAuthorByName("Anders And");

        // Opret flere cheeps for forfatteren
        repository.CreateCheep(author, "Første Cheep", DateTime.Now.AddMinutes(-10));
        repository.CreateCheep(author, "Andet Cheep", DateTime.Now.AddMinutes(-5));
        repository.CreateCheep(author, "Tredje Cheep", DateTime.Now);

        // Act
        var result = await repository.GetCheepsFromAuthor(1, "Anders And");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count); // Der skulle være 3 cheeps for denne forfatter
        Assert.Equal("Tredje Cheep", result[0].Text); // Den nyeste cheep skulle være først
        Assert.Equal("Andet Cheep", result[1].Text);
        Assert.Equal("Første Cheep", result[2].Text);
    }

}
