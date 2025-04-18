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
using System.Linq;

namespace iText.Layout.Font {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// Key for
    /// <see cref="FontSelector"/>
    /// caching.
    /// </summary>
    /// <seealso cref="FontSelectorCache"/>
    internal sealed class FontSelectorKey {
        private IList<String> fontFamilies;

        private FontCharacteristics fc;

//\cond DO_NOT_DOCUMENT
        internal FontSelectorKey(IList<String> fontFamilies, FontCharacteristics fc) {
            this.fontFamilies = new List<String>(fontFamilies);
            this.fc = fc;
        }
//\endcond

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Layout.Font.FontSelectorKey that = (iText.Layout.Font.FontSelectorKey)o;
            return Enumerable.SequenceEqual(fontFamilies, that.fontFamilies) && (fc != null ? fc.Equals(that.fc) : that
                .fc == null);
        }

        public override int GetHashCode() {
            int result = fontFamilies != null ? fontFamilies.GetHashCode() : 0;
            result = 31 * result + (fc != null ? fc.GetHashCode() : 0);
            return result;
        }
    }
//\endcond
}
