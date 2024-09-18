using System.Diagnostics;
using Chirp.CLI;
using SimpleDB;

namespace Chirp.Tests;

public class End2End
{
    [Fact]
    public void TestReadCheep()
    {
        // Arrange
        string output = "";
        using (var process = new Process())
        {
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = "run -- read 10"; // Kald CLI'en med "read 10"
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = "../../../../../src/Chirp.CLI"; // Korrekt sti
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            // Læs standard output
            StreamReader reader = process.StandardOutput;
            output = reader.ReadToEnd();
            process.WaitForExit();
        }

        // Act
        string[] cheeps = output.Trim().Split("\n");

        // Assert: Kontroller at outputtet er som forventet
        Assert.Equal("ropf @ 08/01/23 14:09:20: Hello, BDSA students!", cheeps[0].Trim());
        Assert.Equal("adho @ 08/02/23 14:19:38: Welcome to the course!", cheeps[1].Trim());
        Assert.Equal("adho @ 08/02/23 14:37:38: I hope you had a good summer.", cheeps[2].Trim());
        Assert.Equal("ropf @ 08/02/23 15:04:47: Cheeping cheeps on Chirp :)", cheeps[3].Trim());
    }

    
    /*

    [Fact]
    public void TestStoreCheep()
    {
        string expectedCheep = "Hello!!!";
        string output = "";
        using (var process = new Process())
        {

            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = "run -- cheep \"" + expectedCheep + "\""; // Korrekt kommando
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = "../../../../../src/Chirp.CLI"; // Korrekt sti
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            // Læs standard output
            StreamReader reader = process.StandardOutput;
            output = reader.ReadToEnd();
            process.WaitForExit();
        }

        Console.WriteLine("Full Output: " + output);

        string filePath = "chirp_cli_db.csv"; // Erstat med din CSV-fil sti
        string lastLine = string.Empty;

        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lastLine = line; // Opdater den sidste linje
            }
        }

        string[] cheeps = output.Trim().Split('\n');
        string latestCheep = cheeps.Length > 0 ? cheeps.Last().Trim() : string.Empty;
        // Assert: Kontroller at outputtet er som forventet
        Assert.Equal("Hello!!!", lastLine);

    }
    */
}

