using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniTestAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using System.IO;
using LTAF.Engine;
using System.Threading;
using System.Text.RegularExpressions;
using System.Resources;
using System.Reflection;
using Moq;

namespace LTAF.UnitTests.UI
{
    [TestClass]
    public class HtmlElementCollectionTest
    {
        private Mock<IBrowserCommandExecutorFactory> _commandExecutorFactory;

        [TestInitialize]
        public void FixtureSetup()
        {
            _commandExecutorFactory = new Mock<IBrowserCommandExecutorFactory>();
            ServiceLocator.BrowserCommandExecutorFactory = _commandExecutorFactory.Object;
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("http://test");
        }  

        [TestMethod]
        public void WhenCallingExistsById_IfElementExists_ShouldReturnTrue()
        {
            // Arrange
            string html = @"
                <html id='control1'>
                    <foo id='control2'> 
                        <bar id='control3' />
                    </foo>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);

            //Act, Assert
            Assert.IsTrue(element.ChildElements.Exists("control3"));
        }

        [TestMethod]
        public void WhenCallingExistsById_IfElementDoesNotExists_ShouldReturnFalse()
        {
            //Arrange
            string html = @"
                <html id='control1'>
                    <foo id='control2'> 
                        <bar id='control3' />
                    </foo>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);

