namespace Chirp.Core.DTOs;

public class CheepDTO
{
    public required AuthorDTO Author {get; set;}
    public required string Text {get; set;}
    public required string TimeStamp {get; set;}
    public string? Picture => Author.Picture;
}
