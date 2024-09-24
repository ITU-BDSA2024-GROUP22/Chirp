using SimpleDB;
using Chirp.CLI;
using Microsoft.AspNetCore.Builder;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

IDatabaseRepository<Cheep> database = CSVDatabase<Cheep>.Instance;

app.MapGet("/cheeps", () => database.Read(3));
app.MapPost("/cheep", (Cheep cheep) => database.Store(cheep));
app.Run();

public record Cheep(string Author, string Message, long Timestamp);