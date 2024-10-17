using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core.DTOs;
using Chirp.Infrastructure.Repositories;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly CheepRepository _service;

    public UserTimelineModel(CheepRepository service)
    {
        _service = service;
    }

    public required Task<List<CheepDTO>> Cheeps { get; set; }

    public ActionResult OnGet([FromQuery] int? page, string author)
    {
        var currentPage = page ?? 1;
        Cheeps = _service.GetCheepsFromAuthor(currentPage, author);
        return Page();
    }
}
