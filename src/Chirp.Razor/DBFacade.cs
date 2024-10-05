using System.Globalization;
using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.FileProviders;

namespace Chirp.Razor;

public class DBFacade
{
    // var sqlQuery = @"SELECT * FROM message ORDER by message.pub_date desc";

    private readonly SqliteConnection connection;
    private readonly int pageSize = 32;

    public DBFacade()
    {
        var sqlDBFilePath = Environment.GetEnvironmentVariable("CHIRPDBPATH") ?? "/tmp/chirp.db";
        connection =
            new SqliteConnection($"Data Source={sqlDBFilePath}"); //vi har nu en connection til en database
        CreateDB("data/schema.sql");
        CreateDB("data/dump.sql");
    }

    private void CreateDB(string sqlPath)
    {
        var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
        using var reader = embeddedProvider.GetFileInfo(sqlPath).CreateReadStream();
        using var sr = new StreamReader(reader);

        var query = sr.ReadToEnd();

        ExecuteQuery(query);
    }

    private void ExecuteQuery(string sqlQuery)
    {
        Open();
        var command = connection.CreateCommand();

        command.CommandText = sqlQuery;
        command.ExecuteNonQuery();
    }

    public void Open()
    {
        connection.Open();
    }

    public void Close()
    {
        connection.Close();
    }

    public List<Cheep> GetAllCheeps()
    {
        using (connection)
        {
            var results = new List<Cheep>();

            var allCheepsQuery = "SELECT username, text, pub_date FROM message JOIN user ON user_id = author_id";

            Open();
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
        using (connection)
        {
            var results = new List<Cheep>();

            var lowerBound = (pageNumber - 1) * pageSize;
            var pageQuery =
                "SELECT u.username, m.text, m.pub_date FROM message m JOIN user u ON m.author_id = u.user_id ORDER BY m.pub_date DESC LIMIT @pageSize OFFSET @lowerBound";

            Open();
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
        using (connection)
        {
            var results = new List<Cheep>();
            var lowerBound = (pageNumber - 1) * pageSize;

            var authorPageQuery =
                "SELECT u.username, m.text, m.pub_date FROM message m JOIN user u ON m.author_id = u.user_id WHERE u.username = @username ORDER BY m.pub_date DESC LIMIT @pageSize OFFSET @lowerBound";

            Open();
            var command = connection.CreateCommand();
            command.CommandText = authorPageQuery;

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

    //Metode der returnerer alle cheeps en author (user) har skrevet
    public List<Cheep> GetAllCheepsFromAuthor(string username)
    {
        using (connection)
        {
            var results = new List<Cheep>();

            var allCheepsQuery =
                "SELECT u.username, m.text, m.pub_date FROM message m JOIN user u ON m.author_id = u.user_id WHERE u.username = @username ORDER BY m.pub_date DESC ";

            Open();
            var command = connection.CreateCommand();
            command.CommandText = allCheepsQuery;
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


    public static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        var newdateTime = dateTime.ToString("MM/dd/yy HH:mm:ss", CultureInfo.InvariantCulture);
        return newdateTime;
    }
}
