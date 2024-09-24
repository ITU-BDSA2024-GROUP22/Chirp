using SimpleDB;
using Chirp.CLI;
using Microsoft.AspNetCore.Builder;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

// Bed om databasen og så brug database.read til at læse/vise cheeps
IDatabaseRepository<Cheep> database = CSVDatabase<Cheep>.Instance;

app.MapGet("/cheeps", () => database.Read());
app.MapPost("/cheep", (Cheep cheep) => { app.MapPost("/cheep", (Cheep cheep) =>
    {
        if (cheep == null)
        {
            return Results.BadRequest("Invalid Cheep object.");
        }
        // Add logging or debugging here to inspect the `cheep` object
        return Results.Ok("Cheep received.");
    });
});
app.Run();

public record Cheep(string Author, string Message, long Timestamp);