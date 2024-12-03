using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core.DTOs;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly CheepService _service;
    private readonly FollowService _followService;

    public UserTimelineModel(CheepService service, FollowService followService)
    {
        _service = service;
        _followService = followService;
    }

    public required Task<List<CheepDTO>> Cheeps { get; set; }
    public required Task<AuthorDTO> Author { get; set; }
    public Task<Bio> Bio { get; set; }
    public int CurrentPage { get; set; }
    [BindProperty]
    public string Text { get; set; }

    public ActionResult OnGet([FromQuery] int? page, string author)
    {
        Author = _service.GetAuthorByName(author);
        Bio = _service.GetBioFromAuthor(author);
        CurrentPage = page ?? 1;


        if (User.Identity?.IsAuthenticated == true && User.Identity.Name == author)
        {
            Cheeps = _followService.GetCheepsFromFollowing(CurrentPage, author);
        }
        else
        {
            Cheeps = _service.GetCheepsFromAuthor(author, CurrentPage);
        }

        return Page();
    }


    public async Task<IActionResult> OnPostAsync()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return NotFound();
        }

        var authorName = User.Identity.Name;

        var author = await _service.GetAuthorByName(authorName!);

        await _service.CreateCheep(author, Text, DateTime.UtcNow);

        return RedirectToPage("/UserTimeline", new { author = author.UserName, page = 1 });
    }

    public async Task<IActionResult> OnPostUnfollow(string userToFollow)
    {
        var authorName = User.Identity.Name;
        var author = await _service.GetAuthorByName(authorName!);

        if (User.Identity != null && (!User.Identity.IsAuthenticated || string.IsNullOrWhiteSpace(Text)))
        {
            await _followService.UnfollowAuthor(authorName, userToFollow);

        }

        return RedirectToPage("/UserTimeline", new { author = author.UserName, page = 1 });
    }


}
