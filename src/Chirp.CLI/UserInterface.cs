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
    
    public static string dateConverter(long unix) 
    { 
        DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(unix);
        var localTime = dto.LocalDateTime;
        return localTime.ToString("MM'/'dd'/'yy HH':'mm':'ss");
    }
}