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
    //public string Email { get; set; }
    [BindProperty]
    public string Bio { get; set; }

    public async Task<IActionResult> OnGetUserDetails()
    {
        var authorName = User.Identity.Name;

        var author = await _service.GetAuthorByName(authorName);

        Username = author.UserName;
        Displayname = author.DisplayName;
        Bio = author.Bio;
        //Email = User.Identity.;

        if (!User.Identity.IsAuthenticated)
        {
            return NotFound();
        }

        await _service.CreateCheep(author, Text, DateTime.UtcNow);

        return RedirectToPage("/UserTimeline", new { author = author.UserName, page = 1 });
    }

    public async Task<IActionResult> OnGet([FromQuery] int? page, string author)
    {
        if (string.IsNullOrEmpty(author))
        {
            return NotFound("Author parameter is missing");
        }

        Author =  _service.GetAuthorByName(author);
        //Bio = _service.GetBioFromAuthor(await Author).ToString;

        CurrentPage = page ?? 1;
        Cheeps = _service.GetCheepsFromAuthor(author, CurrentPage);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return NotFound("User is not authenticated");
        }

        var author = User.Identity.Name;
        if (author == null)
        {
            return NotFound("author parameter is null");
        }

        Author = _service.GetAuthorByName(author);

        if (string.IsNullOrEmpty(Bio))
        {
            return NotFound("Bio parameter is missing");
        }

        (await Author).Bio = Bio;
        await _service.UpdateBio((await Author), Bio);

        return RedirectToPage("/AboutMe", new { author = (await Author).UserName, page = 1 });
    }
}


