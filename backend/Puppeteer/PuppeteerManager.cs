using PuppeteerSharp;

namespace PuppeteerM
{
    public class PuppeteerManager
    {
        private PuppeteerManager()
        {

        }

        public static async Task<PuppeteerManager> BuildInstance()
        {
            // download the Chromium revision if it does not already exist
            // take quite some time to download (500MB)
            await new BrowserFetcher().DownloadAsync().ConfigureAwait(true);
            var launchOptions = new LaunchOptions
            {
                Headless = true, // = false for testing
            };
            using (var browser = await Puppeteer.LaunchAsync(launchOptions))
            using (var page = await browser.NewPageAsync())
            {
                // visit the target page
                await page.GoToAsync("https://scrapingclub.com/exercise/list_infinite_scroll/");
                // retrieve the HTML source code and log it
                var html = await page.GetContentAsync();
                Console.WriteLine(html);
            }
            return new PuppeteerManager();
        }
    }
}
