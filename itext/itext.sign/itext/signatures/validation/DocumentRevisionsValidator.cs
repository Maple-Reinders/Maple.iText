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
using System.Linq;
using iText.Commons.Actions.Contexts;
using iText.Commons.Datastructures;
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation {
    /// <summary>Validator, which is responsible for document revisions validation according to doc-MDP and field-MDP rules.
    ///     </summary>
    public class DocumentRevisionsValidator {
//\cond DO_NOT_DOCUMENT
        internal const String DOC_MDP_CHECK = "DocMDP check.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String FIELD_MDP_CHECK = "FieldMDP check.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String ACCESS_PERMISSIONS_ADDED = "Access permissions level specified for \"{0}\" approval signature "
             + "is higher than previous one specified. These access permissions will be ignored.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String ACROFORM_REMOVED = "AcroForm dictionary was removed from catalog.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String ANNOTATIONS_MODIFIED = "Field annotations were removed, added or unexpectedly modified.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String BASE_VERSION_DECREASED = "Base version number in developer extension \"{0}\" dictionary was decreased.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String BASE_VERSION_EXTENSION_NOT_PARSABLE = "Base version number in developer extension \"{0}\" dictionary is not parsable.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String DEVELOPER_EXTENSION_REMOVED = "Developer extension \"{0}\" dictionary was removed or unexpectedly modified.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String DIRECT_OBJECT = "{0} must be an indirect reference.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String DOCUMENT_WITHOUT_SIGNATURES = "Document doesn't contain any signatures.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String DSS_REMOVED = "DSS dictionary was removed from catalog.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String EXTENSIONS_REMOVED = "Extensions dictionary was removed from the catalog.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String EXTENSIONS_TYPE = "Developer extensions must be a dictionary.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String EXTENSION_LEVEL_DECREASED = "Extension level number in developer extension \"{0}\" dictionary was decreased.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String FIELD_NOT_DICTIONARY = "Form field \"{0}\" or one of its widgets is not a dictionary. It will not be validated.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String FIELD_REMOVED = "Form field {0} was removed or unexpectedly modified.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String LOCKED_FIELD_KIDS_ADDED = "Kids were added to locked form field \"{0}\".";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String LOCKED_FIELD_KIDS_REMOVED = "Kids were removed from locked form field \"{0}\" .";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String LOCKED_FIELD_MODIFIED = "Locked form field \"{0}\" or one of its widgets was modified.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String LOCKED_FIELD_REMOVED = "Locked form field \"{0}\" was removed from the document.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String NOT_ALLOWED_ACROFORM_CHANGES = "PDF document AcroForm contains changes other than " 
            + "document timestamp (docMDP level >= 1), form fill-in and digital signatures (docMDP level >= 2), " 
            + "adding or editing annotations (docMDP level 3), which are not allowed.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String NOT_ALLOWED_CATALOG_CHANGES = "PDF document catalog contains changes other than " + 
            "DSS dictionary and DTS addition (docMDP level >= 1), " + "form fill-in and digital signatures (docMDP level >= 2), "
             + "adding or editing annotations (docMDP level 3).";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String NOT_ALLOWED_CERTIFICATION_SIGNATURE = "Certification signature is applied after " + 
            "the approval signature which is not allowed.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String OBJECT_REMOVED = "Object \"{0}\", which is not allowed to be removed, was removed from the document through XREF table.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String PAGES_MODIFIED = "Pages structure was unexpectedly modified.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String PAGE_ANNOTATIONS_MODIFIED = "Page annotations were unexpectedly modified.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String PAGE_MODIFIED = "Page was unexpectedly modified.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String TABS_MODIFIED = "Tabs entry in a page dictionary was unexpectedly modified.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String PERMISSIONS_REMOVED = "Permissions dictionary was removed from the catalog.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String PERMISSIONS_TYPE = "Permissions must be a dictionary.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String PERMISSION_REMOVED = "Permission \"{0}\" dictionary was removed or unexpectedly modified.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String REFERENCE_REMOVED = "Signature reference dictionary was removed or unexpectedly modified.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String REVISIONS_READING_EXCEPTION = "IOException occurred during document revisions reading.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String REVISIONS_RETRIEVAL_FAILED = "Wasn't possible to retrieve document revisions.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String REVISIONS_RETRIEVAL_FAILED_UNEXPECTEDLY = "Unexpected exception while retrieving document revisions.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String SIGNATURE_MODIFIED = "Signature {0} was unexpectedly modified.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String SIGNATURE_REVISION_NOT_FOUND = "Not possible to identify document revision corresponding to the first signature in the document.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String STRUCT_TREE_CONTENT_MODIFIED = "Struct tree content element is unexpectedly modified.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String STRUCT_TREE_ELEMENT_MODIFIED = "Struct tree element is unexpectedly modified.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String STRUCT_TREE_ROOT_ADDED = "StructTreeRoot which contains not allowed entries was added to the catalog.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String STRUCT_TREE_ROOT_MODIFIED = "StructTreeRoot was unexpectedly modified.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String STRUCT_TREE_ROOT_NOT_DICT = "StructTreeRoot, which is not a dictionary, was modified.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String STRUCT_TREE_ROOT_REMOVED = "StructTreeRoot was removed from the catalog.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String TOO_MANY_CERTIFICATION_SIGNATURES = "Document contains more than one certification signature.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String UNEXPECTED_ENTRY_IN_XREF = "New PDF document revision contains unexpected entry \"{0}\" in XREF table.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String UNEXPECTED_FORM_FIELD = "New PDF document revision contains unexpected form field \"{0}\".";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String UNKNOWN_ACCESS_PERMISSIONS = "Access permissions level number specified for \"{0}\" signature "
             + "is undefined. Default level 2 will be used instead.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String UNRECOGNIZED_ACTION = "Signature field lock dictionary contains unrecognized " + "\"Action\" value \"{0}\". \"All\" will be used instead.";
//\endcond

        private const float EPS = 1e-5f;

        private static readonly PdfDictionary DUMMY_STRUCT_TREE_ELEMENT = new PdfDictionary(JavaCollectionsUtil.SingletonMap
            (PdfName.K, (PdfObject)new PdfArray()));

        private readonly ICollection<String> lockedFields = new HashSet<String>();

        private readonly SignatureValidationProperties properties;

        private IMetaInfo metaInfo = new ValidationMetaInfo();

        private AccessPermissions accessPermissions = AccessPermissions.ANNOTATION_MODIFICATION;

        private AccessPermissions requestedAccessPermissions = AccessPermissions.UNSPECIFIED;

        private ReportItem.ReportItemStatus unexpectedXrefChangesStatus = ReportItem.ReportItemStatus.INFO;

        private ICollection<PdfObject> checkedAnnots;

        private ICollection<PdfDictionary> newlyAddedFields;

        private ICollection<PdfDictionary> removedTaggedObjects;

        private ICollection<PdfDictionary> addedTaggedObjects;

        private Tuple2<ICollection<PdfIndirectReference>, ICollection<PdfIndirectReference>> usuallyModifiedObjects;

        /// <summary>
        /// Creates new instance of
        /// <see cref="DocumentRevisionsValidator"/>.
        /// </summary>
        /// <param name="chainBuilder">
        /// See
        /// <see cref="ValidatorChainBuilder"/>
        /// </param>
        protected internal DocumentRevisionsValidator(ValidatorChainBuilder chainBuilder) {
            this.properties = chainBuilder.GetProperties();
        }

        /// <summary>
        /// Sets the
        /// <see cref="iText.Commons.Actions.Contexts.IMetaInfo"/>
        /// that will be used during new
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// creations.
        /// </summary>
        /// <param name="metaInfo">meta info to set</param>
        /// <returns>
        /// the same
        /// <see cref="DocumentRevisionsValidator"/>
        /// instance.
        /// </returns>
        public virtual iText.Signatures.Validation.DocumentRevisionsValidator SetEventCountingMetaInfo(IMetaInfo metaInfo
            ) {
            this.metaInfo = metaInfo;
            return this;
        }

        /// <summary>Set access permissions to be used during docMDP validation.</summary>
        /// <remarks>
        /// Set access permissions to be used during docMDP validation.
        /// If value is provided, access permission related signature parameters will be ignored during the validation.
        /// </remarks>
        /// <param name="accessPermissions">
        /// 
        /// <see cref="iText.Signatures.AccessPermissions"/>
        /// docMDP validation level
        /// </param>
        /// <returns>
        /// the same
        /// <see cref="DocumentRevisionsValidator"/>
        /// instance.
        /// </returns>
        public virtual iText.Signatures.Validation.DocumentRevisionsValidator SetAccessPermissions(AccessPermissions
             accessPermissions) {
            this.requestedAccessPermissions = accessPermissions;
            return this;
        }

        /// <summary>
        /// Set the status to be used for the report items produced during docMDP validation in case revision contains
        /// unexpected changes in the XREF table.
        /// </summary>
        /// <remarks>
        /// Set the status to be used for the report items produced during docMDP validation in case revision contains
        /// unexpected changes in the XREF table. Default value is
        /// <see cref="iText.Signatures.Validation.Report.ReportItem.ReportItemStatus.INFO"/>.
        /// </remarks>
        /// <param name="status">
        /// 
        /// <see cref="iText.Signatures.Validation.Report.ReportItem.ReportItemStatus"/>
        /// to be used in case of unexpected changes in the XREF table
        /// </param>
        /// <returns>
        /// the same
        /// <see cref="DocumentRevisionsValidator"/>
        /// instance.
        /// </returns>
        public virtual iText.Signatures.Validation.DocumentRevisionsValidator SetUnexpectedXrefChangesStatus(ReportItem.ReportItemStatus
             status) {
            this.unexpectedXrefChangesStatus = status;
            return this;
        }

        /// <summary>Validate all document revisions according to docMDP and fieldMDP transform methods.</summary>
        /// <param name="context">the validation context in which to validate document revisions</param>
        /// <param name="document">the document to be validated</param>
        /// <returns>
        /// 
        /// <see cref="iText.Signatures.Validation.Report.ValidationReport"/>
        /// which contains detailed validation results.
        /// </returns>
        public virtual ValidationReport ValidateAllDocumentRevisions(ValidationContext context, PdfDocument document
            ) {
            return ValidateAllDocumentRevisions(context, document, null);
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Validate all document revisions according to docMDP and fieldMDP transform methods and collect validation report
        /// related to the single signature field checks if specified.
        /// </summary>
        /// <param name="context">the validation context in which to validate document revisions</param>
        /// <param name="document">the document to be validated</param>
        /// <param name="signatureName">signature field to collect validation result for. If null, all signatures will be checked
        ///     </param>
        /// <returns>
        /// 
        /// <see cref="iText.Signatures.Validation.Report.ValidationReport"/>
        /// which contains detailed validation results.
        /// </returns>
        internal virtual ValidationReport ValidateAllDocumentRevisions(ValidationContext context, PdfDocument document
            , String signatureName) {
            ResetClassFields();
            ValidationContext localContext = context.SetValidatorContext(ValidatorContext.DOCUMENT_REVISIONS_VALIDATOR
                );
            ValidationReport report = new ValidationReport();
            PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
            revisionsReader.SetEventCountingMetaInfo(metaInfo);
            IList<DocumentRevision> documentRevisions;
            try {
                documentRevisions = revisionsReader.GetAllRevisions();
            }
            catch (System.IO.IOException) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, REVISIONS_RETRIEVAL_FAILED, ReportItem.ReportItemStatus
                    .INDETERMINATE));
                return report;
            }
            catch (Exception e) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, REVISIONS_RETRIEVAL_FAILED_UNEXPECTEDLY, e, ReportItem.ReportItemStatus
                    .INDETERMINATE));
                return report;
            }
            MergeRevisionsInLinearizedDocument(document, documentRevisions);
            SignatureUtil signatureUtil = new SignatureUtil(document);
            IList<String> signatures = new List<String>(signatureUtil.GetSignatureNames());
            if (signatures.IsEmpty()) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, DOCUMENT_WITHOUT_SIGNATURES, ReportItem.ReportItemStatus
                    .INFO));
                return report;
            }
            bool updateAccessPermissions = true;
            bool documentSigned = false;
            bool certificationSignatureFound = false;
            bool collectRevisionsValidationReport = signatureName == null;
            String currentSignatureName = signatures[0];
            PdfSignature currentSignature = signatureUtil.GetSignature(currentSignatureName);
            for (int i = 0; i < documentRevisions.Count; i++) {
                if (currentSignature != null && RevisionContainsSignature(documentRevisions[i], currentSignatureName, document
                    , report)) {
                    if (IsCertificationSignature(currentSignature)) {
                        if (certificationSignatureFound) {
                            report.AddReportItem(new ReportItem(DOC_MDP_CHECK, TOO_MANY_CERTIFICATION_SIGNATURES, ReportItem.ReportItemStatus
                                .INDETERMINATE));
                        }
                        else {
                            if (documentSigned) {
                                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, NOT_ALLOWED_CERTIFICATION_SIGNATURE, ReportItem.ReportItemStatus
                                    .INDETERMINATE));
                            }
                            else {
                                certificationSignatureFound = true;
                                if (updateAccessPermissions) {
                                    UpdateCertificationSignatureAccessPermissions(currentSignature, report);
                                }
                            }
                        }
                    }
                    documentSigned = true;
                    if (updateAccessPermissions) {
                        UpdateApprovalSignatureAccessPermissions(signatureUtil.GetSignatureFormFieldDictionary(currentSignatureName
                            ), report);
                        UpdateApprovalSignatureFieldLock(documentRevisions[i], signatureUtil.GetSignatureFormFieldDictionary(currentSignatureName
                            ), document, report);
                    }
                    if (signatureName != null && signatureName.Equals(currentSignatureName)) {
                        updateAccessPermissions = false;
                        collectRevisionsValidationReport = true;
                    }
                    signatures.JRemoveAt(0);
                    if (signatures.IsEmpty()) {
                        currentSignature = null;
                    }
                    else {
                        currentSignatureName = signatures[0];
                        currentSignature = signatureUtil.GetSignature(currentSignatureName);
                    }
                }
                if (documentSigned && i < documentRevisions.Count - 1) {
                    ValidationReport validationReport = new ValidationReport();
                    ValidateRevision(documentRevisions[i], documentRevisions[i + 1], document, validationReport, localContext);
                    if (collectRevisionsValidationReport) {
                        report.Merge(validationReport);
                    }
                }
                if (StopValidation(report, localContext)) {
                    break;
                }
            }
            if (!documentSigned) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, SIGNATURE_REVISION_NOT_FOUND, ReportItem.ReportItemStatus
                    .INVALID));
            }
            return report;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void ValidateRevision(DocumentRevision previousRevision, DocumentRevision currentRevision
            , PdfDocument originalDocument, ValidationReport validationReport, ValidationContext context) {
            CreateDocumentAndPerformOperation(previousRevision, originalDocument, validationReport, (documentWithoutRevision
                ) => CreateDocumentAndPerformOperation(currentRevision, originalDocument, validationReport, (documentWithRevision
                ) => ValidateRevision(validationReport, context, documentWithoutRevision, documentWithRevision, currentRevision
                )));
        }
