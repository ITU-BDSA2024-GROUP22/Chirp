using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core.DTOs;
using Chirp.Infrastructure.Repositories;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly CheepService _service;

    public UserTimelineModel(CheepService service)
    {
        _service = service;
    }

    public required Task<List<CheepDTO>> Cheeps { get; set; }
    public required Task<AuthorDTO> Author { get; set; }
    public int CurrentPage { get; set; }
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
        if (!User.Identity.IsAuthenticated || string.IsNullOrWhiteSpace(Text))
        {
            return Page();
        }

        var author = await this.Author;

        var postingAuthorName = User.Identity.Name;
        var postingAuthor = await _service.GetAuthorByName(postingAuthorName);

        await _service.CreateCheep(author, Text, DateTime.UtcNow);

        return RedirectToPage("/UserTimeline", new { author = author.UserName, page = 1 });
    }
}
