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
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Navigation {
    public class PdfStringDestination : PdfDestination {
        public PdfStringDestination(String @string)
            : this(new PdfString(@string)) {
        }

        public PdfStringDestination(PdfString pdfObject)
            : base(pdfObject) {
        }

        public override PdfObject GetDestinationPage(IPdfNameTreeAccess names) {
            PdfObject destination = names.GetEntry((PdfString)GetPdfObject());
            if (destination is PdfArray) {
                return ((PdfArray)destination).Get(0);
            }
            else {
                if (destination is PdfDictionary) {
                    PdfArray destinationArray = ((PdfDictionary)destination).GetAsArray(PdfName.D);
                    return destinationArray != null ? destinationArray.Get(0) : null;
                }
            }
            return null;
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
}
