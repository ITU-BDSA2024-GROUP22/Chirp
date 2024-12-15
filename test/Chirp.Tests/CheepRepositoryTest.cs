using Chirp.Core.Interfaces;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Tests;

public class CheepRepositoryTest
{

    private static async Task<ICheepRepository> SetUpRepositoryAsync()
    {
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
        var result = await repository.GetAuthorByName("Anna");
        Assert.Null(result);
    }


    [Fact]
    public async Task CreateCheepTest()
    {
        var repository = await SetUpRepositoryAsync();

        await repository.CreateAuthor("Anders And", "anders@and.dk");
        var author = repository.GetAuthorByName("Anders And");

        await repository.CreateCheep(await author, "Group 22 is so cool", DateTime.Now);

        var cheeps = await repository.GetCheepsFromAuthor(1, "Anders And");

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

        for (var i = 1; i <= 33; i++)
        {
            string number = i.ToString();
            var date = DateTime.Now.AddDays(i);
            await repository.CreateCheep(await author, number, date);
        }
        var cheepsPage1 = await repository.GetCheeps(1);
        var cheepsPage2 = await repository.GetCheeps(2);

        Assert.NotNull(cheepsPage1);
        Assert.Equal("33", cheepsPage1[0].Text);

        Assert.NotNull(cheepsPage2);
        Assert.Equal("1", cheepsPage2[0].Text);
    }


    [Fact]
    public async void GetCheepsFromAuthorTest()
    {
        var repository = await SetUpRepositoryAsync();

        await repository.CreateAuthor("Anders And", "anders@and.dk");

        var author = repository.GetAuthorByName("Anders And");

        await repository.CreateCheep(await author, "Første Cheep", DateTime.Now.AddMinutes(-10));
        await repository.CreateCheep(await author, "Andet Cheep", DateTime.Now.AddMinutes(-5));
        await repository.CreateCheep(await author, "Tredje Cheep", DateTime.Now);

        var result = await repository.GetCheepsFromAuthor(1, "Anders And");

        Assert.NotNull(result);
        Assert.Equal(3, result.Count); // Der skulle være 3 cheeps for denne forfatter
        Assert.Equal("Tredje Cheep", result[0].Text); // Den nyeste cheep skulle være først
        Assert.Equal("Andet Cheep", result[1].Text);
        Assert.Equal("Første Cheep", result[2].Text);
    }

}
