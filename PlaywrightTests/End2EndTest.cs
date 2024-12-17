using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace PlaywrightTests
{
    [TestFixture]
    public class EndToEndTests : PageTest
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _page;
        private string _randomEmail;
        private string _randomUser;

        [SetUp]
        public async Task Init()
        {
            _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 150,
            });
            var context = await _browser.NewContextAsync();
            _page = await context.NewPageAsync();
            _randomEmail = GenerateRandomEmail();
            _randomUser = GenerateRandomUsername();
        }

        [TearDown]
        public async Task Cleanup()
        {
            await _browser.CloseAsync();
            _playwright.Dispose();
        }

        [Test]
        public async Task UserJourney()
        {
            await _page.GotoAsync("http://localhost:5273/", new PageGotoOptions { Timeout = 60000 });
            await RegisterUser();
            await ConfirmRegistration();
            await LoginUser();
            await PostMessage();
            await NavigateTimelines();
            await UpdateProfile();
            await PostAnotherMessage();
            await ChangeProfilePicture();
            await LogoutUser();
        }

        private string GenerateRandomEmail()
        {
            var random = new Random();
            var randomNumber = random.Next(1000, 9999);
            return $"testuser{randomNumber}@example.com";
        }

        private string GenerateRandomUsername()
        {
            var random = new Random();
            var randomNumber = random.Next(1000, 9999);
            return $"testuser{randomNumber}";
        }

        private async Task RegisterUser()
        {
            await _page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
            await _page.GetByPlaceholder("Name", new() { Exact = true }).FillAsync(_randomUser);
            await _page.GetByPlaceholder("name@example.com").FillAsync(_randomEmail);
            await _page.GetByPlaceholder("Password", new() { Exact = true }).FillAsync("Letme111n!");
            await _page.GetByPlaceholder("Confirm Password").FillAsync("Letme111n!");
            await _page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        }

        private async Task ConfirmRegistration()
        {
            await _page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        }

        private async Task LoginUser()
        {
            await _page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
            await _page.GetByPlaceholder("Username").FillAsync(_randomUser);
            await _page.GetByPlaceholder("Password").FillAsync("Letme111n!");
            await _page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        }

        private async Task PostMessage()
        {
            await _page.GetByPlaceholder("Write your cheep here...").FillAsync("Yay for playwright!");
            await _page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        }

        private async Task NavigateTimelines()
        {
            await _page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
            await _page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        }

        private async Task UpdateProfile()
        {
            await _page.GetByRole(AriaRole.Link, new() { Name = "about me" }).ClickAsync();
            await _page.GetByLabel("Upload Profile Picture:").SetInputFilesAsync(new[] { "TestFiles/peach.png" });
            await _page.GetByRole(AriaRole.Button, new() { Name = "Upload" }).ClickAsync();
            await _page.GetByPlaceholder("Write your bio here...").FillAsync("It's PEACH time!");
            await _page.GetByRole(AriaRole.Button, new() { Name = "Save Bio" }).ClickAsync();
        }

        private async Task PostAnotherMessage()
        {
            await _page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
            await _page.GetByPlaceholder("Write your cheep here...").FillAsync("Another cheep!");
            await _page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

        }

        private async Task ChangeProfilePicture()
        {
            await _page.GetByRole(AriaRole.Link, new() { Name = "about me" }).ClickAsync();
            await _page.GetByLabel("Change Profile Picture:").SetInputFilesAsync(new[] { "TestFiles/luigi.png" });
            await _page.GetByRole(AriaRole.Button, new() { Name = "Upload" }).ClickAsync();
        }

        private async Task LogoutUser()
        {
            await _page.GetByRole(AriaRole.Link, new() { Name = $"logout [{_randomUser}]" }).ClickAsync();
            await _page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();
        }
    }
}
