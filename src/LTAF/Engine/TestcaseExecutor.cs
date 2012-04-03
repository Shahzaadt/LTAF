using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;

namespace LTAF.Engine
{
    /// <summary>
    /// Class that is in charge of starting tests and monitoring them
    /// </summary>
    public class TestcaseExecutor
    {
        private IList<Testcase> _testcases;
        private int _threadId = -1;
        private IDictionary<string, string> _requestDetails;

        /// <summary>Event raised after a testcase has been executed</summary>
        public event EventHandler<TestcaseExecutionEventArgs> TestcaseExecuted;

        /// <summary>Event raised before a testcase begins execution</summary>
        public event EventHandler<TestcaseExecutionEventArgs> TestcaseExecuting;

        /// <summary>Event raised after a test run completes</summary>
        public event EventHandler<TestRunFinishedEventArgs> TestRunFinished;

        /// <summary>Event raised after a test run is started</summary>
        public event EventHandler TestRunStarted;

        /// <summary>
        /// TestcaseExecutor
        /// </summary>
        public TestcaseExecutor()
        {
        }

        /// <summary>
        /// Setup
        /// </summary>
        protected void Setup(IList<Testcase> testcases, HttpRequest request)
        {
            _testcases = testcases;

            _requestDetails = new Dictionary<string, string>();
            foreach (string key in request.Params.Keys)
            {
                _requestDetails.Add(key, request.Params[key]);
            }
        }

        /// <summary>
        /// Start
        /// </summary>
        public int Start(IList<Testcase> testcases, HttpRequest request)
        {
            Setup(testcases, request);

            TestcaseExecutorTaskInfo taskInfo = new TestcaseExecutorTaskInfo(_testcases, new BrowserQueueCreated(QueueCreated), _requestDetails);
            WaitCallback methodTarget = new WaitCallback(RunTestsInternal);
            bool isQueued = ThreadPool.QueueUserWorkItem(methodTarget, taskInfo);

            while (_threadId == -1)
            {
                Thread.Sleep(200);
            }

            return _threadId;
        }

        private void QueueCreated(int threadId)
        {
            _threadId = threadId;
        }

        private void RunTestsInternal(object state)
        {
            TestcaseExecutorTaskInfo taskInfo = (TestcaseExecutorTaskInfo)state;
            int threadId = Thread.CurrentThread.ManagedThreadId;

            // create a browser queue with this thread id
            CommandManager.CreateBrowserQueue(threadId);

            //notify the page that we just created a browser queue and send the thread id
            taskInfo.NotifyBrowserQueueCreated(threadId);

            BrowserCommand command = new BrowserCommand(BrowserCommand.FunctionNames.TestRunStarted);
            command.Description = "Test Run Started";
            command.Handler.RequiresElementFound = false;
            command.Handler.SetArguments(_testcases.Count);
            CommandManager.ExecuteCommand(threadId, command, 10);

            OnTestRunStarted(new EventArgs());

            foreach (Testcase testcase in taskInfo.Testcases)
            {
                testcase.TestcaseExecuting += new EventHandler<TestcaseExecutionEventArgs>(testcase_TestcaseExecuting);
                testcase.TestcaseExecuted += new EventHandler<TestcaseExecutionEventArgs>(testcase_TestcaseExecuted);
                
                // [05/05/2009] clear the traces that might have been left from a previous test.
                WebTestConsole.Clear();

                try
                {
                    testcase.Execute();
                }
                catch (TimeoutException)
                {
                    // Abort the test run if we receive a TimeoutException because
                    // this likely means the browser has been closed and we don't
                    // want the server to keep sending test commands and timing out
                    // for each of them.
                    return;
                }
            }

            command = new BrowserCommand(BrowserCommand.FunctionNames.TestRunFinished);
            command.Description = "Test Run Finished";
            command.Handler.RequiresElementFound = false;
            BrowserInfo result = CommandManager.ExecuteCommand(threadId, command, 10);
            
            // Provide the results of the test run to anyone interested
            string log = result != null ? result.Data : null;
            taskInfo.RequestDetails.Add("TestLog", log);
            
            OnTestRunFinished(new TestRunFinishedEventArgs(taskInfo.Testcases, taskInfo.RequestDetails));
        }

        void testcase_TestcaseExecuted(object sender, TestcaseExecutionEventArgs e)
        {
            OnTestcaseExecuted(e);

            string errorMessage = e.Passed ? null : FormatErrorMessage(e);

            BrowserCommand command = new BrowserCommand(BrowserCommand.FunctionNames.TestcaseExecuted);
            command.Description = "Testcase Finished (" + e.WebTestName + ")";
            command.Handler.SetArguments(e.WebTestName, e.Passed, errorMessage);
            command.Handler.RequiresElementFound = false;
            CommandManager.ExecuteCommand(_threadId, command, 5);
        }

