using System.Data;
using Microsoft.Data.Sqlite;

namespace Chirp.Razor;

public class DBFacade
{
    private SqliteConnection connection;
    private const string sqlGetCheeps = "SELECT username, text, pub_date FROM message JOIN user ON user_id = author_id";
    //private const string sqlGetAuthor = 

    public DBFacade()
    {
        var sqlDBFilePath = System.Environment.GetEnvironmentVariable("CHIRPDBPATH") ?? "/tmp/chirp.db";
        this.connection =
            new SqliteConnection($"Data Source={sqlDBFilePath}"); //vi har nu en connection til en database
    }

    public void Open()
    {
        this.connection.Open();
    }

    public List<CheepViewModel> GetCheeps()
    {
        var result = new List<CheepViewModel>();
        var command = connection.CreateCommand();
        command.CommandText = sqlGetCheeps;

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var dataRecord = (IDataRecord)reader;
            result.Add(new CheepViewModel(dataRecord[0].ToString(), dataRecord[1].ToString(), UnixTimeStampToDateTimeString((Int64)dataRecord[2])));
        }

        return result;
    }
    
    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        return new List<CheepViewModel>();
    }
    
    
    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
    
}