namespace Chirp.CLI;

public static class UserInterface
{
    public static void PrintCheeps(IEnumerable<Program.Cheep> cheeps)
    { 
        foreach (var cheep in cheeps) 
        { 
            Console.WriteLine(cheep.Author + " @ " + dateConverter(cheep.Timestamp) + ": " + cheep.Message); 
        }
    } 
    
    private static string dateConverter(long unix) 
    { 
        DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(unix + 3600);
        return dto.ToString();
    }
}