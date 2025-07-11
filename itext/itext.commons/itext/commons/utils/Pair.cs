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

namespace iText.Commons.Utils {
    /// <summary>Class which represents a pair of key and value.</summary>
    /// <typeparam name="K">key parameter type.</typeparam>
    /// <typeparam name="V">value parameter type.</typeparam>
    [System.ObsoleteAttribute(@"in favour of iText.Commons.Datastructures.Tuple2{T1, T2}")]
    public class Pair<K, V> {
        private readonly K key;

        private readonly V value;

        /// <summary>Creates key-value pair.</summary>
        /// <param name="key">key parameter</param>
        /// <param name="value">value parameter</param>
        public Pair(K key, V value) {
            this.key = key;
            this.value = value;
        }

        /// <summary>Gets key parameter.</summary>
        /// <returns>key parameter.</returns>
        public virtual K GetKey() {
            return key;
        }

        /// <summary>Gets value parameter.</summary>
        /// <returns>value parameter.</returns>
        public virtual V GetValue() {
            return value;
        }
    }
}
