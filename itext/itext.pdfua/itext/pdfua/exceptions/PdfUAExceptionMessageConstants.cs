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

namespace iText.Pdfua.Exceptions {
    /// <summary>Class that bundles all the error message templates as constants.</summary>
    public sealed class PdfUAExceptionMessageConstants {
        public const String ANNOTATION_OF_TYPE_0_SHOULD_HAVE_CONTENTS_OR_ALT_KEY = "Annotation of type {0} shall "
             + "have contents or alternate description (in the form of an Alt entry in the enclosing structure element).";

        public const String ANNOT_TRAP_NET_IS_NOT_PERMITTED = "Annotations of subtype TrapNet shall not be permitted.";

        public const String ARTIFACT_CANT_BE_INSIDE_REAL_CONTENT = "Content marked as artifact may only reside in Artifact content.";

        public const String CANNOT_FIND_PDF_UA_CHECKER_FOR_SPECIFIED_CONFORMANCE = "Cannot find an appropriate " +
             "PDF/UA checker for the specified conformance.";

        public const String CATALOG_SHOULD_CONTAIN_LANG_ENTRY = "Catalog dictionary should contain lang entry.";

        public const String CELL_CANT_BE_DETERMINED_ALGORITHMICALLY = "TD cell row:{0} col:{1} in table {2} does" 
            + " not contain a valid Headers attribute, and Headers for this cell cannot be determined algorithmically.";

        public const String CELL_HAS_INVALID_ROLE = "Cell: row {0} ({1}) col {2} has invalid role.";

        public const String CONTENT_IS_NOT_REAL_CONTENT_AND_NOT_ARTIFACT = "Content is neither marked as Artifact nor tagged as real content.";

        public const String CONTENT_NOT_REFERENCING_FE_NOTE = "Real content that refers to footnotes or endnotes "
             + "shall use the Ref entry on the referring structure element to reference the FENote.";

        public const String CONTENT_WITH_MCID_BUT_MCID_NOT_FOUND_IN_STRUCT_TREE_ROOT = "Content with MCID, but MCID wasn't found in StructTreeRoot.";

        public const String CT_OR_ALT_ENTRY_IS_MISSING_IN_MEDIA_CLIP = "CT or Alt entry is missing from the media "
             + "clip data dictionary.";

        public const String DOCUMENT_SHALL_CONTAIN_VALID_LANG_ENTRY = "Document does not contain valid lang entry.";

        public const String DOCUMENT_SHALL_CONTAIN_XMP_METADATA_STREAM = "Document shall contain a XMP metadata stream.";

        public const String DOCUMENT_USES_BOTH_H_AND_HN = "Document uses both H and H# tags.";

        public const String DOCUMENT_USES_H_TAG = "Document uses H tag: conforming files shall use the explicitly "
             + "numbered heading structure types (H1-Hn) and shall not use the H structure type.";

        public const String DOCUMENT_USES_NOTE_TAG = "Document uses Note tag: " + "conforming files shall not use Note structure type. Instead FENote structure type shall be used.";

        public const String DYNAMIC_XFA_FORMS_SHALL_NOT_BE_USED = "Dynamic XFA forms shall not be used.";

        public const String FE_NOTE_NOT_REFERENCING_CONTENT = "FENote structure element shall use the Ref entry " 
            + "to identify all citations that reference it.";

        public const String FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_F_KEY_AND_UF_KEY = "File specification dictionary shall contain f key and uf key.";

        public const String FONT_SHOULD_BE_EMBEDDED = "Following font(s) are not embedded: {0}";

        public const String FORMULA_SHALL_HAVE_ALT = "Formula tags shall include an alternative representation or "
             + "replacement text.";

        public const String GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE = "The '{0}' glyph either isn't defined in embedded font or doesn't have unicode mapping.";

        public const String H1_IS_SKIPPED = "Heading level 1 is skipped in a descending sequence of header levels.";

        public const String HN_IS_SKIPPED = "Heading level {0} is skipped in a descending sequence of header " + "levels.";

        public const String IMAGE_SHALL_HAVE_ALT = "Figure tags shall include an alternative representation or " +
             "replacement text. call com.itextpdf.kernel.pdf.tagutils.AccessibilityProperties#setActualText or com"
             + ".itextpdf.kernel.pdf.tagutils.AccessibilityProperties#setAlternateDescription to be PDF/UA compliant.";

        public const String INCORRECT_NOTE_TYPE_VALUE = "The value of the NoteType attribute shall be either \"Footnote\", \"Endnote\" or \"None\".";

        public const String INVALID_PDF_VERSION = "Specified document pdf version isn't supported in pdf/ua.";

        public const String LINK_ANNOTATION_SHOULD_HAVE_CONTENTS_KEY = "Annotation of type Link " + "shall contain an alternate description via their Contents key.";

        public const String LINK_ANNOT_IS_NOT_NESTED_WITHIN_LINK = "A link annotation is not nested within a <Link> tag.";

