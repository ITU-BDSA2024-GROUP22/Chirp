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

    public ActionResult OnGet([FromQuery] int? page, string author)
    {

        this.Author = _service.GetAuthorByName(author);

        var currentPage = page ?? 1;
        Cheeps = _service.GetCheepsFromAuthor(author, currentPage);
        return Page();
    }
}
