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
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X509;
using iText.Bouncycastle.Math;
using iText.Bouncycastle.Operator;
using iText.Bouncycastle.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;
using Org.BouncyCastle.X509;

namespace iText.Bouncycastle.Cert {
    /// <summary>
    /// Wrapper class for
    /// <see cref="X509V2CrlGenerator"/>.
    /// </summary>
    public class X509V2CrlGeneratorBC : IX509V2CrlGenerator {
        private readonly X509V2CrlGenerator builder;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="X509V2CrlGenerator"/>.
        /// </summary>
        /// <param name="builder">
        /// 
        /// <see cref="X509V2CrlGenerator"/>
        /// to be wrapped
        /// </param>
        public X509V2CrlGeneratorBC(X509V2CrlGenerator builder) {
            this.builder = builder;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="X509V2CrlGenerator"/>.
        /// </summary>
        /// <param name="x500Name">
        /// X500Name wrapper to create
        /// <see cref="X509V2CrlGenerator"/>
        /// </param>
        /// <param name="date">
        /// Date to create
        /// <see cref="X509V2CrlGenerator"/>
        /// </param>
        public X509V2CrlGeneratorBC(IX500Name x500Name, DateTime date) {
            builder = new X509V2CrlGenerator();
            builder.SetIssuerDN(((X509NameBC)x500Name).GetX509Name());
            builder.SetThisUpdate(date);
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="X509V2CrlGenerator"/>.
        /// </returns>
        public virtual X509V2CrlGenerator GetBuilder() {
            return builder;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX509V2CrlGenerator AddCRLEntry(IBigInteger bigInteger, DateTime date, int i) {
            builder.AddCrlEntry(((BigIntegerBC)bigInteger).GetBigInteger(), date, i);
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public IX509V2CrlGenerator AddExtension(IDerObjectIdentifier objectIdentifier, bool isCritical, 
            IAsn1Encodable extension) {
            builder.AddExtension(((DerObjectIdentifierBC) objectIdentifier).GetDerObjectIdentifier(), isCritical,
                ((Asn1EncodableBC) extension).GetEncodable());
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX509V2CrlGenerator SetNextUpdate(DateTime nextUpdate) {
            builder.SetNextUpdate(nextUpdate);
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX509Crl Build(IContentSigner signer) {
            return new X509CrlBC(builder.Generate(((ContentSignerBC)signer).GetContentSigner()));
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cert.X509V2CrlGeneratorBC that = (iText.Bouncycastle.Cert.X509V2CrlGeneratorBC)o;
            return Object.Equals(builder, that.builder);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(builder);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return builder.ToString();
        }
    }
}
