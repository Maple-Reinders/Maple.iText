/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Cms.EncryptedContentInfo"/>.
    /// </summary>
    public class EncryptedContentInfoBC : ASN1EncodableBC, IEncryptedContentInfo {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.EncryptedContentInfo"/>.
        /// </summary>
        /// <param name="encryptedContentInfo">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Cms.EncryptedContentInfo"/>
        /// to be wrapped
        /// </param>
        public EncryptedContentInfoBC(EncryptedContentInfo encryptedContentInfo)
            : base(encryptedContentInfo) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.EncryptedContentInfo"/>.
        /// </summary>
        /// <param name="data">ASN1ObjectIdentifier wrapper</param>
        /// <param name="algorithmIdentifier">AlgorithmIdentifier wrapper</param>
        /// <param name="octetString">ASN1OctetString wrapper</param>
        public EncryptedContentInfoBC(IASN1ObjectIdentifier data, IAlgorithmIdentifier algorithmIdentifier, IASN1OctetString
             octetString)
            : base(new EncryptedContentInfo(((ASN1ObjectIdentifierBC)data).GetASN1ObjectIdentifier(), ((AlgorithmIdentifierBC
                )algorithmIdentifier).GetAlgorithmIdentifier(), ((ASN1OctetStringBC)octetString).GetASN1OctetString())
                ) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Cms.EncryptedContentInfo"/>.
        /// </returns>
        public virtual EncryptedContentInfo GetEncryptedContentInfo() {
            return (EncryptedContentInfo)GetEncodable();
        }
    }
}