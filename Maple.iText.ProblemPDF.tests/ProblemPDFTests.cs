using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace iText.ProblemPDF.tests
{
    [TestClass]
    public class ProblemPDFTests
    {
        [TestMethod]
        public void TestProblemPDFs()
        {
            // Arrange
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            string strWorkPath = GetParent(strExeFilePath, "ProblemPDFs");
            Console.WriteLine($"Path: {strWorkPath}");
            DirectoryInfo d = new DirectoryInfo($"{strWorkPath}");

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

        private string GetParent(string path, string parentName)
        {
            var dir = new DirectoryInfo(path);

            if (dir.Parent == null)
            {
                return null;
            }

            if (dir.Parent.Name == parentName)
            {
                return dir.Parent.FullName;
            }

            return GetParent(dir.Parent.FullName, parentName);
        }
    }
}
