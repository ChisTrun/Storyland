using Backend.Domain.Contract;
using ExporterEPUB;

namespace Backend.Tests.Exporter.ConcreteTest
{
    public class EPUBTest : ExporterTestBase
    {
        protected override IExporter GetExporter()
        {
            return new EPUBExport();
        }
    }
}
