using backend.Domain.Contract;
using ExporterEPUB;

namespace backend.Tests.Exporter.ConcreteTest
{
    public class EPUBTest : ExporterTestBase
    {
        protected override IExporter GetExporter()
        {
            return new EPUBExport();
        }
    }
}
