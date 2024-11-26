using System.ComponentModel.DataAnnotations;

namespace Chirp.Core;

public class Bio
{
    public int BioId { get; set; }

    public string? AuthorId { get; set; }

    public required Author Author { get; set; }

    public required string Text {get; set;}
}
