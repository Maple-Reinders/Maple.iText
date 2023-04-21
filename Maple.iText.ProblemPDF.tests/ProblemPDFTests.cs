using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace iText.ProblemPDF.tests
{
    [TestClass]
    public class ProblemPDFTests
    {
        [TestMethod]
        public void TestProblemPDFs()
        {
            // Arrange - Use hardcoded path used by agent:
            DirectoryInfo d = new DirectoryInfo("D:\\a\\1\\s\\Maple.iText\\ProblemPDFs");

            FileInfo[] Files = d.GetFiles("*.pdf");

            var hitFile = false;

            foreach (FileInfo file in Files)
            {
                hitFile = true;

                // Act
                var fileStream = file.OpenRead();
                iText.Kernel.Pdf.PdfDocument Doc = new iText.Kernel.Pdf.PdfDocument(new iText.Kernel.Pdf.PdfReader(fileStream).SetUnethicalReading(true));
                var numPages = Doc.GetNumberOfPages();

                // Assert
                Assert.IsTrue(numPages > 0);
            }

            // Sanity check to make sure PDF files are processed
            Assert.IsTrue(hitFile);
        }
    }
}
