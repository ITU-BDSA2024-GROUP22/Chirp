using Chirp.CLI;

namespace Chirp.Tests;

public class UserInterfaceTests
{
    [Fact]
    public void TestDateConverter()
    {
        long unixTimestamp = 1690891760;
        var expectedTime = "08/01/23 14:09:20";

        var result = UserInterface.dateConverter(unixTimestamp);

        Assert.Equal(expectedTime, result);
    }

    [Fact]
    public void PrintCheeps_ShouldPrintCorrectly()
    {
        var cheeps = new List<Program.Cheep>
        {
            new("Alice", "Hello, World!", 1633098671),
            new("Bob", "Goodbye, World!", 1633102271)
        };

        var expectedOutput = "Alice @ 10/01/21 16:31:11: Hello, World!\n" +
                             "Bob @ 10/01/21 17:31:11: Goodbye, World!\n";

        using (var sw = new StringWriter())
        {
            Console.SetOut(sw);

            UserInterface.PrintCheeps(cheeps);

            var actualOutput = sw.ToString();

            Assert.Equal(expectedOutput, actualOutput);
        }
    }
}