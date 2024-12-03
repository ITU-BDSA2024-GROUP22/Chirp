using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Core;

public class Author : IdentityUser
{
    public ICollection<Cheep> Cheeps { get; set; } = new List<Cheep>();
    //public int AuthorId { get; set; }
    //Author now uses the build-in IdentityUser's Id property

    //[Required]
    //[StringLength(160)] public string? DisplayName { get; set; }

    [Required]
    public required List<Author> FollowingList { get; set; }  = new List<Author>();

    public Bio? Bio { get; set; }
    public string? Picture { get; set; }


}
