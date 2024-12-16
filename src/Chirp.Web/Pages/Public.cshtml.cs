using Chirp.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly CheepService _service;
    private readonly FollowService _followService;

    public PublicModel(CheepService service, FollowService followService)
    {
        _service = service;
        _followService = followService;
    }

    public required Task<List<CheepDTO>> Cheeps { get; set; }
    public required Task<AuthorDTO> Author { get; set; }

    [BindProperty]
    public required string Text { get; set; }
    public int CurrentPage { get; set; }
    public int CheepsCount { get; private set; }

    /// <summary>
    /// Handles the GET request for displaying the public timeline of Cheeps.
    /// It fetches Cheeps from the service and author details if the user is authenticated.
    /// </summary>
    /// <param name="page">The current page for pagination, defaulting to 1 if not provided.</param>
    /// <returns>The current page of public Cheeps along with author information if the user is authenticated.</returns>
    public ActionResult OnGet([FromQuery] int? page)
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            if (User.Identity.Name != null) this.Author = _service.GetAuthorByName(User.Identity.Name);
        }

        CurrentPage = page ?? 1; // Default to page 1 if no page parameter
        Cheeps = _service.GetCheeps(CurrentPage);

        CheepsCount = (Cheeps?.Result?.Count ?? 0);

        return Page();
    }

    // <summary>
    /// Handles the POST request for creating a new Cheep.
    /// It ensures the user is authenticated and the Text field is not empty before creating the Cheep.
    /// </summary>
    /// <returns>A redirect to the public page after successfully posting the Cheep.</returns>
    public async Task<IActionResult> OnPostAsync()
    {
        if (User.Identity != null && (!User.Identity.IsAuthenticated || string.IsNullOrWhiteSpace(Text)))
        {
            return Page();
        }

        if (User.Identity != null)
        {
            var authorName = User.Identity.Name;
            if (authorName != null)
            {
                var author = await _service.GetAuthorByName(authorName);
                await _service.CreateCheep(author, Text, DateTime.UtcNow);
            }
        }

        return RedirectToPage("/Public", new { page = 1 });
    }

    /// <summary>
    /// Handles the POST request for following a user.
    /// It ensures the user is authenticated before performing the follow action.
    /// </summary>
    /// <param name="userToFollow">The username of the user to follow.</param>
    /// <returns>A redirect to the public page after the follow action is performed.</returns>
    public async Task<IActionResult> OnPostFollow(string userToFollow)
    {
        if (User.Identity != null && (!User.Identity.IsAuthenticated || string.IsNullOrWhiteSpace(Text)))
        {
            var author = User.Identity.Name;
            Console.WriteLine("Fra OnPostFollow method + " + userToFollow);
            await _followService.FollowAuthor(author, userToFollow);
        }

        return RedirectToPage("/Public", new { page = 1 });
    }

    public async Task<IActionResult> OnPostUnfollow(string userToFollow)
    {
        if (User.Identity != null && (!User.Identity.IsAuthenticated || string.IsNullOrWhiteSpace(Text)))
        {
            var author = User.Identity.Name;
            await _followService.UnfollowAuthor(author, userToFollow);
        }

        return RedirectToPage("/Public", new { page = 1 });
    }
}
