using Backend.Domain.Contract;
using PDFExporter;

namespace Backend.Tests.Exporter.ConcreteTest
{
    public class PDFTest : ExporterTestBase
    {
        protected override IExporter GetExporter()
        {
            return new PDFExport();
        }
    }
}
