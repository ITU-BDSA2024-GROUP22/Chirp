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
        
        //Helge lecture4-notes
        
        var baseURL = "https://bdsagroup22chirpremotedb.azurewebsites.net/";
        using HttpClient client = new();
        client.BaseAddress = new Uri(baseURL);

        // Sequential execution
        var watch = System.Diagnostics.Stopwatch.StartNew();
        // first HTTP request
        var fstResponse = await client.GetAsync("/");
        // second HTTP request
        var sndResponse = await client.GetAsync("/");
        watch.Stop();

        Console.WriteLine($"Sequential HTTP requests ... done after {watch.ElapsedMilliseconds}ms");

        // Concurrent execution
        watch = System.Diagnostics.Stopwatch.StartNew();
        // first HTTP request
        var fstRequestTask = client.GetAsync("/");
        // second HTTP request
        var sndRequestTask = client.GetAsync("/");

        var thrdResponse = await fstRequestTask;
        var frthResponse = await sndRequestTask;
        watch.Stop();

        Console.WriteLine($"Concurrent HTTP requests ... done after {watch.ElapsedMilliseconds}ms");
        /*
        //Testing our release workflow with a new branch :)))))
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
            var unixTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

            database.Store(new Cheep(Environment.UserName, message, unixTime));
        }
        else if (arguments["read"].IsTrue)
        {
            var cheeps = database.Read(3);
            UserInterface.PrintCheeps(cheeps);
        }
        */
    }

    public record Cheep(string Author, string Message, long Timestamp);
}