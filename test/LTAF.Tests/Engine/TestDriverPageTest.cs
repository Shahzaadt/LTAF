using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using LTAF.Engine;
using System.Threading;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;

namespace LTAF.UnitTests.Engine
{
    [TestClass]
    public class TestDriverPageTest
    {
        [TestMethod]
        public void OnLoadComplete_ShowConsoleFromQueryString()
        {
            MockAspNetService mockAspNet = new MockAspNetService();
            mockAspNet.SetIsPostBack(false);
            NameValueCollection col = new NameValueCollection();
            col.Add("console", "false");
            mockAspNet.SetQueryString(col);

            TestDriverPage page = new TestDriverPage(mockAspNet);
            page.WriteLogToDiskCheckBox = new CheckBox();
            page.ShowConsoleCheckBox = new CheckBox();
            page.FooterPanel = new Panel();
            page.TestsPanel = new Panel();
            page.OnLoadCompleteInternal();

            UnitTestAssert.AreEqual("testsNoConsole", page.TestsPanel.CssClass);
            UnitTestAssert.IsFalse(page.FooterPanel.Visible);
        }

        [TestMethod]
        public void OnLoadComplete_ShowsConsoleIfCheckboxIsSet()
        {
            MockAspNetService mockAspNet = new MockAspNetService();
            mockAspNet.SetIsPostBack(true);

            TestDriverPage page = new TestDriverPage(mockAspNet);
            page.ShowConsoleCheckBox = new CheckBox();
            page.ShowConsoleCheckBox.Checked = true;

            page.TestsPanel = new Panel();
            page.FooterPanel = new Panel();

            page.OnLoadCompleteInternal();
            UnitTestAssert.AreEqual("tests", page.TestsPanel.CssClass);
            UnitTestAssert.IsTrue(page.FooterPanel.Visible);
        }

        [TestMethod]
        public void OnLoadComplete_HidesConsoleIfCheckboxIsNotSet()
        {
            MockAspNetService mockAspNet = new MockAspNetService();
            mockAspNet.SetIsPostBack(true);

            TestDriverPage page = new TestDriverPage(mockAspNet);
            page.ShowConsoleCheckBox = new CheckBox();
            page.ShowConsoleCheckBox.Checked = false;

            page.TestsPanel = new Panel();
            page.FooterPanel = new Panel();

            page.OnLoadCompleteInternal();
            UnitTestAssert.AreEqual("testsNoConsole", page.TestsPanel.CssClass);
            UnitTestAssert.IsFalse(page.FooterPanel.Visible);
        }

        [TestMethod]
        public void OnInit_NoLocators()
        {
            ServiceLocator.ApplicationPathFinder = null;
            ServiceLocator.BrowserCommandExecutorFactory = null;
            MockAspNetService mockAspNet = new MockAspNetService();
            mockAspNet.SetIsPostBack(true);

            TestDriverPage page = new TestDriverPage(mockAspNet);
            page.TestCasesTreeView = new System.Web.UI.WebControls.TreeView();
            page.AutoGenerateInterface = false;
            page.OnInitInternal();

            UnitTestAssert.IsNull(ServiceLocator.ApplicationPathFinder);
            UnitTestAssert.IsNull(ServiceLocator.BrowserCommandExecutorFactory);
        }

        [TestMethod]
        public void OnInit_Locators()
        {
            ServiceLocator.ApplicationPathFinder = null;
            ServiceLocator.BrowserCommandExecutorFactory = null;
            MockAspNetService mockAspNet = new MockAspNetService();
            mockAspNet.SetIsPostBack(false);
            mockAspNet.SetApplicationPath("foo");

            TestDriverPage page = new TestDriverPage(mockAspNet);
            page.TestCasesTreeView = new System.Web.UI.WebControls.TreeView();
            page.AutoGenerateInterface = false;
            page.OnInitInternal();

            UnitTestAssert.IsInstanceOfType(ServiceLocator.ApplicationPathFinder, typeof(ApplicationPathFinder));
            UnitTestAssert.IsInstanceOfType(ServiceLocator.BrowserCommandExecutorFactory, typeof(BrowserCommandExecutorFactory));
        }

