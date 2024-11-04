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
        public async Task UserJourney()
        {
            await _page.GotoAsync("http://localhost:5273/");
            //await Expect(_page.GetByText("Confirm Form Resubmission This web page requires data that you entered earlier")).ToBeVisibleAsync();
            await RegisterUser();
            await LoginUser();
            await PostMessage();
            await LogoutUser();
        }

        private async Task RegisterUser()
        {
            await _page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
            await _page.GetByPlaceholder("name@example.com").FillAsync("brop@itu.dk");
            await _page.GetByLabel("Password", new() { Exact = true }).FillAsync("Letme111n!");
            await _page.GetByLabel("Confirm Password").FillAsync("Letme111n!"); //
            await _page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        }

        private async Task LoginUser()
        {
            await _page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
            await _page.GetByPlaceholder("name@example.com").FillAsync("brop@itu.dk");
            await _page.GetByPlaceholder("password").FillAsync("Letme111n!");
            await _page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        }

        private async Task PostMessage()
        {
            await _page.Locator("#Text").FillAsync("I am doing PlayWright tests!");
            await _page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        }

        private async Task LogoutUser()
        {
            await _page.GetByRole(AriaRole.Link, new() { Name = "Logout [brop@itu.dk]" }).ClickAsync();
            await _page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();
        }
    }
}
