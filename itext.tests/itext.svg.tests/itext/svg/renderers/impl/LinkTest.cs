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
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    public class LinkTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/LinkTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/svg/renderers/impl/LinkTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CircleLinkTest() {
            //TODO: DEVSIX-8710 update cmp file after fix
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circleLink");
        }

        [NUnit.Framework.Test]
        public virtual void TextLinkTest() {
            //TODO: DEVSIX-8710 update cmp file after fix
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textLink");
        }

        [NUnit.Framework.Test]
        public virtual void CombinedElementsLinkTest() {
            //TODO: DEVSIX-8710 update cmp file after fix
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "combinedElementsLink");
        }

        [NUnit.Framework.Test]
        public virtual void PathLinkTest() {
            //TODO: DEVSIX-8710 update cmp file after fix
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathLink");
        }

        [NUnit.Framework.Test]
        public virtual void LineLinkTest() {
            //TODO: DEVSIX-8710 update cmp file after fix
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "lineLink");
        }

        [NUnit.Framework.Test]
        public virtual void PolygonLinkTest() {
            //TODO: DEVSIX-8710 update cmp file after fix
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "polygonLink");
        }

        [NUnit.Framework.Test]
        public virtual void GroupLinkTest() {
            //TODO: DEVSIX-8710 update cmp file after fix
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "groupLink");
        }

        [NUnit.Framework.Test]
        public virtual void NestedSvgLinkTest() {
            //TODO: DEVSIX-8710 update cmp file after fix
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "nestedSvgLink");
        }
    }
}
