namespace Chirp.Razor;

public class Author
{
    public string name { get; set; }
    public string email { get; set; }
    public ICollection<Cheep> cheeps { get; set; }
}
