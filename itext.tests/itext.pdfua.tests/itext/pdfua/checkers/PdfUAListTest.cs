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
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUAListTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/PdfUAListTest/";

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

        public static IList<PdfUAConformance> TestSources() {
            return JavaUtil.ArraysAsList(PdfUAConformance.PDF_UA_1, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void ValidListTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_78());
            framework.AssertBothValid("validListTest", pdfUAConformance);
        }

        private sealed class _Generator_78 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_78() {
            }

            public IBlockElement Generate() {
                List list = new List(ListNumberingType.DECIMAL);
                list.Add(new ListItem("item1"));
                list.Add(new ListItem("item2"));
                list.Add(new ListItem("item3"));
                list.SetFont(PdfUAListTest.LoadFont());
                return list;
            }
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void LblAndLBodyInListItemTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_96());
            framework.AssertBothValid("lblAndLBodyInListItemTest", pdfUAConformance);
        }

        private sealed class _Generator_96 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_96() {
            }

            public IBlockElement Generate() {
                Div list = new Div();
                list.GetAccessibilityProperties().SetRole(StandardRoles.L);
                Div item = new Div();
                item.GetAccessibilityProperties().SetRole(StandardRoles.LI);
                Paragraph lbl = new Paragraph("label");
                lbl.GetAccessibilityProperties().SetRole(StandardRoles.LBL);
                Paragraph lBody = new Paragraph("body");
                lBody.GetAccessibilityProperties().SetRole(StandardRoles.LBODY);
                item.Add(lbl);
                item.Add(lBody);
                list.Add(item);
                list.SetFont(PdfUAListTest.LoadFont());
                return list;
            }
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void InvalidListItemRoleTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_125());
            framework.AssertBothFail("invalidListItemRoleTest", PdfUAExceptionMessageConstants.LIST_ITEM_CONTENT_HAS_INVALID_TAG
                , pdfUAConformance);
        }

        private sealed class _Generator_125 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_125() {
            }

            public IBlockElement Generate() {
                List list = new List(ListNumberingType.DECIMAL);
                ListItem item1 = new ListItem("item1");
                item1.GetAccessibilityProperties().SetRole(StandardRoles.P);
                list.Add(item1);
                list.Add(new ListItem("item2"));
                list.Add(new ListItem("item3"));
                list.SetFont(PdfUAListTest.LoadFont());
                return list;
            }
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void ArtifactInListItemTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_146());
            framework.AssertBothValid("artifactInListItemTest", pdfUAConformance);
        }

        private sealed class _Generator_146 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_146() {
            }

            public IBlockElement Generate() {
                List list = new List(ListNumberingType.DECIMAL);
                ListItem item1 = new ListItem("item1");
                item1.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                list.Add(item1);
                list.Add(new ListItem("item2"));
                list.Add(new ListItem("item3"));
                list.SetFont(PdfUAListTest.LoadFont());
                return list;
            }
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void NoListNumberingTest(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                pdfDoc.GetTagStructureContext().NormalizeDocumentRootTag();
                PdfStructElem list = pdfUAConformance == PdfUAConformance.PDF_UA_1 ? pdfDoc.GetStructTreeRoot().AddKid(new 
                    PdfStructElem(pdfDoc, PdfName.L)) : ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0]).AddKid(new 
                    PdfStructElem(pdfDoc, PdfName.L));
                PdfStructElem listItem1 = list.AddKid(new PdfStructElem(pdfDoc, PdfName.LI));
                listItem1.AddKid(new PdfStructElem(pdfDoc, PdfName.Lbl));
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("noListNumberingTest", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("noListNumberingTest", PdfUAExceptionMessageConstants.LIST_NUMBERING_IS_NOT_SPECIFIED
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void NoneListNumberingTest(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                pdfDoc.GetTagStructureContext().NormalizeDocumentRootTag();
                PdfStructElem list = pdfUAConformance == PdfUAConformance.PDF_UA_1 ? pdfDoc.GetStructTreeRoot().AddKid(new 
                    PdfStructElem(pdfDoc, PdfName.L)) : ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0]).AddKid(new 
                    PdfStructElem(pdfDoc, PdfName.L));
                PdfDictionary attributes = new PdfDictionary();
                attributes.Put(PdfName.O, PdfName.List);
                attributes.Put(PdfName.ListNumbering, PdfName.None);
                list.SetAttributes(attributes);
                PdfStructElem listItem1 = list.AddKid(new PdfStructElem(pdfDoc, PdfName.LI));
                listItem1.AddKid(new PdfStructElem(pdfDoc, PdfName.Lbl));
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("noneListNumberingTest", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("noneListNumberingTest", PdfUAExceptionMessageConstants.LIST_NUMBERING_IS_NOT_SPECIFIED
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void NoListNumberingNoLblTest(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                pdfDoc.GetTagStructureContext().NormalizeDocumentRootTag();
                PdfStructElem list = pdfUAConformance == PdfUAConformance.PDF_UA_1 ? pdfDoc.GetStructTreeRoot().AddKid(new 
                    PdfStructElem(pdfDoc, PdfName.L)) : ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0]).AddKid(new 
                    PdfStructElem(pdfDoc, PdfName.L));
                PdfStructElem listItem1 = list.AddKid(new PdfStructElem(pdfDoc, PdfName.LI));
                listItem1.AddKid(new PdfStructElem(pdfDoc, PdfName.LBody));
            }
            );
            framework.AssertBothValid("noListNumberingNoLblTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void InvalidNestedListTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_230());
            framework.AssertBothFail("invalidNestedListTest", PdfUAExceptionMessageConstants.LIST_ITEM_CONTENT_HAS_INVALID_TAG
                , pdfUAConformance);
        }

        private sealed class _Generator_230 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_230() {
            }

            public IBlockElement Generate() {
                Div list = new Div();
                list.GetAccessibilityProperties().SetRole(StandardRoles.L);
                PdfDictionary attributes = new PdfDictionary();
                attributes.Put(PdfName.O, PdfName.List);
                attributes.Put(PdfName.ListNumbering, PdfName.Unordered);
                list.GetAccessibilityProperties().AddAttributes(new PdfStructureAttributes(attributes));
                List nestedList = new List(ListNumberingType.DECIMAL);
                ListItem nestedItem = new ListItem("item4");
                nestedItem.GetAccessibilityProperties().SetRole(StandardRoles.P);
                nestedList.Add(nestedItem);
                list.Add(new ListItem("item1"));
                list.Add(new ListItem("item2"));
                list.Add(new ListItem("item3"));
                list.Add(nestedList);
                list.SetFont(PdfUAListTest.LoadFont());
                return list;
            }
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void SeveralListNumberingsFirstValidTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_261());
            // ListNumbering Decimal will be added to the beginning when processing List layout element.
            framework.AssertBothValid("severalListNumberingsFirstValidTest", pdfUAConformance);
        }

        private sealed class _Generator_261 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_261() {
            }

            public IBlockElement Generate() {
                List list = new List(ListNumberingType.DECIMAL);
                list.Add(new ListItem("item1"));
                list.Add(new ListItem("item2"));
                list.Add(new ListItem("item3"));
                list.SetFont(PdfUAListTest.LoadFont());
                PdfDictionary attributes = new PdfDictionary();
                attributes.Put(PdfName.O, PdfName.List);
                attributes.Put(PdfName.ListNumbering, PdfName.None);
                list.GetAccessibilityProperties().AddAttributes(new PdfStructureAttributes(attributes));
                list.GetAccessibilityProperties().AddAttributes(new PdfStructureAttributes(attributes));
                list.GetAccessibilityProperties().AddAttributes(new PdfStructureAttributes(attributes));
                return list;
            }
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void SeveralListNumberingsFirstInvalidTest(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                pdfDoc.GetTagStructureContext().NormalizeDocumentRootTag();
                PdfStructElem list = pdfUAConformance == PdfUAConformance.PDF_UA_1 ? pdfDoc.GetStructTreeRoot().AddKid(new 
                    PdfStructElem(pdfDoc, PdfName.L)) : ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0]).AddKid(new 
                    PdfStructElem(pdfDoc, PdfName.L));
                PdfDictionary validAttributes = new PdfDictionary();
                validAttributes.Put(PdfName.O, PdfName.List);
                validAttributes.Put(PdfName.ListNumbering, PdfName.Ordered);
                PdfDictionary invalidAttributes = new PdfDictionary();
                invalidAttributes.Put(PdfName.O, PdfName.List);
                invalidAttributes.Put(PdfName.ListNumbering, PdfName.None);
                PdfArray attributes = new PdfArray();
                attributes.Add(invalidAttributes);
                attributes.Add(validAttributes);
                attributes.Add(invalidAttributes);
                list.SetAttributes(attributes);
                PdfStructElem listItem1 = list.AddKid(new PdfStructElem(pdfDoc, PdfName.LI));
                listItem1.AddKid(new PdfStructElem(pdfDoc, PdfName.Lbl));
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("severalListNumberingsFirstInvalidTest", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("severalListNumberingsFirstInvalidTest", PdfUAExceptionMessageConstants.LIST_NUMBERING_IS_NOT_SPECIFIED
                        , pdfUAConformance);
                }
            }
        }

        private static PdfFont LoadFont() {
            try {
                return PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                    );
            }
            catch (System.IO.IOException e) {
                throw new Exception(e.Message);
            }
        }
    }
}