        [TestMethod]
        public void OnInit_NoUI()
        {
            MockAspNetService mockAspNet = new MockAspNetService();
            mockAspNet.SetIsPostBack(false);

            TestDriverPage page = new TestDriverPage(mockAspNet);
            page.TestCasesTreeView = new System.Web.UI.WebControls.TreeView();
            page.AutoGenerateInterface = false;
            page.OnInitInternal();

            UnitTestAssert.IsNull(page.ThreadIdLabel);
        }

        [TestMethod]
        public void GenerateInterfaceCreatesPlaceHolder()
        {
            MockAspNetService mockAspNet = new MockAspNetService();
            PlaceHolder holder = new PlaceHolder();
            mockAspNet.FindControlResults.Add("DriverPageContentPlaceHolder", holder);
            
            TestDriverPage page = new TestDriverPage(mockAspNet);
            page.GenerateInterface();

            UnitTestAssert.AreSame(holder, page.ContentPlaceHolder);
        }

        [TestMethod]
        public void GenerateInterfaceCreatesHeaderDiv()
        {
            MockAspNetService mockAspNet = new MockAspNetService();
            mockAspNet.FindControlResults.Add("DriverPageContentPlaceHolder", new PlaceHolder());

            TestDriverPage page = new TestDriverPage(mockAspNet);

            page.GenerateInterface();

            Panel control = (Panel)page.ContentPlaceHolder.Controls[0];

            UnitTestAssert.AreEqual("Header", control.ID);
        }

		[TestMethod]
		public void GenerateInterfaceCreatesTitleDiv()
		{
			MockAspNetService mockAspNet = new MockAspNetService();
			mockAspNet.FindControlResults.Add("DriverPageContentPlaceHolder", new PlaceHolder());

			TestDriverPage page = new TestDriverPage(mockAspNet);

			page.GenerateInterface();

			Panel control = (Panel)page.ContentPlaceHolder.Controls[0].Controls[0];

			UnitTestAssert.AreEqual("title", control.ID);
		}

		[TestMethod]
		public void GenerateInterfaceCreatesHelpIcon()
		{
			MockAspNetService mockAspNet = new MockAspNetService();
			mockAspNet.FindControlResults.Add("DriverPageContentPlaceHolder", new PlaceHolder());

			TestDriverPage page = new TestDriverPage(mockAspNet);

			page.GenerateInterface();

			Image control = (Image)page.ContentPlaceHolder.Controls[0].Controls[0].Controls[1];
			UnitTestAssert.AreEqual("help", control.ID);
		}

        [TestMethod]
        public void GenerateInterfaceCreatesMenuPanel()
        {
            MockAspNetService mockAspNet = new MockAspNetService();
            mockAspNet.FindControlResults.Add("DriverPageContentPlaceHolder", new PlaceHolder());

            TestDriverPage page = new TestDriverPage(mockAspNet);

            page.GenerateInterface();

            Panel control = (Panel)page.ContentPlaceHolder.Controls[0].Controls[1];
            UnitTestAssert.AreEqual("MenuPanel", control.ID);
        }

        [TestMethod]
        public void GenerateInterfaceCreatesWriteToLogCheckbox()
        {
            MockAspNetService mockAspNet = new MockAspNetService();
            mockAspNet.FindControlResults.Add("DriverPageContentPlaceHolder", new PlaceHolder());

            TestDriverPage page = new TestDriverPage(mockAspNet);

            page.GenerateInterface();

            CheckBox control = (CheckBox)page.ContentPlaceHolder.Controls[0].Controls[1].Controls[0];
            UnitTestAssert.AreEqual("WriteLogCheckBox", control.ID);
        }

        [TestMethod]
        public void GenerateInterfaceCreatesShowConsoleCheckbox()
        {
            MockAspNetService mockAspNet = new MockAspNetService();
            mockAspNet.FindControlResults.Add("DriverPageContentPlaceHolder", new PlaceHolder());

            TestDriverPage page = new TestDriverPage(mockAspNet);

            page.GenerateInterface();

            CheckBox control = (CheckBox)page.ContentPlaceHolder.Controls[0].Controls[1].Controls[2];
            UnitTestAssert.AreEqual("ShowConsoleCheckBox", control.ID);
        }

