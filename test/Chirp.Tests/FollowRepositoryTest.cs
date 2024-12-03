using Chirp.Core.DTOs;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Tests;

public class FollowRepositoryTest
{
    public Task<DBContext> context;
    private static async Task<DBContext> CreateContext()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<DBContext>().UseSqlite(connection);

        var context = new DBContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); // Applies the schema to the database

        return context;
    }

    [Fact]
    public async Task AddFollowTest()
    {
        var followRepository = new FollowRepository(context);
        var cheepRepository = new CheepRepository(await CreateContext());

        await cheepRepository.CreateAuthor("Sten Ben", "sten@ben.dk");
        await cheepRepository.CreateAuthor("Peter Plys", "peter@plys.com");

        await followRepository.AddFollow("sten@ben.dk", "Peter Plys");

        var followingList = await followRepository.GetFollowingList("Sten Ben");

        Assert.NotNull(followingList);
        Assert.Single(followingList);
        Assert.Equal("Peter Plys", followingList.First().DisplayName);
    }

    [Fact]
    public async Task RemoveFollowTest()
    {
        var followRepository = new FollowRepository(await CreateContext());
        var cheepRepository = new CheepRepository(await CreateContext());

        await cheepRepository.CreateAuthor("Sten Ben", "sten@ben.dk");
        await cheepRepository.CreateAuthor("Peter Plys", "peter@plys.com");

        await followRepository.AddFollow("Sten Ben", "Peter Plys");
        await followRepository.Unfollow("Sten Ben", "Peter Plys");

        var followingList = await followRepository.GetFollowingList("Sten Ben");

        Assert.NotNull(followingList);
        Assert.Empty(followingList);
    }

    [Fact]
    public async Task GetCheepsFromFollowingTest()
    {
        var followRepository = new FollowRepository(await CreateContext());
        var cheepRepository = new CheepRepository(await CreateContext());

        await cheepRepository.CreateAuthor("Sten Ben", "sten@ben.dk");
        await cheepRepository.CreateAuthor("Peter Plys", "peter@plys.com");
        var peterPlys = await cheepRepository.GetAuthorByName("Peter Plys");
        await cheepRepository.CreateCheep(peterPlys!, "Jeg elsker honning", DateTime.UtcNow);

        await followRepository.AddFollow("Sten Ben", "Peter Plys");

        var cheeps = await followRepository.GetCheepsFromFollowing(1, "Sten Ben");

        Assert.NotNull(cheeps);
        Assert.Single(cheeps);
        Assert.Equal("Jeg elsker honning", cheeps.First().Text);
    }
}
