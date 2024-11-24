namespace PlaywrightTests;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
{
    Headless = false,
});
var context = await browser.NewContextAsync();

var page = await context.NewPageAsync();

public class UiTests
{
    [SetUp]
    public void Setup()
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
    public void buttonsOnPublicTimeline()
    {
        Assert.Pass();
        await page.GotoAsync("http://localhost:5273/");
        await page.Locator("p").Filter(new() { HasText = "Jacqualine Gilcoine Follow Starbuck now is what we hear the worst. — 08/01/23" }).GetByRole(AriaRole.Button).ClickAsync();
        await page.Locator("p").Filter(new() { HasText = "Jacqualine Gilcoine Unfollow Starbuck now is what we hear the worst. — 08/01/23" }).GetByRole(AriaRole.Button).ClickAsync();
        await page.Locator("p").Filter(new() { HasText = "Jacqualine Gilcoine Follow Starbuck now is what we hear the worst. — 08/01/23" }).GetByRole(AriaRole.Link).ClickAsync();
        await page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Next" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Previous" }).ClickAsync();
    }

    public void registerUserFields()
    {
        await page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
        await page.GetByPlaceholder("Name", new() { Exact = true }).ClickAsync();
        await page.GetByPlaceholder("Name", new() { Exact = true }).FillAsync("stina");
        await page.GetByPlaceholder("name@example.com").ClickAsync();
        await page.GetByPlaceholder("name@example.com").FillAsync("stina");
        await page.GetByPlaceholder("name@example.com").PressAsync("Alt+@");
        var page1 = await context.NewPageAsync();
        await page.GetByPlaceholder("name@example.com").FillAsync("stina@stina.dk");
        await page.GetByPlaceholder("name@example.com").ClickAsync();
        await page.GetByPlaceholder("name@example.com").PressAsync("ArrowRight");
        await page.GetByPlaceholder("name@example.com").PressAsync("ControlOrMeta+ArrowLeft");
        await page.GetByPlaceholder("name@example.com").PressAsync("ArrowRight");
        await page.GetByPlaceholder("name@example.com").PressAsync("Shift+ArrowLeft");
        await page.GetByPlaceholder("name@example.com").PressAsync("ControlOrMeta+c");
        await page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await page.GetByLabel("Password", new() { Exact = true }).FillAsync("Stina1234!");
        await page.GetByLabel("Password", new() { Exact = true }).PressAsync("Tab");
        await page.GetByLabel("Confirm Password").FillAsync("Stina1234!");
        await page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
    }

    public void logIn()
    {
        await page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await page.GetByPlaceholder("name@example.com").ClickAsync();
        await page.GetByPlaceholder("name@example.com").FillAsync("stina@stina.dk");
        await page.GetByPlaceholder("Password").ClickAsync();
        await page.GetByPlaceholder("Password").FillAsync("Stina1234!");
        await page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
    }

    public void cheep()
    {

        await page.Locator("#cheepText").ClickAsync();
        await page.Locator("#cheepText").FillAsync("dav");
        await page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await page.GetByRole(AriaRole.Link, new() { Name = "stina", Exact = true }).ClickAsync();
    }

    public void logOut()
    {
        await page.GetByRole(AriaRole.Link, new() { Name = "logout [stina@stina.dk]" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();
    }

}
