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
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Mac;
using iText.Kernel.Pdf.Event;
using iText.Kernel.Utils;

namespace iText.Kernel.Pdf {
    /// <summary>Writes the PDF to the specified output.</summary>
    /// <remarks>
    /// Writes the PDF to the specified output. Writing can be customized using
    /// <see cref="WriterProperties"/>.
    /// </remarks>
    public class PdfWriter : PdfOutputStream {
        private static readonly byte[] OBJ = ByteUtils.GetIsoBytes(" obj\n");

        private static readonly byte[] ENDOBJ = ByteUtils.GetIsoBytes("\nendobj\n");

        protected internal WriterProperties properties;

        //forewarned is forearmed
        protected internal bool isUserWarnedAboutAcroFormCopying;

//\cond DO_NOT_DOCUMENT
        /// <summary>Currently active object stream.</summary>
        /// <remarks>
        /// Currently active object stream.
        /// Objects are written to the object stream if fullCompression set to true.
        /// </remarks>
        internal PdfObjectStream objectStream = null;
//\endcond

        /// <summary>Is used to avoid duplications on object copying.</summary>
        /// <remarks>
        /// Is used to avoid duplications on object copying.
        /// It stores hashes of the indirect reference from the source document and the corresponding
        /// indirect references of the copied objects from the new document.
        /// </remarks>
        private readonly IDictionary<PdfIndirectReference, PdfIndirectReference> copiedObjects = new LinkedDictionary
            <PdfIndirectReference, PdfIndirectReference>();

        /// <summary>Is used in smart mode to serialize and store serialized objects content.</summary>
        private readonly SmartModePdfObjectsSerializer smartModeSerializer = new SmartModePdfObjectsSerializer();

        private Stream originalOutputStream;

        /// <summary>Create a PdfWriter writing to the passed File and with default writer properties.</summary>
        /// <param name="file">File to write to.</param>
        public PdfWriter(FileInfo file)
            : this(file.FullName) {
        }

        /// <summary>Create a PdfWriter writing to the passed outputstream and with default writer properties.</summary>
        /// <param name="os">Outputstream to write to.</param>
        public PdfWriter(Stream os)
            : this(os, new WriterProperties()) {
        }

        /// <summary>
        /// Creates
        /// <see cref="PdfWriter"/>
        /// instance, which writes to the passed
        /// <see cref="System.IO.Stream"/>
        /// ,
        /// using provided
        /// <see cref="WriterProperties"/>.
        /// </summary>
        /// <param name="os">
        /// 
        /// <see cref="System.IO.Stream"/>
        /// in which writing should happen
        /// </param>
        /// <param name="properties">
        /// 
        /// <see cref="WriterProperties"/>
        /// to be used during the writing
        /// </param>
        public PdfWriter(Stream os, WriterProperties properties)
            : base(new CountOutputStream(FileUtil.WrapWithBufferedOutputStream(os))) {
            this.properties = properties;
        }

        /// <summary>Create a PdfWriter writing to the passed filename and with default writer properties.</summary>
        /// <param name="filename">filename of the resulting pdf.</param>
        public PdfWriter(String filename)
            : this(filename, new WriterProperties()) {
        }

        /// <summary>Create a PdfWriter writing to the passed filename and using the passed writer properties.</summary>
        /// <param name="filename">filename of the resulting pdf.</param>
        /// <param name="properties">writerproperties to use.</param>
        public PdfWriter(String filename, WriterProperties properties)
            : this(FileUtil.GetBufferedOutputStream(filename), properties) {
        }

        /// <summary>Indicates if to use full compression mode.</summary>
        /// <returns>true if to use full compression, false otherwise.</returns>
        public virtual bool IsFullCompression() {
            return properties.isFullCompression != null && (bool)properties.isFullCompression;
        }

        /// <summary>Gets default compression level for @see PdfStream.</summary>
        /// <remarks>
        /// Gets default compression level for @see PdfStream.
        /// For more details @see
        /// <see cref="iText.IO.Source.DeflaterOutputStream"/>.
        /// </remarks>
        /// <returns>compression level.</returns>
        public virtual int GetCompressionLevel() {
            return properties.compressionLevel;
        }

