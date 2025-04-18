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
using iText.StyledXmlParser.Css;
using iText.Test;

namespace iText.StyledXmlParser.Css.Parse {
    [NUnit.Framework.Category("UnitTest")]
    public class CssRuleSetParserTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ParsePropertyDeclarationsTest() {
            String src = "float:right; clear:right;width:22.0em; margin:0 0 1.0em 1.0em; background:#f9f9f9; " + "border:1px solid #aaa;padding:0.2em;border-spacing:0.4em 0; text-align:center; "
                 + "line-height:1.4em; font-size:88%;";
            String[] expected = new String[] { "float: right", "clear: right", "width: 22.0em", "margin: 0 0 1.0em 1.0em"
                , "background: #f9f9f9", "border: 1px solid #aaa", "padding: 0.2em", "border-spacing: 0.4em 0", "text-align: center"
                , "line-height: 1.4em", "font-size: 88%" };
            IList<CssDeclaration> declarations = CssRuleSetParser.ParsePropertyDeclarations(src);
            NUnit.Framework.Assert.AreEqual(expected.Length, declarations.Count);
            for (int i = 0; i < expected.Length; i++) {
                NUnit.Framework.Assert.AreEqual(expected[i], declarations[i].ToString());
            }
        }
    }
}
