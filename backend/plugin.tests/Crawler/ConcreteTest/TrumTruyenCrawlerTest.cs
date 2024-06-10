using backend.Domain.Contract;
using plugin.tests.Crawler;
using TrumTruyenHtmlCrawler;

namespace PluginBase.Tests.Crawler.ConcreteTest;

public class TrumTruyenCrawlerTest : CrawlerTestBase
{
    protected override ICrawler GetInstance()
    {
        return new TrumTruyenCrawler();
    }
}
