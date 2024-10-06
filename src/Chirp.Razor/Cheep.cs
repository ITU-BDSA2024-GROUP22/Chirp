namespace Chirp.Razor;

public class Cheep
{
    string text { get; set; }
    DateTime timeStamp { get; set; }
    int authorID { get; set; }
    Author author { get; set; }
}