/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using iText.Commons.Utils;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Select {
    [NUnit.Framework.Category("UnitTest")]
    public class MultiLocaleTest : ExtendedITextTest {
        private readonly CultureInfo defaultLocale = System.Threading.Thread.CurrentThread.CurrentUICulture;

        public static ICollection<CultureInfo> Locales() {
            return JavaUtil.ArraysAsList(System.Globalization.CultureInfo.InvariantCulture, new CultureInfo("tr", false
                ));
        }

        [NUnit.Framework.TearDown]
        public virtual void SetDefaultLocale() {
            System.Threading.Thread.CurrentThread.CurrentUICulture = defaultLocale;
        }

        [NUnit.Framework.TestCaseSource("Locales")]
        public virtual void TestByAttribute(CultureInfo locale) {
            System.Threading.Thread.CurrentThread.CurrentUICulture = locale;
            String h = "<div Title=Foo /><div Title=Bar /><div Style=Qux /><div title=Balim /><div title=SLIM />" + "<div data-name='with spaces'/>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            Elements withTitle = doc.Select("[title]");
            NUnit.Framework.Assert.AreEqual(4, withTitle.Count);
            Elements foo = doc.Select("[TITLE=foo]");
            NUnit.Framework.Assert.AreEqual(1, foo.Count);
            Elements foo2 = doc.Select("[title=\"foo\"]");
            NUnit.Framework.Assert.AreEqual(1, foo2.Count);
            Elements foo3 = doc.Select("[title=\"Foo\"]");
            NUnit.Framework.Assert.AreEqual(1, foo3.Count);
            Elements dataName = doc.Select("[data-name=\"with spaces\"]");
            NUnit.Framework.Assert.AreEqual(1, dataName.Count);
            NUnit.Framework.Assert.AreEqual("with spaces", dataName.First().Attr("data-name"));
            Elements not = doc.Select("div[title!=bar]");
            NUnit.Framework.Assert.AreEqual(5, not.Count);
            NUnit.Framework.Assert.AreEqual("Foo", not.First().Attr("title"));
            Elements starts = doc.Select("[title^=ba]");
            NUnit.Framework.Assert.AreEqual(2, starts.Count);
            NUnit.Framework.Assert.AreEqual("Bar", starts.First().Attr("title"));
            NUnit.Framework.Assert.AreEqual("Balim", starts.Last().Attr("title"));
            Elements ends = doc.Select("[title$=im]");
            NUnit.Framework.Assert.AreEqual(2, ends.Count);
            NUnit.Framework.Assert.AreEqual("Balim", ends.First().Attr("title"));
            NUnit.Framework.Assert.AreEqual("SLIM", ends.Last().Attr("title"));
            Elements contains = doc.Select("[title*=i]");
            NUnit.Framework.Assert.AreEqual(2, contains.Count);
            NUnit.Framework.Assert.AreEqual("Balim", contains.First().Attr("title"));
            NUnit.Framework.Assert.AreEqual("SLIM", contains.Last().Attr("title"));
        }

        [NUnit.Framework.TestCaseSource("Locales")]
        public virtual void TestPseudoContains(CultureInfo locale) {
            System.Threading.Thread.CurrentThread.CurrentUICulture = locale;
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>The Rain.</p> <p class=light>The <i>RAIN</i>.</p> <p>Rain, the.</p></div>"
                );
            Elements ps1 = doc.Select("p:contains(Rain)");
            NUnit.Framework.Assert.AreEqual(3, ps1.Count);
            Elements ps2 = doc.Select("p:contains(the rain)");
            NUnit.Framework.Assert.AreEqual(2, ps2.Count);
            NUnit.Framework.Assert.AreEqual("The Rain.", ps2.First().Html());
            NUnit.Framework.Assert.AreEqual("The <i>RAIN</i>.", ps2.Last().Html());
            Elements ps3 = doc.Select("p:contains(the Rain):has(i)");
            NUnit.Framework.Assert.AreEqual(1, ps3.Count);
            NUnit.Framework.Assert.AreEqual("light", ps3.First().ClassName());
            Elements ps4 = doc.Select(".light:contains(rain)");
            NUnit.Framework.Assert.AreEqual(1, ps4.Count);
            NUnit.Framework.Assert.AreEqual("light", ps3.First().ClassName());
            Elements ps5 = doc.Select(":contains(rain)");
            NUnit.Framework.Assert.AreEqual(8, ps5.Count);
            // html, body, div,...
            Elements ps6 = doc.Select(":contains(RAIN)");
            NUnit.Framework.Assert.AreEqual(8, ps6.Count);
        }

        [NUnit.Framework.TestCaseSource("Locales")]
        public virtual void ContainsOwn(CultureInfo locale) {
            System.Threading.Thread.CurrentThread.CurrentUICulture = locale;
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p id=1>Hello <b>there</b> igor</p>");
            Elements ps = doc.Select("p:containsOwn(Hello IGOR)");
            NUnit.Framework.Assert.AreEqual(1, ps.Count);
            NUnit.Framework.Assert.AreEqual("1", ps.First().Id());
            NUnit.Framework.Assert.AreEqual(0, doc.Select("p:containsOwn(there)").Count);
            Document doc2 = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>Hello <b>there</b> IGOR</p>");
            NUnit.Framework.Assert.AreEqual(1, doc2.Select("p:containsOwn(igor)").Count);
        }

        [NUnit.Framework.TestCaseSource("Locales")]
        public virtual void ContainsData(CultureInfo locale) {
            System.Threading.Thread.CurrentThread.CurrentUICulture = locale;
            String html = "<p>function</p><script>FUNCTION</script><style>item</style><span><!-- comments --></span>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element body = doc.Body();
            Elements dataEls1 = body.Select(":containsData(function)");
            Elements dataEls2 = body.Select("script:containsData(function)");
            Elements dataEls3 = body.Select("span:containsData(comments)");
            Elements dataEls4 = body.Select(":containsData(o)");
            Elements dataEls5 = body.Select("style:containsData(ITEM)");
            NUnit.Framework.Assert.AreEqual(2, dataEls1.Count);
            // body and script
            NUnit.Framework.Assert.AreEqual(1, dataEls2.Count);
            NUnit.Framework.Assert.AreEqual(dataEls1.Last(), dataEls2.First());
            NUnit.Framework.Assert.AreEqual("<script>FUNCTION</script>", dataEls2.OuterHtml());
            NUnit.Framework.Assert.AreEqual(1, dataEls3.Count);
            NUnit.Framework.Assert.AreEqual("span", dataEls3.First().TagName());
            NUnit.Framework.Assert.AreEqual(3, dataEls4.Count);
            NUnit.Framework.Assert.AreEqual("body", dataEls4.First().TagName());
            NUnit.Framework.Assert.AreEqual("script", dataEls4[1].TagName());
            NUnit.Framework.Assert.AreEqual("span", dataEls4[2].TagName());
            NUnit.Framework.Assert.AreEqual(1, dataEls5.Count);
        }

        [NUnit.Framework.TestCaseSource("Locales")]
        public virtual void TestByAttributeStarting(CultureInfo locale) {
            System.Threading.Thread.CurrentThread.CurrentUICulture = locale;
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1 ATTRIBUTE data-name=jsoup>Hello</div><p data-val=5 id=2>There</p><p id=3>No</p>"
                );
            Elements withData = doc.Select("[^data-]");
            NUnit.Framework.Assert.AreEqual(2, withData.Count);
            NUnit.Framework.Assert.AreEqual("1", withData.First().Id());
            NUnit.Framework.Assert.AreEqual("2", withData.Last().Id());
            withData = doc.Select("p[^data-]");
            NUnit.Framework.Assert.AreEqual(1, withData.Count);
            NUnit.Framework.Assert.AreEqual("2", withData.First().Id());
            NUnit.Framework.Assert.AreEqual(1, doc.Select("[^attrib]").Count);
        }
    }
}
