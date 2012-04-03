using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LTAF.Engine;
using UnitTestAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Moq;
using LTAF.Emulators;

namespace LTAF.UnitTests
{
    [TestClass]
    public class HtmlPageTest
    {
        private int _newWindowRefreshCount = 0;
        private Mock<IBrowserCommandExecutorFactory> _commandExecutorFactory;

        [TestInitialize]
        public void Initialize()
        {
            _commandExecutorFactory = new Mock<IBrowserCommandExecutorFactory>();
            ServiceLocator.BrowserCommandExecutorFactory = _commandExecutorFactory.Object;
        }

        [TestCleanup]
        public void Cleanup()
        {
            ServiceLocator.WebResourceReader = null;
        }

        [TestMethod]
        public void IsServerError_IfPageHasNoError_ShouldReturnFalse()
        {
            //arrange
            var commandExecutor = new Mock<IBrowserCommandExecutor>();
            commandExecutor.Setup(m => m.ExecuteCommand(It.IsAny<int>(), null, It.IsAny<BrowserCommand>(), It.IsAny<int>()))
                .Returns(new BrowserInfo() { Data = "<html><body><h1>Success Page</h1></body></html>"  });
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExecutor.Object);

            MockResourceReader resourceReader = new MockResourceReader();
            resourceReader.SetResourceString("System.Web", "System.Web", "Error_Formatter_ASPNET_Error", "Server Error in '{0}' Application.");
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("http://test");
            ServiceLocator.WebResourceReader = resourceReader;

            //act
            var testPage = new HtmlPage("somePage");

            //assert
            Assert.IsFalse(testPage.IsServerError());

        }

        [TestMethod]
        public void IsServerError_IfPageHasError_ReturnsTrue()
        {
            //arrange
            var commandExecutor = new Mock<IBrowserCommandExecutor>();
            commandExecutor.Setup(m => m.ExecuteCommand(It.IsAny<int>(), null, It.IsAny<BrowserCommand>(), It.IsAny<int>()))
                .Returns(new BrowserInfo() { Data = "<html><body><h1>Server Error in '/SampleWebSite' Application.</h1></body></html>" });
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExecutor.Object);

            MockResourceReader resourceReader = new MockResourceReader();
            resourceReader.SetResourceString("System.Web", "System.Web", "Error_Formatter_ASPNET_Error", "Server Error in '{0}' Application.");
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("http://test");
            ServiceLocator.WebResourceReader = resourceReader;

            //act
            var testPage = new HtmlPage("somePage");

