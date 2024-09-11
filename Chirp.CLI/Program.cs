
using System.Globalization;
using System.IO;
using Chirp.CLI;
using CsvHelper;
using DocoptNet;
using SimpleDB;

public class Program
{
    public record Cheep(string Author, string Message, long Timestamp);
    
    public static void Main(string[] args)
    {
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
        
        IDatabaseRepository<Cheep> database = new CSVDatabase<Cheep>();
        
        if (arguments["cheep"].IsTrue) // args 0 == "read" 
        {
            string message = arguments["<message>"].ToString();
            int unixTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

            database.Store(new Cheep(Environment.UserName, message, unixTime));
        }    
        else if(arguments["read"].IsTrue)
        {
            var cheeps = database.Read(3);
            UserInterface.PrintCheeps(cheeps);
        }
    }
}
