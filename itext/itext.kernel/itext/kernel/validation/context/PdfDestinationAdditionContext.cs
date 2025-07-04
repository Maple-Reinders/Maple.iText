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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Validation;

namespace iText.Kernel.Validation.Context {
    /// <summary>Class which contains context in which destination was added.</summary>
    public class PdfDestinationAdditionContext : IValidationContext {
        private readonly PdfDestination destination;

        private readonly PdfAction action;

        /// <summary>
        /// Creates
        /// <see cref="PdfDestinationAdditionContext"/>
        /// instance.
        /// </summary>
        /// <param name="destination">
        /// 
        /// <see cref="iText.Kernel.Pdf.Navigation.PdfDestination"/>
        /// instance which was added
        /// </param>
        public PdfDestinationAdditionContext(PdfDestination destination) {
            this.destination = destination;
            this.action = null;
        }

        /// <summary>
        /// Creates
        /// <see cref="PdfDestinationAdditionContext"/>
        /// instance.
        /// </summary>
        /// <param name="destinationObject">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// which represents destination
        /// </param>
        public PdfDestinationAdditionContext(PdfObject destinationObject) {
            // Second check is needed in case of destination page being partially flushed.
            if (destinationObject != null && !destinationObject.IsFlushed() && (!(destinationObject is PdfArray) || !(
                (PdfArray)destinationObject).Get(0).IsFlushed())) {
                this.destination = PdfDestination.MakeDestination(destinationObject, false);
            }
            else {
                this.destination = null;
            }
            this.action = null;
        }

        public PdfDestinationAdditionContext(PdfAction action) {
            this.destination = null;
            this.action = action;
        }

        public virtual ValidationType GetType() {
            return ValidationType.DESTINATION_ADDITION;
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Kernel.Pdf.Navigation.PdfDestination"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.Navigation.PdfDestination"/>
        /// instance
        /// </returns>
        public virtual PdfDestination GetDestination() {
            return destination;
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// instance
        /// </returns>
        public virtual PdfAction GetAction() {
            return action;
        }
    }
}