//\endcond

        private void MergeRevisionsInLinearizedDocument(PdfDocument document, IList<DocumentRevision> documentRevisions
            ) {
            if (documentRevisions.Count > 1) {
                // We need to check if document is linearized in first revision
                // We don't need to populate validation report in case of exceptions, it will happen later
                CreateDocumentAndPerformOperation(documentRevisions[0], document, new ValidationReport(), (firstRevisionDocument
                    ) => {
                    if (IsLinearizedPdf(document)) {
                        ICollection<PdfIndirectReference> mergedModifiedReferences = new HashSet<PdfIndirectReference>(documentRevisions
                            [0].GetModifiedObjects());
                        mergedModifiedReferences.AddAll(documentRevisions[1].GetModifiedObjects());
                        DocumentRevision mergedRevision = new DocumentRevision(documentRevisions[0].GetEofOffset(), mergedModifiedReferences
                            );
                        documentRevisions.Add(0, mergedRevision);
                        documentRevisions.JRemoveAt(1);
                        documentRevisions.JRemoveAt(1);
                    }
                    return true;
                }
                );
            }
        }

        private bool ValidateRevision(ValidationReport validationReport, ValidationContext context, PdfDocument documentWithoutRevision
            , PdfDocument documentWithRevision, DocumentRevision currentRevision) {
            usuallyModifiedObjects = new Tuple2<ICollection<PdfIndirectReference>, ICollection<PdfIndirectReference>>(
                CreateUsuallyModifiedObjectsSet(documentWithoutRevision), CreateUsuallyModifiedObjectsSet(documentWithRevision
                ));
            if (!CompareCatalogs(documentWithoutRevision, documentWithRevision, validationReport, context)) {
                return false;
            }
            ICollection<PdfIndirectReference> currentAllowedReferences = CreateAllowedReferences(documentWithRevision);
            ICollection<PdfIndirectReference> previousAllowedReferences = CreateAllowedReferences(documentWithoutRevision
                );
            foreach (PdfIndirectReference indirectReference in currentRevision.GetModifiedObjects()) {
                if (indirectReference.IsFree()) {
                    // In this boolean flag we check that reference which is about to be removed is the one which
                    // changed in the new revision. For instance DSS reference was 5 0 obj and changed to be 6 0 obj.
                    // In this case and only in this case reference with obj number 5 can be safely removed.
                    bool referenceAllowedToBeRemoved = previousAllowedReferences.Any((reference) => reference != null && reference
                        .GetObjNumber() == indirectReference.GetObjNumber()) && !currentAllowedReferences.Any((reference) => reference
                         != null && reference.GetObjNumber() == indirectReference.GetObjNumber());
                    // If some reference wasn't in the previous document, it is safe to remove it,
                    // since it is not possible to introduce new reference and remove it at the same revision.
                    bool referenceWasInPrevDocument = documentWithoutRevision.GetPdfObject(indirectReference.GetObjNumber()) !=
                         null;
                    if (!IsMaxGenerationObject(indirectReference) && referenceWasInPrevDocument && !referenceAllowedToBeRemoved
                        ) {
                        validationReport.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(OBJECT_REMOVED, indirectReference
                            .GetObjNumber()), unexpectedXrefChangesStatus));
                    }
                }
                else {
                    if (!CheckAllowedReferences(currentAllowedReferences, previousAllowedReferences, indirectReference, documentWithoutRevision
                        ) && !IsAllowedStreamObj(indirectReference, documentWithRevision)) {
                        validationReport.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(UNEXPECTED_ENTRY_IN_XREF
                            , indirectReference.GetObjNumber()), unexpectedXrefChangesStatus));
                    }
                }
            }
            return validationReport.GetValidationResult() == ValidationReport.ValidationResult.VALID;
        }

//\cond DO_NOT_DOCUMENT
        //
        //
        // Revisions validation util section:
        //
        //
        internal virtual AccessPermissions GetAccessPermissions() {
            return requestedAccessPermissions == AccessPermissions.UNSPECIFIED ? accessPermissions : requestedAccessPermissions;
        }
