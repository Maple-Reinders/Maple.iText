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
    public abstract class TreeBuilder {
        protected internal iText.StyledXmlParser.Jsoup.Parser.Parser parser;

//\cond DO_NOT_DOCUMENT
        internal CharacterReader reader;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal Tokeniser tokeniser;
//\endcond

        protected internal Document doc;

        // current doc we are building into
        protected internal List<iText.StyledXmlParser.Jsoup.Nodes.Element> stack;

        // the stack of open elements
        protected internal String baseUri;

        // current base uri, for creating new elements
        protected internal Token currentToken;

        // currentToken is used only for error tracking.
        protected internal ParseSettings settings;

        private Token.StartTag start = new Token.StartTag();

        // start tag to process
        private Token.EndTag end = new Token.EndTag();

//\cond DO_NOT_DOCUMENT
        internal abstract ParseSettings DefaultSettings();
//\endcond

        protected internal virtual void InitialiseParse(TextReader input, String baseUri, iText.StyledXmlParser.Jsoup.Parser.Parser
             parser) {
            Validate.NotNull(input, "String input must not be null");
            Validate.NotNull(baseUri, "BaseURI must not be null");
            Validate.NotNull(parser);
            doc = new Document(baseUri);
            doc.Parser(parser);
            this.parser = parser;
            settings = parser.Settings();
            reader = new CharacterReader(input);
            currentToken = null;
            tokeniser = new Tokeniser(reader, parser.GetErrors());
            stack = new List<iText.StyledXmlParser.Jsoup.Nodes.Element>(32);
            this.baseUri = baseUri;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual Document Parse(TextReader input, String baseUri, iText.StyledXmlParser.Jsoup.Parser.Parser
             parser) {
            InitialiseParse(input, baseUri, parser);
            RunParser();
            // tidy up - as the Parser and Treebuilder are retained in document for settings / fragments
            reader.Close();
            reader = null;
            tokeniser = null;
            stack = null;
            return doc;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Create a new copy of this TreeBuilder</summary>
        /// <returns>copy, ready for a new parse</returns>
        internal abstract TreeBuilder NewInstance();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal abstract IList<iText.StyledXmlParser.Jsoup.Nodes.Node> ParseFragment(String inputFragment, iText.StyledXmlParser.Jsoup.Nodes.Element
             context, String baseUri, iText.StyledXmlParser.Jsoup.Parser.Parser parser);
//\endcond

        protected internal virtual void RunParser() {
            Tokeniser tokeniser = this.tokeniser;
            iText.StyledXmlParser.Jsoup.Parser.TokenType eof = iText.StyledXmlParser.Jsoup.Parser.TokenType.EOF;
            while (true) {
                Token token = tokeniser.Read();
                Process(token);
                token.Reset();
                if (token.type == eof) {
                    break;
                }
            }
        }

        protected internal abstract bool Process(Token token);

        protected internal virtual bool ProcessStartTag(String name) {
            Token.StartTag start = this.start;
            if (currentToken == start) {
                // don't recycle an in-use token
                return Process(new Token.StartTag().Name(name));
            }
            return Process(((Token.Tag)start.Reset()).Name(name));
        }

        public virtual bool ProcessStartTag(String name, Attributes attrs) {
            Token.StartTag start = this.start;
            if (currentToken == start) {
                // don't recycle an in-use token
                return Process(new Token.StartTag().NameAttr(name, attrs));
            }
            start.Reset();
            start.NameAttr(name, attrs);
            return Process(start);
        }

        protected internal virtual bool ProcessEndTag(String name) {
            if (currentToken == end) {
                // don't recycle an in-use token
                return Process(new Token.EndTag().Name(name));
            }
            return Process(((Token.Tag)end.Reset()).Name(name));
        }

        protected internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element CurrentElement() {
            int size = stack.Count;
            return size > 0 ? stack[size - 1] : null;
        }

        /// <summary>If the parser is tracking errors, add an error at the current position.</summary>
        /// <param name="msg">error message</param>
        protected internal virtual void Error(String msg) {
            ParseErrorList errors = parser.GetErrors();
            if (errors.CanAddError()) {
                errors.Add(new ParseError(reader.Pos(), msg));
            }
        }

        /// <summary>(An internal method, visible for Element.</summary>
        /// <remarks>
        /// (An internal method, visible for Element. For HTML parse, signals that script and style text should be treated as
        /// Data Nodes).
        /// </remarks>
        protected internal virtual bool IsContentForTagData(String normalName) {
            return false;
        }
    }
}
