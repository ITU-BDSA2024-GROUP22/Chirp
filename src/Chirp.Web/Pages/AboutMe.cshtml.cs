using Chirp.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;


public class AboutMeModel : PageModel
{
    private readonly CheepService _service;

    public AboutMeModel(CheepService service)
    {
        _service = service;
    }

    public required Task<List<CheepDTO>> Cheeps { get; set; }
    public required Task<AuthorDTO> Author { get; set; }
    public int CurrentPage { get; set; }
    [BindProperty]
    public string Text { get; set; }

    public ActionResult OnGet([FromQuery] int? page, string author)
    {
        this.Author = _service.GetAuthorByName(author);

        CurrentPage = page ?? 1;
        Cheeps = _service.GetCheepsFromAuthor(author, CurrentPage);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return NotFound();
        }

        var authorName = User.Identity.Name;

        var author = await _service.GetAuthorByName(authorName);

        await _service.CreateCheep(author, Text, DateTime.UtcNow);

        return RedirectToPage("/UserTimeline", new { author = author.UserName, page = 1 });
    }
}


