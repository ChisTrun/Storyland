using backend.Domain.Contract;
using PDFExporter;

namespace backend.Tests.Exporter.ConcreteTest
{
    public class PDFTest : ExporterTestBase
    {
        protected override IExporter GetExporter() => new PDFExport();
    }
}
