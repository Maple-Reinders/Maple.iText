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

namespace iText.StyledXmlParser.Logs {
    /// <summary>Class that bundles all the error message templates as constants.</summary>
    public sealed class StyledXmlParserLogMessageConstant {
        /// <summary>The Constant SHORTHAND_PROPERTY_CANNOT_BE_EMPTY.</summary>
        public const String SHORTHAND_PROPERTY_CANNOT_BE_EMPTY = "{0} shorthand property cannot be empty.";

        /// <summary>The Constant DEFAULT_VALUE_OF_CSS_PROPERTY_UNKNOWN.</summary>
        public const String DEFAULT_VALUE_OF_CSS_PROPERTY_UNKNOWN = "Default value of the css property \"{0}\" is unknown.";

        /// <summary>The Constant ERROR_ADDING_CHILD_NODE.</summary>
        public const String ERROR_ADDING_CHILD_NODE = "Error adding child node.";

        /// <summary>The Constant ERROR_PARSING_COULD_NOT_MAP_NODE.</summary>
        public const String ERROR_PARSING_COULD_NOT_MAP_NODE = "Could not map node type: {0}";

        /// <summary>The Constant ERROR_PARSING_CSS_SELECTOR.</summary>
        public const String ERROR_PARSING_CSS_SELECTOR = "Error while parsing css selector: {0}";

        /// <summary>The Constant ONLY_THE_LAST_BACKGROUND_CAN_INCLUDE_BACKGROUND_COLOR.</summary>
        public const String ONLY_THE_LAST_BACKGROUND_CAN_INCLUDE_BACKGROUND_COLOR = "Only the last background can include a background color.";

        /// <summary>The Constant UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED.</summary>
        public const String UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED = "Unknown absolute metric length parsed \"{0}\".";

        public const String UNKNOWN_METRIC_ANGLE_PARSED = "Unknown metric angle parsed: \"{0}\".";

        /// <summary>The Constant UNKNOWN__PROPERTY.</summary>
        public const String UNKNOWN_PROPERTY = "Unknown {0} property: \"{1}\".";

        public const String URL_IS_EMPTY_IN_CSS_EXPRESSION = "url function is empty in expression:{0}";

        public const String URL_IS_NOT_CLOSED_IN_CSS_EXPRESSION = "url function is not properly closed in expression:{0}";

        /// <summary>The Constant QUOTES_PROPERTY_INVALID.</summary>
        public const String QUOTES_PROPERTY_INVALID = "Quote property \"{0}\" is invalid. It should contain even number of <string> values.";

        /// <summary>The Constant QUOTE_IS_NOT_CLOSED_IN_CSS_EXPRESSION.</summary>
        public const String QUOTE_IS_NOT_CLOSED_IN_CSS_EXPRESSION = "The quote is not closed in css expression: {0}";

        /// <summary>The Constant INVALID_CSS_PROPERTY_DECLARATION.</summary>
        public const String INVALID_CSS_PROPERTY_DECLARATION = "Invalid css property declaration: {0}";

        /// <summary>The Constant INVALID_CSS_VARIABLE_NESTING.</summary>
        public const String INVALID_CSS_VARIABLE_COUNT = "Css var expression count too high, possible cyclic backreference at declaration: {0}";

        /// <summary>The Constant ERROR_DURING_CSS_VARIABLE_RESOLVING.</summary>
        public const String ERROR_DURING_CSS_VARIABLE_RESOLVING = "Css var expression can't be resolved at declaration: {0}";

        /// <summary>The Constant INCORRECT_CHARACTER_SEQUENCE.</summary>
        public const String INCORRECT_CHARACTER_SEQUENCE = "Incorrect character sequence.";

        public const String INCORRECT_RESOLUTION_UNIT_VALUE = "Resolution value unit should be either dpi, dppx or dpcm!";

        /// <summary>The Constant RULE_IS_NOT_SUPPORTED.</summary>
        public const String RULE_IS_NOT_SUPPORTED = "The rule @{0} is unsupported. All selectors in this rule will be ignored.";

        /// <summary>The Constant RESOURCE_WITH_GIVEN_URL_WAS_FILTERED_OUT.</summary>
        [Obsolete]
        public const String RESOURCE_WITH_GIVEN_URL_WAS_FILTERED_OUT = "Resource with given URL ({0}) was filtered out.";

        /// <summary>The Constant UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_DATA_URI.</summary>
        public const String UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_DATA_URI = "Unable to retrieve image with data URI {0}";

        /// <summary>The Constant UNABLE_TO_RETRIEVE_RESOURCE_WITH_GIVEN_RESOURCE_SIZE_BYTE_LIMIT.</summary>
        public const String UNABLE_TO_RETRIEVE_RESOURCE_WITH_GIVEN_RESOURCE_SIZE_BYTE_LIMIT = "Unable to retrieve resource with given URL ({0}) and resource size byte limit ({1}).";

        /// <summary>The Constant UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_BASE_URI.</summary>
        public const String UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_BASE_URI = "Unable to retrieve image with given base URI ({0}) and image source path ({1})";

        public const String UNABLE_TO_RESOLVE_IMAGE_URL = "Unable to resolve image path with given base URI ({0}) and image source path ({1})";

        /// <summary>The Constant UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI.</summary>
        public const String UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI = "Unable to retrieve stream with given base URI ({0}) and source path ({1})";

        public const String UNABLE_TO_PROCESS_EXTERNAL_CSS_FILE = "Unable to process external css file";

        public const String UNABLE_TO_RETRIEVE_FONT = "Unable to retrieve font:\n {0}";

        public const String UNSUPPORTED_PSEUDO_CSS_SELECTOR = "Unsupported pseudo css selector: {0}";

        /// <summary>The Constant WAS_NOT_ABLE_TO_DEFINE_BACKGROUND_CSS_SHORTHAND_PROPERTIES.</summary>
        public const String WAS_NOT_ABLE_TO_DEFINE_BACKGROUND_CSS_SHORTHAND_PROPERTIES = "Was not able to define one of the background CSS shorthand properties: {0}";

        /// <summary>The Constant ERROR_RESOLVING_PARENT_STYLES.</summary>
        public const String ERROR_RESOLVING_PARENT_STYLES = "Element parent styles are not resolved. Styles for current element might be incorrect.";

        /// <summary>The Constant ERROR_LOADING_FONT.</summary>
        public const String ERROR_LOADING_FONT = "Error while loading font";

        public const String IMPORT_MUST_COME_BEFORE = "Imported rules must come before all other types of rules, except @charset rules and layer creating @layer statements. "
             + "Rule will be ignored. ";

        public const String IMPORT_RULE_URL_CAN_NOT_BE_RESOLVED = "Import rule URL can't be resolved because of base URI absence.";

        /// <summary>Instantiates a new log message constant.</summary>
        private StyledXmlParserLogMessageConstant() {
        }
        //Private constructor will prevent the instantiation of this class directly
    }
}
