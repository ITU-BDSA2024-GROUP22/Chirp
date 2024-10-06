namespace Chirp.Razor;

public class Author
{
    string name { get; set; }
    string email { get; set; }
    ICollection<Cheep> cheeps { get; set; }
}