        [TestMethod]
        public void GenerateInterfaceCreatesErrorsLabel()
        {
            MockAspNetService mockAspNet = new MockAspNetService();
            mockAspNet.FindControlResults.Add("DriverPageContentPlaceHolder", new PlaceHolder());

            TestDriverPage page = new TestDriverPage(mockAspNet);

            page.GenerateInterface();

            Label control =(Label)page.ContentPlaceHolder.Controls[0].Controls[1].Controls[4];
            UnitTestAssert.AreEqual("ErrorsLabel", control.ID);
        }

        [TestMethod]
        public void GenerateInterfaceCreatesButtonPanel()
        {
            MockAspNetService mockAspNet = new MockAspNetService();
            mockAspNet.FindControlResults.Add("DriverPageContentPlaceHolder", new PlaceHolder());

            TestDriverPage page = new TestDriverPage(mockAspNet);

            page.GenerateInterface();

            Panel control = (Panel)page.ContentPlaceHolder.Controls[0].Controls[1].Controls[6];
            UnitTestAssert.AreEqual("ButtonPanel", control.ID);
        }

        [TestMethod]
        public void GenerateInterfaceCreatesButton()
        {
            MockAspNetService mockAspNet = new MockAspNetService();
            mockAspNet.FindControlResults.Add("DriverPageContentPlaceHolder", new PlaceHolder());

            TestDriverPage page = new TestDriverPage(mockAspNet);

            page.GenerateInterface();

            Button control = (Button)page.ContentPlaceHolder.Controls[0].Controls[1].Controls[6].Controls[0];
            UnitTestAssert.AreEqual("RunTestcasesButton", control.ID);
        }

		[TestMethod]
		public void GenerateInterfaceCreatesRunFailedTestsButton()
		{
			MockAspNetService mockAspNet = new MockAspNetService();
			mockAspNet.FindControlResults.Add("DriverPageContentPlaceHolder", new PlaceHolder());

			TestDriverPage page = new TestDriverPage(mockAspNet);

			page.GenerateInterface();

			Button control = (Button)page.ContentPlaceHolder.Controls[0].Controls[1].Controls[6].Controls[1];
			UnitTestAssert.AreEqual("failedTests", control.ID);
		}

        [TestMethod]
        public void GenerateInterfaceCreatesSpinnerImage()
        {
            MockAspNetService mockAspNet = new MockAspNetService();
            mockAspNet.FindControlResults.Add("DriverPageContentPlaceHolder", new PlaceHolder());

            TestDriverPage page = new TestDriverPage(mockAspNet);

            page.GenerateInterface();

            //headerDiv.MenuPanel.ButtonPanel
            HtmlImage control = (HtmlImage)page.ContentPlaceHolder.Controls[0].Controls[1].Controls[6].Controls[3];
            UnitTestAssert.AreEqual("spinner", control.ID);
        }

        [TestMethod]
        public void GenerateInterfaceCreatesThreadLabel()
        {
            MockAspNetService mockAspNet = new MockAspNetService();
            mockAspNet.FindControlResults.Add("DriverPageContentPlaceHolder", new PlaceHolder());

            TestDriverPage page = new TestDriverPage(mockAspNet);

            page.GenerateInterface();

            Label control = (Label)page.ContentPlaceHolder.Controls[0].Controls[1].Controls[6].Controls[5];
            UnitTestAssert.AreEqual("ThreadId", control.ID);
        }

		[TestMethod]
		public void GenerateInterfaceCreatesTestsDiv()
		{
			MockAspNetService mockAspNet = new MockAspNetService();
			mockAspNet.FindControlResults.Add("DriverPageContentPlaceHolder", new PlaceHolder());

			TestDriverPage page = new TestDriverPage(mockAspNet);

			page.GenerateInterface();

			Panel control = (Panel)page.ContentPlaceHolder.Controls[1];
			UnitTestAssert.AreEqual("Tests", control.ID);
            UnitTestAssert.AreEqual("tests", control.CssClass);
		}

