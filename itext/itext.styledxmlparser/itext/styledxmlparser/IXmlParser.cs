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
using System.IO;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser {
    /// <summary>Interface for the XML parsing operations that accept XML and return a document node.</summary>
    public interface IXmlParser {
        /// <summary>
        /// Parses XML provided as an
        /// <c>InputStream</c>
        /// and an encoding.
        /// </summary>
        /// <param name="XmlStream">the Xml stream</param>
        /// <param name="charset">
        /// the character set. If
        /// <see langword="null"/>
        /// then parser should detect encoding from stream.
        /// </param>
        /// <returns>a document node</returns>
        IDocumentNode Parse(Stream XmlStream, String charset);

        /// <summary>
        /// Parses XML provided as a
        /// <c>String</c>.
        /// </summary>
        /// <param name="Xml">the Xml string</param>
        /// <returns>a document node</returns>
        IDocumentNode Parse(String Xml);
    }
}
