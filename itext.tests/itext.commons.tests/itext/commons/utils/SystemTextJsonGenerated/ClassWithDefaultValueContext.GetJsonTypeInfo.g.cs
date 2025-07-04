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
    public partial class ClassWithDefaultValueContext : global::System.Text.Json.Serialization.Metadata.IJsonTypeInfoResolver
    {
        /// <inheritdoc/>
        public override global::System.Text.Json.Serialization.Metadata.JsonTypeInfo? GetTypeInfo(global::System.Type type)
        {
            Options.TryGetTypeInfo(type, out global::System.Text.Json.Serialization.Metadata.JsonTypeInfo? typeInfo);
            return typeInfo;
        }

        global::System.Text.Json.Serialization.Metadata.JsonTypeInfo? global::System.Text.Json.Serialization.Metadata.IJsonTypeInfoResolver.GetTypeInfo(global::System.Type type, global::System.Text.Json.JsonSerializerOptions options)
        {
            if (type == typeof(double))
            {
                return Create_Double(options);
            }
            if (type == typeof(double?))
            {
                return Create_NullableDouble(options);
            }
            if (type == typeof(global::iText.Commons.Utils.JsonUtilTest.ClassWithDefaultValue))
            {
                return Create_ClassWithDefaultValue(options);
            }
            if (type == typeof(int))
            {
                return Create_Int32(options);
            }
            if (type == typeof(int?))
            {
                return Create_NullableInt32(options);
            }
            if (type == typeof(string))
            {
                return Create_String(options);
            }
            return null;
        }
    }
}
#endif
