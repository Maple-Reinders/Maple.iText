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
using iText.Forms.Fields;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FlatteningRotatedTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/FlatteningRotatedTest/";

        public static readonly String destinationFolder = TestUtil.GetOutputPath() + "/forms/FlatteningRotatedTest/";

        public static ICollection<Object[]> InputFileNames() {
            IList<Object[]> inputFileNames = new List<Object[]>();
            for (int pageRot = 0; pageRot < 360; pageRot += 90) {
                for (int fieldRot = 0; fieldRot < 360; fieldRot += 90) {
                    inputFileNames.Add(new Object[] { "FormFlatteningDefaultAppearance_" + pageRot + "_" + fieldRot });
                }
            }
            return inputFileNames;
        }

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.TestCaseSource("InputFileNames")]
        public virtual void FormFlatteningTest_DefaultAppearanceGeneration_Rot(String inputPdfFileName) {
            String src = sourceFolder + inputPdfFileName + ".pdf";
            String dest = destinationFolder + inputPdfFileName + ".pdf";
            String dest_flattened = destinationFolder + inputPdfFileName + "_flattened.pdf";
            String cmp = sourceFolder + "cmp_" + inputPdfFileName + ".pdf";
            String cmp_flattened = sourceFolder + "cmp_" + inputPdfFileName + "_flattened.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfReader(src), new PdfWriter(dest))) {
                PdfAcroForm form = PdfFormCreator.GetAcroForm(doc, true);
                foreach (PdfFormField field in form.GetAllFormFields().Values) {
                    field.SetValue("Long Long Text");
                    field.GetFirstFormAnnotation().SetBorderWidth(1);
                    field.GetFirstFormAnnotation().SetBorderColor(ColorConstants.BLUE);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, destinationFolder, "diff_"));
            using (PdfDocument doc_1 = new PdfDocument(new PdfReader(dest), new PdfWriter(dest_flattened))) {
                PdfFormCreator.GetAcroForm(doc_1, true).FlattenFields();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest_flattened, cmp_flattened, destinationFolder
                , "diff_"));
        }
    }
}
