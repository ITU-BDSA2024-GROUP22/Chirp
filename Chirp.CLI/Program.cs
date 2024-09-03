// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        
        try
        {
            using StreamReader reader = new("chirp_cli_db.csv");
            string text = reader.ReadLine();

            while (!reader.EndOfStream)
            {
                text = reader.ReadLine();
                
                int firstIndex = text.IndexOf(',');
                int lastIndex = text.LastIndexOf(',');
                
                string author = text.Substring(0,firstIndex);
                string message = text.Substring(firstIndex + 1, lastIndex - firstIndex - 1).TrimStart('\"').TrimEnd('\"');
                int date = int.Parse(text.Substring(lastIndex + 1));

                string output = (author + " @ " + dateConverter(date) + ": " + message);
                
                Console.WriteLine(output);
            }
            
        }
        catch (IOException e)
        {
            Console.WriteLine("The file could not be read");
            Console.WriteLine(e.Message);
        }
    }

    static string dateConverter(int unix)
    {
        DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(unix + 3600);
        return dto.ToString();
    }

    static string toCSV()
    {
        string user = Environment.UserName;
    }
}

/*
List<string> cheeps = new() { "Hello, BDSA students!", "Welcome to the course!", "I hope you had a good summer." };

foreach (var cheep in cheeps)
{
    Console.WriteLine(cheep);
    Thread.Sleep(1000);
}
*/