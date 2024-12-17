using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Core;

/// <summary>
/// Represents an author, that can send cheeps, have a profile picture and bio and
/// follow other authors.
/// Author inherits from AspNetCore's Identity IdentityUser
/// </summary>
public class Author : IdentityUser
{
    public ICollection<Cheep> Cheeps { get; set; } = new List<Cheep>();

    [Required]
    public required List<Author> FollowingList { get; set; }  = new List<Author>();

    public Bio? Bio { get; set; }
    public string? Picture { get; set; }

}
