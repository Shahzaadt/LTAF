using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using LTAF;
using Moq;


namespace LTAF.UnitTests.UI
{
    [TestClass]
    public class HtmlTableElementTest
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
        public void TwoRowsAreReturned()
        {
            string html = @"
                <html id='control1'>
                    <table id='MyTable'> 
                        <tbody>
                            <tr>First</tr>
                            <tr>Second</tr>
                        </tbody>
                    </table>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);
            HtmlTableElement table = (HtmlTableElement) element.ChildElements.Find("MyTable");
            UnitTestAssert.AreEqual(2, table.Rows.Count);
            UnitTestAssert.AreEqual("First", table.Rows[0].CachedInnerText);
            UnitTestAssert.AreEqual("Second", table.Rows[1].CachedInnerText);
        }

        [TestMethod]
        public void NoRowsAreReturnedWhenTBodyIsNotThre()
        {
            string html = @"
                <html id='control1'>
                    <table id='MyTable'> 
                    </table>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html);
            HtmlTableElement table = (HtmlTableElement)element.ChildElements.Find("MyTable");
            UnitTestAssert.AreEqual(0, table.Rows.Count);
        }


        [TestMethod]
        public void RowsAreRefreshedWhenTableIsRefreshed()
        {
            MockCommandExecutor commandExecutor = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExecutor);
            var testPage = new HtmlPage();

            string html = @"
                <html id='control1'>
                    <table id='MyTable'> 
                        <tbody>
                            <tr>First</tr>
                            <tr>Second</tr>
                        </tbody>
                    </table>
                </html>
                ";

            commandExecutor.SetBrowserInfo(new LTAF.Engine.BrowserInfo()
            {
                Data = @"<table id='MyTable'>
                            <tbody>
                                <tr>First Refreshed</tr>
                                <tr>Second Refreshed</tr>
                            </tbody>
                        </table>"
            });

            HtmlElement element = HtmlElement.Create(html, testPage, false);
            HtmlTableElement table = (HtmlTableElement)element.ChildElements.Find("MyTable");
            table.ChildElements.Refresh();
            UnitTestAssert.AreEqual(2, table.Rows.Count);
            UnitTestAssert.AreEqual("First Refreshed", table.Rows[0].CachedInnerText);
            UnitTestAssert.AreEqual("Second Refreshed", table.Rows[1].CachedInnerText);
        }

        [TestMethod]
        public void RowsAreRefreshedWhenTBodyIsRefreshed()
        {
            MockCommandExecutor commandExecutor = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExecutor);
            var testPage = new HtmlPage();

            string html = @"
                <html id='control1'>
                    <table id='MyTable'> 
                        <tbody>
                            <tr>First</tr>
                            <tr>Second</tr>
                        </tbody>
                    </table>
                </html>
                ";

            commandExecutor.SetBrowserInfo(new LTAF.Engine.BrowserInfo()
            {
                Data = @"<tbody>
                                <tr>First Refreshed</tr>
                                <tr>Second Refreshed</tr>
                            </tbody>"
            });

            HtmlElement element = HtmlElement.Create(html, testPage, false);
            HtmlTableElement table = (HtmlTableElement)element.ChildElements.Find("MyTable");
            table.TBody.ChildElements.Refresh();
            UnitTestAssert.AreEqual(2, table.Rows.Count);
            UnitTestAssert.AreEqual("First Refreshed", table.Rows[0].CachedInnerText);
            UnitTestAssert.AreEqual("Second Refreshed", table.Rows[1].CachedInnerText);
        }

        [TestMethod]
        public void TableIsRefreshedThenTBodyIsRefreshed()
        {
            MockCommandExecutor commandExecutor = new MockCommandExecutor();
            _commandExecutorFactory.Setup(m => m.CreateBrowserCommandExecutor(It.IsAny<string>(), It.IsAny<HtmlPage>())).Returns(commandExecutor);
            var testPage = new HtmlPage();

            string html = @"
                <html id='control1'>
                    <table id='MyTable'> 
                        <tbody>
                            <tr>First</tr>
                            <tr>Second</tr>
                        </tbody>
                    </table>
                </html>
                ";

            HtmlElement element = HtmlElement.Create(html, testPage, false);
            HtmlTableElement table = (HtmlTableElement)element.ChildElements.Find("MyTable");

            commandExecutor.SetBrowserInfo(new LTAF.Engine.BrowserInfo()
            {
                Data = @"<table id='MyTable'>
                            <tbody>
                                <tr>First Refreshed</tr>
                                <tr>Second Refreshed</tr>
                            </tbody>
                        </table>"
            });

            table.ChildElements.Refresh();
            UnitTestAssert.AreEqual(2, table.Rows.Count);
            UnitTestAssert.AreEqual("First Refreshed", table.Rows[0].CachedInnerText);
            UnitTestAssert.AreEqual("Second Refreshed", table.Rows[1].CachedInnerText);

            commandExecutor.SetBrowserInfo(new LTAF.Engine.BrowserInfo()
            {
                Data = @"<tbody>
                                <tr>First Refreshed again</tr>
                                <tr>Second Refreshed again</tr>
                                <tr>Third Refreshed again</tr>
                            </tbody>"
            });

            table.TBody.ChildElements.Refresh();
            UnitTestAssert.AreEqual(3, table.Rows.Count);
            UnitTestAssert.AreEqual("First Refreshed again", table.Rows[0].CachedInnerText);
            UnitTestAssert.AreEqual("Second Refreshed again", table.Rows[1].CachedInnerText);
            UnitTestAssert.AreEqual("Third Refreshed again", table.Rows[2].CachedInnerText);
        }
    }
}
