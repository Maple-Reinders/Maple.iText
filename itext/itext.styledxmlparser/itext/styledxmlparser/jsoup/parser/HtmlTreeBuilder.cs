/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using System.Collections.Generic;
using System.IO;
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Jsoup.Select;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>HTML Tree Builder; creates a DOM from Tokens.</summary>
    public class HtmlTreeBuilder : TreeBuilder {
//\cond DO_NOT_DOCUMENT
        // tag searches. must be sorted, used in StringUtil.inSorted. HtmlTreeBuilderTest validates they're sorted.
        internal static readonly String[] TagsSearchInScope = new String[] { "applet", "caption", "html", "marquee"
            , "object", "table", "td", "th" };
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static readonly String[] TagSearchList = new String[] { "ol", "ul" };
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static readonly String[] TagSearchButton = new String[] { "button" };
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static readonly String[] TagSearchTableScope = new String[] { "html", "table" };
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static readonly String[] TagSearchSelectScope = new String[] { "optgroup", "option" };
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static readonly String[] TagSearchEndTags = new String[] { "dd", "dt", "li", "optgroup", "option"
            , "p", "rp", "rt" };
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static readonly String[] TagSearchSpecial = new String[] { "address", "applet", "area", "article"
            , "aside", "base", "basefont", "bgsound", "blockquote", "body", "br", "button", "caption", "center", "col"
            , "colgroup", "command", "dd", "details", "dir", "div", "dl", "dt", "embed", "fieldset", "figcaption", 
            "figure", "footer", "form", "frame", "frameset", "h1", "h2", "h3", "h4", "h5", "h6", "head", "header", 
            "hgroup", "hr", "html", "iframe", "img", "input", "isindex", "li", "link", "listing", "marquee", "menu"
            , "meta", "nav", "noembed", "noframes", "noscript", "object", "ol", "p", "param", "plaintext", "pre", 
            "script", "section", "select", "style", "summary", "table", "tbody", "td", "textarea", "tfoot", "th", 
            "thead", "title", "tr", "ul", "wbr", "xmp" };
//\endcond

        public const int MaxScopeSearchDepth = 100;

        // prevents the parser bogging down in exceptionally broken pages
        private HtmlTreeBuilderState state;

        // the current state
        private HtmlTreeBuilderState originalState;

        // original / marked state
        private bool baseUriSetFromDoc;

        private iText.StyledXmlParser.Jsoup.Nodes.Element headElement;

        // the current head element
        private FormElement formElement;

        // the current form element
        private iText.StyledXmlParser.Jsoup.Nodes.Element contextElement;

        // fragment parse context -- could be null even if fragment parsing
        private List<iText.StyledXmlParser.Jsoup.Nodes.Element> formattingElements;

        // active (open) formatting elements
        private IList<String> pendingTableCharacters;

        // chars in table to be shifted out
        private Token.EndTag emptyEnd;

        // reused empty end tag
        private bool framesetOk;

        // if ok to go into frameset
        private bool fosterInserts;

        // if next inserts should be fostered
        private bool fragmentParsing;

//\cond DO_NOT_DOCUMENT
        // if parsing a fragment of html
        internal override ParseSettings DefaultSettings() {
            return ParseSettings.htmlDefault;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal override TreeBuilder NewInstance() {
            return new HtmlTreeBuilder();
        }
//\endcond

        protected internal override void InitialiseParse(TextReader input, String baseUri, iText.StyledXmlParser.Jsoup.Parser.Parser
             parser) {
            base.InitialiseParse(input, baseUri, parser);
            // this is a bit mucky.
            state = HtmlTreeBuilderState.Initial;
            originalState = null;
            baseUriSetFromDoc = false;
            headElement = null;
            formElement = null;
            contextElement = null;
            formattingElements = new List<iText.StyledXmlParser.Jsoup.Nodes.Element>();
            pendingTableCharacters = new List<String>();
            emptyEnd = new Token.EndTag();
            framesetOk = true;
            fosterInserts = false;
            fragmentParsing = false;
        }

//\cond DO_NOT_DOCUMENT
        internal override IList<iText.StyledXmlParser.Jsoup.Nodes.Node> ParseFragment(String inputFragment, iText.StyledXmlParser.Jsoup.Nodes.Element
             context, String baseUri, iText.StyledXmlParser.Jsoup.Parser.Parser parser) {
            // context may be null
            state = HtmlTreeBuilderState.Initial;
            InitialiseParse(new StringReader(inputFragment), baseUri, parser);
            contextElement = context;
            fragmentParsing = true;
            iText.StyledXmlParser.Jsoup.Nodes.Element root = null;
            if (context != null) {
                if (context.OwnerDocument() != null) {
                    // quirks setup:
                    doc.QuirksMode(context.OwnerDocument().QuirksMode());
                }
                // initialise the tokeniser state:
                String contextTag = context.NormalName();
                if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.In(contextTag, "title", "textarea")) {
                    tokeniser.Transition(TokeniserState.Rcdata);
                }
                else {
                    if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.In(contextTag, "iframe", "noembed", "noframes", "style"
                        , "xmp")) {
                        tokeniser.Transition(TokeniserState.Rawtext);
                    }
                    else {
                        if (contextTag.Equals("script")) {
                            tokeniser.Transition(TokeniserState.ScriptData);
                        }
                        else {
                            if (contextTag.Equals(("noscript"))) {
                                tokeniser.Transition(TokeniserState.Data);
                            }
                            else {
                                // if scripting enabled, rawtext
                                if (contextTag.Equals("plaintext")) {
                                    tokeniser.Transition(TokeniserState.Data);
                                }
                                else {
                                    tokeniser.Transition(TokeniserState.Data);
                                }
                            }
                        }
                    }
                }
                // default
                root = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf(contextTag
                    , settings), baseUri);
                doc.AppendChild(root);
                stack.Add(root);
                ResetInsertionMode();
                // setup form element to nearest form on context (up ancestor chain). ensures form controls are associated
                // with form correctly
                Elements contextChain = context.Parents();
                contextChain.Add(0, context);
                foreach (iText.StyledXmlParser.Jsoup.Nodes.Element parent in contextChain) {
                    if (parent is FormElement) {
                        formElement = (FormElement)parent;
                        break;
                    }
                }
            }
            RunParser();
            if (context != null) {
                // depending on context and the input html, content may have been added outside of the root el
                // e.g. context=p, input=div, the div will have been pushed out.
                IList<iText.StyledXmlParser.Jsoup.Nodes.Node> nodes = root.SiblingNodes();
                if (!nodes.IsEmpty()) {
                    root.InsertChildren(-1, nodes);
                }
                return root.ChildNodes();
            }
            else {
                return doc.ChildNodes();
            }
        }
