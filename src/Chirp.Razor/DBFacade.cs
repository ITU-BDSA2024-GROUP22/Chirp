using System.Data;
using Microsoft.Data.Sqlite;

namespace Chirp.Razor;

public class DBFacade
{
    private const string sqlGetCheeps = "SELECT username, text, pub_date FROM message JOIN user ON user_id = author_id";

    private readonly SqliteConnection connection;
    //private const string sqlGetAuthor = 

    public DBFacade()
    {
        var sqlDBFilePath = Environment.GetEnvironmentVariable("CHIRPDBPATH") ?? "/tmp/chirp.db";
        connection =
            new SqliteConnection($"Data Source={sqlDBFilePath}"); //vi har nu en connection til en database
    }

    public void Open()
    {
        connection.Open();
    }

    public List<Cheep> GetCheeps()
    {
        var result = new List<Cheep>();
        var command = connection.CreateCommand();
        command.CommandText = sqlGetCheeps;

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var dataRecord = (IDataRecord)reader;
            result.Add(new Cheep(dataRecord[0].ToString(), dataRecord[1].ToString(),
                (long)dataRecord[2])); // Tjek at den viser unix ordentligt på hjemmeside
        }

        return result;
    }

    public List<Cheep> GetCheepsFromAuthor(string author)
    {
        return new List<Cheep>();
    }


    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}