        /// <summary>Sets default compression level for @see PdfStream.</summary>
        /// <remarks>
        /// Sets default compression level for @see PdfStream.
        /// For more details @see
        /// <see cref="iText.IO.Source.DeflaterOutputStream"/>.
        /// </remarks>
        /// <param name="compressionLevel">compression level.</param>
        /// <returns>
        /// this
        /// <see cref="PdfWriter"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfWriter SetCompressionLevel(int compressionLevel) {
            this.properties.SetCompressionLevel(compressionLevel);
            return this;
        }

        /// <summary>Gets defined pdf version for the document.</summary>
        /// <returns>version for the document</returns>
        public virtual PdfVersion GetPdfVersion() {
            return properties.pdfVersion;
        }

        /// <summary>Gets the writer properties.</summary>
        /// <returns>
        /// The
        /// <see cref="WriterProperties"/>
        /// of the current PdfWriter.
        /// </returns>
        public virtual WriterProperties GetProperties() {
            return properties;
        }

        /// <summary>Sets the smart mode.</summary>
        /// <remarks>
        /// Sets the smart mode.
        /// <br />
        /// In smart mode when resources (such as fonts, images,...) are
        /// encountered, a reference to these resources is saved
        /// in a cache, so that they can be reused.
        /// This requires more memory, but reduces the file size
        /// of the resulting PDF document.
        /// </remarks>
        /// <param name="smartMode">True for enabling smart mode.</param>
        /// <returns>
        /// this
        /// <see cref="PdfWriter"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfWriter SetSmartMode(bool smartMode) {
            this.properties.smartMode = smartMode;
            return this;
        }

        /// <summary>
        /// Initializes
        /// <see cref="PdfEncryption"/>
        /// object if any encryption is specified in
        /// <see cref="WriterProperties"/>.
        /// </summary>
        /// <param name="version">
        /// 
        /// <see cref="PdfVersion"/>
        /// version of the document in question
        /// </param>
        protected internal virtual void InitCryptoIfSpecified(PdfVersion version) {
            EncryptionProperties encryptProps = properties.encryptionProperties;
            // Suppress MAC properties for PDF version < 2.0 and old deprecated encryption algorithms
            // if default ones have been passed to WriterProperties
            int encryptionAlgorithm = crypto == null ? (encryptProps.encryptionAlgorithm & EncryptionConstants.ENCRYPTION_MASK
                ) : crypto.GetEncryptionAlgorithm();
            if (document.properties.disableMac) {
                encryptProps.macProperties = null;
            }
            if (encryptProps.macProperties == EncryptionProperties.DEFAULT_MAC_PROPERTIES) {
                if (version == null || version.CompareTo(PdfVersion.PDF_2_0) < 0 || encryptionAlgorithm < EncryptionConstants
                    .ENCRYPTION_AES_256) {
                    encryptProps.macProperties = null;
                }
            }
            AbstractMacIntegrityProtector mac = encryptProps.macProperties == null ? null : document.GetDiContainer().
                GetInstance<IMacContainerLocator>().CreateMacIntegrityProtector(document, encryptProps.macProperties);
            if (properties.IsStandardEncryptionUsed()) {
                crypto = new PdfEncryption(encryptProps.userPassword, encryptProps.ownerPassword, encryptProps.standardEncryptPermissions
                    , encryptProps.encryptionAlgorithm, ByteUtils.GetIsoBytes(this.document.GetOriginalDocumentId().GetValue
                    ()), version, mac);
            }
            else {
                if (properties.IsPublicKeyEncryptionUsed()) {
                    crypto = new PdfEncryption(encryptProps.publicCertificates, encryptProps.publicKeyEncryptPermissions, encryptProps
                        .encryptionAlgorithm, version, mac);
                }
            }
        }

        /// <summary>Flushes the object.</summary>
        /// <remarks>Flushes the object. Override this method if you want to define custom behaviour for object flushing.
        ///     </remarks>
        /// <param name="pdfObject">object to flush.</param>
        /// <param name="canBeInObjStm">indicates whether object can be placed into object stream.</param>
        protected internal virtual void FlushObject(PdfObject pdfObject, bool canBeInObjStm) {
            PdfIndirectReference indirectReference = pdfObject.GetIndirectReference();
            if (IsFullCompression() && canBeInObjStm) {
                PdfObjectStream objectStream = GetObjectStream();
                objectStream.AddObject(pdfObject);
            }
            else {
                indirectReference.SetOffset(GetCurrentPos());
                WriteToBody(pdfObject);
            }
            indirectReference.SetState(PdfObject.FLUSHED).ClearState(PdfObject.MUST_BE_FLUSHED);
            switch (pdfObject.GetObjectType()) {
                case PdfObject.BOOLEAN:
                case PdfObject.NAME:
                case PdfObject.NULL:
                case PdfObject.NUMBER:
                case PdfObject.STRING: {
                    ((PdfPrimitiveObject)pdfObject).content = null;
                    break;
                }

                case PdfObject.ARRAY: {
                    PdfArray array = ((PdfArray)pdfObject);
                    MarkArrayContentToFlush(array);
                    array.ReleaseContent();
                    break;
                }

                case PdfObject.STREAM:
                case PdfObject.DICTIONARY: {
                    PdfDictionary dictionary = ((PdfDictionary)pdfObject);
                    MarkDictionaryContentToFlush(dictionary);
                    dictionary.ReleaseContent();
                    break;
                }

                case PdfObject.INDIRECT_REFERENCE: {
                    MarkObjectToFlush(((PdfIndirectReference)pdfObject).GetRefersTo(false));
                    break;
                }
            }
        }

        /// <summary>Copies a PdfObject either stand alone or as part of the PdfDocument passed as documentTo.</summary>
        /// <param name="obj">object to copy</param>
        /// <param name="documentTo">optional target document</param>
        /// <param name="allowDuplicating">allow that some objects will become duplicated by this action</param>
        /// <returns>the copies object</returns>
        protected internal virtual PdfObject CopyObject(PdfObject obj, PdfDocument documentTo, bool allowDuplicating
            ) {
            return CopyObject(obj, documentTo, allowDuplicating, NullCopyFilter.GetInstance());
        }

        /// <summary>Copies a PdfObject either stand alone or as part of the PdfDocument passed as documentTo.</summary>
        /// <param name="obj">object to copy</param>
        /// <param name="documentTo">optional target document</param>
        /// <param name="allowDuplicating">allow that some objects will become duplicated by this action</param>
        /// <param name="copyFilter">
        /// 
        /// <see cref="iText.Kernel.Utils.ICopyFilter"/>
        /// a filter to apply while copying arrays and dictionaries
        /// *             Use
        /// <see cref="iText.Kernel.Utils.NullCopyFilter"/>
        /// for no filtering
        /// </param>
        /// <returns>the copies object</returns>
        protected internal virtual PdfObject CopyObject(PdfObject obj, PdfDocument documentTo, bool allowDuplicating
            , ICopyFilter copyFilter) {
            if (obj is PdfIndirectReference) {
                obj = ((PdfIndirectReference)obj).GetRefersTo();
            }
            if (obj == null) {
                obj = PdfNull.PDF_NULL;
            }
            if (CheckTypeOfPdfDictionary(obj, PdfName.Catalog)) {
                ILogger logger = ITextLogManager.GetLogger(typeof(PdfReader));
                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.MAKE_COPY_OF_CATALOG_DICTIONARY_IS_FORBIDDEN);
                obj = PdfNull.PDF_NULL;
            }
            PdfIndirectReference indirectReference = obj.GetIndirectReference();
            bool tryToFindDuplicate = !allowDuplicating && indirectReference != null;
            if (tryToFindDuplicate) {
                PdfIndirectReference copiedIndirectReference = copiedObjects.Get(indirectReference);
                if (copiedIndirectReference != null) {
                    return copiedIndirectReference.GetRefersTo();
                }
            }
            SerializedObjectContent serializedContent = null;
            if (properties.smartMode && tryToFindDuplicate && !CheckTypeOfPdfDictionary(obj, PdfName.Page) && !CheckTypeOfPdfDictionary
                (obj, PdfName.OCG) && !CheckTypeOfPdfDictionary(obj, PdfName.OCMD)) {
                serializedContent = smartModeSerializer.SerializeObject(obj);
                PdfIndirectReference objectRef = smartModeSerializer.GetSavedSerializedObject(serializedContent);
                if (objectRef != null) {
                    copiedObjects.Put(indirectReference, objectRef);
                    return objectRef.refersTo;
                }
            }
            PdfObject newObject = obj.NewInstance();
            if (indirectReference != null) {
                PdfIndirectReference indRef = newObject.MakeIndirect(documentTo).GetIndirectReference();
                if (serializedContent != null) {
                    smartModeSerializer.SaveSerializedObject(serializedContent, indRef);
                }
                copiedObjects.Put(indirectReference, indRef);
            }
            newObject.CopyContent(obj, documentTo, copyFilter);
            return newObject;
        }

        /// <summary>Writes object to body of PDF document.</summary>
        /// <param name="pdfObj">object to write.</param>
        protected internal virtual void WriteToBody(PdfObject pdfObj) {
            if (crypto != null) {
                crypto.SetHashKeyForNextObject(pdfObj.GetIndirectReference().GetObjNumber(), pdfObj.GetIndirectReference()
                    .GetGenNumber());
            }
            WriteInteger(pdfObj.GetIndirectReference().GetObjNumber()).WriteSpace().WriteInteger(pdfObj.GetIndirectReference
                ().GetGenNumber()).WriteBytes(OBJ);
            Write(pdfObj);
            WriteBytes(ENDOBJ);
        }

        /// <summary>Writes PDF header.</summary>
        protected internal virtual void WriteHeader() {
            WriteByte('%').WriteString(document.GetPdfVersion().ToString()).WriteString("\n%\u00e2\u00e3\u00cf\u00d3\n"
                );
        }

        /// <summary>Flushes all objects which have not been flushed yet.</summary>
        /// <param name="forbiddenToFlush">
        /// a
        /// <see cref="Java.Util.Set{E}"/>
        /// of
        /// <see cref="PdfIndirectReference">references</see>
        /// that are forbidden to be flushed
        /// automatically.
        /// </param>
        protected internal virtual void FlushWaitingObjects(ICollection<PdfIndirectReference> forbiddenToFlush) {
            PdfXrefTable xref = document.GetXref();
            bool needFlush = true;
            while (needFlush) {
                needFlush = false;
                for (int i = 1; i < xref.Size(); i++) {
                    PdfIndirectReference indirectReference = xref.Get(i);
                    if (indirectReference != null && !indirectReference.IsFree() && indirectReference.CheckState(PdfObject.MUST_BE_FLUSHED
                        ) && !forbiddenToFlush.Contains(indirectReference)) {
                        PdfObject obj = indirectReference.GetRefersTo(false);
                        if (obj != null) {
                            obj.Flush();
                            needFlush = true;
                        }
                    }
                }
            }
            if (objectStream != null && objectStream.GetSize() > 0) {
                objectStream.Flush();
                objectStream = null;
            }
        }

        /// <summary>Flushes all modified objects which have not been flushed yet.</summary>
        /// <remarks>Flushes all modified objects which have not been flushed yet. Used in case incremental updates.</remarks>
        /// <param name="forbiddenToFlush">
        /// a
        /// <see cref="Java.Util.Set{E}"/>
        /// of
        /// <see cref="PdfIndirectReference">references</see>
        /// that are forbidden to be flushed
        /// automatically.
        /// </param>
        protected internal virtual void FlushModifiedWaitingObjects(ICollection<PdfIndirectReference> forbiddenToFlush
            ) {
            PdfXrefTable xref = document.GetXref();
            for (int i = 1; i < xref.Size(); i++) {
                PdfIndirectReference indirectReference = xref.Get(i);
                if (null != indirectReference && !indirectReference.IsFree() && !forbiddenToFlush.Contains(indirectReference
                    )) {
                    bool isModified = indirectReference.CheckState(PdfObject.MODIFIED);
                    if (isModified) {
                        PdfObject obj = indirectReference.GetRefersTo(false);
                        if (obj != null) {
                            if (!obj.Equals(objectStream)) {
                                obj.Flush();
                            }
                        }
                    }
                }
            }
            if (objectStream != null && objectStream.GetSize() > 0) {
                objectStream.Flush();
                objectStream = null;
            }
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void Finish() {
            if (document != null && !document.IsClosed()) {
                // Writer is always closed as part of document closing
                document.DispatchEvent(new PdfDocumentEvent(PdfDocumentEvent.START_WRITER_CLOSING));
                if (IsByteArrayWritingMode()) {
                    CompleteByteArrayWritingMode();
                }
            }
            Dispose();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Gets the current object stream.</summary>
        /// <returns>object stream.</returns>
        internal virtual PdfObjectStream GetObjectStream() {
            if (!IsFullCompression()) {
                return null;
            }
            if (objectStream == null) {
                objectStream = new PdfObjectStream(document);
            }
            else {
                if (objectStream.GetSize() == PdfObjectStream.MAX_OBJ_STREAM_SIZE) {
                    objectStream.Flush();
                    objectStream = new PdfObjectStream(objectStream);
                }
            }
            return objectStream;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Flush all copied objects.</summary>
        /// <param name="docId">id of the source document</param>
        internal virtual void FlushCopiedObjects(long docId) {
            IList<PdfIndirectReference> remove = new List<PdfIndirectReference>();
            foreach (KeyValuePair<PdfIndirectReference, PdfIndirectReference> copiedObject in copiedObjects) {
                PdfDocument document = copiedObject.Key.GetDocument();
                if (document != null && document.GetDocumentId() == docId) {
                    if (copiedObject.Value.refersTo != null) {
                        copiedObject.Value.refersTo.Flush();
                        remove.Add(copiedObject.Key);
                    }
                }
            }
            foreach (PdfIndirectReference ird in remove) {
                copiedObjects.JRemove(ird);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void EnableByteArrayWritingMode() {
            if (IsByteArrayWritingMode()) {
                throw new PdfException("Byte array writing mode is already enabled");
            }
            else {
                this.originalOutputStream = this.outputStream;
                this.outputStream = new ByteArrayOutputStream();
            }
        }
//\endcond

        private void CompleteByteArrayWritingMode() {
            byte[] baos = ((ByteArrayOutputStream)GetOutputStream()).ToArray();
            originalOutputStream.Write(baos, 0, baos.Length);
            originalOutputStream.Dispose();
        }

        private bool IsByteArrayWritingMode() {
            return originalOutputStream != null;
        }

        private void MarkArrayContentToFlush(PdfArray array) {
            for (int i = 0; i < array.Size(); i++) {
                MarkObjectToFlush(array.Get(i, false));
            }
        }

        private void MarkDictionaryContentToFlush(PdfDictionary dictionary) {
            foreach (PdfObject item in dictionary.Values(false)) {
                MarkObjectToFlush(item);
            }
        }

        private void MarkObjectToFlush(PdfObject pdfObject) {
            if (pdfObject != null) {
                PdfIndirectReference indirectReference = pdfObject.GetIndirectReference();
                if (indirectReference != null) {
                    if (!indirectReference.CheckState(PdfObject.FLUSHED)) {
                        indirectReference.SetState(PdfObject.MUST_BE_FLUSHED);
                    }
                }
                else {
                    if (pdfObject.GetObjectType() == PdfObject.INDIRECT_REFERENCE) {
                        if (!pdfObject.CheckState(PdfObject.FLUSHED)) {
                            pdfObject.SetState(PdfObject.MUST_BE_FLUSHED);
                        }
                    }
                    else {
                        if (pdfObject.GetObjectType() == PdfObject.ARRAY) {
                            MarkArrayContentToFlush((PdfArray)pdfObject);
                        }
                        else {
                            if (pdfObject.GetObjectType() == PdfObject.DICTIONARY) {
                                MarkDictionaryContentToFlush((PdfDictionary)pdfObject);
                            }
                        }
                    }
                }
            }
        }

        private static bool CheckTypeOfPdfDictionary(PdfObject dictionary, PdfName expectedType) {
            return dictionary.IsDictionary() && expectedType.Equals(((PdfDictionary)dictionary).GetAsName(PdfName.Type
                ));
        }
    }
}
