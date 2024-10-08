﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private const int PageSize = 32;
    private readonly CheepRepository _service;

    public PublicModel(CheepRepository service)
    {
        _service = service;
    }

    public Task<List<Cheep>> Cheeps { get; set; }


    public ActionResult OnGet([FromQuery] int? page)
    {
        var currentPage = page ?? 1; // Default to page 1 if no page parameter
        Cheeps = _service.GetCheeps(currentPage);
        return Page();
    }
}
