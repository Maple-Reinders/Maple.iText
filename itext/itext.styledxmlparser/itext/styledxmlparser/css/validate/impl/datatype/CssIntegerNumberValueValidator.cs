/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Css.Validate;

namespace iText.StyledXmlParser.Css.Validate.Impl.Datatype {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Css.Validate.ICssDataTypeValidator"/>
    /// implementation for integer numeric elements.
    /// </summary>
    public class CssIntegerNumberValueValidator : ICssDataTypeValidator {
        private readonly bool allowedNegative;

        private readonly bool allowedZero;

        /// <summary>
        /// Creates a new
        /// <see cref="CssIntegerNumberValueValidator"/>
        /// instance.
        /// </summary>
        /// <param name="allowedNegative">is negative value allowed</param>
        /// <param name="allowedZero">is zero value allowed</param>
        public CssIntegerNumberValueValidator(bool allowedNegative, bool allowedZero) {
            this.allowedNegative = allowedNegative;
            this.allowedZero = allowedZero;
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool IsValid(String objectString) {
            if (objectString == null) {
                return false;
            }
            if (CommonCssConstants.INITIAL.Equals(objectString) || CommonCssConstants.INHERIT.Equals(objectString) || 
                CommonCssConstants.UNSET.Equals(objectString)) {
                return true;
            }
            if (!CssTypesValidationUtils.IsIntegerNumber(objectString)) {
                return false;
            }
            if (CssTypesValidationUtils.IsNegativeValue(objectString) && !allowedNegative) {
                return false;
            }
            if (CssTypesValidationUtils.IsZero(objectString) && !allowedZero) {
                return false;
            }
            return true;
        }
    }
}