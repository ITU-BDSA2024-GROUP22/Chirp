
using System.Net;
using System.Net.Http.Headers;
using Chirp.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure;
using Chirp.Web;

namespace Chirp.Tests;

public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

public IntegrationTests(WebApplicationFactory<Program> factory)
{
  _factory = factory.WithWebHostBuilder(builder =>
  {
      builder.ConfigureServices(services =>
      {
          var descriptor = services.SingleOrDefault(
              d => d.ServiceType == typeof(DbContextOptions<DBContext>));

          if (descriptor != null)
          {
              services.Remove(descriptor);
          }

          services.AddDbContext<DBContext>(options =>
          {
              options.UseInMemoryDatabase("InMemoryChirpTestDB");
          });

          var sp = services.BuildServiceProvider();
          using (var scope = sp.CreateScope())
          {
              var scopedServices = scope.ServiceProvider;
              var db = scopedServices.GetRequiredService<DBContext>();

              db.Database.EnsureDeleted();
              db.Database.EnsureCreated();

              SeedTestData(db);
          }
      });
  });
  _client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true, HandleCookies = true });
}

// Seed test data in the in-memory database
private void SeedTestData(DBContext context)
{
  var authorAdrian = new Author
  {
      UserName = "Adrian",
      Email = "adrian@example.com",
      FollowingList = new List<Author>()
  };

  var authorHelge = new Author
  {
      UserName = "Helge",
      Email = "helge@example.com",
      FollowingList = new List<Author>()
  };

  context.Authors.Add(authorAdrian);
  context.Authors.Add(authorHelge);

  var adrianCheep = new Cheep
  {
      Author = authorAdrian,
      Text = "Hej, velkommen til kurset",
      TimeStamp = DateTime.Now
  };

  var helgeCheep = new Cheep
  {
      Author = authorHelge,
      Text = "Hello, BDSA students!",
      TimeStamp = DateTime.Now
  };

  context.Cheeps.Add(adrianCheep);
  context.Cheeps.Add(helgeCheep);

  context.SaveChanges();
}


    [Fact]
    public async Task TimeLineTest()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();

        var publicTL = await response.Content.ReadAsStringAsync();
        Assert.Contains("Public Timeline", publicTL);
    }


    [Fact]
    public async Task AuthorTest()
    {
        var author = new Author
        {
            UserName = "Alice",
            Email = "Alice@example.com",
            FollowingList = new List<Author>()
        };

        var authorCheep = new Cheep
        {
            Author = author,
            Text = "Hello, BDSA students!",
            TimeStamp = DateTime.Now
        };

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DBContext>();

            db.Authors.Add(author);
            db.Cheeps.Add(authorCheep);
            db.SaveChanges();
        }

        var response = await _client.GetAsync("/Alice");
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();

        Assert.Contains("Hello, BDSA students!", responseContent);
    }

    [Fact]
    public async Task PrivateTimeLineTest()
    {
        var author = new Author
        {
            UserName = "Bob",
            Email = "Bob@example.com",
            FollowingList = new List<Author>()
        };

        var authorCheep = new Cheep
        {
            Author = author,
            Text = "Hello, and welcome",
            TimeStamp = DateTime.Now
        };

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DBContext>();

            db.Authors.Add(author);
            db.Cheeps.Add(authorCheep);
            db.SaveChanges();
        }

        var response = await _client.GetAsync("/Bob");
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();

        Assert.Contains("Hello, and welcome", responseContent);
        Assert.Contains("Bob", responseContent);
    }

    [Fact]
    public async Task AuthorTimelineTest()
    {
        var testAuthor = new Author
        {
            UserName = "Charlie",
            Email = "Charlie@example.com",
            FollowingList = new List<Author>()
        };

        var authorCheepOne = new Cheep
        {
            Author = testAuthor,
            Text = "First test cheep",
            TimeStamp = DateTime.UtcNow.AddMinutes(-10)
        };
        var authorCheepTwo = new Cheep
        {
            Author = testAuthor,
            Text = "Second test cheep",
            TimeStamp = DateTime.UtcNow.AddMinutes(-5)
        };

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DBContext>();

            db.Authors.Add(testAuthor);
            db.Cheeps.Add(authorCheepOne);
            db.Cheeps.Add(authorCheepTwo);
            db.SaveChanges();
        }

        var response = await _client.GetAsync("/Charlie");
        response.EnsureSuccessStatusCode(); // Verify a successful response

        var responseContent = await response.Content.ReadAsStringAsync();

        // Asserting
        Assert.Contains("Charlie's Timeline", responseContent);
        Assert.Contains("First test cheep", responseContent);
        Assert.Contains("Second test cheep", responseContent);
    }

    [Fact]
    public async Task BioUserTimelineTest()
    {
        var author = new Author
        {
            UserName = "Diana",
            Email = "Diana@example.com",
            FollowingList = new List<Author>()
        };

        var authorCheep = new Cheep
        {
            Author = author,
            Text = "What is up",
            TimeStamp = DateTime.Now
        };

        var authorBio = new Bio
        {
            Author = author,
            Text = "This is a bio"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DBContext>();

            db.Authors.Add(author);
            db.Cheeps.Add(authorCheep);
            db.Bios.Add(authorBio);
            db.SaveChanges();
        }

        var response = await _client.GetAsync("/Diana");
        var responseContent = await response.Content.ReadAsStringAsync();

        // Asserting
        Assert.Contains("Diana's Timeline", responseContent);
        Assert.Contains("What is up", responseContent);
        Assert.Contains("This is a bio", responseContent);
    }

    [Fact]
    public async Task AboutMeTest()
    {
        var author = new Author
        {
            UserName = "Earl",
            Email = "Earl@example.com",
            FollowingList = new List<Author>()
        };

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DBContext>();

            db.Authors.Add(author);
            db.SaveChanges();
        }

        var response = await _client.GetAsync("/Earl/about-me");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
