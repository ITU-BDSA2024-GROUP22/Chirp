using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Core;

public class Author : IdentityUser
{
    public ICollection<Cheep> Cheeps { get; set; } = new List<Cheep>();
    //public int AuthorId { get; set; }
    //Author now uses the build-in IdentityUser's Id property

    //[Required]
    [StringLength(160)] public string? DisplayName { get; set; }
    public string? Bio {get; set;}
}