//\endcond

        private static Stream CreateInputStreamFromRevision(PdfDocument originalDocument, DocumentRevision revision
            ) {
            RandomAccessFileOrArray raf = originalDocument.GetReader().GetSafeFile();
            WindowRandomAccessSource source = new WindowRandomAccessSource(raf.CreateSourceView(), 0, revision.GetEofOffset
                ());
            return new RASInputStream(source);
        }

        private static bool IsLinearizedPdf(PdfDocument originalDocument) {
            for (int i = 0; i < originalDocument.GetNumberOfPdfObjects(); ++i) {
                PdfObject @object = originalDocument.GetPdfObject(i);
                if (@object is PdfDictionary) {
                    PdfDictionary dictionary = (PdfDictionary)@object;
                    if (dictionary.ContainsKey(new PdfName("Linearized"))) {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IsStructTreeElement(PdfObject @object) {
            if (@object is PdfDictionary) {
                PdfDictionary objectDictionary = (PdfDictionary)@object;
                PdfName type = objectDictionary.GetAsName(PdfName.Type);
                return type == null || PdfName.StructElem.Equals(type);
            }
            return false;
        }

        private static PdfObject GetObjectFromStructTreeContent(PdfObject structTreeContent) {
            return structTreeContent is PdfDictionary ? ((PdfDictionary)structTreeContent).Get(PdfName.Obj) : null;
        }

        private bool StopValidation(ValidationReport result, ValidationContext validationContext) {
            return !properties.GetContinueAfterFailure(validationContext) && result.GetValidationResult() == ValidationReport.ValidationResult
                .INVALID;
        }

        private void UpdateApprovalSignatureAccessPermissions(PdfDictionary signatureField, ValidationReport report
            ) {
            PdfDictionary fieldLock = signatureField.GetAsDictionary(PdfName.Lock);
            if (fieldLock == null || fieldLock.GetAsNumber(PdfName.P) == null) {
                return;
            }
            PdfNumber p = fieldLock.GetAsNumber(PdfName.P);
            AccessPermissions newAccessPermissions;
            switch (p.IntValue()) {
                case 1: {
                    newAccessPermissions = AccessPermissions.NO_CHANGES_PERMITTED;
                    break;
                }

                case 2: {
                    newAccessPermissions = AccessPermissions.FORM_FIELDS_MODIFICATION;
                    break;
                }

                case 3: {
                    newAccessPermissions = AccessPermissions.ANNOTATION_MODIFICATION;
                    break;
                }

                default: {
                    // Do nothing.
                    return;
                }
            }
            if (accessPermissions.CompareTo(newAccessPermissions) < 0) {
                PdfString fieldName = signatureField.GetAsString(PdfName.T);
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(ACCESS_PERMISSIONS_ADDED, fieldName
                     == null ? "" : fieldName.GetValue()), ReportItem.ReportItemStatus.INDETERMINATE));
            }
            else {
                accessPermissions = newAccessPermissions;
            }
        }

        private void UpdateApprovalSignatureFieldLock(DocumentRevision revision, PdfDictionary signatureField, PdfDocument
             document, ValidationReport report) {
            PdfDictionary fieldLock = signatureField.GetAsDictionary(PdfName.Lock);
            if (fieldLock == null || fieldLock.GetAsName(PdfName.Action) == null) {
                return;
            }
            PdfName action = fieldLock.GetAsName(PdfName.Action);
            if (PdfName.Include.Equals(action)) {
                PdfArray fields = fieldLock.GetAsArray(PdfName.Fields);
                if (fields != null) {
                    foreach (PdfObject fieldName in fields) {
                        if (fieldName is PdfString) {
                            lockedFields.Add(((PdfString)fieldName).ToUnicodeString());
                        }
                    }
                }
            }
            else {
                if (PdfName.Exclude.Equals(action)) {
                    PdfArray fields = fieldLock.GetAsArray(PdfName.Fields);
                    IList<String> excludedFields = JavaCollectionsUtil.EmptyList<String>();
                    if (fields != null) {
                        excludedFields = fields.ToList().Select((field) => field is PdfString ? ((PdfString)field).ToUnicodeString
                            () : null).ToList();
                    }
                    LockAllFormFields(revision, excludedFields, document, report);
                }
                else {
                    if (!PdfName.All.Equals(action)) {
                        report.AddReportItem(new ReportItem(FIELD_MDP_CHECK, MessageFormatUtil.Format(UNRECOGNIZED_ACTION, action.
                            GetValue()), ReportItem.ReportItemStatus.INVALID));
                    }
                    LockAllFormFields(revision, JavaCollectionsUtil.EmptyList<String>(), document, report);
                }
            }
        }

        private void LockAllFormFields(DocumentRevision revision, IList<String> excludedFields, PdfDocument originalDocument
            , ValidationReport report) {
            CreateDocumentAndPerformOperation(revision, originalDocument, report, (document) => {
                PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(document, false);
                if (acroForm != null) {
                    foreach (String fieldName in acroForm.GetAllFormFields().Keys) {
                        if (!excludedFields.Contains(fieldName)) {
                            lockedFields.Add(fieldName);
                        }
                    }
                }
                return true;
            }
            );
        }

        private void UpdateCertificationSignatureAccessPermissions(PdfSignature signature, ValidationReport report
            ) {
            PdfArray references = signature.GetPdfObject().GetAsArray(PdfName.Reference);
            foreach (PdfObject reference in references) {
                PdfDictionary referenceDict = (PdfDictionary)reference;
                PdfName transformMethod = referenceDict.GetAsName(PdfName.TransformMethod);
                if (PdfName.DocMDP.Equals(transformMethod)) {
                    PdfDictionary transformParameters = referenceDict.GetAsDictionary(PdfName.TransformParams);
                    if (transformParameters == null || transformParameters.GetAsNumber(PdfName.P) == null) {
                        accessPermissions = AccessPermissions.FORM_FIELDS_MODIFICATION;
                        return;
                    }
                    PdfNumber p = transformParameters.GetAsNumber(PdfName.P);
                    switch (p.IntValue()) {
                        case 1: {
                            accessPermissions = AccessPermissions.NO_CHANGES_PERMITTED;
                            break;
                        }

                        case 2: {
                            accessPermissions = AccessPermissions.FORM_FIELDS_MODIFICATION;
                            break;
                        }

                        case 3: {
                            accessPermissions = AccessPermissions.ANNOTATION_MODIFICATION;
                            break;
                        }

                        default: {
                            report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(UNKNOWN_ACCESS_PERMISSIONS, signature
                                .GetName()), ReportItem.ReportItemStatus.INDETERMINATE));
                            accessPermissions = AccessPermissions.FORM_FIELDS_MODIFICATION;
                            break;
                        }
                    }
                    return;
                }
            }
        }

        private bool IsCertificationSignature(PdfSignature signature) {
            if (PdfName.DocTimeStamp.Equals(signature.GetType()) || PdfName.ETSI_RFC3161.Equals(signature.GetSubFilter
                ())) {
                // Timestamp is never a certification signature.
                return false;
            }
            PdfArray references = signature.GetPdfObject().GetAsArray(PdfName.Reference);
            if (references != null) {
                foreach (PdfObject reference in references) {
                    if (reference is PdfDictionary) {
                        PdfDictionary referenceDict = (PdfDictionary)reference;
                        PdfName transformMethod = referenceDict.GetAsName(PdfName.TransformMethod);
                        return PdfName.DocMDP.Equals(transformMethod);
                    }
                }
            }
            return false;
        }

        private bool RevisionContainsSignature(DocumentRevision revision, String signature, PdfDocument originalDocument
            , ValidationReport report) {
            return CreateDocumentAndPerformOperation(revision, originalDocument, report, (document) => {
                SignatureUtil signatureUtil = new SignatureUtil(document);
                return signatureUtil.SignatureCoversWholeDocument(signature);
            }
            );
        }

        private bool CreateDocumentAndPerformOperation(DocumentRevision revision, PdfDocument originalDocument, ValidationReport
             report, Func<PdfDocument, bool> operation) {
            try {
                using (Stream inputStream = CreateInputStreamFromRevision(originalDocument, revision)) {
                    using (PdfReader reader = new PdfReader(inputStream, originalDocument.GetReader().GetPropertiesCopy()).SetStrictnessLevel
                        (PdfReader.StrictnessLevel.CONSERVATIVE)) {
                        using (PdfDocument documentWithRevision = new PdfDocument(reader, new DocumentProperties().SetEventCountingMetaInfo
                            (metaInfo))) {
                            return (bool)operation.Invoke(documentWithRevision);
                        }
                    }
                }
            }
            catch (System.IO.IOException exception) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, REVISIONS_READING_EXCEPTION, exception, ReportItem.ReportItemStatus
                    .INDETERMINATE));
                return false;
            }
            catch (Exception exception) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, REVISIONS_READING_EXCEPTION, exception, ReportItem.ReportItemStatus
                    .INDETERMINATE));
                return false;
            }
        }

        private void ResetClassFields() {
            lockedFields.Clear();
            accessPermissions = AccessPermissions.ANNOTATION_MODIFICATION;
        }

        //
        //
        // Compare catalogs section:
        //
        //
        private bool CompareCatalogs(PdfDocument documentWithoutRevision, PdfDocument documentWithRevision, ValidationReport
             report, ValidationContext context) {
            PdfDictionary previousCatalog = documentWithoutRevision.GetCatalog().GetPdfObject();
            PdfDictionary currentCatalog = documentWithRevision.GetCatalog().GetPdfObject();
            PdfDictionary previousCatalogCopy = CopyCatalogEntriesToCompare(previousCatalog);
            PdfDictionary currentCatalogCopy = CopyCatalogEntriesToCompare(currentCatalog);
            removedTaggedObjects = new HashSet<PdfDictionary>();
            addedTaggedObjects = new HashSet<PdfDictionary>();
            if (!ComparePdfObjects(previousCatalogCopy, currentCatalogCopy, usuallyModifiedObjects)) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, NOT_ALLOWED_CATALOG_CHANGES, ReportItem.ReportItemStatus
                    .INVALID));
                return false;
            }
            bool result = CompareExtensions(previousCatalog.Get(PdfName.Extensions), currentCatalog.Get(PdfName.Extensions
                ), report);
            if (StopValidation(report, context)) {
                return result;
            }
            result = result && ComparePermissions(previousCatalog.Get(PdfName.Perms), currentCatalog.Get(PdfName.Perms
                ), report);
            if (StopValidation(report, context)) {
                return result;
            }
            result = result && CompareDss(previousCatalog.Get(PdfName.DSS), currentCatalog.Get(PdfName.DSS), report);
            if (StopValidation(report, context)) {
                return result;
            }
            result = result && CompareAcroFormsWithFieldMDP(documentWithoutRevision, documentWithRevision, report);
            if (StopValidation(report, context)) {
                return result;
            }
            result = result && CompareAcroForms(previousCatalog.GetAsDictionary(PdfName.AcroForm), currentCatalog.GetAsDictionary
                (PdfName.AcroForm), report);
            if (StopValidation(report, context)) {
                return result;
            }
            result = result && ComparePages(previousCatalog.GetAsDictionary(PdfName.Pages), currentCatalog.GetAsDictionary
                (PdfName.Pages), report);
            if (StopValidation(report, context)) {
                return result;
            }
            return result && CompareStructTreeRoot(previousCatalog.Get(PdfName.StructTreeRoot), currentCatalog.Get(PdfName
                .StructTreeRoot), report);
        }

        // Compare catalogs nested methods section:
        private bool CompareStructTreeRoot(PdfObject previousStructTreeRoot, PdfObject currentStructTreeRoot, ValidationReport
             report) {
            if (previousStructTreeRoot == currentStructTreeRoot) {
                return true;
            }
            if (!(previousStructTreeRoot is PdfDictionary) && currentStructTreeRoot is PdfDictionary) {
                CompareStructTreeElementKids(DUMMY_STRUCT_TREE_ELEMENT, (PdfDictionary)currentStructTreeRoot, report);
                if (addedTaggedObjects.Contains(currentStructTreeRoot)) {
                    return true;
                }
                else {
                    report.AddReportItem(new ReportItem(DOC_MDP_CHECK, STRUCT_TREE_ROOT_ADDED, ReportItem.ReportItemStatus.INVALID
                        ));
                    return false;
                }
            }
            if (!(currentStructTreeRoot is PdfDictionary) && previousStructTreeRoot is PdfDictionary) {
                CompareStructTreeElementKids((PdfDictionary)previousStructTreeRoot, DUMMY_STRUCT_TREE_ELEMENT, report);
                if (removedTaggedObjects.Contains(previousStructTreeRoot)) {
                    return true;
                }
                else {
                    report.AddReportItem(new ReportItem(DOC_MDP_CHECK, STRUCT_TREE_ROOT_REMOVED, ReportItem.ReportItemStatus.INVALID
                        ));
                    return false;
                }
            }
            if (!(previousStructTreeRoot is PdfDictionary)) {
                if (ComparePdfObjects(previousStructTreeRoot, currentStructTreeRoot, usuallyModifiedObjects)) {
                    return true;
                }
                else {
                    report.AddReportItem(new ReportItem(DOC_MDP_CHECK, STRUCT_TREE_ROOT_NOT_DICT, ReportItem.ReportItemStatus.
                        INVALID));
                    return false;
                }
            }
            PdfDictionary previousStructTreeRootDict = new PdfDictionary((PdfDictionary)previousStructTreeRoot);
            PdfDictionary currentStructTreeRootDict = new PdfDictionary((PdfDictionary)currentStructTreeRoot);
            // Here we remove entries which are allowed to be modified in any way. Those won't be compared.
            previousStructTreeRootDict.Remove(PdfName.IDTree);
            currentStructTreeRootDict.Remove(PdfName.IDTree);
            previousStructTreeRootDict.Remove(PdfName.ParentTree);
            currentStructTreeRootDict.Remove(PdfName.ParentTree);
            previousStructTreeRootDict.Remove(PdfName.ParentTreeNextKey);
            currentStructTreeRootDict.Remove(PdfName.ParentTreeNextKey);
            // Here we remove actual content, which will be compared in a special manner.
            previousStructTreeRootDict.Remove(PdfName.K);
            currentStructTreeRootDict.Remove(PdfName.K);
            // Everything else is expected to remain unmodified and compared directly.
            if (!ComparePdfObjects(previousStructTreeRootDict, currentStructTreeRootDict, usuallyModifiedObjects)) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, STRUCT_TREE_ROOT_MODIFIED, ReportItem.ReportItemStatus.
                    INVALID));
                return false;
            }
            if (CompareStructTreeElementKids((PdfDictionary)previousStructTreeRoot, (PdfDictionary)currentStructTreeRoot
                , report)) {
                return true;
            }
            else {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, STRUCT_TREE_ROOT_MODIFIED, ReportItem.ReportItemStatus.
                    INVALID));
                return false;
            }
        }

        private bool CompareStructTreeElementKids(PdfDictionary previousStructElement, PdfDictionary currentStructElement
            , ValidationReport report) {
            PdfArray previousKids;
            if (previousStructElement.Get(PdfName.K) is PdfArray) {
                previousKids = previousStructElement.GetAsArray(PdfName.K);
            }
            else {
                previousKids = new PdfArray(previousStructElement.Get(PdfName.K));
            }
            PdfArray currentKids;
            if (currentStructElement.Get(PdfName.K) is PdfArray) {
                currentKids = currentStructElement.GetAsArray(PdfName.K);
            }
            else {
                currentKids = new PdfArray(currentStructElement.Get(PdfName.K));
            }
            int i = 0;
            int j = 0;
            bool comparisonHappened = false;
            while (i < previousKids.Size() || j < currentKids.Size()) {
                PdfObject previousKid = i < previousKids.Size() ? previousKids.Get(i) : DUMMY_STRUCT_TREE_ELEMENT;
                PdfObject currentKid = j < currentKids.Size() ? currentKids.Get(j) : DUMMY_STRUCT_TREE_ELEMENT;
                if (!IsStructTreeElement(previousKid)) {
                    PdfObject previousContentObject = GetObjectFromStructTreeContent(previousKid);
                    if (previousContentObject != null && removedTaggedObjects.Contains(previousContentObject)) {
                        i++;
                        continue;
                    }
                }
                if (!IsStructTreeElement(currentKid)) {
                    PdfObject currentContentObject = GetObjectFromStructTreeContent(currentKid);
                    if (currentContentObject != null && addedTaggedObjects.Contains(currentContentObject)) {
                        j++;
                        continue;
                    }
                }
                if (IsStructTreeElement(previousKid) && IsStructTreeElement(currentKid)) {
                    bool kidsComparisonResult = CompareStructTreeElementKids((PdfDictionary)previousKid, (PdfDictionary)currentKid
                        , report);
                    if (removedTaggedObjects.Contains(previousKid)) {
                        i++;
                        continue;
                    }
                    if (addedTaggedObjects.Contains(currentKid)) {
                        j++;
                        continue;
                    }
                    comparisonHappened = true;
                    if (!kidsComparisonResult || !CompareStructTreeElements((PdfDictionary)previousKid, (PdfDictionary)currentKid
                        , report)) {
                        return false;
                    }
                }
                else {
                    if (!IsStructTreeElement(previousKid) && !IsStructTreeElement(currentKid)) {
                        comparisonHappened = true;
                        if (!CompareStructTreeContents(previousKid, currentKid, report)) {
                            return false;
                        }
                    }
                    else {
                        if (IsStructTreeElement(previousKid)) {
                            CompareStructTreeElementKids((PdfDictionary)previousKid, DUMMY_STRUCT_TREE_ELEMENT, report);
                            if (removedTaggedObjects.Contains(previousKid)) {
                                i++;
                                continue;
                            }
                            return false;
                        }
                        else {
                            CompareStructTreeElementKids(DUMMY_STRUCT_TREE_ELEMENT, (PdfDictionary)currentKid, report);
                            if (addedTaggedObjects.Contains(currentKid)) {
                                j++;
                                continue;
                            }
                            return false;
                        }
                    }
                }
                i++;
                j++;
            }
            if (!comparisonHappened && previousStructElement != DUMMY_STRUCT_TREE_ELEMENT) {
                removedTaggedObjects.Add(previousStructElement);
            }
            if (!comparisonHappened && currentStructElement != DUMMY_STRUCT_TREE_ELEMENT) {
                addedTaggedObjects.Add(currentStructElement);
            }
            return true;
        }

        private bool CompareStructTreeElements(PdfDictionary previousStructElement, PdfDictionary currentStructElement
            , ValidationReport report) {
            PdfDictionary previousStructElementCopy = new PdfDictionary(previousStructElement);
            previousStructElementCopy.Remove(PdfName.K);
            previousStructElementCopy.Remove(PdfName.P);
            previousStructElementCopy.Remove(PdfName.Ref);
            previousStructElementCopy.Remove(PdfName.Pg);
            PdfDictionary currentStructElementCopy = new PdfDictionary(currentStructElement);
            currentStructElementCopy.Remove(PdfName.K);
            currentStructElementCopy.Remove(PdfName.P);
            currentStructElementCopy.Remove(PdfName.Ref);
            currentStructElementCopy.Remove(PdfName.Pg);
            if (!ComparePdfObjects(previousStructElementCopy, currentStructElementCopy, usuallyModifiedObjects)) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, STRUCT_TREE_ELEMENT_MODIFIED, ReportItem.ReportItemStatus
                    .INVALID));
                return false;
            }
            return CompareIndirectReferencesObjNums(previousStructElement.Get(PdfName.P), currentStructElement.Get(PdfName
                .P), report, "Struct tree element parent entry") && CompareIndirectReferencesObjNums(previousStructElement
                .Get(PdfName.Ref), currentStructElement.Get(PdfName.Ref), report, "Struct tree element ref entry") && 
                CompareIndirectReferencesObjNums(previousStructElement.Get(PdfName.Pg), currentStructElement.Get(PdfName
                .Pg), report, "Struct tree element page entry");
        }

        private bool CompareStructTreeContents(PdfObject previousStructTreeContent, PdfObject currentStructTreeContent
            , ValidationReport report) {
            if (previousStructTreeContent is PdfDictionary && currentStructTreeContent is PdfDictionary) {
                PdfDictionary previousContentDictionary = (PdfDictionary)previousStructTreeContent;
                PdfDictionary currentContentDictionary = (PdfDictionary)currentStructTreeContent;
                PdfDictionary previousContentDictionaryCopy = new PdfDictionary(previousContentDictionary);
                previousContentDictionaryCopy.Remove(PdfName.Pg);
                previousContentDictionaryCopy.Remove(PdfName.Obj);
                PdfDictionary currentContentDictionaryCopy = new PdfDictionary(currentContentDictionary);
                currentContentDictionaryCopy.Remove(PdfName.Pg);
                currentContentDictionaryCopy.Remove(PdfName.Obj);
                if (!ComparePdfObjects(previousContentDictionaryCopy, currentContentDictionaryCopy, usuallyModifiedObjects
                    )) {
                    report.AddReportItem(new ReportItem(DOC_MDP_CHECK, STRUCT_TREE_CONTENT_MODIFIED, ReportItem.ReportItemStatus
                        .INVALID));
                    return false;
                }
                return CompareIndirectReferencesObjNums(previousContentDictionary.Get(PdfName.Pg), currentContentDictionary
                    .Get(PdfName.Pg), report, "Object reference dictionary page entry") && CompareIndirectReferencesObjNums
                    (previousContentDictionary.Get(PdfName.Obj), currentContentDictionary.Get(PdfName.Obj), report, "Object reference dictionary obj entry"
                    );
            }
            else {
                return ComparePdfObjects(previousStructTreeContent, currentStructTreeContent, usuallyModifiedObjects);
            }
        }

        private bool CompareExtensions(PdfObject previousExtensions, PdfObject currentExtensions, ValidationReport
             report) {
            if (previousExtensions == null || ComparePdfObjects(previousExtensions, currentExtensions, usuallyModifiedObjects
                )) {
                return true;
            }
            if (currentExtensions == null) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, EXTENSIONS_REMOVED, ReportItem.ReportItemStatus.INVALID
                    ));
                return false;
            }
            if (!(previousExtensions is PdfDictionary) || !(currentExtensions is PdfDictionary)) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, EXTENSIONS_TYPE, ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            PdfDictionary previousExtensionsDictionary = (PdfDictionary)previousExtensions;
            PdfDictionary currentExtensionsDictionary = (PdfDictionary)currentExtensions;
            bool result = true;
            foreach (KeyValuePair<PdfName, PdfObject> previousExtension in previousExtensionsDictionary.EntrySet()) {
                PdfDictionary currentExtension = currentExtensionsDictionary.GetAsDictionary(previousExtension.Key);
                if (currentExtension == null) {
                    report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(DEVELOPER_EXTENSION_REMOVED, previousExtension
                        .Key), ReportItem.ReportItemStatus.INVALID));
                    result = false;
                }
                else {
                    PdfDictionary currentExtensionCopy = new PdfDictionary(currentExtension);
                    currentExtensionCopy.Remove(PdfName.ExtensionLevel);
                    currentExtensionCopy.Remove(PdfName.BaseVersion);
                    PdfDictionary previousExtensionCopy = new PdfDictionary((PdfDictionary)previousExtension.Value);
                    previousExtensionCopy.Remove(PdfName.ExtensionLevel);
                    previousExtensionCopy.Remove(PdfName.BaseVersion);
                    // Apart from extension level and base version dictionaries are expected to be equal.
                    if (!ComparePdfObjects(previousExtensionCopy, currentExtensionCopy, usuallyModifiedObjects)) {
                        report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(DEVELOPER_EXTENSION_REMOVED, previousExtension
                            .Key), ReportItem.ReportItemStatus.INVALID));
                        result = false;
                        continue;
                    }
                    PdfNumber previousExtensionLevel = ((PdfDictionary)previousExtension.Value).GetAsNumber(PdfName.ExtensionLevel
                        );
                    PdfNumber currentExtensionLevel = currentExtension.GetAsNumber(PdfName.ExtensionLevel);
                    if (previousExtensionLevel != null) {
                        if (currentExtensionLevel == null || previousExtensionLevel.IntValue() > currentExtensionLevel.IntValue()) {
                            report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(EXTENSION_LEVEL_DECREASED, previousExtension
                                .Key), ReportItem.ReportItemStatus.INVALID));
                            result = false;
                        }
                    }
                    PdfName previousBaseVersion = ((PdfDictionary)previousExtension.Value).GetAsName(PdfName.BaseVersion);
                    PdfName currentBaseVersion = currentExtension.GetAsName(PdfName.BaseVersion);
                    if (previousBaseVersion != null) {
                        try {
                            if (currentBaseVersion == null || Double.Parse(previousBaseVersion.GetValue(), System.Globalization.CultureInfo.InvariantCulture
                                ) > Double.Parse(currentBaseVersion.GetValue(), System.Globalization.CultureInfo.InvariantCulture) + EPS
                                ) {
                                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(BASE_VERSION_DECREASED, previousExtension
                                    .Key), ReportItem.ReportItemStatus.INVALID));
                                result = false;
                            }
                        }
                        catch (FormatException) {
                            report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(BASE_VERSION_EXTENSION_NOT_PARSABLE
                                , previousExtension.Key), ReportItem.ReportItemStatus.INVALID));
                        }
                    }
                }
            }
            return result;
        }

        private bool ComparePermissions(PdfObject previousPerms, PdfObject currentPerms, ValidationReport report) {
            if (previousPerms == null || ComparePdfObjects(previousPerms, currentPerms, usuallyModifiedObjects)) {
                return true;
            }
            if (currentPerms == null) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, PERMISSIONS_REMOVED, ReportItem.ReportItemStatus.INVALID
                    ));
                return false;
            }
            if (!(previousPerms is PdfDictionary) || !(currentPerms is PdfDictionary)) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, PERMISSIONS_TYPE, ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            PdfDictionary previousPermsDictionary = (PdfDictionary)previousPerms;
            PdfDictionary currentPermsDictionary = (PdfDictionary)currentPerms;
            bool result = true;
            foreach (KeyValuePair<PdfName, PdfObject> previousPermission in previousPermsDictionary.EntrySet()) {
                PdfDictionary currentPermission = currentPermsDictionary.GetAsDictionary(previousPermission.Key);
                if (currentPermission == null) {
                    report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(PERMISSION_REMOVED, previousPermission
                        .Key), ReportItem.ReportItemStatus.INVALID));
                    result = false;
                }
                else {
                    // Perms dictionary is the signature dictionary.
                    if (!CompareSignatureDictionaries(previousPermission.Value, currentPermission, report)) {
                        report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(PERMISSION_REMOVED, previousPermission
                            .Key), ReportItem.ReportItemStatus.INVALID));
                        result = false;
                    }
                }
            }
            return result;
        }

        private bool CompareDss(PdfObject previousDss, PdfObject currentDss, ValidationReport report) {
            if (previousDss == null) {
                return true;
            }
            if (currentDss == null) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, DSS_REMOVED, ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            return true;
        }

        private bool CompareAcroFormsWithFieldMDP(PdfDocument documentWithoutRevision, PdfDocument documentWithRevision
            , ValidationReport report) {
            PdfAcroForm currentAcroForm = PdfFormCreator.GetAcroForm(documentWithRevision, false);
            PdfAcroForm previousAcroForm = PdfFormCreator.GetAcroForm(documentWithoutRevision, false);
            if (currentAcroForm == null || previousAcroForm == null) {
                // This is not a part of FieldMDP validation.
                return true;
            }
            if (accessPermissions == AccessPermissions.NO_CHANGES_PERMITTED) {
                // In this case FieldMDP makes no sense, because related changes are forbidden anyway.
                return true;
            }
            bool result = true;
            foreach (KeyValuePair<String, PdfFormField> previousField in previousAcroForm.GetAllFormFields()) {
                if (lockedFields.Contains(previousField.Key)) {
                    // For locked form fields nothing can change,
                    // however annotations can contain page link which should be excluded from direct comparison.
                    PdfFormField currentFormField = currentAcroForm.GetField(previousField.Key);
                    if (currentFormField == null) {
                        report.AddReportItem(new ReportItem(FIELD_MDP_CHECK, MessageFormatUtil.Format(LOCKED_FIELD_REMOVED, previousField
                            .Key), ReportItem.ReportItemStatus.INVALID));
                        result = false;
                        continue;
                    }
                    if (!CompareFormFieldWithFieldMDP(previousField.Value.GetPdfObject(), currentFormField.GetPdfObject(), previousField
                        .Key, report)) {
                        result = false;
                    }
                }
            }
            return result;
        }

        private bool CompareFormFieldWithFieldMDP(PdfDictionary previousField, PdfDictionary currentField, String 
            fieldName, ValidationReport report) {
            PdfDictionary previousFieldCopy = new PdfDictionary(previousField);
            previousFieldCopy.Remove(PdfName.Kids);
            previousFieldCopy.Remove(PdfName.P);
            previousFieldCopy.Remove(PdfName.Parent);
            previousFieldCopy.Remove(PdfName.V);
            PdfDictionary currentFieldCopy = new PdfDictionary(currentField);
            currentFieldCopy.Remove(PdfName.Kids);
            currentFieldCopy.Remove(PdfName.P);
            currentFieldCopy.Remove(PdfName.Parent);
            currentFieldCopy.Remove(PdfName.V);
            if (!ComparePdfObjects(previousFieldCopy, currentFieldCopy, usuallyModifiedObjects)) {
                report.AddReportItem(new ReportItem(FIELD_MDP_CHECK, MessageFormatUtil.Format(LOCKED_FIELD_MODIFIED, fieldName
                    ), ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            PdfObject prevValue = previousField.Get(PdfName.V);
            PdfObject currValue = currentField.Get(PdfName.V);
            if (PdfName.Sig.Equals(currentField.GetAsName(PdfName.FT))) {
                if (!CompareSignatureDictionaries(prevValue, currValue, report)) {
                    report.AddReportItem(new ReportItem(FIELD_MDP_CHECK, MessageFormatUtil.Format(LOCKED_FIELD_MODIFIED, fieldName
                        ), ReportItem.ReportItemStatus.INVALID));
                    return false;
                }
            }
            else {
                if (!ComparePdfObjects(prevValue, currValue, usuallyModifiedObjects)) {
                    report.AddReportItem(new ReportItem(FIELD_MDP_CHECK, MessageFormatUtil.Format(LOCKED_FIELD_MODIFIED, fieldName
                        ), ReportItem.ReportItemStatus.INVALID));
                    return false;
                }
            }
            if (!CompareIndirectReferencesObjNums(previousField.Get(PdfName.P), currentField.Get(PdfName.P), report, "Page object with which field annotation is associated"
                ) || !CompareIndirectReferencesObjNums(previousField.Get(PdfName.Parent), currentField.Get(PdfName.Parent
                ), report, "Form field parent")) {
                report.AddReportItem(new ReportItem(FIELD_MDP_CHECK, MessageFormatUtil.Format(LOCKED_FIELD_MODIFIED, fieldName
                    ), ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            PdfArray previousKids = previousField.GetAsArray(PdfName.Kids);
            PdfArray currentKids = currentField.GetAsArray(PdfName.Kids);
            if (previousKids == null && currentKids != null) {
                report.AddReportItem(new ReportItem(FIELD_MDP_CHECK, MessageFormatUtil.Format(LOCKED_FIELD_KIDS_ADDED, fieldName
                    ), ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            if (previousKids != null && currentKids == null) {
                report.AddReportItem(new ReportItem(FIELD_MDP_CHECK, MessageFormatUtil.Format(LOCKED_FIELD_KIDS_REMOVED, fieldName
                    ), ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            if (previousKids == currentKids) {
                return true;
            }
            if (previousKids.Size() < currentKids.Size()) {
                report.AddReportItem(new ReportItem(FIELD_MDP_CHECK, MessageFormatUtil.Format(LOCKED_FIELD_KIDS_ADDED, fieldName
                    ), ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            if (previousKids.Size() > currentKids.Size()) {
                report.AddReportItem(new ReportItem(FIELD_MDP_CHECK, MessageFormatUtil.Format(LOCKED_FIELD_KIDS_REMOVED, fieldName
                    ), ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            for (int i = 0; i < previousKids.Size(); ++i) {
                PdfDictionary previousKid = previousKids.GetAsDictionary(i);
                PdfDictionary currentKid = currentKids.GetAsDictionary(i);
                if (previousKid == null || currentKid == null) {
                    report.AddReportItem(new ReportItem(FIELD_MDP_CHECK, MessageFormatUtil.Format(FIELD_NOT_DICTIONARY, fieldName
                        ), ReportItem.ReportItemStatus.INDETERMINATE));
                    continue;
                }
                if (PdfFormAnnotationUtil.IsPureWidget(previousKid) && !CompareFormFieldWithFieldMDP(previousKid, currentKid
                    , fieldName, report)) {
                    return false;
                }
            }
            return true;
        }

        private bool CompareAcroForms(PdfDictionary prevAcroForm, PdfDictionary currAcroForm, ValidationReport report
            ) {
            checkedAnnots = new HashSet<PdfObject>();
            newlyAddedFields = new HashSet<PdfDictionary>();
            if (prevAcroForm == null) {
                if (currAcroForm == null) {
                    return true;
                }
                PdfArray fields = currAcroForm.GetAsArray(PdfName.Fields);
                foreach (PdfObject field in fields) {
                    PdfDictionary fieldDict = (PdfDictionary)field;
                    if (!IsAllowedSignatureField(fieldDict, report)) {
                        report.AddReportItem(new ReportItem(DOC_MDP_CHECK, NOT_ALLOWED_ACROFORM_CHANGES, ReportItem.ReportItemStatus
                            .INVALID));
                        return false;
                    }
                }
                return true;
            }
            if (currAcroForm == null) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, ACROFORM_REMOVED, ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            PdfDictionary previousAcroFormCopy = CopyAcroformDictionary(prevAcroForm);
            PdfDictionary currentAcroFormCopy = CopyAcroformDictionary(currAcroForm);
            PdfArray prevFields = prevAcroForm.GetAsArray(PdfName.Fields);
            PdfArray currFields = currAcroForm.GetAsArray(PdfName.Fields);
            if (!ComparePdfObjects(previousAcroFormCopy, currentAcroFormCopy, usuallyModifiedObjects) || (prevFields.Size
                () > currFields.Size()) || !CompareFormFields(prevFields, currFields, report)) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, NOT_ALLOWED_ACROFORM_CHANGES, ReportItem.ReportItemStatus
                    .INVALID));
                return false;
            }
            return true;
        }

        private bool CompareFormFields(PdfArray prevFields, PdfArray currFields, ValidationReport report) {
            ICollection<PdfDictionary> prevFieldsSet = PopulateFormFields(prevFields);
            ICollection<PdfDictionary> currFieldsSet = PopulateFormFields(currFields);
            foreach (PdfDictionary previousField in prevFieldsSet) {
                PdfDictionary currentField = RetrieveTheSameField(currFieldsSet, previousField);
                if (currentField == null || !CompareFields(previousField, currentField, report)) {
                    PdfString fieldName = previousField.GetAsString(PdfName.T);
                    report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(FIELD_REMOVED, fieldName == null
                         ? "" : fieldName.GetValue()), ReportItem.ReportItemStatus.INVALID));
                    return false;
                }
                if (PdfFormAnnotationUtil.IsPureWidgetOrMergedField(previousField)) {
                    checkedAnnots.Add(previousField);
                }
                if (PdfFormAnnotationUtil.IsPureWidgetOrMergedField(currentField)) {
                    checkedAnnots.Add(currentField);
                }
                currFieldsSet.Remove(currentField);
            }
            foreach (PdfDictionary field in currFieldsSet) {
                if (!IsAllowedSignatureField(field, report)) {
                    return false;
                }
            }
            return CompareWidgets(prevFields, currFields, report);
        }

        private PdfDictionary RetrieveTheSameField(ICollection<PdfDictionary> currFields, PdfDictionary previousField
            ) {
            foreach (PdfDictionary currentField in currFields) {
                PdfDictionary prevFormDict = CopyFieldDictionary(previousField);
                PdfDictionary currFormDict = CopyFieldDictionary(currentField);
                if (ComparePdfObjects(prevFormDict, currFormDict, usuallyModifiedObjects) && CompareIndirectReferencesObjNums
                    (prevFormDict.Get(PdfName.Parent), currFormDict.Get(PdfName.Parent), new ValidationReport(), "Form field parent"
                    ) && CompareIndirectReferencesObjNums(prevFormDict.Get(PdfName.P), currFormDict.Get(PdfName.P), new ValidationReport
                    (), "Page object with which field annotation is associated")) {
                    return currentField;
                }
            }
            return null;
        }

        /// <summary>DocMDP level &gt;= 2 allows setting values of the fields and accordingly update the widget appearances of them.
        ///     </summary>
        /// <remarks>
        /// DocMDP level &gt;= 2 allows setting values of the fields and accordingly update the widget appearances of them. But
        /// you cannot change the form structure, so it is not allowed to add, remove or rename fields, change most of their
        /// properties.
        /// </remarks>
        /// <param name="previousField">field from the previous revision to check</param>
        /// <param name="currentField">field from the current revision to check</param>
        /// <param name="report">validation report</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the changes of the field are allowed,
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        private bool CompareFields(PdfDictionary previousField, PdfDictionary currentField, ValidationReport report
            ) {
            PdfObject prevValue = previousField.Get(PdfName.V);
            PdfObject currValue = currentField.Get(PdfName.V);
            if (prevValue == null && currValue == null && PdfName.Ch.Equals(currentField.GetAsName(PdfName.FT))) {
                // Choice field: if the items in the I entry differ from those in the V entry, the V entry shall be used.
                prevValue = previousField.Get(PdfName.I);
                currValue = currentField.Get(PdfName.I);
            }
            if (PdfName.Sig.Equals(currentField.GetAsName(PdfName.FT))) {
                if (!CompareSignatureDictionaries(prevValue, currValue, report)) {
                    PdfString fieldName = currentField.GetAsString(PdfName.T);
                    report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(SIGNATURE_MODIFIED, fieldName 
                        == null ? "" : fieldName.GetValue()), ReportItem.ReportItemStatus.INVALID));
                    return false;
                }
            }
            else {
                if (GetAccessPermissions() == AccessPermissions.NO_CHANGES_PERMITTED && !ComparePdfObjects(prevValue, currValue
                    , usuallyModifiedObjects)) {
                    return false;
                }
            }
            return CompareFormFields(previousField.GetAsArray(PdfName.Kids), currentField.GetAsArray(PdfName.Kids), report
                );
        }

        private bool CompareSignatureDictionaries(PdfObject prevSigDict, PdfObject curSigDict, ValidationReport report
            ) {
            if (prevSigDict == null) {
                return true;
            }
            if (curSigDict == null) {
                return false;
            }
            if (!(prevSigDict is PdfDictionary) || !(curSigDict is PdfDictionary)) {
                return false;
            }
            PdfDictionary currentSigDictCopy = new PdfDictionary((PdfDictionary)curSigDict);
            currentSigDictCopy.Remove(PdfName.Reference);
            PdfDictionary previousSigDictCopy = new PdfDictionary((PdfDictionary)prevSigDict);
            previousSigDictCopy.Remove(PdfName.Reference);
            // Apart from the reference, dictionaries are expected to be equal.
            if (!ComparePdfObjects(previousSigDictCopy, currentSigDictCopy, usuallyModifiedObjects)) {
                return false;
            }
            PdfArray previousReference = ((PdfDictionary)prevSigDict).GetAsArray(PdfName.Reference);
            PdfArray currentReference = ((PdfDictionary)curSigDict).GetAsArray(PdfName.Reference);
            return CompareSignatureReferenceDictionaries(previousReference, currentReference, report);
        }

        private bool CompareSignatureReferenceDictionaries(PdfArray previousReferences, PdfArray currentReferences
            , ValidationReport report) {
            if (previousReferences == null || ComparePdfObjects(previousReferences, currentReferences, usuallyModifiedObjects
                )) {
                return true;
            }
            if (currentReferences == null || previousReferences.Size() != currentReferences.Size()) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, REFERENCE_REMOVED, ReportItem.ReportItemStatus.INVALID)
                    );
                return false;
            }
            else {
                for (int i = 0; i < previousReferences.Size(); ++i) {
                    PdfDictionary currentReferenceCopy = new PdfDictionary(currentReferences.GetAsDictionary(i));
                    currentReferenceCopy.Remove(PdfName.Data);
                    PdfDictionary previousReferenceCopy = new PdfDictionary(previousReferences.GetAsDictionary(i));
                    previousReferenceCopy.Remove(PdfName.Data);
                    // Apart from the data, dictionaries are expected to be equal. Data is an indirect reference
                    // to the object in the document upon which the object modification analysis should be performed.
                    if (!ComparePdfObjects(previousReferenceCopy, currentReferenceCopy, usuallyModifiedObjects) || !CompareIndirectReferencesObjNums
                        (previousReferences.GetAsDictionary(i).Get(PdfName.Data), currentReferences.GetAsDictionary(i).Get(PdfName
                        .Data), report, "Data entry in the signature reference dictionary")) {
                        report.AddReportItem(new ReportItem(DOC_MDP_CHECK, REFERENCE_REMOVED, ReportItem.ReportItemStatus.INVALID)
                            );
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CompareWidgets(PdfArray prevFields, PdfArray currFields, ValidationReport report) {
            if (GetAccessPermissions() == AccessPermissions.ANNOTATION_MODIFICATION) {
                return true;
            }
            IList<PdfDictionary> prevAnnots = PopulateWidgetAnnotations(prevFields);
            IList<PdfDictionary> currAnnots = PopulateWidgetAnnotations(currFields);
            if (prevAnnots.Count != currAnnots.Count) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, ANNOTATIONS_MODIFIED, ReportItem.ReportItemStatus.INVALID
                    ));
                return false;
            }
            for (int i = 0; i < prevAnnots.Count; i++) {
                PdfDictionary prevAnnot = new PdfDictionary(prevAnnots[i]);
                RemoveAppearanceRelatedProperties(prevAnnot);
                PdfDictionary currAnnot = new PdfDictionary(currAnnots[i]);
                RemoveAppearanceRelatedProperties(currAnnot);
                if (!ComparePdfObjects(prevAnnot, currAnnot, usuallyModifiedObjects) || !CompareIndirectReferencesObjNums(
                    prevAnnots[i].Get(PdfName.P), currAnnots[i].Get(PdfName.P), report, "Page object with which annotation is associated"
                    ) || !CompareIndirectReferencesObjNums(prevAnnots[i].Get(PdfName.Parent), currAnnots[i].Get(PdfName.Parent
                    ), report, "Annotation parent")) {
                    report.AddReportItem(new ReportItem(DOC_MDP_CHECK, ANNOTATIONS_MODIFIED, ReportItem.ReportItemStatus.INVALID
                        ));
                    return false;
                }
                checkedAnnots.Add(prevAnnots[i]);
                checkedAnnots.Add(currAnnots[i]);
            }
            return true;
        }

        private bool ComparePages(PdfDictionary prevPages, PdfDictionary currPages, ValidationReport report) {
            if (prevPages == null ^ currPages == null) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, PAGES_MODIFIED, ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            if (prevPages == null) {
                return true;
            }
            PdfDictionary previousPagesCopy = new PdfDictionary(prevPages);
            previousPagesCopy.Remove(PdfName.Kids);
            previousPagesCopy.Remove(PdfName.Parent);
            PdfDictionary currentPagesCopy = new PdfDictionary(currPages);
            currentPagesCopy.Remove(PdfName.Kids);
            currentPagesCopy.Remove(PdfName.Parent);
            if (!ComparePdfObjects(previousPagesCopy, currentPagesCopy, usuallyModifiedObjects) || !CompareIndirectReferencesObjNums
                (prevPages.Get(PdfName.Parent), currPages.Get(PdfName.Parent), report, "Page tree node parent")) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, PAGES_MODIFIED, ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            PdfArray prevKids = prevPages.GetAsArray(PdfName.Kids);
            PdfArray currKids = currPages.GetAsArray(PdfName.Kids);
            if (prevKids.Size() != currKids.Size()) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, PAGES_MODIFIED, ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            for (int i = 0; i < currKids.Size(); ++i) {
                PdfDictionary previousKid = prevKids.GetAsDictionary(i);
                PdfDictionary currentKid = currKids.GetAsDictionary(i);
                if (PdfName.Pages.Equals(previousKid.GetAsName(PdfName.Type))) {
                    // Compare page tree nodes.
                    if (!ComparePages(previousKid, currentKid, report)) {
                        return false;
                    }
                }
                else {
                    // Compare page objects (leaf node in the page tree).
                    PdfDictionary previousPageCopy = new PdfDictionary(previousKid);
                    previousPageCopy.Remove(PdfName.Annots);
                    previousPageCopy.Remove(PdfName.Parent);
                    previousPageCopy.Remove(PdfName.StructParents);
                    previousPageCopy.Remove(PdfName.Tabs);
                    PdfDictionary currentPageCopy = new PdfDictionary(currentKid);
                    currentPageCopy.Remove(PdfName.Annots);
                    currentPageCopy.Remove(PdfName.Parent);
                    currentPageCopy.Remove(PdfName.StructParents);
                    currentPageCopy.Remove(PdfName.Tabs);
                    if (!ComparePdfObjects(previousPageCopy, currentPageCopy, usuallyModifiedObjects) || !CompareIndirectReferencesObjNums
                        (previousKid.Get(PdfName.Parent), currentKid.Get(PdfName.Parent), report, "Page parent")) {
                        report.AddReportItem(new ReportItem(DOC_MDP_CHECK, PAGE_MODIFIED, ReportItem.ReportItemStatus.INVALID));
                        return false;
                    }
                    if (!CompareTabs(previousKid.GetAsName(PdfName.Tabs), currentKid.GetAsName(PdfName.Tabs))) {
                        report.AddReportItem(new ReportItem(DOC_MDP_CHECK, TABS_MODIFIED, ReportItem.ReportItemStatus.INVALID));
                        return false;
                    }
                    PdfArray prevNotModifiableAnnots = GetAnnotsNotAllowedToBeModified(previousKid);
                    PdfArray currNotModifiableAnnots = GetAnnotsNotAllowedToBeModified(currentKid);
                    if (!ComparePageAnnotations(prevNotModifiableAnnots, currNotModifiableAnnots, report)) {
                        report.AddReportItem(new ReportItem(DOC_MDP_CHECK, PAGE_ANNOTATIONS_MODIFIED, ReportItem.ReportItemStatus.
                            INVALID));
                        return false;
                    }
                    CollectRemovedAndAddedAnnotations(previousKid.GetAsArray(PdfName.Annots), currentKid.GetAsArray(PdfName.Annots
                        ));
                }
            }
            return true;
        }

        private bool CompareTabs(PdfName previousTabs, PdfName currentTabs) {
            if (GetAccessPermissions() == AccessPermissions.ANNOTATION_MODIFICATION) {
                return true;
            }
            return Object.Equals(previousTabs, currentTabs) || (previousTabs == null && currentTabs.Equals(PdfName.S));
        }

        private void CollectRemovedAndAddedAnnotations(PdfArray previousAnnotations, PdfArray currentAnnotations) {
            ValidationReport dummyReport = new ValidationReport();
            IList<PdfDictionary> prevAnnots = new List<PdfDictionary>();
            if (previousAnnotations != null) {
                foreach (PdfObject annot in previousAnnotations) {
                    if (annot is PdfDictionary) {
                        prevAnnots.Add((PdfDictionary)annot);
                    }
                }
            }
            IList<PdfDictionary> currAnnots = new List<PdfDictionary>();
            if (currentAnnotations != null) {
                foreach (PdfObject annot in currentAnnotations) {
                    if (annot is PdfDictionary) {
                        currAnnots.Add((PdfDictionary)annot);
                    }
                }
            }
            // Each previous annotation which is not present in current annotations is considered to be removed.
            removedTaggedObjects.AddAll(prevAnnots.Where((prevAnnot) => !currAnnots.Any((currAnnot) => ComparePageAnnotations
                (prevAnnot, currAnnot, dummyReport))).ToList());
            // Each current annotation which is not present in previous annotations is considered to be added.
            addedTaggedObjects.AddAll(currAnnots.Where((currAnnot) => !prevAnnots.Any((prevAnnot) => ComparePageAnnotations
                (prevAnnot, currAnnot, dummyReport))).ToList());
        }

        // Logic above additionally collects modified annotations as added and removed. This is expected.
        private bool ComparePageAnnotations(PdfArray prevAnnots, PdfArray currAnnots, ValidationReport report) {
            if (prevAnnots == null && currAnnots == null) {
                return true;
            }
            if (prevAnnots == null || currAnnots == null || prevAnnots.Size() != currAnnots.Size()) {
                return false;
            }
            for (int i = 0; i < prevAnnots.Size(); i++) {
                PdfDictionary prevAnnot = prevAnnots.GetAsDictionary(i);
                PdfDictionary currAnnot = currAnnots.GetAsDictionary(i);
                if (!ComparePageAnnotations(prevAnnot, currAnnot, report)) {
                    return false;
                }
            }
            return true;
        }

        private bool ComparePageAnnotations(PdfDictionary prevAnnot, PdfDictionary currAnnot, ValidationReport report
            ) {
            PdfDictionary prevAnnotCopy = new PdfDictionary(prevAnnot);
            prevAnnotCopy.Remove(PdfName.P);
            prevAnnotCopy.Remove(PdfName.Parent);
            PdfDictionary currAnnotCopy = new PdfDictionary(currAnnot);
            currAnnotCopy.Remove(PdfName.P);
            currAnnotCopy.Remove(PdfName.Parent);
            if (PdfName.Sig.Equals(currAnnot.GetAsName(PdfName.FT))) {
                if (!CompareSignatureDictionaries(prevAnnot.Get(PdfName.V), currAnnot.Get(PdfName.V), report)) {
                    return false;
                }
                else {
                    prevAnnotCopy.Remove(PdfName.V);
                    currAnnotCopy.Remove(PdfName.V);
                }
            }
            return ComparePdfObjects(prevAnnotCopy, currAnnotCopy, usuallyModifiedObjects) && CompareIndirectReferencesObjNums
                (prevAnnot.Get(PdfName.P), currAnnot.Get(PdfName.P), report, "Page object with which annotation is associated"
                ) && CompareIndirectReferencesObjNums(prevAnnot.Get(PdfName.Parent), currAnnot.Get(PdfName.Parent), report
                , "Annotation parent");
        }

        // Compare catalogs util methods section:
        private bool CompareIndirectReferencesObjNums(PdfObject prevObj, PdfObject currObj, ValidationReport report
            , String type) {
            if (prevObj == null ^ currObj == null) {
                return false;
            }
            if (prevObj == null) {
                return true;
            }
            PdfIndirectReference prevObjRef = prevObj.GetIndirectReference();
            PdfIndirectReference currObjRef = currObj.GetIndirectReference();
            if (prevObjRef == null || currObjRef == null) {
                if (report != null) {
                    report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(DIRECT_OBJECT, type), ReportItem.ReportItemStatus
                        .INVALID));
                }
                return false;
            }
            return IsSameReference(prevObjRef, currObjRef);
        }

        /// <summary>
        /// DocMDP level &lt;=2 allows adding new fields in the following cases:
        /// docMDP level 1: allows adding only DocTimeStamp signature fields;
        /// docMDP level 2: same as level 1 and also adding and then signing signature fields,
        /// so signature dictionary shouldn't be null.
        /// </summary>
        /// <param name="field">newly added field entry</param>
        /// <param name="report">validation report</param>
        /// <returns>true if newly added field is allowed to be added, false otherwise.</returns>
        private bool IsAllowedSignatureField(PdfDictionary field, ValidationReport report) {
            PdfDictionary value = field.GetAsDictionary(PdfName.V);
            if (!PdfName.Sig.Equals(field.GetAsName(PdfName.FT)) || value == null || (GetAccessPermissions() == AccessPermissions
                .NO_CHANGES_PERMITTED && !PdfName.DocTimeStamp.Equals(value.GetAsName(PdfName.Type)))) {
                PdfString fieldName = field.GetAsString(PdfName.T);
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(UNEXPECTED_FORM_FIELD, fieldName
                     == null ? "" : fieldName.GetValue()), ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            if (PdfFormAnnotationUtil.IsPureWidgetOrMergedField(field)) {
                checkedAnnots.Add(field);
            }
            else {
                PdfArray kids = field.GetAsArray(PdfName.Kids);
                checkedAnnots.AddAll(PopulateWidgetAnnotations(kids));
            }
            newlyAddedFields.Add(field);
            return true;
        }

        private ICollection<PdfDictionary> PopulateFormFields(PdfArray fieldsArray) {
            ICollection<PdfDictionary> fields = new HashSet<PdfDictionary>();
            if (fieldsArray != null) {
                for (int i = 0; i < fieldsArray.Size(); ++i) {
                    PdfDictionary fieldDict = (PdfDictionary)fieldsArray.Get(i);
                    if (PdfFormField.IsFormField(fieldDict)) {
                        fields.Add(fieldDict);
                    }
                }
            }
            return fields;
        }

        private IList<PdfDictionary> PopulateWidgetAnnotations(PdfArray fieldsArray) {
            IList<PdfDictionary> annotations = new List<PdfDictionary>();
            if (fieldsArray != null) {
                for (int i = 0; i < fieldsArray.Size(); ++i) {
                    PdfDictionary annotDict = (PdfDictionary)fieldsArray.Get(i);
                    if (PdfFormAnnotationUtil.IsPureWidget(annotDict)) {
                        annotations.Add(annotDict);
                    }
                }
            }
            return annotations;
        }

        private PdfArray GetAnnotsNotAllowedToBeModified(PdfDictionary page) {
            PdfArray annots = page.GetAsArray(PdfName.Annots);
            if (annots == null || GetAccessPermissions() == AccessPermissions.ANNOTATION_MODIFICATION) {
                return null;
            }
            PdfArray annotsCopy = new PdfArray(annots);
            foreach (PdfObject annot in annots) {
                // checkedAnnots contains all the fields' widget annotations from the Acroform which were already validated
                // during the compareAcroForms call, so we shouldn't check them once again
                if (checkedAnnots.Contains(annot)) {
                    annotsCopy.Remove(annot);
                }
            }
            return annotsCopy;
        }

        private PdfDictionary CopyCatalogEntriesToCompare(PdfDictionary catalog) {
            PdfDictionary catalogCopy = new PdfDictionary(catalog);
            catalogCopy.Remove(PdfName.Metadata);
            catalogCopy.Remove(PdfName.Extensions);
            catalogCopy.Remove(PdfName.Perms);
            catalogCopy.Remove(PdfName.DSS);
            catalogCopy.Remove(PdfName.AcroForm);
            catalogCopy.Remove(PdfName.Pages);
            catalogCopy.Remove(PdfName.StructTreeRoot);
            catalogCopy.Remove(PdfName.Version);
            return catalogCopy;
        }

        private PdfDictionary CopyAcroformDictionary(PdfDictionary acroForm) {
            PdfDictionary acroFormCopy = new PdfDictionary(acroForm);
            acroFormCopy.Remove(PdfName.Fields);
            acroFormCopy.Remove(PdfName.DR);
            acroFormCopy.Remove(PdfName.DA);
            return acroFormCopy;
        }

        private PdfDictionary CopyFieldDictionary(PdfDictionary field) {
            PdfDictionary formDict = new PdfDictionary(field);
            formDict.Remove(PdfName.V);
            // Value for the choice fields could be specified by the /I key.
            formDict.Remove(PdfName.I);
            formDict.Remove(PdfName.Parent);
            formDict.Remove(PdfName.Kids);
            // Remove also annotation related properties (e.g. in case of the merged field).
            RemoveAppearanceRelatedProperties(formDict);
            return formDict;
        }

        private void RemoveAppearanceRelatedProperties(PdfDictionary annotDict) {
            annotDict.Remove(PdfName.P);
            annotDict.Remove(PdfName.Parent);
            if (GetAccessPermissions() == AccessPermissions.FORM_FIELDS_MODIFICATION) {
                annotDict.Remove(PdfName.AP);
                annotDict.Remove(PdfName.AS);
                annotDict.Remove(PdfName.M);
                annotDict.Remove(PdfName.F);
            }
            if (GetAccessPermissions() == AccessPermissions.ANNOTATION_MODIFICATION) {
                foreach (PdfName key in new PdfDictionary(annotDict).KeySet()) {
                    if (!PdfFormField.GetFormFieldKeys().Contains(key)) {
                        annotDict.Remove(key);
                    }
                }
            }
        }

        //
        //
        // Compare PDF objects util section:
        //
        //
        private static bool ComparePdfObjects(PdfObject pdfObject1, PdfObject pdfObject2, Tuple2<ICollection<PdfIndirectReference
            >, ICollection<PdfIndirectReference>> usuallyModifiedObjects) {
            return ComparePdfObjects(pdfObject1, pdfObject2, new List<Tuple2<PdfObject, PdfObject>>(), usuallyModifiedObjects
                );
        }

        private static bool ComparePdfObjects(PdfObject pdfObject1, PdfObject pdfObject2, IList<Tuple2<PdfObject, 
            PdfObject>> visitedObjects, Tuple2<ICollection<PdfIndirectReference>, ICollection<PdfIndirectReference
            >> usuallyModifiedObjects) {
            foreach (Tuple2<PdfObject, PdfObject> pair in visitedObjects) {
                if (pair.GetFirst() == pdfObject1) {
                    return pair.GetSecond() == pdfObject2;
                }
            }
            visitedObjects.Add(new Tuple2<PdfObject, PdfObject>(pdfObject1, pdfObject2));
            if (Object.Equals(pdfObject1, pdfObject2)) {
                return true;
            }
            if (pdfObject1 == null || pdfObject2 == null) {
                return false;
            }
            if (pdfObject1.GetType() != pdfObject2.GetType()) {
                return false;
            }
            if (pdfObject1.GetIndirectReference() != null && usuallyModifiedObjects.GetFirst().Any((reference) => IsSameReference
                (reference, pdfObject1.GetIndirectReference())) && pdfObject2.GetIndirectReference() != null && usuallyModifiedObjects
                .GetSecond().Any((reference) => IsSameReference(reference, pdfObject2.GetIndirectReference()))) {
                // These two objects are expected to not be completely equal, we check them independently.
                // However, we still need to make sure those are same instances.
                return IsSameReference(pdfObject1.GetIndirectReference(), pdfObject2.GetIndirectReference());
            }
            // We don't allow objects to change from being direct to indirect and vice versa.
            // Acrobat allows it, but such change can invalidate the document.
            if (pdfObject1.GetIndirectReference() == null ^ pdfObject2.GetIndirectReference() == null) {
                return false;
            }
            switch (pdfObject1.GetObjectType()) {
                case PdfObject.BOOLEAN:
                case PdfObject.NAME:
                case PdfObject.NULL:
                case PdfObject.LITERAL:
                case PdfObject.NUMBER:
                case PdfObject.STRING: {
                    return pdfObject1.Equals(pdfObject2);
                }

                case PdfObject.INDIRECT_REFERENCE: {
                    return ComparePdfObjects(((PdfIndirectReference)pdfObject1).GetRefersTo(), ((PdfIndirectReference)pdfObject2
                        ).GetRefersTo(), visitedObjects, usuallyModifiedObjects);
                }

                case PdfObject.ARRAY: {
                    return ComparePdfArrays((PdfArray)pdfObject1, (PdfArray)pdfObject2, visitedObjects, usuallyModifiedObjects
                        );
                }

                case PdfObject.DICTIONARY: {
                    return ComparePdfDictionaries((PdfDictionary)pdfObject1, (PdfDictionary)pdfObject2, visitedObjects, usuallyModifiedObjects
                        );
                }

                case PdfObject.STREAM: {
                    return ComparePdfStreams((PdfStream)pdfObject1, (PdfStream)pdfObject2, visitedObjects, usuallyModifiedObjects
                        );
                }

                default: {
                    return false;
                }
            }
        }

        private static bool ComparePdfArrays(PdfArray array1, PdfArray array2, IList<Tuple2<PdfObject, PdfObject>>
             visitedObjects, Tuple2<ICollection<PdfIndirectReference>, ICollection<PdfIndirectReference>> usuallyModifiedObjects
            ) {
            if (array1.Size() != array2.Size()) {
                return false;
            }
            for (int i = 0; i < array1.Size(); i++) {
                if (!ComparePdfObjects(array1.Get(i), array2.Get(i), visitedObjects, usuallyModifiedObjects)) {
                    return false;
                }
            }
            return true;
        }

        private static bool ComparePdfDictionaries(PdfDictionary dictionary1, PdfDictionary dictionary2, IList<Tuple2
            <PdfObject, PdfObject>> visitedObjects, Tuple2<ICollection<PdfIndirectReference>, ICollection<PdfIndirectReference
            >> usuallyModifiedObjects) {
            ICollection<KeyValuePair<PdfName, PdfObject>> entrySet1 = dictionary1.EntrySet();
            ICollection<KeyValuePair<PdfName, PdfObject>> entrySet2 = dictionary2.EntrySet();
            if (entrySet1.Count != entrySet2.Count) {
                return false;
            }
            foreach (KeyValuePair<PdfName, PdfObject> entry1 in entrySet1) {
                if (!entrySet2.Any((entry2) => entry2.Key.Equals(entry1.Key) && ComparePdfObjects(entry2.Value, entry1.Value
                    , visitedObjects, usuallyModifiedObjects))) {
                    return false;
                }
            }
            return true;
        }

        private static bool ComparePdfStreams(PdfStream stream1, PdfStream stream2, IList<Tuple2<PdfObject, PdfObject
            >> visitedObjects, Tuple2<ICollection<PdfIndirectReference>, ICollection<PdfIndirectReference>> usuallyModifiedObjects
            ) {
            return JavaUtil.ArraysEquals(stream1.GetBytes(), stream2.GetBytes()) && ComparePdfDictionaries(stream1, stream2
                , visitedObjects, usuallyModifiedObjects);
        }

        private static bool IsSameReference(PdfIndirectReference indirectReference1, PdfIndirectReference indirectReference2
            ) {
            if (indirectReference1 == indirectReference2) {
                return true;
            }
            if (indirectReference1 == null || indirectReference2 == null) {
                return false;
            }
            return indirectReference1.GetObjNumber() == indirectReference2.GetObjNumber() && indirectReference1.GetGenNumber
                () == indirectReference2.GetGenNumber();
        }

        private static bool IsMaxGenerationObject(PdfIndirectReference indirectReference) {
            return indirectReference.GetObjNumber() == 0 && indirectReference.GetGenNumber() == 65535;
        }

        //
        //
        // Allowed references section:
        //
        //
        private ICollection<PdfIndirectReference> CreateUsuallyModifiedObjectsSet(PdfDocument document) {
            ICollection<PdfIndirectReference> usuallyModifiedObjectsSet = new HashSet<PdfIndirectReference>();
            usuallyModifiedObjectsSet.Add(document.GetCatalog().GetPdfObject().GetIndirectReference());
            PdfDictionary catalog = document.GetCatalog().GetPdfObject();
            if (catalog.Get(PdfName.Pages) != null) {
                usuallyModifiedObjectsSet.Add(catalog.Get(PdfName.Pages).GetIndirectReference());
                if (catalog.GetAsDictionary(PdfName.Pages) != null) {
                    AddPageEntriesToSet(catalog.GetAsDictionary(PdfName.Pages), usuallyModifiedObjectsSet);
                }
            }
            if (catalog.Get(PdfName.StructTreeRoot) != null) {
                usuallyModifiedObjectsSet.Add(catalog.Get(PdfName.StructTreeRoot).GetIndirectReference());
                if (catalog.GetAsDictionary(PdfName.StructTreeRoot) != null && catalog.GetAsDictionary(PdfName.StructTreeRoot
                    ).Get(PdfName.K) != null) {
                    AddStructTreeElementsToSet(catalog.GetAsDictionary(PdfName.StructTreeRoot).Get(PdfName.K), usuallyModifiedObjectsSet
                        );
                }
            }
            return usuallyModifiedObjectsSet;
        }

        private void AddStructTreeElementsToSet(PdfObject structTreeRootKids, ICollection<PdfIndirectReference> set
            ) {
            if (structTreeRootKids is PdfArray) {
                set.Add(structTreeRootKids.GetIndirectReference());
                AddStructTreeElementsToSet((PdfArray)structTreeRootKids, set);
            }
            else {
                AddStructTreeElementsToSet(new PdfArray(structTreeRootKids), set);
            }
        }

        private void AddStructTreeElementsToSet(PdfArray structTreeRootKids, ICollection<PdfIndirectReference> set
            ) {
            foreach (PdfObject kid in structTreeRootKids) {
                if (kid != null) {
                    set.Add(kid.GetIndirectReference());
                    if (IsStructTreeElement(kid) && ((PdfDictionary)kid).Get(PdfName.K) != null) {
                        AddStructTreeElementsToSet(((PdfDictionary)kid).Get(PdfName.K), set);
                    }
                }
            }
        }

        private void AddPageEntriesToSet(PdfDictionary page, ICollection<PdfIndirectReference> set) {
            PdfArray kids = page.GetAsArray(PdfName.Kids);
            if (kids != null) {
                set.Add(kids.GetIndirectReference());
                for (int i = 0; i < kids.Size(); ++i) {
                    set.Add(kids.Get(i).GetIndirectReference());
                    PdfDictionary pageNode = kids.GetAsDictionary(i);
                    if (pageNode != null && PdfName.Pages.Equals(pageNode.GetAsName(PdfName.Type))) {
                        AddPageEntriesToSet(pageNode, set);
                    }
                }
            }
        }

        private ICollection<PdfIndirectReference> CreateAllowedReferences(PdfDocument document) {
            // Each indirect reference in the set is an allowed reference to be present in the new xref table
            // or the same entry in the previous document.
            // If any reference is null, we expect this object to be newly generated or direct reference.
            ICollection<PdfIndirectReference> allowedReferences = new HashSet<PdfIndirectReference>();
            if (document.GetTrailer().Get(PdfName.Info) != null) {
                allowedReferences.Add(document.GetTrailer().Get(PdfName.Info).GetIndirectReference());
            }
            if (document.GetCatalog().GetPdfObject() == null) {
                return allowedReferences;
            }
            allowedReferences.Add(document.GetCatalog().GetPdfObject().GetIndirectReference());
            if (document.GetCatalog().GetPdfObject().Get(PdfName.Metadata) != null) {
                allowedReferences.Add(document.GetCatalog().GetPdfObject().Get(PdfName.Metadata).GetIndirectReference());
            }
            PdfDictionary dssDictionary = document.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.DSS);
            if (dssDictionary != null) {
                allowedReferences.Add(dssDictionary.GetIndirectReference());
                allowedReferences.AddAll(CreateAllowedDssEntries(document));
            }
            PdfDictionary acroForm = document.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.AcroForm);
            if (acroForm != null) {
                allowedReferences.Add(acroForm.GetIndirectReference());
                PdfArray fields = acroForm.GetAsArray(PdfName.Fields);
                CreateAllowedFormFieldEntries(fields, allowedReferences);
                PdfDictionary resources = acroForm.GetAsDictionary(PdfName.DR);
                if (resources != null) {
                    allowedReferences.Add(resources.GetIndirectReference());
                    AddAllNestedDictionaryEntries(allowedReferences, resources);
                }
            }
            PdfDictionary pagesDictionary = document.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.Pages);
            if (pagesDictionary != null) {
                allowedReferences.Add(pagesDictionary.GetIndirectReference());
                allowedReferences.AddAll(CreateAllowedPagesEntries(pagesDictionary));
            }
            PdfDictionary structTreeRoot = document.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.StructTreeRoot
                );
            if (structTreeRoot != null) {
                allowedReferences.Add(structTreeRoot.GetIndirectReference());
                allowedReferences.AddAll(CreateAllowedStructTreeRootEntries(structTreeRoot));
            }
            return allowedReferences;
        }

        private bool CheckAllowedReferences(ICollection<PdfIndirectReference> currentAllowedReferences, ICollection
            <PdfIndirectReference> previousAllowedReferences, PdfIndirectReference indirectReference, PdfDocument 
            documentWithoutRevision) {
            foreach (PdfIndirectReference currentAllowedReference in currentAllowedReferences) {
                if (IsSameReference(currentAllowedReference, indirectReference)) {
                    return documentWithoutRevision.GetPdfObject(indirectReference.GetObjNumber()) == null || previousAllowedReferences
                        .Any((reference) => IsSameReference(reference, indirectReference));
                }
            }
            return false;
        }

        private bool IsAllowedStreamObj(PdfIndirectReference indirectReference, PdfDocument document) {
            PdfObject pdfObject = document.GetPdfObject(indirectReference.GetObjNumber());
            if (pdfObject is PdfStream) {
                PdfName type = ((PdfStream)pdfObject).GetAsName(PdfName.Type);
                return PdfName.XRef.Equals(type) || PdfName.ObjStm.Equals(type);
            }
            return false;
        }

        // Allowed references creation nested methods section:
        private ICollection<PdfIndirectReference> CreateAllowedDssEntries(PdfDocument document) {
            ICollection<PdfIndirectReference> allowedReferences = new HashSet<PdfIndirectReference>();
            PdfDictionary dssDictionary = document.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.DSS);
            PdfArray certs = dssDictionary.GetAsArray(PdfName.Certs);
            if (certs != null) {
                allowedReferences.Add(certs.GetIndirectReference());
                for (int i = 0; i < certs.Size(); ++i) {
                    allowedReferences.Add(certs.Get(i).GetIndirectReference());
                }
            }
            PdfArray ocsps = dssDictionary.GetAsArray(PdfName.OCSPs);
            if (ocsps != null) {
                allowedReferences.Add(ocsps.GetIndirectReference());
                for (int i = 0; i < ocsps.Size(); ++i) {
                    allowedReferences.Add(ocsps.Get(i).GetIndirectReference());
                }
            }
            PdfArray crls = dssDictionary.GetAsArray(PdfName.CRLs);
            if (crls != null) {
                allowedReferences.Add(crls.GetIndirectReference());
                for (int i = 0; i < crls.Size(); ++i) {
                    allowedReferences.Add(crls.Get(i).GetIndirectReference());
                }
            }
            PdfDictionary vris = dssDictionary.GetAsDictionary(PdfName.VRI);
            if (vris != null) {
                allowedReferences.Add(vris.GetIndirectReference());
                foreach (KeyValuePair<PdfName, PdfObject> vri in vris.EntrySet()) {
                    allowedReferences.Add(vri.Value.GetIndirectReference());
                    if (vri.Value is PdfDictionary) {
                        PdfDictionary vriDictionary = (PdfDictionary)vri.Value;
                        PdfArray vriCerts = vriDictionary.GetAsArray(PdfName.Cert);
                        if (vriCerts != null) {
                            allowedReferences.Add(vriCerts.GetIndirectReference());
                            for (int i = 0; i < vriCerts.Size(); ++i) {
                                allowedReferences.Add(vriCerts.Get(i).GetIndirectReference());
                            }
                        }
                        PdfArray vriOcsps = vriDictionary.GetAsArray(PdfName.OCSP);
                        if (vriOcsps != null) {
                            allowedReferences.Add(vriOcsps.GetIndirectReference());
                            for (int i = 0; i < vriOcsps.Size(); ++i) {
                                allowedReferences.Add(vriOcsps.Get(i).GetIndirectReference());
                            }
                        }
                        PdfArray vriCrls = vriDictionary.GetAsArray(PdfName.CRL);
                        if (vriCrls != null) {
                            allowedReferences.Add(vriCrls.GetIndirectReference());
                            for (int i = 0; i < vriCrls.Size(); ++i) {
                                allowedReferences.Add(vriCrls.Get(i).GetIndirectReference());
                            }
                        }
                        if (vriDictionary.Get(new PdfName("TS")) != null) {
                            allowedReferences.Add(vriDictionary.Get(new PdfName("TS")).GetIndirectReference());
                        }
                    }
                }
            }
            return allowedReferences;
        }

        private ICollection<PdfIndirectReference> CreateAllowedStructTreeRootEntries(PdfDictionary structTreeRoot) {
            ICollection<PdfIndirectReference> allowedReferences = new HashSet<PdfIndirectReference>();
            PdfDictionary idTree = structTreeRoot.GetAsDictionary(PdfName.IDTree);
            if (idTree != null) {
                CreateAllowedTreeEntries(idTree, allowedReferences, PdfName.Names);
            }
            PdfDictionary parentTree = structTreeRoot.GetAsDictionary(PdfName.ParentTree);
            if (parentTree != null) {
                CreateAllowedTreeEntries(parentTree, allowedReferences, PdfName.Nums);
            }
            if (structTreeRoot.Get(PdfName.K) != null) {
                allowedReferences.Add(structTreeRoot.Get(PdfName.K).GetIndirectReference());
                CreateAllowedStructTreeRootKidsEntries(structTreeRoot.Get(PdfName.K), allowedReferences);
            }
            return allowedReferences;
        }

        private void CreateAllowedTreeEntries(PdfObject treeNode, ICollection<PdfIndirectReference> allowedReferences
            , PdfName contentName) {
            if (!(treeNode is PdfDictionary)) {
                return;
            }
            PdfDictionary treeNodeDictionary = (PdfDictionary)treeNode;
            allowedReferences.Add(treeNodeDictionary.GetIndirectReference());
            PdfArray content = treeNodeDictionary.GetAsArray(contentName);
            if (content != null) {
                allowedReferences.Add(content.GetIndirectReference());
                for (int i = 1; i < content.Size(); i += 2) {
                    // This object can either be an array or actual content element.
                    // We only add allowed reference in case of an array.
                    PdfArray contentArray = content.GetAsArray(i);
                    if (contentArray != null) {
                        allowedReferences.Add(contentArray.GetIndirectReference());
                    }
                }
            }
            PdfArray kids = treeNodeDictionary.GetAsArray(PdfName.Kids);
            if (kids != null) {
                allowedReferences.Add(kids.GetIndirectReference());
                foreach (PdfObject kid in kids) {
                    CreateAllowedTreeEntries(kid, allowedReferences, contentName);
                }
            }
        }

        private void CreateAllowedStructTreeRootKidsEntries(PdfObject structTreeRootKids, ICollection<PdfIndirectReference
            > allowedReferences) {
            if (structTreeRootKids is PdfArray) {
                allowedReferences.Add(structTreeRootKids.GetIndirectReference());
                CreateAllowedStructTreeRootKidsEntries((PdfArray)structTreeRootKids, allowedReferences);
            }
            else {
                CreateAllowedStructTreeRootKidsEntries(new PdfArray(structTreeRootKids), allowedReferences);
            }
        }

        private void CreateAllowedStructTreeRootKidsEntries(PdfArray structTreeRootKids, ICollection<PdfIndirectReference
            > allowedReferences) {
            foreach (PdfObject kid in structTreeRootKids) {
                if (kid != null) {
                    allowedReferences.Add(kid.GetIndirectReference());
                    if (IsStructTreeElement(kid)) {
                        PdfDictionary structTreeElementCopy = new PdfDictionary((PdfDictionary)kid);
                        PdfObject kids = structTreeElementCopy.Remove(PdfName.K);
                        structTreeElementCopy.Remove(PdfName.P);
                        structTreeElementCopy.Remove(PdfName.Ref);
                        structTreeElementCopy.Remove(PdfName.Pg);
                        AddAllNestedDictionaryEntries(allowedReferences, structTreeElementCopy);
                        if (kids != null) {
                            CreateAllowedStructTreeRootKidsEntries(kids, allowedReferences);
                        }
                    }
                }
            }
        }

        private ICollection<PdfIndirectReference> CreateAllowedPagesEntries(PdfDictionary pagesDictionary) {
            ICollection<PdfIndirectReference> allowedReferences = new HashSet<PdfIndirectReference>();
            PdfArray kids = pagesDictionary.GetAsArray(PdfName.Kids);
            if (kids != null) {
                allowedReferences.Add(kids.GetIndirectReference());
                for (int i = 0; i < kids.Size(); ++i) {
                    PdfDictionary pageNode = kids.GetAsDictionary(i);
                    allowedReferences.Add(kids.Get(i).GetIndirectReference());
                    if (pageNode != null) {
                        if (PdfName.Pages.Equals(pageNode.GetAsName(PdfName.Type))) {
                            allowedReferences.AddAll(CreateAllowedPagesEntries(pageNode));
                        }
                        else {
                            PdfObject annots = pageNode.Get(PdfName.Annots);
                            if (annots != null) {
                                allowedReferences.Add(annots.GetIndirectReference());
                                if (GetAccessPermissions() == AccessPermissions.ANNOTATION_MODIFICATION) {
                                    AddAllNestedArrayEntries(allowedReferences, (PdfArray)annots);
                                }
                            }
                        }
                    }
                }
            }
            return allowedReferences;
        }

        private void CreateAllowedFormFieldEntries(PdfArray fields, ICollection<PdfIndirectReference> allowedReferences
            ) {
            if (fields == null) {
                return;
            }
            foreach (PdfObject field in fields) {
                PdfDictionary fieldDict = (PdfDictionary)field;
                if (PdfFormField.IsFormField(fieldDict)) {
                    PdfObject value = fieldDict.Get(PdfName.V);
                    if (GetAccessPermissions() != AccessPermissions.NO_CHANGES_PERMITTED || (value is PdfDictionary && PdfName
                        .DocTimeStamp.Equals(((PdfDictionary)value).GetAsName(PdfName.Type)))) {
                        allowedReferences.Add(fieldDict.GetIndirectReference());
                        PdfString fieldName = PdfFormCreator.CreateFormField(fieldDict).GetFieldName();
                        if (newlyAddedFields.Contains(fieldDict)) {
                            // For newly generated form field all references are allowed to be added.
                            AddAllNestedDictionaryEntries(allowedReferences, fieldDict);
                        }
                        else {
                            if (fieldName == null || !lockedFields.Contains(fieldName.GetValue())) {
                                // For already existing form field only several entries are allowed to be updated.
                                if (value != null) {
                                    allowedReferences.Add(value.GetIndirectReference());
                                }
                                if (PdfFormAnnotationUtil.IsPureWidgetOrMergedField(fieldDict)) {
                                    AddWidgetAnnotation(allowedReferences, fieldDict);
                                }
                                else {
                                    PdfArray kids = fieldDict.GetAsArray(PdfName.Kids);
                                    CreateAllowedFormFieldEntries(kids, allowedReferences);
                                }
                            }
                        }
                    }
                }
                else {
                    // Add annotation.
                    AddWidgetAnnotation(allowedReferences, fieldDict);
                }
            }
        }

        private void AddWidgetAnnotation(ICollection<PdfIndirectReference> allowedReferences, PdfDictionary annotDict
            ) {
            allowedReferences.Add(annotDict.GetIndirectReference());
            if (GetAccessPermissions() == AccessPermissions.ANNOTATION_MODIFICATION) {
                PdfDictionary pureAnnotDict = new PdfDictionary(annotDict);
                foreach (PdfName key in annotDict.KeySet()) {
                    if (PdfFormField.GetFormFieldKeys().Contains(key)) {
                        pureAnnotDict.Remove(key);
                    }
                }
                AddAllNestedDictionaryEntries(allowedReferences, pureAnnotDict);
            }
            else {
                PdfObject appearance = annotDict.Get(PdfName.AP);
                if (appearance != null) {
                    allowedReferences.Add(appearance.GetIndirectReference());
                    if (appearance is PdfDictionary) {
                        AddAllNestedDictionaryEntries(allowedReferences, (PdfDictionary)appearance);
                    }
                }
                PdfObject appearanceState = annotDict.Get(PdfName.AS);
                if (appearanceState != null) {
                    allowedReferences.Add(appearanceState.GetIndirectReference());
                }
                PdfObject timeStamp = annotDict.Get(PdfName.M);
                if (timeStamp != null) {
                    allowedReferences.Add(timeStamp.GetIndirectReference());
                }
            }
        }

        private void AddAllNestedDictionaryEntries(ICollection<PdfIndirectReference> allowedReferences, PdfDictionary
             dictionary) {
            foreach (KeyValuePair<PdfName, PdfObject> entry in dictionary.EntrySet()) {
                PdfObject value = entry.Value;
                if (value.GetIndirectReference() != null && allowedReferences.Any((reference) => IsSameReference(reference
                    , value.GetIndirectReference()))) {
                    // Required to not end up in an infinite loop.
                    continue;
                }
                allowedReferences.Add(value.GetIndirectReference());
                if (value is PdfDictionary) {
                    AddAllNestedDictionaryEntries(allowedReferences, (PdfDictionary)value);
                }
                if (value is PdfArray) {
                    AddAllNestedArrayEntries(allowedReferences, (PdfArray)value);
                }
            }
        }

        private void AddAllNestedArrayEntries(ICollection<PdfIndirectReference> allowedReferences, PdfArray pdfArray
            ) {
            for (int i = 0; i < pdfArray.Size(); ++i) {
                PdfObject arrayEntry = pdfArray.Get(i);
                if (arrayEntry.GetIndirectReference() != null && allowedReferences.Any((reference) => IsSameReference(reference
                    , arrayEntry.GetIndirectReference()))) {
                    // Required to not end up in an infinite loop.
                    continue;
                }
                allowedReferences.Add(arrayEntry.GetIndirectReference());
                if (arrayEntry is PdfDictionary) {
                    AddAllNestedDictionaryEntries(allowedReferences, (PdfDictionary)arrayEntry);
                }
                if (arrayEntry is PdfArray) {
                    AddAllNestedArrayEntries(allowedReferences, (PdfArray)arrayEntry);
                }
            }
        }
    }
}
