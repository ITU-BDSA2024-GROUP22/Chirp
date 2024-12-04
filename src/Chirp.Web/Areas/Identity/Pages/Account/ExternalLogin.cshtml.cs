// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Chirp.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Chirp.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<Author> _signInManager;
        private readonly UserManager<Author> _userManager;
        private readonly IUserStore<Author> _userStore;
        private readonly IUserEmailStore<Author> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<Author> signInManager,
            UserManager<Author> userManager,
            IUserStore<Author> userStore,
            ILogger<ExternalLoginModel> logger,
            IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _logger = logger;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ProviderDisplayName { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }


            public string UserName { get; set; }
        }

        public IActionResult OnGet() => RedirectToPage("./Login");

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                _logger.LogError("Remote error during external login: {RemoteError}", remoteError);
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                _logger.LogError("External login info is null.");
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Check if the user exists with this external login
            var existingUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (existingUser != null)
            {
                await _signInManager.SignInAsync(existingUser, isPersistent: false);
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.",
                    existingUser.UserName, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }

            // If no external login exists, proceed with user creation
            return await OnPostConfirmationAsync(returnUrl);
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
{
    returnUrl = returnUrl ?? Url.Content("~/");

    var info = await _signInManager.GetExternalLoginInfoAsync();
    if (info == null)
    {
        ErrorMessage = "Error loading external login information during confirmation.";
        _logger.LogError("External login info is null during confirmation.");
        return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
    }

    try
    {
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var username = info.Principal.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username))
        {
            _logger.LogWarning("Missing email or username during confirmation. Email: {Email}, Username: {Username}",
                email, username);
            ErrorMessage = "Could not retrieve required user information.";
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        }

        // Check if a user with this email already exists
        var existingUser = await _userManager.FindByNameAsync(username);
        if (existingUser != null)
        {
            // Attach external login to the existing user
            var loginResult = await _userManager.AddLoginAsync(existingUser, info);
            if (loginResult.Succeeded)
            {
                await _signInManager.SignInAsync(existingUser, isPersistent: false);
                _logger.LogInformation("Attached external login to existing user: {Email}", email);
                return LocalRedirect(returnUrl);
            }

            foreach (var error in loginResult.Errors)
            {
                _logger.LogError("Error attaching external login: {Error}", error.Description);
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }

        // Create a new user
        var user = CreateUser();
        username = await GenerateUniqueUsername(username);
        await _userStore.SetUserNameAsync(user, username, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, email, CancellationToken.None);

        var result = await _userManager.CreateAsync(user);
        if (result.Succeeded)
        {
            result = await _userManager.AddLoginAsync(user, info);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }
        }

        foreach (var error in result.Errors)
        {
            _logger.LogError("Error during user creation: {Error}", error.Description);
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while confirming external login.");
        ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
    }

    return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
}



        private Author CreateUser()
        {
            try
            {
                return Activator.CreateInstance<Author>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(Author)}'. " +
                                                    $"Ensure that '{nameof(Author)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                                                    $"override the external login page in /Areas/Identity/Pages/Account/ExternalLogin.cshtml");
            }
        }

        private async Task<string> GenerateUniqueUsername(string baseUsername)
        {
            string uniqueUsername = baseUsername;
            int counter = 1;

            while (await _userManager.FindByNameAsync(uniqueUsername) != null)
            {
                uniqueUsername = $"{baseUsername}{counter}";
                counter++;
            }

            return uniqueUsername;
        }


        private IUserEmailStore<Author> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }

            return (IUserEmailStore<Author>)_userStore;
        }
    }
}
