using NUnit.Framework;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("iText.Layout.Tests")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Apryse Group NV")]
[assembly: AssemblyProduct("iText")]
[assembly: AssemblyCopyright("Copyright (c) 1998-2025 Apryse Group NV")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: Guid("9ad347a8-ea5b-462b-810c-998f04471bb7")]

[assembly: AssemblyVersion("9.3.0.0")]
[assembly: AssemblyFileVersion("9.3.0.0")]
[assembly: AssemblyInformationalVersion("9.3.0-SNAPSHOT")]

[assembly: Parallelizable(ParallelScope.ContextMask)]

#if !NETSTANDARD2_0
[assembly: NUnit.Framework.Timeout(600000)]
#endif
