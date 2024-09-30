using System.Data;
using Chirp.SQLite;
using Microsoft.Data.Sqlite;
using SimpleDB;

public class SQLiteDB : IDatabaseRepository<Cheep>
{
    private readonly string sqlDBFilePath = "/tmp/chirp.db";

    private SQLiteDB()
    {
        var sqlQuery = @"SELECT * FROM message ORDER by message.pub_date desc";
        using (var connection = new SqliteConnection($"Data Source={sqlDBFilePath}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                // https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqldatareader?view=dotnet-plat-ext-7.0#examples
                var dataRecord = (IDataRecord)reader;
                for (var i = 0; i < dataRecord.FieldCount; i++)
                    Console.WriteLine($"{dataRecord.GetName(i)}: {dataRecord[i]}");

                // See https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqldatareader.getvalues?view=dotnet-plat-ext-7.0
                // for documentation on how to retrieve complete columns from query results
                var values = new object[reader.FieldCount];
                var fieldCount = reader.GetValues(values);
                for (var i = 0; i < fieldCount; i++)
                    Console.WriteLine($"{reader.GetName(i)}: {values[i]}");
            }
        }
    }

    public static SQLiteDB Instance { get; } = new();

    public List<Cheep> Read(int pageNumber)
    {
        var pageSize = 32;

        using (var connection = new SqliteConnection($"Data Source={sqlDBFilePath}"))
        {
            var results = new List<Cheep>();

            var lowerBound = (pageNumber - 1) * pageSize;
            var pageQuery = "SELECT * FROM Cheeps ORDER BY CreatedDate DESC LIMIT {pageSize} OFFSET {lowerBound}";

            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = pageQuery;

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

    public void Store(Cheep record)
    {
        throw new NotImplementedException();
    }
}