using System.Globalization;
using CsvHelper;

namespace SimpleDB.Tests;

public class CSVDatabaseUnitTests
{
    [Fact]
    public void Test_Read()
    {
        var csvContent = "Author,Message,Timestamp\nUserOne,Hello,1625150703\nUserTwo,World,1625150803";
        using var reader = new StringReader(csvContent);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        
        var records = csv.GetRecords<Program.Cheep>().ToList();
        
        Assert.NotNull(records);
        Assert.Equal(2, records.Count);
    }
}