using Chirp.CLI;
using DocoptNet;
using SimpleDB;

public class Program
{
    public static void Main(string[] args)
    {
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
    }

    public record Cheep(string Author, string Message, long Timestamp);
}