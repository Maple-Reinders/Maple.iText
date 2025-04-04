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
namespace iText.Layout.Element {
    /// <summary>
    /// A marker subinterface of
    /// <see cref="IElement"/>
    /// that specifies that the layout object
    /// is, by definition, on the lowest tier in the object hierarchy.
    /// </summary>
    /// <remarks>
    /// A marker subinterface of
    /// <see cref="IElement"/>
    /// that specifies that the layout object
    /// is, by definition, on the lowest tier in the object hierarchy. A
    /// <see cref="ILeafElement">leaf element</see>
    /// must not act as a container for other
    /// elements.
    /// </remarks>
    public interface ILeafElement : IElement {
    }
}
