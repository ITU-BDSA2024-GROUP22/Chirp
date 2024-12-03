namespace Chirp.Core.DTOs;

public class AuthorDTO
{
    public static AuthorDTO fromAuthor(Author author)
    {
        return new AuthorDTO()
        {
            UserName = author.UserName,
            Email = author.Email,
            Picture = author.Picture
        };
    }
    public required string? UserName {get; set;}
    public required string? Email { get; set; }

    public List<Author> FollowingList { get; set; }
    public string? Picture { get; set; }
}
