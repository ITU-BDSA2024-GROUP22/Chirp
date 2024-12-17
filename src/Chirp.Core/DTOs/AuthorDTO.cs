namespace Chirp.Core.DTOs;
/// <summary>
/// Data transfer object (DTO) for representing an author.
/// This class is used for transferring author data between layers in the application.
/// </summary>
public class AuthorDTO
{
    /// <summary>
    /// Creates an <see cref="AuthorDTO"/> from an <see cref="Author"/>.
    /// This method maps the properties of an <see cref="Author"/> object to an <see cref="AuthorDTO"/>.
    /// </summary>
    /// <param name="author">The <see cref="Author"/> object to convert into a DTO.</param>
    /// <returns>An instance of <see cref="AuthorDTO"/> populated with data from the given <see cref="Author"/>.</returns>
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
