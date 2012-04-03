using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Security.AccessControl;

namespace LTAF.Runner
{
    public class LTAFRunner
    {
        private const string LTAF_WEBSITE_PASSED = "Testcase PASSED";
        private const string LTAF_WEBSITE_FAILED = "Testcase FAILED";
        private const string LTAF_TESTLOGNAME = "TestLog.txt";
        private const string LTAF_STARTUPFILENAME = "Startup.txt";

        internal IFileSystem FileSystem
        {
            get;
            set;
        }

        internal IWebServer WebServer
        {
            get;
            set;
        }

        internal IOperatingSystem OperatingSystem
        {
            get;
            set;
        }

        internal int PollForLogInMilliseconds
        {
            get;
            set;
        }

        internal int StartupTimeoutInMilliseconds
        {
            get;
            set;
        }

        internal bool WaitForStartup
        {
            get;
            set;
        }

        internal bool WaitForLog
        {
            get;
            set;
        }

        public LTAFRunner()
            : this(new FileSystem(), new OperatingSystem(), new WebServer())
        {
        }

        internal LTAFRunner(IFileSystem fileSystem, IOperatingSystem operatingSystem, IWebServer webServer)
        {
            this.WebServer = webServer;
            this.OperatingSystem = operatingSystem;
            this.FileSystem = fileSystem;
            this.WaitForStartup = true;
            this.WaitForLog = true;
            this.StartupTimeoutInMilliseconds = 60000; // 1 mins
        }

        static int Main(string[] args)
        {
            LTAFRunner runner = new LTAFRunner();
            return runner.Run(args);  
        }

        public int Run(string[] arguments)
        {
            RunParameters p = new RunParameters();
            p.Parse(arguments);

            return Run(p);
        }

        public int Run(RunParameters args)
        {
            if (args.PrintHelp)
            {
                this.PrintHelp();
                return 0;
            }

            WebSiteResult result = null;

            try
            {
                result = RunWebSite(args);
            }
            catch (Exception e)
            {
                WriteLineToConsole("Error: " + e.ToString(), ConsoleColor.Red);
            }

            if (result == null)
            {
                return 1;
            }
            else
            {
                WriteLog(result);

                if (result.Passed)
                {
                    WriteLineToConsole(String.Format("Website '{0}' passed!", result.WebSiteName), ConsoleColor.Green);
                    return 0;
                }
                else
                {
                    WriteLineToConsole(String.Format("Website '{0}' failed!", result.WebSiteName), ConsoleColor.Red);
                    return 1;
                }
            }
        }

        private WebSiteResult RunWebSite(RunParameters args)
        {
            if (!Path.IsPathRooted(args.WebSitePath))
            {
                args.WebSitePath = Path.Combine(FileSystem.CurrentDirectory, args.WebSitePath);
            }

            if (!FileSystem.DirectoryExists(args.WebSitePath))
            {
                return new WebSiteResult() { 
                    Passed = false, 
                    Log = String.Format("Path to website '{0}' not found.", args.WebSitePath)
                };
            }

            string webSiteName = Path.GetFileName(args.WebSitePath);
            WriteLineToConsole(String.Format("Creating virtual directory for '{0}' website...", webSiteName));
            WebServer.CreateVirtualDirectory(webSiteName, args.WebSitePath, "localhost");
            

            FileSystem.AddDirectoryAccessRule(args.WebSitePath, 
                new FileSystemAccessRule("NETWORK SERVICE", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.InheritOnly, AccessControlType.Allow));

            WriteLineToConsole(String.Format("Executing tests for '{0}' website...", webSiteName));
            string vdirUrl = "http://localhost/" + webSiteName;

            WebSiteResult webResult = this.RunLTAFAndWait(args, vdirUrl);
            webResult.WebSiteName = webSiteName;

            return webResult;
        }