        void testcase_TestcaseExecuting(object sender, TestcaseExecutionEventArgs e)
        {
            OnTestcaseExecuting(e);

            BrowserCommand command = new BrowserCommand(BrowserCommand.FunctionNames.TestcaseExecuting);
            command.Description = "Testcase Starting ("+ e.WebTestName +")";
            command.Handler.SetArguments(e.WebTestName);
            command.Handler.RequiresElementFound = false;
            CommandManager.ExecuteCommand(_threadId, command, 5);
        }

        /// <summary>
        /// Create an error message for a failed test case
        /// </summary>
        /// <param name="e">Information about the failure including the exception and test case name</param>
        /// <returns>HTML for the error message</returns>
        private string FormatErrorMessage(TestcaseExecutionEventArgs e)
        {
            // Make sure we have the necessary information to create the error message
            if (e == null || e.Exception == null || e.Passed)
                return null;

            string testName = HttpUtility.HtmlEncode(e.WebTestName);
            string message = HttpUtility.HtmlEncode(e.Exception.Message);
            
            // Generated HTML markup displaying the error message
            StringBuilder html = new StringBuilder();

            // Add the header
            html.Append("<div style=\"font-family: 'Verdana'; font-size: 8.5pt; color: black; background-color: white; width: 100%;\">");
            html.Append("<span><h1 style=\"font-size: 18pt; color: red; font-weight: normal;\">Error in Test");
            if (!string.IsNullOrEmpty(testName))
                html.AppendFormat(" '{0}'", testName);
            html.Append("<hr width=\"100%\" size=\"1\" color=\"silver\" /></h1>");

            // Add the error message
            html.Append("<h2 style=\"font-size: 14pt; color: maroon; font-weight: normal;\"><i>");
            html.Append(message);
            html.Append("</i></h2></span>");

            // Add the exception details
            html.Append("<font face=\"Arial, Helvetica, Geneva, SunSans-Regular, sans-serif\">");
            html.Append("<b style=\"margin-top: -5px;\">Exception Details: </b>");
            html.AppendFormat("{0}: {1}", HttpUtility.HtmlEncode(e.Exception.GetType().FullName), message);
            html.Append("<br /><br />");

            // Add the source code (if available)
            string fileName = null;
            int sourceLine = 0;
            string methodName = null;
            string sourceCode = GetSourceCode(e.Exception, out fileName, out sourceLine, out methodName);
            if (!string.IsNullOrEmpty(sourceCode))
            {
                html.Append("<b style=\"margin-top: -5px;\">Source Error:</b> <br><br>");
                html.Append("<table width=\"100%\" bgcolor=\"#ffffcc\"><tr><td>");
                html.AppendLine("<code><pre style=\"font-family: 'Lucida Console'; font-size: 9pt;\">");
                html.AppendLine();
                html.Append(sourceCode);
                html.Append("</pre></code></td></tr></table><br />");
                html.Append("<b style=\"margin-top: -5px;\">Source File: </b> ");
                html.Append(fileName);
                html.Append("<b style=\"margin-top: -5px;\"> &nbsp;&nbsp; Line: </b> ");
                html.Append(sourceLine);
                html.Append("<b style=\"margin-top: -5px;\"> &nbsp;&nbsp; Method: </b> ");
                html.Append(methodName);
                html.Append("<br /><br />");
            }

            // Add the stack trace
            html.Append("<b style=\"margin-top: -5px;\">Stack Trace:</b> <br><br>");
            html.Append("<table width=\"100%\" bgcolor=\"#ffffcc\"><tr><td>");
            html.AppendLine("<code><pre style=\"font-family: 'Lucida Console'; font-size: 9pt;\">");
            for (Exception ex = e.Exception; ex != null; ex = ex.InnerException)
            {
                html.AppendLine();
                html.AppendFormat("[{0}: {1}]", HttpUtility.HtmlEncode(ex.GetType().FullName), HttpUtility.HtmlEncode(ex.Message));
                html.AppendLine();
                html.AppendLine(HttpUtility.HtmlEncode(ex.StackTrace));
            }
            html.Append("</pre></code></td></tr></table><br />");

            // Add the version information for all the necessary products
            html.Append("<hr width=\"100%\" size=\"1\" color=\"silver\" />");
            html.Append("<b style=\"margin-top: -5px;\">Version Information:</b>&nbsp;");

            try
            {
                html.AppendFormat("Microsoft .NET Framework Version: {0}; ", GetVersionInfo(typeof(object)));
                html.AppendFormat("ASP.NET Version: {0}; ", GetVersionInfo(typeof(Control)));
                if (TestDriverPage.SystemWebExtensionsAbstractions.SystemWebExtensionsAssembly != null)
                {
                    Type extenderControl = TestDriverPage.SystemWebExtensionsAbstractions.SystemWebExtensionsAssembly.GetType("System.Web.UI.ExtenderControl");
                    if (extenderControl != null)
                        html.AppendFormat("ASP.NET AJAX Version: {0}; ", GetVersionInfo(extenderControl));
                }
                html.AppendFormat("LTAF Version: {0}", GetVersionInfo(typeof(WebTestMethodAttribute)));
            }
            catch (System.Security.SecurityException)
            {
                // If running under medium trust, LTAF won't have FileIO permissions to get the versioninfo.
            }

            // Add the footer
            html.Append("</font></div>");

            return html.ToString();
        }

