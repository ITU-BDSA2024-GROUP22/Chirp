using System.ComponentModel.DataAnnotations;

namespace Chirp.Core;

public class Cheep
{
    public required Author Author { get; set; }

    [Required]
    [StringLength(160, ErrorMessage = "Cheeps cannot be longer than 160 characters.")]
    public required string Text { get; set; }
    public required DateTime TimeStamp { get; set; }
    public string? AuthorId { get; set; }
    public int CheepId { get; set; }


}
