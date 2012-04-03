using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LTAF.Emulators;
using LTAF.Engine;
using MSAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using System.Net;
using System.IO;
using LTAF.UnitTests.Mock;
using Moq;

namespace LTAF.UnitTests.Emulators
{
    /*
     * Back Log:
     *  - Need to print headers and post data on the log
     *  - Need to handle form posts when multiple submit buttons are present. Only one should be valid  :(
     */
    
    [TestClass]
    public class BrowserEmulatorTest
    {
        private Mock<IBrowserCommandExecutorFactory> _commandExecutorFactory;

        [TestInitialize]
        public void Initialize()
        {
            _commandExecutorFactory = new Mock<IBrowserCommandExecutorFactory>();
            ServiceLocator.BrowserCommandExecutorFactory = _commandExecutorFactory.Object;
        }

        #region Constructor
        /// <summary> Verifies that virtual path can not be nul or empty and valid exception is thrown</summary>
        [TestMethod]
        public void WhenCallingConstructor_IfUrlIsEmpty_ShouldThrowAnError()
        {
            //Act, Assert
            ExceptionAssert.Throws<UriFormatException>(
                () => new BrowserEmulator("", new MockWebRequestor(), new MockBrowserEmulatorLog(), new MockFileSystem()));
        }

        [TestMethod]
        public void WhenCallingConstructor_IfUrlHasNoProtocol_ShouldThrowAnError()
        {
            //Act, Assert
            ExceptionAssert.Throws<UriFormatException>(
                () => new BrowserEmulator("/noprotocol", new MockWebRequestor(), new MockBrowserEmulatorLog(), new MockFileSystem()));
        }

        [TestMethod]
        public void WhenCallingConstructor_IfUrlDoesNotEndWithSlash_ShouldAddEndSlash()
        {
            //Act
            BrowserEmulator emulator = new BrowserEmulator("http://server/app", new MockWebRequestor(), new MockBrowserEmulatorLog(), new MockFileSystem());

            //Assert
            Assert.AreEqual("http://server/app/", emulator.CurrentUri.AbsoluteUri);
        }
        #endregion
        
        #region SilentMode Tests
        /// <summary> Verifies that when SilentMode = true, there no output in the log </summary>
        [TestMethod]
        public void WhenSendCommands_IfSilentModeIsTrue_ShouldNotWriteToOutput()
        {
            // Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><span id=\"Span1\" /></body></html>"));
            
            MockBrowserEmulatorLog mockLog = new MockBrowserEmulatorLog();
            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, mockLog, new MockFileSystem());
            emulator.SilentMode = true;

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var htmlPage = new HtmlPage(new Uri("http://localhost/foo"));
            htmlPage.Navigate("Default.htm");

            // Assert
            MSAssert.IsTrue(mockLog.Output.Length <= 0);
        }

        /// <summary> Verifies that when SilentMode = false , there is valid output in the log </summary>
        [TestMethod]
        public void WhenSendCommands_IfSilentModeIsFalse_ShouldWriteToOutput()
        {
            // Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                   new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><span id=\"Span1\" /></body></html>"));

            MockBrowserEmulatorLog mockLog = new MockBrowserEmulatorLog();
            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, mockLog, new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var htmlPage = new HtmlPage(new Uri("http://localhost/foo"));
            htmlPage.Navigate("Default.htm");

            // Assert
            string output = mockLog.Output.ToString().ToLowerInvariant();
            MSAssert.IsTrue(output.Contains("[command started]"));
            MSAssert.IsTrue(output.Contains("[request]"));
            MSAssert.IsTrue(output.Contains("[response]"));
        }
        #endregion
        
        #region Event Tests
        /// <summary> Verifies that BrowserCommandExecuting is fired correctly </summary>
        [TestMethod]
        public void WhenSendingCommand_ShouldRaiseExecutingEvent()
        {
            // Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/Default.htm", 
                new MockHttpWebResponse("http://localhost/foo/Default.htm", "<html><body><span id=\"Span1\" /></body></html>"));

