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
using iText.Test;

namespace iText.Commons.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class FileUtilTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/commons/utils/FileUtilTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void GetBufferedOutputStreamTest() {
            String filePath = DESTINATION_FOLDER + "bufferedOutput.txt";
            String text = "Hello world!";
            using (Stream @out = FileUtil.GetBufferedOutputStream(filePath)) {
                @out.Write(text.GetBytes(System.Text.Encoding.UTF8));
            }
            byte[] resultBytes = File.ReadAllBytes(System.IO.Path.Combine(filePath));
            NUnit.Framework.Assert.AreEqual(text, iText.Commons.Utils.JavaUtil.GetStringForBytes(resultBytes, System.Text.Encoding
                .UTF8));
        }

        [NUnit.Framework.Test]
        public virtual void GetFileOutputStreamTest() {
            String filePath = DESTINATION_FOLDER + "fileOutput.txt";
            FileInfo file = new FileInfo(filePath);
            String text = "Hello world!";
            using (Stream @out = FileUtil.GetFileOutputStream(file)) {
                @out.Write(text.GetBytes(System.Text.Encoding.UTF8));
            }
            byte[] resultBytes = File.ReadAllBytes(System.IO.Path.Combine(filePath));
            NUnit.Framework.Assert.AreEqual(text, iText.Commons.Utils.JavaUtil.GetStringForBytes(resultBytes, System.Text.Encoding
                .UTF8));
        }
    }
}
