using System.ComponentModel.DataAnnotations;

namespace Chirp.Razor;

public class Cheep
{
    public Author Author { get; set; }

    [Required]
    [StringLength(160)]
    public required string Text { get; set; }
    public DateTime TimeStamp { get; set; }
    public int AuthorId { get; set; }
    public int CheepId { get; set; }


}
