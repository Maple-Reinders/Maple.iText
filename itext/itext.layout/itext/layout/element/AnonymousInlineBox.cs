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
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Renderer;

namespace iText.Layout.Element {
    /// <summary>
    /// A layout element that represents anonymous box,
    /// see https://developer.mozilla.org/en-US/docs/Web/CSS/Visual_formatting_model#anonymous_boxes.
    /// </summary>
    public class AnonymousInlineBox : Paragraph {
        /// <summary>
        /// Creates an
        /// <see cref="AnonymousInlineBox"/>.
        /// </summary>
        public AnonymousInlineBox()
            : base() {
        }

        /// <summary><inheritDoc/></summary>
        public override AccessibilityProperties GetAccessibilityProperties() {
            if (tagProperties == null) {
                tagProperties = new DefaultAccessibilityProperties(StandardRoles.NONSTRUCT);
            }
            return tagProperties;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override IRenderer MakeNewRenderer() {
            return new AnonymousInlineBoxRenderer(this);
        }
    }
}
