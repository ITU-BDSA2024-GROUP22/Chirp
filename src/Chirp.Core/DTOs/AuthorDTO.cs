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

    /// <summary>
    /// Gets or sets the username of the author.
    /// The <see cref="UserName"/> is required and must be unique for each author.
    /// </summary>
    public required string? UserName {get; set;}

    /// <summary>
    /// Gets or sets the email address of the author.
    /// The <see cref="Email"/> is required and represents the author's contact email.
    /// </summary>
    public required string? Email { get; set; }

    /// <summary>
    /// Gets or sets the list of authors that this author is following.
    /// This list may contain other <see cref="Author"/> objects.
    /// </summary>
    public List<Author> FollowingList { get; set; }

    /// <summary>
    /// Gets or sets the profile picture URL of the author.
    /// This property is optional and may be null if no picture is set.
    /// </summary>
    public string? Picture { get; set; }
}