        private WebSiteResult RunLTAFAndWait(RunParameters args, string vdirUrl)
        {
            string startupPath = Path.Combine(args.WebSitePath, @"Test\" + LTAF_STARTUPFILENAME);
            string logPath = Path.Combine(args.WebSitePath, @"Test\" + LTAF_TESTLOGNAME);
            if (FileSystem.FileExists(startupPath))
            {
                FileSystem.DeleteFile(startupPath);
            }
            if (FileSystem.FileExists(logPath))
            {
                FileSystem.DeleteFile(logPath);
            }

            WebSiteResult result = new WebSiteResult();

            StringBuilder url = new StringBuilder(vdirUrl.TrimEnd(new char[1] { '/' }) + "/Test/Default.aspx?");

			// append tag name
            if (!String.IsNullOrEmpty(args.TagName))
            {
                url.Append(String.Format("tag={0}&filter=true&", args.TagName));
            }

			//append auto run and auto log
            url.Append("run=true&log=true&console=false");

	
            args.Browser.Open(OperatingSystem, url.ToString());

            string content = String.Empty;
            if (WaitForStartup && !WaitForFile(startupPath, StartupTimeoutInMilliseconds, PollForLogInMilliseconds))
            {
                result.Passed = false;
                result.Log = String.Format("Timed out waiting for browser to start up. Time out: {0} ms.", StartupTimeoutInMilliseconds);
            }
            else if (WaitForLog && !WaitForFile(logPath, args.ExecutionTimeout, PollForLogInMilliseconds, out content))
            {
                result.Passed = false;
                result.Log = String.Format("Execution timed out. Time out: {0} ms.", args.ExecutionTimeout);
            }
            else
            {
                result.Log = content;
                result.Passed = false;

                if (result.Log.Contains(LTAF_WEBSITE_FAILED))
                {
                    //if any test failed, then the whole suite failed.
                    result.Passed = false;
                }
                else if (result.Log.Contains(LTAF_WEBSITE_PASSED))
                {
                    //at least must have one testcase
                    result.Passed = true;
                }
            }

            args.Browser.Close(OperatingSystem);

            return result;
        }

        private bool WaitForFile(string path, int timeoutInMilliseconds, int pollInMilliseconds)
        {
            DateTime timeout = DateTime.Now.AddMilliseconds(timeoutInMilliseconds);

            while (!FileSystem.FileExists(path) && DateTime.Now < timeout)
            {
                Thread.Sleep(pollInMilliseconds);
            }

            if (!FileSystem.FileExists(path))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool WaitForFile(string path, int timeoutInMilliseconds, int pollInMilliseconds, out string content)
        {
            bool waitSuccess = WaitForFile(path, timeoutInMilliseconds, pollInMilliseconds);
            if (waitSuccess)
            {
                try
                {
                    content = FileSystem.ReadFile(path);
                }
                catch(IOException)
                {
                    //Attempt again, LTAF might still be writting the log
                    System.Threading.Thread.Sleep(5000);
                    content = FileSystem.ReadFile(path);
                }
            }
            else
            {
                content = null;
            }
            return waitSuccess;
        }

        private void WriteLog(WebSiteResult result)
        {
            StringBuilder logBuilder = new StringBuilder();

            if (result.Passed)
            {
                logBuilder.AppendLine("Test Run PASSED");
            }
            else
            {
                logBuilder.AppendLine("Test Run FAILED");
            }

            logBuilder.Append(result.Log);

            FileSystem.WriteFile(Path.Combine(FileSystem.CurrentDirectory, LTAF_TESTLOGNAME), logBuilder.ToString());
        }

        private void WriteLineToConsole(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            WriteLineToConsole(message);
            Console.ResetColor();
        }

        private void WriteLineToConsole(string message)
        {
            Console.WriteLine(message);
        }

        private void PrintHelp()
        {
            Console.WriteLine("Utility to run all LTAF tests");
            Console.WriteLine("");
            Console.WriteLine("LTAFRunner.exe [/?] [/p:path] [/b:browser] [/t:tagName] [timeout:<minutes>] [/silent]");
            Console.WriteLine("");
            Console.WriteLine("/?\t\tPrint this help text.");
            Console.WriteLine("/path\t\tThe physical path that contains the test website.");
            Console.WriteLine("/browser\tThe browser to run the tests with: 'ie' (default), 'ff'.");
            Console.WriteLine("/tag\t\tThe WebTestTag to filter tests to run within the website.");
            Console.WriteLine("/timeout\tTimeout in minutes (defaults to 30)");
        }
    }
}