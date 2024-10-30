using Chirp.Core.DTOs;
using Chirp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private const int PageSize = 32;
    private readonly CheepService _service;

    public PublicModel(CheepService service)
    {
        _service = service;
    }

    public required Task<List<CheepDTO>> Cheeps { get; set; }

    [BindProperty]
    public string Text { get; set; }

    public ActionResult OnGet([FromQuery] int? page)
    {
        var currentPage = page ?? 1; // Default to page 1 if no page parameter
        Cheeps = _service.GetCheeps(currentPage);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!User.Identity.IsAuthenticated || string.IsNullOrWhiteSpace(Text))
        {
            return Page();
        }

        var authorName = User.Identity.Name;
        var author = await _service.GetAuthorByName(authorName);

        await _service.CreateCheep(author, Text, DateTime.UtcNow);

        return RedirectToPage(new { page = 1 });
    }
}
