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
﻿// <auto-generated/>
#if NETSTANDARD2_0

#nullable enable annotations
#nullable disable warnings

// Suppress warnings about [Obsolete] member usage in generated code.
#pragma warning disable CS0612, CS0618

namespace iText.Commons.Utils
{
    public partial class ClassWithDefaultValueContext
    {
        private global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<int>? _Int32;
        
        /// <summary>
        /// Defines the source generated JSON serialization contract metadata for a given type.
        /// </summary>
        #nullable disable annotations // Marking the property type as nullable-oblivious.
        public global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<int> Int32
        #nullable enable annotations
        {
            get => _Int32 ??= (global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<int>)Options.GetTypeInfo(typeof(int));
        }
        
        private global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<int> Create_Int32(global::System.Text.Json.JsonSerializerOptions options)
        {
            if (!TryGetTypeInfoForRuntimeCustomConverter<int>(options, out global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<int> jsonTypeInfo))
            {
                jsonTypeInfo = global::System.Text.Json.Serialization.Metadata.JsonMetadataServices.CreateValueInfo<int>(options, global::System.Text.Json.Serialization.Metadata.JsonMetadataServices.Int32Converter);
            }
        
            jsonTypeInfo.OriginatingResolver = this;
            return jsonTypeInfo;
        }
    }
}
#endif