        [TestMethod]
        public void GenerateInterfaceCreatesTreeView()
        {
            MockAspNetService mockAspNet = new MockAspNetService();
            mockAspNet.FindControlResults.Add("DriverPageContentPlaceHolder", new PlaceHolder());

            TestDriverPage page = new TestDriverPage(mockAspNet);

            page.GenerateInterface();

            TreeView control = (TreeView)page.ContentPlaceHolder.Controls[1].Controls[0];
            UnitTestAssert.AreEqual("testcasesTreeView", control.ID);
        }

		[TestMethod]
		public void TestcasesTreeView_LoadAddsRootNode()
		{
			MockAspNetService mockAspNet = new MockAspNetService();
			TestDriverPage page = new TestDriverPage(mockAspNet);
			page.TestCasesTreeView = new TreeView();
			page.TestcasesTreeview_LoadInternal();
			UnitTestAssert.AreEqual(1, page.TestCasesTreeView.Nodes.Count);
			UnitTestAssert.AreEqual("All Test Cases", page.TestCasesTreeView.Nodes[0].Text);
		}

		[TestMethod]
		public void TestcasesTreeView_LoadSelectsNodesIfTargetIsTreeview()
		{
			MockAspNetService mockAspNet = new MockAspNetService();
			NameValueCollection form = new NameValueCollection();
			form.Add("__EVENTTARGET", null);
			mockAspNet.SetRequestForm(form);
			mockAspNet.SetIsPostBack(true);

			TestDriverPage page = new TestDriverPage(mockAspNet);
			page.TestCasesTreeView = new TreeView();
			TreeNode rootNode = new TreeNode("Foobar");
			rootNode.Checked = true;
			page.TestCasesTreeView.Nodes.Add(rootNode);
			page.TestcasesTreeview_LoadInternal();

			UnitTestAssert.IsTrue(rootNode.Checked);
		}

		[TestMethod]
		public void GenerateInterfaceCreatesFooterDiv()
		{
			MockAspNetService mockAspNet = new MockAspNetService();
			mockAspNet.FindControlResults.Add("DriverPageContentPlaceHolder", new PlaceHolder());

			TestDriverPage page = new TestDriverPage(mockAspNet);

			page.GenerateInterface();

			Panel control = (Panel)page.ContentPlaceHolder.Controls[2];
			UnitTestAssert.AreEqual("Footer", control.ID);
		}

        [TestMethod]
        public void GenerateInterfaceCreatesCommandsLabel()
        {
            MockAspNetService mockAspNet = new MockAspNetService();
            mockAspNet.FindControlResults.Add("DriverPageContentPlaceHolder", new PlaceHolder());

            TestDriverPage page = new TestDriverPage(mockAspNet);

            page.GenerateInterface();

            Label control = (Label)page.ContentPlaceHolder.Controls[2].Controls[0];
            UnitTestAssert.AreEqual("Commands", control.ID);
        }

        [TestMethod]
        public void GenerateInterfaceCreatesCommandsArea()
        {
            MockAspNetService mockAspNet = new MockAspNetService();
            mockAspNet.FindControlResults.Add("DriverPageContentPlaceHolder", new PlaceHolder());

            TestDriverPage page = new TestDriverPage(mockAspNet);

            page.GenerateInterface();

            Panel control = (Panel)page.ContentPlaceHolder.Controls[2].Controls[2];
            UnitTestAssert.AreEqual("TraceConsole", control.ID);
        }

		[TestMethod]
		public void GenerateInterfaceRegistersScriptBlock()
		{
			MockAspNetService mockAspNet = new MockAspNetService();
			mockAspNet.FindControlResults.Add("DriverPageContentPlaceHolder", new PlaceHolder());

			TestDriverPage page = new TestDriverPage(mockAspNet);

			page.GenerateInterface();

			UnitTestAssert.AreEqual(1, mockAspNet.ClientScriptBlocks.Count);
		}
    }
}
