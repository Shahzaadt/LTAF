using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniTestAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using LTAF.Engine;
using System.Threading;
using LTAF.UnitTests.Mock;

namespace LTAF.UnitTests.Engine
{
    [TestClass]
    public class TestcaseLoggerTest
    {
        [TestMethod]
        public void WriteStartupFile()
        {
            MockFileSystem fileSystem = new MockFileSystem();
            TestcaseLogger logger = new TestcaseLogger("D:\\foo", new TestcaseExecutor(), fileSystem);
            logger.WriteStartupFile();
            UniTestAssert.IsTrue(fileSystem.Files.ContainsKey(@"D:\foo\Startup.txt"));
        }

        [TestMethod]
        public void VerifyLogAllPassed()
        {
            MockFileSystem fileSystem = new MockFileSystem();
            TestcaseLogger logger = new TestcaseLogger("D:\\foo", new TestcaseExecutor(), fileSystem);

            logger.LogTestcaseExecuted(null, new TestcaseExecutionEventArgs("TestClass1", "TestMethod1", true));
            logger.LogTestcaseExecuted(null, new TestcaseExecutionEventArgs("TestClass1", "TestMethod2", true));
            logger.LogTestRunFinished(null, new TestRunFinishedEventArgs());

            UniTestAssert.IsTrue(fileSystem.Files.ContainsKey(@"D:\foo\TestLog.txt"));
            string logContent = fileSystem.Files[@"D:\foo\TestLog.txt"];
            UniTestAssert.IsTrue(logContent.Contains("Testcase PASSED 'TestClass1.TestMethod1'"));
            UniTestAssert.IsTrue(logContent.Contains("Testcase PASSED 'TestClass1.TestMethod2'"));
            UniTestAssert.IsTrue(logContent.Contains("Test Run Finished."));

        }

        [TestMethod]
        public void VerifyLog1Passed1Fail()
        {
            MockFileSystem fileSystem = new MockFileSystem();
            TestcaseLogger logger = new TestcaseLogger("D:\\foo", new TestcaseExecutor(), fileSystem);

            logger.LogTestcaseExecuted(null, new TestcaseExecutionEventArgs("TestClass1", "TestMethod1", true));
            logger.LogTestcaseExecuted(null, new TestcaseExecutionEventArgs("TestClass1", "TestMethod2", false) 
                {  Exception = new Exception("Dummmy Error")});
            logger.LogTestRunFinished(null, new TestRunFinishedEventArgs());

            UniTestAssert.IsTrue(fileSystem.Files.ContainsKey(@"D:\foo\TestLog.txt"));
            string logContent = fileSystem.Files[@"D:\foo\TestLog.txt"];
            UniTestAssert.IsTrue(logContent.Contains("Testcase PASSED 'TestClass1.TestMethod1'"), logContent);
            UniTestAssert.IsTrue(logContent.Contains("Testcase FAILED 'TestClass1.TestMethod2'"), logContent);
            UniTestAssert.IsTrue(logContent.Contains("Test Run Finished."), logContent);

        }
    }
}
