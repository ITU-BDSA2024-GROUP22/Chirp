using Microsoft.AspNetCore.Identity;

namespace Chirp.Core;

public class Author : IdentityUser<int>
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public ICollection<Cheep> Cheeps { get; set; } = new List<Cheep>();
    public int AuthorId { get; set; }
}
