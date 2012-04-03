using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.Services;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Drawing;
using System.Collections.Specialized;
using System.Web.Script.Services;
using LTAF;


namespace LTAF.Engine
{

    /// <summary>
    /// TestDriverPage
    /// </summary>
    [GenerateScriptType(typeof(PopupAction))]
    public class TestDriverPage : Page
    {
        private IAspNetPageService _aspNetService = null;
        private static SystemWebExtensionsWrapper _systemWebExtensionsAbstractions;
        private static TestCaseManager _testcaseManager;

        /// <summary>TestcaseTypes</summary>
        protected static Dictionary<string, Type> _testcaseTypes;


        private TestcaseExecutor _testcaseExecutor = null;
        private bool _generateInterface = true;
        private TreeView _testcasesTreeview;
        private PlaceHolder _contentPlaceHolder;
        private Label _errorsLabel;
        private Label _threadId;

        #region Constructor
        /// <summary>
        /// ctor
        /// </summary>
        public TestDriverPage()
            : this(new AspNetPageService())
        {
        }

        internal TestDriverPage(IAspNetPageService aspNetService)
        {
            _aspNetService = aspNetService;
            if (_systemWebExtensionsAbstractions == null)
            {
                _systemWebExtensionsAbstractions = new SystemWebExtensionsWrapper(aspNetService);
            }
            if (_testcaseManager == null)
            {
                _testcaseManager = new TestCaseManager();
            }
        }

        internal TestDriverPage(IAspNetPageService aspNetService, SystemWebExtensionsWrapper sweWrapper): this(aspNetService)
        {
            _systemWebExtensionsAbstractions = sweWrapper;
        }
        #endregion

        #region Page Controls

        /// <summary>
        /// Checkbox that indicates whether to write the log to disk when test run is finished
        /// </summary>
        protected internal CheckBox WriteLogToDiskCheckBox
        {
            get;
            set;
        }

        /// <summary>
        /// Checkbox that indicates whether to show the testcase console
        /// </summary>
        protected internal CheckBox ShowConsoleCheckBox
        {
            get;
            set;
        }

        /// <summary>
        /// Panel that represents the middle area of the driver UI
        /// </summary>
        protected internal Panel TestsPanel
        {
            get;
            set;
        }

        /// <summary>
        /// Panel that represents the footer area of the driver UI
        /// </summary>
        protected internal Panel FooterPanel
        {
            get;
            set;
        }


        /// <summary>
        /// Reference to the place holder used to generate all the page's content
        /// </summary>
        protected internal PlaceHolder ContentPlaceHolder
        {
            get
            {
                if (_contentPlaceHolder == null)
                {
                    _contentPlaceHolder = _aspNetService.FindControl(this, "DriverPageContentPlaceHolder") as PlaceHolder;
                }
                return _contentPlaceHolder;
            }
            set { _contentPlaceHolder = value; }
        }

        /// <summary>
        /// Reference to the TreeView containing the test cases
        /// </summary>
        protected internal TreeView TestCasesTreeView
        {
            get { return _testcasesTreeview; }
            set { _testcasesTreeview = value; }
        }

        /// <summary>
        /// Reference to the label that displays the ThreadId
        /// </summary>
        protected internal Label ThreadIdLabel
        {
            get { return _threadId; }
            set { _threadId = value; }
        }

        /// <summary>
        /// Reference to the label that displays errors
        /// </summary>
        protected Label Errors
        {
            get { return _errorsLabel; }
            set { _errorsLabel = value; }
        }

        #endregion

        #region Properties
        /// <summary>
        /// Reference to a class that abstract the use of types/methods inside System.Web.Extensions
        /// </summary>
        internal static SystemWebExtensionsWrapper SystemWebExtensionsAbstractions
        {
            get
            {
                return _systemWebExtensionsAbstractions;
            }
            set
            {
                _systemWebExtensionsAbstractions = value;
            }
        }

        /// <summary>
        /// Whether the test interface should be generated automatically.
        /// This must be set before OnInit to take effect.
        /// </summary>
        protected internal bool AutoGenerateInterface
        {
            get { return _generateInterface; }
            set { _generateInterface = value; }
        }

        /// <summary>
        /// The testcase executor object to be used by the driver page
        /// </summary>
        protected TestcaseExecutor TestcaseExecutor
        {
            get
            {
                if (_testcaseExecutor == null)
                {
                    _testcaseExecutor = new TestcaseExecutor();
                }
                return _testcaseExecutor;
            }
            set
            {
                _testcaseExecutor = value;
            }
        }

