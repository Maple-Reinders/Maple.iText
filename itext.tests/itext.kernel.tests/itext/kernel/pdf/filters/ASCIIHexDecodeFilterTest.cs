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
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel.Pdf.Filters {
    [NUnit.Framework.Category("UnitTest")]
    public class ASCIIHexDecodeFilterTest : ExtendedITextTest {
        public static readonly String SOURCE_FILE = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/filters/ASCIIHex.bin";

        [NUnit.Framework.Test]
        public virtual void DecodingTest() {
            FileInfo file = new FileInfo(SOURCE_FILE);
            byte[] bytes = File.ReadAllBytes(file.FullName);
            String expectedResult = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " + "Donec ac malesuada tellus. "
                 + "Quisque a arcu semper, tristique nibh eu, convallis lacus. " + "Donec neque justo, condimentum sed molestie ac, mollis eu nibh. "
                 + "Vivamus pellentesque condimentum fringilla. " + "Nullam euismod ac risus a semper. " + "Etiam hendrerit scelerisque sapien tristique varius.";
            String decoded = iText.Commons.Utils.JavaUtil.GetStringForBytes(ASCIIHexDecodeFilter.ASCIIHexDecode(bytes)
                );
            NUnit.Framework.Assert.AreEqual(expectedResult, decoded);
        }

        [NUnit.Framework.Test]
        public virtual void DecodingIllegalaCharacterTest() {
            byte[] bytes = "4c6f72656d20697073756d2eg>".GetBytes();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => ASCIIHexDecodeFilter.ASCIIHexDecode
                (bytes));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.ILLEGAL_CHARACTER_IN_ASCIIHEXDECODE, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void DecodingSkipWhitespacesTest() {
            byte[] bytes = "4c 6f 72 65 6d 20 69 70 73 75 6d 2e>".GetBytes();
            String expectedResult = "Lorem ipsum.";
            String decoded = iText.Commons.Utils.JavaUtil.GetStringForBytes(ASCIIHexDecodeFilter.ASCIIHexDecode(bytes)
                );
            NUnit.Framework.Assert.AreEqual(expectedResult, decoded);
        }
    }
}
