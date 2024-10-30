using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Core;

public class Cheep
{
    public required Author Author { get; set; }

    [Required]
    [StringLength(160)] // Work to do this differently later
    public required string Text { get; set; }
    public required DateTime TimeStamp { get; set; }
    public int AuthorId { get; set; }
    public int CheepId { get; set; }


}
