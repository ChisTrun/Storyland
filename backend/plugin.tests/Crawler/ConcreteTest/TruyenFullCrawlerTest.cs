using PluginBase.Contract;
using TruyenFullPlugin;

namespace plugin.tests.Crawler.ConcreteTest;

public class TruyenFullCrawlerTest : CrawlerTestBase
{
    protected override ICrawler GetInstance()
    {
        return new TruyenFullCommand();
    }
}
