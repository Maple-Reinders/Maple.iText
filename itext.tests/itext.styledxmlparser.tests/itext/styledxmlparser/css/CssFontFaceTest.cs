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
using System.Collections.Generic;
using iText.StyledXmlParser.Css.Font;
using iText.Test;

namespace iText.StyledXmlParser.Css {
    [NUnit.Framework.Category("UnitTest")]
    public class CssFontFaceTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CreateCssFontFaceTest() {
            IList<CssDeclaration> properties = new List<CssDeclaration>();
            properties.Add(new CssDeclaration("font-family", "Droid Italic"));
            properties.Add(new CssDeclaration("src", "url(\"web-fonts/droid-serif-italic.ttf\")"));
            CssFontFace fontFace = CssFontFace.Create(properties);
            NUnit.Framework.Assert.IsNotNull(fontFace);
            NUnit.Framework.Assert.AreEqual("droid italic", fontFace.GetFontFamily());
            IList<CssFontFace.CssFontFaceSrc> sources = fontFace.GetSources();
            NUnit.Framework.Assert.IsNotNull(sources);
            NUnit.Framework.Assert.AreEqual(1, sources.Count);
            NUnit.Framework.Assert.AreEqual("web-fonts/droid-serif-italic.ttf", sources[0].GetSrc());
        }

        [NUnit.Framework.Test]
        public virtual void CreateCssFontFaceNullSrcTest() {
            IList<CssDeclaration> properties = new List<CssDeclaration>();
            properties.Add(new CssDeclaration("font-family", "Droid Italic"));
            properties.Add(new CssDeclaration("src", null));
            CssFontFace fontFace = CssFontFace.Create(properties);
            NUnit.Framework.Assert.IsNull(fontFace);
        }

        [NUnit.Framework.Test]
        public virtual void CreateCssFontFaceNullFontFamilyTest() {
            IList<CssDeclaration> properties = new List<CssDeclaration>();
            properties.Add(new CssDeclaration("font-family", ""));
            properties.Add(new CssDeclaration("src", "some_directory/droid-serif-italic.ttf"));
            CssFontFace fontFace = CssFontFace.Create(properties);
            NUnit.Framework.Assert.IsNull(fontFace);
        }

        [NUnit.Framework.Test]
        public virtual void ParseFormatTest() {
            NUnit.Framework.Assert.AreEqual(CssFontFace.FontFormat.None, CssFontFace.CssFontFaceSrc.ParseFormat(null));
            NUnit.Framework.Assert.AreEqual(CssFontFace.FontFormat.TrueType, CssFontFace.CssFontFaceSrc.ParseFormat("Truetype"
                ));
            NUnit.Framework.Assert.AreEqual(CssFontFace.FontFormat.OpenType, CssFontFace.CssFontFaceSrc.ParseFormat("Opentype"
                ));
            NUnit.Framework.Assert.AreEqual(CssFontFace.FontFormat.WOFF, CssFontFace.CssFontFaceSrc.ParseFormat("Woff"
                ));
            NUnit.Framework.Assert.AreEqual(CssFontFace.FontFormat.WOFF2, CssFontFace.CssFontFaceSrc.ParseFormat("Woff2"
                ));
            NUnit.Framework.Assert.AreEqual(CssFontFace.FontFormat.EOT, CssFontFace.CssFontFaceSrc.ParseFormat("Embedded-opentype"
                ));
            NUnit.Framework.Assert.AreEqual(CssFontFace.FontFormat.SVG, CssFontFace.CssFontFaceSrc.ParseFormat("Svg"));
        }

        [NUnit.Framework.Test]
        public virtual void IsSupportedFontFormatTest() {
            NUnit.Framework.Assert.IsTrue(CssFontFace.IsSupportedFontFormat(CssFontFace.FontFormat.None));
            NUnit.Framework.Assert.IsTrue(CssFontFace.IsSupportedFontFormat(CssFontFace.FontFormat.TrueType));
            NUnit.Framework.Assert.IsTrue(CssFontFace.IsSupportedFontFormat(CssFontFace.FontFormat.OpenType));
            NUnit.Framework.Assert.IsTrue(CssFontFace.IsSupportedFontFormat(CssFontFace.FontFormat.WOFF));
            NUnit.Framework.Assert.IsTrue(CssFontFace.IsSupportedFontFormat(CssFontFace.FontFormat.WOFF2));
            NUnit.Framework.Assert.IsFalse(CssFontFace.IsSupportedFontFormat(CssFontFace.FontFormat.EOT));
            NUnit.Framework.Assert.IsFalse(CssFontFace.IsSupportedFontFormat(CssFontFace.FontFormat.SVG));
        }
    }
}
