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
using System.Collections.Generic;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class CertificateInfoTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String EXPECTED_EXCEPTION_MESSAGE = FACTORY.GetBouncyCastleFactoryTestUtil().GetCertificateInfoTestConst
            ();

        private const String ENCODED_DN = "MD0xCzAJBgNVBAYMAkJFMQ4wDAYDVQQKDAVpVGV4dDEeMBwGA1UEAwwVaVRleHRUZXN0SW50ZXJtZWRpYXRl";

        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures" + "/CertificateInfoTest/";

        [NUnit.Framework.Test]
        public virtual void X500InvalidDirectoryConstructorTest() {
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new CertificateInfo.X500Name("some_dir"));
        }

        [NUnit.Framework.Test]
        public virtual void X500ValidDirectoryConstructorTest() {
            CertificateInfo.X500Name name = new CertificateInfo.X500Name("some=dir,another=dir,some=value");
            IDictionary<String, IList<String>> values = name.GetFields();
            NUnit.Framework.Assert.AreEqual(2, values.Count);
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("dir", "value"), values.Get("SOME"));
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.SingletonList("dir"), values.Get("ANOTHER"));
        }

        [NUnit.Framework.Test]
        public virtual void X500GetFieldTest() {
            CertificateInfo.X500Name name = new CertificateInfo.X500Name("some=value,another=dir,some=dir");
            NUnit.Framework.Assert.AreEqual("value", name.GetField("SOME"));
            NUnit.Framework.Assert.AreEqual("dir", name.GetField("ANOTHER"));
        }

        [NUnit.Framework.Test]
        public virtual void X500GetFieldArrayTest() {
            CertificateInfo.X500Name name = new CertificateInfo.X500Name("some=value,another=dir,some=dir");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("value", "dir"), name.GetFieldArray("SOME"));
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.SingletonList("dir"), name.GetFieldArray("ANOTHER"));
        }

        [NUnit.Framework.Test]
        public virtual void X509NameTokenizerNextTokenComplicatedTest() {
            CertificateInfo.X509NameTokenizer tokenizer = new CertificateInfo.X509NameTokenizer("quoted\",\"comma=escaped\\,comma_escaped\\\"quote"
                );
            String token = tokenizer.NextToken();
            NUnit.Framework.Assert.AreEqual("quoted,comma=escaped,comma_escaped\"quote", token);
            NUnit.Framework.Assert.IsNull(tokenizer.NextToken());
        }

        [NUnit.Framework.Test]
        public virtual void GetIssuerFieldsExceptionTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => CertificateInfo.GetIssuer(new 
                byte[] { 4, 8, 15, 16, 23, 42 }));
            NUnit.Framework.Assert.AreEqual(EXPECTED_EXCEPTION_MESSAGE, exception.InnerException.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetSubjectExceptionTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => CertificateInfo.GetSubject(
                new byte[] { 4, 8, 15, 16, 23, 42 }));
            NUnit.Framework.Assert.AreEqual(EXPECTED_EXCEPTION_MESSAGE, exception.InnerException.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DistinguishedNameEncodingAndComparisonTest() {
            IX509Certificate cert = (IX509Certificate)PemFileHelper.ReadFirstChain(CERTS_SRC + "intermediate.pem")[0];
            IX500Name name = FACTORY.CreateX500Name(cert);
            IX500Name differentlyEncodedName = FACTORY.CreateX500Name(FACTORY.CreateASN1Sequence(Convert.FromBase64String
                (ENCODED_DN)));
            NUnit.Framework.Assert.IsTrue(differentlyEncodedName.Equals(name));
            NUnit.Framework.Assert.IsTrue(name.Equals(differentlyEncodedName));
        }

        [NUnit.Framework.Test]
        public virtual void DistinguishedNameEncodingAndComparisonIntegrationTest() {
            NUnit.Framework.Assert.DoesNotThrow(() => {
                PdfDocument doc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "signatureWithNameEncodingDifferences.pdf"
                    ));
                SignatureUtil signUtil = new SignatureUtil(doc);
                IList<String> signNames = signUtil.GetSignatureNames();
                PdfPKCS7 pkcs7 = signUtil.ReadSignatureData(signNames[0]);
                NUnit.Framework.Assert.IsNotNull(pkcs7);
            }
            );
        }
    }
}
