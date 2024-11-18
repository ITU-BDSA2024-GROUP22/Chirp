using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
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

        [SetUp]
        public async Task Init()
        {
            _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
            });
            var context = await _browser.NewContextAsync();
            _page = await context.NewPageAsync();
			_randomEmail = GenerateRandomEmail();
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
            await _page.GotoAsync("http://localhost:5273/");
            await RegisterUser();
            await ConfirmRegistration();
            await LoginUser();
            await PostMessage();
            await NavigateTimelines();
            await LogoutUser();
        }

		private string GenerateRandomEmail()
		{
    	var random = new Random();
    	var randomNumber = random.Next(1000, 9999); // Generates a random number between 1000 and 9999
    	return $"testuser{randomNumber}@example.com";
		}

        private async Task RegisterUser()
        {
            await _page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
            await _page.GetByPlaceholder("Name", new() { Exact = true }).FillAsync("OleBropp");
            await _page.GetByPlaceholder("name@example.com").FillAsync("bole");
            await _page.GetByPlaceholder("name@example.com").PressAsync("Alt+@");
            await _page.GetByPlaceholder("name@example.com").FillAsync(_randomEmail);
            await _page.GetByLabel("Password", new() { Exact = true }).FillAsync("Letme111n!");
            await _page.GetByLabel("Confirm Password").FillAsync("Letme111n!");
            await _page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        }

        private async Task ConfirmRegistration()
        {
            await _page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        }

        private async Task LoginUser()
        {
            await _page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
            await _page.GetByPlaceholder("name@example.com").FillAsync(_randomEmail);
            await _page.GetByPlaceholder("Password").FillAsync("Letme111n!");
            await _page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        }

        private async Task PostMessage()
        {
            await _page.Locator("#cheepText").ClickAsync();
            await _page.Locator("#cheepText").FillAsync("Yay for playwright!!");
            await _page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        }

        private async Task NavigateTimelines()
        {
            await _page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
            await _page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
            await _page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        }

        private async Task LogoutUser()
        {

            await _page.GetByRole(AriaRole.Link, new() { Name = $"Logout [{_randomEmail}]" }).ClickAsync();
            await _page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();
        }
    }
}
