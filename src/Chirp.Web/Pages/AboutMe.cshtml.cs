using Chirp.Core;
using Chirp.Core.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;


public class AboutMeModel : PageModel
{
    private readonly CheepService _service;

    public AboutMeModel(CheepService service)
    {
        _service = service;
    }

    public required Task<List<CheepDTO>> Cheeps { get; set; }
    public required Task<AuthorDTO> Author { get; set; }
    public Task<Bio> Bio { get; set; }
    public int CurrentPage { get; set; }
    [BindProperty]
    public string BioText { get; set; }
    public int CheepsCount { get; private set; }

    /// <summary>
    /// Handles GET requests to the page, loading data about an author and their Cheeps
    /// </summary>
    /// <param name="page">The current page number for Cheeps pagination. Optional</param>
    /// <param name="author">The name of the author whose data should be retrieved</param>
    /// <returns>
    /// An IActionResult, which can be a NotFound response if the `author` parameter is missing,
    /// or a `Page()` view with the loaded data
    /// </returns>
    public async Task<IActionResult> OnGet([FromQuery] int? page, string author)
    {
        if (string.IsNullOrEmpty(author))
        {
            return NotFound("Author parameter is missing");
        }

        Author =  _service.GetAuthorByName(author);

        Bio = _service.GetBioFromAuthor((await Author).UserName);
        BioText = (await Bio)?.Text ?? string.Empty;

        CurrentPage = page ?? 1;
        Cheeps = _service.GetCheepsFromAuthor(author, CurrentPage);

        CheepsCount = (Cheeps?.Result?.Count ?? 0);

        return Page();
    }

    /// <summary>
    /// Handles POST requests to update the user's bio
    /// </summary>
    /// <returns>
    /// A RedirectToPage action to "AboutMe" after successfully updating the bio,
    /// or a NotFound response if the user is not authenticated
    /// </returns>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return NotFound("User is not authenticated");
        }

        var author = User.Identity.Name;
        if (author == null)
        {
            return NotFound("author parameter is null");
        }

        Author = _service.GetAuthorByName(author);

        await _service.UpdateBio((await Author), BioText);

        return RedirectToPage("/AboutMe", new { author = (await Author).UserName, page = 1 });
    }

    /// <summary>
    /// Handles POST requests to "forget" the user's profile.
    /// Deletes user data, including their profile picture, logs the user out,
    /// and redirects to a public page
    /// </summary>
    /// <returns>
    /// A RedirectToPage action to a public page after successful deletion,
    /// or a NotFound response if the user is not authenticated
    /// </returns>
    public async Task<IActionResult> OnPostForget()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return NotFound("User is not authenticated");
        }

        var author = User.Identity.Name;

        if (author == null)
        {
            return NotFound("author parameter is null");
        }

        Author = _service.GetAuthorByName(author);


        if (Author == null)
        {
            return NotFound($"No author with the name '{author}' was found.");
        }

        var author_picture = ((await Author).Picture);

        if (!string.IsNullOrEmpty(author_picture))
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", author_picture.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        var signInManager = HttpContext.RequestServices.GetService(typeof(SignInManager<Author>)) as SignInManager<Author>;
        if (signInManager != null)
        {
            await signInManager.SignOutAsync();
        }
        else
        {
            return StatusCode(500, "Sign-in manager service not available.");
        }

        // Proceed to delete the author's data
        await _service.DeleteAuthor(await Author);

        // Redirect to a public or confirmation page
        return RedirectToPage("/Public");
    }

    /// <summary>
    /// Handles POST requests to update the user's profile picture.
    /// Saves the new picture to the "wwwroot/uploads" directory and updates the database with the new URL
    /// </summary>
    /// <param name="profilePicture">The uploaded file for the profile picture</param>
    /// <returns>
    /// A RedirectToPage action to "AboutMe" with the updated profile picture,
    /// or a validation error if the file is invalid
    /// </returns>
    public async Task<IActionResult> OnPostPictureAsync(IFormFile profilePicture)
    {
        if (!User.Identity.IsAuthenticated || User.Identity.Name == null)
        {
            return NotFound("User is not authenticated");
        }
        var author = User.Identity.Name;
        Author = _service.GetAuthorByName(author);
        var author_picture = ((await Author).Picture);

        if (!string.IsNullOrEmpty(author_picture))
        {
            var previousPicturePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", author_picture.TrimStart('/'));
            if (System.IO.File.Exists(previousPicturePath))
            {
                try
                {
                    System.IO.File.Delete(previousPicturePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting file: {ex.Message}");
                }
            }
        }


        if (profilePicture == null || profilePicture.Length == 0)
        {
            ModelState.AddModelError("ProfilePicture", "Please upload a valid picture.");
            return Page();
        }

        // Save the uploaded file to "wwwroot/uploads"
        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(uploadFolder);

        var fileName = $"{Guid.NewGuid()}_{profilePicture.FileName}";
        var filePath = Path.Combine(uploadFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await profilePicture.CopyToAsync(stream);
        }

        // Generate the relative URL for the uploaded file
        var pictureUrl = $"/uploads/{fileName}";

        // Update the author's profile picture in the database
        await _service.SetAuthorPictureAsync(User.Identity.Name, pictureUrl);

        return RedirectToPage("/AboutMe", new { author = User.Identity.Name });
    }
}


