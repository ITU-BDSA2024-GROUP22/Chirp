namespace Chirp.Core.DTOs;

public class AuthorDTO
{
    public static AuthorDTO fromAuthor(Author author)
    {
        return new AuthorDTO()
        {
            DisplayName = string.IsNullOrWhiteSpace(author.DisplayName)
                ? author.UserName ?? "Anonymous"
                : author.DisplayName,
            UserName = author.UserName
        };
    }

    public required string? DisplayName {get; set;}
    public required string? UserName {get; set;}
    public string? Bio {get; set;}
}
