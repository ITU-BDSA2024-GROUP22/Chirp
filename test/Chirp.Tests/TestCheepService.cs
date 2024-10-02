namespace Chirp.Tests;

public class TestCheepService
{

    //Test that dateConverter from DBFacade converts timestamp correctly

    [Fact]
    public void TestDateConverter()
    {
        double unixTimestamp = 1690891760;
        var expectedTime = "08/01/23 12:09:20";

        var result = Razor.DBFacade.UnixTimeStampToDateTimeString(unixTimestamp);

        Assert.Equal(expectedTime, result);
    }
}
