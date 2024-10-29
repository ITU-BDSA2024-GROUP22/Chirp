using Microsoft.AspNetCore.Identity;

namespace Chirp.Core;

public class Author : IdentityUser
{
    public ICollection<Cheep> Cheeps { get; set; } = new List<Cheep>();
    public int AuthorId { get; set; }
}
