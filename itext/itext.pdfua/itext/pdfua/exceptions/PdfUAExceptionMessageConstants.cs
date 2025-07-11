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

        public const String ANNOT_CONTENTS_IS_NULL_OR_EMPTY = "Annotation shall specify an alternate " + "description or author's intent via its Contents key.";

        public const String ANNOT_TRAP_NET_IS_NOT_PERMITTED = "Annotations of subtype TrapNet shall not be permitted.";

        public const String ARTIFACT_CANT_BE_INSIDE_REAL_CONTENT = "Content marked as artifact may only reside in Artifact content.";

        public const String CANNOT_FIND_PDF_UA_CHECKER_FOR_SPECIFIED_CONFORMANCE = "Cannot find an appropriate " +
             "PDF/UA checker for the specified conformance.";

        public const String CATALOG_SHOULD_CONTAIN_LANG_ENTRY = "Catalog dictionary should contain lang entry.";

        public const String CELL_CANT_BE_DETERMINED_ALGORITHMICALLY = "TD cell row:{0} col:{1} in table {2} does" 
            + " not contain a valid Headers attribute, and Headers for this cell cannot be determined algorithmically.";

        public const String CELL_HAS_INVALID_ROLE = "Cell: row {0} ({1}) col {2} has invalid role.";

        public const String CONTENTS_AND_ALT_SHALL_BE_IDENTICAL = "If annotation has Contents entry and " + "enclosing structure element has Alt entry, the values shall be identical.";

        public const String CONTENT_IS_NOT_REAL_CONTENT_AND_NOT_ARTIFACT = "Content is neither marked as Artifact nor tagged as real content.";

        public const String CONTENT_NOT_REFERENCING_FE_NOTE = "Real content that refers to footnotes or endnotes "
             + "shall use the Ref entry on the referring structure element to reference the FENote.";

        public const String CONTENT_WITH_MCID_BUT_MCID_NOT_FOUND_IN_STRUCT_TREE_ROOT = "Content with MCID, but MCID wasn't found in StructTreeRoot.";

        public const String CT_OR_ALT_ENTRY_IS_MISSING_IN_MEDIA_CLIP = "CT or Alt entry is missing from the media "
             + "clip data dictionary.";

        public const String DEPRECATED_ANNOTATIONS_ARE_NOT_ALLOWED = "{0} annotations are deprecated in PDF 2.0 " 
            + "and therefore shall not be present.";

        public const String DESC_IS_REQUIRED_ON_ALL_FILE_SPEC_FROM_THE_EMBEDDED_FILES = "The Desc entry " + "shall be present on all file specification dictionaries present in the EmbeddedFiles name tree "
             + "of a conforming file.";

        public const String DESTINATION_NOT_STRUCTURE_DESTINATION = "All destinations whose target lies within the same document shall be structure destinations.";

        public const String DIFFERENT_LINKS_IN_SINGLE_STRUCT_ELEM = "Link annotations that target different " + "locations shall be in separate Link or Reference structure elements instead of a single one.";

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

        public const String FILE_SPEC_SHALL_CONTAIN_AFRELATIONSHIP = "When a file attachment annotation references"
             + " file specification dictionary, that file specification dictionary shall include an AFRelationship entry.";

        public const String FONT_SHOULD_BE_EMBEDDED = "Following font(s) are not embedded: {0}";

        public const String FORMULA_SHALL_HAVE_ALT = "Formula tags shall include an alternative representation or "
             + "replacement text.";

        public const String FORM_STRUCT_ELEM_SHALL_CONTAIN_AT_MOST_ONE_WIDGET = "Form structure element shall " + 
            "contain at most one widget annotation.";

        public const String FORM_STRUCT_ELEM_WITHOUT_ROLE_SHALL_CONTAIN_ONE_WIDGET = "If the Form structure " + "element omits a Role PrintField attribute, it shall have only one child: an object reference "
             + "identifying the widget annotation.";

        public const String GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE = "The '{0}' glyph either isn't defined in embedded font or doesn't have unicode mapping.";

        public const String H1_IS_SKIPPED = "Heading level 1 is skipped in a descending sequence of header levels.";

        public const String HN_IS_SKIPPED = "Heading level {0} is skipped in a descending sequence of header " + "levels.";

        public const String IMAGE_SHALL_HAVE_ALT = "Figure tags shall include an alternative representation or " +
             "replacement text. call com.itextpdf.kernel.pdf.tagutils.AccessibilityProperties#setActualText or com"
             + ".itextpdf.kernel.pdf.tagutils.AccessibilityProperties#setAlternateDescription to be PDF/UA compliant.";

        public const String INCORRECT_NOTE_TYPE_VALUE = "The value of the NoteType attribute shall be either \"Footnote\", \"Endnote\" or \"None\".";

        public const String INVALID_PDF_VERSION = "Specified document pdf version isn't supported in pdf/ua.";

        public const String INVISIBLE_ANNOT_SHALL_BE_AN_ARTIFACT = "Annotations with the Invisible flag set " + "shall be artifacts.";

        public const String LINK_ANNOTATION_SHOULD_HAVE_CONTENTS_KEY = "Annotation of type Link " + "shall contain an alternate description via their Contents key.";

        public const String LINK_ANNOT_IS_NOT_NESTED_WITHIN_LINK = "A link annotation is not nested within a <Link> tag.";

        public const String LINK_ANNOT_IS_NOT_NESTED_WITHIN_LINK_OR_REFERENCE = "A link annotation is either not tagged or not nested within a <Link> or <Reference> tag.";

        public const String LIST_ITEM_CONTENT_HAS_INVALID_TAG = "Any real content within an LI structure element "
             + "that is not enclosed in a Lbl structure element shall be enclosed in an LBody structure element.";

        public const String LIST_NUMBERING_IS_NOT_SPECIFIED = "If Lbl structure elements are present, the " + "ListNumbering attribute shall be specified for the respective L structure element; "
             + "the value None shall not be used.";

        public const String MARKUP_ANNOT_IS_NOT_TAGGED_AS_ANNOT = "Markup annotation shall be enclosed by " + "Annot tag in the tag tree structure.";

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

        public const String NON_SYMBOLIC_TTF_SHALL_CONTAIN_MAC_ROMAN_OR_MICROSOFT_UNI_CMAP = "For all non-symbolic TrueType fonts used for rendering, the embedded TrueType font program shall "
             + "contain at least the Microsoft Unicode (3, 1 – Platform ID = 3, Encoding ID = 1), or the " + "Macintosh Roman (1, 0 – Platform ID = 1, Encoding ID = 0) “cmap” subtable.";

        public const String NON_SYMBOLIC_TTF_SHALL_CONTAIN_NON_SYMBOLIC_CMAP = "For all non-symbolic TrueType " + 
            "fonts used for rendering, the embedded TrueType font program shall contain one or several non-symbolic "
             + "cmap entries such that all necessary glyph lookups can be carried out.";

        public const String NON_SYMBOLIC_TTF_SHALL_NOT_DEFINE_DIFFERENCES = "All non-symbolic TrueType fonts " + "shall not define a Differences array unless all the glyph names in the Differences array are listed "
             + "in the Adobe Glyph List and the embedded font program contains at least the Microsoft Unicode " + 
            "(3, 1 – Platform ID = 3, Encoding ID = 1) encoding in the “cmap” subtable.";

        public const String NON_SYMBOLIC_TTF_SHALL_SPECIFY_MAC_ROMAN_OR_WIN_ANSI_ENCODING = "All non-symbolic " + 
            "TrueType fonts shall have either MacRomanEncoding or WinAnsiEncoding as the value for the Encoding key "
             + "in the Font dictionary, or as the value for the BaseEncoding key in the dictionary that is the value of "
             + "the Encoding key in the Font dictionary";

        public const String NON_UNIQUE_ID_ENTRY_IN_STRUCT_TREE_ROOT = "ID entry '{0}' shall be unique among all elements in the document’s structure hierarchy";

        public const String NOTE_TAG_SHALL_HAVE_ID_ENTRY = "Note tags shall include a unique ID entry.";

        public const String NO_VIEW_ANNOT_SHALL_BE_AN_ARTIFACT = "Annotations with the NoView flag set and " + "the ToggleNoView flag not set shall be artifacts.";

        public const String OCG_PROPERTIES_CONFIG_SHALL_BE_AN_ARRAY = "Optional Content properties " + "configs shall be an array.";

        public const String OCG_SHALL_NOT_CONTAIN_AS_ENTRY = "An AS entry appears in an Optional Content.";

        public const String ONE_OR_MORE_STANDARD_ROLE_REMAPPED = "One or more standard types are remapped.";

        public const String PAGE_WITH_ANNOT_DOES_NOT_HAVE_TABS_WITH_S = "A page with annotation(s) doesn't " + "contain Tabs key with S value.";

        public const String PAGE_WITH_ANNOT_DOES_NOT_HAVE_TABS_WITH_VALID_CONTENT = "Page that includes an " + "annotation shall contain a Tabs entry in its page dictionary, and its value shall be A, W or S.";

        public const String POPUP_ANNOTATIONS_ARE_NOT_ALLOWED = "Popup annotations shall not be present in the " +
             "tag structure tree.";

        public const String PRINTER_MARK_IS_NOT_PERMITTED = "Annotations of subtype PrinterMark shall not be" + " included in logical structure.";

        public const String PRINTER_MARK_SHALL_BE_AN_ARTIFACT = "PrinterMark annotation shall be an artifact.";

        public const String PUA_CONTENT_WITHOUT_ALT = "In all cases, where real content maps to Unicode PUA values, an ActualText or Alt entry shall be present.";

        public const String P_VALUE_IS_ABSENT_IN_ENCRYPTION_DICTIONARY = "Permissions are absent " + "in pdf encryption dictionary.";

        public const String RC_DIFFERENT_FROM_CONTENTS = "If markup annotation contains both RC and Contents " + "entries, they shall be textually equivalent.";

        public const String REAL_CONTENT_CANT_BE_INSIDE_ARTIFACT = "Content marked as content may not reside in Artifact content.";

        public const String REAL_CONTENT_INSIDE_ARTIFACT_OR_VICE_VERSA = "Tagged content is present inside content marked as Artifact or vice versa.";

        public const String ROWS_SPAN_DIFFERENT_NUMBER_OF_COLUMNS = "Table rows {0} and {1} span different number of columns.";

        public const String SAME_LINKS_IN_DIFFERENT_STRUCT_ELEMS = "Multiple link annotations targeting the same "
             + "location shall be included in a single Link or Reference structure element instead of separate ones.";

        public const String STAMP_ANNOT_SHALL_SPECIFY_NAME_OR_CONTENTS = "Stamp annotation shall contain either " 
            + "Name or Contents entry in order to describe the author’s intent.";

        public const String STRUCTURE_TYPE_IS_ROLE_MAPPED_TO_OTHER_STRUCTURE_TYPE_IN_THE_SAME_NAMESPACE = "Structure type {0}:{1} is role mapped to other structure type in the same namespace.";

        public const String SUSPECTS_ENTRY_IN_MARK_INFO_DICTIONARY_SHALL_NOT_HAVE_A_VALUE_OF_TRUE = "Suspects entry in mark info dictionary shall not have a value of true.";

        public const String SYMBOLIC_TTF_SHALL_CONTAIN_EXACTLY_ONE_OR_AT_LEAST_MICROSOFT_SYMBOL_CMAP = "For " + "symbolic TrueType fonts the “cmap” table in the embedded font program shall either contain exactly one "
             + "encoding or it shall contain, at least, the Microsoft Symbol (Platform ID = 3, Encoding ID = 0) encoding.";

        public const String SYMBOLIC_TTF_SHALL_CONTAIN_MAC_ROMAN_OR_MICROSOFT_SYMBOL_CMAP = "For symbolic TrueType"
             + " fonts the 'cmap' subtable in the embedded font program shall either contain the Microsoft Symbol (3,0 –"
             + " Platform ID=3, Encoding ID=0) or the Mac Roman (1,0 – Platform ID=1, Encoding ID=0) encoding.";

        public const String SYMBOLIC_TTF_SHALL_NOT_CONTAIN_ENCODING = "Symbolic TrueType fonts shall not contain "
             + "an Encoding entry in the font dictionary.";

        public const String TABLE_CONTAINS_EMPTY_CELLS = "Cell: row {0} ({1}) col {2} is empty, each row should " 
            + "have the same amount of columns when taking into account spanning.";

        public const String TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING = "Tag hasn't been added before adding content to the canvas.";

        public const String TAG_MAPPING_DOESNT_TERMINATE_WITH_STANDARD_TYPE = "\"{0}\" tag mapping does not terminate with a standard type.";

        public const String TENTH_BIT_OF_P_VALUE_IN_ENCRYPTION_SHOULD_BE_NON_ZERO = "10th bit of P value of " + "Encryption dictionary should be 1 if the document is tagged.";

        public const String TEXT_FIELD_V_AND_RV_SHALL_BE_TEXTUALLY_EQUIVALENT = "For text fields, when RV entry " 
            + "is present, a V entry shall also be present, and they shall be textually equivalent.";

        public const String TEXT_STRING_USES_UNICODE_PUA = "Text strings intended to be human readable shall not use the Unicode PUA.";

        public const String TOCI_SHALL_IDENTIFY_REF = "Each TOCI structure element shall contain the Ref entry, either directly on the TOCI structure element"
             + " itself or on at least one of its descendant structure elements.";

        public const String VIEWER_PREFERENCES_IS_FALSE = "ViewerPreferences dictionary of the Catalog dictionary "
             + "contains a DisplayDocTitle entry with a value of false.";

        public const String WIDGET_SHALL_BE_FORM_OR_ARTIFACT = "Widget annotation shall be either Form structure "
             + "element or an Artifact.";

        public const String WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS = "If label for a widget annotation is not " + 
            "present, Contents entry shall be provided.\n" + "For PdfFormAnnotation use PdfFormAnnotation#setAlternativeDescription(\"Your contents description\");\n"
             + "for PdfAnnotation use PdfAnnotation#setContents(\"Your contents description\");\n" + "for the layout engine use Element#getAccesibilityProperties().setAlternateDescription or "
             + "FormField#setAlternativeDescription(\"Your contents description\").";

        public const String WIDGET_WITH_AA_SHALL_PROVIDE_CONTENTS = "If an additional action (AA) entry is present"
             + " in a widget annotation, Contents entry shall describe the action's intent.\n" + "For PdfFormAnnotation use PdfFormAnnotation#setAlternativeDescription(\"Your contents description\");\n"
             + "for PdfAnnotation use PdfAnnotation#setContents(\"Your contents description\");\n" + "for the layout engine use Element#getAccesibilityProperties().setAlternateDescription or "
             + "FormField#setAlternativeDescription(\"Your contents description\").";

        public const String WIDGET_WITH_ZERO_HEIGHT_SHALL_BE_AN_ARTIFACT = "Widget annotation of zero height " + "and width shall be an artifact.";

        public const String XFA_FORMS_SHALL_NOT_BE_PRESENT = "XFA forms shall not be present in PDF/UA-2 document.";

        private PdfUAExceptionMessageConstants() {
        }
        // Empty constructor.
    }
}
