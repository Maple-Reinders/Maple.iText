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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Font.Constants;

namespace iText.IO.Font {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// If you are using True Type fonts, you can declare the paths of the different ttf- and ttc-files
    /// to this class first and then create fonts in your code using one of the getFont method
    /// without having to enter a path as parameter.
    /// </summary>
    internal class FontRegisterProvider {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.IO.Font.FontRegisterProvider
            ));

        /// <summary>This is a map of postscriptfontnames of fonts and the path of their font file.</summary>
        private readonly IDictionary<String, String> fontNames = new Dictionary<String, String>();

        /// <summary>This is a map of fontfamilies.</summary>
        private readonly IDictionary<String, IList<String>> fontFamilies = new Dictionary<String, IList<String>>();

//\cond DO_NOT_DOCUMENT
        /// <summary>Creates new FontRegisterProvider</summary>
        internal FontRegisterProvider() {
            RegisterStandardFonts();
            RegisterStandardFontFamilies();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Constructs a <c>Font</c>-object.</summary>
        /// <param name="fontName">the name of the font</param>
        /// <param name="style">the style of this font</param>
        /// <returns>the Font constructed based on the parameters</returns>
        internal virtual FontProgram GetFont(String fontName, int style) {
            return GetFont(fontName, style, true);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Constructs a <c>Font</c>-object.</summary>
        /// <param name="fontName">the name of the font</param>
        /// <param name="style">the style of this font</param>
        /// <param name="cached">
        /// true if the font comes from the cache or is added to
        /// the cache if new, false if the font is always created new
        /// </param>
        /// <returns>the Font constructed based on the parameters</returns>
        internal virtual FontProgram GetFont(String fontName, int style, bool cached) {
            if (fontName == null) {
                return null;
            }
            String lowerCaseFontName = StringNormalizer.ToLowerCase(fontName);
            IList<String> family = !lowerCaseFontName.EqualsIgnoreCase(StandardFonts.TIMES_ROMAN) ? fontFamilies.Get(lowerCaseFontName
                ) : fontFamilies.Get(StringNormalizer.ToLowerCase(StandardFontFamilies.TIMES));
            if (family != null) {
                lock (family) {
                    // some bugs were fixed here by Daniel Marczisovszky
                    int s = style == FontStyles.UNDEFINED ? FontStyles.NORMAL : style;
                    foreach (String f in family) {
                        String lcf = StringNormalizer.ToLowerCase(f);
                        int fs = FontStyles.NORMAL;
                        if (lcf.Contains("bold")) {
                            fs |= FontStyles.BOLD;
                        }
                        if (lcf.Contains("italic") || lcf.Contains("oblique")) {
                            fs |= FontStyles.ITALIC;
                        }
                        if ((s & FontStyles.BOLDITALIC) == fs) {
                            fontName = f;
                            break;
                        }
                    }
                }
            }
            return GetFontProgram(fontName, cached);
        }
//\endcond

        protected internal virtual void RegisterStandardFonts() {
            fontNames.Put(StringNormalizer.ToLowerCase(StandardFonts.COURIER), StandardFonts.COURIER);
            fontNames.Put(StringNormalizer.ToLowerCase(StandardFonts.COURIER_BOLD), StandardFonts.COURIER_BOLD);
            fontNames.Put(StringNormalizer.ToLowerCase(StandardFonts.COURIER_OBLIQUE), StandardFonts.COURIER_OBLIQUE);
            fontNames.Put(StringNormalizer.ToLowerCase(StandardFonts.COURIER_BOLDOBLIQUE), StandardFonts.COURIER_BOLDOBLIQUE
                );
            fontNames.Put(StringNormalizer.ToLowerCase(StandardFonts.HELVETICA), StandardFonts.HELVETICA);
            fontNames.Put(StringNormalizer.ToLowerCase(StandardFonts.HELVETICA_BOLD), StandardFonts.HELVETICA_BOLD);
            fontNames.Put(StringNormalizer.ToLowerCase(StandardFonts.HELVETICA_OBLIQUE), StandardFonts.HELVETICA_OBLIQUE
                );
            fontNames.Put(StringNormalizer.ToLowerCase(StandardFonts.HELVETICA_BOLDOBLIQUE), StandardFonts.HELVETICA_BOLDOBLIQUE
                );
            fontNames.Put(StringNormalizer.ToLowerCase(StandardFonts.SYMBOL), StandardFonts.SYMBOL);
            fontNames.Put(StringNormalizer.ToLowerCase(StandardFonts.TIMES_ROMAN), StandardFonts.TIMES_ROMAN);
            fontNames.Put(StringNormalizer.ToLowerCase(StandardFonts.TIMES_BOLD), StandardFonts.TIMES_BOLD);
            fontNames.Put(StringNormalizer.ToLowerCase(StandardFonts.TIMES_ITALIC), StandardFonts.TIMES_ITALIC);
            fontNames.Put(StringNormalizer.ToLowerCase(StandardFonts.TIMES_BOLDITALIC), StandardFonts.TIMES_BOLDITALIC
                );
            fontNames.Put(StringNormalizer.ToLowerCase(StandardFonts.ZAPFDINGBATS), StandardFonts.ZAPFDINGBATS);
        }

        protected internal virtual void RegisterStandardFontFamilies() {
            IList<String> family;
            family = new List<String>();
            family.Add(StandardFonts.COURIER);
            family.Add(StandardFonts.COURIER_BOLD);
            family.Add(StandardFonts.COURIER_OBLIQUE);
            family.Add(StandardFonts.COURIER_BOLDOBLIQUE);
            fontFamilies.Put(StringNormalizer.ToLowerCase(StandardFontFamilies.COURIER), family);
            family = new List<String>();
            family.Add(StandardFonts.HELVETICA);
            family.Add(StandardFonts.HELVETICA_BOLD);
            family.Add(StandardFonts.HELVETICA_OBLIQUE);
            family.Add(StandardFonts.HELVETICA_BOLDOBLIQUE);
            fontFamilies.Put(StringNormalizer.ToLowerCase(StandardFontFamilies.HELVETICA), family);
            family = new List<String>();
            family.Add(StandardFonts.SYMBOL);
            fontFamilies.Put(StringNormalizer.ToLowerCase(StandardFontFamilies.SYMBOL), family);
            family = new List<String>();
            family.Add(StandardFonts.TIMES_ROMAN);
            family.Add(StandardFonts.TIMES_BOLD);
            family.Add(StandardFonts.TIMES_ITALIC);
            family.Add(StandardFonts.TIMES_BOLDITALIC);
            fontFamilies.Put(StringNormalizer.ToLowerCase(StandardFontFamilies.TIMES), family);
            family = new List<String>();
            family.Add(StandardFonts.ZAPFDINGBATS);
            fontFamilies.Put(StringNormalizer.ToLowerCase(StandardFontFamilies.ZAPFDINGBATS), family);
        }

        protected internal virtual FontProgram GetFontProgram(String fontName, bool cached) {
            FontProgram fontProgram = null;
            fontName = fontNames.Get(StringNormalizer.ToLowerCase(fontName));
            if (fontName != null) {
                fontProgram = FontProgramFactory.CreateFont(fontName, cached);
            }
            return fontProgram;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Register a font by giving explicitly the font family and name.</summary>
        /// <param name="familyName">the font family</param>
        /// <param name="fullName">the font name</param>
        /// <param name="path">the font path</param>
        internal virtual void RegisterFontFamily(String familyName, String fullName, String path) {
            if (path != null) {
                fontNames.Put(fullName, path);
            }
            IList<String> family;
            lock (fontFamilies) {
                family = fontFamilies.Get(familyName);
                if (family == null) {
                    family = new List<String>();
                    fontFamilies.Put(familyName, family);
                }
            }
            lock (family) {
                if (!family.Contains(fullName)) {
                    int fullNameLength = fullName.Length;
                    bool inserted = false;
                    for (int j = 0; j < family.Count; ++j) {
                        if (family[j].Length >= fullNameLength) {
                            family.Add(j, fullName);
                            inserted = true;
                            break;
                        }
                    }
                    if (!inserted) {
                        family.Add(fullName);
                        String newFullName = StringNormalizer.ToLowerCase(fullName);
                        if (newFullName.EndsWith("regular")) {
                            //remove "regular" at the end of the font name
                            newFullName = newFullName.JSubstring(0, newFullName.Length - 7).Trim();
                            //insert this font name at the first position for higher priority
                            family.Add(0, fullName.JSubstring(0, newFullName.Length));
                        }
                    }
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Register a font file, either .ttf or .otf, .afm or a font from TrueType Collection.</summary>
        /// <remarks>
        /// Register a font file, either .ttf or .otf, .afm or a font from TrueType Collection.
        /// If a TrueType Collection is registered, an additional index of the font program can be specified
        /// </remarks>
        /// <param name="path">the path to a ttf- or ttc-file</param>
        internal virtual void RegisterFont(String path) {
            RegisterFont(path, null);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Register a font file and use an alias for the font contained in it.</summary>
        /// <param name="path">the path to a font file</param>
        /// <param name="alias">the alias you want to use for the font</param>
        internal virtual void RegisterFont(String path, String alias) {
            try {
                String pathLc = StringNormalizer.ToLowerCase(path);
                if (pathLc.EndsWith(".ttf") || pathLc.EndsWith(".otf") || pathLc.IndexOf(".ttc,", StringComparison.Ordinal
                    ) > 0) {
                    FontProgramDescriptor descriptor = FontProgramDescriptorFactory.FetchDescriptor(path);
                    fontNames.Put(descriptor.GetFontNameLowerCase(), path);
                    if (alias != null) {
                        String lcAlias = StringNormalizer.ToLowerCase(alias);
                        fontNames.Put(lcAlias, path);
                        if (lcAlias.EndsWith("regular")) {
                            //do this job to give higher priority to regular fonts in comparison with light, narrow, etc
                            SaveCopyOfRegularFont(lcAlias, path);
                        }
                    }
                    // register all the font names with all the locales
                    foreach (String name in descriptor.GetFullNameAllLangs()) {
                        fontNames.Put(name, path);
                        if (name.EndsWith("regular")) {
                            //do this job to give higher priority to regular fonts in comparison with light, narrow, etc
                            SaveCopyOfRegularFont(name, path);
                        }
                    }
                    if (descriptor.GetFamilyNameEnglishOpenType() != null) {
                        foreach (String fullName in descriptor.GetFullNamesEnglishOpenType()) {
                            RegisterFontFamily(descriptor.GetFamilyNameEnglishOpenType(), fullName, null);
                        }
                    }
                }
                else {
                    if (pathLc.EndsWith(".ttc")) {
                        TrueTypeCollection ttc = new TrueTypeCollection(path);
                        for (int i = 0; i < ttc.GetTTCSize(); i++) {
                            String fullPath = path + "," + i;
                            if (alias != null) {
                                RegisterFont(fullPath, alias + "," + i);
                            }
                            else {
                                RegisterFont(fullPath);
                            }
                        }
                    }
                    else {
                        if (pathLc.EndsWith(".afm") || pathLc.EndsWith(".pfm")) {
                            FontProgramDescriptor descriptor = FontProgramDescriptorFactory.FetchDescriptor(path);
                            RegisterFontFamily(descriptor.GetFamilyNameLowerCase(), descriptor.GetFullNameLowerCase(), null);
                            fontNames.Put(descriptor.GetFontNameLowerCase(), path);
                            fontNames.Put(descriptor.GetFullNameLowerCase(), path);
                        }
                    }
                }
                LOGGER.LogTrace(MessageFormatUtil.Format("Registered {0}", path));
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(e);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        // remove regular and correct last symbol
        // do this job to give higher priority to regular fonts in comparison with light, narrow, etc
        // Don't use this method for not regular fonts!
        internal virtual bool SaveCopyOfRegularFont(String regularFontName, String path) {
            //remove "regular" at the end of the font name
            String alias = regularFontName.JSubstring(0, regularFontName.Length - 7).Trim();
            if (!fontNames.ContainsKey(alias)) {
                fontNames.Put(alias, path);
                return true;
            }
            return false;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Register all the fonts in a directory.</summary>
        /// <param name="dir">the directory</param>
        /// <returns>the number of fonts registered</returns>
        internal virtual int RegisterFontDirectory(String dir) {
            return RegisterFontDirectory(dir, false);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Register all the fonts in a directory and possibly its subdirectories.</summary>
        /// <param name="dir">the directory</param>
        /// <param name="scanSubdirectories">recursively scan subdirectories if <c>true</c></param>
        /// <returns>the number of fonts registered</returns>
        internal virtual int RegisterFontDirectory(String dir, bool scanSubdirectories) {
            LOGGER.LogDebug(MessageFormatUtil.Format("Registering directory {0}, looking for fonts", dir));
            int count = 0;
            try {
                String[] files = FileUtil.ListFilesInDirectory(dir, scanSubdirectories);
                if (files == null) {
                    return 0;
                }
                foreach (String file in files) {
                    try {
                        String suffix = file.Length < 4 ? null : StringNormalizer.ToLowerCase(file.Substring(file.Length - 4));
                        if (".afm".Equals(suffix) || ".pfm".Equals(suffix)) {
                            /* Only register Type 1 fonts with matching .pfb files */
                            String pfb = file.JSubstring(0, file.Length - 4) + ".pfb";
                            if (FileUtil.FileExists(pfb)) {
                                RegisterFont(file, null);
                                ++count;
                            }
                        }
                        else {
                            if (".ttf".Equals(suffix) || ".otf".Equals(suffix) || ".ttc".Equals(suffix)) {
                                RegisterFont(file, null);
                                ++count;
                            }
                        }
                    }
                    catch (Exception) {
                    }
                }
            }
            catch (Exception) {
            }
            //empty on purpose
            //empty on purpose
            return count;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Register fonts in some probable directories.</summary>
        /// <remarks>
        /// Register fonts in some probable directories. It usually works in Windows,
        /// Linux and Solaris.
        /// </remarks>
        /// <returns>the number of fonts registered</returns>
        internal virtual int RegisterSystemFontDirectories() {
            int count = 0;
            String[] withSubDirs = new String[] { FileUtil.GetFontsDir(), "/usr/share/X11/fonts", "/usr/X/lib/X11/fonts"
                , "/usr/openwin/lib/X11/fonts", "/usr/share/fonts", "/usr/X11R6/lib/X11/fonts" };
            foreach (String directory in withSubDirs) {
                count += RegisterFontDirectory(directory, true);
            }
            String[] withoutSubDirs = new String[] { "/Library/Fonts", "/System/Library/Fonts" };
            foreach (String directory in withoutSubDirs) {
                count += RegisterFontDirectory(directory, false);
            }
            return count;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Gets a set of registered font names.</summary>
        /// <returns>a set of registered fonts</returns>
        internal virtual ICollection<String> GetRegisteredFonts() {
            return fontNames.Keys;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Gets a set of registered font names.</summary>
        /// <returns>a set of registered font families</returns>
        internal virtual ICollection<String> GetRegisteredFontFamilies() {
            return fontFamilies.Keys;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Checks if a certain font is registered.</summary>
        /// <param name="fontname">the name of the font that has to be checked.</param>
        /// <returns>true if the font is found</returns>
        internal virtual bool IsRegisteredFont(String fontname) {
            return fontNames.ContainsKey(StringNormalizer.ToLowerCase(fontname));
        }
//\endcond

        public virtual void ClearRegisteredFonts() {
            fontNames.Clear();
            RegisterStandardFonts();
        }

        public virtual void ClearRegisteredFontFamilies() {
            fontFamilies.Clear();
            RegisterStandardFontFamilies();
        }
    }
//\endcond
}
