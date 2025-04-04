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
using iText.IO.Exceptions;
using iText.IO.Font.Constants;
using iText.IO.Font.Otf;
using iText.Test;

namespace iText.IO.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class FontProgramTest : ExtendedITextTest {
        private const String notExistingFont = "some-font.ttf";

        [NUnit.Framework.SetUp]
        public virtual void ClearFonts() {
            FontProgramFactory.ClearRegisteredFonts();
            FontProgramFactory.ClearRegisteredFontFamilies();
            FontCache.ClearSavedFonts();
        }

        [NUnit.Framework.Test]
        public virtual void ExceptionMessageTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => FontProgramFactory.CreateFont
                (notExistingFont));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.NOT_FOUND_AS_FILE_OR_RESOURCE
                , notExistingFont), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void BoldTest() {
            FontProgram fp = FontProgramFactory.CreateFont(StandardFonts.HELVETICA);
            fp.SetBold(true);
            NUnit.Framework.Assert.IsTrue((fp.GetPdfFontFlags() & (1 << 18)) != 0, "Bold expected");
            fp.SetBold(false);
            NUnit.Framework.Assert.IsTrue((fp.GetPdfFontFlags() & (1 << 18)) == 0, "Not Bold expected");
        }

        [NUnit.Framework.Test]
        public virtual void RegisterDirectoryOpenTypeTest() {
            FontProgramFactory.ClearRegisteredFonts();
            FontProgramFactory.ClearRegisteredFontFamilies();
            FontCache.ClearSavedFonts();
            FontProgramFactory.RegisterFontDirectory(iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/");
            NUnit.Framework.Assert.AreEqual(43, FontProgramFactory.GetRegisteredFonts().Count);
            NUnit.Framework.Assert.IsNull(FontCache.GetFont(iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/FreeSansBold.ttf"));
            NUnit.Framework.Assert.IsTrue(FontProgramFactory.GetRegisteredFonts().Contains("free sans lihavoitu"));
        }

        [NUnit.Framework.Test]
        public virtual void RegisterDirectoryType1Test() {
            FontProgramFactory.RegisterFontDirectory(iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory) + "/resources/itext/io/font/type1/");
            FontProgram computerModern = FontProgramFactory.CreateRegisteredFont("computer modern");
            FontProgram cmr10 = FontProgramFactory.CreateRegisteredFont("cmr10");
            NUnit.Framework.Assert.IsNull(computerModern);
            NUnit.Framework.Assert.IsNull(cmr10);
        }

        [NUnit.Framework.Test]
        public virtual void RegisterDirectoryType1RecursivelyTest() {
            FontProgramFactory.RegisterFontDirectoryRecursively(iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory) + "/resources/itext/io/font/type1/");
            FontProgram computerModern = FontProgramFactory.CreateRegisteredFont("computer modern");
            FontProgram cmr10 = FontProgramFactory.CreateRegisteredFont("cmr10");
            NUnit.Framework.Assert.IsNotNull(computerModern);
            NUnit.Framework.Assert.IsNotNull(cmr10);
        }

        [NUnit.Framework.Test]
        public virtual void CidFontWithCmapTest() {
            char space = ' ';
            FontProgram fp = FontProgramFactory.CreateFont("KozMinPro-Regular", "UniJIS-UCS2-HW-H", true);
            Glyph glyph = fp.GetGlyph(space);
            NUnit.Framework.Assert.AreEqual(new char[] { space }, glyph.GetUnicodeChars());
            NUnit.Framework.Assert.AreEqual(32, glyph.GetUnicode());
            NUnit.Framework.Assert.AreEqual(231, glyph.GetCode());
            NUnit.Framework.Assert.AreEqual(500, glyph.GetWidth());
            fp = FontProgramFactory.CreateFont("KozMinPro-Regular", null, true);
            glyph = fp.GetGlyph(space);
            NUnit.Framework.Assert.AreEqual(new char[] { space }, glyph.GetUnicodeChars());
            NUnit.Framework.Assert.AreEqual(32, glyph.GetUnicode());
            NUnit.Framework.Assert.AreEqual(1, glyph.GetCode());
            NUnit.Framework.Assert.AreEqual(278, glyph.GetWidth());
        }

        [NUnit.Framework.Test]
        public virtual void StandardFontsTest() {
            CheckStandardFont(StandardFonts.COURIER);
            CheckStandardFont(StandardFonts.COURIER_BOLD);
            CheckStandardFont(StandardFonts.COURIER_BOLDOBLIQUE);
            CheckStandardFont(StandardFonts.COURIER_OBLIQUE);
            CheckStandardFont(StandardFonts.HELVETICA);
            CheckStandardFont(StandardFonts.HELVETICA_BOLD);
            CheckStandardFont(StandardFonts.HELVETICA_BOLDOBLIQUE);
            CheckStandardFont(StandardFonts.HELVETICA_OBLIQUE);
            CheckStandardFont(StandardFonts.SYMBOL);
            CheckStandardFont(StandardFonts.TIMES_BOLD);
            CheckStandardFont(StandardFonts.TIMES_BOLDITALIC);
            CheckStandardFont(StandardFonts.TIMES_ITALIC);
            CheckStandardFont(StandardFonts.TIMES_ROMAN);
            CheckStandardFont(StandardFonts.ZAPFDINGBATS);
        }

        private void CheckStandardFont(String fontName) {
            FontProgram font = FontProgramFactory.CreateFont(fontName, null, false);
            NUnit.Framework.Assert.IsTrue(font is Type1Font);
            NUnit.Framework.Assert.AreEqual(fontName, font.GetFontNames().GetFontName());
        }
    }
}
