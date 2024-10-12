/*
using Chirp.Web;


namespace Chirp.Tests;

public class TestDBFacade
{
    private DBFacade _dbFacade;

    public TestDBFacade()
    {
        // Initialiserer DBFacade i konstruktøren
        _dbFacade = new DBFacade();
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


    [Fact]
    public void TestGetAllCheepsFromAuthor()
    {
        //Først laver jeg en mock database og tilføjer nogle cheeps

        //Antal cheeps i databasen
        var expectedCheepsInDatabase = 388;
        var result = _dbFacade.GetAllCheepsFromAuthor("Jacqualine Gilcoine");

        Assert.Equal(expectedCheepsInDatabase, result.Count);
    }


    [Fact]
    public void TestGetCheepsFromAuthorMellieYost()
    {
        //Først laver jeg en mock database og tilføjer nogle cheeps

        //Antal cheeps i databasen
        var expectedCheepsInDatabase = 7;
        var result = _dbFacade.GetAllCheepsFromAuthor("Mellie Yost");

        Assert.Equal(expectedCheepsInDatabase, result.Count);
    }



    //Returnerer antallet af cheeps som bliver returnet. Det er 32 grundet fejl i paginisation
    [Fact]
    public void TestPagesFromAuthor()
    {
        //Først laver jeg en mock database og tilføjer nogle cheeps

        //Antal Cheeps som Jacqualine Gilcoine har skrevet der er i databasen

        var expectedCheepsInDatabase = 32;

        //Lige nu fungerer det ikke helt rigtigt grundet at vi egentlig siger at den skal lave os 1 side med kun Jacqualines
        //cheep
        var result = _dbFacade.GetCheepsFromAuthor(1, "Jacqualine Gilcoine");

        Assert.Equal(expectedCheepsInDatabase, result.Count);
    }

}
*/
