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

namespace iText.Svg.Renderers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class StrokeTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/StrokeTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/svg/renderers/impl/StrokeTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void NormalLineStrokeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "normalLineStroke");
        }

        [NUnit.Framework.Test]
        public virtual void NoLineStrokeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "noLineStroke");
        }

        [NUnit.Framework.Test]
        public virtual void NoLineStrokeWidthTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "noLineStrokeWidth");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeWithDashesTest() {
            // TODO DEVSIX-8854 Draw SVG elements with transparent stroke in 2 steps
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "strokeWithDashes");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeWithDashesAcrobatBugTest() {
            // Acrobat displays the result incorrectly, however e.g. Xodo PDF Studio displays the document exactly the same
            // as svg (in terms of stroke opacity and view box). Same issue is reproduced in the
            // DefaultStyleInheritanceIntegrationTest#usePropertiesInheritanceTest and nestedInheritanceTest,
            // ClipPathSvgNodeRendererIntegrationTest#clipPathComplexTest.
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "strokeWithDashesAcrobatBug");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacityTest() {
            // TODO DEVSIX-8854 Draw SVG elements with transparent stroke in 2 steps
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "strokeOpacity");
        }

        [NUnit.Framework.Test]
        public virtual void OverrideStrokeWidthTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "overrideStrokeWidth");
        }

        [NUnit.Framework.Test]
        public virtual void AdvancedStrokeTest() {
            //TODO: update cmp-file after DEVSIX-2258
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "strokeAdvanced");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeWidthMeasureUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "strokeWidthMeasureUnitsTest");
        }

        [NUnit.Framework.Test]
        public virtual void PathLengthTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "path-length");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeAttributesTest() {
            //TODO DEVSIX-2258: update cmp after supporting
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "stroke-attributes");
        }

        [NUnit.Framework.Test]
        public virtual void ZeroStrokeWidthTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "zeroStrokeWidth");
        }

        [NUnit.Framework.Test]
        public virtual void NegativeStrokeWidthTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "negativeStrokeWidth");
        }

        [NUnit.Framework.Test]
        public virtual void HeightWidthZeroTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "heightWidthZero");
        }

        [NUnit.Framework.Test]
        public virtual void HeightWidthNegativeTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "heightWidthNegative");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeDashArrayLinesTest() {
            //TODO: update cmp-file after DEVSIX-2258
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "strokeDashArrayLines");
        }

        //TODO DEVSIX-2507: Update cmp file after supporting
        [NUnit.Framework.Test]
        public virtual void StrokeTextTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "strokeText");
        }

        //TODO DEVSIX-2507: Update cmp file after supporting
        [NUnit.Framework.Test]
        public virtual void StrokeTspanTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "strokeTspan");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeObjectsOverlap1Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "strokeOnGroup");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeObjectsOverlap2Test() {
            //TODO DEVSIX-7338: SVG stroke on group applied incorrectly
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "strokeOnGroup2");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeObjectsOverlap3Test() {
            //TODO DEVSIX-7338: SVG stroke on group applied incorrectly
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "strokeOnGroupNoInsideStroke");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeObjectsOverlap4Test() {
            //TODO DEVSIX-7338: SVG stroke on group applied incorrectly
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "strokeOnGroupNoInsideStroke2");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeObjectsOverlap5Test() {
            //TODO DEVSIX-7338: Update cmp file
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "strokeOnGroupNoInsideStroke3");
        }
    }
}