            MockBrowserEmulatorLog mockLog = new MockBrowserEmulatorLog();
            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, mockLog, new MockFileSystem());
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");

            // Assert
            string output = mockLog.Output.ToString().ToLowerInvariant();
            MSAssert.IsTrue(output.Contains("[command started] navigatetourl"));
            MSAssert.IsTrue(output.Contains("[command started] getpagedom"));
        }

        /// <summary> Test that a client can hook up to the RequestSending to inspect and modify our request object </summary>
        [TestMethod]
        public void WhenSendingCommand_IfEventHandlerModifiesRequest_ShouldUseModifiedRequest()
        {
            // Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/valid", 
                new MockHttpWebResponse("http://localhost/foo/valid", "<html><body><span id=\"Span1\" /></body></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());
            emulator.RequestSending += new EventHandler<RequestSendingEventArgs>(RequestSendingHandler);

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act            
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");

            // Assert
            MSAssert.IsTrue(page.Elements.Find("Span1") != null);
        }

        private void RequestSendingHandler(object sender, RequestSendingEventArgs e)
        {
            MockWebRequestor mockRequestor = new MockWebRequestor();
            e.Request = mockRequestor.CreateRequest("http://localhost/foo/valid");
        }

        private MockBrowserEmulatorLog _mockLogForEvent = new MockBrowserEmulatorLog();

        /// <summary> Test that a client can hook up to the ResponseReceived to inspect and modify our response object </summary>
        [TestMethod]
        public void WhenEventHandlerReceivesResponse_ShouldBeAbleToInspect()
        {
            // Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><span id=\"Span1\" /></body></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, _mockLogForEvent, new MockFileSystem());

            emulator.ResponseReceived += new EventHandler<ResponseReceivedEventArgs>(ResponseReceivedHandler);

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");

            // Assert
            string output = _mockLogForEvent.Output.ToString().ToLowerInvariant();
            MSAssert.IsTrue(output.Contains("[response url /foo/default.htm]"));
        }
        
        private void ResponseReceivedHandler(object sender, ResponseReceivedEventArgs e)
        {
            _mockLogForEvent.WriteLine("[response url " + e.Response.ResponseUri.AbsolutePath + "]");
        }
        #endregion
        
        #region Navigate Tests
        [TestMethod]
        public void WhenNavigating_IfResponseContainsRedirect_CurrentUriIsUpdated()
        {
            // Arrange
            MockWebRequestor requestor = new MockWebRequestor();
            requestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/redirect.htm", "<html><body></body></html>"));
            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", requestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");

            //Assert
            MSAssert.AreEqual("/foo/redirect.htm", emulator.CurrentUri.AbsolutePath);
            Assert.AreEqual("get", requestor.RequestHistory.Last().Method.ToLowerInvariant());
            Assert.IsTrue(string.IsNullOrEmpty(requestor.RequestHistory.Last().ContentType));
        }

        /// <summary> Verifies that if invalid markup was returned by server, there is an InvalidOperationException thrown</summary>
        [TestMethod]
        public void WhenNavigating_IfResponseContainsInvalidMarkup_ShouldThrowException()
        {
            // Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/Default.htm", new MockHttpWebResponse("http://localhost/foo/Default.htm",
                @""
            ));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            var page = new HtmlPage(new Uri("http://localhost/foo"));
            
            // Act, Assert
            ExceptionAssert.ThrowsInvalidOperation(
                () => page.Navigate("Default.htm"), "Response did not contain html markup.");
        }

        private class MockFaultyWebRequestor : IWebRequestor
        {
            public WebExceptionStatus ExceptionStatus { get; set; }

            public MockFaultyWebRequestor()
            {
                ExceptionStatus = WebExceptionStatus.UnknownError;
            }

            public HttpWebRequest CreateRequest(string url)
            {
                return new MockHttpWebRequest(url);
            }

            public HttpWebResponse ExecuteRequest(HttpWebRequest request)
            {
                MockHttpWebResponse response = null;
                if (ExceptionStatus != WebExceptionStatus.Timeout)
                {
                    response = new MockHttpWebResponse("http://localhost", "<html><span id='errorMessage'>Error Page</span></html>");
                }

                throw new WebException("Server failed.", null, ExceptionStatus, response);
            }
        }

        [TestMethod]
        public void WhenNavigating_IfServerThrowsWithNoResponse_ShouldReturnPageWithExceptionMessage()
        {
            // Arrange
            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", 
                new MockFaultyWebRequestor() { ExceptionStatus = WebExceptionStatus.Timeout }, 
                new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            
            //Act, Assert
            ExceptionAssert.Throws<WebException>(
                () => page.Navigate("Default.htm"), "Server failed.");
        }

        [TestMethod]
        public void WhenNavigating_IfServerThrows500Error_ShouldRerturnErrorPage()
        {
            // Arrange
            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", new MockFaultyWebRequestor(), 
                new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");

            //Assert
            MSAssert.AreEqual("Error Page", page.Elements.Find("errorMessage").CachedInnerText);
        }
        #endregion
        
        #region Cookies

        [TestMethod]
        public void WhenNavigatingWithCookies_IfAllParametersAreSpecified_ShouldSetCorrectHeaders ()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();

            CookieCollection cookies = new CookieCollection();
            Cookie cookie1 = new Cookie("somecookie", "somevalue", "/foo", "localhost");
            cookie1.Expires = DateTime.MaxValue;
            cookie1.HttpOnly = true;

            Cookie cookie2 = new Cookie("somecookie2", "somevalue2", "/foo", "localhost");
            cookies.Add(cookie1);
            cookies.Add(cookie2);

            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><input id=\"text1\" /></body></html>", 
                    cookies));

            MockBrowserEmulatorLog mockLog = new MockBrowserEmulatorLog();
            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, mockLog, new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            // first request with no cookies - response has Set-Cookie header and cookies
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");

            // Assert
            MSAssert.IsNull(mockRequestor.RequestHistory[0].Headers["Cookie"]);

            // Act
            // second request with previous cookies - response does not return cookie
            mockRequestor.SetResponseForUrl("http://localhost/foo/default1.htm",
                new MockHttpWebResponse("http://localhost/foo/default1.htm", "<html><body><input id=\"text1\" /></body></html>"));

            page.Navigate("Default1.htm");

            CookieContainer container = mockRequestor.RequestHistory[0].CookieContainer;

            // Assert
            MSAssert.AreEqual(container.Count, 2);
            MSAssert.IsTrue(string.Compare(mockRequestor.RequestHistory[1].Headers["Cookie"],
                "somecookie=somevalue; expires=12/31/9999 11:59:59 PM; path=/foo; domain=localhost; httponly, somecookie2=somevalue2; path=/foo; domain=localhost", 
                StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        [TestMethod]
        public void WhenNavigatingWithCoockies_IfThreeSubsequentRequestsChangingCookies_ShouldUpdateCoockies()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();

            CookieCollection cookies = new CookieCollection();
            cookies.Add(new Cookie("somecookie", "somevalue", "/foo", "localhost"));
            cookies.Add(new Cookie("somecookie2", "somevalue2", "/foo", "localhost"));
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><input id=\"text1\" /></body></html>", cookies));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            // first request with no cookies - response has Set-Cookie header and cookies
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");

            // second request with previous cookies - response does not return cookie
            mockRequestor.SetResponseForUrl("http://localhost/foo/default1.htm",
                new MockHttpWebResponse("http://localhost/foo/default1.htm", "<html><body><input id=\"text1\" /></body></html>"));
            
            page.Navigate("Default1.htm");

            // third request with same previous cookies - response sets new value for one of cookies
            cookies = new CookieCollection();
            cookies.Add(new Cookie("somecookie", "newvalue", "/foo", "localhost"));
            mockRequestor.SetResponseForUrl("http://localhost/foo/default2.htm",
                new MockHttpWebResponse("http://localhost/foo/default2.htm", "<html><body><input id=\"text1\" /></body></html>", cookies));

            page.Navigate("Default2.htm");

            // last request with one of cookies having new value, and another one having same value
            mockRequestor.SetResponseForUrl("http://localhost/foo/default3.htm",
                new MockHttpWebResponse("http://localhost/foo/default3.htm", "<html><body><input id=\"text1\" /></body></html>"));

            page.Navigate("Default3.htm");

            // Assert
            MSAssert.IsNull(mockRequestor.RequestHistory[0].Headers["Cookie"]);
            MSAssert.AreEqual(mockRequestor.RequestHistory[1].CookieContainer.Count, 2);
            MSAssert.IsTrue(string.Compare(mockRequestor.RequestHistory[1].Headers["Cookie"],
                "somecookie=somevalue; path=/foo; domain=localhost, somecookie2=somevalue2; path=/foo; domain=localhost",
                StringComparison.InvariantCultureIgnoreCase) == 0);
            MSAssert.AreEqual(mockRequestor.RequestHistory[2].CookieContainer.Count, 2);
            MSAssert.IsTrue(string.Compare(mockRequestor.RequestHistory[2].Headers["Cookie"],
                "somecookie=somevalue; path=/foo; domain=localhost, somecookie2=somevalue2; path=/foo; domain=localhost",
                StringComparison.InvariantCultureIgnoreCase) == 0);
            MSAssert.AreEqual(mockRequestor.RequestHistory[3].CookieContainer.Count, 2);
            MSAssert.IsTrue(string.Compare(mockRequestor.RequestHistory[3].Headers["Cookie"],
                "somecookie=newvalue; path=/foo; domain=localhost, somecookie2=somevalue2; path=/foo; domain=localhost", 
                StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        #endregion

        #region Click Submit Button Tests
        [TestMethod]
        public void WhenClickOnFormSubmitButton_IfNoFormExists_ShouldThrowError()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo1/default1",
                new MockHttpWebResponse("http://localhost/foo1/default1",
                    @"
                    <html>
                        <body>
                            <input type='submit' name='button1' value='button1' />
                        </body>
                    </html>"
                ));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo1", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo1"));
            page.Navigate("Default1");

            ExceptionAssert.ThrowsInvalidOperation(
                () => page.Elements.Find("button1").Click(), "Unable to locate parent form to submit.");
        }

        [TestMethod]
        public void WhenClickOnFormSubmitButton_IfMultipleSubmitButtonsExists_ShouldOnlyContainClickedButtonInPostData()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo1/default1",
                new MockHttpWebResponse("http://localhost/foo1/default1",
                    @"
                     <html>
                        <form id='form1'>
                            <div>
                                <input type='text' name='input1' value='value1' />
                                <input type='submit' name='submit1' value='submit1' />
                                <input type='submit' name='submit2' value='submit2' />                            
                            </div>
                        </form>
                    </html>"
                ));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo1", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo1"));
            page.Navigate("Default1");
            page.Elements.Find("submit2").Click();

            //Assert
            MSAssert.IsTrue(mockRequestor.RequestHistory.Last().GetRequestData().Equals("input1=value1&submit2=submit2",
                StringComparison.InvariantCultureIgnoreCase));
        }
        #endregion

        #region SetText Tests
        [TestMethod]
        public void WhenSetTextBoxIsCalled_ShouldUpdateElementValueProperty()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><input id=\"text1\" /></body></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");
            HtmlInputElement text = (HtmlInputElement) page.Elements.Find("text1");
            text.SetText("sometext");

            // Assert
            MSAssert.AreEqual("sometext", text.CachedAttributes.Value);
        }

        [TestMethod]
        public void WhenSettingText_IfSourceIsNull_ShouldThrowAnError()
        {
            //Arrange
            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", new MockWebRequestor(), new MockBrowserEmulatorLog(), new FileSystem());
            EmulatedBrowserCommandExecutor commandHandler = new EmulatedBrowserCommandExecutor(emulator);

            // Act, Assert
            BrowserCommand command = new BrowserCommand(BrowserCommand.FunctionNames.SetTextBox);
            ExceptionAssert.ThrowsInvalidOperation(
                () => commandHandler.ExecuteCommand(0, null, command, 0),
                "The 'SetText' command requires the HtmlElement source to be an instance of 'LTAF.HtmlInputElement or LTAF.HtmlTextAreaElement'.");
        }

        [TestMethod]
        public void WhenSettingText_IfSourceIsNotInputElement_ShouldThrowAnError()
        {
            //Arrange
            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", new MockWebRequestor(), new MockBrowserEmulatorLog(), new MockFileSystem());
            EmulatedBrowserCommandExecutor commandHandler = new EmulatedBrowserCommandExecutor(emulator);

            // Act, Assert
            BrowserCommand command = new BrowserCommand(BrowserCommand.FunctionNames.SetTextBox);
            ExceptionAssert.ThrowsInvalidOperation(
                () => commandHandler.ExecuteCommand(0, HtmlElement.Create("<foo></foo>"), command, 0),
                "The 'SetText' command requires the HtmlElement source to be an instance of 'LTAF.HtmlInputElement or LTAF.HtmlTextAreaElement'.");
        }

        [TestMethod]
        public void WhenSetTextBoxIsCalled_IfTextArea_ShouldUpdateInnerText()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><textarea name=\"sometextarea\" rows=\"4\" cols=\"50\">[old text]</textarea></body></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");
            HtmlTextAreaElement text = (HtmlTextAreaElement) page.Elements.Find("sometextarea");
            text.SetText("[new text]");

            // Assert
            MSAssert.AreEqual("[new text]", text.CachedInnerText);
        }

        #endregion
        
        #region Click Generic Tests
        [TestMethod]
        public void WhenClicking_IfSourceIsNull_ShouldThrowAnError()
        {
            //Arrange
            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", new MockWebRequestor(), new MockBrowserEmulatorLog(), new MockFileSystem());
            EmulatedBrowserCommandExecutor commandHandler = new EmulatedBrowserCommandExecutor(emulator);

            // Act, Assert
            BrowserCommand command = new BrowserCommand(BrowserCommand.FunctionNames.ClickElement);
            ExceptionAssert.ThrowsInvalidOperation(
                () => commandHandler.ExecuteCommand(0, null, command, 0),
                "The 'ClickElement' command requires the HtmlElement source to be set.");
        }

        [TestMethod]
        public void WhenClicking_IfSourceIsNotInputElement_ShouldThrowAnError()
        {
            //Arrange
            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", new MockWebRequestor(), new MockBrowserEmulatorLog(), new MockFileSystem());
            EmulatedBrowserCommandExecutor commandHandler = new EmulatedBrowserCommandExecutor(emulator);

            // Act, Assert
            BrowserCommand command = new BrowserCommand(BrowserCommand.FunctionNames.ClickElement);
            ExceptionAssert.Throws<NotSupportedException>(
                () => commandHandler.ExecuteCommand(0, HtmlElement.Create("<foo></foo>"), command, 0),
                "The 'ClickElement' command is not supported on the element '<foo id=''>'");
        }
        #endregion
        
        #region Click Anchor Tests
        [TestMethod]
        public void WhenClickLink_IfHasHref_ShouldSendGetRequest()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default",
                new MockHttpWebResponse("http://localhost/foo/default", "<html><body><a name='link1' href='http://localhost/foo/page2' /></body></html>"));
            mockRequestor.SetResponseForUrl("http://localhost/foo/page2",
             new MockHttpWebResponse("http://localhost/foo/page2", "<html>2nd Page</html>" ));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("default");
            page.Elements.Find("link1").Click();

            // Assert
            Assert.AreEqual("http://localhost/foo/page2", mockRequestor.RequestHistory.Last().RequestUri.AbsoluteUri);
        }

        [TestMethod]
        public void WhenClickLink_IfHrefIsRelative_ShouldSendRequestWithCombinedUrl()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default",
                new MockHttpWebResponse("http://localhost/foo/default", "<html><body><a name='link1' href='/foo/page2' /></body></html>"));
            mockRequestor.SetResponseForUrl("http://localhost/foo/page2",
             new MockHttpWebResponse("http://localhost/foo/page2", "<html>2nd Page</html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("default");
            page.Elements.Find("link1").Click();

            // Assert
            Assert.AreEqual("http://localhost/foo/page2", mockRequestor.RequestHistory.Last().RequestUri.AbsoluteUri);
        }

        [TestMethod]
        public void WhenClickLink_IfHrefEmpty_ShouldSendRequestToLastKnownUrl()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default",
                new MockHttpWebResponse("http://localhost/foo/default", "<html><body><a name='link1' href='' /></body></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("default");
            page.Elements.Find("link1").Click();

            // Assert
            Assert.AreEqual("http://localhost/foo/default", mockRequestor.RequestHistory.Last().RequestUri.AbsoluteUri);
        }

        [TestMethod]
        public void WhenClickLink_IfHrefIsMissing_ShouldThrowError()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default",
                new MockHttpWebResponse("http://localhost/foo/default", "<html><body><a name='link1' /></body></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("default");

            // Act, Assert
            ExceptionAssert.ThrowsInvalidOperation(
                () => page.Elements.Find("link1").Click(), "Anchor element does not contain an Href attribute.");
        }
        #endregion
      
        #region FormSubmit Tests
        [TestMethod]
        public void WhenSubmittingForm_IfTextAreaElementsExist_ShouldIncludeThemToPostdata()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm",
                    "<html><body><form id='form1'><textarea name=\"sometextarea\" rows=\"4\" cols=\"50\">[old text]</textarea><input type='submit' id=\"button1\" /></form></body></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");
            page.Elements.Find("sometextarea").CachedInnerText = "[new text]";
            ((HtmlFormElement)page.Elements.Find("form1")).Submit();

            // Assert
            MSAssert.IsTrue(mockRequestor.RequestHistory.Last().GetRequestData().Equals("sometextarea=%5bnew+text%5d",
                StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        public void WhenSubmittinForm_ShouldSendPostRequestToFormActionUrl()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", 
                    "<html><body><form id='form1' action='http://form/action'><input type='submit' id=\"button1\" /></form></body></html>"));
            mockRequestor.SetResponseForUrl("http://form/action",
             new MockHttpWebResponse("http://form/action", "<html>Form posted!</html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");
            ((HtmlFormElement)page.Elements.Find("form1")).Submit();

            // Assert
            MSAssert.AreEqual("http://form/action", mockRequestor.RequestHistory.Last().RequestUri.AbsoluteUri);
        }

        [TestMethod]
        public void WhenSubmittingForm_IfFormHasVirtualUrl_ShouldSendPostRequestCombiningPageUrlWithForm()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default",
                new MockHttpWebResponse("http://localhost/foo/default",  
                    "<html><body><form id='form1' action='action'><input type='submit' id=\"button1\" /></form></body></html>"));
            mockRequestor.SetResponseForUrl("http://localhost/foo/action",
                new MockHttpWebResponse("http://localhost/foo/action", "<html>Form posted!</html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default");
            ((HtmlFormElement)page.Elements.Find("form1")).Submit();

            // Assert
            MSAssert.AreEqual("http://localhost/foo/action", mockRequestor.RequestHistory.Last().RequestUri.AbsoluteUri);
        }
       
        [TestMethod]
        public void WhenSubmittingForm_ShouldSendPostRequestWithFormDataAndHeaders()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default",
                new MockHttpWebResponse("http://localhost/foo/default",
                    @"
                    <html>
                        <body>
                            <form id='form1' action='action' method='put'>
                                <input type='text' name='textbox1' value='value1' />
                                <input type='submit' name='button1' value='button1' />
                            </form>
                        </body>
                    </html>"));

            mockRequestor.SetResponseForUrl("http://localhost/foo/action",
             new MockHttpWebResponse("http://localhost/foo/action", "<html>Form posted!</html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default");
            ((HtmlFormElement)page.Elements.Find("form1")).Submit();

            // Assert
            MSAssert.IsTrue(mockRequestor.RequestHistory.Last().GetRequestData().Equals("textbox1=value1&button1=button1", 
                StringComparison.InvariantCultureIgnoreCase));
            MSAssert.AreEqual("put", mockRequestor.RequestHistory.Last().Method.ToLowerInvariant());
            MSAssert.AreEqual("application/x-www-form-urlencoded", mockRequestor.RequestHistory.Last().ContentType);
        }
       
        [TestMethod]
        public void WhenSubmittingForm_IfActionAndMethodAreMissing_ShouldSendPostRequestWithDefaultData()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo2/default2",
                new MockHttpWebResponse("http://localhost/foo2/default2",
                    @"
                    <html>
                        <body>
                            <form id='form2' action='default2'>
                                <input type='text' name='textbox1' value='value1' />
                                <input type='submit' name='button1' value='button1' />
                            </form>
                        </body>
                    </html>"
                ));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo2", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo2"));
            page.Navigate("default2");
            ((HtmlFormElement)page.Elements.Find("form2")).Submit();

            // Assert
            MSAssert.IsTrue(string.IsNullOrEmpty(mockRequestor.RequestHistory.First().GetRequestData()));
            MSAssert.AreEqual("http://localhost/foo2/default2", mockRequestor.RequestHistory.Last().RequestUri.AbsoluteUri);
            MSAssert.AreEqual("post", mockRequestor.RequestHistory.Last().Method.ToLowerInvariant());
        }
        #endregion

        #region Click Checkbox Tests
        [TestMethod]
        public void WhenClickCheckbox_IfNotChecked_ShouldSetChecked()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default",
                new MockHttpWebResponse("http://localhost/foo/default", "<html><body><input type='checkbox' name='somecheckbox' value='somecheckboxvalue' /></body></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("default");
            page.Elements.Find("somecheckbox").Click();

            // Assert
            Assert.AreEqual(true, ((HtmlInputElement) page.Elements.Find("somecheckbox")).CachedAttributes.Checked);
        }

        [TestMethod]
        public void WhenClickCheckBox_ShouldToggleCheckedValue()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default",
                new MockHttpWebResponse("http://localhost/foo/default", "<html><body><input type='checkbox' name='somecheckbox' value='somecheckboxvalue' checked /></body></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("default");
            var checkbox = page.Elements.Find("somecheckbox");
            checkbox.Click();

            // Assert
            Assert.AreEqual(false, ((HtmlInputElement)page.Elements.Find("somecheckbox")).CachedAttributes.Checked);

            // Act
            checkbox.Click();

            // Assert
            Assert.AreEqual(true, ((HtmlInputElement)page.Elements.Find("somecheckbox")).CachedAttributes.Checked);
        }

        #endregion

        #region Click Radio Tests
        [TestMethod]
        public void WhenClickRadio_IfNoOneChecked_ShouldSetChecked()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default",
                new MockHttpWebResponse("http://localhost/foo/default", "<html><body><input type='radio' name='someradio' value='someradiovalue1' /> " + 
                    " <input type='radio' name='someradio' value='someradiovalue2' />" +
                    " <input type='radio' name='someradio' value='someradiovalue3' /> </body></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("default");
            HtmlElement radio = page.Elements.Find(new { name="someradio", value = "someradiovalue1" });
            radio.Click();

            // Assert
            Assert.AreEqual(true, ((HtmlInputElement)radio).CachedAttributes.Checked);
        }

        [TestMethod]
        public void WhenClickRadio_IfOneChecked_ShouldUncheckOldAndCheckNew()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default",
                new MockHttpWebResponse("http://localhost/foo/default", @"
                <html><body>
                    <input type='radio' name='someradio' value='someradiovalue1' /> 
                    <input type='radio' name='someradio' value='someradiovalue2' checked />
                    <input type='radio' name='someradio' value='someradiovalue3' /> 
                </body></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("default");
            HtmlInputElement radio3 = (HtmlInputElement)page.Elements.Find(new { name = "someradio", value = "someradiovalue3" });
            radio3.Click();

            // Assert
            Assert.AreEqual(true, radio3.CachedAttributes.Checked);

            // Act 
            HtmlInputElement radio1 = (HtmlInputElement)page.Elements.Find(new { name = "someradio", value = "someradiovalue1" });
            radio1.Click();

            // Assert
            Assert.AreEqual(true, radio1.CachedAttributes.Checked);
            Assert.AreEqual(false, radio3.CachedAttributes.Checked);

            //Act 
            radio1.Click();

            // Assert
            Assert.AreEqual(true, radio1.CachedAttributes.Checked);
        }

        [TestMethod]
        public void WhenClickRadio_IfSomeOtherRadioPresent_ShouldCheckUncheckOnlyRadiosWithGivenName()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default",
                new MockHttpWebResponse("http://localhost/foo/default", "<html><body><input type='radio' name='someradio' value='someradiovalue1' checked /> " +
                    " <input type='radio' name='someradio' value='someradiovalue2' />" +
                    " <input type='radio' name='someradio' value='someradiovalue3' />"  +
                    " <input type='radio' name='someradio2' value='someradiovalue' checked /> </body></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("default");
            HtmlInputElement radio = (HtmlInputElement)page.Elements.Find(new { name = "someradio", value = "someradiovalue2" });
            radio.Click();

            // Assert
            Assert.AreEqual(true, radio.CachedAttributes.Checked);
            Assert.AreEqual(true, ((HtmlInputElement)page.Elements.Find("someradio2")).CachedAttributes.Checked);
        }

        #endregion

        #region FileUpload tests

        [TestMethod]
        public void WhenOneFileToUploadSpecified_IfNoPostData_ShouldOutputOnlyOneFileInMultipartFormat()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default",
                new MockHttpWebResponse("http://localhost/foo/default",
                    @"
                    <html>
                        <body>
                            <form id='form1' method='post'>
                                <input type='file' name='fileupload' value='' />
                                <input type='submit' />
                            </form>
                        </body>
                    </html>"));

            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.Files.Add(@"c:\temp\somefile.ext",
@"
Some file Content
Some file Content
Some file Content
"
            );

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, 
                new MockBrowserEmulatorLog(), fileSystem);

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default");

            page.Elements.Find("input", 0).SetText(@"c:\temp\somefile.ext");

            page.Elements.Find("input", 1).Click();

            // Assert
            Assert.AreEqual("post", mockRequestor.RequestHistory.Last().Method.ToLowerInvariant());
            Assert.AreEqual("multipart/form-data; boundary=----------------------------8ccae08efb39ed2", mockRequestor.RequestHistory.Last().ContentType);
            Assert.IsTrue(mockRequestor.RequestHistory.Last().GetRequestData().Equals(
                @"
------------------------------8ccae08efb39ed2
Content-Disposition: form-data; name=""fileupload""; filename=""c:\temp\somefile.ext""
 Content-Type: application/octet-stream


Some file Content
Some file Content
Some file Content

------------------------------8ccae08efb39ed2
",
            StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        public void WhenOneFileToUploadSpecified_IfPostDataAlsoSpecified_ShouldOutputPostDataAndFileInMultipartFormat()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default",
                new MockHttpWebResponse("http://localhost/foo/default",
                    @"
                    <html>
                        <body>
                            <form id='form1' method='post'>
                                <input type='text' name='textbox1' value='value1' />
                                <input type='text' name='textbox2' value='value2' />
                                <input type='file' name='fileupload' value='' />
                                <input type='submit' />
                            </form>
                        </body>
                    </html>"));

            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.Files.Add(@"c:\temp\somefile.ext",
@"
Some file Content
Some file Content
Some file Content
"
            );

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor,
                new MockBrowserEmulatorLog(), fileSystem);

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default");
            page.Elements.Find("input", 2).SetText(@"c:\temp\somefile.ext");

            page.Elements.Find("input", 3).Click();

            // Assert
            Assert.AreEqual("post", mockRequestor.RequestHistory.Last().Method.ToLowerInvariant());
            Assert.AreEqual("multipart/form-data; boundary=----------------------------8ccae08efb39ed2", mockRequestor.RequestHistory.Last().ContentType);
            Assert.IsTrue(mockRequestor.RequestHistory.Last().GetRequestData().Equals(
                @"
------------------------------8ccae08efb39ed2
Content-Disposition: form-data; name=""textbox1"";

value1
------------------------------8ccae08efb39ed2
Content-Disposition: form-data; name=""textbox2"";

value2
------------------------------8ccae08efb39ed2
Content-Disposition: form-data; name=""fileupload""; filename=""c:\temp\somefile.ext""
 Content-Type: application/octet-stream


Some file Content
Some file Content
Some file Content

------------------------------8ccae08efb39ed2
",
            StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        public void WhenSeveralFilesToUploadSpecified_IfNamesAreDuplicated_ShouldAddIndex()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default",
                new MockHttpWebResponse("http://localhost/foo/default",
                    @"
                    <html>
                        <body>
                            <form id='form1' method='post'>
                                <input type='text' name='textbox1' value='value1' />
                                <input type='text' name='textbox2' value='value2' />
                                <input type='file' name='fileupload' value='' />
                                <input type='file' name='otherfileupload' value='' />
                                <input type='submit' />
                            </form>
                        </body>
                    </html>"));

            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.Files.Add(@"c:\temp\somefile.ext",
@"
Some file Content
Some file Content
Some file Content
"
            );
            fileSystem.Files.Add(@"c:\temp\somefile1.ext",
@"
Some file1 Content
Some file1 Content
Some file1 Content
"
            );
            fileSystem.Files.Add(@"c:\temp\somefile2.ext",
@"
Some file2 Content
Some file2 Content
Some file2 Content
"
            );


            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor,
                new MockBrowserEmulatorLog(), fileSystem);

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default");
            page.Elements.Find("input", 2).SetText(@"c:\temp\somefile.ext;c:\temp\somefile1.ext");
            page.Elements.Find("input", 3).SetText(@"c:\temp\somefile2.ext");

            page.Elements.Find("input", 4).Click();

            // Assert
            Assert.AreEqual("post", mockRequestor.RequestHistory.Last().Method.ToLowerInvariant());
            Assert.AreEqual("multipart/form-data; boundary=----------------------------8ccae08efb39ed2", mockRequestor.RequestHistory.Last().ContentType);
            Assert.IsTrue(mockRequestor.RequestHistory.Last().GetRequestData().Equals(
                @"
------------------------------8ccae08efb39ed2
Content-Disposition: form-data; name=""textbox1"";

value1
------------------------------8ccae08efb39ed2
Content-Disposition: form-data; name=""textbox2"";

value2
------------------------------8ccae08efb39ed2
Content-Disposition: form-data; name=""fileupload""; filename=""c:\temp\somefile.ext""
 Content-Type: application/octet-stream


Some file Content
Some file Content
Some file Content

------------------------------8ccae08efb39ed2
Content-Disposition: form-data; name=""fileupload1""; filename=""c:\temp\somefile1.ext""
 Content-Type: application/octet-stream


Some file1 Content
Some file1 Content
Some file1 Content

------------------------------8ccae08efb39ed2
Content-Disposition: form-data; name=""otherfileupload""; filename=""c:\temp\somefile2.ext""
 Content-Type: application/octet-stream


Some file2 Content
Some file2 Content
Some file2 Content

------------------------------8ccae08efb39ed2
",
            StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        public void WhenFileIsEmpty_ShouldStillBeUploaded()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default",
                new MockHttpWebResponse("http://localhost/foo/default",
                    @"
                    <html>
                        <body>
                            <form id='form1' method='post'>
                                <input type='file' name='fileupload' value='' />
                                <input type='submit' />
                            </form>
                        </body>
                    </html>"));

            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.Files.Add(@"c:\temp\somefile.ext", @"");

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor,
                new MockBrowserEmulatorLog(), fileSystem);

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default");

            page.Elements.Find("input", 0).SetText(@"c:\temp\somefile.ext");

            page.Elements.Find("input", 1).Click();

            // Assert
            Assert.AreEqual("post", mockRequestor.RequestHistory.Last().Method.ToLowerInvariant());
            Assert.AreEqual("multipart/form-data; boundary=----------------------------8ccae08efb39ed2", mockRequestor.RequestHistory.Last().ContentType);
            Assert.IsTrue(mockRequestor.RequestHistory.Last().GetRequestData().Equals(
                @"
------------------------------8ccae08efb39ed2
Content-Disposition: form-data; name=""fileupload""; filename=""c:\temp\somefile.ext""
 Content-Type: application/octet-stream


------------------------------8ccae08efb39ed2
",
            StringComparison.InvariantCultureIgnoreCase));

        }

        #endregion

        #region GetElementInnerHtml Tests

        [TestMethod]
        public void WhenGetElementInnerHtml_IfElementHasNoFullClosingTag_ShouldReturnEmptyString()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><input id=\"text1\" /></body><div id=\"div1\" /></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");
            HtmlElement div = page.Elements.Find("div1");

            // Assert
            MSAssert.IsTrue(string.IsNullOrEmpty(div.GetInnerHtml()));
        }

        [TestMethod]
        public void WhenGetElementInnerHtml_IfElementHasFullClosingTagAndNoHtml_ShouldReturnEmptyString()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><input id=\"text1\" /></body><div id=\"div1\"></div></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");
            HtmlElement div = page.Elements.Find("div1");

            // Assert
            MSAssert.IsTrue(string.IsNullOrEmpty(div.GetInnerHtml()));
        }

        [TestMethod]
        public void WhenGetElementInnerHtml_IfElementHasNoInnerHtml_ShouldReturnValidInnerTextString()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><input id=\"text1\" /></body><div id=\"div1\" >Some Text </div></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");
            HtmlElement div = page.Elements.Find("div1");

            // Assert
            MSAssert.AreEqual("Some Text", div.GetInnerHtml());
        }

        [TestMethod]
        public void WhenGetElementInnerHtml_IfElementHasInnerHtmlAndText_ShouldReturnBothInnerHtmlAndText()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><input id=\"text1\" /></body><div id=\"div1\" >Some Text <ul> <p/> some other text</ul></div></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");
            HtmlElement div = page.Elements.Find("div1");

            // Assert
            MSAssert.AreEqual("Some Text <ul> <p/> some other text</ul>", div.GetInnerHtml());
        }

        [TestMethod]
        public void WhenGetElementInnerHtml_IfSourceIsNull_ShouldThrowAnError()
        {
            // Arrange
            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", new MockWebRequestor(), new MockBrowserEmulatorLog(), new FileSystem());
            EmulatedBrowserCommandExecutor commandHandler = new EmulatedBrowserCommandExecutor(emulator);

            // Act, Assert
            BrowserCommand command = new BrowserCommand(BrowserCommand.FunctionNames.GetElementInnerHtml);
            ExceptionAssert.ThrowsInvalidOperation(
                () => commandHandler.ExecuteCommand(0, null, command, 0),
                "The 'GetElementInnerHtml' command requires the HtmlElement source to be set.");
        }

        #endregion 

        #region GetElementDom Tests

        [TestMethod]
        public void WhenFindInElementChildrenCollection_ShouldCallGetElementDomAndUpdateElementsAndFindShouldSucceed()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><input id=\"text1\" /></body><div id=\"div1\" >Some Text <ul> <p/> some other text</ul></div></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");
            HtmlElement div = page.Elements.Find("div1");

            HtmlElement ul = div.ChildElements.Find("ul", 0);

            // Assert
            MSAssert.AreEqual("ul", ul.TagName);
        }

        [TestMethod]
        public void WhenGetElementDom_IfSourceIsNull_ShouldThrowAnError()
        {
            // Arrange
            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", new MockWebRequestor(), new MockBrowserEmulatorLog(), new FileSystem());
            EmulatedBrowserCommandExecutor commandHandler = new EmulatedBrowserCommandExecutor(emulator);

            // Act, Assert
            BrowserCommand command = new BrowserCommand(BrowserCommand.FunctionNames.GetElementDom);
            ExceptionAssert.ThrowsInvalidOperation(
                () => commandHandler.ExecuteCommand(0, null, command, 0),
                "The 'GetElementDom' command requires the HtmlElement source to be set.");
        }

        #endregion

        #region SetSelectBoxIndex

        [TestMethod]
        public void WhenSetSelectBoxIndex_IfSourceIsNull_ShouldThrowAnError()
        {
            // Arrange
            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", new MockWebRequestor(), new MockBrowserEmulatorLog(), new FileSystem());
            EmulatedBrowserCommandExecutor commandHandler = new EmulatedBrowserCommandExecutor(emulator);

            // Act, Assert
            BrowserCommand command = new BrowserCommand(BrowserCommand.FunctionNames.SetSelectBoxIndex);
            ExceptionAssert.ThrowsInvalidOperation(
                () => commandHandler.ExecuteCommand(0, null, command, 0),
                "The 'SetSelectBoxIndex' command requires the HtmlElement source to be set.");
        }


        [TestMethod]
        public void WhenSetSelectBoxIndex_IfIndexIsOutOfRange_ShouldThrowAnError()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", @"
