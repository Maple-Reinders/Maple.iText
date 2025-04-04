/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Pdfa.Exceptions;
using iText.Test;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfA2AcroFormCheckTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String cmpFolder = sourceFolder + "cmp/PdfA2AcroFormCheckTest/";

        public static readonly String destinationFolder = TestUtil.GetOutputPath() + "/pdfa/PdfA2AcroFormCheckTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void AcroFormCheck01() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_2B, new PdfOutputIntent("Custom", "", "http://www.color.org"
                , "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfDictionary acroForm = new PdfDictionary();
            acroForm.Put(PdfName.NeedAppearances, new PdfBoolean(true));
            doc.GetCatalog().Put(PdfName.AcroForm, acroForm);
            try {
                doc.Close();
                NUnit.Framework.Assert.Fail("PdfAConformanceException expected");
            }
            catch (PdfAConformanceException) {
            }
        }

        [NUnit.Framework.Test]
        public virtual void AcroFormCheck02() {
            String outPdf = destinationFolder + "pdfA2b_acroFormCheck02.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA2b_acroFormCheck02.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_2B, new PdfOutputIntent("Custom", "", "http://www.color.org"
                , "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfDictionary acroForm = new PdfDictionary();
            acroForm.Put(PdfName.NeedAppearances, new PdfBoolean(false));
            doc.GetCatalog().Put(PdfName.AcroForm, acroForm);
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void AcroFormCheck03() {
            String outPdf = destinationFolder + "pdfA2b_acroFormCheck03.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA2b_acroFormCheck03.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_2B, new PdfOutputIntent("Custom", "", "http://www.color.org"
                , "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfDictionary acroForm = new PdfDictionary();
            doc.GetCatalog().Put(PdfName.AcroForm, acroForm);
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void AcroFormCheck04() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_2B, new PdfOutputIntent("Custom", "", "http://www.color.org"
                , "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfDictionary acroForm = new PdfDictionary();
            acroForm.Put(PdfName.XFA, new PdfArray());
            doc.GetCatalog().Put(PdfName.AcroForm, acroForm);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.THE_INTERACTIVE_FORM_DICTIONARY_SHALL_NOT_CONTAIN_THE_XFA_KEY
                , e.Message);
        }

        private void CompareResult(String outPdf, String cmpPdf) {
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }
    }
}
