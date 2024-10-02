using Chirp.Razor;


namespace Chirp.Tests;

public class ProgramUnitTests
{
    private DBFacade _dbFacade;

    public ProgramUnitTests()
    {
        // Initialiserer DBFacade i konstruktøren
        _dbFacade = new DBFacade();
    }

    //Test that dateConverter from DBFacade converts timestamp correctly
    [Fact]
    public void TestDateConverter()
    {
        double unixTimestamp = 1690891760;
        var expectedTime = "08/01/23 12:09:20";

        var result = Razor.DBFacade.UnixTimeStampToDateTimeString(unixTimestamp);

        Assert.Equal(expectedTime, result);
    }

    [Fact]
    public void TestGetCheeps()
    {
        //Først laver jeg en mock database og tilføjer nogle cheeps

        //Antal cheeps i databasen
        var expectedCheepsInDatabase = 686;
        var result = _dbFacade.GetAllCheeps();

        Assert.Equal(expectedCheepsInDatabase, result.Count);
    }

}
