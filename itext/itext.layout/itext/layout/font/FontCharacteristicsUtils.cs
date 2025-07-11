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
using iText.Commons.Utils;
using iText.IO.Font.Constants;

namespace iText.Layout.Font {
//\cond DO_NOT_DOCUMENT
    internal sealed class FontCharacteristicsUtils {
//\cond DO_NOT_DOCUMENT
        internal static short NormalizeFontWeight(short fw) {
            fw = (short)((fw / 100) * 100);
            if (fw < FontWeights.THIN) {
                return FontWeights.THIN;
            }
            if (fw > FontWeights.BLACK) {
                return FontWeights.BLACK;
            }
            return fw;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static short ParseFontWeight(String fw) {
            if (fw == null || fw.Length == 0) {
                return -1;
            }
            fw = StringNormalizer.Normalize(fw);
            switch (fw) {
                case "bold": {
                    return FontWeights.BOLD;
                }

                case "normal": {
                    return FontWeights.NORMAL;
                }

                default: {
                    try {
                        return NormalizeFontWeight((short)Convert.ToInt32(fw, System.Globalization.CultureInfo.InvariantCulture));
                    }
                    catch (FormatException) {
                        return -1;
                    }
                    break;
                }
            }
        }
//\endcond
    }
//\endcond
}
