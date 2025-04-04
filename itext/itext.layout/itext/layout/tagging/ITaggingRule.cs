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
namespace iText.Layout.Tagging {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// Implementation of the interface is used to create required children
    /// structure for the specified role.
    /// </summary>
    /// <remarks>
    /// Implementation of the interface is used to create required children
    /// structure for the specified role. E.g. table must have TRs as children.
    /// </remarks>
    internal interface ITaggingRule {
        /// <summary>Action which creates required children structure for the role.</summary>
        /// <param name="taggingHelper">tagging helper.</param>
        /// <param name="taggingHintKey">element for which children structure will be created.</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the structure was created successfully,
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        bool OnTagFinish(LayoutTaggingHelper taggingHelper, TaggingHintKey taggingHintKey);
    }
//\endcond
}
