using SimpleDB;
using Chirp.CLI;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

IDatabaseRepository<Cheep> database = CSVDatabase<Cheep>.Instance;

app.MapGet("/cheeps", () => database.Read(3));
app.MapPost("/cheep", (Cheep cheep) => database.Store(cheep));
app.Run();

public record Cheep(string Author, string Message, long Timestamp);