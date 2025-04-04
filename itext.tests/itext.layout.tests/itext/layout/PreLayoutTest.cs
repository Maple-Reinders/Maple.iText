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
using System.Text;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PreLayoutTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/PreLayoutTest/";

        public static readonly String destinationFolder = TestUtil.GetOutputPath() + "/layout/PreLayoutTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void PreLayoutTest01() {
            String outFileName = destinationFolder + "preLayoutTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_preLayoutTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName)).SetTagged();
            Document document = new Document(pdfDocument, PageSize.DEFAULT, false);
            IList<Text> pageNumberTexts = new List<Text>();
            IList<IRenderer> pageNumberRenderers = new List<IRenderer>();
            document.SetProperty(Property.FONT, PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
            for (int i = 0; i < 200; i++) {
                document.Add(new Paragraph("This is just junk text"));
                if (i % 10 == 0) {
                    Text pageNumberText = new Text("Page #: {pageNumber}");
                    IRenderer renderer = new TextRenderer(pageNumberText);
                    pageNumberText.SetNextRenderer(renderer);
                    pageNumberRenderers.Add(renderer);
                    Paragraph pageNumberParagraph = new Paragraph().Add(pageNumberText);
                    pageNumberTexts.Add(pageNumberText);
                    document.Add(pageNumberParagraph);
                }
            }
            foreach (IRenderer renderer in pageNumberRenderers) {
                String currentData = renderer.ToString().Replace("{pageNumber}", renderer.GetOccupiedArea().GetPageNumber(
                    ).ToString());
                ((TextRenderer)renderer).SetText(currentData);
                ((Text)renderer.GetModelElement()).SetNextRenderer(renderer);
            }
            document.Relayout();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void PreLayoutTest02() {
            String outFileName = destinationFolder + "preLayoutTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_preLayoutTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDoc, PageSize.DEFAULT, false);
            document.Add(new Paragraph("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"));
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < 1200; i++) {
                text.Append("A very long text is here...");
            }
            Paragraph twoColumnParagraph = new Paragraph();
            twoColumnParagraph.SetNextRenderer(new PreLayoutTest.TwoColumnParagraphRenderer(twoColumnParagraph));
            iText.Layout.Element.Text textElement = new iText.Layout.Element.Text(text.ToString());
            twoColumnParagraph.Add(textElement).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
            document.Add(twoColumnParagraph);
            document.Add(new Paragraph("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"));
            int paragraphLastPageNumber = -1;
            IList<IRenderer> documentChildRenderers = document.GetRenderer().GetChildRenderers();
            for (int i = documentChildRenderers.Count - 1; i >= 0; i--) {
                if (documentChildRenderers[i].GetModelElement() == twoColumnParagraph) {
                    paragraphLastPageNumber = documentChildRenderers[i].GetOccupiedArea().GetPageNumber();
                    break;
                }
            }
            twoColumnParagraph.SetNextRenderer(new PreLayoutTest.TwoColumnParagraphRenderer(twoColumnParagraph, paragraphLastPageNumber
                ));
            document.Relayout();
            //Close document. Drawing of content is happened on close
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ColumnDocumentRendererRelayoutTest() {
            String outFileName = destinationFolder + "columnDocumentRendererRelayoutTest.pdf";
            String cmpFileName = sourceFolder + "cmp_columnDocumentRendererRelayoutTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName)).SetTagged();
            Document document = new Document(pdfDoc, PageSize.DEFAULT, false);
            Rectangle column1 = new Rectangle(40, 40, 200, 720);
            Rectangle column2 = new Rectangle(300, 40, 200, 720);
            document.SetRenderer(new ColumnDocumentRenderer(document, false, new Rectangle[] { column1, column2 }));
            String text = "The series continues with Harry Potter and the Chamber of Secrets, describing Harry's second year at Hogwarts. He and his friends investigate a 50-year-old mystery that appears uncannily related to recent sinister events at the school. Ron's younger sister, Ginny Weasley, enrols in her first year at Hogwarts, and finds an old notebook which turns out to be a previous student's diary, Tom Marvolo Riddle, who later turns out to be Voldemort. The memory of Tom Riddle is inside of the diary and when Ginny begins to confide in the diary Voldemort begins to possess her. Ginny becomes possessed by Voldemort through the diary and unconsciously opens the \"Chamber of Secrets\", unleashing an ancient monster, later revealed to be a basilisk, which begins attacking students at Hogwarts. The novel delves into the history of Hogwarts and a legend revolving around the Chamber that soon frightens everyone in the school. The book also introduces a new Defence Against the Dark Arts teacher, Gilderoy Lockhart, a highly cheerful, self-conceited wizard who goes around as if he is the most wonderful person who ever existed, who knows absolutely every single thing there is to know about everything, who later turns out to be a fraud. Harry discovers that prejudice exists in the wizarding world, and learns that Voldemort's reign of terror was often directed at wizards who were descended from muggles. Harry also learns that his ability to speak the snake language Parseltongue is rare and often associated with the Dark Arts. The novel ends after Harry saves Ginny's life by destroying the basilisk and the enchanted diary which has been the source of the problems.";
            for (int i = 0; i < 3; i++) {
                text = text + " " + text;
            }
            document.Add(new Paragraph(text));
            document.Relayout();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

//\cond DO_NOT_DOCUMENT
        internal class TwoColumnParagraphRenderer : ParagraphRenderer {
//\cond DO_NOT_DOCUMENT
            internal int oneColumnPage = -1;
//\endcond

            public TwoColumnParagraphRenderer(Paragraph modelElement)
                : base(modelElement) {
            }

            public TwoColumnParagraphRenderer(Paragraph modelElement, int oneColumnPage)
                : this(modelElement) {
                this.oneColumnPage = oneColumnPage;
            }

            public override IList<Rectangle> InitElementAreas(LayoutArea area) {
                IList<Rectangle> areas = new List<Rectangle>();
                if (area.GetPageNumber() != oneColumnPage) {
                    Rectangle firstArea = area.GetBBox().Clone();
                    Rectangle secondArea = area.GetBBox().Clone();
                    firstArea.SetWidth(firstArea.GetWidth() / 2 - 5);
                    secondArea.SetX(secondArea.GetX() + secondArea.GetWidth() / 2 + 5);
                    secondArea.SetWidth(firstArea.GetWidth());
                    areas.Add(firstArea);
                    areas.Add(secondArea);
                }
                else {
                    areas.Add(area.GetBBox());
                }
                return areas;
            }

            public override IRenderer GetNextRenderer() {
                return new PreLayoutTest.TwoColumnParagraphRenderer((Paragraph)modelElement, oneColumnPage);
            }
        }
//\endcond
    }
}
