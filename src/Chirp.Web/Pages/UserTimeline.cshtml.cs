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
    public int CheepsCount { get; private set; }

    /// <summary>
    /// Handles the GET request for displaying the timeline of a specific author.
    /// Depending on whether the current user is the author or a follower, it fetches the appropriate Cheeps
    /// </summary>
    /// <param name="page">Optional query parameter for the current page of Cheeps to display</param>
    /// <param name="author">The username of the author whose timeline is being viewed</param>
    /// <returns>The current page of the user's timeline with Cheeps, author info, and bio</returns>
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

        CheepsCount = (Cheeps?.Result?.Count ?? 0);

        return Page();
    }

    /// <summary>
    /// Handles the POST request for creating a new Cheep.
    /// If the user is authenticated, a Cheep is created and the page is redirected to the updated timeline
    /// </summary>
    /// <returns>A redirect to the user's timeline page after posting the new Cheep</returns>
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

    /// <summary>
    /// Handles the POST request for unfollowing another user.
    /// If the user is authenticated and the provided username is valid, the user will be unfollowed
    /// </summary>
    /// <param name="userToFollow">The username of the user to unfollow</param>
    /// <returns>A redirect to the user's timeline after the unfollow action</returns>
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