//\endcond

        protected internal override bool Process(Token token) {
            currentToken = token;
            return this.state.Process(token, this);
        }

//\cond DO_NOT_DOCUMENT
        internal virtual bool Process(Token token, HtmlTreeBuilderState state) {
            currentToken = token;
            return state.Process(token, this);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void Transition(HtmlTreeBuilderState state) {
            this.state = state;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual HtmlTreeBuilderState State() {
            return state;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void MarkInsertionMode() {
            originalState = state;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual HtmlTreeBuilderState OriginalState() {
            return originalState;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void FramesetOk(bool framesetOk) {
            this.framesetOk = framesetOk;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool FramesetOk() {
            return framesetOk;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual Document GetDocument() {
            return doc;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual String GetBaseUri() {
            return baseUri;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void MaybeSetBaseUri(iText.StyledXmlParser.Jsoup.Nodes.Element @base) {
            if (baseUriSetFromDoc) {
                // only listen to the first <base href> in parse
                return;
            }
            String href = @base.AbsUrl("href");
            if (href.Length != 0) {
                // ignore <base target> etc
                baseUri = href;
                baseUriSetFromDoc = true;
                doc.SetBaseUri(href);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        // set on the doc so doc.createElement(Tag) will get updated base, and to update all descendants
        internal virtual bool IsFragmentParsing() {
            return fragmentParsing;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void Error(HtmlTreeBuilderState state) {
            if (parser.GetErrors().CanAddError()) {
                parser.GetErrors().Add(new ParseError(reader.Pos(), "Unexpected token [{0}] when in state [{1}]", currentToken
                    .TokenType(), state));
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element Insert(Token.StartTag startTag) {
            // cleanup duplicate attributes:
            if (startTag.HasAttributes() && !startTag.attributes.IsEmpty()) {
                int dupes = startTag.attributes.Deduplicate(settings);
                if (dupes > 0) {
                    Error("Duplicate attribute");
                }
            }
            // handle empty unknown tags
            // when the spec expects an empty tag, will directly hit insertEmpty, so won't generate this fake end tag.
            if (startTag.IsSelfClosing()) {
                iText.StyledXmlParser.Jsoup.Nodes.Element el = InsertEmpty(startTag);
                stack.Add(el);
                tokeniser.Transition(TokeniserState.Data);
                // handles <script />, otherwise needs breakout steps from script data
                tokeniser.Emit(((Token.Tag)emptyEnd.Reset()).Name(el.TagName()));
                // ensure we get out of whatever state we are in. emitted for yielded processing
                return el;
            }
            iText.StyledXmlParser.Jsoup.Nodes.Element el_1 = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf(startTag.Name(), settings), null, settings.NormalizeAttributes(startTag.attributes));
            Insert(el_1);
            return el_1;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element InsertStartTag(String startTagName) {
            iText.StyledXmlParser.Jsoup.Nodes.Element el = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf(startTagName, settings), null);
            Insert(el);
            return el;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void Insert(iText.StyledXmlParser.Jsoup.Nodes.Element el) {
            InsertNode(el);
            stack.Add(el);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element InsertEmpty(Token.StartTag startTag) {
            iText.StyledXmlParser.Jsoup.Parser.Tag tag = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf(startTag.Name(
                ), settings);
            iText.StyledXmlParser.Jsoup.Nodes.Element el = new iText.StyledXmlParser.Jsoup.Nodes.Element(tag, null, settings
                .NormalizeAttributes(startTag.attributes));
            InsertNode(el);
            if (startTag.IsSelfClosing()) {
                if (tag.IsKnownTag()) {
                    if (!tag.IsEmpty()) {
                        tokeniser.Error("Tag cannot be self closing; not a void tag");
                    }
                }
                else {
                    // unknown tag, remember this is self closing for output
                    tag.SetSelfClosing();
                }
            }
            return el;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual FormElement InsertForm(Token.StartTag startTag, bool onStack) {
            iText.StyledXmlParser.Jsoup.Parser.Tag tag = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf(startTag.Name(
                ), settings);
            FormElement el = new FormElement(tag, null, settings.NormalizeAttributes(startTag.attributes));
            SetFormElement(el);
            InsertNode(el);
            if (onStack) {
                stack.Add(el);
            }
            return el;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void Insert(Token.Comment commentToken) {
            Comment comment = new Comment(commentToken.GetData());
            InsertNode(comment);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void Insert(Token.Character characterToken) {
            iText.StyledXmlParser.Jsoup.Nodes.Node node;
            iText.StyledXmlParser.Jsoup.Nodes.Element el = CurrentElement();
            if (el == null) {
                el = doc;
            }
            // allows for whitespace to be inserted into the doc root object (not on the stack)
            String tagName = el.NormalName();
            String data = characterToken.GetData();
            if (characterToken.IsCData()) {
                node = new CDataNode(data);
            }
            else {
                if (IsContentForTagData(tagName)) {
                    node = new DataNode(data);
                }
                else {
                    node = new TextNode(data);
                }
            }
            el.AppendChild(node);
        }
//\endcond

        // doesn't use insertNode, because we don't foster these; and will always have a stack.
        private void InsertNode(iText.StyledXmlParser.Jsoup.Nodes.Node node) {
            // if the stack hasn't been set up yet, elements (doctype, comments) go into the doc
            if (stack.IsEmpty()) {
                doc.AppendChild(node);
            }
            else {
                if (IsFosterInserts()) {
                    InsertInFosterParent(node);
                }
                else {
                    CurrentElement().AppendChild(node);
                }
            }
            // connect form controls to their form element
            if (node is iText.StyledXmlParser.Jsoup.Nodes.Element && ((iText.StyledXmlParser.Jsoup.Nodes.Element)node)
                .Tag().IsFormListed()) {
                if (formElement != null) {
                    formElement.AddElement((iText.StyledXmlParser.Jsoup.Nodes.Element)node);
                }
            }
        }

//\cond DO_NOT_DOCUMENT
        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element Pop() {
            int size = stack.Count;
            return stack.JRemoveAt(size - 1);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void Push(iText.StyledXmlParser.Jsoup.Nodes.Element element) {
            stack.Add(element);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual List<iText.StyledXmlParser.Jsoup.Nodes.Element> GetStack() {
            return stack;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool OnStack(iText.StyledXmlParser.Jsoup.Nodes.Element el) {
            return IsElementInQueue(stack, el);
        }
//\endcond

        private const int maxQueueDepth = 256;

        // an arbitrary tension point between real HTML and crafted pain
        private bool IsElementInQueue(List<iText.StyledXmlParser.Jsoup.Nodes.Element> queue, iText.StyledXmlParser.Jsoup.Nodes.Element
             element) {
            int bottom = queue.Count - 1;
            int upper = bottom >= maxQueueDepth ? bottom - maxQueueDepth : 0;
            for (int pos = bottom; pos >= upper; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = queue[pos];
                if (next == element) {
                    return true;
                }
            }
            return false;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element GetFromStack(String elName) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = stack[pos];
                if (next.NormalName().Equals(elName)) {
                    return next;
                }
            }
            return null;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool RemoveFromStack(iText.StyledXmlParser.Jsoup.Nodes.Element el) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = stack[pos];
                if (next == el) {
                    stack.JRemoveAt(pos);
                    return true;
                }
            }
            return false;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element PopStackToClose(String elName) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element el = stack[pos];
                stack.JRemoveAt(pos);
                if (el.NormalName().Equals(elName)) {
                    return el;
                }
            }
            return null;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        // elnames is sorted, comes from Constants
        internal virtual void PopStackToClose(params String[] elNames) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = stack[pos];
                stack.JRemoveAt(pos);
                if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(next.NormalName(), elNames)) {
                    break;
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void PopStackToBefore(String elName) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = stack[pos];
                if (next.NormalName().Equals(elName)) {
                    break;
                }
                else {
                    stack.JRemoveAt(pos);
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void ClearStackToTableContext() {
            ClearStackToContext("table");
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void ClearStackToTableBodyContext() {
            ClearStackToContext("tbody", "tfoot", "thead", "template");
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void ClearStackToTableRowContext() {
            ClearStackToContext("tr", "template");
        }
//\endcond

        private void ClearStackToContext(params String[] nodeNames) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = stack[pos];
                if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.In(next.NormalName(), nodeNames) || next.NormalName().
                    Equals("html")) {
                    break;
                }
                else {
                    stack.JRemoveAt(pos);
                }
            }
        }

//\cond DO_NOT_DOCUMENT
        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element AboveOnStack(iText.StyledXmlParser.Jsoup.Nodes.Element
             el) {
            System.Diagnostics.Debug.Assert(OnStack(el));
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = stack[pos];
                if (next == el) {
                    return stack[pos - 1];
                }
            }
            return null;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void InsertOnStackAfter(iText.StyledXmlParser.Jsoup.Nodes.Element after, iText.StyledXmlParser.Jsoup.Nodes.Element
             @in) {
            int i = stack.LastIndexOf(after);
            Validate.IsTrue(i != -1);
            stack.Add(i + 1, @in);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void ReplaceOnStack(iText.StyledXmlParser.Jsoup.Nodes.Element @out, iText.StyledXmlParser.Jsoup.Nodes.Element
             @in) {
            ReplaceInQueue(stack, @out, @in);
        }
//\endcond

        private void ReplaceInQueue(List<iText.StyledXmlParser.Jsoup.Nodes.Element> queue, iText.StyledXmlParser.Jsoup.Nodes.Element
             @out, iText.StyledXmlParser.Jsoup.Nodes.Element @in) {
            int i = queue.LastIndexOf(@out);
            Validate.IsTrue(i != -1);
            queue[i] = @in;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void ResetInsertionMode() {
            bool last = false;
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element node = stack[pos];
                if (pos == 0) {
                    last = true;
                    node = contextElement;
                }
                String name = node != null ? node.NormalName() : "";
                if ("select".Equals(name)) {
                    Transition(HtmlTreeBuilderState.InSelect);
                    break;
                }
                else {
                    // frag
                    if (("td".Equals(name) || "th".Equals(name) && !last)) {
                        Transition(HtmlTreeBuilderState.InCell);
                        break;
                    }
                    else {
                        if ("tr".Equals(name)) {
                            Transition(HtmlTreeBuilderState.InRow);
                            break;
                        }
                        else {
                            if ("tbody".Equals(name) || "thead".Equals(name) || "tfoot".Equals(name)) {
                                Transition(HtmlTreeBuilderState.InTableBody);
                                break;
                            }
                            else {
                                if ("caption".Equals(name)) {
                                    Transition(HtmlTreeBuilderState.InCaption);
                                    break;
                                }
                                else {
                                    if ("colgroup".Equals(name)) {
                                        Transition(HtmlTreeBuilderState.InColumnGroup);
                                        break;
                                    }
                                    else {
                                        // frag
                                        if ("table".Equals(name)) {
                                            Transition(HtmlTreeBuilderState.InTable);
                                            break;
                                        }
                                        else {
                                            if ("head".Equals(name)) {
                                                Transition(HtmlTreeBuilderState.InBody);
                                                break;
                                            }
                                            else {
                                                // frag
                                                if ("body".Equals(name)) {
                                                    Transition(HtmlTreeBuilderState.InBody);
                                                    break;
                                                }
                                                else {
                                                    if ("frameset".Equals(name)) {
                                                        Transition(HtmlTreeBuilderState.InFrameset);
                                                        break;
                                                    }
                                                    else {
                                                        // frag
                                                        if ("html".Equals(name)) {
                                                            Transition(HtmlTreeBuilderState.BeforeHead);
                                                            break;
                                                        }
                                                        else {
                                                            // frag
                                                            if (last) {
                                                                Transition(HtmlTreeBuilderState.InBody);
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
//\endcond

        // frag
        private String[] specificScopeTarget = new String[] { null };

        private bool InSpecificScope(String targetName, String[] baseTypes, String[] extraTypes) {
            specificScopeTarget[0] = targetName;
            return InSpecificScope(specificScopeTarget, baseTypes, extraTypes);
        }

        private bool InSpecificScope(String[] targetNames, String[] baseTypes, String[] extraTypes) {
            // https://html.spec.whatwg.org/multipage/parsing.html#has-an-element-in-the-specific-scope
            int bottom = stack.Count - 1;
            int top = bottom > MaxScopeSearchDepth ? bottom - MaxScopeSearchDepth : 0;
            // don't walk too far up the tree
            for (int pos = bottom; pos >= top; pos--) {
                String elName = stack[pos].NormalName();
                if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(elName, targetNames)) {
                    return true;
                }
                if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(elName, baseTypes)) {
                    return false;
                }
                if (extraTypes != null && iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(elName, extraTypes)) {
                    return false;
                }
            }
            //Validate.fail("Should not be reachable"); // would end up false because hitting 'html' at root (basetypes)
            return false;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual bool InScope(String[] targetNames) {
            return InSpecificScope(targetNames, TagsSearchInScope, null);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool InScope(String targetName) {
            return InScope(targetName, null);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool InScope(String targetName, String[] extras) {
            return InSpecificScope(targetName, TagsSearchInScope, extras);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool InListItemScope(String targetName) {
            return InScope(targetName, TagSearchList);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool InButtonScope(String targetName) {
            return InScope(targetName, TagSearchButton);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool InTableScope(String targetName) {
            return InSpecificScope(targetName, TagSearchTableScope, null);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool InSelectScope(String targetName) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element el = stack[pos];
                String elName = el.NormalName();
                if (elName.Equals(targetName)) {
                    return true;
                }
                if (!iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(elName, TagSearchSelectScope)) {
                    // all elements except
                    return false;
                }
            }
            Validate.Fail("Should not be reachable");
            return false;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void SetHeadElement(iText.StyledXmlParser.Jsoup.Nodes.Element headElement) {
            this.headElement = headElement;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element GetHeadElement() {
            return headElement;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool IsFosterInserts() {
            return fosterInserts;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void SetFosterInserts(bool fosterInserts) {
            this.fosterInserts = fosterInserts;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual FormElement GetFormElement() {
            return formElement;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void SetFormElement(FormElement formElement) {
            this.formElement = formElement;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void NewPendingTableCharacters() {
            pendingTableCharacters = new List<String>();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual IList<String> GetPendingTableCharacters() {
            return pendingTableCharacters;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>11.2.5.2 Closing elements that have implied end tags</summary>
        /// <remarks>
        /// 11.2.5.2 Closing elements that have implied end tags
        /// <para />
        /// When the steps below require the UA to generate implied end tags, then, while the current node is a dd element, a
        /// dt element, an li element, an option element, an optgroup element, a p element, an rp element, or an rt element,
        /// the UA must pop the current node off the stack of open elements.
        /// </remarks>
        /// <param name="excludeTag">
        /// If a step requires the UA to generate implied end tags but lists an element to exclude from the
        /// process, then the UA must perform the above steps as if that element was not in the above list.
        /// </param>
        internal virtual void GenerateImpliedEndTags(String excludeTag) {
            while ((excludeTag != null && !CurrentElement().NormalName().Equals(excludeTag)) && iText.StyledXmlParser.Jsoup.Internal.StringUtil
                .InSorted(CurrentElement().NormalName(), TagSearchEndTags)) {
                Pop();
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void GenerateImpliedEndTags() {
            GenerateImpliedEndTags(null);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool IsSpecial(iText.StyledXmlParser.Jsoup.Nodes.Element el) {
            String name = el.NormalName();
            return iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, TagSearchSpecial);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element LastFormattingElement() {
            return formattingElements.Count > 0 ? formattingElements[formattingElements.Count - 1] : null;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual int PositionOfElement(iText.StyledXmlParser.Jsoup.Nodes.Element el) {
            for (int i = 0; i < formattingElements.Count; i++) {
                if (el == formattingElements[i]) {
                    return i;
                }
            }
            return -1;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element RemoveLastFormattingElement() {
            int size = formattingElements.Count;
            if (size > 0) {
                return formattingElements.JRemoveAt(size - 1);
            }
            else {
                return null;
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        // active formatting elements
        internal virtual void PushActiveFormattingElements(iText.StyledXmlParser.Jsoup.Nodes.Element @in) {
            this.CheckActiveFormattingElements(@in);
            formattingElements.Add(@in);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void PushWithBookmark(iText.StyledXmlParser.Jsoup.Nodes.Element @in, int bookmark) {
            this.CheckActiveFormattingElements(@in);
            formattingElements.Add(bookmark, @in);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void CheckActiveFormattingElements(iText.StyledXmlParser.Jsoup.Nodes.Element @in) {
            int numSeen = 0;
            for (int pos = formattingElements.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element el = formattingElements[pos];
                if (el == null) {
                    // marker
                    break;
                }
                if (IsSameFormattingElement(@in, el)) {
                    numSeen++;
                }
                if (numSeen == 3) {
                    formattingElements.JRemoveAt(pos);
                    break;
                }
            }
        }
//\endcond

        private bool IsSameFormattingElement(iText.StyledXmlParser.Jsoup.Nodes.Element a, iText.StyledXmlParser.Jsoup.Nodes.Element
             b) {
            // same if: same namespace, tag, and attributes. Element.equals only checks tag, might in future check children
            return a.NormalName().Equals(b.NormalName()) && 
                        // a.namespace().equals(b.namespace()) &&
                        a.Attributes().Equals(b.Attributes());
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void ReconstructFormattingElements() {
            iText.StyledXmlParser.Jsoup.Nodes.Element last = LastFormattingElement();
            if (last == null || OnStack(last)) {
                return;
            }
            iText.StyledXmlParser.Jsoup.Nodes.Element entry = last;
            int size = formattingElements.Count;
            int pos = size - 1;
            bool skip = false;
            while (true) {
                if (pos == 0) {
                    // step 4. if none before, skip to 8
                    skip = true;
                    break;
                }
                entry = formattingElements[--pos];
                // step 5. one earlier than entry
                if (entry == null || OnStack(entry)) {
                    // step 6 - neither marker nor on stack
                    break;
                }
            }
            // jump to 8, else continue back to 4
            while (true) {
                if (!skip) {
                    // step 7: on later than entry
                    entry = formattingElements[++pos];
                }
                Validate.NotNull(entry);
                // should not occur, as we break at last element
                // 8. create new element from element, 9 insert into current node, onto stack
                skip = false;
                // can only skip increment from 4.
                iText.StyledXmlParser.Jsoup.Nodes.Element newEl = InsertStartTag(entry.NormalName());
                newEl.Attributes().AddAll(entry.Attributes());
                // 10. replace entry with new entry
                formattingElements[pos] = newEl;
                // 11
                if (pos == size - 1) {
                    // if not last entry in list, jump to 7
                    break;
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void ClearFormattingElementsToLastMarker() {
            while (!formattingElements.IsEmpty()) {
                iText.StyledXmlParser.Jsoup.Nodes.Element el = RemoveLastFormattingElement();
                if (el == null) {
                    break;
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void RemoveFromActiveFormattingElements(iText.StyledXmlParser.Jsoup.Nodes.Element el) {
            for (int pos = formattingElements.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = formattingElements[pos];
                if (next == el) {
                    formattingElements.JRemoveAt(pos);
                    break;
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool IsInActiveFormattingElements(iText.StyledXmlParser.Jsoup.Nodes.Element el) {
            return IsElementInQueue(formattingElements, el);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element GetActiveFormattingElement(String nodeName) {
            for (int pos = formattingElements.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = formattingElements[pos];
                if (next == null) {
                    // scope marker
                    break;
                }
                else {
                    if (next.NormalName().Equals(nodeName)) {
                        return next;
                    }
                }
            }
            return null;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void ReplaceActiveFormattingElement(iText.StyledXmlParser.Jsoup.Nodes.Element @out, iText.StyledXmlParser.Jsoup.Nodes.Element
             @in) {
            ReplaceInQueue(formattingElements, @out, @in);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void InsertMarkerToFormattingElements() {
            formattingElements.Add(null);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void InsertInFosterParent(iText.StyledXmlParser.Jsoup.Nodes.Node @in) {
            iText.StyledXmlParser.Jsoup.Nodes.Element fosterParent;
            iText.StyledXmlParser.Jsoup.Nodes.Element lastTable = GetFromStack("table");
            bool isLastTableParent = false;
            if (lastTable != null) {
                if (lastTable.Parent() != null) {
                    fosterParent = (iText.StyledXmlParser.Jsoup.Nodes.Element)lastTable.Parent();
                    isLastTableParent = true;
                }
                else {
                    fosterParent = AboveOnStack(lastTable);
                }
            }
            else {
                // no table == frag
                fosterParent = stack[0];
            }
            if (isLastTableParent) {
                Validate.NotNull(lastTable);
                // last table cannot be null by this point.
                lastTable.Before(@in);
            }
            else {
                fosterParent.AppendChild(@in);
            }
        }
//\endcond

        public override String ToString() {
            return "TreeBuilder{" + "currentToken=" + currentToken + ", state=" + state + ", currentElement=" + CurrentElement
                () + '}';
        }

        protected internal override bool IsContentForTagData(String normalName) {
            return (normalName.Equals("script") || normalName.Equals("style"));
        }
    }
}
