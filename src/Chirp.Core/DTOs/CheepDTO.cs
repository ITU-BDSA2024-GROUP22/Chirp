namespace Chirp.Core.DTOs;

/// <summary>
/// Data transfer object (DTO) for representing a Cheep.
/// This class is used for transferring Cheep data between layers in the application.
/// </summary>
public class CheepDTO
{
    public required AuthorDTO Author {get; set;}

    public required string Text {get; set;}

    public required string TimeStamp {get; set;}

    public string? Picture => Author.Picture;
}
