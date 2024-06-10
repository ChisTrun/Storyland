using backend.Domain.Contract;
using plugin.tests.Crawler;
using TruyenFullPlugin;

namespace PluginBase.Tests.Crawler.ConcreteTest;

public class TruyenFullCrawlerTest : CrawlerTestBase
{
    protected override ICrawler GetInstance()
    {
        return new TruyenFullCommand();
    }
}
