using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LTAF.Runner;
using System.Collections;
using System.Threading;
using System.Security.AccessControl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTAF.Runner.Tests
{
    [TestClass]
    public class LTAFRunnerTest
    {
        MockOperatingSystem _os;
        MockFileSystem _fileSystem;
        MockWebServer _webSever;

        #region Helper Methods
        private LTAFRunner CreateRunner()
        {
            _os = new MockOperatingSystem();
            _fileSystem = new MockFileSystem();
            _webSever = new MockWebServer();

            _fileSystem = new MockFileSystem();
            _fileSystem.Directories.Add(@"D:\LTAF\WebSite");
            _fileSystem.Directories.Add(@"D:\LTAF\WebSite\Test");

            LTAFRunner runner = new LTAFRunner(_fileSystem, _os, _webSever);


            return runner;
        }

        public class ExecuteTestsTaskInfo
        {
            public MockFileSystem FileSystem;
            public string PathToWebApp;
            public int Milliseconds;
            public string LogContent;
        }

        private void SimulateWebSiteTests(LTAFRunner runner, string pathToWebApp, MockFileSystem fileSystem, string logContent)
        {
            runner.PollForLogInMilliseconds = 5;
            runner.WaitForStartup = false;

            ExecuteTestsTaskInfo taskInfo = new ExecuteTestsTaskInfo()
            {
                FileSystem = fileSystem,
                Milliseconds = 100,
                PathToWebApp = pathToWebApp,
                LogContent = logContent
            };
            Thread t = new Thread(WriteLogAfterMilliseconds);
            t.Start(taskInfo);
        }

        private void WriteLogAfterMilliseconds(object data)
        {
            ExecuteTestsTaskInfo taskInfo = (ExecuteTestsTaskInfo)data;

            Thread.Sleep(taskInfo.Milliseconds);

            //Need a lock
            taskInfo.FileSystem.Files.Add(Path.Combine(taskInfo.PathToWebApp, @"Test\Startup.txt"), "dummy startup");
            taskInfo.FileSystem.Files.Add(Path.Combine(taskInfo.PathToWebApp, @"Test\TestLog.txt"), taskInfo.LogContent);
        }
        #endregion

        [TestMethod]
        public void RelativePath()
        {
            //Arrange
            LTAFRunner runner = CreateRunner();
            _fileSystem.CurrentDirectory = @"D:\MyCompany\Product";
            _fileSystem.Directories.Add(@"D:\MyCompany\Product\WebSite");
            _fileSystem.Directories.Add(@"D:\MyCompany\Product\WebSite\Test");
            SimulateWebSiteTests(runner, @"D:\MyCompany\Product\WebSite", _fileSystem, "Testcase PASSED");

            //ACt
            int code = runner.Run(new RunParameters()
            {
                WebSitePath = "WebSite",
                ExecutionTimeout = 1000
            });

            //Assert
            string logContent = _fileSystem.Files.Where(k => k.Key.Contains("Product\\TestLog.txt")).First().Value;
            Assert.IsTrue(logContent.Contains("Test Run PASSED"), logContent);
        }

        [TestMethod]
        public void KillProcess()
        {
            //Arrange
            LTAFRunner runner = CreateRunner();

            SimulateWebSiteTests(runner, @"D:\LTAF\WebSite", _fileSystem, "log content");


            //Act
            runner.Run(new RunParameters()
            {
                WebSitePath = @"D:\LTAF\WebSite",
                TagName = "foobar"
            });


            //Assert
            Assert.AreEqual("iexplore", _os.KillProcessName);
        }

        [TestMethod]
        public void CreateProcessWithTag()
        {
            //Arrange
            LTAFRunner runner = CreateRunner();

            SimulateWebSiteTests(runner, @"D:\LTAF\WebSite", _fileSystem, "log content");


            //Act
            runner.Run(new RunParameters()
            {
                WebSitePath = @"D:\LTAF\WebSite",
                TagName = "foobar"
            });


            //Assert
            Assert.AreEqual("http://localhost/WebSite/Test/Default.aspx?tag=foobar&filter=true&run=true&log=true&console=false", _os.CreateProcessArguments);
            Assert.AreEqual("iexplore.exe", _os.CreateProcessName);
        }

        [TestMethod]
        public void CreateProcess()
        {
            //Arrange
            LTAFRunner runner = CreateRunner();

            SimulateWebSiteTests(runner, @"D:\LTAF\WebSite", _fileSystem, "log content");


            //Act
            runner.Run(new RunParameters()
            {
                WebSitePath = @"D:\LTAF\WebSite",
            });


            //Assert
            Assert.AreEqual("http://localhost/WebSite/Test/Default.aspx?run=true&log=true&console=false", _os.CreateProcessArguments);
            Assert.AreEqual("iexplore.exe", _os.CreateProcessName);
        }

        [TestMethod]
        public void DeleteLogBeforeRunning()
        {
            //Arrange
            LTAFRunner runner = CreateRunner();
            _fileSystem.Files.Add(@"D:\LTAF\WebSite\Test\TestLog.txt", "foo");

            SimulateWebSiteTests(runner, @"D:\LTAF\WebSite", _fileSystem, "log content");


            //Act
            runner.Run(new RunParameters()
            {
                WebSitePath = @"D:\LTAF\WebSite",
            });

            //Assert
            string logContent = _fileSystem.Files.Where(k => k.Key.Contains("LTAFRunner\\TestLog.txt")).First().Value;
            Assert.IsTrue(logContent.Contains("log content"), logContent);
        }

        [TestMethod]
        public void DeleteStartupBeforeRunning()
        {
            //Arrange
            LTAFRunner runner = CreateRunner();
            _fileSystem.Files.Add(@"D:\LTAF\WebSite\Test\Startup.txt", "foo");

            SimulateWebSiteTests(runner, @"D:\LTAF\WebSite", _fileSystem, "log content");


            //Act
            runner.Run(new RunParameters()
            {
                WebSitePath = @"D:\LTAF\WebSite",
            });

            //Assert
            string logContent = _fileSystem.Files.Where(k => k.Key.Contains("Test\\Startup.txt")).First().Value;
            Assert.IsTrue(logContent.Contains("dummy startup"), logContent);
        }

        [TestMethod]
        public void TimeoutIfStartupIsNotProduced()
        {
            //Arrange
            LTAFRunner runner = CreateRunner();
            runner.PollForLogInMilliseconds = 0;
            runner.StartupTimeoutInMilliseconds = 5;
            runner.WaitForStartup = true;

            //Act
            int code = runner.Run(new RunParameters()
            {
                WebSitePath = @"D:\LTAF\WebSite",
            });

            //Assert
            Assert.AreEqual(1, code);  
            string logContent = _fileSystem.Files.Where(k => k.Key.Contains("LTAFRunner\\TestLog.txt")).First().Value;
            Assert.IsTrue(logContent.Contains("Timed out waiting for browser to start up."), logContent);
        }

        [TestMethod]
        public void WebSiteFail()
        {
            //Arrange
            LTAFRunner runner = CreateRunner();

            SimulateWebSiteTests(runner, @"D:\LTAF\WebSite", _fileSystem, "Testcase FAILED");

            //ACt
            int code = runner.Run(new RunParameters()
            {
                WebSitePath = @"D:\LTAF\WebSite",
                ExecutionTimeout = 1000
            });

            //Assert
            Assert.AreEqual(1, code);
            string logContent = _fileSystem.Files.Where(k => k.Key.Contains("LTAFRunner\\TestLog.txt")).First().Value;
            Assert.IsTrue(logContent.Contains("Test Run FAILED"), logContent);
            Assert.IsTrue(logContent.Contains("Testcase FAILED"), logContent);   
        }

        [TestMethod]
        public void WebSitePass()
        {
            //Arrange
            LTAFRunner runner = CreateRunner();
         
            SimulateWebSiteTests(runner, @"D:\LTAF\WebSite", _fileSystem, "Testcase PASSED");

            //ACt
            int code = runner.Run(new RunParameters()
                {
				    WebSitePath = @"D:\LTAF\WebSite",
                    ExecutionTimeout=1000
                });

            //Assert
            Assert.AreEqual(0, code);
            string logContent = _fileSystem.Files.Where(k => k.Key.Contains("LTAFRunner\\TestLog.txt")).First().Value;
            Assert.IsTrue(logContent.Contains("Test Run PASSED"), logContent);
            Assert.IsTrue(logContent.Contains("Testcase PASSED"), logContent);
        }

        [TestMethod]
        public void CreateVDirForWebSite()
        {
            //Arrange
            LTAFRunner runner = CreateRunner();
            runner.WaitForLog = false;
            runner.WaitForStartup = false;

            //Act
            int code = runner.Run(new RunParameters() { WebSitePath = @"D:\LTAF\WebSite" });

            //Assert
            Assert.AreEqual("WebSite", _webSever.AppNames[0]);
            Assert.AreEqual("localhost", _webSever.ServerNames[0]);
            Assert.AreEqual(@"D:\LTAF\WebSite", _webSever.AppDirectories[0]);

        }

        [TestMethod]
        public void WebSiteNotFound()
        {
            //Arrange
            LTAFRunner runner = CreateRunner();
            runner.WaitForLog = false;
            runner.WaitForStartup = false;

            //Act
            int code = runner.Run(new RunParameters() { WebSitePath = @"D:\LTAF\Foo" });

            //Assert
            Assert.AreEqual(1, code);
            string logContent = _fileSystem.Files.Where(k => k.Key.Contains("LTAFRunner\\TestLog.txt")).First().Value;
            Assert.IsTrue(logContent.Contains("Test Run FAILED"), logContent);
            Assert.IsTrue(logContent.Contains("Path to website 'D:\\LTAF\\Foo' not found."), logContent);  
        }

        [TestMethod]
        public void SetACLForWebSite()
        {
            //Arrange
            LTAFRunner runner = CreateRunner();
            runner.WaitForLog = false;
            runner.WaitForStartup = false;

            //ACt
            runner.Run(new RunParameters() { WebSitePath = @"D:\LTAF\WebSite" });

            //Assert
            Assert.AreEqual(1, _fileSystem.DirectoryAccessRules.Count);
            Assert.IsTrue(_fileSystem.DirectoryAccessRules.ContainsKey(@"D:\LTAF\WebSite"));
        }
    }
}
