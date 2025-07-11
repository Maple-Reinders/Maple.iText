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
using iText.Test;

namespace iText.IO.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class Type1FontTest : ExtendedITextTest {
        public static readonly String FONTS_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/type1/testPackage/";

        [NUnit.Framework.Test]
        public virtual void FillUsingEncodingTest() {
            FontEncoding fontEncoding = FontEncoding.CreateFontEncoding("WinAnsiEncoding");
            Type1Font type1StdFont = (Type1Font)FontProgramFactory.CreateFont("Helvetica", true);
            NUnit.Framework.Assert.AreEqual(149, type1StdFont.codeToGlyph.Count);
            type1StdFont.InitializeGlyphs(fontEncoding);
            NUnit.Framework.Assert.AreEqual(217, type1StdFont.codeToGlyph.Count);
            NUnit.Framework.Assert.AreEqual(0x2013, type1StdFont.codeToGlyph.Get(150).GetUnicode());
            NUnit.Framework.Assert.AreEqual(new char[] { (char)0x2013 }, type1StdFont.codeToGlyph.Get(150).GetChars());
        }

        [NUnit.Framework.Test]
        public virtual void GetFontStreamBytesTest() {
            FontProgram fp = FontProgramFactory.CreateType1Font(FONTS_FOLDER + "cmr10.afm", FONTS_FOLDER + "cmr10.pfb"
                );
            NUnit.Framework.Assert.AreEqual(26864, ((Type1Font)fp).GetFontStreamBytes().Length);
        }
    }
}
