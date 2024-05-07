using PuppeteerSharp;

namespace HeadlessBrowser
{
    public class PuppeteerService
    {
        // todo: upgrade so there's only one instance of browser along the program

        public static async Task PerformHeadlessBrowser(Func<IBrowser, IPage, Task> browsingAction)
        {
            var browserFetcher = new BrowserFetcher(
                new BrowserFetcherOptions() { Path = $"{AppDomain.CurrentDomain.BaseDirectory}\\Chromium" }
                );
            // download the Chromium revision if it does not already exist in the (absolute) directory
            // take quite some time to download (500MB ~ 30 mins)
            var installedBrowser = await browserFetcher.DownloadAsync().ConfigureAwait(true);
            var launchOptions = new LaunchOptions
            {
                Headless = true, // = false for observing
                ExecutablePath = installedBrowser.GetExecutablePath(),
            };
            using var browser = await Puppeteer.LaunchAsync(launchOptions);
            using var page = await browser.NewPageAsync();
            await browsingAction.Invoke(browser, page);
        }
    }
}
