using System.Collections.Generic;
using System.Globalization;
using CsvHelper;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
     private string dbPath = "../SimpleDB/chirp_cli_db.csv";
    
    public IEnumerable<T> Read(int? limit = null)
    {
        try
        {
            using (StreamReader reader = new StreamReader(dbPath))
            using (CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var cheeps = csvReader.GetRecords<T>().ToList();
                return cheeps;
                //
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