        /// <summary>
        /// Get Html-formatted source code corresponding to the invocation of an exception
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="fileName">Name of the file containing the source code</param>
        /// <param name="line">line</param>
        /// <param name="methodName">Name of the method containing the source code</param>
        /// <returns>Html-formatted source code</returns>
        private static string GetSourceCode(Exception exception, out string fileName, out int line, out string methodName)
        {
            // Assign default values for the output parameters
            fileName = null;
            line = 0;
            methodName = null;

            if (exception == null)
                return null;

            // String builder to contain the source code
            StringBuilder source = new StringBuilder();

            // Find the source code by looking through the stack trace for any
            // code contained in a file on the local system
            Regex parseStackTrace = new Regex(@"(at (.+) in (.+)\:line (\d+))+", RegexOptions.Multiline);
            MatchCollection matches = parseStackTrace.Matches(exception.StackTrace);
            if (matches.Count <= 0)
                return null;
            
            // Find the first match that isn't from one of our asserts
            Match sourceMatch = null;

            foreach (Match match in matches)
            {
                string method = match.Groups[2].Value;
                if (!string.IsNullOrEmpty(method) && !method.StartsWith(typeof(Assert).Namespace))
                {
                    sourceMatch = match;
                    break;
                }
            }
            if (sourceMatch == null)
                return null;

            // Get the information about the file with the source
            fileName = sourceMatch.Groups[3].Value;
            line = int.Parse(sourceMatch.Groups[4].Value);
            methodName = sourceMatch.Groups[2].Value;

            // Make sure we can find the source code
            if (!File.Exists(fileName))
                return null;

            try
            {
                // Get the lines of the file
                string[] lines = File.ReadAllLines(fileName);

                // Add the five lines of source code centered on the line in question
                for (int i = line - 2; i <= line + 2; i++)
                {
                    int lineOffset = i - 1;
                    bool highlight = i == line;

                    if (highlight)
                        source.Append("<font color=\"red\">");

                    if (lineOffset >= 0 && lineOffset < lines.Length)
                        source.AppendFormat("Line {0}: {1}", i, HttpUtility.HtmlEncode(lines[lineOffset]));

                    if (highlight)
                        source.Append("</font>");

                    source.AppendLine();
                }

                // Return the formatted source code
                return source.ToString();
            }
            catch (Exception)
            {
            }
            
            // If hit any exceptions (like not having permission to read
            // the file, etc.) indicate that we couldn't load the source code
            return null;
        }

        /// <summary>
        /// Get the version information from a type's assembly
        /// </summary>
        /// <param name="type">Type in the assembly</param>
        /// <returns>Version information</returns>
        private string GetVersionInfo(Type type)
        {
            try
            {
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(type.Module.FullyQualifiedName);
                return string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}",
                    versionInfo.FileMajorPart, versionInfo.FileMinorPart,
                    versionInfo.FileBuildPart, versionInfo.FilePrivatePart);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Called before a testcase is executed
        /// </summary>
        protected virtual void OnTestcaseExecuting(TestcaseExecutionEventArgs e)
        {
            if (TestcaseExecuting != null)
            {
                TestcaseExecuting(this, e);
            }
        }

        /// <summary>
        /// Called after a testcase is executed
        /// </summary>
        protected virtual void OnTestcaseExecuted(TestcaseExecutionEventArgs e)
        {
            if (TestcaseExecuted != null)
            {
                TestcaseExecuted(this, e);
            }
        }

        /// <summary>
        /// Called when a test run is started
        /// </summary>
        protected virtual void OnTestRunStarted(EventArgs e)
        {
            if (TestRunStarted != null)
            {
                TestRunStarted(this, e);
            }
        }

        /// <summary>
        /// Called when a test run is finished
        /// </summary>
        protected virtual void OnTestRunFinished(TestRunFinishedEventArgs e)
        {
            if (TestRunFinished != null)
            {
                TestRunFinished(this, e);
            }
        }
    }
}