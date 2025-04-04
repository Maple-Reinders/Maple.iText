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
using iText.Svg.Exceptions;
using iText.Svg.Renderers;

namespace iText.Svg.Dummy.Renderers.Impl {
    public class DummyProcessableSvgNodeRenderer : DummySvgNodeRenderer {
        private bool processed = false;

        public override void Draw(SvgDrawContext context) {
            if (processed) {
                throw new SvgProcessingException("Cannot process svg renderer twice");
            }
            processed = true;
        }

        public virtual bool IsProcessed() {
            return processed;
        }
    }
}
