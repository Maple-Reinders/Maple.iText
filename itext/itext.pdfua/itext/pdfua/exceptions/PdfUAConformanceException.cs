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
using iText.Kernel.Exceptions;

namespace iText.Pdfua.Exceptions {
    /// <summary>Exception that is thrown when the PDF Document doesn't adhere to the PDF/UA specification.</summary>
    public class PdfUAConformanceException : PdfException {
        /// <summary>Creates a PdfUAConformanceException.</summary>
        /// <param name="message">the error message</param>
        public PdfUAConformanceException(String message)
            : base(message) {
        }

        /// <summary>Creates a PdfUAConformanceException.</summary>
        /// <param name="message">the detail message.</param>
        /// <param name="cause">the cause.</param>
        public PdfUAConformanceException(String message, Exception cause)
            : base(message, cause) {
        }
    }
}
