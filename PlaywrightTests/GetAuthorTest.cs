/*
namespace PlaywrightTests;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Threading.Tasks;
{
    [TestFixture]
    public class GetAuthorTests : PageTest
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _page;

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
        }

        [TearDown]
        public async Task Cleanup()
        {
            await _browser.CloseAsync();
            _playwright.Dispose();
        }

        [Test]
        public async Task Should_DisplayAuthorDetails_When_AuthorExists()
        {
            await _page.GotoAsync("http://localhost:5273/authors");

            // Udfyld søgefelt med eksisterende forfatternavn
            await _page.GetByPlaceholder("Search for author...").FillAsync("John Doe");
            await _page.GetByRole(AriaRole.Button, new() { Name = "Search" }).ClickAsync();

            // Kontroller, at forfatternavn vises
            var authorDetails = await _page.Locator(".author-details").InnerTextAsync();
            Assert.That(authorDetails, Does.Contain("John Doe"));
        }

        [Test]
        public async Task Should_DisplayErrorMessage_When_AuthorDoesNotExist()
        {
            await _page.GotoAsync("http://localhost:5273/authors");

            // Udfyld søgefelt med et ikke-eksisterende forfatternavn
            await _page.GetByPlaceholder("Search for author...").FillAsync("Unknown Author");
            await _page.GetByRole(AriaRole.Button, new() { Name = "Search" }).ClickAsync();

            // Kontroller, at fejlmeddelelse vises
            var errorMessage = await _page.Locator(".error-message").InnerTextAsync();
            Assert.That(errorMessage, Does.Contain("No author with name Unknown Author was found."));
        }
    }
}

}
*/
