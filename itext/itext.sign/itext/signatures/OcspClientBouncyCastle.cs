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
using Microsoft.Extensions.Logging;
using iText.Bouncycastleconnector;
using iText.Commons;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Math;
using iText.IO.Util;

namespace iText.Signatures {
    /// <summary>OcspClient implementation using BouncyCastle.</summary>
    public class OcspClientBouncyCastle : IOcspClientBouncyCastle {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        /// <summary>The Logger instance.</summary>
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Signatures.OcspClientBouncyCastle
            ));

        /// <summary>
        /// Creates new
        /// <see cref="OcspClientBouncyCastle"/>
        /// instance.
        /// </summary>
        public OcspClientBouncyCastle() {
        }

        // Empty constructor in order for default one to not be removed if another one is added.
        /// <summary><inheritDoc/></summary>
        public virtual IBasicOcspResponse GetBasicOCSPResp(IX509Certificate checkCert, IX509Certificate rootCert, 
            String url) {
            try {
                IOcspResponse ocspResponse = GetOcspResponse(checkCert, rootCert, url);
                if (ocspResponse == null) {
                    return null;
                }
                if (ocspResponse.GetStatus() != BOUNCY_CASTLE_FACTORY.CreateOCSPResponseStatus().GetSuccessful()) {
                    return null;
                }
                return BOUNCY_CASTLE_FACTORY.CreateBasicOCSPResponse(ocspResponse.GetResponseObject());
            }
            catch (Exception ex) {
                LOGGER.LogError(ex.Message);
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded(IX509Certificate checkCert, IX509Certificate rootCert, String url) {
            try {
                IBasicOcspResponse basicResponse = GetBasicOCSPResp(checkCert, rootCert, url);
                if (basicResponse != null) {
                    ISingleResponse[] responses = basicResponse.GetResponses();
                    if (responses.Length == 1) {
                        ISingleResponse resp = responses[0];
                        ICertStatus status = resp.GetCertStatus();
                        if (!BOUNCY_CASTLE_FACTORY.CreateCertificateStatus().GetGood().Equals(status)) {
                            if (BOUNCY_CASTLE_FACTORY.CreateRevokedStatus(status) == null) {
                                LOGGER.LogInformation(iText.IO.Logs.IoLogMessageConstant.OCSP_STATUS_IS_UNKNOWN);
                            }
                            else {
                                LOGGER.LogInformation(iText.IO.Logs.IoLogMessageConstant.OCSP_STATUS_IS_REVOKED);
                            }
                        }
                        return basicResponse.GetEncoded();
                    }
                }
            }
            catch (Exception ex) {
                LOGGER.LogError(ex.Message);
            }
            return null;
        }

        /// <summary>Generates an OCSP request using BouncyCastle.</summary>
        /// <param name="issuerCert">certificate of the issues</param>
        /// <param name="serialNumber">serial number</param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.Ocsp.IOcspRequest"/>
        /// an OCSP request wrapper
        /// </returns>
        protected internal static IOcspRequest GenerateOCSPRequest(IX509Certificate issuerCert, IBigInteger serialNumber
            ) {
            // Generate the id for the certificate we are looking for
            ICertID id = SignUtils.GenerateCertificateId(issuerCert, serialNumber, BOUNCY_CASTLE_FACTORY.CreateCertificateID
                ().GetHashSha1());
            // basic request generation with nonce
            return SignUtils.GenerateOcspRequestWithNonce(id);
        }

        /// <summary>Retrieves certificate status from the OCSP response.</summary>
        /// <param name="basicOcspRespBytes">encoded basic OCSP response</param>
        /// <returns>good, revoked or unknown certificate status retrieved from the OCSP response, or null if an error occurs.
        ///     </returns>
        protected internal static ICertStatus GetCertificateStatus(byte[] basicOcspRespBytes) {
            try {
                IBasicOcspResponse basicResponse = BOUNCY_CASTLE_FACTORY.CreateBasicOCSPResponse(BOUNCY_CASTLE_FACTORY.CreateASN1Primitive
                    (basicOcspRespBytes));
                if (basicResponse != null) {
                    ISingleResponse[] responses = basicResponse.GetResponses();
                    if (responses.Length >= 1) {
                        return responses[0].GetCertStatus();
                    }
                }
            }
            catch (Exception) {
            }
            // Ignore exception.
            return null;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Gets an OCSP response object using BouncyCastle.</summary>
        /// <param name="checkCert">to certificate to check</param>
        /// <param name="rootCert">the parent certificate</param>
        /// <param name="url">
        /// to get the verification. If it's null it will be taken
        /// from the check cert or from other implementation specific source
        /// </param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IOcspResponse"/>
        /// an OCSP response wrapper
        /// </returns>
        internal virtual IOcspResponse GetOcspResponse(IX509Certificate checkCert, IX509Certificate rootCert, String
             url) {
            if (checkCert == null || rootCert == null) {
                return null;
            }
            if (url == null) {
                url = CertificateUtil.GetOCSPURL(checkCert);
            }
            if (url == null) {
                return null;
            }
            Stream @in = CreateRequestAndResponse(checkCert, rootCert, url);
            return @in == null ? null : BOUNCY_CASTLE_FACTORY.CreateOCSPResponse(StreamUtil.InputStreamToArray(@in));
        }
//\endcond

        /// <summary>
        /// Create OCSP request and get the response for this request, represented as
        /// <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="checkCert">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// certificate to get OCSP response for
        /// </param>
        /// <param name="rootCert">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// root certificate from which OCSP request will be built
        /// </param>
        /// <param name="url">
        /// 
        /// <see cref="System.Uri"/>
        /// link, which is expected to be used to get OCSP response from
        /// </param>
        /// <returns>
        /// OCSP response bytes, represented as
        /// <see cref="System.IO.Stream"/>
        /// </returns>
        protected internal virtual Stream CreateRequestAndResponse(IX509Certificate checkCert, IX509Certificate rootCert
            , String url) {
            LOGGER.LogInformation("Getting OCSP from " + url);
            IOcspRequest request = GenerateOCSPRequest(rootCert, checkCert.GetSerialNumber());
            byte[] array = request.GetEncoded();
            Uri urlt = new Uri(url);
            return SignUtils.GetHttpResponseForOcspRequest(array, urlt);
        }
    }
}
