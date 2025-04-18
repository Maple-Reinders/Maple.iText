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
using iText.Commons.Utils;
using iText.IO.Exceptions;
using iText.IO.Source;

namespace iText.IO.Util {
    /// <summary>This file is a helper class for internal usage only.</summary>
    /// <remarks>
    /// This file is a helper class for internal usage only.
    /// Be aware that its API and functionality may be changed in future.
    /// </remarks>
    public sealed class StreamUtil {
        private const int TRANSFER_SIZE = 64 * 1024;

        private static readonly byte[] escR = ByteUtils.GetIsoBytes("\\r");

        private static readonly byte[] escN = ByteUtils.GetIsoBytes("\\n");

        private static readonly byte[] escT = ByteUtils.GetIsoBytes("\\t");

        private static readonly byte[] escB = ByteUtils.GetIsoBytes("\\b");

        private static readonly byte[] escF = ByteUtils.GetIsoBytes("\\f");

        private StreamUtil() {
        }

        /// <summary>
        /// This method is an alternative for the
        /// <c>InputStream.skip()</c>
        /// -method that doesn't seem to work properly for big values of
        /// <paramref name="size"/>.
        /// </summary>
        /// <param name="stream">
        /// the
        /// <c>InputStream</c>
        /// </param>
        /// <param name="size">the number of bytes to skip</param>
        public static void Skip(Stream stream, long size) {
            long n;
            while (size > 0) {
                n = stream.Skip(size);
                if (n <= 0) {
                    break;
                }
                size -= n;
            }
        }

        /// <summary>
        /// Escapes a
        /// <c>byte</c>
        /// array according to the PDF conventions.
        /// </summary>
        /// <param name="bytes">
        /// the
        /// <c>byte</c>
        /// array to escape
        /// </param>
        /// <returns>
        /// an escaped
        /// <c>byte</c>
        /// array
        /// </returns>
        public static byte[] CreateEscapedString(byte[] bytes) {
            return CreateBufferedEscapedString(bytes).ToByteArray();
        }

        /// <summary>
        /// Escapes a
        /// <c>byte</c>
        /// array according to the PDF conventions.
        /// </summary>
        /// <param name="outputStream">
        /// the
        /// <c>OutputStream</c>
        /// an escaped
        /// <c>byte</c>
        /// array write to.
        /// </param>
        /// <param name="bytes">
        /// the
        /// <c>byte</c>
        /// array to escape.
        /// </param>
        public static void WriteEscapedString(Stream outputStream, byte[] bytes) {
            ByteBuffer buf = CreateBufferedEscapedString(bytes);
            try {
                outputStream.Write(buf.GetInternalBuffer(), 0, buf.Size());
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.CANNOT_WRITE_BYTES, e);
            }
        }

        public static void WriteHexedString(Stream outputStream, byte[] bytes) {
            ByteBuffer buf = CreateBufferedHexedString(bytes);
            try {
                outputStream.Write(buf.GetInternalBuffer(), 0, buf.Size());
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.CANNOT_WRITE_BYTES, e);
            }
        }

        public static ByteBuffer CreateBufferedEscapedString(byte[] bytes) {
            ByteBuffer buf = new ByteBuffer(bytes.Length * 2 + 2);
            buf.Append('(');
            foreach (byte b in bytes) {
                switch (b) {
                    case (byte)'\r': {
                        buf.Append(escR);
                        break;
                    }

                    case (byte)'\n': {
                        buf.Append(escN);
                        break;
                    }

                    case (byte)'\t': {
                        buf.Append(escT);
                        break;
                    }

                    case (byte)'\b': {
                        buf.Append(escB);
                        break;
                    }

                    case (byte)'\f': {
                        buf.Append(escF);
                        break;
                    }

                    case (byte)'(':
                    case (byte)')':
                    case (byte)'\\': {
                        buf.Append('\\').Append(b);
                        break;
                    }

                    default: {
                        if (b < 8 && b >= 0) {
                            buf.Append("\\00").Append(JavaUtil.IntegerToOctalString(b));
                        }
                        else {
                            if (b >= 8 && b < 32) {
                                buf.Append("\\0").Append(JavaUtil.IntegerToOctalString(b));
                            }
                            else {
                                buf.Append(b);
                            }
                        }
                        break;
                    }
                }
            }
            buf.Append(')');
            return buf;
        }

        public static ByteBuffer CreateBufferedHexedString(byte[] bytes) {
            ByteBuffer buf = new ByteBuffer(bytes.Length * 2 + 2);
            buf.Append('<');
            foreach (byte b in bytes) {
                buf.AppendHex(b);
            }
            buf.Append('>');
            return buf;
        }

        public static void TransferBytes(Stream input, Stream output) {
            byte[] buffer = new byte[TRANSFER_SIZE];
            for (; ; ) {
                int len = input.JRead(buffer, 0, TRANSFER_SIZE);
                if (len > 0) {
                    output.Write(buffer, 0, len);
                }
                else {
                    break;
                }
            }
        }

        public static void TransferBytes(RandomAccessFileOrArray input, Stream output) {
            byte[] buffer = new byte[TRANSFER_SIZE];
            for (; ; ) {
                int len = input.Read(buffer, 0, TRANSFER_SIZE);
                if (len > 0) {
                    output.Write(buffer, 0, len);
                }
                else {
                    break;
                }
            }
        }

        /// <summary>Reads the full content of a stream and returns them in a byte array</summary>
        /// <param name="stream">the stream to read</param>
        /// <returns>a byte array containing all of the bytes from the stream</returns>
        public static byte[] InputStreamToArray(Stream stream) {
            byte[] b = new byte[8192];
            MemoryStream output = new MemoryStream();
            while (true) {
                int read = stream.Read(b);
                if (read < 1) {
                    break;
                }
                output.Write(b, 0, read);
            }
            output.Dispose();
            return output.ToArray();
        }

        /// <summary>
        /// Copy bytes from the
        /// <c>RandomAccessSource</c>
        /// to
        /// <c>OutputStream</c>.
        /// </summary>
        /// <param name="source">
        /// the
        /// <c>RandomAccessSource</c>
        /// copy from.
        /// </param>
        /// <param name="start">start position of source copy from.</param>
        /// <param name="length">length copy to.</param>
        /// <param name="output">
        /// the
        /// <c>OutputStream</c>
        /// copy to.
        /// </param>
        public static void CopyBytes(IRandomAccessSource source, long start, long length, Stream output) {
            if (length <= 0) {
                return;
            }
            long idx = start;
            byte[] buf = new byte[8192];
            while (length > 0) {
                long n = source.Get(idx, buf, 0, (int)Math.Min((long)buf.Length, length));
                if (n <= 0) {
                    throw new EndOfStreamException();
                }
                output.Write(buf, 0, (int)n);
                idx += n;
                length -= n;
            }
        }

        /// <summary>
        /// Reads
        /// <paramref name="len"/>
        /// bytes from an input stream.
        /// </summary>
        /// <param name="input">the stream to read</param>
        /// <param name="b">the buffer into which the data is read.</param>
        /// <param name="off">an int specifying the offset into the data.</param>
        /// <param name="len">an int specifying the number of bytes to read.</param>
        public static void ReadFully(Stream input, byte[] b, int off, int len) {
            if (len < 0) {
                throw new IndexOutOfRangeException();
            }
            int n = 0;
            while (n < len) {
                int count = input.JRead(b, off + n, len - n);
                if (count < 0) {
                    throw new EndOfStreamException();
                }
                n += count;
            }
        }
    }
}
