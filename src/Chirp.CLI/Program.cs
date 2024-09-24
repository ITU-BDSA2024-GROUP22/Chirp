using Chirp.CLI;
using DocoptNet;
using SimpleDB;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

public class Program
{
    public static async Task Main(string[] args)
    {
        //var baseURL = "https://bdsagroup22chirpremotedb.azurewebsites.net/";
        var baseURL = "http://localhost:5012";
        
        const string usage = @"Chirp CLI version.

        Usage:
        chirp read <limit>
        chirp cheep <message>
        chirp (-h | --help)
        chirp --version

        Options:
        -h --help     Show this screen.
        --version     Show version.
        ";

        var arguments = new Docopt().Apply(usage, args, version: "1.0", exit: true)!;

        IDatabaseRepository<Cheep> database = CSVDatabase<Cheep>.Instance;

        if (arguments["cheep"].IsTrue) // args 0 == "read" 
        {
            var message = arguments["<message>"].ToString();
            WriteCheep(message, baseURL);
        }
        else if (arguments["read"].IsTrue)
        {
            ReadCheep(baseURL);
        }
        
    }

    public record Cheep(string Author, string Message, long Timestamp);

    public static void WriteCheep(string message, string baseURL)
    {
        var unixTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        var cheep = new Cheep(Environment.UserName, message, unixTime);
        using HttpClient client = new();
        client.BaseAddress = new Uri(baseURL);
        var response = client.PostAsJsonAsync("/cheep", cheep).Result;
        response.EnsureSuccessStatusCode();   
    }
    
    public static void ReadCheep(string baseURL)
    {
        using HttpClient client = new();
        client.BaseAddress = new Uri(baseURL);
        var response = client.GetFromJsonAsync<List<Cheep>>("/cheeps").Result;
        UserInterface.PrintCheeps(response); 
    }
    
    
}