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
using iText.Commons.Utils;
using iText.IO.Util;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg;
using iText.Svg.Css.Impl;
using iText.Svg.Processors.Impl;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Css {
    [NUnit.Framework.Category("UnitTest")]
    public class SvgStyleResolverTest : ExtendedITextTest {
        private static readonly String baseUri = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/css/SvgStyleResolver/";

        //Single element test
        //Inherits values from parent?
        //Calculates values from parent
        [NUnit.Framework.Test]
        public virtual void SvgCssResolverBasicAttributeTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupCircle = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("circle"), "");
            Attributes circleAttributes = jsoupCircle.Attributes();
            circleAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute("id", "circle1"));
            circleAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute("cx", "95"));
            circleAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute("cy", "95"));
            circleAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute("rx", "53"));
            circleAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute("ry", "53"));
            circleAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute("style", "stroke-width:1.5;stroke:#da0000;"
                ));
            AbstractCssContext cssContext = new SvgCssContext();
            INode circle = new JsoupElementNode(jsoupCircle);
            SvgProcessorContext context = new SvgProcessorContext(new SvgConverterProperties());
            ICssResolver resolver = new SvgStyleResolver(circle, context);
            IDictionary<String, String> actual = resolver.ResolveStyles(circle, cssContext);
            IDictionary<String, String> expected = new Dictionary<String, String>();
            expected.Put("id", "circle1");
            expected.Put("cx", "95");
            expected.Put("cy", "95");
            expected.Put("rx", "53");
            expected.Put("ry", "53");
            expected.Put("stroke-width", "1.5");
            expected.Put("stroke", "#da0000");
            expected.Put("font-size", "12pt");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void SvgCssResolverStylesheetTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupLink = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf(SvgConstants.Tags.LINK), "");
            Attributes linkAttributes = jsoupLink.Attributes();
            linkAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute(SvgConstants.Attributes.XMLNS, "http://www.w3.org/1999/xhtml"
                ));
            linkAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute(SvgConstants.Attributes.REL, SvgConstants.Attributes
                .STYLESHEET));
            linkAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute(SvgConstants.Attributes.HREF, "styleSheetWithLinkStyle.css"
                ));
            linkAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute("type", "text/css"));
            JsoupElementNode node = new JsoupElementNode(jsoupLink);
            SvgConverterProperties scp = new SvgConverterProperties();
            scp.SetBaseUri(baseUri);
            SvgProcessorContext processorContext = new SvgProcessorContext(scp);
            SvgStyleResolver sr = new SvgStyleResolver(node, processorContext);
            IDictionary<String, String> attr = sr.ResolveStyles(node, new SvgCssContext());
            IDictionary<String, String> expectedAttr = new Dictionary<String, String>();
            expectedAttr.Put(SvgConstants.Attributes.XMLNS, "http://www.w3.org/1999/xhtml");
            expectedAttr.Put(SvgConstants.Attributes.REL, SvgConstants.Attributes.STYLESHEET);
            expectedAttr.Put(SvgConstants.Attributes.HREF, "styleSheetWithLinkStyle.css");
            expectedAttr.Put(SvgConstants.Attributes.FONT_SIZE, "12pt");
            expectedAttr.Put("type", "text/css");
            // Attribute from external stylesheet
            expectedAttr.Put(SvgConstants.Attributes.FILL, "black");
            NUnit.Framework.Assert.AreEqual(expectedAttr, attr);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI
            , LogLevel = LogLevelConstants.ERROR)]
        public virtual void SvgCssResolverInvalidNameStylesheetTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupLink = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf(SvgConstants.Tags.LINK), "");
            iText.StyledXmlParser.Jsoup.Nodes.Attributes linkAttributes = jsoupLink.Attributes();
            linkAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute(SvgConstants.Attributes.XMLNS, "http://www.w3.org/1999/xhtml"
                ));
            linkAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute(SvgConstants.Attributes.REL, SvgConstants.Attributes
                .STYLESHEET));
            linkAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute(SvgConstants.Attributes.HREF, "!invalid name!externalSheet.css"
                ));
            linkAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute("type", "text/css"));
            JsoupElementNode node = new JsoupElementNode(jsoupLink);
            SvgConverterProperties scp = new SvgConverterProperties();
            scp.SetBaseUri(baseUri);
            SvgProcessorContext processorContext = new SvgProcessorContext(scp);
            SvgStyleResolver sr = new SvgStyleResolver(node, processorContext);
            IDictionary<String, String> attr = sr.ResolveStyles(node, new SvgCssContext());
            IDictionary<String, String> expectedAttr = new Dictionary<String, String>();
            expectedAttr.Put(SvgConstants.Attributes.XMLNS, "http://www.w3.org/1999/xhtml");
            expectedAttr.Put(SvgConstants.Attributes.REL, SvgConstants.Attributes.STYLESHEET);
            expectedAttr.Put(SvgConstants.Attributes.HREF, "!invalid name!externalSheet.css");
            expectedAttr.Put(SvgConstants.Attributes.FONT_SIZE, "12pt");
            expectedAttr.Put("type", "text/css");
            NUnit.Framework.Assert.AreEqual(expectedAttr, attr);
        }

        [NUnit.Framework.Test]
        public virtual void SvgCssResolverXlinkTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupImage = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("image"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Attributes imageAttributes = jsoupImage.Attributes();
            imageAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute("xlink:href", "itis.jpg"));
            JsoupElementNode node = new JsoupElementNode(jsoupImage);
            SvgConverterProperties scp = new SvgConverterProperties();
            scp.SetBaseUri(baseUri);
            SvgProcessorContext processorContext = new SvgProcessorContext(scp);
            SvgStyleResolver sr = new SvgStyleResolver(node, processorContext);
            IDictionary<String, String> attr = sr.ResolveStyles(node, new SvgCssContext());
            String fileName = baseUri + "itis.jpg";
            String expectedUrl = UrlUtil.ToNormalizedURI(fileName).ToString();
            String expectedUrlAnotherValidVersion;
            if (expectedUrl.StartsWith("file:///")) {
                expectedUrlAnotherValidVersion = "file:/" + expectedUrl.Substring("file:///".Length);
            }
            else {
                if (expectedUrl.StartsWith("file:/")) {
                    expectedUrlAnotherValidVersion = "file:///" + expectedUrl.Substring("file:/".Length);
                }
                else {
                    expectedUrlAnotherValidVersion = expectedUrl;
                }
            }
            String url = attr.Get("xlink:href");
            // Both variants(namely with triple and single slashes) are valid.
            NUnit.Framework.Assert.IsTrue(expectedUrl.Equals(url) || expectedUrlAnotherValidVersion.Equals(url));
        }

        [NUnit.Framework.Test]
        public virtual void SvgCssResolveHashXlinkTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupImage = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("image"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Attributes imageAttributes = jsoupImage.Attributes();
            imageAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute("xlink:href", "#testid"));
            JsoupElementNode node = new JsoupElementNode(jsoupImage);
            SvgConverterProperties scp = new SvgConverterProperties();
            scp.SetBaseUri(baseUri);
            SvgProcessorContext processorContext = new SvgProcessorContext(scp);
            SvgStyleResolver sr = new SvgStyleResolver(node, processorContext);
            IDictionary<String, String> attr = sr.ResolveStyles(node, new SvgCssContext());
            NUnit.Framework.Assert.AreEqual("#testid", attr.Get("xlink:href"));
        }

        [NUnit.Framework.Test]
        public virtual void OverrideDefaultStyleTest() {
            ICssResolver styleResolver = new SvgStyleResolver(new SvgProcessorContext(new SvgConverterProperties()));
            iText.StyledXmlParser.Jsoup.Nodes.Element svg = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            svg.Attributes().Put(SvgConstants.Attributes.STYLE, "stroke:white");
            INode svgNode = new JsoupElementNode(svg);
            IDictionary<String, String> resolvedStyles = styleResolver.ResolveStyles(svgNode, new SvgCssContext());
            NUnit.Framework.Assert.AreEqual("white", resolvedStyles.Get(SvgConstants.Attributes.STROKE));
        }

        [NUnit.Framework.Test]
        public virtual void SvgCssResolverStyleTagTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element styleTag = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("style"), "");
            TextNode styleContents = new TextNode("\n" + "\tellipse{\n" + "\t\tstroke-width:1.76388889;\n" + "\t\tstroke:#da0000;\n"
                 + "\t\tstroke-opacity:1;\n" + "\t}\n" + "  ");
            JsoupElementNode jSoupStyle = new JsoupElementNode(styleTag);
            jSoupStyle.AddChild(new JsoupTextNode(styleContents));
            iText.StyledXmlParser.Jsoup.Nodes.Element ellipse = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("ellipse"), "");
            JsoupElementNode jSoupEllipse = new JsoupElementNode(ellipse);
            SvgProcessorContext context = new SvgProcessorContext(new SvgConverterProperties());
            SvgStyleResolver resolver = new SvgStyleResolver(jSoupStyle, context);
            AbstractCssContext svgContext = new SvgCssContext();
            IDictionary<String, String> actual = resolver.ResolveStyles(jSoupEllipse, svgContext);
            IDictionary<String, String> expected = new Dictionary<String, String>();
            expected.Put("stroke-width", "1.76388889");
            expected.Put("stroke", "#da0000");
            expected.Put("stroke-opacity", "1");
            expected.Put("font-size", "12pt");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void FontsResolverTagTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element styleTag = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("style"), "");
            TextNode styleContents = new TextNode("\n" + "\t@font-face{\n" + "\t\tfont-family:Courier;\n" + "\t\tsrc:url(#Super Sans);\n"
                 + "\t}\n" + "  ");
            JsoupElementNode jSoupStyle = new JsoupElementNode(styleTag);
            jSoupStyle.AddChild(new JsoupTextNode(styleContents));
            SvgProcessorContext context = new SvgProcessorContext(new SvgConverterProperties());
            SvgStyleResolver resolver = new SvgStyleResolver(jSoupStyle, context);
            IList<CssFontFaceRule> fontFaceRuleList = resolver.GetFonts();
            NUnit.Framework.Assert.AreEqual(1, fontFaceRuleList.Count);
            NUnit.Framework.Assert.AreEqual(2, fontFaceRuleList[0].GetProperties().Count);
        }

        [NUnit.Framework.Test]
        public virtual void NestedCssVariableTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element styleTag = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("style"), "");
            TextNode styleContents = new TextNode("\tspan {\n" + "\t\t--test-var: 30px;\n" + "\t}\n" + ".a {\n" + "    --test-var2: 50px;\n"
                 + "}\n" + "div {\n" + "    margin: var(--test-var,var(--test-var2));\n" + "}\n");
            JsoupElementNode jSoupStyle = new JsoupElementNode(styleTag);
            jSoupStyle.AddChild(new JsoupTextNode(styleContents));
            iText.StyledXmlParser.Jsoup.Nodes.Element div = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("div"), "");
            div.Attributes().Put("class", "a");
            JsoupElementNode jSoupDiv = new JsoupElementNode(div);
            SvgProcessorContext context = new SvgProcessorContext(new SvgConverterProperties());
            SvgStyleResolver resolver = new SvgStyleResolver(jSoupStyle, context);
            AbstractCssContext svgContext = new SvgCssContext();
            IDictionary<String, String> actual = resolver.ResolveStyles(jSoupDiv, svgContext);
            IDictionary<String, String> expected = new Dictionary<String, String>();
            expected.Put("class", "a");
            expected.Put("--test-var2", "50px");
            expected.Put("margin-top", "50px");
            expected.Put("margin-right", "50px");
            expected.Put("margin-bottom", "50px");
            expected.Put("margin-left", "50px");
            expected.Put("font-size", "12pt");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION, 
            Count = 2)]
        [NUnit.Framework.Test]
        public virtual void InvalidValueCssVariableTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element styleTag = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("style"), "");
            TextNode styleContents = new TextNode("    p {\n" + "        word-break: var(--test-var);\n" + "    }\n" +
                 "    div {\n" + "\t\t--test-var: incorrect;\n" + "\t}");
            JsoupElementNode jSoupStyle = new JsoupElementNode(styleTag);
            jSoupStyle.AddChild(new JsoupTextNode(styleContents));
            iText.StyledXmlParser.Jsoup.Nodes.Element div = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("div"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element paragraph = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("p"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element nestedParagraph = new iText.StyledXmlParser.Jsoup.Nodes.Element(
                iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("p"), "");
            JsoupElementNode jSoupParagraph = new JsoupElementNode(paragraph);
            JsoupElementNode jSoupNestedParagraph = new JsoupElementNode(nestedParagraph);
            JsoupElementNode jSoupDiv = new JsoupElementNode(div);
            jSoupDiv.AddChild(jSoupNestedParagraph);
            SvgProcessorContext context = new SvgProcessorContext(new SvgConverterProperties());
            SvgStyleResolver resolver = new SvgStyleResolver(jSoupStyle, context);
            AbstractCssContext svgContext = new SvgCssContext();
            jSoupDiv.SetStyles(resolver.ResolveStyles(jSoupDiv, svgContext));
            IDictionary<String, String> nestedStyles = resolver.ResolveStyles(jSoupNestedParagraph, svgContext);
            IDictionary<String, String> styles = resolver.ResolveStyles(jSoupParagraph, svgContext);
            IDictionary<String, String> expectedStyles = new Dictionary<String, String>();
            expectedStyles.Put("font-size", "12pt");
            NUnit.Framework.Assert.AreEqual(expectedStyles, styles);
            IDictionary<String, String> expectedNestedStyles = new Dictionary<String, String>();
            expectedNestedStyles.Put("--test-var", "incorrect");
            expectedNestedStyles.Put("font-size", "12pt");
            NUnit.Framework.Assert.AreEqual(expectedNestedStyles, nestedStyles);
        }

        public static IEnumerable<Object[]> DivVariablesTestProvider() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { "a", "30px", "30px" }, new Object[] { "b", "50px"
                , "30px" }, new Object[] { "c d", "35px", "35px" } });
        }

        [NUnit.Framework.TestCaseSource("DivVariablesTestProvider")]
        public virtual void CssVariableInTheSameScopeTest(String divClass, String expectedMargin, String expectedVarValue
            ) {
            iText.StyledXmlParser.Jsoup.Nodes.Element styleTag = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("style"), "");
            TextNode styleContents = new TextNode("\tdiv {\n" + "\t\t--test-var: 30px;\n" + "\t}\n" + "    div.a {\n" 
                + "        margin: var(--test-var,40px);\n" + "    }\n" + "    div.b {\n" + "        margin: var(--other-var,50px);\n"
                 + "    }\n" + "    div.c {\n" + "        --test-var: 35px;\n" + "    }\n" + "    div.c.d {\n" + "        margin: var(--test-var,40px);\n"
                 + "    }");
            JsoupElementNode jSoupStyle = new JsoupElementNode(styleTag);
            jSoupStyle.AddChild(new JsoupTextNode(styleContents));
            iText.StyledXmlParser.Jsoup.Nodes.Element div = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("div"), "");
            div.Attributes().Put("class", divClass);
            JsoupElementNode jSoupDiv = new JsoupElementNode(div);
            SvgProcessorContext context = new SvgProcessorContext(new SvgConverterProperties());
            SvgStyleResolver resolver = new SvgStyleResolver(jSoupStyle, context);
            AbstractCssContext svgContext = new SvgCssContext();
            IDictionary<String, String> actual = resolver.ResolveStyles(jSoupDiv, svgContext);
            IDictionary<String, String> expected = new Dictionary<String, String>();
            expected.Put("class", divClass);
            expected.Put("--test-var", expectedVarValue);
            expected.Put("margin-top", expectedMargin);
            expected.Put("margin-right", expectedMargin);
            expected.Put("margin-bottom", expectedMargin);
            expected.Put("margin-left", expectedMargin);
            expected.Put("font-size", "12pt");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void CssVariableInheritanceTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element styleTag = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("style"), "");
            TextNode styleContents = new TextNode("\tol {\n" + "\t\t--test-var: circle;\n" + "\t}\n" + "    ul {\n" + 
                "        list-style-type: var(--test-var, square);\n" + "    }\n" + "    ol {\n" + "        list-style-type: var(--test-var, square);\n"
                 + "    }");
            JsoupElementNode jSoupStyle = new JsoupElementNode(styleTag);
            jSoupStyle.AddChild(new JsoupTextNode(styleContents));
            iText.StyledXmlParser.Jsoup.Nodes.Element unorderedList = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("ul"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element orderedList = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("ol"), "");
            JsoupElementNode jSoupUnorderedList = new JsoupElementNode(unorderedList);
            JsoupElementNode jSoupOrderedList = new JsoupElementNode(orderedList);
            JsoupElementNode jSoupItemUnordered = new JsoupElementNode(new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("li"), ""));
            JsoupElementNode jSoupItemOrdered = new JsoupElementNode(new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("li"), ""));
            jSoupUnorderedList.AddChild(jSoupItemUnordered);
            jSoupOrderedList.AddChild(jSoupItemOrdered);
            SvgProcessorContext context = new SvgProcessorContext(new SvgConverterProperties());
            SvgStyleResolver resolver = new SvgStyleResolver(jSoupStyle, context);
            AbstractCssContext svgContext = new SvgCssContext();
            jSoupUnorderedList.SetStyles(resolver.ResolveStyles(jSoupUnorderedList, svgContext));
            jSoupOrderedList.SetStyles(resolver.ResolveStyles(jSoupOrderedList, svgContext));
            IDictionary<String, String> unorderedStyles = resolver.ResolveStyles(jSoupItemUnordered, svgContext);
            IDictionary<String, String> orderedStyles = resolver.ResolveStyles(jSoupItemOrdered, svgContext);
            IDictionary<String, String> expectedUnorderedStyles = new Dictionary<String, String>();
            expectedUnorderedStyles.Put("font-size", "12pt");
            expectedUnorderedStyles.Put("list-style-type", "square");
            NUnit.Framework.Assert.AreEqual(expectedUnorderedStyles, unorderedStyles);
            IDictionary<String, String> expectedOrderedStyles = new Dictionary<String, String>();
            expectedOrderedStyles.Put("--test-var", "circle");
            expectedOrderedStyles.Put("list-style-type", "circle");
            expectedOrderedStyles.Put("font-size", "12pt");
            NUnit.Framework.Assert.AreEqual(expectedOrderedStyles, orderedStyles);
        }

        [NUnit.Framework.Test]
        public virtual void FontFamilyResolvingTest1() {
            IDictionary<String, String> expected = new Dictionary<String, String>();
            expected.Put("font-family", "courier");
            expected.Put("font-size", "12pt");
            FontFamilyResolving("font-family:Courier;", expected);
        }

        [NUnit.Framework.Test]
        public virtual void FontFamilyResolvingTest2() {
            IDictionary<String, String> expected = new Dictionary<String, String>();
            expected.Put("font-family", "Courier");
            expected.Put("font-size", "12pt");
            FontFamilyResolving("font-family:'Courier';", expected);
        }

        [NUnit.Framework.Test]
        public virtual void FontFamilyResolvingTest3() {
            IDictionary<String, String> expected = new Dictionary<String, String>();
            expected.Put("font-family", "Courier");
            expected.Put("font-size", "12pt");
            FontFamilyResolving("font-family:\"Courier\";", expected);
        }

        [NUnit.Framework.Test]
        public virtual void FontFamilyResolvingTest4() {
            IDictionary<String, String> expected = new Dictionary<String, String>();
            expected.Put("font-family", " Courier");
            expected.Put("font-size", "12pt");
            FontFamilyResolving("font-family:\" Courier\" ;", expected);
        }

        [NUnit.Framework.Test]
        public virtual void FontFamilyResolvingTest5() {
            IDictionary<String, String> expected = new Dictionary<String, String>();
            expected.Put("font-family", "Courier");
            expected.Put("font-size", "12pt");
            FontFamilyResolving("font-family:\"Courier\", serif, Times;", expected);
        }

        [NUnit.Framework.Test]
        public virtual void FontFamilyResolvingTest6() {
            IDictionary<String, String> expected = new Dictionary<String, String>();
            expected.Put("font-family", "serif");
            expected.Put("font-size", "12pt");
            FontFamilyResolving("font-family:serif, \"Courier\", Times;", expected);
        }

        private static void FontFamilyResolving(String styleAttr, IDictionary<String, String> expected) {
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupText = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("text"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Attributes textAttributes = jsoupText.Attributes();
            textAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute("style", styleAttr));
            INode text = new JsoupElementNode(jsoupText);
            ICssResolver resolver = new SvgStyleResolver(text, new SvgProcessorContext(new SvgConverterProperties()));
            IDictionary<String, String> actual = resolver.ResolveStyles(text, new SvgCssContext());
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
