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
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Test;

namespace iText.Kernel.Utils {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TaggedPdfReaderToolTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/utils/TaggedPdfReaderToolTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/kernel/utils/TaggedPdfReaderToolTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void TaggedPdfReaderToolTest01() {
            String filename = "iphone_user_guide.pdf";
            String outXmlPath = DESTINATION_FOLDER + "outXml01.xml";
            String cmpXmlPath = SOURCE_FOLDER + "cmpXml01.xml";
            PdfReader reader = new PdfReader(SOURCE_FOLDER + filename);
            using (Stream outXml = FileUtil.GetFileOutputStream(outXmlPath)) {
                using (PdfDocument document = new PdfDocument(reader)) {
                    TaggedPdfReaderTool tool = new TaggedPdfReaderTool(document);
                    tool.SetRootTag("root");
                    tool.ConvertToXml(outXml);
                }
            }
            CompareTool compareTool = new CompareTool();
            if (!compareTool.CompareXmls(outXmlPath, cmpXmlPath)) {
                NUnit.Framework.Assert.Fail("Resultant xml is different.");
            }
        }

        [NUnit.Framework.Test]
        public virtual void NoStructTreeRootInDocTest() {
            String outXmlPath = DESTINATION_FOLDER + "noStructTreeRootInDoc.xml";
            try {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
                TaggedPdfReaderTool tool = new TaggedPdfReaderTool(pdfDocument);
                using (Stream outXml = FileUtil.GetFileOutputStream(outXmlPath)) {
                    Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => tool.ConvertToXml(outXml, "UTF-8"
                        ));
                    NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DOCUMENT_DOES_NOT_CONTAIN_STRUCT_TREE_ROOT, 
                        exception.Message);
                }
            }
            catch (System.IO.IOException) {
                NUnit.Framework.Assert.Fail("IOException is not expected to be triggered");
            }
        }

        [NUnit.Framework.Test]
        public virtual void CyclicReferencesTest() {
            String outXmlPath = DESTINATION_FOLDER + "cyclicReferences.xml";
            String cmpXmlPath = SOURCE_FOLDER + "cmp_cyclicReferences.xml";
            PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()));
            doc.SetTagged();
            PdfStructElem kid1 = new PdfStructElem(doc, PdfStructTreeRoot.ConvertRoleToPdfName(StandardRoles.P));
            PdfStructElem kid2 = new PdfStructElem(doc, PdfStructTreeRoot.ConvertRoleToPdfName(StandardRoles.DIV));
            doc.GetStructTreeRoot().AddKid(kid1);
            doc.GetStructTreeRoot().AddKid(kid2);
            kid1.AddKid(kid2);
            kid2.AddKid(kid1);
            TaggedPdfReaderTool tool = new TaggedPdfReaderTool(doc);
            using (Stream outXml = FileUtil.GetFileOutputStream(outXmlPath)) {
                tool.ConvertToXml(outXml, "UTF-8");
            }
            CompareTool compareTool = new CompareTool();
            if (!compareTool.CompareXmls(outXmlPath, cmpXmlPath)) {
                NUnit.Framework.Assert.Fail("Resultant xml is different.");
            }
        }
    }
}
