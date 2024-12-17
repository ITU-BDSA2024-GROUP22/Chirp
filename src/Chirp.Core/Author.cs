using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Core;

/// <summary>
/// represents an author, that cansend cheeps, have a profile picture and bio and
/// follow other authors
/// </summary>
public class Author : IdentityUser
{
    public ICollection<Cheep> Cheeps { get; set; } = new List<Cheep>();

    [Required]
    public required List<Author> FollowingList { get; set; }  = new List<Author>();

    public Bio? Bio { get; set; }
    public string? Picture { get; set; }

}
