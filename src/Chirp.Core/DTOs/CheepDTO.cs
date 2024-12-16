namespace Chirp.Core.DTOs;

/// <summary>
/// Data transfer object (DTO) for representing a Cheep.
/// This class is used for transferring Cheep data between layers in the application.
/// </summary>
public class CheepDTO
{
    /// <summary>
    /// Gets or sets the author of the Cheep.
    /// The <see cref="AuthorDTO"/> contains information about the author of the Cheep.
    /// </summary>
    public required AuthorDTO Author {get; set;}

    /// <summary>
    /// Gets or sets the text content of the Cheep.
    /// The <see cref="Text"/> is a required property representing the message or content of the Cheep.
    /// </summary>
    public required string Text {get; set;}

    /// <summary>
    /// Gets or sets the timestamp of when the Cheep was created.
    /// The <see cref="TimeStamp"/> is a required property and should be in a string format (e.g., "MM/dd/yy HH:mm:ss").
    /// </summary>
    public required string TimeStamp {get; set;}

    /// <summary>
    /// Gets the picture URL of the author of the Cheep.
    /// This property retrieves the <see cref="Author.Picture"/> for displaying the author's profile picture.
    /// If no picture is set, this will return null.
    /// </summary>
    public string? Picture => Author.Picture;
}