            //Act,Assert
            Assert.IsFalse(element.ChildElements.Exists(new HtmlElementFindParams("control4"), 0));
        }

        [TestMethod]
        public void WhenCallingExistsByRegex_IfElementExists_ShouldReturnTrue()
        {
            //Arrange
            string html = @"
                <html id='control1'>
                    <foo id='ctl0_control2'> 
                        <bar id='ctl1_control2' />
                    </foo>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);

            //Act,Assert
            Assert.IsTrue(element.ChildElements.Exists("ctl1.*", MatchMethod.Regex));
        }

        [TestMethod]
        public void WhenCallingExistsByRegex_IfElementDoesNotExists_ShouldReturnFalse()
        {
            //Arrange
            string html = @"
                <html id='control1'>
                    <foo id='ctl0_control2'> 
                        <bar id='ctl1_control2' />
                    </foo>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);

            //Act,Assert
            Assert.IsFalse(element.ChildElements.Exists(new HtmlElementFindParams("ctl3.*", MatchMethod.Regex), 0));
        }

        [TestMethod]
        public void WhenCallingFindByIdEndsWith_ShouldReturnTheElement()
        {
            //Arrange
            string html = @"
                <html id='control1'>
                    <foo id='control2'> 
                        <bar id='ctl_control3' />
                        <invalid id='control3' />
                    </foo>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);

            //Act,Assert
            Assert.AreEqual("bar", element.ChildElements.Find("control3").TagName);
        }

        [TestMethod]
        public void WhenCallingFindByTag_ShouldReturnTheElement()
        {
            //Arrange
            string html = @"
                <html id='control1'>
                    <div id='control2'> 
                        <div id='control3' />
                    </div>
                    <div id='control4' />
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);

            //Act,Assert
            Assert.AreEqual("control3", element.ChildElements.Find("div", 1).Id);
            Assert.AreEqual("control4", element.ChildElements.Find("div", 2).Id);
        }

        [TestMethod]
        public void WhenCallingFindByIdEndsWithAndIndex_ShouldReturnTheElement()
        {
            //Arrange
            string html = @"
                <html>
                    <div id='control1'> 
                        <div id='control1' foo=bar />
                    </div>
                    <div id='control1' />
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);

            //Act,Assert
            HtmlElementFindParams findParams = new HtmlElementFindParams("control1");
            findParams.Index = 1;
            Assert.AreEqual("bar", element.ChildElements.Find(findParams).GetAttributes().Dictionary["foo"]);
        }

        [TestMethod]
        public void WhenCallingFindByTagAndIndex_ShouldReturnTheElement()
        {
            //Arrange
            string html = @"
                <html>
                    <div id='control1'> 
                        <div id='control1' foo='bar' />
                    </div>
                    <div id='control1' test='a' />
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);

            //Act,Assert
            HtmlElementFindParams findParams = new HtmlElementFindParams();
            findParams.TagName = "div";
            findParams.Index = 2;
            Assert.AreEqual("a", element.ChildElements.Find(findParams).GetAttributes().Dictionary["test"]);
        }

        [TestMethod]
        public void WhenCallingFindByInnerTextAndIndex_ShouldReturnTheElement()
        {
            string html = @"
                <html>
                    <div foo='a' id='control1' > 
                        <div id='control2' foo='b'>
                            some text
                        </div>
                    </div>
                    <div id='control3' foo='c'>
                        some text
                    </div>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);

            HtmlElementFindParams findParams = new HtmlElementFindParams();
            findParams.InnerText = "some text";
            findParams.Index = 1;
            Assert.AreEqual("control3", element.ChildElements.Find(findParams).Id);
        }

        [TestMethod]
        public void WhenCallingFindByTagInerTextAndIndex_ShouldReturnTheElement()
        {
            string html = @"
                <html>
                    <div foo='a' id='control1' > 
                        <span id='control2' foo='b'>
                            some text
                        </span>
                    </div>
                    <div id='control3' foo='c'>
                        some text
                    </div>
                    <span id='control4'>
                        some text
                    </span>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);

            HtmlElementFindParams findParams = new HtmlElementFindParams();
            findParams.TagName = "span";
            findParams.InnerText = "some text";
            findParams.Index = 1;
            Assert.AreEqual("control4", element.ChildElements.Find(findParams).Id);
        }

        [TestMethod]
        public void WhenCallingFindByTagInerTextAndIndex_IfMultipleSiblingsMatsh_ShouldReturnTheElement()
        {
            string html = @"
                <html>
                    <div foo=foo bar=bar> 
                        <span foo=foo bar=bar>text</span>
                        <div foo=foo bar=bar>text</div>
                        <span foo=foo />
                        <span bar=bar />
                        <span foo=foo bar=bar>wrong</span>
                        <span foo=foo bar=bar id='span1'>text</span>
                    </div>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);

            HtmlElementFindParams findParams = new HtmlElementFindParams();
            findParams.TagName = "span";
            findParams.InnerText = "text";
            findParams.Index = 1;
            Assert.AreEqual("span1", element.ChildElements.Find(findParams).Id);
        }

        [TestMethod]
        public void WhenCallingFind_IfElementDoestNotExist_ShouldThrowAnException()
        {
            // Arrange
            string html = @"
                <html id='control1'>
                    <foo id='control2'> 
                        <bar id='control3' />
                    </foo>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);
            HtmlElementFindParams p = new HtmlElementFindParams("control4");

            // Act/Assert
            ExceptionAssert.Throws<ElementNotFoundException>(
                () => element.ChildElements.Find(p, 0));
        }

        [TestMethod]
        public void WhenCallingFindByAllParameters_ShouldReturnElement()
        {
            string html = @"
                <html>
                    <div foo=foo bar=bar> 
                        <span foo=foo bar=bar>text</span>
                        <div foo=foo bar=bar>text</div>
                        <span foo=foo />
                        <span bar=bar />
                        <span foo=foo bar=bar>wrong</span>
                        <span foo=foo bar=bar id='span1'>text</span>
                    </div>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);

            HtmlElementFindParams findParams = new HtmlElementFindParams();
            findParams.Attributes.Add("foo", "foo");
            findParams.Attributes.Add("bar", "bar");
            findParams.TagName = "span";
            findParams.InnerText = "text";
            findParams.Index = 1;
            Assert.AreEqual("span1", element.ChildElements.Find(findParams).Id);
        }

        [TestMethod]
        public void WhenCallingFind_IfTagIsKnown_ShouldReturnAStrongTypedElement()
        {
            string html = @"
                <html id='control1'>
                    <input id='control2' type='button' /> 
                    <a id='control3' href='foo' />
                    <select id='control4' value='bar' />
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);
            Assert.AreEqual(HtmlInputElementType.Button, ((HtmlInputElement)element.ChildElements.Find("control2")).GetAttributes().Type);
            Assert.AreEqual("foo", ((HtmlAnchorElement)element.ChildElements.Find("control3")).GetAttributes().HRef);
            Assert.AreEqual("bar", ((HtmlSelectElement)element.ChildElements.Find("control4")).GetAttributes().Value);
        }

        [TestMethod]
        public void WhenCallingFindAllByTag_IfNoMatchesExist_ShouldReturnEmptyCollection()
        {
            string html = @"
                <html>
                    <div id='div1'> 
                    </div>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);

            Assert.AreEqual(0, element.ChildElements.FindAll(new HtmlElementFindParams("div2"), 0).Count);
        }

        [TestMethod]
        public void WhenCallingFindAllByTag_IfMatchingElementsAreNested_ShouldReturnCollectionWithAllOfThem()
        {
            string html = @"
                <html>
                    <div id='div1'> 
                        <div id='div2'>
                            <div id='div3'>
                            </div>
                        </div>
                    </div>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);

            ReadOnlyCollection<HtmlElement> divs = element.ChildElements.FindAll("div");

            Assert.AreEqual(3, divs.Count);
            Assert.AreEqual("div3", divs[2].Id);
        }

        [TestMethod]
        public void WhenCallingFindAllByTagAndInnerText_ShouldReturnMatchedElements()
        {
            string html = @"
                <html>
                    <div id='div1'> Zoo
                        <div id='div2'>Foo
                            <div id='div3'>Bar
                            </div>
                        </div>
                    </div>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);
            ReadOnlyCollection<HtmlElement> divs = element.ChildElements.FindAll("div", "Bar");
            Assert.AreEqual(1, divs.Count);
            Assert.AreEqual("div3", divs[0].Id);
        }

        [TestMethod]
        public void WhenCallingFindByIdLiteral_ShouldReturnMatchedElement()
        {
            string html = @"
                <html>
                    <div id='ctl0_foo' />
                    <span id='foo' />
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);
            Assert.AreEqual("span", element.ChildElements.Find("foo", MatchMethod.Literal).TagName);
        }

        [TestMethod]
        public void WhenCallingFindByIdRegex_ShouldReturnMatchedElement()
        {
            string html = @"
                <html>
                    <span id='foo' />
                    <div id='ctl0_foo' />
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);
            Assert.AreEqual("span", element.ChildElements.Find("^foo$", MatchMethod.Regex).TagName);
        }

        [TestMethod]
        public void WhenCallingFindByIdRegex_IfFullMatch_ShouldReturnMatchedElement()
        {
            string html = @"
                <html>
                    <div id='ctl0_foo' />
                    <span id='foo' />
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);
            Assert.AreEqual("div", element.ChildElements.Find("foo", MatchMethod.Regex).TagName);
        }

        [TestMethod]
        public void WhenCallingFindByIdRegex_IfMatchesTheBginning_ShouldReturnMatchedElement()
        {
            string html = @"
                <html>
                    <div id='ctl0_foo' />
                    <span id='foo' />
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);
            Assert.AreEqual("span", element.ChildElements.Find("^foo", MatchMethod.Regex).TagName);
        }

        [TestMethod]
        public void WhenCallingFindByIdRegex_IfMatchContainsWildCards_ShouldReturnMatchedElement()
        {
            string html = @"
                <html>
                    <div id='LoginView1'>
                        <span id='LoginView1_foo' /> 
                    </div>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);
            Assert.AreEqual("span", element.ChildElements.Find("LoginView1.*foo", MatchMethod.Regex).TagName);
        }

        [TestMethod]
        public void WhenCallingFindByIdRegex_IfMatchContainsWildCardsAndIdContainsAspNetAutoId_ShouldReturnMatchedElement()
        {
            string html = @"
                <html>
                    <div id='LoginView1'>
                        <span id='LoginView1_ctl03_GridView1_ctl01_foo' /> 
                    </div>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);
            Assert.AreEqual("span", element.ChildElements.Find("LoginView1.*foo", MatchMethod.Regex).TagName);
        }

        [TestMethod]
        public void WhenCallingFindByIdRegex_IfNoMatchFound_ShouldThrowError()
        {
            //Arrange
            string html = @"
                <html>
                </html>
                ";

            //Act,Assert
            HtmlElement element = HtmlElement.Create(html);
            HtmlElementFindParams findParams = new HtmlElementFindParams("foo.*bar", MatchMethod.Regex);

            ExceptionAssert.Throws<ElementNotFoundException>(
                () => element.ChildElements.Find(findParams, 0));
        }

        [TestMethod]
        public void WhenCallingFindByIdRegex_IfCaseDoNotMatch_ShouldReturnTheElement()
        {
            string html = @"
                <html>
                    <div id='LoginView1'>
                        <span id='LoginView1_ctl03_GridView1_ctl01_foo' /> 
                    </div>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);
            Assert.AreEqual("span", element.ChildElements.Find("LOGINVIEW1.*GRIDVIEW1.*FOO", MatchMethod.Regex).TagName);
        }

        [TestMethod]
        public void WhenCallingFindByIdRegexWithWildards_IfDoNotMatch_ShouldThrowAnError()
        {
            //Arrange
            string html = @"
                <html>
                    <div id='LoginView1'>
                        <span id='LoginView1_ctl03_GridView1_ctl01_foo' /> 
                    </div>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);

            //Act,Assert
            ExceptionAssert.Throws<ElementNotFoundException>(
                () => element.ChildElements.Find(new HtmlElementFindParams("LoginView1.*GridView1.*bar", MatchMethod.Regex), 0));
        }

        [TestMethod]
        public void WhenCallingFind_IfLoadingSourceFromLiveDotCom_ShouldLocateElements()
        {
            // live.com

            Assembly unitTestAssembly = Assembly.GetExecutingAssembly();

            using (StreamReader reader = new StreamReader(unitTestAssembly.GetManifestResourceStream("LTAF.UnitTests.UI.TestFiles.TextFile1.txt")))
            {
                string html = reader.ReadToEnd();
                HtmlElement element = HtmlElement.Create(html, null, true);
                Assert.AreEqual("Sign in", element.ChildElements.Find("ppToolBar").ChildElements.Find("signIn").CachedInnerText);
            }
        }

        [TestMethod]
        public void WhenCallingFind_IfLoadingSourceFromHotmailDotCom_ShouldLocateElements()
        {
            // Hotmail.com

            Assembly unitTestAssembly = Assembly.GetExecutingAssembly();
            using (StreamReader reader = new StreamReader(unitTestAssembly.GetManifestResourceStream("LTAF.UnitTests.UI.TestFiles.TextFile2.txt")))
            {
                string html = reader.ReadToEnd();
                HtmlElement element = HtmlElement.Create(html, null, true);
                Assert.AreEqual("fa:layout", element.ChildElements.Find("ContactEdit").TagName);

                HtmlElement name = element.ChildElements.Find("FirstLastName");
                Assert.AreEqual("Federico Silva Armas", name.CachedInnerText);
                Assert.AreEqual("BoldText", name.GetAttributes().Dictionary["class"]);

                Assert.AreEqual("Contacts", element.ChildElements.Find("SearchContacts").ChildElements.Find("span", 0).CachedInnerText);

                Assert.AreEqual("text", element.ChildElements.Find("FindBySubjectInput").GetAttributes().Dictionary["type"]);
            }
        }
       
        [TestMethod]
        public void WhenRefreshing_ShouldNotGetGetAttributes()
        {
            MockCommandExecutor commandExecutor = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExecutor);
            var testPage = new HtmlPage();

            string html = @"
                <html id='control1'>
                    <span id='MySpan' randomAttribute='foo'>
                        Span text
                    </span> 
                </html>
                ";

            commandExecutor.SetBrowserInfo(new LTAF.Engine.BrowserInfo()
            {
                Data = @"<span id='MySpan' randomAttribute='bar'>
                        Span text
                        </span>"
            });

            var element = HtmlElement.Create(html, testPage, false);
            var span = element.ChildElements.Find("MySpan");
            span.ChildElements.Refresh();
            Assert.IsNull(commandExecutor.ExecutedCommands[0].Handler.Arguments);
        }

        [TestMethod]
        public void WhenRefreshing_IfPassingOneAttribute_ShouldRefreshDomIncludingThatAttribute()
        {
            MockCommandExecutor commandExecutor = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExecutor);
            var testPage = new HtmlPage();

            string html = @"
                <html id='control1'>
                    <span id='MySpan' randomAttribute='foo'>
                        Span text
                    </span> 
                </html>
                ";

            commandExecutor.SetBrowserInfo(new LTAF.Engine.BrowserInfo()
            {
                Data = @"<span id='MySpan' randomAttribute='bar'>
                        Span text
                        </span>"
            });

            var element = HtmlElement.Create(html, testPage, false);
            var span = element.ChildElements.Find("MySpan");
            span.ChildElements.Refresh(new string[] { "randomAttribute" });
            Assert.AreEqual("randomattribute", commandExecutor.ExecutedCommands[0].Handler.Arguments[0]);
        }

        [TestMethod]
        public void WhenRefreshing_IfPassingTwoAttribute_ShouldRefreshDomIncludingThoseAttributes()
        {
            MockCommandExecutor commandExecutor = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExecutor);
            var testPage = new HtmlPage();

            string html = @"
                <html id='control1'>
                    <span id='MySpan' randomAttribute='foo'>
                        Span text
                    </span> 
                </html>
                ";

            commandExecutor.SetBrowserInfo(new LTAF.Engine.BrowserInfo()
            {
                Data = @"<span id='MySpan' randomAttribute='bar'>
                        Span text
                        </span>"
            });

            var element = HtmlElement.Create(html, testPage, false);
            var span = element.ChildElements.Find("MySpan");
            span.ChildElements.Refresh(new string[] { "randomAttribute", "anotherAttribute" });
            Assert.AreEqual("randomattribute-anotherattribute", commandExecutor.ExecutedCommands[0].Handler.Arguments[0]);
        }

        [TestMethod]
        public void WhenCallingFindAfter_ShouldNotSearchDescendants()
        {
            //Arrange
            string html = @"
                <html id='control1'>
                    <div id='CURRENT'>
                        <input id='target' type='button' /> 
                        <a id='control3' href='foo' />
                        <select id='TARGET' value='bar' />
                    </div>
                </html>
                ";

            HtmlElement root = HtmlElement.Create(html);
            var current = root.ChildElements.Find("CURRENT");

            //Act,Assert
            ExceptionAssert.ThrowsElementNotFound(
                () => root.ChildElements.FindAfter(current, new HtmlElementFindParams("TARGET")));
        }

        [TestMethod]
        public void WhenCallingFindAfter_ShouldNotReturnPriorSiblings()
        {
            //Arrange
            string html = @"
                <html id='control1'>
                    <input id='control2' type='button' /> 
                    <a id='control3' href='foo' />
                    <select id='control4' value='bar' />
                </html>
                ";

            HtmlElement root = HtmlElement.Create(html);
            var control3 = root.ChildElements.Find("control3");

            //Act,Assert
            ExceptionAssert.ThrowsElementNotFound(
                () => root.ChildElements.FindAfter(control3, new HtmlElementFindParams("control2")));
        }

        [TestMethod]
        public void WhenCallingFindAfter_ShouldReturnsFollowingSiblings()
        {
            string html = @"
                <html id='control1'>
                    <input id='control2' type='button' /> 
                    <a id='CURRENT' href='foo' />
                    <select id='control4' value='bar' />
                    <select id='control5' value='bar' />
                </html>
                ";

            HtmlElement root = HtmlElement.Create(html);
            var current = root.ChildElements.Find("CURRENT");

            Assert.AreEqual("control4", root.ChildElements.FindAfter(current, new HtmlElementFindParams("control4")).Id);
            Assert.AreEqual("control5", root.ChildElements.FindAfter(current, new HtmlElementFindParams("control5")).Id);
        }

        [TestMethod]
        public void WhenCallingFindAfter_IfMatchIsOutsideOFCollection_ShouldReturnError()
        {
            //Arrange
            string html = @"
                <html id='control1'>
                    <input id='control2' type='button' /> 
                    <span id='COLLECTION'>
                        <a id='CURRENT' href='foo' />
                    </span>
                    <span id='TARGET'/>
                    <select id='control4' value='bar' />
                    <select id='control5' value='bar' />
                </html>
                ";

            HtmlElement root = HtmlElement.Create(html);
            var collection = root.ChildElements.Find("COLLECTION").ChildElements;
            var current = root.ChildElements.Find("CURRENT");

            //Act,Assert
            ExceptionAssert.ThrowsElementNotFound(
                () => collection.FindAfter(current, "TARGET"));
        }

        [TestMethod]
        public void WhenCallingFindAfter_IfCannotFindElementAndRootIsThePage_ShouldThrowError()
        {
            //Arrange
            string html = @"
                <html id='control1'>
                    <input id='control2' type='button' /> 
                    <span id='something'>
                        <a id='CURRENT' href='foo' />
                    </span>
                    <span id='something'/>
                    <select id='control4' value='bar' />
                    <select id='control5' value='bar' />
                </html>
                ";

            HtmlElement root = HtmlElement.Create(html);
            var current = root.ChildElements.Find("CURRENT");

            HtmlElementCollection pageCollection = new HtmlElementCollection(root.ChildElements, root.ParentPage, null);

            //Act,Assert
            ExceptionAssert.ThrowsElementNotFound(
                () => pageCollection.FindAfter(current, "TARGET"));
        }

        [TestMethod]
        public void WhenCallingFindAfter_IfRootIsThePage_ShouldReturnElement()
        {
            string html = @"
                <html id='control1'>
                    <input id='control2' type='button' /> 
                    <span id='something'>
                        <a id='CURRENT' href='foo' />
                    </span>
                    <span id='TARGET'/>
                    <select id='control4' value='bar' />
                    <select id='control5' value='bar' />
                </html>
                ";

            HtmlElement root = HtmlElement.Create(html);
            var current = root.ChildElements.Find("CURRENT");

            HtmlElementCollection pageCollection = new HtmlElementCollection(root.ChildElements, root.ParentPage, null);

            Assert.AreEqual("TARGET", pageCollection.FindAfter(current, "TARGET").Id);
        }

        [TestMethod]
        public void WhenCallingFindAfter_ShouldReturnFollowingChildrenOfSiblings()
        {
            string html = @"
                <html id='control1'>
                    <input id='somecontrol0' type='button' /> 
                    <a id='current' href='foo' />
                    <span id='sibling'>
                        <span id='somecontrol1' />
                        <span id='somecontrol2' />
                    </span>
                    <span id='sibling2'>
                        <span id='somecontrol1' />
                        <span id='targetControl' />
                    </span>

                </html>
                ";

            HtmlElement root = HtmlElement.Create(html);
            var control3 = root.ChildElements.Find("current");
            Assert.AreEqual("targetControl", root.ChildElements.FindAfter(control3, new HtmlElementFindParams("targetControl")).Id);
        }

        [TestMethod]
        public void WhenCallingFindAfter_IfMatchIsPriorChildrenOfSiblings_ShouldThrowError()
        {
            //Arrange
            string html = @"
                <html id='control1'>
                    <input id='somecontrol0' type='button' /> 
                    <span id='sibling2'>
                        <span id='somecontrol1' />
                        <span id='targetControl' />
                    </span>
                    <a id='current' href='foo' />
                    <span id='sibling'>
                        <span id='somecontrol1' />
                        <span id='somecontrol2' />
                    </span>
                    <span id='sibling2'>
                        <span id='somecontrol1' />
                    </span>

                </html>
                ";

            HtmlElement root = HtmlElement.Create(html);
            var control3 = root.ChildElements.Find("current");

            //Act,Assert
            ExceptionAssert.ThrowsElementNotFound(
                () => root.ChildElements.FindAfter(control3, new HtmlElementFindParams("targetControl")));
        }

        [TestMethod]
        public void WhenCallingFindAfter_ShouldReturnChildrenOfAncestorsSiblings()
        {
            string html = @"
                <html id='control1'>
                    <input id='somecontrol0' type='button' /> 
                    <a id='somecontrol5' href='foo' />
                    <span id='TARGET' title='before'/>
                    <span id='priorSibling'>
                        <span id='somecontrol1' />
                        <span id='TARGET' title='before'/>
                    </span>
                    <span id='sibling'>
                        <span id='somecontrol1' />
                        <span id='CURRENT' />
                        <span id='somecontrol3' />
                    </span>
                    <span id='sibling2'>
                        <span id='somecontrol1' />
                        <span id='TARGET' title='after'/>
                    </span>

                </html>
                ";

            HtmlElement root = HtmlElement.Create(html);
            var control3 = root.ChildElements.Find("CURRENT");
            Assert.AreEqual("TARGET", root.ChildElements.FindAfter(control3, new HtmlElementFindParams("TARGET")).Id);
            Assert.AreEqual("after", root.ChildElements.FindAfter(control3, new HtmlElementFindParams("TARGET")).GetAttributes().Title);
        }

        [TestMethod]
        public void WhenCallingFindAfter_ShouldReturnSiblingsOfParent()
        {
            string html = @"
                <html id='control1'>
                    <span id='something'>
                        <span id='CURRENT' title='before'>NewLast</span>
                        <span id='somecontrol1' />
                    </span>
                    <INPUT id='ListView1_ctrl6_ctl03_DeleteButton'/>
                </html>
                ";

            HtmlElement root = HtmlElement.Create(html);
            var control3 = root.ChildElements.Find("span", "NewLast", 0);
            Assert.AreEqual("ListView1_ctrl6_ctl03_DeleteButton", root.ChildElements.FindAfter(control3, "DeleteButton").Id);
        }

        [TestMethod]
        public void WhenCallingFindAfter_IfParentOfCurrentMatches_ShouldNotBeReturned()
        {
            string html = @"
                <html id='control1'>
                    <span id='TARGET' title='before'>
                        <span id='somecontrol1' />
                        <span id='CURRENT' />
                        <span id='somecontrol3' />
                    </span>
                    <span id='sibling2'>
                        <span id='somecontrol1' />
                        <span id='TARGET' title='after'/>
                    </span>

                </html>
                ";

            HtmlElement root = HtmlElement.Create(html);
            var current = root.ChildElements.Find("CURRENT");
            Assert.AreEqual("TARGET", root.ChildElements.FindAfter(current, new HtmlElementFindParams("TARGET")).Id);
            Assert.AreEqual("after", root.ChildElements.FindAfter(current, new HtmlElementFindParams("TARGET")).GetAttributes().Title);
        }

        [TestMethod]
        public void WhenCallingFindAfter_IfPrecedingElementIsAfterTheCollection_ShouldThrowError()
        {
            //Arrange
            string html = @"
                <html id='control1'>
                    <span id='TARGET' />
                    <span id='ParentOfCollection'>
                        <span id='currentCollection'>
                            <span id='somecontrol1' />
                            <span id='someOtherControl' />
                        </span>
                    </span>
                    <span id='anotherCollection'>
                        <span id='CURRENT' />
                        <span id='Somecontrol' title='before'>
                            <span id='somecontrol1' />
                            <span id='TARGET' />
                            <span id='somecontrol3' />
                        </span>
                    </span>
                </html>
                ";

            HtmlElement root = HtmlElement.Create(html);
            var currentCollection = root.ChildElements.Find("ParentOfCollection").ChildElements;
            var currentElement = root.ChildElements.Find("CURRENT");

            //Act,Assert
            ExceptionAssert.Throws <ArgumentException>(
                () => currentCollection.FindAfter(currentElement, new HtmlElementFindParams("TARGET")));
        }

        [TestMethod]
        public void WhenCallingFindAfter_IfPrecedingElementIsBeforeTheCollection_ShouldThrowError()
        {
            //Arrange
            string html = @"
                <html id='control1'>
                    <span id='TARGET' />
                    <span id='CURRENT' />
                    <span id='ParentOfCollection'>
                        <span id='currentCollection'>
                            <span id='somecontrol1' />
                            <span id='someOtherControl' />
                        </span>
                    </span>
                    <span id='anotherCollection'>
                        <span id='Somecontrol' title='before'>
                            <span id='somecontrol1' />
                            <span id='TARGET' />
                            <span id='somecontrol3' />
                        </span>
                    </span>
                </html>
                ";

            HtmlElement root = HtmlElement.Create(html);
            var currentCollection = root.ChildElements.Find("ParentOfCollection").ChildElements;
            var currentElement = root.ChildElements.Find("CURRENT");

            //Act,Assert
            ExceptionAssert.Throws<ArgumentException>(
                () => currentCollection.FindAfter(currentElement, new HtmlElementFindParams("TARGET")));
        }

        [TestMethod]
        public void WhenFindingByAttributes_IfMatchingAttributesAsLiteral_ShouldReturnElement()
        {
            string html = @"
				<html>
					<body>
						<input id='foo' class='style1' />
					</body>
				</html>
				";

            HtmlElement root = HtmlElement.Create(html);

            HtmlElementFindParams args = new HtmlElementFindParams();
            args.Attributes.Add("class", "style1");

            Assert.AreEqual("foo", root.ChildElements.Find(args, 0).Id);
        }

        [TestMethod]
        public void WhenCallingExistsByAttributes_IfAttributesDoNotMatchAsLiterals_ShouldReturnFalse()
        {
            string html = @"
				<html>
					<body>
						<input id='foo' class='style1' />
					</body>
				</html>
				";

            HtmlElement root = HtmlElement.Create(html);

            HtmlElementFindParams args = new HtmlElementFindParams();
            args.Attributes.Add("class", "style2");

            Assert.IsFalse(root.ChildElements.Exists(args, 0));
        }

        [TestMethod]
        public void WhenCallingExistsByAttributes_IfAttributesMatchWithContains_ShouldReturnTrue()
        {
            string html = @"
				<html>
					<body>
						<input id='foo' class='style1 style2 style3' />
					</body>
				</html>
				";

            HtmlElement root = HtmlElement.Create(html);

            HtmlElementFindParams args = new HtmlElementFindParams();
            args.Attributes.Add("class", "style2", MatchMethod.Contains);

            Assert.IsTrue(root.ChildElements.Exists(args, 0));
        }

        [TestMethod]
        public void WhenCallingExistsByAttributes_IfAttributesNotMatchWithContains_ShouldReturnFalse()
        {
            string html = @"
				<html>
					<body>
						<input id='foo' class='style1 style2 style3' />
					</body>
				</html>
				";

            HtmlElement root = HtmlElement.Create(html);

            HtmlElementFindParams args = new HtmlElementFindParams();
            args.Attributes.Add("class", "style4", MatchMethod.Contains);

            Assert.IsFalse(root.ChildElements.Exists(args, 0));
        }

        [TestMethod]
        public void WhenCallingExistsByAttributes_IfAttributesMatchWithEndsWith_ShouldReturnTrue()
        {
            string html = @"
				<html>
					<body>
						<input id='foo' class='style1 style2 style3' />
					</body>
				</html>
				";

            HtmlElement root = HtmlElement.Create(html);

            HtmlElementFindParams args = new HtmlElementFindParams();
            args.Attributes.Add("class", "style3", MatchMethod.EndsWith);

            Assert.IsTrue(root.ChildElements.Exists(args, 0));
        }

        [TestMethod]
        public void WhenCallingExistsByAttributes_IfAttributeNotMatchWithEndsWith_ShouldReturnFalse()
        {
            string html = @"
				<html>
					<body>
						<input id='foo' class='style1 style2 style3' />
					</body>
				</html>
				";

            HtmlElement root = HtmlElement.Create(html);

            HtmlElementFindParams args = new HtmlElementFindParams();
            args.Attributes.Add("class", "style1", MatchMethod.EndsWith);

            Assert.IsFalse(root.ChildElements.Exists(args, 0));
        }

        [TestMethod]
        public void WhenCallingExistsByAttributes_IfAllAttributesMatch_ShouldReturnTrue()
        {
            string html = @"
				<html>
					<body>
						<input id='foo' class='style1 style2 style3' style='height:30px;width:20px' />
					</body>
				</html>
				";

            HtmlElement root = HtmlElement.Create(html);

            HtmlElementFindParams args = new HtmlElementFindParams();
            args.Attributes.Add("class", "style2", MatchMethod.Contains);
            args.Attributes.Add("style", "width:20px", MatchMethod.Regex);

            Assert.IsTrue(root.ChildElements.Exists(args, 0));
        }

        [TestMethod]
        public void WhenCallingExistsByAttributes_IfAllAttributesNotMatch_ShouldReturnFalse()
        {
            string html = @"
				<html>
					<body>
						<input id='foo' class='style1 style2 style3' style='height:30px;width:20px' />
					</body>
				</html>
				";

            HtmlElement root = HtmlElement.Create(html);

            HtmlElementFindParams args = new HtmlElementFindParams();
            args.Attributes.Add("class", "style2", MatchMethod.Contains);
            args.Attributes.Add("style", "width:10px", MatchMethod.Regex);

            Assert.IsFalse(root.ChildElements.Exists(args, 0));
        }

        [TestMethod]
        public void WhenFindingByAttributes_IfTimeoutToFindIsZero_ShouldNotRefreshTheCollection()
        {
            //Arrange
            string html = @"
				<html>
					<body>
                        <input id='input1' type='textbox' />
						<input id='input2' type='submit' />
					</body>
				</html>
				";

            var browserCommandExecutor = new Mock<IBrowserCommandExecutor>();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(browserCommandExecutor.Object);

            HtmlPage page = new HtmlPage(new Uri("http://foo"));
            HtmlElement root = HtmlElement.Create(html, page, true);

            root.ChildElements.Find(new { @type = "submit" });
        }
    }
}