// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Chirp.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Handles external login functionality, including managing the external provider's authentication,
    /// user account creation, and linking the external account to the application user.
    /// </summary>
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
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        /// <summary>
        /// Handles the callback from the external provider after the user has authenticated.
        /// </summary>
        /// <param name="returnUrl">The URL to redirect to after completing the callback.</param>
        /// <param name="remoteError">Any error returned by the external provider.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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
            var existingUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (existingUser != null)
            {
                await _signInManager.SignInAsync(existingUser, isPersistent: false);
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.",
                    existingUser.UserName, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            return await OnPostConfirmationAsync(returnUrl);
        }

        /// <summary>
        /// Handles user confirmation after authentication with the external provider.
        /// Creates a new user account if none exists with the respective Github claims.
        /// </summary>
        /// <param name="returnUrl">The URL to redirect to after completing confirmation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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

            if (ModelState.IsValid)
            {
                try
                {
                    var user = CreateUser();

                    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                    var username = info.Principal.FindFirstValue(ClaimTypes.Name);

                    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
                    {
                        _logger.LogWarning(
                            "Missing email or username during confirmation. Email: {Email}, Username: {Username}",
                            email, username);
                        ModelState.AddModelError(string.Empty,
                            "Could not retrieve required user information from external provider.");
                        return Page();
                    }

                    var existingUser = await _userManager.FindByNameAsync(username);
                    if (existingUser != null)
                    {
                        username = await GenerateUniqueUsername(username);
                    }

                    await _userStore.SetUserNameAsync(user, username, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, email, CancellationToken.None);

                    var result = await _userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        result = await _userManager.AddLoginAsync(user, info);
                        if (result.Succeeded)
                        {
                            _logger.LogInformation("User created an account using {Name} provider.",
                                info.LoginProvider);

                            await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
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
            }

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;
            return Page();
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

        /// <summary>
        /// /// Generates a unique username by appending a number to the base username if it already exists.
        /// It checks if the username is taken and, if so, increments a counter until an available username is found.
        /// </summary>
        /// <param name="baseUsername">The base username to start with.</param>
        /// <returns>A unique username that is not already taken.</returns>
        private async Task<string> GenerateUniqueUsername(string baseUsername)
        {
            var uniqueUsername = baseUsername;
            var counter = 1;

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