        /// <summary>
        /// Whether logging should be detailed or rudimentary
        /// </summary>
        public WebTestLogDetail LogDetail
        {
            get
            {
                object result = ViewState["LogDetail"];
                return result == null ? WebTestLogDetail.Default : (WebTestLogDetail) result;
            }
            set { ViewState["LogDetail"] = value; }
        }
        #endregion

        /// <summary>
        /// OnInit
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            OnInitInternal();
        }

        /// <summary>
        /// Sets up the service locators and the UI
        /// </summary>
        internal void OnInitInternal()
        {
            SystemWebExtensionsAbstractions.Initialize(this);

            if (!_aspNetService.GetIsPostBack(this))
            {
                _testcaseTypes = new Dictionary<string, Type>();
                SetupServiceLocator(_aspNetService.GetApplicationPath(this));
            }

            if (_generateInterface)
            {
                GenerateInterface();
            }

            _testcasesTreeview.Load += new EventHandler(TestcasesTreeview_Load);
        }

        /// <summary>
        /// Sets up the global ServiceLocator
        /// </summary>
        private void SetupServiceLocator(string applicationPath)
        {
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder(applicationPath);
            ServiceLocator.BrowserCommandExecutorFactory = new BrowserCommandExecutorFactory();
        }

        /// <summary>
        /// OnLoadComplete
        /// </summary>
        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);
            OnLoadCompleteInternal();
        }

        internal void OnLoadCompleteInternal()
        {
            // Only auto-select or auto-run on the first request
            if (!_aspNetService.GetIsPostBack(this))
            {
                // Get query string values
                QueryStringParameters queryParams = new QueryStringParameters();
                queryParams.LoadFromQueryString(_aspNetService.GetQueryString(this));

                BrowserVersions browser = BrowserUtility.GetBrowser(_aspNetService.GetBrowserName(this), _aspNetService.GetBrowserMajorVersion(this));

                LogDetail = queryParams.LogDetail;
                this.WriteLogToDiskCheckBox.Checked = queryParams.WriteLog;
                this.ShowConsoleCheckBox.Checked = queryParams.ShowConsole;

                // Auto-select tests with a given tag
                if (!String.IsNullOrEmpty(queryParams.Tag))
                {
                    _testcaseManager.SelectTaggedTests(this.TestCasesTreeView, queryParams.Tag, browser);
                }
                else if (queryParams.Run)
                {
                    _testcaseManager.SelectAllTests(this.TestCasesTreeView, browser);
                }

                // Optionally remove unselected tests
                if (queryParams.Filter)
                {
                    _testcaseManager.FilterIgnoredTests(this.TestCasesTreeView);
                }

                // Auto-run the selected tests (or all tests if none are selected)
                if (queryParams.Run)
                {
                    RunTestCases();
                }
            }

            // Change UI depending if showing console or not.
            if (this.ShowConsoleCheckBox.Checked)
            {
                this.TestsPanel.CssClass = "tests";
                this.FooterPanel.Visible = true;
            }
            else
            {
                this.TestsPanel.CssClass = "testsNoConsole";
                this.FooterPanel.Visible = false;
            }
        }

        /// <summary>
        /// Creates the default UI
        /// </summary>
        internal void GenerateInterface()
        {
            PlaceHolder mainContent = ContentPlaceHolder;
            //Create Header Panel
            Panel headerDiv = new Panel();
            headerDiv.ID = "Header";
            headerDiv.CssClass = "header";
            mainContent.Controls.Add(headerDiv);

            //Create Test Panel
            this.TestsPanel = new Panel();
            this.TestsPanel.ID = "Tests";
            this.TestsPanel.CssClass = "tests";
            mainContent.Controls.Add(this.TestsPanel);

            //Create Footer Panel
            this.FooterPanel = new Panel();
            this.FooterPanel.ID = "Footer";
            this.FooterPanel.CssClass = "footer";
            mainContent.Controls.Add(this.FooterPanel);

            //Add Title bar to Header
            Panel titleDiv = new Panel();
            titleDiv.ID = "title";
            titleDiv.CssClass = "titleBar";

            //Add Help to Title
            System.Web.UI.WebControls.Image helpImage = new System.Web.UI.WebControls.Image();
            helpImage.ID = "help";
            helpImage.CssClass = "helpIcon";
            helpImage.ImageUrl = _aspNetService.GetClientScriptWebResourceUrl(this, typeof(TestDriverPage), "LTAF.Engine.Resources.helpicon.gif");
            string helpPageUrl = _aspNetService.GetClientScriptWebResourceUrl(this, typeof(TestDriverPage), "LTAF.Engine.Resources.HelpPage.htm");
            titleDiv.Controls.Add(new LiteralControl("<a href=\"#\" onclick=\"javascript:window.open('" + helpPageUrl + "','Help','toolbar=0, height=500, width=700, scrollbars=yes, resizable=yes');\" style=\"text-decoration: none\" title=\"help (new window)\">"));
            titleDiv.Controls.Add(helpImage);
            titleDiv.Controls.Add(new LiteralControl("</a>"));

            //Add Text to Title
            titleDiv.Controls.Add(new LiteralControl("<b>Lightweight Test Automation Framework</b>"));
            headerDiv.Controls.Add(titleDiv);

            //Add the Menu to the Header
            Panel menuPanel = new Panel();
            menuPanel.ID = "MenuPanel";
            menuPanel.CssClass = "menu";
            headerDiv.Controls.Add(menuPanel);

            //Add Write to Disk Option to Menu
            this.WriteLogToDiskCheckBox = new CheckBox();
            this.WriteLogToDiskCheckBox.ID = "WriteLogCheckBox";
            this.WriteLogToDiskCheckBox.Text = "Write Log to Disk";
            menuPanel.Controls.Add(this.WriteLogToDiskCheckBox);
            menuPanel.Controls.Add(new LiteralControl("&nbsp;&nbsp;&nbsp;"));

            //Add Show Test Console Option to Menu
            this.ShowConsoleCheckBox = new CheckBox();
            this.ShowConsoleCheckBox.ID = "ShowConsoleCheckBox";
            this.ShowConsoleCheckBox.Checked = true;
            this.ShowConsoleCheckBox.AutoPostBack = true;
            this.ShowConsoleCheckBox.Text = "Show Console";
            menuPanel.Controls.Add(this.ShowConsoleCheckBox);
            menuPanel.Controls.Add(new LiteralControl("<br />"));

            //Add Error Console to Menu
            _errorsLabel = new Label();
            _errorsLabel.ID = "ErrorsLabel";
            _errorsLabel.ForeColor = Color.Red;
            _errorsLabel.Font.Bold = true;
            menuPanel.Controls.Add(_errorsLabel);
            menuPanel.Controls.Add(new LiteralControl("<br />"));

            //Add Control Buttons to Menu
            Panel buttonPanel = new Panel();
            buttonPanel.ID = "ButtonPanel";
            buttonPanel.HorizontalAlign = HorizontalAlign.Center;
            menuPanel.Controls.Add(buttonPanel);

            //Add Run Button to Button Menu
            Button runButton = new Button();
            runButton.ID = "RunTestcasesButton";
            runButton.Text = "Run Tests";
            runButton.Click += new EventHandler(RunTestcasesButton_Click);
            runButton.OnClientClick = "return prepareClientUI()";
            buttonPanel.Controls.Add(runButton);
            headerDiv.DefaultButton = runButton.ID;

            //Add Run Failed Button to Button Menu
            Button runFailedButton = new Button();
            runFailedButton.ID = "failedTests";
            runFailedButton.Text = "Run Failed Tests";
            runFailedButton.OnClientClick = "RerunFailedTests();return false;";
            runFailedButton.Attributes.Add("disabled", "disabled");
            buttonPanel.Controls.Add(runFailedButton);

            //Add Progress Spinner to Button Menu
            HtmlImage spinnerImage = new HtmlImage();
            spinnerImage.ID = "spinner";
            spinnerImage.Alt = ".";
            spinnerImage.Width = 12;
            spinnerImage.Height = 12;
            spinnerImage.Style.Add("visibility", "hidden");
            spinnerImage.Src = _aspNetService.GetClientScriptWebResourceUrl(this, typeof(TestDriverPage), "LTAF.Engine.Resources.spinner.gif");
            buttonPanel.Controls.Add(new LiteralControl("<br />"));
            buttonPanel.Controls.Add(spinnerImage);

            //Add Thread Label to Button Menu
            _threadId = new Label();
            _threadId.ID = "ThreadId";
            _threadId.EnableViewState = false;
            _threadId.Font.Italic = true;
            buttonPanel.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
            buttonPanel.Controls.Add(_threadId);

            //Add Test Case Treeview
            _testcasesTreeview = new TreeView();
            _testcasesTreeview.ID = "testcasesTreeView";
            _testcasesTreeview.Font.Size = FontUnit.Small;
            _testcasesTreeview.Font.Names = new string[3] { "Franklin", "Gothic", "Medium" };
            _testcasesTreeview.ShowCheckBoxes = TreeNodeTypes.Leaf;
            this.TestsPanel.Controls.Add(_testcasesTreeview);

            //Add Commands Label to Footer
            Label commandsLabel = new Label();
            commandsLabel.AssociatedControlID = "TraceConsole";
            commandsLabel.ID = "Commands";
            commandsLabel.Text = "Console: ";
            this.FooterPanel.Controls.Add(commandsLabel);

            //Add Debug Console to Footer
            Panel debugArea = new Panel();
            debugArea.ID = "TraceConsole";
            debugArea.CssClass = "debugConsole";
            this.FooterPanel.Controls.Add(new LiteralControl("<br />"));
            this.FooterPanel.Controls.Add(debugArea);

            string scriptBlock = @"
                function prepareClientUI()
                {
                    document.getElementById('ThreadId').innerHTML = '[Loading Tests. Please Wait.]';
                    return true;
                }";
            _aspNetService.RegisterClientScriptBlock(this, typeof(Page), "BeforeTestExecution", scriptBlock, true);
        }

        private void TestcasesTreeview_Load(object sender, EventArgs e)
        {
            TestcasesTreeview_LoadInternal();   
        }

        internal void TestcasesTreeview_LoadInternal()
        {
            if (!_aspNetService.GetIsPostBack(this))
            {
                // Load the test case tree nodes in alphabetical order and store them in ViewState
                TreeNode root = null;
                if (_testcasesTreeview.Nodes.Count > 0)
                {
                    root = _testcasesTreeview.Nodes[0];
                }
                else
                {
                    root = new TreeNode("All Test Cases");
                    _testcasesTreeview.Nodes.Add(root);
                }
                _testcaseManager.PopulateTreeView(root);
            }
            else if (_aspNetService.GetRequestForm(this)["__EVENTTARGET"] == _testcasesTreeview.ClientID)
            {
                // if post  came from treeview, then we toggle checkbox selection
                if (_testcasesTreeview.SelectedNode != null)
                {
                    bool select = !IsChildNodeChecked(_testcasesTreeview.SelectedNode);
                    UpdateChildNodes(_testcasesTreeview.SelectedNode, select);
                }
            }
        }

        private bool IsChildNodeChecked(TreeNode parentNode)
        {
            foreach (TreeNode node in parentNode.ChildNodes)
            {
                if (node.Checked || IsChildNodeChecked(node))
                {
                    return true;
                }
            }
            return false;
        }

        private void UpdateChildNodes(TreeNode parentNode, bool select)
        {
            foreach (TreeNode node in parentNode.ChildNodes)
            {
                node.Checked = select;
                UpdateChildNodes(node, select);
            }
        }

        private void RunTestcasesButton_Click(object sender, EventArgs e)
        {
            RunTestCases();
        }

        /// <summary>
        /// RunTestCases
        /// </summary>
        protected virtual void RunTestCases()
        {
            IList<Testcase> testsToExecute = _testcaseManager.GetSelectedTestCases(_testcasesTreeview);

            if (WriteLogToDiskCheckBox.Checked)
            {
                string testPath = Server.MapPath("");
                TestcaseLogger logger = new TestcaseLogger(testPath, this.TestcaseExecutor);

                // verify user has write access to folder
                try
                {
                    logger.WriteStartupFile();
                }
                catch (UnauthorizedAccessException)
                {
                    _errorsLabel.Text = String.Format("Error: Write to log feature requires write access to '{0}' directory.", testPath);
                    return;
                }               
            }

            // Get a testcase executor and start
            int threadId = TestcaseExecutor.Start(testsToExecute, Request);
            _threadId.Text = "[Current ThreadId: " + threadId.ToString() + "]";

            string startupScript = @"
            TestExecutor.set_threadId(" + threadId + @");
            TestExecutor.set_logDetail(" + ((int) LogDetail) + @");
            TestExecutor.TestcaseExecutedCallback = TreeView_TestcaseExecuted;
            TestExecutor.start();";

            SystemWebExtensionsAbstractions.RegisterStartupScript(this, typeof(Page), "LTAF.Startup", startupScript, true);
        }

        /// <summary>
        /// GetCommand
        /// </summary>
        [WebMethod]
        public static BrowserCommand GetCommand(int threadId, BrowserInfo resultOfLastCommand)
        {
            return CommandManager.SetResultAndGetNextCommand(threadId, resultOfLastCommand);
        }
    }
}