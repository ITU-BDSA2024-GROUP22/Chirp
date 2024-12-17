namespace Chirp.Core;

/// <summary>
/// Represents the bio of an author in the Chirp application.
/// </summary>
public class Bio
{
    public int BioId { get; set; }

    public string? AuthorId { get; set; }

    public required Author Author { get; set; }

    public string? Text { get; set;}

}
