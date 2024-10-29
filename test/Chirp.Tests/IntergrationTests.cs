using Chirp.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure;
using Chirp.Web;

namespace Chirp.Tests
{
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

                        db.Database.EnsureCreated();

                        SeedTestData(db);
                    }
                });
            });
            _client = _factory.CreateClient();
        }

        // Seed test data in the in-memory database
        private void SeedTestData(DBContext context)
        {
            var authorAdrian = new Author
            {
                UserName = "Adrian",
                Email = "adrian@example.com"
            };

            var authorHelge = new Author
            {
                UserName = "Helge",
                Email = "helge@example.com"
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
        public async void TimeLineTest()
        {
            var response = await _client.GetAsync("/");
            response.EnsureSuccessStatusCode();

            var publicTL = await response.Content.ReadAsStringAsync();
            Assert.Contains("Public Timeline", publicTL);
        }

        [Fact]
        public async void AuthorTest()
        {
            var response = await _client.GetAsync("/Helge");
            response.EnsureSuccessStatusCode();

            var helgeCheep = await response.Content.ReadAsStringAsync();
            Assert.Contains("Hello, BDSA students!", helgeCheep);
        }

        [Fact]
        public async void PrivateTimeLineTest()
        {
            var response = await _client.GetAsync("/Adrian");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Hej, velkommen til kurset", responseString);
            Assert.Contains("Adrian", responseString);
        }
    }
}
