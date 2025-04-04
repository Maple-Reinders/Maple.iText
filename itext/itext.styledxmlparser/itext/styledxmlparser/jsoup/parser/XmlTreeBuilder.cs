/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using System.Collections.Generic;
using System.IO;
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Nodes;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>
    /// Use the
    /// <c>XmlTreeBuilder</c>
    /// when you want to parse XML without any of the HTML DOM rules being applied to the
    /// document.
    /// </summary>
    /// <remarks>
    /// Use the
    /// <c>XmlTreeBuilder</c>
    /// when you want to parse XML without any of the HTML DOM rules being applied to the
    /// document.
    /// <para />
    /// Usage example:
    /// <c>Document xmlDoc = Jsoup.parse(html, baseUrl, Parser.xmlParser());</c>
    /// </remarks>
    public class XmlTreeBuilder : TreeBuilder {
//\cond DO_NOT_DOCUMENT
        internal override ParseSettings DefaultSettings() {
            return ParseSettings.preserveCase;
        }
//\endcond

        protected internal override void InitialiseParse(TextReader input, String baseUri, iText.StyledXmlParser.Jsoup.Parser.Parser
             parser) {
            base.InitialiseParse(input, baseUri, parser);
            stack.Add(doc);
            // place the document onto the stack. differs from HtmlTreeBuilder (not on stack)
            doc.OutputSettings().Syntax(iText.StyledXmlParser.Jsoup.Nodes.Syntax.xml).EscapeMode(Entities.EscapeMode.xhtml
                ).PrettyPrint(false);
        }

//\cond DO_NOT_DOCUMENT
        // as XML, we don't understand what whitespace is significant or not
        internal virtual Document Parse(TextReader input, String baseUri) {
            return Parse(input, baseUri, new iText.StyledXmlParser.Jsoup.Parser.Parser(this));
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual Document Parse(String input, String baseUri) {
            return Parse(new StringReader(input), baseUri, new iText.StyledXmlParser.Jsoup.Parser.Parser(this));
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal override TreeBuilder NewInstance() {
            return new XmlTreeBuilder();
        }
//\endcond

        protected internal override bool Process(Token token) {
            // start tag, end tag, doctype, comment, character, eof
            switch (token.type) {
                case iText.StyledXmlParser.Jsoup.Parser.TokenType.StartTag: {
                    Insert(token.AsStartTag());
                    break;
                }

                case iText.StyledXmlParser.Jsoup.Parser.TokenType.EndTag: {
                    PopStackToClose(token.AsEndTag());
                    break;
                }

                case iText.StyledXmlParser.Jsoup.Parser.TokenType.Comment: {
                    Insert(token.AsComment());
                    break;
                }

                case iText.StyledXmlParser.Jsoup.Parser.TokenType.Character: {
                    Insert(token.AsCharacter());
                    break;
                }

                case iText.StyledXmlParser.Jsoup.Parser.TokenType.Doctype: {
                    Insert(token.AsDoctype());
                    break;
                }

                case iText.StyledXmlParser.Jsoup.Parser.TokenType.EOF: {
                    // could put some normalisation here if desired
                    break;
                }

                default: {
                    Validate.Fail("Unexpected token type: " + token.type);
                    break;
                }
            }
            return true;
        }

        private void InsertNode(iText.StyledXmlParser.Jsoup.Nodes.Node node) {
            CurrentElement().AppendChild(node);
        }

//\cond DO_NOT_DOCUMENT
        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element Insert(Token.StartTag startTag) {
            iText.StyledXmlParser.Jsoup.Parser.Tag tag = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf(startTag.Name(
                ), settings);
            if (startTag.HasAttributes()) {
                startTag.attributes.Deduplicate(settings);
            }
            iText.StyledXmlParser.Jsoup.Nodes.Element el = new iText.StyledXmlParser.Jsoup.Nodes.Element(tag, null, settings
                .NormalizeAttributes(startTag.attributes));
            InsertNode(el);
            if (startTag.IsSelfClosing()) {
                if (!tag.IsKnownTag()) {
                    // unknown tag, remember this is self closing for output. see above.
                    tag.SetSelfClosing();
                }
            }
            else {
                stack.Add(el);
            }
            return el;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void Insert(Token.Comment commentToken) {
            Comment comment = new Comment(commentToken.GetData());
            iText.StyledXmlParser.Jsoup.Nodes.Node insert = comment;
            if (commentToken.bogus && comment.IsXmlDeclaration()) {
                // xml declarations are emitted as bogus comments (which is right for html, but not xml)
                // so we do a bit of a hack and parse the data as an element to pull the attributes out
                XmlDeclaration decl = comment.AsXmlDeclaration();
                // else, we couldn't parse it as a decl, so leave as a comment
                if (decl != null) {
                    insert = decl;
                }
            }
            InsertNode(insert);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void Insert(Token.Character token) {
            String data = token.GetData();
            InsertNode(token.IsCData() ? new CDataNode(data) : new TextNode(data));
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void Insert(Token.Doctype d) {
            DocumentType doctypeNode = new DocumentType(settings.NormalizeTag(d.GetName()), d.GetPublicIdentifier(), d
                .GetSystemIdentifier());
            doctypeNode.SetPubSysKey(d.GetPubSysKey());
            InsertNode(doctypeNode);
        }
//\endcond

        /// <summary>If the stack contains an element with this tag's name, pop up the stack to remove the first occurrence.
        ///     </summary>
        /// <remarks>
        /// If the stack contains an element with this tag's name, pop up the stack to remove the first occurrence. If not
        /// found, skips.
        /// </remarks>
        /// <param name="endTag">tag to close</param>
        private void PopStackToClose(Token.EndTag endTag) {
            String elName = settings.NormalizeTag(endTag.tagName);
            iText.StyledXmlParser.Jsoup.Nodes.Element firstFound = null;
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = stack[pos];
                if (next.NodeName().Equals(elName)) {
                    firstFound = next;
                    break;
                }
            }
            if (firstFound == null) {
                return;
            }
            // not found, skip
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = stack[pos];
                stack.JRemoveAt(pos);
                if (next == firstFound) {
                    break;
                }
            }
        }

//\cond DO_NOT_DOCUMENT
        internal virtual IList<iText.StyledXmlParser.Jsoup.Nodes.Node> ParseFragment(String inputFragment, String 
            baseUri, iText.StyledXmlParser.Jsoup.Parser.Parser parser) {
            InitialiseParse(new StringReader(inputFragment), baseUri, parser);
            RunParser();
            return doc.ChildNodes();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal override IList<iText.StyledXmlParser.Jsoup.Nodes.Node> ParseFragment(String inputFragment, iText.StyledXmlParser.Jsoup.Nodes.Element
             context, String baseUri, iText.StyledXmlParser.Jsoup.Parser.Parser parser) {
            return ParseFragment(inputFragment, baseUri, parser);
        }
//\endcond
    }
}
