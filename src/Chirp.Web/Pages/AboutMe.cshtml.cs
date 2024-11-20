using Chirp.Core;
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
    public string Username { get; set; }
    public string Displayname { get; set; }
    public string Email { get; set; }
    public string Bio { get; set; }

    public async Task<IActionResult> OnGetUserDetails()
    {
        var authorName = User.Identity.Name;

        var author = await _service.GetAuthorByName(authorName);

        Username = author.UserName;
        Displayname = author.DisplayName;
        //Email = User.Identity.;

        if (!User.Identity.IsAuthenticated)
        {
            return NotFound();
        }

        await _service.CreateCheep(author, Text, DateTime.UtcNow);

        return RedirectToPage("/UserTimeline", new { author = author.UserName, page = 1 });
    }

    public ActionResult OnGet([FromQuery] int? page, string author)
    {
        this.Author = _service.GetAuthorByName(author);

        CurrentPage = page ?? 1;
        Cheeps = _service.GetCheepsFromAuthor(author, CurrentPage);
        return Page();
    }
}


