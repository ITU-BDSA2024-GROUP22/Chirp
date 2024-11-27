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
    public Task<Bio> Bio { get; set; }
    public int CurrentPage { get; set; }
    [BindProperty]
    public string BioText { get; set; }

    public async Task<IActionResult> OnGet([FromQuery] int? page, string author)
    {
        if (string.IsNullOrEmpty(author))
        {
            return NotFound("Author parameter is missing");
        }

        Author =  _service.GetAuthorByName(author);

        Bio = _service.GetBioFromAuthor((await Author).UserName);
        BioText = (await Bio)?.Text ?? string.Empty;

        Console.WriteLine("bio in cshtml.cs: " + (await Bio)?.Text);

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

        if (string.IsNullOrEmpty(BioText))
        {
            return NotFound("Bio parameter is missing");
        }

        await _service.UpdateBio((await Author), BioText);

        return RedirectToPage("/AboutMe", new { author = (await Author).UserName, page = 1 });
    }

    public async Task<IActionResult> OnPostForget()
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

         await _service.DeleteAuthor((await Author));

         return RedirectToPage("/Public");
    }
}


