using System.ComponentModel.DataAnnotations;

namespace Chirp.Core;

public class Cheep
{
    public required Author Author { get; set; }

    [Required]
    [StringLength(160)]
    public required string Text { get; set; }
    public required DateTime TimeStamp { get; set; }
    public int AuthorId { get; set; }
    public int CheepId { get; set; }


}
