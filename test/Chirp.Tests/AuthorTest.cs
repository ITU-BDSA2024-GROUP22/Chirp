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

    //Denne test bliver muligvis fjernet, pga hvor er mail????

    /*
    [Fact]
    public async void GetEmail()
    {
        var repository = await SetUpRepositoryAsync();
        await repository.CreateAuthor("Anders And", "anders@and.dk");

        var author = repository.GetAuthorByName("Anders And");

        Assert.Equal("anders@and.dk", author.Email);
    }
    */

    //Skal måske fjernes grundet af .Username ikke fungerer længere??
    /*

    [Fact]
    public async void GetName()
    {
        var repository = await SetUpRepositoryAsync();
        await repository.CreateAuthor("Anders And", "anders@and.dk");

        var author = repository.GetAuthorByName("Anders And");

        Assert.Equal("Anders And", author.UserName);
    }
    */

}
