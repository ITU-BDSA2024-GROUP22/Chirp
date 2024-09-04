// See https://aka.ms/new-console-template for more information


static void Main(String[]args)
{
    try
    {
        using StreamReader reader = new("chirp_cli_db.csv");
        while(reader.hasNext)
        { 
            string text = reader.Readline();

            string[] array =  text.split(",");
            string date = dateConverter(array[2]);

            Console.WriteLine(text);
        }
    }
    catch (IOException e)
    {
        Console.WriteLine("The file could not be read");
        Console.WriteLine(e.Message);
    }

}

static string dateConverter(int unix)
{
    int date = unix;
    int d = date % 100;
    int m = (date / 100) % 100;
    int y = date / 10000;

    var result = new DateTime(y, m, d);

}






/*
List<string> cheeps = new() { "Hello, BDSA students!", "Welcome to the course!", "I hope you had a good summer." };

foreach (var cheep in cheeps)
{
    Console.WriteLine(cheep);
    Thread.Sleep(1000);
}

/*
List<string> cheeps = new() { "Hello, BDSA students!", "Welcome to the course!", "I hope you had a good summer." };

foreach (var cheep in cheeps)
{
    Console.WriteLine(cheep);
    Thread.Sleep(1000);
}
*/