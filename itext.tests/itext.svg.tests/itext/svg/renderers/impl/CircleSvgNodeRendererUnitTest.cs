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
using iText.Layout.Font;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("UnitTest")]
    public class CircleSvgNodeRendererUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NoViewPortTest() {
            CircleSvgNodeRenderer renderer = new CircleSvgNodeRenderer();
            SvgDrawContext context = new SvgDrawContext(new ResourceResolver(""), new FontProvider());
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("r", "50%");
            renderer.SetAttributesAndStyles(styles);
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => renderer.SetParameters(context
                ));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.ILLEGAL_RELATIVE_VALUE_NO_VIEWPORT_IS_SET, e.Message
                );
        }
    }
}
