using System.ComponentModel.DataAnnotations;

namespace Chirp.Razor;

public class Author
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public ICollection<Cheep> Cheeps { get; set; } = new List<Cheep>();
    public int AuthorId { get; set; }
}
