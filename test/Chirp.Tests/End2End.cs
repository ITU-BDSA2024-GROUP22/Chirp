using System.Diagnostics;
using System.Net.Http.Json;
using Chirp.CLI;
using SimpleDB;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Chirp.Tests;

public class End2End : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;
    private readonly string baseUrl = "http://localhost:5012";
    
    public End2End(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public void WriteCheepTest()
    {
        
        string message = "This is a test cheep";
        
        Program.WriteCheep(message, baseUrl);
        
        var cheeps = client.GetFromJsonAsync<List<Cheep>>("/cheeps").Result;
        
        Assert.Contains(cheeps, c => c.Message == message);
    }
    
    public record Cheep(string Author, string Message, long Timestamp);

}



