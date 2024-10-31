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
    public async void CheckThatRepositoryIsEmptyTest()
    {
        var repository = await SetUpRepositoryAsync();

        // Act
        var result = await repository.GetCheeps(1);
        Assert.Empty(result);
    }

    [Fact]
    public async void CreateAuthorTest()
    {
        var repository = await SetUpRepositoryAsync();
        await repository.CreateAuthor("Anders And", "anders@and.dk");
        var author = repository.GetAuthorByName("Anders And");
        Assert.NotNull(author);
    }

    [Fact]
    public async void GetAuthorByNameNotFoundTest()
    {
        var repository = await SetUpRepositoryAsync();
        await Assert.ThrowsAsync<KeyNotFoundException>(() => repository.GetAuthorByName("Anna"));
    }


    [Fact]
    public async Task CreateCheepTest()
    {
        var repository = await SetUpRepositoryAsync();

        //Create author first
        await repository.CreateAuthor("Anders And", "anders@and.dk");
        var author = repository.GetAuthorByName("Anders And");

        //Now create new cheep with the author
        await repository.CreateCheep(await author, "Group 22 is so cool", DateTime.Now);

        var cheeps = await repository.GetCheepsFromAuthor(1, "Anders And");
        //Check if the Anders Ands page has cheeps now
        Assert.NotNull(repository.GetCheepsFromAuthor(1, "Anders And"));
        Assert.Single(cheeps);
        Assert.Equal("Group 22 is so cool", cheeps.First().Text);
    }

    [Fact]
    public async void GetCheepTest()
    {
        var repository = await SetUpRepositoryAsync();
        await repository.CreateAuthor("Anders And", "anders@and.dk");
        var author = repository.GetAuthorByName("Anders And");

        //Create 33 cheeps
        for (var i = 1; i <= 33; i++)
        {
            string number = i.ToString();
            var date = DateTime.Now.AddDays(i);
            await repository.CreateCheep(await author, number, date);
        }
        var cheepsPage1 = await repository.GetCheeps(1);
        var cheepsPage2 = await repository.GetCheeps(2);

        //Nyeste cheep skal være den på første side
        Assert.NotNull(cheepsPage1);
        Assert.Equal("33", cheepsPage1[0].Text);

        //Første cheep lavet skal være den første på side 2
        Assert.NotNull(cheepsPage2);
        Assert.Equal("1", cheepsPage2[0].Text);
    }


    [Fact]
    public async void GetCheepsFromAuthorTest()
    {
        // Arrange
        var repository = await SetUpRepositoryAsync();

        // Opret en forfatter og nogle cheeps for denne forfatter
        await repository.CreateAuthor("Anders And", "anders@and.dk");

        //Gemme vores nye author i en instans af Author
        var author = repository.GetAuthorByName("Anders And");

        // Opret flere cheeps for forfatteren
        await repository.CreateCheep(await author, "Første Cheep", DateTime.Now.AddMinutes(-10));
        await repository.CreateCheep(await author, "Andet Cheep", DateTime.Now.AddMinutes(-5));
        await repository.CreateCheep(await author, "Tredje Cheep", DateTime.Now);

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
