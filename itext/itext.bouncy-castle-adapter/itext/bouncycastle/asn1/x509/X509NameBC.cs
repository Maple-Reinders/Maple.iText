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

using iText.Commons.Bouncycastle.Asn1.X500;
using Org.BouncyCastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.X509Name"/>.
    /// </summary>
    public class X509NameBC : Asn1EncodableBC, IX500Name {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.X509Name"/>.
        /// </summary>
        /// <param name="x500Name">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.X509Name"/>
        /// to be wrapped
        /// </param>
        public X509NameBC(X509Name x500Name)
            : base(x500Name) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.X509Name"/>.
        /// </returns>
        public virtual X509Name GetX509Name() {
            return (X509Name)GetEncodable();
        }

        public string GetName()
        {
            return GetX509Name().ToString();
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || !this.GetType().IsAssignableFrom(o.GetType()))
            {
                return false;
            }
            X509NameBC that = (X509NameBC)o;
            return GetX509Name().Equivalent(that.GetX509Name());
        }
    }
}