        public const String LIST_ITEM_CONTENT_HAS_INVALID_TAG = "Any real content within an LI structure element "
             + "that is not enclosed in a Lbl structure element shall be enclosed in an LBody structure element.";

        public const String LIST_NUMBERING_IS_NOT_SPECIFIED = "If Lbl structure elements are present, the " + "ListNumbering attribute shall be specified for the respective L structure element; "
             + "the value None shall not be used.";

        public const String MATH_NOT_CHILD_OF_FORMULA = "The math structure type shall occur only as a child of a Formula structure element.";

        public const String METADATA_SHALL_BE_PRESENT_IN_THE_CATALOG_DICTIONARY = "Metadata shall be present in the catalog dictionary";

        public const String METADATA_SHALL_CONTAIN_DC_TITLE_ENTRY = "Metadata shall contain dc:title entry.";

        public const String METADATA_SHALL_CONTAIN_UA_VERSION_IDENTIFIER = "Metadata shall contain correct pdfuaid:part version identifier.";

        public const String MISSING_FORM_FIELD_DESCRIPTION = "Document form fields missing both TU entry and " + "alternative description. For PdfFormfields use PdfFormfield#setAlternativeName"
             + "(\"Your alternative description\"); For the layout engine use Element#getAccesibilityProperties()"
             + ".setAlternateDescription(\"your alternative description\")";

        public const String MISSING_VIEWER_PREFERENCES = "ViewerPreferences dictionary of the Catalog dictionary "
             + "does not contain a DisplayDocTitle entry.";

        public const String MORE_THAN_ONE_H_TAG = "A node contains more than one H tag.";

        public const String NAME_ENTRY_IS_MISSING_OR_EMPTY_IN_OCG = "Name entry is missing or has " + "an empty string as its value in an Optional Content Configuration Dictionary.";

        public const String NON_UNIQUE_ID_ENTRY_IN_STRUCT_TREE_ROOT = "ID entry '{0}' shall be unique among all elements in the document’s structure hierarchy";

        public const String NOTE_TAG_SHALL_HAVE_ID_ENTRY = "Note tags shall include a unique ID entry.";

        public const String OCG_PROPERTIES_CONFIG_SHALL_BE_AN_ARRAY = "Optional Content properties " + "configs shall be an array.";

        public const String OCG_SHALL_NOT_CONTAIN_AS_ENTRY = "An AS entry appears in an Optional Content.";

        public const String ONE_OR_MORE_STANDARD_ROLE_REMAPPED = "One or more standard types are remapped.";

        public const String PAGE_WITH_ANNOT_DOES_NOT_HAVE_TABS_WITH_S = "A page with annotation(s) doesn't " + "contain Tabs key with S value.";

        public const String PRINTER_MARK_IS_NOT_PERMITTED = "Annotations of subtype PrinterMark shall not be" + " included in logical structure.";

        public const String P_VALUE_IS_ABSENT_IN_ENCRYPTION_DICTIONARY = "Permissions are absent " + "in pdf encryption dictionary.";

        public const String REAL_CONTENT_CANT_BE_INSIDE_ARTIFACT = "Content marked as content may not reside in Artifact content.";

        public const String REAL_CONTENT_INSIDE_ARTIFACT_OR_VICE_VERSA = "Tagged content is present inside content marked as Artifact or vice versa.";

        public const String STRUCTURE_TYPE_IS_ROLE_MAPPED_TO_OTHER_STRUCTURE_TYPE_IN_THE_SAME_NAMESPACE = "Structure type {0}:{1} is role mapped to other structure type in the same namespace.";

        public const String SUSPECTS_ENTRY_IN_MARK_INFO_DICTIONARY_SHALL_NOT_HAVE_A_VALUE_OF_TRUE = "Suspects entry in mark info dictionary shall not have a value of true.";

        public const String TABLE_CONTAINS_EMPTY_CELLS = "Cell: row {0} ({1}) col {2} is empty, each row should " 
            + "have the same amount of columns when taking into account spanning.";

        public const String TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING = "Tag hasn't been added before adding content to the canvas.";

        public const String TAG_MAPPING_DOESNT_TERMINATE_WITH_STANDARD_TYPE = "\"{0}\" tag mapping does not terminate with a standard type.";

        public const String TENTH_BIT_OF_P_VALUE_IN_ENCRYPTION_SHOULD_BE_NON_ZERO = "10th bit of P value of " + "Encryption dictionary should be 1 if the document is tagged.";

        public const String TOCI_SHALL_IDENTIFY_REF = "Each TOCI structure element shall contain the Ref entry, either directly on the TOCI structure element"
             + " itself or on at least one of its descendant structure elements.";

        public const String VIEWER_PREFERENCES_IS_FALSE = "ViewerPreferences dictionary of the Catalog dictionary "
             + "contains a DisplayDocTitle entry with a value of false.";

        public const String XFA_FORMS_SHALL_NOT_BE_PRESENT = "XFA forms shall not be present.";

        private PdfUAExceptionMessageConstants() {
        }
        // Empty constructor.
    }
}
