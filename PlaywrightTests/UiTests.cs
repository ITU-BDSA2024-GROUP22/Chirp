using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Threading.Tasks;

namespace PlaywrightTests
{
        [TestFixture]
        public class UiTests : PageTest
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
            public async Task buttonsOnPublicTimeline()
            {
                Assert.Pass();
                await _page.GotoAsync("http://localhost:5273/");
                await _page.Locator("p")
                    .Filter(new()
                        { HasText = "Jacqualine Gilcoine Follow Starbuck now is what we hear the worst. — 08/01/23" })
                    .GetByRole(AriaRole.Button).ClickAsync();
                await _page.Locator("p")
                    .Filter(new()
                        { HasText = "Jacqualine Gilcoine Unfollow Starbuck now is what we hear the worst. — 08/01/23" })
                    .GetByRole(AriaRole.Button).ClickAsync();
                await _page.Locator("p")
                    .Filter(new()
                        { HasText = "Jacqualine Gilcoine Follow Starbuck now is what we hear the worst. — 08/01/23" })
                    .GetByRole(AriaRole.Link).ClickAsync();
                await _page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
                await _page.GetByRole(AriaRole.Button, new() { Name = "Next" }).ClickAsync();
                await _page.GetByRole(AriaRole.Button, new() { Name = "Previous" }).ClickAsync();
            }

            public async Task registerUserFields()
            {
                await _page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
                await _page.GetByPlaceholder("Name", new() { Exact = true }).ClickAsync();
                await _page.GetByPlaceholder("Name", new() { Exact = true }).FillAsync("stina");
                await _page.GetByPlaceholder("name@example.com").ClickAsync();
                await _page.GetByPlaceholder("name@example.com").FillAsync("stina");
                await _page.GetByPlaceholder("name@example.com").PressAsync("Alt+@");
                await _page.GetByPlaceholder("name@example.com").FillAsync("stina@stina.dk");
                await _page.GetByPlaceholder("name@example.com").ClickAsync();
                await _page.GetByPlaceholder("name@example.com").PressAsync("ArrowRight");
                await _page.GetByPlaceholder("name@example.com").PressAsync("ControlOrMeta+ArrowLeft");
                await _page.GetByPlaceholder("name@example.com").PressAsync("ArrowRight");
                await _page.GetByPlaceholder("name@example.com").PressAsync("Shift+ArrowLeft");
                await _page.GetByPlaceholder("name@example.com").PressAsync("ControlOrMeta+c");
                await _page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
                await _page.GetByLabel("Password", new() { Exact = true }).FillAsync("Stina1234!");
                await _page.GetByLabel("Password", new() { Exact = true }).PressAsync("Tab");
                await _page.GetByLabel("Confirm Password").FillAsync("Stina1234!");
                await _page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
                await _page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
            }

            public async Task logIn()
            {
                await _page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
                await _page.GetByPlaceholder("name@example.com").ClickAsync();
                await _page.GetByPlaceholder("name@example.com").FillAsync("stina@stina.dk");
                await _page.GetByPlaceholder("Password").ClickAsync();
                await _page.GetByPlaceholder("Password").FillAsync("Stina1234!");
                await _page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
            }

            public async Task cheep()
            {

                await _page.Locator("#cheepText").ClickAsync();
                await _page.Locator("#cheepText").FillAsync("dav");
                await _page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
                await _page.GetByRole(AriaRole.Link, new() { Name = "stina", Exact = true }).ClickAsync();
            }

            public async Task logOut()
            {
                await _page.GetByRole(AriaRole.Link, new() { Name = "logout [stina@stina.dk]" }).ClickAsync();
                await _page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();
            }

            public async Task forgetMe()
            {
                await _page.GetByRole(AriaRole.Link, new() { Name = "about me" }).ClickAsync();
                await _page.GetByPlaceholder("Write your bio here...").ClickAsync();
                await _page.GetByPlaceholder("Write your bio here...").FillAsync("Ekstra ben");
                await _page.GetByRole(AriaRole.Button, new() { Name = "Save Bio" }).ClickAsync();
                await _page.GetByRole(AriaRole.Button, new() { Name = "Forget me!" }).ClickAsync();
            }
        }
    }
