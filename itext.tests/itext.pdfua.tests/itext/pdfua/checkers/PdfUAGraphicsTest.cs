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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Pdfua;
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Exceptions;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfua.Checkers {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUAGraphicsTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/PdfUAGraphicsTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/PdfUAGraphicsTest/";

        private static readonly String DOG = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/img/DOG.bmp";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private UaValidationTestFramework framework;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void InitializeFramework() {
            framework = new UaValidationTestFramework(DESTINATION_FOLDER);
        }

        public static IList<PdfUAConformance> Data() {
            return JavaUtil.ArraysAsList(PdfUAConformance.PDF_UA_1, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageWithoutAlternativeDescription_ThrowsInLayout(PdfUAConformance pdfUAConformance) {
            PdfDocument pdfDoc = pdfUAConformance == PdfUAConformance.PDF_UA_1 ? (PdfDocument)new PdfUATestPdfDocument
                (new PdfWriter(new MemoryStream())) : (PdfDocument)new PdfUA2TestPdfDocument(new PdfWriter(new MemoryStream
                ()));
            Document document = new Document(pdfDoc);
            iText.Layout.Element.Image img = new Image(ImageDataFactory.Create(DOG));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                document.Add(img);
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LayoutCheckUtilTest() {
            NUnit.Framework.Assert.DoesNotThrow(() => new LayoutCheckUtil(null).CheckRenderer(null));
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageWithEmptyAlternativeDescription_ThrowsInLayout(PdfUAConformance pdfUAConformance) {
            PdfDocument pdfDoc = pdfUAConformance == PdfUAConformance.PDF_UA_1 ? (PdfDocument)new PdfUATestPdfDocument
                (new PdfWriter(new MemoryStream())) : (PdfDocument)new PdfUA2TestPdfDocument(new PdfWriter(new MemoryStream
                ()));
            Document document = new Document(pdfDoc);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            img.GetAccessibilityProperties().SetAlternateDescription("");
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                document.Add(img);
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT, e.Message);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageCustomRole_Ok(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDocument) => {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
                    pdfDocument.GetTagStructureContext().SetDocumentDefaultNamespace(@namespace);
                    pdfDocument.GetStructTreeRoot().AddNamespace(@namespace);
                    @namespace.AddNamespaceRoleMapping("CustomImage", StandardRoles.FIGURE);
                }
                PdfStructTreeRoot root = pdfDocument.GetStructTreeRoot();
                root.AddRoleMapping("CustomImage", StandardRoles.FIGURE);
            }
            );
            framework.AddSuppliers(new _Generator_148());
            framework.AssertBothValid("imageWithCustomRoleOk", pdfUAConformance);
        }

        private sealed class _Generator_148 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_148() {
            }

            public IBlockElement Generate() {
                iText.Layout.Element.Image img = null;
                try {
                    img = new iText.Layout.Element.Image(ImageDataFactory.Create(PdfUAGraphicsTest.DOG));
                }
                catch (UriFormatException) {
                    throw new Exception();
                }
                img.GetAccessibilityProperties().SetRole("CustomImage");
                img.GetAccessibilityProperties().SetAlternateDescription("ff");
                return new Div().Add(img);
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageCustomDoubleMapping_Ok(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDocument) => {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
                    pdfDocument.GetTagStructureContext().SetDocumentDefaultNamespace(@namespace);
                    pdfDocument.GetStructTreeRoot().AddNamespace(@namespace);
                    @namespace.AddNamespaceRoleMapping("CustomImage", StandardRoles.FIGURE);
                    @namespace.AddNamespaceRoleMapping("CustomImage2", "CustomImage");
                }
                PdfStructTreeRoot root = pdfDocument.GetStructTreeRoot();
                root.AddRoleMapping("CustomImage", StandardRoles.FIGURE);
                root.AddRoleMapping("CustomImage2", "CustomImage");
            }
            );
            framework.AddSuppliers(new _Generator_180());
            framework.AssertBothValid("imageWithDoubleMapping", pdfUAConformance);
        }

        private sealed class _Generator_180 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_180() {
            }

            public IBlockElement Generate() {
                iText.Layout.Element.Image img = null;
                try {
                    img = new iText.Layout.Element.Image(ImageDataFactory.Create(PdfUAGraphicsTest.DOG));
                }
                catch (UriFormatException) {
                    throw new Exception();
                }
                img.GetAccessibilityProperties().SetRole("CustomImage2");
                img.GetAccessibilityProperties().SetAlternateDescription("ff");
                return new Div().Add(img);
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageCustomRoleNoAlternateDescription_Throws(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDocument) => {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
                    pdfDocument.GetTagStructureContext().SetDocumentDefaultNamespace(@namespace);
                    pdfDocument.GetStructTreeRoot().AddNamespace(@namespace);
                    @namespace.AddNamespaceRoleMapping("CustomImage", StandardRoles.FIGURE);
                }
                PdfStructTreeRoot root = pdfDocument.GetStructTreeRoot();
                root.AddRoleMapping("CustomImage", StandardRoles.FIGURE);
            }
            );
            framework.AddSuppliers(new _Generator_210());
            framework.AssertBothFail("imageWithCustomRoleAndNoDescription", pdfUAConformance);
        }

        private sealed class _Generator_210 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_210() {
            }

            public IBlockElement Generate() {
                iText.Layout.Element.Image img = null;
                try {
                    img = new iText.Layout.Element.Image(ImageDataFactory.Create(PdfUAGraphicsTest.DOG));
                }
                catch (UriFormatException) {
                    throw new Exception();
                }
                img.GetAccessibilityProperties().SetRole("CustomImage");
                return new Div().Add(img);
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageCustomDoubleMapping_Throws(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDocument) => {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
                    pdfDocument.GetTagStructureContext().SetDocumentDefaultNamespace(@namespace);
                    pdfDocument.GetStructTreeRoot().AddNamespace(@namespace);
                    @namespace.AddNamespaceRoleMapping("CustomImage", StandardRoles.FIGURE);
                    @namespace.AddNamespaceRoleMapping("CustomImage2", "CustomImage");
                }
                PdfStructTreeRoot root = pdfDocument.GetStructTreeRoot();
                root.AddRoleMapping("CustomImage", StandardRoles.FIGURE);
                root.AddRoleMapping("CustomImage2", "CustomImage");
            }
            );
            framework.AddSuppliers(new _Generator_242());
            framework.AssertBothFail("imageCustomDoubleMapping_Throws", pdfUAConformance);
        }

        private sealed class _Generator_242 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_242() {
            }

            public IBlockElement Generate() {
                iText.Layout.Element.Image img = null;
                try {
                    img = new iText.Layout.Element.Image(ImageDataFactory.Create(PdfUAGraphicsTest.DOG));
                }
                catch (UriFormatException) {
                    throw new Exception();
                }
                img.GetAccessibilityProperties().SetRole("CustomImage2");
                return new Div().Add(img);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithValidAlternativeDescription_OK() {
            String OUTPUT_FILE = DESTINATION_FOLDER + "imageWithValidAlternativeDescription_OK.pdf";
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(OUTPUT_FILE));
            Document document = new Document(pdfDoc);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            img.GetAccessibilityProperties().SetAlternateDescription("Alternative description");
            document.Add(img);
            document.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(OUTPUT_FILE));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(OUTPUT_FILE, SOURCE_FOLDER + "cmp_imageWithValidAlternativeDescription_OK.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithValidActualText_OK() {
            String OUTPUT_FILE = DESTINATION_FOLDER + "imageWithValidActualText_OK.pdf";
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(OUTPUT_FILE));
            Document document = new Document(pdfDoc);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            img.GetAccessibilityProperties().SetActualText("Actual text");
            document.Add(img);
            document.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(OUTPUT_FILE));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(OUTPUT_FILE, SOURCE_FOLDER + "cmp_imageWithValidActualText_OK.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithCaption_OK() {
            String OUTPUT_FILE = DESTINATION_FOLDER + "imageWithCaption_OK.pdf";
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(OUTPUT_FILE));
            Document document = new Document(pdfDoc);
            Div imgWithCaption = new Div();
            imgWithCaption.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
            imgWithCaption.GetAccessibilityProperties().SetAlternateDescription("Alternative description");
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            img.SetNeutralRole();
            Paragraph caption = new Paragraph("Caption");
            caption.SetFont(PdfFontFactory.CreateFont(FONT));
            caption.GetAccessibilityProperties().SetRole(StandardRoles.CAPTION);
            imgWithCaption.Add(img);
            imgWithCaption.Add(caption);
            document.Add(imgWithCaption);
            document.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(OUTPUT_FILE));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(OUTPUT_FILE, SOURCE_FOLDER + "cmp_imageWithCaption_OK.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageWithCaptionWithoutAlternateDescription_Throws(PdfUAConformance pdfUAConformance) {
            PdfDocument pdfDoc = pdfUAConformance == PdfUAConformance.PDF_UA_1 ? (PdfDocument)new PdfUATestPdfDocument
                (new PdfWriter(new MemoryStream())) : (PdfDocument)new PdfUA2TestPdfDocument(new PdfWriter(new MemoryStream
                ()));
            Document document = new Document(pdfDoc);
            Div imgWithCaption = new Div();
            imgWithCaption.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            img.SetNeutralRole();
            Paragraph caption = new Paragraph("Caption");
            caption.SetFont(PdfFontFactory.CreateFont(FONT));
            caption.GetAccessibilityProperties().SetRole(StandardRoles.CAPTION);
            imgWithCaption.Add(img);
            imgWithCaption.Add(caption);
            // will not throw in layout but will throw on close this is expected
            document.Add(imgWithCaption);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                document.Close();
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT, e.Message);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageWithoutActualText_ThrowsInLayout(PdfUAConformance pdfUAConformance) {
            PdfDocument pdfDoc = pdfUAConformance == PdfUAConformance.PDF_UA_1 ? (PdfDocument)new PdfUATestPdfDocument
                (new PdfWriter(new MemoryStream())) : (PdfDocument)new PdfUA2TestPdfDocument(new PdfWriter(new MemoryStream
                ()));
            Document document = new Document(pdfDoc);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            img.GetAccessibilityProperties().SetActualText(null);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                document.Add(img);
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT, e.Message);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageWithEmptyActualText_ThrowsInLayout(PdfUAConformance pdfUAConformance) {
            PdfDocument pdfDoc = pdfUAConformance == PdfUAConformance.PDF_UA_1 ? (PdfDocument)new PdfUATestPdfDocument
                (new PdfWriter(new MemoryStream())) : (PdfDocument)new PdfUA2TestPdfDocument(new PdfWriter(new MemoryStream
                ()));
            Document document = new Document(pdfDoc);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            img.GetAccessibilityProperties().SetActualText("");
            NUnit.Framework.Assert.DoesNotThrow(() => document.Add(img));
        }

        [NUnit.Framework.Test]
        public virtual void ImageDirectlyOnCanvas_OK() {
            String OUTPUT_FILE = DESTINATION_FOLDER + "imageDirectlyOnCanvas_OK.pdf";
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(OUTPUT_FILE));
            Document document = new Document(pdfDoc);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            img.GetAccessibilityProperties().SetAlternateDescription("Hello");
            document.Add(img);
            iText.Layout.Element.Image img2 = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            img2.GetAccessibilityProperties().SetActualText("Some actual text on layout img");
            document.Add(img2);
            TagTreePointer pointerForImage = new TagTreePointer(pdfDoc);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            pointerForImage.SetPageForTagging(page);
            TagTreePointer tmp = pointerForImage.AddTag(StandardRoles.FIGURE);
            tmp.GetProperties().SetActualText("Some text");
            canvas.OpenTag(tmp.GetTagReference());
            canvas.AddImageAt(ImageDataFactory.Create(DOG), 400, 400, false);
            canvas.CloseTag();
            TagTreePointer ttp = pointerForImage.AddTag(StandardRoles.FIGURE);
            ttp.GetProperties().SetAlternateDescription("Alternate description");
            canvas.OpenTag(ttp.GetTagReference());
            canvas.AddImageAt(ImageDataFactory.Create(DOG), 200, 200, false);
            canvas.CloseTag();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(OUTPUT_FILE));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            new CompareTool().CompareByContent(OUTPUT_FILE, SOURCE_FOLDER + "cmp_imageDirectlyOnCanvas_OK.pdf", DESTINATION_FOLDER
                , "diff_");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageDirectlyOnCanvasWithoutAlternateDescription_ThrowsOnClose(PdfUAConformance pdfUAConformance
            ) {
            PdfDocument pdfDoc = pdfUAConformance == PdfUAConformance.PDF_UA_1 ? (PdfDocument)new PdfUATestPdfDocument
                (new PdfWriter(new MemoryStream())) : (PdfDocument)new PdfUA2TestPdfDocument(new PdfWriter(new MemoryStream
                ()));
            TagTreePointer pointerForImage = new TagTreePointer(pdfDoc);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            pointerForImage.SetPageForTagging(page);
            TagTreePointer tmp = pointerForImage.AddTag(StandardRoles.FIGURE);
            canvas.OpenTag(tmp.GetTagReference());
            canvas.AddImageAt(ImageDataFactory.Create(DOG), 200, 200, false);
            canvas.CloseTag();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                pdfDoc.Close();
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT, e.Message);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageDirectlyOnCanvasWithEmptyActualText_OK(PdfUAConformance pdfUAConformance) {
            PdfDocument pdfDoc = pdfUAConformance == PdfUAConformance.PDF_UA_1 ? (PdfDocument)new PdfUATestPdfDocument
                (new PdfWriter(new MemoryStream())) : (PdfDocument)new PdfUA2TestPdfDocument(new PdfWriter(new MemoryStream
                ()));
            TagTreePointer pointerForImage = new TagTreePointer(pdfDoc);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            pointerForImage.SetPageForTagging(page);
            TagTreePointer tmp = pointerForImage.AddTag(StandardRoles.FIGURE);
            tmp.GetProperties().SetActualText("");
            canvas.OpenTag(tmp.GetTagReference());
            canvas.AddImageAt(ImageDataFactory.Create(DOG), 200, 200, false);
            canvas.CloseTag();
            NUnit.Framework.Assert.DoesNotThrow(() => pdfDoc.Close());
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestOverflowImage(PdfUAConformance pdfUAConformance) {
            PdfDocument pdfDoc = pdfUAConformance == PdfUAConformance.PDF_UA_1 ? (PdfDocument)new PdfUATestPdfDocument
                (new PdfWriter(new MemoryStream())) : (PdfDocument)new PdfUA2TestPdfDocument(new PdfWriter(new MemoryStream
                ()));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            Document document = new Document(pdfDoc);
            document.Add(new Div().SetHeight(730).SetBackgroundColor(ColorConstants.CYAN));
            NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                document.Add(img);
            }
            );
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestEmbeddedImageInTable(PdfUAConformance pdfUAConformance) {
            PdfDocument pdfDoc = pdfUAConformance == PdfUAConformance.PDF_UA_1 ? (PdfDocument)new PdfUATestPdfDocument
                (new PdfWriter(new MemoryStream())) : (PdfDocument)new PdfUA2TestPdfDocument(new PdfWriter(new MemoryStream
                ()));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            Document document = new Document(pdfDoc);
            Table table = new Table(2);
            for (int i = 0; i <= 20; i++) {
                table.AddCell(new Paragraph("Cell " + i));
            }
            table.AddCell(img);
            NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                document.Add(table);
            }
            );
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestEmbeddedImageInDiv(PdfUAConformance pdfUAConformance) {
            PdfDocument pdfDoc = pdfUAConformance == PdfUAConformance.PDF_UA_1 ? (PdfDocument)new PdfUATestPdfDocument
                (new PdfWriter(new MemoryStream())) : (PdfDocument)new PdfUA2TestPdfDocument(new PdfWriter(new MemoryStream
                ()));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            Document document = new Document(pdfDoc);
            Div div = new Div();
            div.Add(img);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                document.Add(div);
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT, e.Message);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestEmbeddedImageInParagraph(PdfUAConformance pdfUAConformance) {
            PdfDocument pdfDoc = pdfUAConformance == PdfUAConformance.PDF_UA_1 ? (PdfDocument)new PdfUATestPdfDocument
                (new PdfWriter(new MemoryStream())) : (PdfDocument)new PdfUA2TestPdfDocument(new PdfWriter(new MemoryStream
                ()));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            Document document = new Document(pdfDoc);
            Paragraph paragraph = new Paragraph();
            paragraph.Add(img);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                document.Add(paragraph);
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT, e.Message);
        }
    }
}
