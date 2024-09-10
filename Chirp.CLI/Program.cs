// See https://aka.ms/new-console-template for more information

using System.Globalization;
using System.IO;
using CsvHelper;

class Program
{
    public record Cheep(string Author, string Message, long Timestamp);

    static void Main(string[] args)
    {
        string input = args[0];
        if (args.Length > 0 && input == "cheep")
        {
            string message = string.Join(" ", args, 1, args.Length - 1);
            toCSV(message);
        }    
        else
        {
            try
            {
                using (StreamReader reader = new StreamReader("chirp_cli_db.csv"))
                using (CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                     var cheeps = csvReader.GetRecords<Cheep>();
                     foreach (var cheep in cheeps)
                     {
                         Console.WriteLine(cheep.Author + " @ " + dateConverter(cheep.Timestamp) + ": " + cheep.Message);
                     }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read");
                Console.WriteLine(e.Message);
            }
        }
    }

    static string dateConverter(long unix)
    {
        DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(unix + 3600);
        return dto.ToString();
    }

    static void toCSV(string message)
    {
        string user = Environment.UserName;
        int unixTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        Cheep cheep = new Cheep(user, message, unixTime);
        
        using (StreamWriter writer = File.AppendText("chirp_cli_db.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.NextRecord();
            csv.WriteRecord(cheep);
        }
    }
}
