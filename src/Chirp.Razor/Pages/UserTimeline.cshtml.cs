using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;

    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }

    public List<Cheep> Cheeps { get; set; }

    public ActionResult OnGet([FromQuery] int? page, string author)
    {
        var currentPage = page ?? 1;
        Cheeps = _service.GetCheepsFromAuthor(author, currentPage);
        return Page();
    }
}
