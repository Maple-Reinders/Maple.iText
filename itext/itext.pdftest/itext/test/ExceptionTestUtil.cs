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
namespace iText.Test
{
    /// <summary>
    /// Class containing the exception messages.
    /// </summary>
    public static class ExceptionTestUtil
    {
        private static string docTypeIsDisallowed =
            "For security reasons DTD is prohibited in this XML document. " +
            "To enable DTD processing set the DtdProcessing property on XmlReaderSettings " +
            "to Parse and pass the settings into XmlReader.Create method.";

        private const string XXE_TEST_MESSAGE = "An error has occurred while opening external entity 'file:///D:/abc.txt': Test message";

        /// <summary>
        /// Returns message about disallowed DOCTYPE.
        /// </summary>
        /// <returns>Message for case when DOCTYPE is disallowed in XML</returns>
        public static string GetDoctypeIsDisallowedExceptionMessage()
        {
            return docTypeIsDisallowed;
        }

        /// <summary>
        /// Returns test message for case with XXE.
        /// </summary>
        /// <returns>Message with text for testing</returns>
        public static string GetXxeTestMessage()
        {
            return XXE_TEST_MESSAGE;
        }

        public static void SetDoctypeIsDisallowedExceptionMessage(string docTypeIsDisallowedString)
        {
            docTypeIsDisallowed = docTypeIsDisallowedString;
        }
    }
}
