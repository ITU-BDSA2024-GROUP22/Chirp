using System.Data;
using Microsoft.Data.Sqlite;
using SimpleDB;
using Chirp.CLI;

public class SQLiteDB : IDatabaseRepository<Cheeps>
{
    string sqlDBFilePath = "/tmp/chirp.db";

    private static SQLiteDB instance = new();

    public static SQLiteDB Instance
    {
        get { return instance; }
    }

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
                for (int i = 0; i < dataRecord.FieldCount; i++)
                    Console.WriteLine($"{dataRecord.GetName(i)}: {dataRecord[i]}");

                // See https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqldatareader.getvalues?view=dotnet-plat-ext-7.0
                // for documentation on how to retrieve complete columns from query results
                Object[] values = new Object[reader.FieldCount];
                int fieldCount = reader.GetValues(values);
                for (int i = 0; i < fieldCount; i++)
                    Console.WriteLine($"{reader.GetName(i)}: {values[i]}");
            }
        }
    }
}    
