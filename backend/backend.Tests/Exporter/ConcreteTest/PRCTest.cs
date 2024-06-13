using backend.Domain.Contract;
using PRCExporter;

namespace backend.Tests.Exporter.ConcreteTest
{
    public class PRCTest : ExporterTestBase
    {
        protected override IExporter GetExporter()
        {
            return new PRCExport();
        }
    }
}
