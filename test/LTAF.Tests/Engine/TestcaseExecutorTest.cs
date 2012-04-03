using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniTestAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using LTAF.Engine;
using System.Threading;

namespace LTAF.UnitTests.Engine
{
    [TestClass]
    public class TestcaseExecutorTest
    {
        private TestableTestcaseExecutor _executor;
        [TestMethod]
        public void VerifyEventsAreCodedCorrectly()
        {
            _executor = new TestableTestcaseExecutor();
            _executor.TestcaseExecuting += new EventHandler<TestcaseExecutionEventArgs>(executor_TestcaseExecuting);
            _executor.TestcaseExecuted += new EventHandler<TestcaseExecutionEventArgs>(executor_TestcaseExecuted);
            _executor.TestRunFinished += new EventHandler<TestRunFinishedEventArgs>(executor_TestRunFinished);
            _executor.TestRunStarted += new EventHandler(executor_TestRunStarted);

            _executor.RaiseRunFinishedEvent(new TestRunFinishedEventArgs(null, new Dictionary<string, string>() {{"TestLog", "Test Log" }}));
            _executor.RaiseRunStartedEvent(new EventArgs());
            _executor.RaiseTestcaseExecutedEvent(new TestcaseExecutionEventArgs("TestWebTestClassName", "TestWebTestMethodName", true));
            _executor.RaiseTestcaseExecutingEvent(new TestcaseExecutionEventArgs("TestWebTestClassName", "TestWebTestMethodName", true));
        }

        void executor_TestRunStarted(object sender, EventArgs e)
        {
            UniTestAssert.AreSame(_executor, sender);
        }

        void executor_TestRunFinished(object sender, TestRunFinishedEventArgs e)
        {
            UniTestAssert.AreSame(_executor, sender);
            UniTestAssert.AreEqual("Test Log", e.TestLog);
        }

        void executor_TestcaseExecuted(object sender, TestcaseExecutionEventArgs e)
        {
            UniTestAssert.AreSame(_executor, sender);
            UniTestAssert.AreEqual("TestWebTestClassName.TestWebTestMethodName", e.WebTestName);
        }

        void executor_TestcaseExecuting(object sender, TestcaseExecutionEventArgs e)
        {
            UniTestAssert.AreSame(_executor, sender);
            UniTestAssert.AreEqual("TestWebTestClassName.TestWebTestMethodName", e.WebTestName);
        }
    }
}
