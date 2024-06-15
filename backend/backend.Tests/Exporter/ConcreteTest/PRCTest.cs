using Backend.Domain.Contract;
using PRCExporter;

namespace Backend.Tests.Exporter.ConcreteTest
{
    public class PRCTest : ExporterTestBase
    {
        protected override IExporter GetExporter() => new PRCExport();
    }
}
