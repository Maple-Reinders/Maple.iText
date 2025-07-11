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
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("UnitTest")]
    public class MarkerSvgNodeRendererUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NoObjectBoundingBoxTest() {
            MarkerSvgNodeRenderer renderer = new MarkerSvgNodeRenderer();
            NUnit.Framework.Assert.IsNull(renderer.GetObjectBoundingBox(null));
        }

        [NUnit.Framework.Test]
        public virtual void NullViewportTest() {
            MarkerSvgNodeRenderer renderer = new MarkerSvgNodeRenderer();
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("markerwidth", "300pt");
            styles.Put("markerheight", "300pt");
            renderer.SetAttributesAndStyles(styles);
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => renderer.ApplyViewBox(new SvgDrawContext
                (null, null)));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.CURRENT_VIEWPORT_IS_NULL, e.Message);
        }
    }
}
