using PluginBase.Contract;
using TrumTruyenHtmlCrawler;

namespace plugin.tests.Crawler.ConcreteTest;

public class TrumTruyenCrawlerTest : CrawlerTestBase
{
    protected override ICrawler GetInstance()
    {
        return new TrumTruyenCrawler();
    }
}
