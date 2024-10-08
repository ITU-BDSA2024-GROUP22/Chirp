namespace Chirp.Razor;

public class Author
{
    public string Name { get; set; }
    public string Email { get; set; }
    public ICollection<Cheep> Cheeps { get; set; }
    public int AuthorId { get; set; }
}
