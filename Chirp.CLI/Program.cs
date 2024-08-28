// See https://aka.ms/new-console-template for more information

try
{
    using StreamReader reader = new("chirp_cli_db.csv");
    string text = reader.ReadToEnd();
    Console.WriteLine(text);
}
catch (IOException e)
{
    Console.WriteLine("The file could not be read");
    Console.WriteLine(e.Message);
}

/*
List<string> cheeps = new() { "Hello, BDSA students!", "Welcome to the course!", "I hope you had a good summer." };

foreach (var cheep in cheeps)
{
    Console.WriteLine(cheep);
    Thread.Sleep(1000);
}
*/