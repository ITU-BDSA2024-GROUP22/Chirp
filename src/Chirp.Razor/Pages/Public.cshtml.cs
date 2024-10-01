using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private const int PageSize = 32;
    private readonly ICheepService _service;

    public PublicModel(ICheepService service)
    {
        _service = service;
    }

    public List<Cheep> Cheeps { get; set; }


    public ActionResult OnGet([FromQuery] int? page)
    {
        var currentPage = page ?? 1; // Default to page 1 if no page parameter
        Cheeps = _service.GetCheeps(currentPage);
        return Page();
    }


    /*
    public ActionResult OnGet()
    {
        Cheeps = _service.GetCheeps();
        return Page();
    }
    */
}