<html>
    <body>
        <select id='select1' >
            <option value='o1' selected /> 
            <option value='o2' selected /> 
        </select>
    </body>
</html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");
            HtmlSelectElement select = (HtmlSelectElement)page.Elements.Find("select1");

            // Assert
            ExceptionAssert.ThrowsElementNotFound(() => select.SetSelectedIndex(2));
        }

        [TestMethod]
        public void WhenSetSelectBoxIndex_IfNegativeIndexIsOutOfRange_ShouldThrowAnError()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><select id='select1' ><option value='o1' selected /> <option value='o2' selected /> </select></body></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");
            HtmlSelectElement select = (HtmlSelectElement)page.Elements.Find("select1");

            // Assert
            ExceptionAssert.ThrowsInvalidOperation(
                    () => select.SetSelectedIndex(-2),
                    "The command 'SetSelectBoxIndex', negative index out of range: '-2'. Use -1 to unselect all options for select html element.");
        }

        [TestMethod]
        public void WhenSetSelectBoxIndex_IfIndexNegativeOne_ShouldCleanAllSelectedItems()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><select id='select1' ><option value='o1' selected /> <option value='o2' selected /> </select></body></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");
            HtmlSelectElement select = (HtmlSelectElement)page.Elements.Find("select1");
            select.SetSelectedIndex(-1);

            // Assert
            var options = select.ChildElements.FindAll("option");
            MSAssert.IsFalse(((HtmlOptionElement)options[0]).CachedAttributes.Selected);
            MSAssert.IsFalse(((HtmlOptionElement)options[1]).CachedAttributes.Selected);
        }

        [TestMethod]
        public void WhenSetSelectBoxIndex_IfIndexIsValid_ShouldCleanOldSelectedItemsAndSelectNewOne()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><select id='select1' ><option value='o1' selected /> <option value='o2' /> </select></body></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());

            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");
            HtmlSelectElement select = (HtmlSelectElement)page.Elements.Find("select1");
            select.SetSelectedIndex(1);

            // Assert
            var options = select.ChildElements.FindAll("option");
            MSAssert.IsFalse(((HtmlOptionElement)options[0]).CachedAttributes.Selected);
            MSAssert.IsTrue(((HtmlOptionElement)options[1]).CachedAttributes.Selected);
        }

        #endregion

        #region GetInnerText

        [TestMethod]
        public void WhenGetInnerText_IfElementHasOnlyInnerText_ShouldReturnValidInnerTextString()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><input id=\"text1\" /></body><div id=\"div1\" >Some Text </div></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");
            HtmlElement div = page.Elements.Find("div1");

            // Assert
            MSAssert.AreEqual("Some Text", div.GetInnerText());
        }

        [TestMethod]
        public void WhenGetInnerText_IfElementHasEmptyInnerText_ShouldReturnEmptyInnerTextString()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><input id=\"text1\" /></body><div id=\"div1\" > </div></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");
            HtmlElement div = page.Elements.Find("div1");

            // Assert
            MSAssert.IsTrue(string.IsNullOrEmpty(div.GetInnerText()));
        }

        [TestMethod]
        public void WhenGetInnerText_IfElementHasInnerHtmlTags_ShouldReturnValidInnerTextString()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><input id=\"text1\" /></body><div id=\"div1\" >Some Text <p>xxxxxx </p><br/> <div>yyyy yyyy</div> another part of text</div></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");
            HtmlElement div = page.Elements.Find("div1");

            // Assert
            MSAssert.AreEqual("Some Textanother part of text", div.GetInnerText());
        }

        [TestMethod]
        public void WhenGetInnerText_IfElementInnerHtmlAtTheEnd_ShouldReturnValidInnerTextString()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><input id=\"text1\" /></body><div id=\"div1\" >Some Text <p/></div></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");
            HtmlElement div = page.Elements.Find("div1");

            // Assert
            MSAssert.AreEqual("Some Text", div.GetInnerText());
        }

        [TestMethod]
        public void WhenGetInnerText_IfElementHasOnlyInnerHtmlTags_ShouldReturnEmptyInnerTextString()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><input id=\"text1\" /></body><div id=\"div1\" ><p>xxxxxx </p><br/> <div>yyyy yyyy</div></div></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");
            HtmlElement div = page.Elements.Find("div1");

            // Assert
            MSAssert.IsTrue(string.IsNullOrEmpty(div.GetInnerText()));
        }

        #endregion

        #region GetInnerTextRecursively

        [TestMethod]
        public void WhenGetInnerTextRecursively_IfElementHasInnerHtmlTags_ShouldReturnValidRecursiveInnerTextString()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><input id=\"text1\" /></body><div id=\"div1\" >Some Text <p>xxxxxx </p><br/> <div>yyyy yyyy</div> another part of text</div></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");
            HtmlElement div = page.Elements.Find("div1");

            // Assert
            MSAssert.AreEqual("Some Text xxxxxx  yyyy yyyy another part of text", div.GetInnerTextRecursively());
        }

        [TestMethod]
        public void WhenGetInnerTextRecursively_IfElementHasInnerHtmlTagsAndNoText_ShouldReturnEmptyString()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><input id=\"text1\" /></body><div id=\"div1\" ><p></p><br/><div g=sdsadsads></div></div></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");
            HtmlElement div = page.Elements.Find("div1");

            // Assert
            MSAssert.IsTrue(string.IsNullOrEmpty(div.GetInnerTextRecursively()));
        }

        [TestMethod]
        public void WhenGetInnerTextRecursively_IfElementHasInnerTextAfterAllTags_ShouldReturnValidInnerText()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><input id=\"text1\" /></body><div id=\"div1\" ><p></p><br/><div></div> Some Text </div></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");
            HtmlElement div = page.Elements.Find("div1");

            // Assert
            MSAssert.AreEqual("Some Text", div.GetInnerTextRecursively());
        }

        #endregion

        #region GetElementAttributes Tests

        [TestMethod]
        public void WhenGetElementAttributes_ShouldReturnAllAttributes()
        {
            //Arrange
            MockWebRequestor mockRequestor = new MockWebRequestor();
            mockRequestor.SetResponseForUrl("http://localhost/foo/default.htm",
                new MockHttpWebResponse("http://localhost/foo/default.htm", "<html><body><input id=\"text1\" style=\"somevalue\" someattr=\"somevalue2\" /></body><div id=\"div1\" >Some Text <ul> <p/> some other text</ul></div></html>"));

            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", mockRequestor, new MockBrowserEmulatorLog(), new MockFileSystem());
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(emulator.CreateCommandExecutor());

            // Act
            var page = new HtmlPage(new Uri("http://localhost/foo"));
            page.Navigate("Default.htm");
            HtmlElementAttributeReader attributes = page.Elements.Find("text1").GetAttributes();

            // Assert
            MSAssert.AreEqual(3, attributes.Dictionary.Count);
        }

        [TestMethod]
        public void WhenGetElementAttributes_IfSourceIsNull_ShouldThrowAnError()
        {
            // Arrange
            BrowserEmulator emulator = new BrowserEmulator("http://localhost/foo", new MockWebRequestor(), new MockBrowserEmulatorLog(), new FileSystem());
            EmulatedBrowserCommandExecutor commandHandler = new EmulatedBrowserCommandExecutor(emulator);

            // Act, Assert
            BrowserCommand command = new BrowserCommand(BrowserCommand.FunctionNames.GetElementAttributes);
            ExceptionAssert.ThrowsInvalidOperation(
                () => commandHandler.ExecuteCommand(0, null, command, 0),
                "The 'GetElementAttributes' command requires the HtmlElement source to be set.");
        }

        #endregion
    }     
}
