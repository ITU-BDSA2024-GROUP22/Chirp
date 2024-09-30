using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    private const int PageSize = 32;
    public List<CheepViewModel> Cheeps { get; set; }

    public PublicModel(ICheepService service)
    {
        ICheepService _service = service;
    }
    
    
    public ActionResult OnGet([FromQuery] int? page)
    {
        int currentPage = page ?? 1; // Default to page 1 if no page parameter
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
