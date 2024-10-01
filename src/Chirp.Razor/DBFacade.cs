using Microsoft.Data.Sqlite;

namespace Chirp.Razor;

public class DBFacade
{
    private const string sqlGetCheeps = "SELECT username, text, pub_date FROM message JOIN user ON user_id = author_id";
    //Vi skal ike have alle cheeps på samme tid mere, kun 32 stk ad gangen
    // var sqlQuery = @"SELECT * FROM message ORDER by message.pub_date desc";

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

    public List<Cheep> GetCheeps(int pageNumber)
    {
        var pageSize = 32;

        using (connection)
        {
            var results = new List<Cheep>();

            var lowerBound = (pageNumber - 1) * pageSize;
            //var pageQuery = "SELECT * FROM Cheeps ORDER BY CreatedDate DESC LIMIT @pageSize OFFSET @lowerBound";
            var pageQuery =
                "SELECT u.username, m.text, m.pub_date FROM message m JOIN user u ON m.author_id = u.user_id ORDER BY m.pub_date DESC LIMIT @pageSize OFFSET @lowerBound";

            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = pageQuery;

            command.Parameters.AddWithValue("@pageSize", pageSize);
            command.Parameters.AddWithValue("@lowerBound", lowerBound);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var user = reader.GetString(0);
                var message = reader.GetString(1);
                var unixTime = reader.GetInt64(2);

                results.Add(new Cheep(user, message, unixTime));
            }

            return results;
            /*
            var result = new List<Cheep>();
            var command = connection.CreateCommand();
            command.CommandText = sqlGetCheeps; // husk at indsætte den rigtige query

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var dataRecord = (IDataRecord)reader;
                result.Add(new Cheep(dataRecord[0].ToString(), dataRecord[1].ToString(),
                    (long)dataRecord[2])); // Tjek at den viser unix ordentligt på hjemmeside
            }

            return result;
            */
        }
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