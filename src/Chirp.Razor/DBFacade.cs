using Microsoft.Data.Sqlite;

namespace Chirp.Razor;

public class DBFacade
{
    // var sqlQuery = @"SELECT * FROM message ORDER by message.pub_date desc";

    private readonly SqliteConnection connection;

    public DBFacade()
    {
        var sqlDBFilePath = Environment.GetEnvironmentVariable("CHIRPDBPATH") ?? "/tmp/chirp.db";
        connection =
            new SqliteConnection($"Data Source={sqlDBFilePath}"); //vi har nu en connection til en database
        CreateDB(sqlDBFilePath);
    }

    private void CreateDB(string sqlDBFilePath)
    {
        var schemaSQL = File.ReadAllText("data/schema.sql");
        ExecuteQuery(schemaSQL);

        var dumpSQL = File.ReadAllText("data/dump.sql");
        ExecuteQuery(dumpSQL);
    }

    private void ExecuteQuery(string sqlFile)
    {
        connection.Open();
        var command = connection.CreateCommand();

        command.CommandText = sqlFile;
        command.ExecuteNonQuery();
    }

    public void Open()
    {
        connection.Open();
    }

    public List<Cheep> GetAllCheeps()
    {
        using (connection)
        {
            var results = new List<Cheep>();

            var allCheepsQuery = "SELECT username, text, pub_date FROM message JOIN user ON user_id = author_id";

            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = allCheepsQuery;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var user = reader.GetString(0);
                var message = reader.GetString(1);
                var unixTime = reader.GetInt64(2);

                results.Add(new Cheep(user, message, unixTime));
            }

            return results;
        }
    }

    public List<Cheep> GetCheeps(int pageNumber)
    {
        var pageSize = 32;

        using (connection)
        {
            var results = new List<Cheep>();

            var lowerBound = (pageNumber - 1) * pageSize;
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
        }
    }

    public List<Cheep> GetCheepsFromAuthor(int pageNumber, string username)
    {
        var pageSize = 32;

        using (connection)
        {
            var results = new List<Cheep>();

            var lowerBound = (pageNumber - 1) * pageSize;
            var pageQuery =
                "SELECT u.username, m.text, m.pub_date FROM message m JOIN user u ON m.author_id = u.user_id WHERE u.username = @username ORDER BY m.pub_date DESC LIMIT @pageSize OFFSET @lowerBound";

            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = pageQuery;

            command.Parameters.AddWithValue("@pageSize", pageSize);
            command.Parameters.AddWithValue("@lowerBound", lowerBound);
            command.Parameters.AddWithValue("@username", username);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var user = reader.GetString(0);
                var message = reader.GetString(1);
                var unixTime = reader.GetInt64(2);

                results.Add(new Cheep(user, message, unixTime));
            }

            return results;
        }
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}