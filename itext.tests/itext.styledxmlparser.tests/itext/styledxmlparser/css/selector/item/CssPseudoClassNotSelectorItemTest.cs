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
using iText.Commons.Utils;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Css.Selector;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Test;

namespace iText.StyledXmlParser.Css.Selector.Item {
    [NUnit.Framework.Category("UnitTest")]
    public class CssPseudoClassNotSelectorItemTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/styledxmlparser/css/selector/item/CssPseudoClassNotSelectorItemTest/";

        [NUnit.Framework.Test]
        public virtual void CssPseudoClassNotSelectorItemWithSelectorsListTest() {
            String filename = SOURCE_FOLDER + "cssPseudoClassNotSelectorItemTest.html";
            CssPseudoClassNotSelectorItem item = new CssPseudoClassNotSelectorItem(new CssSelector("p > :not(strong, b.important)"
                ));
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode documentNode = htmlParser.Parse(FileUtil.GetInputStreamForFile(filename), "UTF-8");
            IElementNode body = new JsoupElementNode(((JsoupDocumentNode)documentNode).GetDocument().GetElementsByTag(
                "body")[0]);
            NUnit.Framework.Assert.IsFalse(item.Matches(documentNode));
            NUnit.Framework.Assert.IsTrue(item.Matches(body));
            NUnit.Framework.Assert.IsFalse(item.Matches(null));
        }
    }
}
