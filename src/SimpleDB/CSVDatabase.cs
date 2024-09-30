using System.Globalization;
using CsvHelper;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDataaseRepository<T>
{
    private static CSVDatabase<T> instance;
    private static readonly object padlock = new();

    private readonly string dbPath = "../SimpleDB/chirp_cli_db.csv";

    private CSVDatabase()
    {
    }

    public static CSVDatabase<T> Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null) instance = new CSVDatabase<T>();
                return instance;
            }
        }
    }

    public IEnumerable<T> Read(int? limit = null)
    {
        try
        {
            using (var reader = new StreamReader(dbPath))
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var cheeps = csvReader.GetRecords<T>().ToList();
                return cheeps;
            }
        }
        catch (IOException e)
        {
            Console.WriteLine("The file could not be read");
            Console.WriteLine(e.Message);
        }

        return null;
    }

    public void Store(T record)
    {
        using (var writer = File.AppendText(dbPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.NextRecord();
            csv.WriteRecord(record);
        }
    }
}