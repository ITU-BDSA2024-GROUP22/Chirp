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

    /*
    [Fact]
    public void WriteCheepTest()
    {
    }
    */
    
    
    [Fact]
    
    public void TestReadCheep()
        {
            string output = "";
            using (var process = new Process())
            {
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = "run -- read 10"; 
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WorkingDirectory = "../../../../../src/Chirp.CLI";
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();

                // Læs standard output
                StreamReader reader = process.StandardOutput;
                output = reader.ReadToEnd();
                process.WaitForExit();
            }
        }
    public record Cheep(string Author, string Message, long Timestamp);
}



