using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using LTAF.Engine;
using System.IO;
using Moq;

namespace LTAF.UnitTests.UI
{
    [TestClass]
    public class HtmlElementTest
    {
        private Mock<IBrowserCommandExecutorFactory> _commandExecutorFactory;

        [TestInitialize]
        public void Initialize()
        {
            _commandExecutorFactory = new Mock<IBrowserCommandExecutorFactory>();
            ServiceLocator.BrowserCommandExecutorFactory = _commandExecutorFactory.Object;
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("http://test");
        }

        [TestMethod]
        public void CachedAttributesInheritedType()
        {
            string html = @"
                <html>
                    <input id=foo value=bar>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);
            HtmlInputElement input = (HtmlInputElement)element.ChildElements.Find("foo");
            UnitTestAssert.AreEqual("bar", input.CachedAttributes.Value);
        }

        [TestMethod]
        public void CachedAttributesBaseType()
        {
            string html = @"
                <html>
                    <input id=foo value=bar>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);
            UnitTestAssert.AreEqual("bar", element.ChildElements.Find("foo").CachedAttributes["value"]);

        }

        [TestMethod]
        public void InnerTextGet()
        {
            string html = @"
                <html>
                    <foo a=a>
                        <bar b=b />
                    </foo>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);
            UnitTestAssert.AreEqual(String.Empty, element.CachedInnerText);
        }

        [TestMethod]
        public void InnerTextGetWithSpacesBetweenTags()
        {
            string html = @"
                <html>
                    <foo a=a>
                        some text
                        <bar b=b />
                    </foo>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);
            UnitTestAssert.AreEqual("some text", element.ChildElements[0].CachedInnerText.Trim());
        }

        [TestMethod]
        public void InnerTextGetWithChildTagsBetweenTheText()
        {
            string html = @"
                <html>
                    text1
                    <foo a=a>
                        text2
                        <bar b=b />
                        text3
                    </foo>
                    text4
                </html>
                ";

            HtmlElement element = HtmlElement.Create(Utils.TrimSpaceBetweenTags(html));
            UnitTestAssert.AreEqual("text1text4", element.CachedInnerText);
            UnitTestAssert.AreEqual("text2text3", element.ChildElements[0].CachedInnerText);
        }

        [TestMethod]
        public void OuterHtmlGet()
        {
            string html = @"
                <html>
                    <foo a='a'>
                        <bar b='b'></bar>
                    </foo>
                </html>
                ";
            string expected = @"
                    <foo a='a'>
                        <bar b='b'></bar>
                    </foo>";

            HtmlElement element = HtmlElement.Create(html);
            UnitTestAssert.AreEqual(Utils.TrimSpaceBetweenTags(html), Utils.TrimSpaceBetweenTags(element.GetOuterHtml(false)));
            UnitTestAssert.AreEqual(Utils.TrimSpaceBetweenTags(expected), Utils.TrimSpaceBetweenTags(element.ChildElements[0].GetOuterHtml(false)));
            UnitTestAssert.AreEqual("<bar b='b'></bar>", element.ChildElements[0].ChildElements[0].GetOuterHtml(false).Trim());
        }

        [TestMethod]
        public void OuterHtmlWithTextWithSpaces()
        {
            string html = @"
                <html>
                    some text
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);
            UnitTestAssert.AreEqual(Utils.TrimSpaceBetweenTags(html), Utils.TrimSpaceBetweenTags(element.GetOuterHtml(false)));
        }

        [TestMethod]
        public void InnerTextWithSpaces()
        {
            string html = @"
                <html>
                    some text
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);
            UnitTestAssert.AreEqual("some text", element.CachedInnerText.Trim());

        }

        [TestMethod]
        public void OuterHtmlPrintsAllInnerTextBetweenTags()
        {
            string html = @"
                <html>
                    <foo a='a'>
                        text1
                        <bar b='b'></bar>
                        text2
                    </foo>
                </html>
                ";
            string expected = @"
                    <html>
                        <foo a='a'>
                            text1text2
                            <bar b='b'></bar>
                        </foo>
                    </html>";

            HtmlElement element = HtmlElement.Create(Utils.TrimSpaceBetweenTags(html));
            UnitTestAssert.AreEqual(Utils.TrimSpaceBetweenTags(expected), Utils.TrimSpaceBetweenTags(element.GetOuterHtml(false)));
        }

        [TestMethod]
        public void OuterHtmlWithScriptTags()
        {
            string html = @"
                <html>
                    <script type='javascript'>
                        //<foo>
                    </script>
                </html>
                ";
            HtmlElement element = HtmlElement.Create(html, new HtmlPage(), true);
            UnitTestAssert.AreEqual(Utils.TrimSpaceBetweenTags(html), Utils.TrimSpaceBetweenTags(element.GetOuterHtml(false)));
        }

        [TestMethod]
        public void BuildTargetLocator1()
        {
            string html = @"
                <html>
                    <table id='GridView1'>
                        <a />
                    </table>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html, new HtmlPage(), true);
            element.CanUseTagIndexToLocate = true;

            HtmlElement child = element.ChildElements[0].ChildElements[0];
            BrowserCommandTarget target = child.BuildBrowserCommandTarget();

            UnitTestAssert.AreEqual("GridView1", target.Id);
            UnitTestAssert.AreEqual("a", target.ChildTagName);
            UnitTestAssert.AreEqual(0, target.ChildTagIndex);
        }

        [TestMethod]
        public void BuildTargetLocator2()
        {
            string html = @"
                <html>
                    <table id='GridView1'>
                        <a />
                        <foo>
                            <a />
                            <a />
                        </foo>
                        <a />
                        <bar>
                            <a />
                        </bar>
                    </table>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html, new HtmlPage(), true);
            element.CanUseTagIndexToLocate = true;

            HtmlElement child = element.ChildElements[0].ChildElements[3].ChildElements[0];
            BrowserCommandTarget target = child.BuildBrowserCommandTarget();

            UnitTestAssert.AreEqual("GridView1", target.Id);
            UnitTestAssert.AreEqual("a", target.ChildTagName);
            UnitTestAssert.AreEqual(4, target.ChildTagIndex);
        }

        [TestMethod]
        public void ClickSendsBrowserCommand()
        {
            MockCommandExecutor commandExecutor = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExecutor);
            var testPage = new HtmlPage();
            HtmlElement element = new HtmlElement("input",
                new Dictionary<string, string>() { { "id", "button1" }, { "type", "button" } },
                null,
                testPage);

            element.Click();
            UnitTestAssert.AreEqual("Click", commandExecutor.ExecutedCommands[0].Description);
            UnitTestAssert.AreEqual(2, commandExecutor.ExecutedCommands[0].Handler.Arguments.Length);
            UnitTestAssert.AreEqual(false, (bool)(commandExecutor.ExecutedCommands[0].Handler.Arguments[0]));
        }

        [TestMethod]
        public void ClickWaitForPostbackNone()
        {
            MockCommandExecutor commandExecutor = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExecutor);
            var testPage = new HtmlPage();
            HtmlElement element = new HtmlElement("input",
                new Dictionary<string, string>() { { "id", "button1" }, { "type", "button" } },
                null,
                testPage);

            element.Click(WaitFor.None);
            UnitTestAssert.AreEqual("Click", commandExecutor.ExecutedCommands[0].Description);
            UnitTestAssert.AreEqual(2, commandExecutor.ExecutedCommands[0].Handler.Arguments.Length);

            UnitTestAssert.AreEqual(false, (bool)(commandExecutor.ExecutedCommands[0].Handler.Arguments[0]));
        }

        [TestMethod]
        public void ClickWaitForPostback()
        {
            MockCommandExecutor commandExecutor = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExecutor);
            var testPage = new HtmlPage();
            HtmlElement element = new HtmlElement("input",
                new Dictionary<string, string>() { { "id", "button1" }, { "type", "button" } },
                null,
                testPage);

            element.Click(WaitFor.Postback);
            UnitTestAssert.AreEqual("Click", commandExecutor.ExecutedCommands[0].Description);
            UnitTestAssert.AreEqual(2, commandExecutor.ExecutedCommands[0].Handler.Arguments.Length);

            UnitTestAssert.AreEqual(true, (bool)(commandExecutor.ExecutedCommands[0].Handler.Arguments[0]));
        }

        [TestMethod]
        public void Blur()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            var testPage = new HtmlPage();

            HtmlElement element = HtmlElement.Create("<input id='textbox1' type='textbox' />", testPage, false);
            element.Blur();

            UnitTestAssert.AreEqual("DispatchHtmlEvent", commandExec.ExecutedCommands[0].Handler.ClientFunctionName);
            UnitTestAssert.AreEqual("blur", (string)commandExec.ExecutedCommands[0].Handler.Arguments[0]);
        }

        [TestMethod]
        public void Focus()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            var testPage = new HtmlPage();

            HtmlElement element = HtmlElement.Create("<input id='textbox1' type='textbox' />", testPage, false);
            element.Focus();

            UnitTestAssert.AreEqual("DispatchHtmlEvent", commandExec.ExecutedCommands[0].Handler.ClientFunctionName);
            UnitTestAssert.AreEqual("focus", (string)commandExec.ExecutedCommands[0].Handler.Arguments[0]);
        }

        [TestMethod]
        public void MouseOver()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            var testPage = new HtmlPage();

            HtmlElement element = HtmlElement.Create("<input id='textbox1' type='textbox' />", testPage, false);
            element.MouseOver();

            UnitTestAssert.AreEqual("DispatchMouseEvent", commandExec.ExecutedCommands[0].Handler.ClientFunctionName);
            UnitTestAssert.AreEqual("mouseover", (string)commandExec.ExecutedCommands[0].Handler.Arguments[0]);
        }

        [TestMethod]
        public void GetInnerHtml()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            var testPage = new HtmlPage();

            HtmlElement element = HtmlElement.Create("<input id='textbox1' type='textbox' />", testPage, false);
            element.GetInnerHtml();

            UnitTestAssert.AreEqual("GetElementInnerHtml", commandExec.ExecutedCommands[0].Handler.ClientFunctionName);
        }

        [TestMethod]
        public void GetInnerText()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            var testPage = new HtmlPage();

            HtmlElement element = HtmlElement.Create("<input id='textbox1' type='textbox' />", testPage, false);
            element.GetInnerText();

            UnitTestAssert.AreEqual("GetElementInnerText", commandExec.ExecutedCommands[0].Handler.ClientFunctionName);
        }

        [TestMethod]
        public void GetInnerTextRecursive()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            var testPage = new HtmlPage();

            HtmlElement element = HtmlElement.Create("<input id='textbox1' type='textbox' />", testPage, false);
            element.GetInnerTextRecursively();

            UnitTestAssert.AreEqual("GetElementInnerTextRecursive", commandExec.ExecutedCommands[0].Handler.ClientFunctionName);
        }

        [TestMethod]
        public void GetOuterHtml()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            var testPage = new HtmlPage();

            HtmlElement element = HtmlElement.Create("<input id='textbox1' type='textbox' />", testPage, false);
            element.GetOuterHtml();

            UnitTestAssert.AreEqual("GetElementDom", commandExec.ExecutedCommands[0].Handler.ClientFunctionName);
        }

        [TestMethod]
        public void SetSelectedIndex()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            var testPage = new HtmlPage();

            HtmlSelectElement element = new HtmlSelectElement(
                new Dictionary<string, string>() { { "id", "button1" }, { "type", "textbox" } },
                null,
                testPage);

            element.SetSelectedIndex(1);

            UnitTestAssert.AreEqual("SetSelectBoxIndex", commandExec.ExecutedCommands[0].Handler.ClientFunctionName);
            UnitTestAssert.AreEqual(1, (int)commandExec.ExecutedCommands[0].Handler.Arguments[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(WebTestException))]
        public void SetSelectedIndex_NonSelectElement()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            var testPage = new HtmlPage();

            HtmlElement element = HtmlElement.Create("<input id='textbox1' type='textbox' />", testPage, false);
            element.SetSelectedIndex(1);
        }

        [TestMethod]
        public void SetText()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            var testPage = new HtmlPage();

            HtmlInputElement element = new HtmlInputElement(
                new Dictionary<string, string>() { { "id", "button1" }, { "type", "textbox" } },
                null,
                testPage);

            element.SetText("foo");

            UnitTestAssert.AreEqual("SetTextBox", commandExec.ExecutedCommands[0].Handler.ClientFunctionName);
            UnitTestAssert.AreEqual("foo", (string)commandExec.ExecutedCommands[0].Handler.Arguments[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(WebTestException))]
        public void SetText_NonInputElement()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            var testPage = new HtmlPage();

            HtmlElement element = HtmlElement.Create("<image id='textbox1' type='textbox' />", testPage, false);
            element.SetText("foo");
        }

        [TestMethod]
        public void WaitForAttributeValue()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            var testPage = new HtmlPage();

            HtmlElement element = HtmlElement.Create("<input id='textbox1' type='textbox' />", testPage, false);
            element.WaitForAttributeValue("foo", "bar", 10);

            UnitTestAssert.AreEqual("WaitForDomChange", commandExec.ExecutedCommands[0].Handler.ClientFunctionName);
            UnitTestAssert.AreEqual("foo", (string)commandExec.ExecutedCommands[0].Handler.Arguments[0]);
            UnitTestAssert.AreEqual("bar", (string)commandExec.ExecutedCommands[0].Handler.Arguments[1]);
            UnitTestAssert.AreEqual(10000, (int)commandExec.ExecutedCommands[0].Handler.Arguments[2]);
        }

        [TestMethod]
        public void WaitForInnerText()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            var testPage = new HtmlPage();

            HtmlElement element = HtmlElement.Create("<input id='textbox1' type='textbox' />", testPage, false);
            element.WaitForInnerText("foo", 10);

            UnitTestAssert.AreEqual("WaitForDomChange", commandExec.ExecutedCommands[0].Handler.ClientFunctionName);
            UnitTestAssert.AreEqual("innerHTML", (string)commandExec.ExecutedCommands[0].Handler.Arguments[0]);
            UnitTestAssert.AreEqual("foo", (string)commandExec.ExecutedCommands[0].Handler.Arguments[1]);
            UnitTestAssert.AreEqual(10000, (int)commandExec.ExecutedCommands[0].Handler.Arguments[2]);
        }

        [TestMethod]
        public void WaitUntilNotFound()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            var testPage = new HtmlPage();

            HtmlElement element = HtmlElement.Create("<foo id='foo'><bar>baz</bar></foo>", testPage, false);
            element.WaitUntilNotFound(10);

            UnitTestAssert.AreEqual("WaitUntilDissapears", commandExec.ExecutedCommands[0].Handler.ClientFunctionName);
            UnitTestAssert.AreEqual(10000, (int)commandExec.ExecutedCommands[0].Handler.Arguments[0]);
        }

        [TestMethod]
        public void IsVisible_True()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            commandExec.SetBrowserInfo(new BrowserInfo() { Data = "<input id='button1' type='button' />" });
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            var testPage = new HtmlPage();

            HtmlElement element = HtmlElement.Create("<input id='textbox1' type='textbox' />", testPage, false);
            UnitTestAssert.IsTrue(element.IsVisible());

            UnitTestAssert.AreEqual("GetElementAttributes", commandExec.ExecutedCommands[0].Handler.ClientFunctionName);
        }

        [TestMethod]
        public void IsVisible_False()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            commandExec.SetBrowserInfo(new BrowserInfo() { Data = "<input id='button1' type='button' style='visibility:hidden;' />" });
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            var testPage = new HtmlPage();

            HtmlElement element = HtmlElement.Create("<input id='textbox1' type='textbox' />", testPage, false);
            UnitTestAssert.IsFalse(element.IsVisible());

            UnitTestAssert.AreEqual("GetElementAttributes", commandExec.ExecutedCommands[0].Handler.ClientFunctionName);
        }

        [TestMethod]
        public void ClickWaitForAsyncPostback()
        {
            MockCommandExecutor commandExecutor = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExecutor);
            HtmlPage testPage = new HtmlPage();
            HtmlElement element = new HtmlElement("input",
                new Dictionary<string, string>() { { "id", "button1" }, { "type", "button" } },
                null,
                testPage);

            element.Click(WaitFor.AsyncPostback);
            UnitTestAssert.AreEqual("Click", commandExecutor.ExecutedCommands[0].Description);
            UnitTestAssert.AreEqual(2, commandExecutor.ExecutedCommands[0].Handler.Arguments.Length);
            UnitTestAssert.AreEqual(false, (bool)(commandExecutor.ExecutedCommands[0].Handler.Arguments[0]));

            UnitTestAssert.AreEqual("WaitForScript", commandExecutor.ExecutedCommands[1].Description);
        }

        [TestMethod]
        public void ClickPopupActionNone()
        {
            MockCommandExecutor commandExecutor = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExecutor);
            HtmlPage testPage = new HtmlPage();

            HtmlElement element = new HtmlElement("input",
                new Dictionary<string, string>() { { "id", "button1" }, { "type", "button" } },
                null,
                testPage);

            CommandParameters parameters = new CommandParameters();
            parameters.WaitFor = WaitFor.None;
            parameters.PopupAction = PopupAction.None;
            element.Click(parameters);
            UnitTestAssert.AreEqual("Click", commandExecutor.ExecutedCommands[0].Description);
            UnitTestAssert.AreEqual(2, commandExecutor.ExecutedCommands[0].Handler.Arguments.Length);
            UnitTestAssert.AreEqual(false, (bool)(commandExecutor.ExecutedCommands[0].Handler.Arguments[0]));
            UnitTestAssert.AreEqual(1, commandExecutor.ExecutedCommands.Length);

            UnitTestAssert.AreEqual(PopupAction.None, commandExecutor.ExecutedCommands[0].Handler.PopupAction);
        }

        [TestMethod]
        public void ClickPopupActionAlertOK()
        {
            MockCommandExecutor commandExecutor = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExecutor);
            var testPage = new HtmlPage();
            HtmlElement element = new HtmlElement("input",
                new Dictionary<string, string>() { { "id", "button1" }, { "type", "button" } },
                null,
                testPage);

            element.Click(new CommandParameters(WaitFor.None, PopupAction.AlertOK));
            UnitTestAssert.AreEqual("Click", commandExecutor.ExecutedCommands[0].Description);
            UnitTestAssert.AreEqual(2, commandExecutor.ExecutedCommands[0].Handler.Arguments.Length);
            UnitTestAssert.AreEqual(false, (bool)(commandExecutor.ExecutedCommands[0].Handler.Arguments[0]));
            UnitTestAssert.AreEqual(1, commandExecutor.ExecutedCommands.Length);

            UnitTestAssert.AreEqual(PopupAction.AlertOK, commandExecutor.ExecutedCommands[0].Handler.PopupAction);
        }

        [TestMethod]
        public void ClickPopupActionConfirmOK()
        {
            MockCommandExecutor commandExecutor = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExecutor);
            var testPage = new HtmlPage();
            HtmlElement element = new HtmlElement("input",
                new Dictionary<string, string>() { { "id", "button1" }, { "type", "button" } },
                null,
                testPage);

            element.Click(new CommandParameters(WaitFor.None, PopupAction.ConfirmOK));
            UnitTestAssert.AreEqual("Click", commandExecutor.ExecutedCommands[0].Description);
            UnitTestAssert.AreEqual(2, commandExecutor.ExecutedCommands[0].Handler.Arguments.Length);
            UnitTestAssert.AreEqual(false, (bool)(commandExecutor.ExecutedCommands[0].Handler.Arguments[0]));
            UnitTestAssert.AreEqual(1, commandExecutor.ExecutedCommands.Length);

            UnitTestAssert.AreEqual(PopupAction.ConfirmOK, commandExecutor.ExecutedCommands[0].Handler.PopupAction);
        }

        [TestMethod]
        public void ClickPopupActionConfirmCancel()
        {
            MockCommandExecutor commandExecutor = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExecutor);
            var testPage = new HtmlPage();
            HtmlElement element = new HtmlElement("input",
                new Dictionary<string, string>() { { "id", "button1" }, { "type", "button" } },
                null,
                testPage);

            element.Click(new CommandParameters(WaitFor.None, PopupAction.ConfirmCancel));
            UnitTestAssert.AreEqual("Click", commandExecutor.ExecutedCommands[0].Description);
            UnitTestAssert.AreEqual(2, commandExecutor.ExecutedCommands[0].Handler.Arguments.Length);
            UnitTestAssert.AreEqual(false, (bool)(commandExecutor.ExecutedCommands[0].Handler.Arguments[0]));
            UnitTestAssert.AreEqual(1, commandExecutor.ExecutedCommands.Length);

            UnitTestAssert.AreEqual(PopupAction.ConfirmCancel, commandExecutor.ExecutedCommands[0].Handler.PopupAction);
        }

        [TestMethod]
        public void ClickPopupActionGetTextBack()
        {
            MockCommandExecutor commandExecutor = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExecutor);
            var testPage = new HtmlPage();
            HtmlElement element = new HtmlElement("input",
                new Dictionary<string, string>() { { "id", "button1" }, { "type", "button" } },
                null,
                testPage);

            CommandParameters commandParameters = new CommandParameters(WaitFor.None, PopupAction.ConfirmOK);
            commandExecutor.SetBrowserInfo(new BrowserInfo() { Data = "This is the text from confirm" });

            element.Click(commandParameters);

            UnitTestAssert.AreEqual(PopupAction.ConfirmOK, commandExecutor.ExecutedCommands[0].Handler.PopupAction);
            UnitTestAssert.AreEqual("This is the text from confirm", commandParameters.PopupText);
        }

        [TestMethod]
        public void CreateElementCheckReferencesForParentPage()
        {
            //Arrange
            string html = @"
                <html>
                    <foo>
                        <bar />                    
                    </foo>
                </html>
                ";

            // Act
            HtmlPage testPage = new HtmlPage();
            HtmlElement element = HtmlElement.Create(html, testPage, false);

            // Assert
            Assert.AreEqual(testPage, element.ParentPage);
            Assert.AreEqual(testPage, element.ChildElements[0].ParentPage);
            Assert.AreEqual(testPage, element.ChildElements[0].ChildElements[0].ParentPage);
        }

        [TestMethod]
        public void FindParentForm_ReturnsFirstContainerForm()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                            <div>
                                <input id='input1' />                                            
                            </div>
                        </div>
                    </form>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html).ChildElements.Find("input1");

            //Act
            HtmlElement form = element.FindParentForm();

            // Assert
            Assert.IsInstanceOfType(form, typeof(HtmlFormElement));
        }

        [TestMethod]
        public void FindParentForm_ReturnsNullIfNoFormExists()
        {
            //Arrange
            string html = @"
                <html>
                    <form id='form1'>
                        <div>
                        </div>
                    </form>
                    <input id='input1' />                                            
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html).ChildElements.Find("input1");

            //Act
            HtmlElement form = element.FindParentForm();

            // Assert
            Assert.IsNull(form);
        }
    }
}
