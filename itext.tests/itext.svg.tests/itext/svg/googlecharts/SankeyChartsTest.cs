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

namespace iText.Svg.Googlecharts {
    [NUnit.Framework.Category("IntegrationTest")]
    public class SankeyChartsTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/googlecharts/SankeyChartsTest/";

        public static readonly String destinationFolder = TestUtil.GetOutputPath() + "/svg/googlecharts/SankeyChartsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void SankeyBordersChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "sankeyBordersChart");
        }

        [NUnit.Framework.Test]
        public virtual void SankeyChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "sankeyChart");
        }

        [NUnit.Framework.Test]
        public virtual void SankeyColoredChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "sankeyColoredChart");
        }

        [NUnit.Framework.Test]
        public virtual void SankeyFontsChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "sankeyFontsChart");
        }

        [NUnit.Framework.Test]
        public virtual void SankeyMultilevelChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "sankeyMultilevelChart");
        }

        [NUnit.Framework.Test]
        public virtual void SankeyNodesChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "sankeyNodesChart");
        }
    }
}
