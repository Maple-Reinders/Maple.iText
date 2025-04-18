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

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("UnitTest")]
    public class PathOperatorSplitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestNumbersContainingExponent01() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6457 fix different behavior of Pattern.split method)
            String path = "M10,9.999999999999972C203.33333333333334,9.999999999999972,396.6666666666667,1.4210854715202004e-14,590,1.4210854715202004e-14L590,41.666666666666686C396.6666666666667,41.666666666666686,203.33333333333334,51.66666666666664,10,51.66666666666664Z";
            String[] operators = new String[] { "M10,9.999999999999972", "C203.33333333333334,9.999999999999972,396.6666666666667,1.4210854715202004e-14,590,1.4210854715202004e-14"
                , "L590,41.666666666666686", "C396.6666666666667,41.666666666666686,203.33333333333334,51.66666666666664,10,51.66666666666664"
                , "Z" };
            TestSplitting(path, operators);
        }

        private void TestSplitting(String originalStr, String[] expectedSplitting) {
            String[] result = PathSvgNodeRenderer.SplitPathStringIntoOperators(originalStr);
            NUnit.Framework.Assert.AreEqual(expectedSplitting, result);
        }
    }
}
