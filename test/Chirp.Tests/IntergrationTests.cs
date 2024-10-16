using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Chirp.Tests;
using Web;
using Microsoft.AspNetCore.Mvc.Testing;

public class TestGetHttpClient : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TestGetHttpClient(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
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