            //assert
            Assert.IsTrue(testPage.IsServerError());
        }

        [TestMethod]
        public void ExecuteCommand_ShouldSendsTracesOnlyForNextCommand()
        {
            //arrange
            MockCommandExecutor commandExec = new MockCommandExecutor();
            commandExec.SetBrowserInfo(new BrowserInfo() { Data = "<html><body>Hello</body></html>" });
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);

            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("http://test");
            var testPage = new HtmlPage();

            //act
            WebTestConsole.Clear();
            WebTestConsole.Write("foo");
            WebTestConsole.Write("bar");

            testPage.Elements.Refresh();

            //assert
            UnitTestAssert.AreEqual("GetPageDom", commandExec.ExecutedCommands[0].Handler.ClientFunctionName);
            UnitTestAssert.AreEqual(2, commandExec.ExecutedCommands[0].Traces.Length);
            UnitTestAssert.AreEqual("foo", commandExec.ExecutedCommands[0].Traces[0]);
            UnitTestAssert.AreEqual("bar", commandExec.ExecutedCommands[0].Traces[1]);

            //act
            testPage.Elements.Refresh();

            //assert
            UnitTestAssert.AreEqual(0, commandExec.ExecutedCommands[1].Traces.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(WebTestException))]
        public void GetFrame_ThrowsIfFailsToRefresh()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            _newWindowRefreshCount = 0;
            commandExec.ExecutingCommand += new EventHandler(commandExec_ExecutingCommandThrows);

            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("http://test");
            var testPage = new HtmlPage();

            testPage.GetFramePage(new string[] { "foo" }, 0);

        }

        [TestMethod]
        [ExpectedException(typeof(WebTestException))]
        public void GetPopupWindow_ThrowsIfFailsToRefresh()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            _newWindowRefreshCount = 0;
            commandExec.ExecutingCommand += new EventHandler(commandExec_ExecutingCommandThrows);

            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("http://test");
            var testPage = new HtmlPage();

            testPage.GetPopupPage(1, 0);

        }

        [TestMethod]
        public void GetFrame_KeepsTryingUntilRefreshSucceeds()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            commandExec.SetBrowserInfo(new BrowserInfo() { Data = "<html><body>Hello</body></html>" });
            _newWindowRefreshCount = 0;
            commandExec.ExecutingCommand += new EventHandler(commandExec_ExecutingCommandThrows);

            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("http://test");
            var testPage = new HtmlPage();

            HtmlPage framePage = testPage.GetFramePage(new string[] { "foo" }, timeoutInSeconds: 1, waitBetweenAttemptsInMilliseconds: 0);

            UnitTestAssert.IsNotNull(framePage);
            Assert.AreEqual(_newWindowRefreshCount, 2);

        }

        [TestMethod]
        public void GetPopupWindow_KeepsTryingUntilRefreshSucceeds()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            commandExec.SetBrowserInfo(new BrowserInfo() { Data = "<html><body>Hello</body></html>" });
            _newWindowRefreshCount = 0;
            commandExec.ExecutingCommand += new EventHandler(commandExec_ExecutingCommandThrows);

            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("http://test");
            var testPage = new HtmlPage();

            HtmlPage popupPage = testPage.GetPopupPage(1, timeoutInSeconds: 1, waitBetweenAttemptsInMilliseconds:0);

            UnitTestAssert.IsNotNull(popupPage);
            Assert.AreEqual(_newWindowRefreshCount, 2);

        }

        void commandExec_ExecutingCommandThrows(object sender, EventArgs e)
        {
            _newWindowRefreshCount++;
            if (_newWindowRefreshCount == 1)
            {
                throw new Exception("Exception");
            }
        }

        [TestMethod]
        public void GetPopupWindow_SendsARefreshCommandWithTheCorrectIndex()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            commandExec.SetBrowserInfo(new BrowserInfo() { Data = "<html><body>Hello</body></html>" });
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("http://test");
            var testPage = new HtmlPage();

            HtmlPage popupPage = testPage.GetPopupPage(1);

            UnitTestAssert.IsNotNull(popupPage);
            UnitTestAssert.AreEqual("GetPageDom", commandExec.ExecutedCommands[0].Handler.ClientFunctionName);
            UnitTestAssert.AreEqual("GetPageDom (Popup window: 1)", commandExec.ExecutedCommands[0].Description);
            UnitTestAssert.AreEqual(1, commandExec.ExecutedCommands[0].Target.WindowIndex);
        }

        [TestMethod]
        public void GetPopupWindow_ActionsFromPopupGetSentWithCorrectWindowIndex()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            commandExec.SetBrowserInfo(new BrowserInfo() { Data = "<html><body><input id='Button1' /></body></html>" });
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("http://test");
            var testPage = new HtmlPage();

            HtmlPage popupPage = testPage.GetPopupPage(1);

            popupPage.Elements.Find("Button1").Click();

            UnitTestAssert.AreEqual("ClickElement", commandExec.ExecutedCommands[1].Handler.ClientFunctionName);
            UnitTestAssert.AreEqual("Click (Popup window: 1)", commandExec.ExecutedCommands[1].Description);
            UnitTestAssert.AreEqual(1, commandExec.ExecutedCommands[1].Target.WindowIndex);
        }

        [TestMethod]
        public void GetFrame_ActionsFromFrameGetSentWithCorrectFrameHierarchy()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            commandExec.SetBrowserInfo(new BrowserInfo() { Data = "<html><body><input id='Button1' /></body></html>" });
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("http://test");
            var testPage = new HtmlPage();

            HtmlPage framePage = testPage.GetFramePage("FrameName");

            framePage.Elements.Find("Button1").Click();

            UnitTestAssert.AreEqual("ClickElement", commandExec.ExecutedCommands[1].Handler.ClientFunctionName);
            UnitTestAssert.AreEqual("Click (Frame: FrameName)", commandExec.ExecutedCommands[1].Description);
            UnitTestAssert.AreEqual("FrameName", (string)commandExec.ExecutedCommands[1].Target.FrameHierarchy[0]);
        }

        [TestMethod]
        public void GetFrame_NestedFrames()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            commandExec.SetBrowserInfo(new BrowserInfo() { Data = "<html><body><input id='Button1' /></body></html>" });
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("http://test");
            var testPage = new HtmlPage();

            HtmlPage framePage = testPage.GetFramePage("frame1", "frame2", "frame3");

            framePage.Elements.Find("Button1").Click();
            UnitTestAssert.AreEqual("Click (Frame: frame1-frame2-frame3)", commandExec.ExecutedCommands[1].Description);
            UnitTestAssert.AreEqual("frame1", (string)commandExec.ExecutedCommands[1].Target.FrameHierarchy[0]);
            UnitTestAssert.AreEqual("frame2", (string)commandExec.ExecutedCommands[1].Target.FrameHierarchy[1]);
            UnitTestAssert.AreEqual("frame3", (string)commandExec.ExecutedCommands[1].Target.FrameHierarchy[2]);
        }

        [TestMethod]
        public void GetFrame_MultipleCalls()
        {
            MockCommandExecutor commandExec = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExec);
            commandExec.SetBrowserInfo(new BrowserInfo() { Data = "<html><body><input id='Button1' /></body></html>" });
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("http://test");
            var testPage = new HtmlPage();

            HtmlPage frame1Page = testPage.GetFramePage("frame1");
            HtmlPage frame2Page = frame1Page.GetFramePage("frame2");


            frame2Page.Elements.Find("Button1").Click();
            UnitTestAssert.AreEqual("Click (Frame: frame1-frame2)", commandExec.ExecutedCommands[2].Description);
            UnitTestAssert.AreEqual("frame1", (string)commandExec.ExecutedCommands[1].Target.FrameHierarchy[0]);
            UnitTestAssert.AreEqual("frame2", (string)commandExec.ExecutedCommands[1].Target.FrameHierarchy[1]);

        }

        [TestMethod]
        public void ResolveNavigateUrlInCassiniWithEmptyString()
        {
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("/");
            HtmlPage page = new HtmlPage();
            UnitTestAssert.AreEqual("/", page.ResolveNavigateUrl(""));
        }

        [TestMethod]
        public void ResolveNavigateUrlInCassiniToRoot()
        {
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("/");
            HtmlPage page = new HtmlPage();
            UnitTestAssert.AreEqual("/", page.ResolveNavigateUrl("/"));
        }

        [TestMethod]
        public void ResolveNavigateUrlInCassiniNoLeadingSlash()
        {
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("/");
            HtmlPage page = new HtmlPage();
            UnitTestAssert.AreEqual("/bar/foo/char", page.ResolveNavigateUrl("bar/foo/char"));
        }

        [TestMethod]
        public void ResolveNavigateUrlInCassini()
        {
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("/");
            HtmlPage page = new HtmlPage();
            UnitTestAssert.AreEqual("/bar", page.ResolveNavigateUrl("/bar"));
        }

        [TestMethod]
        public void ResolveNavigateUrlToRootWhenEmpty()
        {
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("/foo");
            HtmlPage page = new HtmlPage();
            UnitTestAssert.AreEqual("/foo", page.ResolveNavigateUrl(""));
        }

        [TestMethod]
        public void ResolveNavigateUrlToRoot()
        {
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("/foo");
            HtmlPage page = new HtmlPage();
            UnitTestAssert.AreEqual("/foo", page.ResolveNavigateUrl("/"));
        }

        [TestMethod]
        public void ResolveNavigateUrlWithAbsoluteVale()
        {
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("/foo");
            HtmlPage page = new HtmlPage();
            UnitTestAssert.AreEqual("http://bar", page.ResolveNavigateUrl("http://bar"));
        }

        [TestMethod]
        public void ResolveNavigateUrlAppendsUrlToApplicationPath()
        {
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("/foo");
            HtmlPage page = new HtmlPage();
            UnitTestAssert.AreEqual("/foo/bar", page.ResolveNavigateUrl("bar"));
        }

        [TestMethod]
        public void ResolveNavigateUrlDoesntAddExtraSlash()
        {
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("/foo");
            HtmlPage page = new HtmlPage();
            UnitTestAssert.AreEqual("/foo/bar", page.ResolveNavigateUrl("/bar"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InvalidOperationGetsThrownWhenApplicationPathIsNull()
        {
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder(null);
            HtmlPage page = new HtmlPage();
        }

        [TestMethod]
        public void NewTestPageHasNoElements()
        {
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("/test");
            HtmlPage page = new HtmlPage();
            UnitTestAssert.AreEqual(0, page.Elements.Count);
        }

        [TestMethod]
        public void CanAccessElementsOnThePage()
        {
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("/test");
            MockCommandExecutor commandExecutor = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExecutor);

            string html = @"
                <html id='control1'>
                    <foo id='control2'> 
                        <bar id='control3' />
                    </foo>
                </html>
                ";

            BrowserInfo browserInfo = new BrowserInfo();
            browserInfo.Data = html;
            commandExecutor.SetBrowserInfo(browserInfo);

            HtmlPage page = new HtmlPage();
            page.Elements.Refresh();
            UnitTestAssert.AreEqual("html", page.RootElement.TagName);
            UnitTestAssert.AreEqual("foo", page.Elements.Find("control2").TagName);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NewTestPageHasNoRootElement()
        {
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("/test");
            HtmlPage page = new HtmlPage();
            object o = page.RootElement;
        }

        [TestMethod]
        public void WhenCreatingPageWithAppPath_ShouldUseItToCreateCommandExecutor()
        {
            //arrange
            var mockExecutor = new Mock<IBrowserCommandExecutor>();
            var mockExecutorFactory = new Mock<IBrowserCommandExecutorFactory>();
            mockExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(
                It.Is<string>(u => u == "http://myapppath/"),
                It.IsAny<HtmlPage>())
                ).Returns(mockExecutor.Object);

            ServiceLocator.BrowserCommandExecutorFactory = mockExecutorFactory.Object;

            //act
            var htmlPage = new HtmlPage(new Uri("http://myapppath"));

            //assert
            UnitTestAssert.AreSame(mockExecutor.Object, htmlPage.BrowserCommandExecutor);
        }

        [TestMethod]
        public void WhenCreatingPage_IfNoExecutorFactoryIsRegistered_ShouldUseTheEmulatorFactory()
        {
            //arrange
            ServiceLocator.BrowserCommandExecutorFactory = null;

            //act
            var htmlPage = new HtmlPage(new Uri("http://myapppath"));
 
            //assert
            UnitTestAssert.IsInstanceOfType(htmlPage.BrowserCommandExecutor, typeof(EmulatedBrowserCommandExecutor));
        }

    }
}
